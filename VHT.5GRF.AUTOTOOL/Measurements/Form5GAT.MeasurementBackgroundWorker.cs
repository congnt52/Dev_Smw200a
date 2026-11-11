using _5GAutoTool.Reports;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Vml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace _5GAutoTool
{
    public partial class Form5GAT
    {
        #region measurementBackgroundWorker_DoWork
        public TestcaseNode CurrentTestcase = new TestcaseNode();
        private ReportManager _reportManager = new ReportManager();
        private ReportEngine _reportEngine = new ReportEngine(@"C:\Reports");
        private CsvSession _csvSession = new CsvSession();
        private string _excelTemplatePath = @"C:\test3gpp\OQC\32T32R10W\templates\Phieudokiem.xlsx";


        //====================================
        // HÀM ĐO CHÍNH
        //====================================
        public void measurementBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            // TODO: Tạo mới report manager khi bắt đầu đo - 07.06.2026
            _reportManager.Clear();

            ProgressLabel($"Staring Measurement Background Worker...");
            // in put
            string PassFail = fail;
            bool isTestcasePass = true;
            string[] Result = new string[5];
            double freqHz = double.Parse(setFrequency);
            double freqMHz = freqHz / 1e6;
            double[] dbRS = new double[20];
            double?[] tseRS = new double?[20];
            double limitMin = 0;
            double limitMax = 0;
            string result = "";
            double tp = 0;
            double RS = 0;
            double? nRS;
            string mode = "";
            string NRTM = "";
            string shortName = "";
            int rruPower = int.Parse(setPower);
            double attSetVal = 0;


            //process variable
            bool reTest = false;
            TestcaseNode testcaseInTesting;
            int completedTask = 0;
            bool interEVM20 = false; //use for testcase EVM & FreqErr TM2.0
            bool interEVM20a = false; //use for testcase EVM & FreqErr TM2.0a
            bool interEVM31 = false; //use for testcase EVM & FreqErr TM3.1
            bool interEVM31a = false; //use for testcase EVM & FreqErr TM3.1a
            bool interEVM32 = false; //use for testcase EVM & FreqErr TM3.2
            bool interEVM33 = false; //use for testcase EVM & FreqErr TM3.3



            // out put
            string[] transPeriod = new string[32];
            string aclrAdjLower = "";
            string aclrAdjUpper = "";
            string aclrAlt1Lower = "";
            string aclrAlt1Upper = "";
            string[] evm = new string[13];
            string[,] freqError = new string[32, 6];
            string[] taeMeas = new string[32];
            string[] tse = new string[9];
            double[] imdACLR = new double[6];
            double[] imdSEM = new double[6];
            double?[,] TSE = new double?[20, 20];
            double[] intraACLR = new double[4];
            double[] intraSEM = new double[4];
            string[,] OFDM = new string[32, 6];
            string[,] Transient = new string[32, 6];
            string[] noise = new string[32];
            int loopTime = 3;
            int timeout = 0;

            List<string> failedTestcases = new List<string>();  //create a List to store Failed Testcase

            // Store failed test results
            Dictionary<string, List<int>> failedTestList = new Dictionary<string, List<int>>();


            OOBexcludeColocationRange = chckExcludeCoLoRange.Checked; //27.3.2024 - OOB option - include/exclude Co-location Bands
            deltaFoobValue = double.Parse(deltaFoob.Text) * 1e6; //in Hz

            // Set frequency for RRU
            Log($"Set frequency to RRU: {freqMHz} MHz");
            //set tan so cho thiet bi
            BBU.CHG_Freq(setFrequency);

            //BBU.TDDMODE("CALIB");  //kiem tra lai xem co dung khong, khong thi xoa

            //TODO: Assign Role
            ProgressLabel($"Assigning Devices...");
            VSG1.Name = "Wanted Signal";
            VSG2.Name = "Interference Signal";
            VSG3.Name = "CW Signal";
            rfSwitch1.Name = "RFS-Master";
            rfSwitch2.Name = "RFS-D1";
            rfSwitch3.Name = "RFS-D2";

            //10.1.2024 - clear Chart
            //chart1.Series.Clear(); //tam thoi comment lai
            enRecordLog = true; //start writing log to file

            //TODO: bắt đầu report - 07.06.2026
            _reportManager.BeginReport(RruSerial, RruType, freqMHz.ToString() + " MHz", setBandwidth + " MHz", txtPower.Text + " dBm", testCase);
            string reportFolder = _reportEngine.CreateReportFolder(_reportManager.Report.Summary.SerialNumber, _reportManager.Report.Summary.StartTime);

            ProgressLabel($"Measurement Start!");
            LogDashedLine($"=============== MEASUREMENT START: {DateTime.Now.ToString("f")} ===============");
            
            //TODO: ========= 18.9.2024 - Background worker define new loop ===========
            TestcasePending = GetTestcasesByStatus(TestcaseNodes, TestcaseStatus.Pending);
            while (TestcasePending.Count > 0)
            {
                if (measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                //test: Pending List to Log
                LogDashedLine("--------- Remaining Testcases in Testing ----------------");
                foreach (TestcaseNode node in TestcasePending)
                {
                    Log($"+ {node.Text}");
                }
                LogDashedLine("-------------------------");

                testcaseInTesting = TestcasePending[0];
                CurrentTestcase = testcaseInTesting;
                if (testcaseInTesting != null)
                {
                    if (testingMode == "TX")
                    {
                        BBU.CHG_RRU_POWER(rruPower);
                    }
                    if (testingMode == "RX")
                    {
                       // BBU.TDDMODE("RX_ONLY");
                    }
                    //change node to Run Status
                    testcaseInTesting.Status = TestcaseStatus.Running;
                    ChangeTestcaseNodeStatus(testcaseInTesting.Text, TestcaseStatus.Running);
                    //UpdateNodeStatusToTreeView(testcaseInTesting, trvTestCases.Nodes[0]);

                    //create a List to store Fail Ports
                    List<int> failedPorts = new List<int>();

                    //create Testing port list after each Testcase
                    ProgressLabel($"Collecting List of Testing Port...");
                    PopulateTestingPortList();
                    //Re-arrange Testing port list by MeasMode
                    string strMeasMode = ReadValueByHeader(testcaseInTesting.Parameters, "MultiMeas");
                    bool isMultiMeas = (strMeasMode.ToUpper() == "TRUE") ? true : false;
                    if (testingMode == "RX" && isMultiMeas)
                    {
                        //re-arrange testingPortList in groups
                        ReArrangeTestingPortList(PortMappings);
                    }
                    Log($"Testing Ports List: {string.Join(", ", testingPortList)}");

                    //Clear data MappingPort List
                    ProgressLabel($"Clearing List of Testing Port...");
                    PortListClearData(PortMappings);

                    //Khai báo report testcase result
                    var headers = GetHeadersFromTestCaseName(testcaseInTesting.Text);

                    _reportManager.BeginTestCase(testcaseInTesting.Text, headers);

                    //string reportFolder = _reportEngine.CreateReportFolder(_reportManager.Report.Summary.SerialNumber, _reportManager.Report.Summary.StartTime);

                    _reportManager.Report.ReportFolder = reportFolder;

                    _csvSession = _reportEngine.BeginCsvTestCase(reportFolder, _reportManager.Report.Summary.SerialNumber, testcaseInTesting.Text, headers);
                    
                    //TODO: run sequence testing port in list
                    foreach (int testingPort in testingPortList)
                    {
                        if (measurementBackgroundWorker.CancellationPending) return;
                        System.Threading.Thread.Sleep(1);

                        //khai báo dữ liệu report cho từng port
                        TestRow portResult = new TestRow();
                        DateTime startTime = DateTime.Now;

                        while (timeout < loopTime)
                        {
                            portResult.Clear();

                            ProgressLabel($"Measuring Port:{testingPort}");
                            reTest = false;                            
                            #region TXcalib
                            //if (testcaseInTesting.Text.Contains("TX Calibration"))
                            //{
                            //    mode = "NRTM_11";
                            //
                            //    DataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode);
                            //    double.TryParse(measClause[i, 3], out limitMax);
                            //    double.TryParse(measClause[i, 2], out limitMin);
                            //    string gainDefault = measClause[i, 5];
                            //    string powerExpected = txtPower.Text;
                            //    limitMax = double.Parse(powerExpected) + 0.5;
                            //    limitMin = double.Parse(powerExpected) - 0.5;
                            //    Measurement.calibTX(VSA, BBU, RRU, "1", (j + 1).ToString(), mode, arrAttDL[j], setFrequency, gainDefault, powerExpected, out Result[0], out Result[1], out Result[2]);
                            //    bool isSuccess = double.TryParse(Result[0], out RS);
                            //    string gain;
                            //    BBU.SHW_TX_GAIN((j + 1).ToString(), out gain);
                            //    if (isSuccess)
                            //    {
                            //        RS = Math.Round(RS, 1);
                            //        if ((RS < limitMin || RS > limitMax))
                            //        {
                            //            PassFail = fail;
                            //            reTest = true;
                            //        }
                            //        else
                            //        {
                            //            PassFail = pass;
                            //        }
                            //        result = RS.ToString();
                            //    }
                            //    else
                            //    {
                            //        result = error;
                            //        PassFail = fail;
                            //        reTest = true;
                            //        BBU.PREPARE_CALIB_RRU((j + 1).ToString(), gainDefault);
                            //    }
                            //    BBU.CHG_RRU_POWER(1000);
                            //    Log("TRX" + (j + 1) + " Power : vsa reading " + RS + "dBm"); //+ " rru reading " + antPower + "dBm"
                            //    dataCalibTx[j, 0] = measClause[i, 2]; // 0 Min limit, 1 Reading Value, 2 Max limit, 3 Testing Time, 4 Result. 
                            //    dataCalibTx[j, 1] = result;     // 1 Reading Value
                            //    dataCalibTx[j, 2] = measClause[i, 3]; // 2 Max limit
                            //    dataCalibTx[j, 3] = DateTime.Now.ToString("hh:mm:ss");
                            //    dataCalibTx[j, 4] = PassFail;
                            //}
                            #endregion
                            #region Pout
                            //TODO: TX - Base station output power                            
                            if (testcaseInTesting.Text == "Base station output power")
                            {
                                mode = "NRTM_11";
                                shortName = "Pout";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.Output5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out Result[0]); //arrAttDL duoc update tu ham doc csv                                                                                                                                                        

                                if (double.TryParse(Result[0], out RS))
                                {
                                    RS = Math.Round(RS, 1);
                                    if (RS < limitMin || RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataPout[testingPort - 1, 0] = PassFail;  // 4 Result
                                dataPout[testingPort - 1, 1] = testTime;  // 3 Testing Time
                                dataPout[testingPort - 1, 2] = result;     // 1 Reading Value

                                // take screenshots and change to continous sweep
                                //if (PassFail == pass || timeout==(loopTime-1))
                                //{
                                //    //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency, PassFail);
                                //    //VSA.SingleSweepOnOff(1); //change to Continous Sweep
                                //}

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "dBm");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.2");
                                portResult.Values.Add("CONFIG", "NRTM1.1");
                                portResult.Values.Add("PARAM", "Pout");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);
                                
                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);

                            }
                            #endregion
                            #region OCB
                            //TODO: TX - Occupied bandwidth 
                            if (testcaseInTesting.Text == "Occupied bandwidth")
                            {
                                mode = "NRTM_11";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                double.TryParse(strLimMax, out limitMax);
                                limitMin = 0;

                                Measurement.OBW5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setBandwidth, out Result[0]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                double tmp;
                                bool isSuccess = double.TryParse(Result[0], out tmp);
                                if (isSuccess)
                                {
                                    RS = Math.Abs(tmp / 1e6);
                                    RS = Math.Round(RS, 1);
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }

                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataOBW[testingPort - 1, 0] = PassFail;  // 4 Result
                                dataOBW[testingPort - 1, 1] = testTime;  // 3 Testing Time
                                dataOBW[testingPort - 1, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "MHz");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.6.2");
                                portResult.Values.Add("CONFIG", "NRTM1.1");
                                portResult.Values.Add("PARAM", "OBW");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", "");
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region ACLR1.1
                            //TODO: TX - Adjacent channel leakage power TM1.1
                            if (testcaseInTesting.Text == "Adjacent channel leakage power TM1.1")
                            {
                                mode = "NRTM_11";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.ACLRPower5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out Result[1], out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);

                                if (double.TryParse(aclrAdjLower, out double tmp))
                                {
                                    double.TryParse(aclrAdjLower, out double tmp1);
                                    double.TryParse(aclrAdjUpper, out double tmp2);
                                    double.TryParse(aclrAlt1Lower, out double tmp3);
                                    double.TryParse(aclrAlt1Upper, out double tmp4);
                                    double tmp5 = Math.Max(tmp1, tmp2);
                                    double tmp6 = Math.Max(tmp3, tmp4);
                                    RS = Math.Abs(Math.Max(tmp5, tmp6));
                                    RS = Math.Round(RS, 1);

                                    if (RS < limitMin)
                                    {
                                        //BBU.CHG_RRU_POWER(int.Parse(setPower));
                                        PassFail = fail;
                                        reTest = true;
                                        if (timeout == loopTime - 1)
                                        {
                                            //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                        }
                                    }
                                    else
                                    {
                                        //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }

                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataACLR[testingPort - 1, 0, 0] = PassFail;  // 4 Result
                                dataACLR[testingPort - 1, 0, 1] = testTime;  // 3 Testing Time
                                dataACLR[testingPort - 1, 0, 2] = result;     // 1 Reading Value


                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "dBc");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.6.3");
                                portResult.Values.Add("CONFIG", "NRTM1.1");
                                portResult.Values.Add("PARAM", "ACLR");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region ACLR1.2
                            //TODO: TX - Adjacent channel leakage power TM1.2
                            if (testcaseInTesting.Text == "Adjacent channel leakage power TM1.2")
                            {
                                mode = "NRTM_12";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);


                                Measurement.ACLRPower5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out Result[1], out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);

                                if (double.TryParse(aclrAdjLower, out double tmp))
                                {
                                    double.TryParse(aclrAdjLower, out double tmp1);
                                    double.TryParse(aclrAdjUpper, out double tmp2);
                                    double.TryParse(aclrAlt1Lower, out double tmp3);
                                    double.TryParse(aclrAlt1Upper, out double tmp4);
                                    double tmp5 = Math.Max(tmp1, tmp2);
                                    double tmp6 = Math.Max(tmp3, tmp4);
                                    RS = Math.Abs(Math.Max(tmp5, tmp6));
                                    RS = Math.Round(RS, 1);

                                    if (RS < limitMin)
                                    {
                                        //BBU.CHG_RRU_POWER(int.Parse(setPower));
                                        PassFail = fail;
                                        reTest = true;
                                        if (timeout == loopTime - 1)
                                        {
                                            //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                        }
                                    }
                                    else
                                    {
                                        //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataACLR[testingPort - 1, 1, 0] = PassFail;  // 4 Result
                                dataACLR[testingPort - 1, 1, 1] = testTime;  // 3 Testing Time
                                dataACLR[testingPort - 1, 1, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "dBc");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.6.3");
                                portResult.Values.Add("CONFIG", "NRTM1.2");
                                portResult.Values.Add("PARAM", "ACLR");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);

                            }
                            #endregion
                            #region OBUE-1.1
                            //TODO: TX - Operating band Unwanted emissions TM1.1
                            if (testcaseInTesting.Text == "Operating band Unwanted emissions TM1.1")
                            {
                                mode = "NRTM_11";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.SEM5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out dbRS[1], out dbRS[2], out dbRS[3], out dbRS[0]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                RS = Math.Round(dbRS[0], 1);
                                if (RS > limitMax)
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                else
                                {
                                    ////TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
                                    PassFail = pass;

                                }
                                result = RS.ToString();
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataSEM[testingPort - 1, 0, 0] = PassFail;
                                dataSEM[testingPort - 1, 0, 1] = testTime;  // 3 Testing Time
                                dataSEM[testingPort - 1, 0, 2] = Math.Round(dbRS[1], 1).ToString();
                                dataSEM[testingPort - 1, 0, 3] = Math.Round(dbRS[2], 1).ToString();
                                dataSEM[testingPort - 1, 0, 4] = Math.Round(dbRS[3], 1).ToString();

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "dB");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.6.4");
                                portResult.Values.Add("CONFIG", "NRTM1.1");
                                portResult.Values.Add("PARAM", "ΔOBUE");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RANGE_1", dataSEM[testingPort - 1, 0, 2]);
                                portResult.Values.Add("RANGE_2", dataSEM[testingPort - 1, 0, 3]);
                                portResult.Values.Add("RANGE_3", dataSEM[testingPort - 1, 0, 4]);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region OBUE-1.2
                            //TODO: TX - Operating band Unwanted emissions TM1.2
                            if (testcaseInTesting.Text == "Operating band Unwanted emissions TM1.2")
                            {
                                mode = "NRTM_12";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.SEM5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out dbRS[1], out dbRS[2], out dbRS[3], out dbRS[0]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                RS = Math.Round(dbRS[0], 1);
                                if (RS > limitMax)
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                else
                                {
                                    ////TakeScreenshotVSA(VSA, "TX", measClause[i, 0], "P" + (j + 1), setFrequency);
                                    PassFail = pass;
                                    //update TestcaseNode status

                                }
                                result = RS.ToString();
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataSEM[testingPort - 1, 1, 0] = PassFail;
                                dataSEM[testingPort - 1, 1, 1] = testTime;  // 3 Testing Time
                                dataSEM[testingPort - 1, 1, 2] = Math.Round(dbRS[1], 1).ToString();
                                dataSEM[testingPort - 1, 1, 3] = Math.Round(dbRS[2], 1).ToString();
                                dataSEM[testingPort - 1, 1, 4] = Math.Round(dbRS[3], 1).ToString();

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "dB");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.6.4");
                                portResult.Values.Add("CONFIG", "NRTM1.2");
                                portResult.Values.Add("PARAM", "ΔOBUE");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RANGE_1", dataSEM[testingPort - 1, 0, 2]);
                                portResult.Values.Add("RANGE_2", dataSEM[testingPort - 1, 0, 3]);
                                portResult.Values.Add("RANGE_3", dataSEM[testingPort - 1, 0, 4]);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region EVM-2.0
                            //TODO: TX - Modulation quality TM2.0 
                            if (testcaseInTesting.Text == "Modulation quality TM2.0")
                            {
                                mode = "NRTM_20";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                dataFreqErr[testingPort - 1, 0, 2] = Result[0];

                                bool isSuccess = double.TryParse(evm[2], out RS); //evm[3] giá trị EVM 256QAM
                                if (isSuccess)
                                {
                                    RS = Math.Round(RS, 1);
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataEVM[testingPort - 1, 0, 0] = PassFail;  // 4 Result
                                dataEVM[testingPort - 1, 0, 1] = testTime;  // 3 Testing Time
                                dataEVM[testingPort - 1, 0, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "%");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.3");
                                portResult.Values.Add("CONFIG", "NRTM2.0");
                                portResult.Values.Add("PARAM", "64QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region EVM-2.0a
                            //TODO: TX - Modulation quality TM2.0a
                            if (testcaseInTesting.Text == "Modulation quality TM2.0a")
                            {
                                mode = "NRTM_20a";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                OFDM[testingPort - 1, 0] = evm[10]; // giá trị OFDM power 2.0a
                                dataFreqErr[testingPort - 1, 1, 2] = Result[0];

                                bool isSuccess = double.TryParse(evm[3], out RS); //evm[3] giá trị EVM 256QAM
                                if (isSuccess)
                                {
                                    RS = Math.Round(RS, 1);
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");

                                dataEVM[testingPort - 1, 1, 0] = PassFail;  // 4 Result
                                dataEVM[testingPort - 1, 1, 1] = testTime;  // 3 Testing Time
                                dataEVM[testingPort - 1, 1, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "%");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.3");
                                portResult.Values.Add("CONFIG", "NRTM2.0a");
                                portResult.Values.Add("PARAM", "256QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region EVM-3.1
                            //TODO: TX - Modulation quality TM3.1
                            if (testcaseInTesting.Text == "Modulation quality TM3.1")
                            {
                                mode = "NRTM_31";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                dataFreqErr[testingPort - 1, 2, 2] = Result[0];
                                bool isSuccess = double.TryParse(evm[2], out RS); //evm[3] giá trị EVM 64QAM
                                if (isSuccess)
                                {
                                    RS = Math.Round(RS, 1);
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");

                                dataEVM[testingPort - 1, 2, 0] = PassFail;  // 4 Result
                                dataEVM[testingPort - 1, 2, 1] = testTime;  // 3 Testing Time
                                dataEVM[testingPort - 1, 2, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "%");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.3");
                                portResult.Values.Add("CONFIG", "NRTM3.1");
                                portResult.Values.Add("PARAM", "64QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region EVM-3.1a
                            //TODO: TX - Modulation quality TM3.1a
                            if (testcaseInTesting.Text == "Modulation quality TM3.1a")
                            {
                                mode = "NRTM_31a";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                OFDM[testingPort - 1, 1] = evm[10]; // giá trị OFDM power 2.0a
                                dataFreqErr[testingPort - 1, 3, 2] = Result[0];
                                bool isSuccess = double.TryParse(evm[3], out RS); //evm[3] giá trị EVM 256QAM
                                if (isSuccess)
                                {
                                    RS = Math.Round(RS, 1);
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;
                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataEVM[testingPort - 1, 3, 0] = PassFail;  // 4 Result
                                dataEVM[testingPort - 1, 3, 1] = testTime;  // 3 Testing Time
                                dataEVM[testingPort - 1, 3, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "%");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.3");
                                portResult.Values.Add("CONFIG", "NRTM3.1a");
                                portResult.Values.Add("PARAM", "256QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);

                            }
                            #endregion
                            #region EVM-3.2
                            //TODO: TX - Modulation quality TM3.2
                            if (testcaseInTesting.Text == "Modulation quality TM3.2")
                            {
                                mode = "NRTM_32";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                dataFreqErr[testingPort - 1, 4, 2] = Result[0];
                                bool isSuccess = double.TryParse(evm[1], out RS); //giá trị EVM 16QAM
                                if (isSuccess)
                                {
                                    RS = Math.Round(RS, 1);
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;
                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataEVM[testingPort - 1, 4, 0] = PassFail;  // 4 Result
                                dataEVM[testingPort - 1, 4, 1] = testTime;  // 3 Testing Time
                                dataEVM[testingPort - 1, 4, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "%");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.3");
                                portResult.Values.Add("CONFIG", "NRTM3.2");
                                portResult.Values.Add("PARAM", "16QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }

                            #endregion
                            #region EVM-3.3
                            //TODO: TX - Modulation quality TM3.3
                            if (testcaseInTesting.Text == "Modulation quality TM3.3")
                            {
                                mode = "NRTM_33";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                dataFreqErr[testingPort - 1, 5, 2] = Result[0];
                                bool isSuccess = double.TryParse(evm[0], out RS); //giá trị EVM QPSK
                                if (isSuccess)
                                {
                                    RS = Math.Round(RS, 1);
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;
                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");

                                dataEVM[testingPort - 1, 5, 0] = PassFail;  // 4 Result
                                dataEVM[testingPort - 1, 5, 1] = testTime;  // 3 Testing Time
                                dataEVM[testingPort - 1, 5, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "%");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.3");
                                portResult.Values.Add("CONFIG", "NRTM3.3");
                                portResult.Values.Add("PARAM", "QPSK");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion

                            #region FRQERR-2.0
                            //TODO: TX - Frequency error TM2.0
                            if (testcaseInTesting.Text == "Frequency error TM2.0")
                            {
                                mode = "NRTM_20";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                bool isSuccess = double.TryParse(dataFreqErr[testingPort - 1, 0, 2], out RS);
                                if (isSuccess)
                                {
                                    RS = Math.Abs(Math.Round(RS, 1));
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    //do lai 
                                    //Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                    ////TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                    //dataFreqErr[testingPort - 1, 0, 2] = Result[0];
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataFreqErr[testingPort - 1, 0, 0] = PassFail;  // 4 Result
                                dataFreqErr[testingPort - 1, 0, 1] = testTime;  // 3 Testing Time
                                dataFreqErr[testingPort - 1, 0, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "Hz");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.2");
                                portResult.Values.Add("CONFIG", "NRTM2.0");
                                portResult.Values.Add("PARAM", "64QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region FRQERR-2.0a
                            //TODO: TX - Frequency error TM2.0a
                            if (testcaseInTesting.Text == "Frequency error TM2.0a")
                            {
                                mode = "NRTM_20a";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                bool isSuccess = double.TryParse(dataFreqErr[testingPort - 1, 1, 2], out RS);
                                if (isSuccess)
                                {
                                    RS = Math.Abs(Math.Round(RS, 1));
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;
                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    //do lai 
                                    //Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                    ////TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                    //dataFreqErr[testingPort - 1, 0, 2] = Result[0];
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataFreqErr[testingPort - 1, 1, 0] = PassFail;  // 4 Result
                                dataFreqErr[testingPort - 1, 1, 1] = testTime;  // 3 Testing Time
                                dataFreqErr[testingPort - 1, 1, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "Hz");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.2");
                                portResult.Values.Add("CONFIG", "NRTM2.0a");
                                portResult.Values.Add("PARAM", "256QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);

                            }
                            #endregion

                            #region FRQERR-3.1
                            //TODO: TX - Frequency error TM3.1
                            if (testcaseInTesting.Text == "Frequency error TM3.1")
                            {
                                mode = "NRTM_31";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                bool isSuccess = double.TryParse(dataFreqErr[testingPort - 1, 2, 2], out RS);
                                if (isSuccess)
                                {
                                    RS = Math.Abs(Math.Round(RS, 1));
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    //do lai 
                                    //Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                    ////TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                    //dataFreqErr[testingPort - 1, 0, 2] = Result[0];
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataFreqErr[testingPort - 1, 2, 0] = PassFail;  // 4 Result
                                dataFreqErr[testingPort - 1, 2, 1] = testTime;  // 3 Testing Time
                                dataFreqErr[testingPort - 1, 2, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "Hz");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.2");
                                portResult.Values.Add("CONFIG", "NRTM3.1");
                                portResult.Values.Add("PARAM", "64QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);

                            }
                            #endregion

                            #region FRQERR-3.1a
                            //TODO: TX - Frequency error TM3.1a
                            if (testcaseInTesting.Text == "Frequency error TM3.1a")
                            {
                                mode = "NRTM_31a";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                bool isSuccess = double.TryParse(dataFreqErr[testingPort - 1, 3, 2], out RS);
                                if (isSuccess)
                                {
                                    RS = Math.Abs(Math.Round(RS, 1));
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    //do lai 
                                    //Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                    ////TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                    //dataFreqErr[testingPort - 1, 0, 2] = Result[0];
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataFreqErr[testingPort - 1, 3, 0] = PassFail;  // 4 Result
                                dataFreqErr[testingPort - 1, 3, 1] = testTime;  // 3 Testing Time
                                dataFreqErr[testingPort - 1, 3, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "Hz");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.2");
                                portResult.Values.Add("CONFIG", "NRTM3.1a");
                                portResult.Values.Add("PARAM", "256QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);

                            }
                            #endregion

                            #region FRQERR-3.2
                            //TODO: TX - Frequency error TM3.2
                            if (testcaseInTesting.Text == "Frequency error TM3.2")
                            {
                                mode = "NRTM_32";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                bool isSuccess = double.TryParse(dataFreqErr[testingPort - 1, 4, 2], out RS);
                                if (isSuccess)
                                {
                                    RS = Math.Abs(Math.Round(RS, 1));
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    //do lai 
                                    //Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                    ////TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                    //dataFreqErr[testingPort - 1, 0, 2] = Result[0];
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataFreqErr[testingPort - 1, 4, 0] = PassFail;  // 4 Result
                                dataFreqErr[testingPort - 1, 4, 1] = testTime;  // 3 Testing Time
                                dataFreqErr[testingPort - 1, 4, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "Hz");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.2");
                                portResult.Values.Add("CONFIG", "NRTM3.2");
                                portResult.Values.Add("PARAM", "16QAM");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);

                            }
                            #endregion

                            #region FRQERR-3.3
                            //TODO: TX - Frequency error TM3.3
                            if (testcaseInTesting.Text == "Frequency error TM3.3")
                            {
                                mode = "NRTM_33";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                bool isSuccess = double.TryParse(dataFreqErr[testingPort - 1, 5, 2], out RS);
                                if (isSuccess)
                                {
                                    RS = Math.Abs(Math.Round(RS, 1));
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;

                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    //do lai 
                                    //Measurement.EVMFreqErr5G(VSA, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, out evm, out Result[0]);
                                    ////TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                    //dataFreqErr[testingPort - 1, 0, 2] = Result[0];
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }

                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataFreqErr[testingPort - 1, 5, 0] = PassFail;  // 4 Result
                                dataFreqErr[testingPort - 1, 5, 1] = testTime;  // 3 Testing Time
                                dataFreqErr[testingPort - 1, 5, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "Hz");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.2");
                                portResult.Values.Add("CONFIG", "NRTM3.3");
                                portResult.Values.Add("PARAM", "QPSK");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region DR
                            //TODO: TX - Total power dynamic range
                            if (testcaseInTesting.Text == "Total power dynamic range")
                            {
                                mode = "NRTM_31";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                RS = Math.Abs(double.Parse(OFDM[testingPort - 1, 1]) - double.Parse(OFDM[testingPort - 1, 0]));
                                RS = Math.Round(RS, 1);
                                if (RS >= limitMin)
                                {
                                    PassFail = pass;

                                }
                                else
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");

                                result = RS.ToString();
                                dataTotalDR[testingPort - 1, 0] = PassFail;  // 4 Result
                                dataTotalDR[testingPort - 1, 1] = testTime;  // 3 Testing Time
                                dataTotalDR[testingPort - 1, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "dB");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.3");
                                portResult.Values.Add("CONFIG", "NRTM3.1a/2.0a");
                                portResult.Values.Add("PARAM", "Dyn");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("PWR_MAX", OFDM[testingPort - 1, 1].ToString());
                                portResult.Values.Add("PWR_MIN", OFDM[testingPort - 1, 0].ToString());

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);

                            }
                            #endregion
                            #region TSE-G
                            //TODO: TX - Transmitter spurious emissions - General
                            if (testcaseInTesting.Text == "Transmitter spurious emissions - General")
                            {
                                mode = "NRTM_11";
                                //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                PassFail = pass;

                                MessageBox.Show("Đổi sang Port" + testingPort);
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));

                                Measurement.Spur5G(VSA, BBU, "General", mode, attSetVal.ToString(), testingPort, setFrequency, setBandwidth, "", out tseRS);
                                SaveCSVdata(VSA, "General", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                //TakeScreenshotVSA(VSA, "General", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                BBU.CHG_RRU_POWER(1000);

                                for (int i2 = 0; i2 < tseRS.Length; i2++)
                                {
                                    if (tseRS[i2] != null)
                                    {
                                        TSE[0, i2] = tseRS[i2];
                                        if (TSE[0, i2] > 0)
                                        {
                                            reTest = true;
                                        }
                                        else { reTest = false; }
                                    }
                                }
                            }


                            //lblStatusProgress_TextChanged(testcaseInTesting.Text + " :Port " + testingPort);
                            //PassFail = pass;

                            //while (PassFail == pass) // Neu 1 chi tieu FAIL thi thoat vong lap ko do chi tieu khac nua
                            //{
                            //    foreach(TestcaseNode node in )
                            //    {

                            //    }
                            //    for (int i1 = 0; i1 < numbersubmeasClause; i1++)
                            //    {
                            //        int count = 0;
                            //        while (count < 3)
                            //        {
                            //            if (measurementBackgroundWorker.CancellationPending) return;
                            //            System.Threading.Thread.Sleep(1);
                            //            if (submeasClause[i1, 0] != null)
                            //            {
                            //                MessageBox.Show("Đổi sang Port" + testingPort);
                            //                if (measurementBackgroundWorker.CancellationPending) return;
                            //                System.Threading.Thread.Sleep(1);

                            //                if (submeasClause[i1, 0] == "++ General")
                            //                {
                            //                    limitMax = double.Parse(submeasClause[i1, 3]);
                            //                    BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
                            //                    // Spurious General 
                            //                    Measurement.Spur5G(VSA, BBU, "General", NRTM, arrAttDL[j], j + 1, setFrequency, "", out tseRS);
                            //                    //==============19.2.2024 save screen shot =====================
                            //                    SaveCSVdata(VSA, "General", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    //TakeScreenshotVSA(VSA, "General", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    BBU.CHG_RRU_POWER(1000);
                            //                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            //                    for (int i2 = 0; i2 < tseRS.Length; i2++)
                            //                    {
                            //                        if (tseRS[i2] != null)
                            //                        {
                            //                            TSE[0, i2] = tseRS[i2];
                            //                            if (TSE[0, i2] > 0)
                            //                            {
                            //                                reTest = true;
                            //                            }
                            //                            else { reTest = false; }
                            //                        }
                            //                    }
                            //                }
                            //                else if (submeasClause[i1, 0] == "++ Protection")
                            //                {
                            //                    limitMax = double.Parse(submeasClause[i1, 3]);
                            //                    BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
                            //                    // Spurious Protection
                            //                    Measurement.Spur5G(VSA, BBU, "Protection", NRTM, arrAttDL[j], j + 1, setFrequency, "", out tseRS);
                            //                    //==============19.2.2024 save screen shot =====================
                            //                    SaveCSVdata(VSA, "Protection", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    //TakeScreenshotVSA(VSA, "Protection", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    BBU.CHG_RRU_POWER(1000);
                            //                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            //                    for (int i2 = 0; i2 < tseRS.Length; i2++)
                            //                    {
                            //                        if (tseRS[i2] != null)
                            //                        {
                            //                            TSE[1, i2] = tseRS[i2];
                            //                            if (TSE[1, i2] > 0)
                            //                            {
                            //                                reTest = true;
                            //                            }
                            //                            else { reTest = false; }
                            //                        }
                            //                    }
                            //                }
                            //                else if (submeasClause[i1, 0] == "++ Co existence")
                            //                {
                            //                    limitMax = double.Parse(submeasClause[i1, 3]);
                            //                    BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
                            //                    // Spurious Co-ex BW100K
                            //                    Measurement.Spur5G(VSA, BBU, "Co-existance", NRTM, arrAttDL[j], j + 1, setFrequency, "100kHz", out tseRS);
                            //                    //==============19.2.2024 save screen shot =====================
                            //                    SaveCSVdata(VSA, "Co-existance", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    //TakeScreenshotVSA(VSA, "Co-existance", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            //                    for (int i2 = 0; i2 < tseRS.Length; i2++)
                            //                    {
                            //                        if (tseRS[i2] != null)
                            //                        {
                            //                            TSE[2, i2] = tseRS[i2];
                            //                            if (TSE[2, i2] > 0)
                            //                            {
                            //                                reTest = true;
                            //                            }
                            //                            else { reTest = false; }
                            //                        }
                            //                    }
                            //                    // Spurious Co-ex BW1M
                            //                    Measurement.Spur5G(VSA, BBU, "Co-existance", NRTM, arrAttDL[j], j + 1, setFrequency, "1MHz", out tseRS);
                            //                    //==============19.2.2024 save screen shot =====================
                            //                    SaveCSVdata(VSA, "Co-existance", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    //TakeScreenshotVSA(VSA, "Co-existance", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    BBU.CHG_RRU_POWER(1000);
                            //                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            //                    for (int i2 = 0; i2 < tseRS.Length; i2++)
                            //                    {
                            //                        if (tseRS[i2] != null)
                            //                        {
                            //                            TSE[3, i2] = tseRS[i2];
                            //                            if (TSE[3, i2] > 0)
                            //                            {
                            //                                reTest = true;
                            //                            }
                            //                            else { reTest = false; }
                            //                        }
                            //                    }
                            //                }
                            //                else if (submeasClause[i1, 0] == "++ Co location")
                            //                {
                            //                    limitMax = double.Parse(submeasClause[i1, 3]);
                            //                    BBU.CHG_RRU_POWER(int.Parse(txtPWR.Text));
                            //                    // Spurious Co-lo 
                            //                    Measurement.Spur5G(VSA, BBU, "Co-location", NRTM, arrAttDL[j], j + 1, setFrequency, "", out tseRS);
                            //                    //==============19.2.2024 save screen shot =====================
                            //                    SaveCSVdata(VSA, "Co-location", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    //TakeScreenshotVSA(VSA, "Co-location", measClause[i, 0], "P" + (j + 1), setFrequency);
                            //                    BBU.CHG_RRU_POWER(1000);
                            //                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                            //                    for (int i2 = 0; i2 < tseRS.Length; i2++)
                            //                    {
                            //                        if (tseRS[i2] != null)
                            //                        {
                            //                            TSE[4, i2] = tseRS[i2];
                            //                            if (TSE[4, i2] > 0)
                            //                            {
                            //                                reTest = true;
                            //                            }
                            //                            else { reTest = false; }
                            //                        }
                            //                    }
                            //                }
                            //                // Neu reTest fail
                            //                if (reTest)
                            //                {
                            //                    count++;
                            //                }
                            //                else
                            //                { count = 3; break; }
                            //            }
                            //        }
                            //        //tseRS[] : Mang ket qua tra ve cua moi lan do. tseRS[i] co the la null
                            //        // TSE[,] : Mang ket qua cua moi lan do 
                            //        // 
                            //        nRS = -100;
                            //        for (int i11 = 0; i11 < tseRS.Length; i11++)
                            //        {
                            //            if ((tseRS[i11] != null))
                            //            {
                            //                for (int i2 = 0; i2 < tseRS.Length; i2++)
                            //                {
                            //                    if ((TSE[i11, i2] != null))
                            //                    {
                            //                        if (TSE[i11, i2] > nRS)
                            //                        {
                            //                            nRS = TSE[i11, i2];
                            //                        }
                            //                    }
                            //                }
                            //            }
                            //        } // Tim max tseRS[]
                            //        if (nRS > 0) { PassFail = fail; }
                            //        else { PassFail = pass; }
                            //        result = Math.Round((double)nRS, 1).ToString();
                            //    }
                            //    break; // 
                            //}
                            //dataTXSUPR[j, 0] = PassFail;  // Result
                            //dataTXSUPR[j, 1] = DateTime.Now.ToString("hh:mm:ss");  // Testing Time
                            //Log("Ket qua cuoi cung  " + result);

                            //}
                            #endregion
                            #region ON/OFF Power   
                            //TODO: TX - Transmitter OFF Power
                            else if (testcaseInTesting.Text == "Transmitter OFF Power")
                            {
                                mode = "NRTM_11";
                                GeneralDataSetupTX(rruCSVFilePathOnOff, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                RFSwitchRRUPort("TX", testingPort - 1);
                                //RRU properties
                                RRU.Testcase = TestCase.TX;
                                RRU.CurrentTestingPort = testingPort;
                                RRU.CurrentTestingSpec = testcaseInTesting.Text;
                                RRU.ReadATTfromCSVdir(rruCSVFilePathOnOff, "TX", double.Parse(setFrequency));
                                MessageBox.Show("Đổi sang port " + testingPort, "Thông báo");
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                BBU.CHG_RRU_POWER(int.Parse(setPower));
                                lblStatusProgress_TextChanged(testcaseInTesting.Text + " :Port " + testingPort);
                                Log(testcaseInTesting.Text + " :Port " + testingPort);
                                Log("Test time: " + (timeout + 1));
                                Measurement.OnOffPower5G(VSA, BBU, mode, testingPort, attSetVal.ToString(), setFrequency, setBandwidth, out Result[0], out Result[1]);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                Log("On/Off Power port " + testingPort + ":" + Result[0] + " dBm/MHz");
                                Log("On/Off Transient port " + testingPort + ":" + Result[1] + " us");
                                Transient[testingPort - 1, 1] = Result[1];
                                bool isSuccess = double.TryParse(Result[0], out RS);
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                BBU.CHG_RRU_POWER(1000);
                                if (isSuccess)
                                {
                                    RS = Math.Round(RS, 1);
                                    if (RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;
                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }

                                string testTime = DateTime.Now.ToString("hh:mm:ss");

                                dataOnOffPower[testingPort - 1, 0] = PassFail;  // 4 Result
                                dataOnOffPower[testingPort - 1, 1] = testTime; // 3 Testing Time
                                dataOnOffPower[testingPort - 1, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "dBm/MHz");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.4.1");
                                portResult.Values.Add("CONFIG", "NRTM1.1");
                                portResult.Values.Add("PARAM", "OFF_Level");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region Trancient Period                            
                            else if (testcaseInTesting.Text == "Transmitter transient period")
                            {
                                mode = "NRTM_11";
                                GeneralDataSetupTX(rruCSVFilePathOnOff, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                RS = double.Parse(Transient[testingPort - 1, 1]);
                                if (RS <= limitMax)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                result = RS.ToString();
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataTransient[testingPort - 1, 0] = PassFail;  // 4 Result
                                dataTransient[testingPort - 1, 1] = testTime;  // 3 Testing Time
                                dataTransient[testingPort - 1, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "us");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.4.2");
                                portResult.Values.Add("CONFIG", "NRTM1.1");
                                portResult.Values.Add("PARAM", "T_period");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);

                            }
                            #endregion
                            #region TAE  
                            //TODO: TX - Time alignment error
                            else if (testcaseInTesting.Text == "Time alignment error")
                            {
                                mode = "NRTM_11";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                Log("Test time: " + (timeout + 1));

                                string taeref = taetmp;
                                Measurement.TAEMeasurement(VSA, BBU, mode, testingPort, attSetVal.ToString(), setFrequency, setBandwidth, ref taeref, ref Result);
                                //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                taetmp = taeref;
                                bool isSuccess = double.TryParse(Result[0], out RS);
                                if (isSuccess)
                                {
                                    RS = Math.Round(RS, 1);
                                    if (limitMin > RS || RS > limitMax)
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    else
                                    {
                                        PassFail = pass;
                                    }
                                    result = RS.ToString();
                                }
                                else
                                {
                                    result = error;
                                    PassFail = fail;
                                    reTest = true;
                                }
                                string testTime = DateTime.Now.ToString("hh:mm:ss");
                                dataTAE[testingPort - 1, 0] = PassFail;  // 4 Result
                                dataTAE[testingPort - 1, 1] = testTime;  // 3 Testing Time
                                dataTAE[testingPort - 1, 2] = result;     // 1 Reading Value

                                //ghi dữ liệu cho report
                                portResult.Values.Add("PORT", testingPort.ToString());
                                portResult.Values.Add("CLAUSE", testcaseInTesting.Text);
                                portResult.Values.Add("UNIT", "ns");
                                portResult.Values.Add("DESC", "TS38.141-1 Chapter 6.5.4");
                                portResult.Values.Add("CONFIG", "NRTM1.1");
                                portResult.Values.Add("PARAM", "TAE");

                                // Kiểm tra nếu có limitMin và limitMax
                                string specString = BuildSpecString(portResult.Values["PARAM"], strLimMin, strLimMax);
                                portResult.Values.Add("SPECS", specString);

                                portResult.Values.Add("MIN", strLimMin);
                                portResult.Values.Add("MAX", strLimMax);

                                portResult.Values.Add("RESULT", result);
                                portResult.Values.Add("PASS/FAIL", PassFail);
                                portResult.Values.Add("TEST TIME", testTime);
                            }
                            #endregion
                            #region IMD-ACLR   
                            //TODO: TX - Transmitter intermodulation (ACLR) - hoi lai Hieu
                            else if (testcaseInTesting.Text == "Transmitter intermodulation (ACLR)")
                            {
                                mode = "NRTM_11";
                                double protectVal = -50.0; //Gioi han bao ve 
                                GeneralDataSetupTX(rruCSVFilePathIMDTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strLimit = ReadValueByHeader(testcaseInTesting.Parameters, "Limit");

                                //6 khoang offset cua nhieu
                                string strPwr_Offset1 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O1");
                                string strPwr_Offset2 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O2");
                                string strPwr_Offset3 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O3");
                                string strPwr_Offset4 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O4");
                                string strPwr_Offset5 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O5");
                                string strPwr_Offset6 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O6");
                                

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                double.TryParse(strLimMin, out protectVal);

                                double.TryParse(strPwr_Offset1, out double pwrOffset1);
                                double.TryParse(strPwr_Offset2, out double pwrOffset2);
                                double.TryParse(strPwr_Offset3, out double pwrOffset3);
                                double.TryParse(strPwr_Offset4, out double pwrOffset4);
                                double.TryParse(strPwr_Offset5, out double pwrOffset5);
                                double.TryParse(strPwr_Offset6, out double pwrOffset6);

                                MessageBox.Show("Đổi sang Port" + testingPort, "Thông báo");

                                BBU.CHG_RRU_POWER(int.Parse(setPower));
                                for (int i1 = 0; i1 < 6; i1++)
                                {
                                    if (protectVal < pwrOffset1)
                                    {
                                        MessageBox.Show($"Công suất {pwrOffset1} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        BBU.StopProcess();
                                        clearFlag = true;
                                        if (measurementBackgroundWorker.IsBusy == true)
                                        {
                                            measurementBackgroundWorker.CancelAsync();
                                        }
                                        timerProcess.Stop();
                                        btnConnect_Enable();
                                        lblStatusProgress_TextChanged("Đã dừng");
                                    }
                                }
                                Measurement.IMDACLR5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setBandwidth, "-25", pwrOffset1.ToString(), out RS);
                                TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET -25", setFrequency);

                                imdACLR[0] = RS;
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                Measurement.IMDACLR5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setBandwidth, "25", pwrOffset6.ToString(), out RS);
                                TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET +25", setFrequency);

                                imdACLR[1] = RS;
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                Measurement.IMDACLR5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setBandwidth, "-15", pwrOffset2.ToString(), out RS);
                                TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET -15", setFrequency);

                                imdACLR[2] = RS;
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                Measurement.IMDACLR5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setBandwidth, "15", pwrOffset5.ToString(), out RS);
                                TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET +15", setFrequency);

                                imdACLR[3] = RS;
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                Measurement.IMDACLR5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setBandwidth, "-5", pwrOffset3.ToString(), out RS);
                                TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET -5", setFrequency);

                                imdACLR[4] = RS;
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                Measurement.IMDACLR5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setBandwidth, "5", pwrOffset4.ToString(), out RS);
                                TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET +5", setFrequency);

                                imdACLR[5] = RS;
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                BBU.CHG_RRU_POWER(1000);

                                RS = 100;
                                for (int x = 0; x < 6; x++)
                                {
                                    if (imdACLR[x] < RS)
                                    {
                                        RS = imdACLR[x];
                                    }
                                }
                                Console.WriteLine("Kết quả cuối cùng  " + RS);
                                if (RS < limitMin)
                                {
                                    BBU.CHG_RRU_POWER(int.Parse(setPower));
                                    PassFail = fail;
                                    reTest = true;
                                }
                                else
                                {
                                    PassFail = pass;
                                }
                                result = Math.Round(RS, 1).ToString();

                            }
                            #endregion
                            #region IMD-SEM  
                            //TODO: TX - Transmitter intermodulation (SEM)
                            else if (testcaseInTesting.Text == "Transmitter intermodulation (SEM)")
                            {
                                mode = "NRTM_11";
                                GeneralDataSetupTX(rruCSVFilePathIMDTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);
                                double protectVal = -50.0; //Gioi han bao ve 

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strLimit = ReadValueByHeader(testcaseInTesting.Parameters, "Limit");

                                //6 khoang offset cua nhieu
                                string strPwr_Offset1 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O1");
                                string strPwr_Offset2 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O2");
                                string strPwr_Offset3 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O3");
                                string strPwr_Offset4 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O4");
                                string strPwr_Offset5 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O5");
                                string strPwr_Offset6 = ReadValueByHeader(testcaseInTesting.Parameters, "IMD O6");


                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                double.TryParse(strLimMin, out protectVal);

                                double.TryParse(strPwr_Offset1, out double pwrOffset1);
                                double.TryParse(strPwr_Offset2, out double pwrOffset2);
                                double.TryParse(strPwr_Offset3, out double pwrOffset3);
                                double.TryParse(strPwr_Offset4, out double pwrOffset4);
                                double.TryParse(strPwr_Offset5, out double pwrOffset5);
                                double.TryParse(strPwr_Offset6, out double pwrOffset6);

                                MessageBox.Show("Đổi sang Port" + testingPort, "Thông báo");
                                BBU.CHG_RRU_POWER(int.Parse(setPower));
                                for (int i1 = 0; i1 < 6; i1++)
                                    {
                                        if (protectVal < pwrOffset1)
                                        {
                                            MessageBox.Show($"Công suất {pwrOffset1} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            BBU.StopProcess();
                                            clearFlag = true;
                                            if (measurementBackgroundWorker.IsBusy == true)
                                            {
                                                measurementBackgroundWorker.CancelAsync();
                                            }
                                            timerProcess.Stop();
                                            btnConnect_Enable();
                                            lblStatusProgress_TextChanged("Đã dừng");
                                        }
                                    }
                                Measurement.IMDSEM5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setPower, setBandwidth, "-25", pwrOffset1.ToString(), out Result[0]);
                                    //==============19.2.2024 save screen shot =====================
                                    TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET -25", setFrequency);
                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                    imdSEM[0] = double.Parse(Result[0]);
                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);

                                    Measurement.IMDSEM5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setPower, setBandwidth, "25", pwrOffset6.ToString(), out Result[0]);
                                    //==============19.2.2024 save screen shot =====================
                                    TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET +25", setFrequency);
                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                    imdSEM[1] = double.Parse(Result[0]);
                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);

                                    Measurement.IMDSEM5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setPower, setBandwidth, "-15", pwrOffset2.ToString(), out Result[0]);
                                    //==============19.2.2024 save screen shot =====================
                                    TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET -15", setFrequency);
                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                    imdSEM[2] = double.Parse(Result[0]);
                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);

                                    Measurement.IMDSEM5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setPower, setBandwidth, "15", pwrOffset5.ToString(), out Result[0]);
                                    //==============19.2.2024 save screen shot =====================
                                    TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET +15", setFrequency);
                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                    imdSEM[3] = double.Parse(Result[0]);
                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);

                                    Measurement.IMDSEM5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setPower, setBandwidth, "-5", pwrOffset3.ToString(), out Result[0]);
                                    //==============19.2.2024 save screen shot =====================
                                    TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET -5", setFrequency);
                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                    imdSEM[4] = double.Parse(Result[0]);
                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    Measurement.IMDSEM5G(VSA, VSG1, BBU, testingPort, mode, attSetVal.ToString(), setFrequency, setPower, setBandwidth, "5", pwrOffset4.ToString(), out Result[0]);
                                    //==============19.2.2024 save screen shot =====================
                                    TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, $"P{testingPort}_OFFSET +5", setFrequency);
                                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                    imdSEM[5] = double.Parse(Result[0]);
                                    BBU.CHG_RRU_POWER(1000);
                                    RS = -100;
                                    for (int x = 0; x < 6; x++)
                                    {
                                        if (imdSEM[x] > RS)
                                        {
                                            RS = imdSEM[x];
                                        }
                                    }
                                    Console.WriteLine("Kết quả cuối cùng  " + RS);

                                    if (RS <= limitMax)
                                    {
                                        PassFail = pass;
                                    }
                                    else
                                    {
                                        PassFail = fail;
                                        reTest = true;
                                    }
                                    result = Math.Round(RS, 1).ToString();
                                }                                
                            
                            #endregion
                            #region IMD-TSE     
                            //TODO: TX - Transmitter intermodulation (SPUR)
                            else if (testcaseInTesting.Text == "Transmitter intermodulation (SPUR)")
                            {
                                mode = "NRTM_11";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);

                                //                    #region IMD SPUR
                                //                    else if (measClause[i, 0] == "+ Transmitter intermodulation (SPUR)")
                                //                    {
                                //                        mode = "NRTM_11";
                                //                        //GeneralDataSetupTX(rruCSVFilePathIMDTX, measClause[i, 0], j, mode);
                                //                        if (measurementBackgroundWorker.CancellationPending) return;
                                //                        System.Threading.Thread.Sleep(1);
                                //                        lblStatusProgress_TextChanged(measClause[i, 0] + " :Port " + (j + 1));
                                //                        MessageBox.Show("Đổi sang Port" + (j + 1), "Thông báo");

                                //                        double.TryParse(measClause[i, 3], out limitMax);
                                //                        BBU.CHG_RRU_POWER(int.Parse(setPower));
                                //                        for (int i1 = 0; i1 < 6; i1++)
                                //                        {
                                //                            if (double.Parse(measClause[i, 6]) < double.Parse(measClause[i, i1 + 7]))
                                //                            {
                                //                                MessageBox.Show($"Công suất {measClause[i, i1 + 7]} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //                                BBU.StopProcess();
                                //                                clearFlag = true;
                                //                                if (measurementBackgroundWorker.IsBusy == true)
                                //                                {
                                //                                    measurementBackgroundWorker.CancelAsync();
                                //                                }
                                //                                timerProcess.Stop();
                                //                                btnConnect_Enable();
                                //                                lblStatusProgress_TextChanged("Đã dừng");
                                //                            }
                                //                        }
                                //                        Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "-25", measClause[i, 7], out tseRS);
                                //                        //==============19.2.2024 save screen shot =====================
                                //                        SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -25", setFrequency);
                                //                        //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -25", setFrequency);
                                //                        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                //                        for (int i1 = 0; i1 < tseRS.Length; i1++)
                                //                        {
                                //                            if (tseRS[i1] != null)
                                //                            {
                                //                                TSE[0, i1] = tseRS[i1];
                                //                            }
                                //                        }
                                //                        if (measurementBackgroundWorker.CancellationPending) return;
                                //                        System.Threading.Thread.Sleep(1);

                                //                        Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "25", measClause[i, 12], out tseRS);
                                //                        //==============19.2.2024 save screen shot =====================

                                //                        SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +25", setFrequency);
                                //                        //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +25", setFrequency);
                                //                        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                //                        for (int i1 = 0; i1 < tseRS.Length; i1++)
                                //                        {
                                //                            if (tseRS[i1] != null)
                                //                            {
                                //                                TSE[0, i1] = tseRS[i1];
                                //                            }
                                //                        }
                                //                        if (measurementBackgroundWorker.CancellationPending) return;
                                //                        System.Threading.Thread.Sleep(1);

                                //                        Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "-15", measClause[i, 8], out tseRS);
                                //                        //==============19.2.2024 save screen shot =====================

                                //                        SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -15", setFrequency);
                                //                        //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -15", setFrequency);
                                //                        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                //                        for (int i1 = 0; i1 < tseRS.Length; i1++)
                                //                        {
                                //                            if (tseRS[i1] != null)
                                //                            {
                                //                                TSE[2, i1] = tseRS[i1];
                                //                            }
                                //                        }
                                //                        if (measurementBackgroundWorker.CancellationPending) return;
                                //                        System.Threading.Thread.Sleep(1);

                                //                        Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "15", measClause[i, 11], out tseRS);
                                //                        //==============19.2.2024 save screen shot =====================

                                //                        SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +15", setFrequency);
                                //                        //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +15", setFrequency);
                                //                        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                //                        for (int i1 = 0; i1 < tseRS.Length; i1++)
                                //                        {
                                //                            if (tseRS[i1] != null)
                                //                            {
                                //                                TSE[3, i1] = tseRS[i1];
                                //                            }
                                //                        }
                                //                        if (measurementBackgroundWorker.CancellationPending) return;
                                //                        System.Threading.Thread.Sleep(1);

                                //                        Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "-5", measClause[i, 9], out tseRS);
                                //                        //==============19.2.2024 save screen shot =====================

                                //                        SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -5", setFrequency);
                                //                        //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET -5", setFrequency);
                                //                        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                //                        for (int i1 = 0; i1 < tseRS.Length; i1++)
                                //                        {
                                //                            if (tseRS[i1] != null)
                                //                            {
                                //                                TSE[4, i1] = tseRS[i1];
                                //                            }
                                //                        }
                                //                        if (measurementBackgroundWorker.CancellationPending) return;
                                //                        System.Threading.Thread.Sleep(1);

                                //                        Measurement.IMDTSE5G(VSA, VSG1, BBU, mode, arrAttDL[j], j, setFrequency, setPower, setBandwidth, "5", measClause[i, 10], out tseRS);
                                //                        //==============19.2.2024 save screen shot =====================
                                //                        SaveCSVdata(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +5", setFrequency);
                                //                        //TakeScreenshotVSA(VSA, "TX", measClause[i, 0], $"P{j + 1}_OFFSET +5", setFrequency);
                                //                        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                                //                        for (int i1 = 0; i1 < tseRS.Length; i1++)
                                //                        {
                                //                            if (tseRS[i1] != null)
                                //                            {
                                //                                TSE[5, i1] = tseRS[i1];
                                //                            }
                                //                        }
                                //                        if (measurementBackgroundWorker.CancellationPending) return;
                                //                        System.Threading.Thread.Sleep(1);
                                //                        BBU.CHG_RRU_POWER(1000);
                                //                        nRS = -100;
                                //                        for (int x = 0; x < TSE.Length; x++)
                                //                        {
                                //                            for (int y = 0; y < TSE.Length; y++)
                                //                            {
                                //                                if (TSE[x, y] > RS)
                                //                                {
                                //                                    nRS = TSE[x, y];
                                //                                }
                                //                            }
                                //                        }
                                //                        if (nRS > limitMax)
                                //                        {
                                //                            PassFail = fail;
                                //                            reTest = true;
                                //                        }
                                //                        else
                                //                        {
                                //                            PassFail = pass;
                                //                        }
                                //                        result = Math.Round((double)nRS, 1).ToString();
                                //                        Console.WriteLine("Ket qua cuôi cùng  " + result);
                                //                    }
                                //                    #endregion





                            }
                            #endregion
                            #region Intra-SEM 
                            //TODO: TX - Intra-system SEM
                            else if (testcaseInTesting.Text == "Intra-system SEM")
                            {
                                mode = "NRTM_11";
                                GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);
                                double protectVal = -50.0; //Gioi han bao ve 

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strLimit = ReadValueByHeader(testcaseInTesting.Parameters, "Limit");

                                //khoang offset cua nhieu
                                string strPwr_Offset1 = ReadValueByHeader(testcaseInTesting.Parameters, "INTRA");
                                double.TryParse(strPwr_Offset1, out double pwrOffset1);

                                System.Threading.Thread.Sleep(1);
                                BBU.TDDMODE("TX_ONLY");
                                MessageBox.Show("Đổi sang Port" + testingPort, "Thông báo");

                                if (protectVal < pwrOffset1)
                                {
                                    MessageBox.Show($"Công suất {pwrOffset1} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    BBU.StopProcess();
                                    clearFlag = true;
                                    if (measurementBackgroundWorker.IsBusy == true)
                                    {
                                        measurementBackgroundWorker.CancelAsync();
                                    }
                                        timerProcess.Stop();
                                        btnConnect_Enable();
                                        lblStatusProgress_TextChanged("Đã dừng");
                                    }
                                    else
                                    {
                                        BBU.CHG_RRU_POWER(int.Parse(setPower));
                                        Measurement.IntraSem(VSA, VSG1, BBU, mode, attSetVal.ToString(), testingPort, setFrequency, setPower, setBandwidth, pwrOffset1.ToString(), out dbRS[0], out dbRS[1], out dbRS[2], ref RS);
                                        TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);

                                        imdACLR[0] = RS;
                                        if (measurementBackgroundWorker.CancellationPending) return;
                                        System.Threading.Thread.Sleep(1);
                                        BBU.CHG_RRU_POWER(1000);
                                        RS = Math.Round(dbRS[0], 1);
                                        if (RS > limitMax)
                                        {
                                            PassFail = fail;
                                            reTest = true;
                                        }
                                        else
                                        {
                                            PassFail = pass;
                                        }
                                        result = RS.ToString();
                                        dataSEM[testingPort, 0, 0] = PassFail;
                                        dataSEM[testingPort, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                        dataSEM[testingPort, 0, 2] = Math.Round(dbRS[1], 1).ToString();
                                        dataSEM[testingPort, 0, 3] = Math.Round(dbRS[2], 1).ToString();
                                        dataSEM[testingPort, 0, 4] = Math.Round(dbRS[3], 1).ToString();
                                    }
                                }
                            #endregion
                            #region Intra-ACLR  
                            //TODO: TX - Intra-system ACLR
                            else if (testcaseInTesting.Text == "Intra-system ACLR")
                            {
                                //mode = "NRTM_11";
                                //GeneralDataSetupTX(rruCSVFilePathTX, testcaseInTesting.Text, testingPort, mode, out attSetVal);

                                //if (measurementBackgroundWorker.CancellationPending) return;
                                //System.Threading.Thread.Sleep(1);

                                //BBU.TDDMODE("TX_ONLY");
                                //MessageBox.Show("Đổi sang Port" + testingPort, "Thông báo");

                                ////Read data
                                //string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                //string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                //string strLimitInterfer = ReadValueByHeader(testcaseInTesting.Parameters, "Limit");
                                //strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                // strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                //strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                //double.TryParse(strLimMax, out limitMax);
                                //double.TryParse(strLimMin, out limitMin);
                                //double.TryParse(strLimitInterfer, out double limitInterfer);

                                //limitInterferSetup;



                                //double.TryParse(measClause[i, 13], out limitInterferSetup);
                                //if (limitInterfer < limitInterferSetup)
                                //{
                                //    MessageBox.Show($"Công suất {measClause[i, 12]} vượt quá giá trị cho phép", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //    BBU.StopProcess();
                                //    clearFlag = true;
                                //    if (measurementBackgroundWorker.IsBusy == true)
                                //    {
                                //        measurementBackgroundWorker.CancelAsync();
                                //    }
                                //    timerProcess.Stop();
                                //    btnConnect_Enable();
                                //    lblStatusProgress_TextChanged("Đã dừng");
                                //}
                                //else
                                //{
                                //    BBU.CHG_RRU_POWER(int.Parse(setPower));
                                //    Measurement.IntraACLR5G(VSA, VSG1, BBU, mode, arrAttDL[j], port, setFrequency, setPower, setBandwidth, measClause[i, 13], ref RS);
                                //    //TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency);
                                //    if (measurementBackgroundWorker.CancellationPending) return;
                                //    System.Threading.Thread.Sleep(1);
                                //    BBU.CHG_RRU_POWER(1000);
                                //    RS = Math.Abs(RS);
                                //    if (RS < limitMin)
                                //    {
                                //        BBU.CHG_RRU_POWER(int.Parse(setPower));
                                //        PassFail = fail;
                                //        reTest = true;
                                //    }
                                //    else
                                //    {
                                //        PassFail = pass;
                                //    }
                                //    result = Math.Round(RS, 1).ToString();
                                //}

                            }
                            #endregion

                            #region Uplink Measurement
                            #region RX-Calib
                            else if (testcaseInTesting.Text == "RX Power Calibration" && testingPort == testingPortList.Min())
                            {
                                //RRU properties
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);

                                bool flagrx = true;
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                BBU.StopProcess();

                                Measurement.calibRX(BBU, testingPortList, VSG1, setFrequency, strWSPwr, out RS, out flagrx);
                                if (flagrx == true)
                                {
                                    if (RS <= limitMax)
                                    {
                                        result = RS.ToString();
                                        PassFail = "PASS";
                                    }
                                    else
                                    {
                                        PassFail = "FAIL";
                                        result = RS.ToString();
                                    }
                                }
                                else
                                {
                                    PassFail = "FAIL";
                                    result = RS.ToString();
                                }
                                dataCalibRx[testingPort - 1, 0] = strLimMin; // 0 Min limit
                                dataCalibRx[testingPort - 1, 1] = result;     // 1 Reading Value
                                dataCalibRx[testingPort - 1, 2] = strLimMax; // 2 Max limit
                                dataCalibRx[testingPort - 1, 3] = DateTime.Now.ToString("hh:mm:ss");  // 3 Testing Time
                                dataCalibRx[testingPort - 1, 4] = PassFail;  // 4 Result
                                ListViewShow("PORT" + testingPort, testcaseInTesting.Text, strLimMin, strLimMax, result, strLimMin, mode, PassFail, Math.Ceiling((DateTime.Now - startTime).TotalSeconds).ToString());
                            }

                            #endregion
                            //TODO: RX - Reference Sensitivity Level
                            #region Psens
                            else if (testcaseInTesting.Text == "Reference Sensitivity Level")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strWSPwrSpec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetH = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetH = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");

                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                wsTrigDelay = wsTrigDelay * 1e-3;
                                VSG1.SetTriggerDelay(wsTrigDelay);

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);

                                //kiem tra neu dang khai bao vsg2 va vsg3 thi tat RF out

                                if (VSG2 != null)
                                {
                      //              VSG2.OnOffSignal(0);
                                }

                                if (VSG3 != null)
                                {
                          //          VSG3.OnOffSignal(0);
                                }

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                ProgressLabel($"Measuring Testcase:{testcaseInTesting.Text}, port = {testingPort}...");
                                double ReferenceSensitivityThreshold = Measurement.oneVSGMode(VSG1, BBU, "SEN", repeat, strLimMax, stepdB, attSetVal.ToString(), testingPort, strWSPwr, setFrequency, isMultiMeas, wsOffsetL, setBandwidth.ToString(), wsTrigDelay,out tp);
                                if ((tp >= 95) && (ReferenceSensitivityThreshold <= double.Parse(strWSPwrSpec)))
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                result = ReferenceSensitivityThreshold.ToString();
                                mode = tp.ToString();
                                dataPsen[testingPort - 1, 0] = PassFail;  // 0 Result
                                dataPsen[testingPort - 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataPsen[testingPort - 1, 2] = tp.ToString();     // 3 Reading Value
                                dataPsen[testingPort - 1, 3] = result;     // 4 Reading Value

                            }
                            #endregion                            

                            #region DYN
                            else if (testcaseInTesting.Text == "Dynamic Range")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                VSG1.AttenuatorLevel = attSetVal;

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetH = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetH = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isAwgnSNR = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);

                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                wsTrigDelay = wsTrigDelay * 1e-3;
                                VSG1.SetTriggerDelay(wsTrigDelay);

                                //string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                //int.TryParse(strWSTrigDelay, out int wsTrigDelay);
                                //VSG1.SetTriggerDelay(wsTrigDelay * 1e-3);

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                Measurement.oneVSGMode(VSG1, BBU, "DR", repeat, strLimMin, stepdB, attSetVal.ToString(), testingPort, strWSPwr, setFrequency, isMultiMeas, wsOffsetL, setBandwidth.ToString(),wsTrigDelay, out tp);
                                if (tp >= 95)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                mode = tp.ToString();
                                result = strISPwr;
                                dataDR[testingPort - 1, 0] = PassFail;  // 0 Result
                                dataDR[testingPort - 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataDR[testingPort - 1, 2] = tp.ToString();     // 3 Reading Value
                                dataDR[testingPort - 1, 3] = result;     // 4 Reading Value

                            }
                            #endregion

                            #region ICS
                            else if (testcaseInTesting.Text == "In-Channel Selectivity (ICS)")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                VSG1.AttenuatorLevel = attSetVal;

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetH = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetH = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetL");
                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);

                                //string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                //double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                //VSG1.SetTriggerDelay(wsTrigDelay * 1e-3);
                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                wsTrigDelay = wsTrigDelay * 1e-3;
                                VSG1.SetTriggerDelay(wsTrigDelay);


                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                string isFreq = "";

                                if (VSG2 != null)
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = double.Parse(setFrequency);
                                    isFreq = VSG2.SetFrequencyValue.ToString(); //23.2.2024
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    VSG2.AttenuatorLevel = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue);
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={VSG2.AttenuatorLevel} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");


                                }

                                //IS at upper edge
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                Measurement.twoVSGMode(VSG1, VSG2, BBU, "ICS", wsOffsetL, isOffsetH, stepdB, attSetVal.ToString(), testingPort, strWSPwr, setFrequency, strISPwr, isFreq, null, null, setBandwidth,wsTrigDelay, out tp);
                                double tmpTP = tp;

                                //IS at lower edge
                                LogDashedLine();
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                Measurement.twoVSGMode(VSG1, VSG2, BBU, "ICS", wsOffsetH, isOffsetL, stepdB, attSetVal.ToString(), testingPort, strWSPwr, setFrequency, strISPwr, isFreq, null, null, setBandwidth,wsTrigDelay, out tp);
                                tp = Math.Min(tmpTP, tp);
                                if (tp >= 95 & tp <= 100)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                result = strISPwr;
                                mode = tp.ToString();
                                dataICS[testingPort - 1, 0] = PassFail;  // 0 Result
                                dataICS[testingPort - 1, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataICS[testingPort - 1, 2] = tp.ToString();     // 3 Reading Value
                                dataICS[testingPort - 1, 3] = result;     // 4 Reading Value


                            }
                            #endregion

                            #region ACS
                            else if (testcaseInTesting.Text == "Adjacent Channel Selectivity (ACS)")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                VSG1.AttenuatorLevel = attSetVal;

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetL");
                                string IS1RBOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetL");
                                string IS1RBOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetR");
                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);

                                //string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                //double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                //VSG1.SetTriggerDelay(wsTrigDelay * 1e-3);

                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                wsTrigDelay = wsTrigDelay * 1e-3;
                                VSG1.SetTriggerDelay(wsTrigDelay);


                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                string wsPower = strWSPwr;
                                string isPower = strISPwr;
                                string offset = isOffsetR;  // xem lại giá trị này
                                string isFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1e6) / 2 + (double.Parse(offset) * 1e6));
                                string isFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1e6) / 2 - (double.Parse(offset) * 1e6));






                                //upper
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                if (VSG2 != null)
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = double.Parse(isFreqUpper);
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    VSG2.AttenuatorLevel = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue);
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={VSG2.AttenuatorLevel} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");

                                }

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                Measurement.twoVSGMode(VSG1, VSG2, BBU, "ACS", wsOffsetR, IS1RBOffsetR, stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqUpper, null, null, setBandwidth,wsTrigDelay, out tp);
                                dataACS[testingPort - 1, 0, 2] = tp.ToString();

                                //lower
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                if (VSG2 != null)
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = double.Parse(isFreqLower);
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    VSG2.AttenuatorLevel = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue);
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={VSG2.AttenuatorLevel} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");
                                }

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                Measurement.twoVSGMode(VSG1, VSG2, BBU, "ACS", wsOffsetL, IS1RBOffsetL, stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqLower, null, null, setBandwidth, wsTrigDelay, out tp);
                                dataACS[testingPort - 1, 1, 2] = tp.ToString();

                                if (double.Parse(dataACS[testingPort - 1, 0, 2]) > double.Parse(dataACS[testingPort - 1, 1, 2]))
                                {
                                    tp = double.Parse(dataACS[testingPort - 1, 1, 2]);
                                }
                                else
                                {
                                    tp = double.Parse(dataACS[testingPort - 1, 0, 2]);
                                }
                                if (tp >= 95)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                result = strISPwr;
                                mode = tp.ToString();
                                dataACS[testingPort - 1, 0, 0] = PassFail;  // 0 Result
                                dataACS[testingPort - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataACS[testingPort - 1, 0, 2] = tp.ToString();     // 3 Reading Value
                                dataACS[testingPort - 1, 0, 3] = result;     // 4 Reading Value


                            }
                            #endregion
                            #region INB
                            else if (testcaseInTesting.Text == "Inband Blocking")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                VSG1.AttenuatorLevel = attSetVal;

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetL");
                                string IS1RBOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetL");
                                string IS1RBOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetR");
                                string deltaFoobMHz = ReadValueByHeader(testcaseInTesting.Parameters, "deltaFoobMHz");

                                //17042026
                                string ISSweepRange = ReadValueByHeader(testcaseInTesting.Parameters, "ISSweepRange(MHz)");
                                string SweepStep = ReadValueByHeader(testcaseInTesting.Parameters, "SweepStep(MHz)");

                                string[] SweepRange = ISSweepRange.Split('-');
                                ///
                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);

                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                VSG1.SetTriggerDelay(wsTrigDelay * 1e-3);

                                string wsPower = strWSPwr;
                                string isPower = strISPwr;
                                //double.TryParse(deltaFoob.Text, out double delta_Foob); //in MHz
                                //delta_Foob *= 1e6; //MHz to Hz
                                double.TryParse(deltaFoobMHz, out double delta_Foob); //in MHz
                                delta_Foob *= 1e6; //MHz to Hz
                                Log($"Delta FOOB: {deltaFoobMHz} MHz");
                                double.TryParse(setFrequency, out double tmpFc);  //in Hz                                  
                                                                                  //define temp variables to store the worst case
                                //double isFrequencyMinL = 0;
                                //double isFrequencyMinR = 0;
                                //double isThroughputMinL = 110;
                                //double isThroughputMinR = 110;
                                //double.TryParse(SweepStep, out double isFreqSweep); //sweep 1MHz
                                double isFreqSweep = double.Parse(SweepStep) * 1e6;

                                //Left side
                                //WS set RbOffset = 223 => TBD
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                double isLowerOffset = double.Parse(isOffsetL) * 1e6; //=30MHz = 30*1e6 Hz
                                double isUpperOffset = double.Parse(isOffsetR) * 1e6;
                                
                                double isFreqLowerTo = (double.Parse(setFrequency) - double.Parse(setBandwidth) * 1e6 / 2 - isLowerOffset)/1e6;
                               double isFreqLowerFrom = txtOperatingFreqFrom - delta_Foob/1e6;
                          
                                double isFreqUpperFrom = (double.Parse(setFrequency) + double.Parse(setBandwidth) * 1e6 / 2 + isUpperOffset)/1e6;
                                double isFreqUpperTo = txtOperatingFreqTo + delta_Foob/1e6;
                                
                                double isFreqStart = isFreqLowerFrom;
                                string wsRbOffset = wsOffsetL;
                                string isRbOffset = IS1RBOffsetL;


                                if (VSG2 != null)
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = isFreqLowerFrom;
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    VSG2.AttenuatorLevel = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue*1e6);
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={VSG2.AttenuatorLevel} dB at f={VSG2.SetFrequencyValue }MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");

                                }
                                Measurement.twoVSGMode(VSG1, VSG2, BBU, "INB2", wsRbOffset, isRbOffset, isFreqSweep, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqStart.ToString(), isFreqLowerTo.ToString(), isFreqUpperFrom.ToString(), setBandwidth,wsTrigDelay,out tp);
                                if (tp >= 95 && tp <= 100)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    //reTest = true;
                                }
                                result = strISPwr;
                                mode = tp.ToString();
                                dataINB[testingPort - 1, 0, 0] = PassFail;  // 0 Result
                                dataINB[testingPort - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataINB[testingPort - 1, 0, 2] = tp.ToString();     // 3 Reading Value
                                dataINB[testingPort - 1, 0, 3] = result;     // 4 Reading Value


                            }
                            #endregion
                            #region NBB
                            else if (testcaseInTesting.Text == "Narrow band blocking")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                VSG1.AttenuatorLevel = attSetVal;

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetL");
                                string IS1RBOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetL");
                                string IS1RBOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetR");
                                string deltaFoobMHz = ReadValueByHeader(testcaseInTesting.Parameters, "deltaFoobMHz");

                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                VSG1.SetTriggerDelay(wsTrigDelay * 1e-3);

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);


                                string wsPower = strWSPwr;
                                string isPower = strISPwr;
                                string isLowerOffset = isOffsetL;
                                string isUpperOffset = isOffsetR;

                                string isFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1e6) / 2 + (double.Parse(isUpperOffset) * 1e6));
                                string isFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1e6) / 2 - (double.Parse(isLowerOffset) * 1e6));



                                //19.2.2024 ====== sua lai bai NBB
                                //IS on right side
                                string wsRbOffset = wsOffsetR;
                                string[] arrRBoffsetR = IS1RBOffsetR.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                double minTP = 101;

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                if (VSG2 != null)
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = double.Parse(isFreqUpper);
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    VSG2.AttenuatorLevel = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue);
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={VSG2.AttenuatorLevel} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");
                                }
                                if (arrRBoffsetR.Length > 0)
                                {
                                    for (int step1NBB = 0; step1NBB < arrRBoffsetR.Length; step1NBB++)
                                    {
                                        if (measurementBackgroundWorker.CancellationPending) return;
                                        LogDashedLine();
                                        System.Threading.Thread.Sleep(1);
                                        Log($"Set IS RBOffset at position:{arrRBoffsetR[step1NBB]}");
                                        Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", wsRbOffset, arrRBoffsetR[step1NBB], stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqUpper, null, null, setBandwidth,wsTrigDelay, out tp);
                                        minTP = Math.Min(minTP, tp);
                                    }
                                }
                                else
                                {
                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", wsRbOffset, "0", stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqUpper, null, null, setBandwidth,wsTrigDelay, out tp);
                                    minTP = Math.Min(minTP, tp);
                                }

                                //Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", measClause[i, 6], "0", double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqUpper, null, null, out tp);
                                //dataNBB[j, 0, 2] = tp.ToString();
                                dataNBB[testingPort - 1, 0, 2] = minTP.ToString();

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                //19.2.2024 ====== sua lai bai NBB
                                //IS on left side
                                wsRbOffset = wsOffsetL;
                                string[] arrRBoffsetL = IS1RBOffsetL.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                minTP = 101;
                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                if (VSG2 != null)
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = double.Parse(isFreqLower);
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    VSG2.AttenuatorLevel = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue);
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={VSG2.AttenuatorLevel} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");
                                }
                                if (arrRBoffsetL.Length > 0)
                                {
                                    for (int step2NBB = 0; step2NBB < arrRBoffsetL.Length; step2NBB++)
                                    {
                                        if (measurementBackgroundWorker.CancellationPending) return;
                                        LogDashedLine();
                                        System.Threading.Thread.Sleep(1);
                                        Log($"Set IS RBOffset at position:{arrRBoffsetL[step2NBB]}");
                                        Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", wsRbOffset, arrRBoffsetL[step2NBB], stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqLower, null, null, setBandwidth,wsTrigDelay, out tp);
                                        minTP = Math.Min(minTP, tp);
                                    }
                                }
                                else
                                {
                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", wsRbOffset, "0", stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqLower, null, null, setBandwidth,wsTrigDelay, out tp);
                                    minTP = Math.Min(minTP, tp);
                                }

                                //Measurement.twoVSGMode(VSG1, VSG2, BBU, "NBB", measClause[i, 6], "99", double.Parse(measClause[i, 7]), arrAttUL[j], (j + 1), wsPower, setFrequency, isPower, isFreqLower, null, null, out tp);
                                //dataNBB[j, 1, 2] = tp.ToString();
                                dataNBB[testingPort - 1, 1, 2] = minTP.ToString();

                                //lay gia tri xau nhat
                                tp = Math.Min(double.Parse(dataNBB[testingPort - 1, 0, 2]), double.Parse(dataNBB[testingPort - 1, 1, 2]));

                                if (tp >= 95)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                result = strISPwr;
                                mode = tp.ToString();
                                dataNBB[testingPort - 1, 0, 0] = PassFail;  // 0 Result
                                dataNBB[testingPort - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataNBB[testingPort - 1, 0, 2] = tp.ToString();     // 3 Reading Value
                                dataNBB[testingPort - 1, 0, 3] = result;     // 4 Reading Value
                                //                    }



                            }
                            #endregion
                            #region OOB
                            else if (testcaseInTesting.Text == "Out-of-band Blocking")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                VSG1.AttenuatorLevel = attSetVal;

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetL");
                                string IS1RBOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetL");
                                string IS1RBOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetR");
                                string deltaFoobMHz = ReadValueByHeader(testcaseInTesting.Parameters, "deltaFoobMHz");

                                string SweepStep = ReadValueByHeader(testcaseInTesting.Parameters, "SweepStep(MHz)");

                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                //double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                //VSG1.SetTriggerDelay(wsTrigDelay * 1e-3);

                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                wsTrigDelay = wsTrigDelay * 1e-3;
                                VSG1.SetTriggerDelay(wsTrigDelay);

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);

                                testCase = "Uplink";
                                specName = "OBB";

                                //code cu
                                string wsPower = strWSPwr;
                                string isPower = strISPwr;
                                string offset = isOffsetR;
                                string isFreqUpper = Convert.ToString(RRU.ChannelBWTop + (double.Parse(offset) * 1e6)); // band n41 2496 – 2690
                                string isFreqLower = Convert.ToString(RRU.ChannelBWBottom - (double.Parse(offset) * 1e6));


                                double isFreq = 1e6; //OOBB - CW start at 1MHz
                                double stepOOBB = double.Parse(SweepStep) * 1e6; //step sweep CW signal _1MHz 

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                if (chkOOB3.Checked)
                                {
                                    deltaFoobValue = double.Parse(deltaFoob.Text) * 1e6; //in Hz
                                    Log($"Set CW Signal delta_Foob = {deltaFoobValue / 1e6} MHz");

                                    double.TryParse(txtOOB1.Text, out double tmpLowerFreq);
                                    double.TryParse(txtOOB2.Text, out double tmpUpperFreq);

                                    string isFreqLowerSpec = Convert.ToString(tmpLowerFreq); // MHz
                                    string isFreqUpperSpec = Convert.ToString(tmpUpperFreq); // MHz
                                    Log($"Set CW Signal frequency range from {tmpLowerFreq} MHz to {tmpUpperFreq} MHz");

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    Measurement.twoVSGMode(VSG1, VSG2, BBU, "OOB2", wsOffsetR, IS1RBOffsetR, stepOOBB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreq.ToString(), isFreqLowerSpec, isFreqUpperSpec, setBandwidth,wsTrigDelay, out tp);
                                }
                                else
                                {
                                    Log("ERROR: You don't config these parameters for Out-of-band", LogLevel.WARN);
                                }

                                if (tp >= 95)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    //reTest = true;
                                }
                                result = strISPwr;
                                mode = tp.ToString();
                                dataOBB[testingPort - 1, 0, 0] = PassFail;  // 0 Result
                                dataOBB[testingPort - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataOBB[testingPort - 1, 0, 2] = tp.ToString();     // 3 Reading Value
                                dataOBB[testingPort - 1, 0, 3] = result;     // 4 Reading Value



                            }
                            #endregion

                            #region OOB-CoL
                            else if (testcaseInTesting.Text == "Out-of-band Blocking Co-location")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                VSG1.AttenuatorLevel = attSetVal;

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetL");
                                string IS1RBOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetL");
                                string IS1RBOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetR");
                                string deltaFoobMHz = ReadValueByHeader(testcaseInTesting.Parameters, "deltaFoobMHz");

                                string SweepStep = ReadValueByHeader(testcaseInTesting.Parameters, "SweepStep(MHz)");

                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                //double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                //VSG1.SetTriggerDelay(wsTrigDelay * 1e-3);

                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                wsTrigDelay = wsTrigDelay * 1e-3;
                                VSG1.SetTriggerDelay(wsTrigDelay);

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);

                                testCase = "Uplink";
                                specName = "OBB";

                                //code cu
                                string wsPower = strWSPwr;
                                string isPower = strISPwr;
                                string offset = isOffsetR;
                                string isFreqUpper = Convert.ToString(RRU.ChannelBWTop + (double.Parse(offset) * 1e6)); // band n41 2496 – 2690
                                string isFreqLower = Convert.ToString(RRU.ChannelBWBottom - (double.Parse(offset) * 1e6));


                                double isFreq = 1e6; //OOBB - CW start at 1MHz
                                double stepOOBB = double.Parse(SweepStep) * 1e6; //step sweep CW signal _1MHz 

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                if (chkOOB3.Checked)
                                {
                                    deltaFoobValue = double.Parse(deltaFoob.Text) * 1e6; //in Hz
                                    Log($"Set CW Signal delta_Foob = {deltaFoobValue / 1e6} MHz");

                                    double.TryParse(txtOOB1.Text, out double tmpLowerFreq);
                                    double.TryParse(txtOOB2.Text, out double tmpUpperFreq);

                                    string isFreqLowerSpec = Convert.ToString(tmpLowerFreq); // MHz
                                    string isFreqUpperSpec = Convert.ToString(tmpUpperFreq); // MHz
                                    Log($"Set CW Signal frequency range from {tmpLowerFreq} MHz to {tmpUpperFreq} MHz");

                                    if (measurementBackgroundWorker.CancellationPending) return;
                                    System.Threading.Thread.Sleep(1);
                                    Measurement.twoVSGMode(VSG1, VSG2, BBU, "OOB1", wsOffsetR, IS1RBOffsetR, stepOOBB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreq.ToString(), isFreqLowerSpec, isFreqUpperSpec, setBandwidth, wsTrigDelay, out tp);
                                }
                                else
                                {
                                    Log("ERROR: You don't config these parameters for Out-of-band", LogLevel.WARN);
                                }

                                if (tp >= 95)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    //reTest = true;
                                }
                                result = strISPwr;
                                mode = tp.ToString();
                                dataOBB[testingPort - 1, 0, 0] = PassFail;  // 0 Result
                                dataOBB[testingPort - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataOBB[testingPort - 1, 0, 2] = tp.ToString();     // 3 Reading Value
                                dataOBB[testingPort - 1, 0, 3] = result;     // 4 Reading Value



                            }
                            #endregion
                            #region RSE
                            else if (testcaseInTesting.Text == "Receiver spurious emission")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetL");
                                string IS1RBOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetL");
                                string IS1RBOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetR");
                                string deltaFoobMHz = ReadValueByHeader(testcaseInTesting.Parameters, "deltaFoobMHz");


                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);


                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                NRTM = "NRTM_11";
                                lblStatusProgress_TextChanged($"{testcaseInTesting.Text} Port {testingPort}");
                                MessageBox.Show("Đổi sang Port" + testingPort, "Thông báo");
                                BBU.TDDMODE("RX_ONLY");
                                Measurement.Spur5G(VSA, BBU, "Rx-Spurious", NRTM, attSetVal.ToString(), testingPort, setFrequency, setBandwidth, "", out tseRS);
                                //==============19.2.2024 save screen shot =====================
                                //TakeScreenshotVSA(VSA, "RX", "Rx-Spurious", $"P{testingPort}", setFrequency);
                                nRS = -100;
                                for (int i1 = 0; i1 < tseRS.Length; i1++)
                                {
                                    if (nRS < tseRS[i1])
                                    {
                                        nRS = tseRS[i1];
                                    }
                                }
                                if (nRS > limitMax)
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                else
                                {
                                    PassFail = pass;
                                    result = Math.Round((double)nRS, 1).ToString();
                                    Console.WriteLine("Ket qua cuoi cung  " + result);
                                }

                            }
                            #endregion
                            #region RX-IMD-G
                            else if (testcaseInTesting.Text == "General intermodulation")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                VSG1.AttenuatorLevel = attSetVal;

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetL");
                                string IS1RBOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetL");
                                string IS1RBOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetR");
                                string deltaFoobMHz = ReadValueByHeader(testcaseInTesting.Parameters, "deltaFoobMHz");
                                string isCWOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISCWOffsetL");
                                string isCWOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISCWOffsetR");

                      

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);

                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                VSG1.SetTriggerDelay(wsTrigDelay * 1e-3);


                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                string wsPower = strWSPwr;
                                string isPower = strISPwr;
                                string isLowerOffset = isOffsetL;
                                string isUpperOffset = isOffsetR;

                                string isFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1000000) / 2 - (double.Parse(isLowerOffset) * 1000000));
                                string isFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1000000) / 2 + (double.Parse(isUpperOffset) * 1000000));
                                string cwLowerOffset = isCWOffsetL;
                                string cwUpperOffset = isCWOffsetR;
                                string cwFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1000000) / 2 - (double.Parse(cwLowerOffset) * 1000000));
                                string cwFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1000000) / 2 + (double.Parse(cwUpperOffset) * 1000000));

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                if ((VSG2 != null) && (VSG3 != null))
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = double.Parse(isFreqUpper);
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    VSG2.AttenuatorLevel = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue);
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={VSG2.AttenuatorLevel} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");

                                    VSG3.Alias = "CW";
                                    VSG3.SetFrequencyValue = double.Parse(cwFreqUpper);
                                    string pathCW = $@"{isCSVFilePathRX}\CWRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double cwfreq);
                                    double cwAttSetVal = VSG3.ReadATTfromCSVFile(path, VSG3.SetFrequencyValue);
                                    VSG3.AttenuatorLevel = cwAttSetVal;
                                    Log($"{VSG3.Alias}: Set ATT_Port{testingPort}={cwAttSetVal} dB at f={VSG3.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG3.Alias}: CSV Path:{VSG3.AttenuatorCSVpath}");

                                }

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                Measurement.threeVSGMode(VSG1, VSG2, VSG3, BBU, "GenIMD", wsOffsetR, IS1RBOffsetR, stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqUpper, cwFreqUpper, setBandwidth, out tp);
                                dataRXIMD[0, testingPort - 1, 0, 2] = tp.ToString();

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                if ((VSG2 != null) && (VSG3 != null))
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = double.Parse(isFreqLower);
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    VSG2.AttenuatorLevel = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue);
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={VSG2.AttenuatorLevel} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");

                                    VSG3.Alias = "CW";
                                    VSG3.SetFrequencyValue = double.Parse(cwFreqLower);
                                    string pathCW = $@"{isCSVFilePathRX}\CWRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double cwfreq);
                                    double cwAttSetVal = VSG3.ReadATTfromCSVFile(path, VSG3.SetFrequencyValue);
                                    VSG3.AttenuatorLevel = cwAttSetVal;
                                    Log($"{VSG3.Alias}: Set ATT_Port{testingPort}={cwAttSetVal} dB at f={VSG3.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG3.Alias}: CSV Path:{VSG3.AttenuatorCSVpath}");

                                }

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                Measurement.threeVSGMode(VSG1, VSG2, VSG3, BBU, "GenIMD", wsOffsetL, IS1RBOffsetL, stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqLower, cwFreqLower, setBandwidth, out tp);
                                dataRXIMD[0, testingPort - 1, 1, 2] = tp.ToString();

                                if (double.Parse(dataRXIMD[0, testingPort - 1, 0, 2]) > double.Parse(dataRXIMD[0, testingPort - 1, 1, 2]))
                                {
                                    tp = double.Parse(dataRXIMD[0, testingPort - 1, 1, 2]);
                                }
                                else
                                {
                                    tp = double.Parse(dataRXIMD[0, testingPort - 1, 0, 2]);
                                }
                                if (tp >= 95)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                result = strISPwr;
                                mode = tp.ToString();
                                dataRXIMD[0, testingPort - 1, 0, 0] = PassFail;  // 0 Result
                                dataRXIMD[0, testingPort - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataRXIMD[0, testingPort - 1, 0, 2] = tp.ToString();     // 3 Reading Value
                                dataRXIMD[0, testingPort - 1, 0, 3] = result;     // 4 Reading Value


                            }
                            #endregion
                            #region RX-IMD-NBB
                            else if (testcaseInTesting.Text == "Narrow-band intermodulation")
                            {
                                VSG1.SetFrequencyValue = double.Parse(setFrequency);
                                GeneralDataSetupRX(testcaseInTesting.Text, testingPort, out attSetVal);
                                VSG1.AttenuatorLevel = attSetVal;

                                string strLimMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                string strLimMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                string strWSPwr = ReadValueByHeader(testcaseInTesting.Parameters, "WS");
                                string strISPwr = ReadValueByHeader(testcaseInTesting.Parameters, "IS");
                                string strRepeat = ReadValueByHeader(testcaseInTesting.Parameters, "repeatNumber");
                                string strStep = ReadValueByHeader(testcaseInTesting.Parameters, "Step (dB)");
                                string wsOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetL");
                                string isOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetR");
                                string wsOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "WSOffsetR");
                                string isOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISNROffsetL");
                                string IS1RBOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetL");
                                string IS1RBOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "IS1RBOffsetR");
                                string deltaFoobMHz = ReadValueByHeader(testcaseInTesting.Parameters, "deltaFoobMHz");
                                string isCWOffsetL = ReadValueByHeader(testcaseInTesting.Parameters, "ISCWOffsetL");
                                string isCWOffsetR = ReadValueByHeader(testcaseInTesting.Parameters, "ISCWOffsetR");

                                string strWSTrigDelay = ReadValueByHeader(testcaseInTesting.Parameters, "TriggerDelay(ms)");
                                double.TryParse(strWSTrigDelay, out double wsTrigDelay);
                                VSG1.SetTriggerDelay(wsTrigDelay * 1e-3);

                                double.TryParse(strLimMax, out limitMax);
                                double.TryParse(strLimMin, out limitMin);
                                int.TryParse(strRepeat, out int repeat);
                                double.TryParse(strStep, out double stepdB);


                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);


                                string wsPower = strWSPwr;
                                string isPower = strISPwr;
                                //string isPowerUpper = measClause[i, 10];
                                string isLowerOffset = isOffsetL;
                                string isUpperOffset = isOffsetR;

                                string isFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1000000) / 2 - (double.Parse(isLowerOffset) * 1000000));
                                string isFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1000000) / 2 + (double.Parse(isUpperOffset) * 1000000));
                                string cwLowerOffset = isCWOffsetL;
                                string cwUpperOffset = isCWOffsetR;
                                string cwFreqLower = Convert.ToString(double.Parse(setFrequency) - (double.Parse(setBandwidth) * 1000000) / 2 - (double.Parse(cwLowerOffset) * 1000000));
                                string cwFreqUpper = Convert.ToString(double.Parse(setFrequency) + (double.Parse(setBandwidth) * 1000000) / 2 + (double.Parse(cwUpperOffset) * 1000000));

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                if ((VSG2 != null) && (VSG3 != null))
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = double.Parse(isFreqUpper);
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    double isAttSetVal = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue);
                                    VSG2.AttenuatorLevel = isAttSetVal;
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={isAttSetVal} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");

                                    VSG3.Alias = "CW";
                                    VSG3.SetFrequencyValue = double.Parse(cwFreqUpper);
                                    string pathCW = $@"{isCSVFilePathRX}\CWRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double cwfreq);
                                    double cwAttSetVal = VSG3.ReadATTfromCSVFile(path, VSG3.SetFrequencyValue);
                                    Log($"{VSG3.Alias}: Set ATT_Port{testingPort}={cwAttSetVal} dB at f={VSG3.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG3.Alias}: CSV Path:{VSG3.AttenuatorCSVpath}");

                                }

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                Measurement.threeVSGMode(VSG1, VSG2, VSG3, BBU, "NBIMD", wsOffsetR, IS1RBOffsetR, stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqUpper, cwFreqUpper, setBandwidth, out tp);
                                dataRXIMD[1, testingPort - 1, 0, 2] = tp.ToString();

                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);

                                if ((VSG2 != null) && (VSG3 != null))
                                {
                                    VSG2.Alias = "IS";
                                    VSG2.SetFrequencyValue = double.Parse(isFreqLower);
                                    string path = $@"{isCSVFilePathRX}\ISRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double freq);
                                    double isAttSetVal = VSG2.ReadATTfromCSVFile(path, VSG2.SetFrequencyValue);
                                    VSG2.AttenuatorLevel = isAttSetVal;
                                    Log($"{VSG2.Alias}: Set ATT_Port{testingPort}={isAttSetVal} dB at f={VSG2.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG2.Alias}: CSV Path:{VSG2.AttenuatorCSVpath}");

                                    VSG3.Alias = "CW";
                                    VSG3.SetFrequencyValue = double.Parse(cwFreqUpper);
                                    string pathCW = $@"{isCSVFilePathRX}\CWRX{testingPort}.csv";
                                    double.TryParse(setFrequency, out double cwfreq);
                                    double cwAttSetVal = VSG3.ReadATTfromCSVFile(path, VSG3.SetFrequencyValue);
                                    VSG3.AttenuatorLevel = cwAttSetVal;
                                    Log($"{VSG3.Alias}: Set ATT_Port{testingPort}={cwAttSetVal} dB at f={VSG3.SetFrequencyValue / 1e6}MHz");
                                    Log($"{VSG3.Alias}: CSV Path:{VSG3.AttenuatorCSVpath}");

                                }


                                if (measurementBackgroundWorker.CancellationPending) return;
                                System.Threading.Thread.Sleep(1);
                                Measurement.threeVSGMode(VSG1, VSG2, VSG3, BBU, "NBIMD", wsOffsetL, IS1RBOffsetL, stepdB, attSetVal.ToString(), testingPort, wsPower, setFrequency, isPower, isFreqLower, cwFreqLower, setBandwidth, out tp);
                                dataRXIMD[1, testingPort - 1, 1, 2] = tp.ToString();

                                if (double.Parse(dataRXIMD[1, testingPort - 1, 0, 2]) > double.Parse(dataRXIMD[1, testingPort - 1, 1, 2]))
                                {
                                    tp = double.Parse(dataRXIMD[1, testingPort - 1, 1, 2]);
                                }
                                else
                                {
                                    tp = double.Parse(dataRXIMD[1, testingPort - 1, 0, 2]);
                                }
                                if (tp >= 95)
                                {
                                    PassFail = pass;
                                }
                                else
                                {
                                    PassFail = fail;
                                    reTest = true;
                                }
                                result = strISPwr;
                                mode = tp.ToString();
                                dataRXIMD[1, testingPort - 1, 0, 0] = PassFail;  // 0 Result
                                dataRXIMD[1, testingPort - 1, 0, 1] = DateTime.Now.ToString("hh:mm:ss");  // 2 Testing Time
                                dataRXIMD[1, testingPort - 1, 0, 2] = tp.ToString();     // 3 Reading Value
                                dataRXIMD[1, testingPort - 1, 0, 3] = result;     // 4 Reading Value

                            }
                            #endregion
                            #endregion

                            //TODO: when fail =================
                            if (reTest)
                            {
                                timeout++;
                                if (timeout == loopTime) //Fail after n times
                                {
                                    //TODO: add data to chart 21.2.2024
                                    //if (!testcaseInTesting.Text.Contains("Blocking"))
                                    //{
                                    //    if (testCase == "Downlink")
                                    //    {
                                    //        PopulateMeasurementChart(testcaseInTesting.Text, $"PORT{testingPort}", result, limitMin.ToString("N3"), limitMax.ToString("N3"));
                                    //    }
                                    //    else if (testCase == "Uplink")
                                    //    {
                                    //        PopulateMeasurementChart(testcaseInTesting.Text, $"PORT{testingPort}", tp.ToString(), "95", "100");
                                    //    }

                                    //}
                                    //counting completed task
                                    isTestcasePass = false;
                                    completedTask++;
                                    timeout = 0;
                                    failedPorts.Add(testingPort);
                                    if (testCase == "Downlink")
                                    {
                                        TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency, PassFail);
                                    }
                                    //ChangeTestcaseNodeStatus(testcaseInTesting.Text, TestcaseStatus.Failed);
                                    ////add result to list view
                                    //string strSpec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                    //string strMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                    //string strMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                    //ListViewShow("PORT" + testingPort, testcaseInTesting.Text, strSpec, strMin, result, strMax, mode, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                                    ////store Failed Testcases and ports

                                    break;
                                }

                                //delete fail result in listview()
                                //deleteListView();
                            }
                            else
                            {
                                //TODO: add data to chart 21.2.2024
                                //if (!testcaseInTesting.Text.Contains("Blocking"))
                                //{
                                //    PopulateMeasurementChart(testcaseInTesting.Text, $"PORT{testingPort}", result, limitMin.ToString("N3"), limitMax.ToString("N3"));
                                //}
                                completedTask++;
                                timeout = 0;
                                if (testCase == "Downlink" && !testcaseInTesting.Text.Contains("Frequency error")) //TODO: tam thoi ngat phan Frequency error 
                                {
                                    TakeScreenshotVSA(VSA, "TX", testcaseInTesting.Text, "P" + testingPort, setFrequency, PassFail);
                                }
                                //ChangeTestcaseNodeStatus(testcaseInTesting.Text, TestcaseStatus.Passed);
                                ////add result to list view
                                //string strSpec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                                //string strMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                                //string strMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                                //ListViewShow("PORT" + testingPort, testcaseInTesting.Text, strSpec, strMin, result, strMax, mode, PassFail, Math.Ceiling((DateTime.Now - timeStep).TotalSeconds).ToString());
                                break;

                            }
                        }
                        //TODO: After a port measurement finish.
                        UpdateProgressBar(completedTask);
                        //add to Result listview
                        string strSpec = ReadValueByHeader(testcaseInTesting.Parameters, "Spec");
                        string strMin = ReadValueByHeader(testcaseInTesting.Parameters, "Min");
                        string strMax = ReadValueByHeader(testcaseInTesting.Parameters, "Max");
                        TimeSpan tactTime = DateTime.Now - startTime;
                        if (testcaseInTesting.Text != "RX Power Calibration")
                        {
                            ListViewShow("PORT" + testingPort, testcaseInTesting.Text, strSpec, strMin, result, strMax, mode, PassFail, tactTime.ToString(@"hh\:mm\:ss"));
                        }
                        if (testcaseInTesting.Text == "Out-of-band Blocking" ||
                            testcaseInTesting.Text == "Out-of-band Blocking Co-location")
                        {
                            portResult.Values["PORT"] = "PORT" + testingPort;
                            portResult.Values["CLAUSE"] = testcaseInTesting.Text;
                            portResult.Values["UNIT"] = "%";
                            portResult.Values["CONFIG"] = "CW";
                            portResult.Values["PARAM"] = "RX Throughput";
                            portResult.Values["SPECS"] = ">= 95%";
                            portResult.Values["RESULT"] = tp.ToString(
                                System.Globalization.CultureInfo.InvariantCulture);
                            portResult.Values["TEST TIME"] = DateTime.Now.ToString("HH:mm:ss");
                            portResult.Values["PASS/FAIL"] = PassFail;
                        }
                        //add to report                        
                        portResult.Values.Add("TACT TIME", tactTime.ToString(@"hh\:mm\:ss"));
                        _reportManager.AddResult(portResult);
                        _reportEngine.AppendCsvRow(_csvSession, _reportManager.CurrentTestCase.Headers, portResult.Values);

                    }

                    //TODO: After Testcase measurement finish, If there are any failed ports, add them to the dictionary
                    if (failedPorts.Count > 0)
                    {
                        failedTestList[testcaseInTesting.Text] = failedPorts;
                        ChangeTestcaseNodeStatus(testcaseInTesting.Text, TestcaseStatus.Failed);
                    }
                    else
                    {
                        ChangeTestcaseNodeStatus(testcaseInTesting.Text, TestcaseStatus.Passed);
                    }
                    //refreshing TestcasePending List for 
                    TestcasePending = GetTestcasesByStatus(TestcaseNodes, TestcaseStatus.Pending);
                    testcaseInTesting = TestcasePending.Count > 0 ? TestcasePending[0] : null;

                    //report
                    TestCaseResult tc = _reportManager.EndTestCase();
                    _reportEngine.EndCsvTestCase(_csvSession, reportFolder, _reportManager.Report.Summary.SerialNumber, tc);
                    _csvSession = null;
                }
            }
            //TODO: After all testcases measurement fisnish, print table fail port lists
            if (failedTestList.Count > 0)
            {
                Log("Failed Measurement List:");
                List<string> headers = new List<string> { "Specification Name", "Failed Port List" };
                log.PrintDictionary(headers, failedTestList);
            }

            clearFlag = true;
            ////dung phat 
            //VSG1.OnOffSignal(0);
            //VSG2.OnOffSignal(0);
            //VSG3.OnOffSignal(0);
            //BBU.StopProcess();
            //measurementBackgroundWorker.CancelAsync();
            //endingTime = lblRuntime.Text;
            //lblStatusProgress_TextChanged("Đang lưu File kết quả");
            //timerProcess.Stop();
            //ExporttoHtml();
            //ExportListView();
            ////lblStatusProgress_TextChanged("Quá trình đo đã kết thúc");
            //txtSerial_TextChanged("");
            //btnConnect_Enable();
        }
        #endregion

        #region measurementBackgroundWorker_Completed
        //TODO: Measurement BackgroundWorker_Completed
        public void measurementBackgroundWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Log("Measurement Canceled!", LogLevel.WARN);
                lblPassFail.Text = "STOP";
                lblPassFail.ForeColor = Color.Blue;
            }
            else if (e.Error != null)
            {
                Log($"An Error occured: {e.Error.Message}", LogLevel.ERROR);
                lblPassFail.Text = "ERROR";
                lblPassFail.ForeColor = Color.Red;
            }
            else
            {
                Log("Measurement Completed!", LogLevel.SUCCESS);

            }

            //dung phat 
            VSG1.OnOffSignal(0);
            VSG2.OnOffSignal(0);
            VSG3.OnOffSignal(0);
            BBU.StopProcess();
            measurementBackgroundWorker.CancelAsync();
            endingTime = lblRuntime.Text;
            lblStatusProgress_TextChanged("Đang lưu File kết quả");
            timerProcess.Stop();
            //ExporttoHtml();
            //ExportListView();
            //ExportLogToTxt();
            

            lblStatusProgress_TextChanged("Quá trình đo đã kết thúc");
            runningTask.Text = "";
            clearFlag = true;
            enRecordLog = false;

            //export report - HTML/CSV/EXCEL
            _reportManager.EndReport();
            _reportEngine.ExportAll( _reportManager.Report, _excelTemplatePath, lvwLog);
            CaptureResultWindow(_reportManager.Report.ReportFolder, txtRRUSerial.Text);

            lblPassFail.Text = (_reportManager.Report.Summary.OverallResult == "PASS") ? "PASS" : "FAIL";
            lblPassFail.ForeColor = (lblPassFail.Text == "PASS")? Color.Green : Color.Red;


            //UE
            txtSerial_TextChanged("");
            btnConnect_Enable();
        }
        #endregion
        #region measurementBackgroundWorker_ProgressChanged
        //TODO: Measurement BackgroundWorker_ProgressChanged
        public void measurementBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Check if UserState is an integer (max value)
            if (e.UserState is int max)
            {
                // Ensure max is greater than zero to avoid division by zero
                if (max > 0)
                {
                    lblProgressVal.Text = $"{(e.ProgressPercentage * 100 / max)}%"; // Format to 2 decimal places
                }
            }
            // Check if UserState is a string (status message)
            else if (e.UserState is string state)
            {
                runningTask.Text = state; // Update running task label with status message
            }
            else
            {
                runningTask.Text = "Unknown state"; // Fallback for unexpected types
            }
        }
        #endregion

        #region Process Functions
        //helper spec string
        private string BuildSpecString(string testName, string limitMin, string limitMax)
        {
            bool hasMin = double.TryParse(limitMin, out double minValue);
            bool hasMax = double.TryParse(limitMax, out double maxValue);

            if (hasMin && hasMax)
            {
                return string.Format("{0} ≤ {1} ≤ {2}", limitMin, testName, limitMax);
            }

            if (hasMin)
            {
                return string.Format("{0} ≥ {1}", testName, limitMin);
            }

            if (hasMax)
            {
                return string.Format("{0} ≤ {1}", testName, limitMax);
            }

            return "";
        }
        //default headers
        private static readonly List<string> _defaultHeaders = new List<string>
        {
            "PORT", 
            "CLAUSE", 
            "UNIT",
            "CONFIG",
            "PARAM",
            "SPECS", 
            //"MIN", 
            //"MAX", 
            "RESULT", 
            "TEST TIME",
            "PASS/FAIL", 
            "TACT TIME"
        };
        //Defined headers        

        private readonly Dictionary<string, List<string>> _testCaseHeaders = new Dictionary<string, List<string>>
        {
            // Khai báo header cho từng testcase riêng
            {
                // Operating Band Unwanted Emissions TM1.1
                "OPERATING BAND UNWANTED EMISSIONS TM1.1",
                new List<string>{"PORT", "CLAUSE", "SPECS", "MIN", "MAX", "RANGE_1", "RANGE_1", "RANGE_3", "TEST TIME", "RESULT", "PASS/FAIL", "TACT TIME"}
            },
            {
                // Operating Band Unwanted Emissions TM1.2
                "OPERATING BAND UNWANTED EMISSIONS TM1.2",
                new List<string>{"PORT", "CLAUSE", "SPECS", "MIN", "MAX", "RANGE_1", "RANGE_1", "RANGE_3", "TEST TIME", "RESULT", "PASS/FAIL", "TACT TIME"}
            },
            {
                // Total power dynamic range
                "TOTAL POWER DYNAMIC RANGE",
                new List<string>{"PORT", "CLAUSE", "SPECS", "MIN", "MAX", "PWR_MAX", "PWR_MIN", "TEST TIME", "RESULT", "PASS/FAIL", "TACT TIME"}
            },

        };

        private List<string> GetHeadersFromTestCaseName(string testCaseName)
        {
            List<string> headers;

            return _testCaseHeaders.TryGetValue(testCaseName.ToUpper(), out headers)? headers : _defaultHeaders;
        }

        public void ProgressLabel(string text)
        {
            if (measurementBackgroundWorker.IsBusy)
            {
                measurementBackgroundWorker.ReportProgress(0, text);
            }
            else
            {
                runningTask.Text = text;
            }

        }

        // Loading Progress Bar
        public void prgLoad_ValueChange(int max, int value)
        {
            ProgressBar status = proLoad;
            status.Invoke(new MethodInvoker(delegate ()
            {
                if (value <= max)
                {
                    proLoad.Maximum = max;
                    proLoad.Minimum = 0;
                    proLoad.Value = value;
                }
                else
                {
                    value = 0;
                };
                lblProgressVal.Text = (max > 0) ? $"{value / max}%" : "0%";

            }));

        }

        private void UpdateProgressBar(int value)
        {
            //calculate Progressbar
            List<TestcaseNode> TestcaseNa = GetTestcasesByStatus(TestcaseNodes, TestcaseStatus.Na);
            int progressBarMax = testingPortList.Count * (TestcaseNodes.Count - TestcaseNa.Count);
            prgLoad_ValueChange(progressBarMax, value);
            measurementBackgroundWorker.ReportProgress(value, progressBarMax);

        }
        #endregion

    }
}
