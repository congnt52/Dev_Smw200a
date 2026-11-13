using _5GAutoTool.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Timer = System.Threading.Timer;

namespace _5GAutoTool
{
    class twoVSG
    {
        public Form5GAT mainForm;
        private static Timer timerSequence;
        private static volatile bool flagStep = false;
        private List<string[]> oobCoData = new List<string[]>();
        private List<string[]> oobCoISsweepRanges = new List<string[]>();
        private bool L1TestMacV2Start = false;
        private bool L1TestMacV3Start = false;
        private int portIndex;
        public twoVSG(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string mode = "", old_mode = "";
        public void Reset()
        {
            mode = "";
            old_mode = "";
        }
        //string[] tmpArr = new string[4]; // 16.2.2024 - bo
        string[] tmpArr = null; //16.2.2024 - them moi
        List<string> csvHeader = new List<string> { "DATE", "TIME", "TYPE", "SPEC", "PORT", "WS FREQ", "WS PWR LVL", "WS RBOFS", "IS FREQ", "IS PWR LVL", "IS RBOFS", "THROUGHPUT" };
        PortMapping currPort;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, "", "twoVSG");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        /// <summary>
        /// THỰC HIỆN CÁC BÀI ĐO CHƯỜNG 7 SỬ DỤNG 1 VSG TẠO TÍN HIỆU WANTED VÀ 1 VSG TẠO TÍN HIỆU NHIỄU
        /// Bao gồm các bài đo: ACS, ICS, NBB, INB, OOB
        /// </summary>   
        public void Measurement(VSG vsg1, VSG vsg2, BBU bbu, string mode, string WsRBOffset, string IsRBOffset, double step, string setAtt, int setPort, string wsPowerStr, string wsFreq, string isPowerStr, string isFreq, string isLowerFreqRange, string isUpperFreqRange, string Bandwidth, double TriggerDelay, out double TP)
        {
            TP = -1;
            string throughput = "ERROR";
            string isPower = "";
            //ServerRedhat - finding PortMapping and switch group port
            currPort = mainForm.PortMappings.FirstOrDefault(item => item.Port == setPort);
            //bbu.SwitchGroup(currPort.XRanIndex.ToString());
            //for debugging
            //Log($"Current Testing Port = {currPort.Port}\tXran = {currPort.XRanIndex}\tStream = {currPort.StreamIndex}");

            //double _setAtt = double.Parse(setAtt);
            double _wsPower = double.Parse(wsPowerStr);
            double _isPower = double.Parse(isPowerStr);
            //double _wsATT = vsg1.Attenuators[setPort - 1];
            double _wsATT = vsg1.AttenuatorLevel;
      //      Log($"{vsg1.Name}: Expected Power Level={_wsPower}dBm, ATT_Port{setPort} Level={_wsATT}dB");
            string wsPower = (_wsPower + _wsATT).ToString("N3"); //công suất tai cong output cua VSG1 WS

            if (mode != "OOB2" && mode != "OOB1" && mode != "INB2")
            {
                //double _isATT = vsg2.Attenuators[setPort - 1];
                double _isATT = vsg2.AttenuatorLevel;
          //      Log($"{vsg2.Alias}: Expected Power Level={_isPower}dBm, ATT_Port{setPort} Level={_isATT}dB");
                isPower = (_isPower + _isATT).ToString("N3"); //công suất tai cong output cua VSG2 IS - 3 numerics
            }
            else
            {
                //double _isATT = vsg2.AttenuatorLevel;
                //Log($"{vsg2.Alias}: Expected Power Level={_isPower}dBm, ATT_Port{setPort} Level={_isATT}dB");
                //isPower = (_isPower + _isATT).ToString("N3");
            }

            //doc gia tri suy hao cua VSGs
            //vsg1.ReadATTfromCSVdir();
            //vsg2.ReadAttennuatorVSGfromCSV();

            L1TestMacV2Start = false;
            L1TestMacV3Start = false;

            //string isType = null;
            double _isFreqLimitLeft = mainForm.txtOperatingFreqFrom - mainForm.deltaFoobValue/1000000;
            double _isFreqLimitRight = mainForm.txtOperatingFreqTo + mainForm.deltaFoobValue/1000000;

            double _isFreqFrom = double.Parse(isLowerFreqRange);
            double _isFreqTo = double.Parse(isUpperFreqRange);
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                //Generate wanted signal_Chung cho tất cả các bài
                //vsg1.SetPsenWaveform(wsPower, wsFreq);
                if (vsg1.Model == "SMW200A" || vsg1.Model == "SMBV100B")
                {
                    switch (mode)
                    {
                        case "ICS":
                         //   vsg1.SetICSWaveform(isPower, isFreq, IsRBOffset, Bandwidth,TriggerDelay);
                            vsg1.TypeOfSignal = "DFT-s-OFDM";
                       //     vsg1.setParameter(1, wsPower, wsFreq,TriggerDelay); //smw200a 
                          //  vsg1.setParameter(2, isPower, isFreq,TriggerDelay); //smw200a
                            bbu.NEW_RXSENS(Bandwidth, WsRBOffset, out throughput);
                            //26.2.2024 - sua code
                            writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                            break;
                        case "ACS":
                        //    vsg1.SetACSWaveform(isPower, isFreq, WsRBOffset, Bandwidth, TriggerDelay);
                            vsg1.TypeOfSignal = "DFT-s-OFDM";
                         //   vsg1.setParameter(1, wsPower, wsFreq,TriggerDelay);
                         //   vsg1.setParameter(2, isPower, isFreq, TriggerDelay);
                            bbu.NEW_RXSENS(Bandwidth, WsRBOffset, out throughput);
                            //26.2.2024 - sua code
                            writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                            break;
                        case "OOB2": // General OOB; CW target -15 dBm at DUT reference plane
                            {
                         //       vsg1.SetOOBWaveform(isPower, isFreq, WsRBOffset, Bandwidth, TriggerDelay);
                                vsg1.TypeOfSignal = "DFT-s-OFDM";
                            //    vsg1.setParameter(1, wsPower, wsFreq, TriggerDelay);
                              //  if (vsg1.Model == "SMW200A") vsg1.OffSignal(2);
                                List<string[]> oobMeasRange = new List<string[]>();
                                oobMeasRange.Add(new string[] { "", isLowerFreqRange, isUpperFreqRange });
                                List<string[]> oobExRange = new List<string[]>();
                                // Retain General OOB exclusion for current operating band plus deltaFoob.
                                // OOB2: Operating Band limits from UI, in MHz.
                                double operatingBottomMHz = mainForm.txtOperatingFreqFrom;
                                double operatingTopMHz = mainForm.txtOperatingFreqTo;

                                // deltaFoobValue is stored in Hz.
                                double deltaFoobMHz = mainForm.deltaFoobValue / 1e6;

                                if (operatingBottomMHz <= 0 ||
                                    operatingTopMHz <= operatingBottomMHz ||
                                    double.IsNaN(deltaFoobMHz) ||
                                    double.IsInfinity(deltaFoobMHz) ||
                                    deltaFoobMHz < 0)
                                {
                                    throw new InvalidOperationException(
                                        "[OOB2] Invalid Operating Band or Delta Foob configuration.");
                                }

                                double excludeBottomMHz = operatingBottomMHz - deltaFoobMHz;
                                double excludeTopMHz = operatingTopMHz + deltaFoobMHz;

                                oobExRange.Add(new string[]
                                {
    excludeBottomMHz.ToString(
        System.Globalization.CultureInfo.InvariantCulture),

    excludeTopMHz.ToString(
        System.Globalization.CultureInfo.InvariantCulture)
                                });

                                Log(string.Format(
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    "[OOB2] Operating Band=[{0}, {1}] MHz, DeltaFoob={2} MHz, Exclusion=[{3}, {4}] MHz",
                                    operatingBottomMHz,
                                    operatingTopMHz,
                                    deltaFoobMHz,
                                    excludeBottomMHz,
                                    excludeTopMHz));
                                // Do not automatically subtract Sheet2 co-location DL ranges from General OOB.
                                // Debug OOB2 frequency units and exclusion range.
                                Log(string.Format(
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    "[OOB2 DEBUG] ChannelBWBottom={0}, ChannelBWTop={1}, DeltaFoobRaw={2}, DeltaFoobMHz={3}",
                                    mainForm.RRU.ChannelBWBottom,
                                    mainForm.RRU.ChannelBWTop,
                                    mainForm.deltaFoobValue,
                                    mainForm.deltaFoobValue / 1e6));

                                Log(string.Format(
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    "[OOB2 DEBUG] Exclusion=[{0}, {1}] MHz",
                                    _isFreqLimitLeft,
                                    _isFreqLimitRight));

                                Log("[OOB2] General: expected CW at DUT = -15 dBm; validate existing isPower/UCOR chain.");
                                oobMeasurementInSequence(bbu, vsg2, mode, setPort, step, 800,
                                    wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset,
                                    oobMeasRange, oobExRange, Bandwidth, out TP);
                                break;
                            }

                        case "OOB1": // Co-location Blocking - WA BS, n40
                            {
                                // Keep wanted-signal setup consistent with OOB2.
                                vsg1.SetOOBWaveform(
                                    isPower, isFreq, WsRBOffset, Bandwidth, TriggerDelay);

                                vsg1.TypeOfSignal = "DFT-s-OFDM";
                          //      vsg1.setParameter(1, wsPower, wsFreq, TriggerDelay);

                          //      if (vsg1.Model == "SMW200A")
                            //        vsg1.OffSignal(2);

                                // Read Co-location DL bands from Excel Sheet2.
                                getISCoLocationRangeFromExcel();

                                if (oobCoISsweepRanges == null ||
                                    oobCoISsweepRanges.Count == 0)
                                {
                                    throw new InvalidOperationException(
                                        "[OOB1] No Co-location ranges found in Sheet2.");
                                }

                                // Exclude supported UL n40 and configured Delta Foob.
                                List<string[]> oobCoExRange =
                                    BuildSupportedUlExclusionsMHz();

                                if (oobCoExRange == null ||
                                    oobCoExRange.Count == 0)
                                {
                                    throw new InvalidOperationException(
                                        "[OOB1] Missing n40 UL exclusion.");
                                }



                                // WA Co-location requires verification of +16 dBm
                                // at the DUT reference plane.
                                // Do not infer verification from the test-case value.
                                // Validate OOB1 test configuration.
                                // IS target is +16 dBm for WA Co-location.
                                double oob1TargetPower;

                                bool validPower = double.TryParse(
                                    isPowerStr,
                                    System.Globalization.NumberStyles.Float,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out oob1TargetPower);

                                if (!validPower || Math.Abs(oob1TargetPower - 16.0) > 0.001)
                                {
                                    throw new InvalidOperationException(
                                        "[OOB1] Invalid WA Co-location IS target. " +
                                        "Expected +16 dBm, configured: " + isPowerStr);
                                }

                                if (oobCoISsweepRanges == null ||
                                    oobCoISsweepRanges.Count == 0)
                                {
                                    throw new InvalidOperationException(
                                        "[OOB1] No Co-location DL ranges found in Sheet2.");
                                }

                                Log("[OOB1] Frequency configuration validated.");
                                Log("[OOB1] IS target = +16 dBm.");
                                Log("[OOB1] SGS100A uses the existing OOB2 correction path.");

                                // Reuse the same measurement function as OOB2.
                                Log("[OOB1] WA Co-location: +16 dBm CW at DUT, PREFSENS+6 wanted");

                                oobMeasurementInSequence(
                                    bbu, vsg2, mode, setPort, step, 800,
                                    wsFreq, wsPowerStr, WsRBOffset,
                                    isFreq, isPowerStr, IsRBOffset,
                                    oobCoISsweepRanges, oobCoExRange,
                                    Bandwidth, out TP);

                                break;
                            }

                                case "INB":
                            //vsg2.SetIBBWaveform(isPower, isFreq);
                                    vsg1.SetIBBWaveform(isPower, isFreq, WsRBOffset, Bandwidth, TriggerDelay);
                                    vsg2.TypeOfSignal = "DFT-s-OFDM";
                                    //vsg2.SetIBBWaveform(isPower, isFreq, IsRBOffset, Bandwidth);
                                    vsg1.setParameter(1, wsPower, wsFreq, TriggerDelay); //smw200a
                                    vsg2.setParameter(2, isPower, isFreq, TriggerDelay); //smw200a
                                    bbu.NEW_RXSENS(Bandwidth, WsRBOffset, out throughput);
                                    //26.2.2024 - sua code
                                    writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                                    break;
                                case "INB2":
                            //measuring INB with TM run continuosly in xx (s)
                                    vsg1.SetIBBWaveform(isPower, isFreq, WsRBOffset, Bandwidth, TriggerDelay);
                                    vsg2.TypeOfSignal = "DFT-s-OFDM";
                                   //vsg2.SetIBBWaveform(isPower, isFreq, IsRBOffset, Bandwidth);
                                    vsg1.setParameter(1, wsPower, wsFreq, TriggerDelay); //smw200a                      
                                    vsg2.setParameter(2, isPower, isFreq, TriggerDelay); //smw200a
                                    inbMeasurementInSequence(bbu, vsg2, mode, setPort, step, 10000, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, _isFreqLimitLeft.ToString(), isLowerFreqRange, isUpperFreqRange, _isFreqLimitRight.ToString(), Bandwidth, out TP);
                                    break;
                            #region comment OOB old
                            //                        case "OOB2":
                            //                            //25.3.2024 - viet lai bai do oob                        
                            //                            //neu su dung smw200a la nguon phat ws => tat bo nguon nhieu o port 2
                            //                            //if (vsg1.Model == "SMW200A")
                            //                            //{
                            //                            //    vsg1.OffSignal(2); //off nguon 2
                            //                            //}
                            //                            vsg1.SetOOBWaveform(isPower, isFreq, WsRBOffset, Bandwidth, TriggerDelay);
                            //                            vsg1.TypeOfSignal = "DFT-s-OFDM";
                            //                            vsg1.setParameter(1, wsPower, wsFreq, TriggerDelay);
                            //                          //  vsg1.setParameter(2, isPower, isFreq, TriggerDelay);
                            //                            //measRange
                            //                            List<string[]> oobMeasRange = new List<string[]>();
                            //                            oobMeasRange.Add(new string[] { "", isLowerFreqRange, isUpperFreqRange });
                            //                            //exceptRange
                            //                            List<string[]> oobExRange = new List<string[]>();
                            //                            //add Co-location bands to exception range if check option
                            //                            ////if (mainForm.OOBexcludeColocationRange)
                            //                            ////{
                            //                            ////    getISCoLocationRangeFromExcel();
                            //                            ////    foreach (var item in oobCoISsweepRanges)
                            //                            ////    {
                            //                            ////        oobExRange.Add(new string[] { item[1].ToString(), item[2].ToString() });
                            //                            ////    }
                            //                            ////}
                            //                            ///

                            //                            Log("[OOB2] Exclude Co-location = " +
                            //                                             mainForm.OOBexcludeColocationRange);

                            //                            if (mainForm.OOBexcludeColocationRange)
                            //                            {
                            //                                getISCoLocationRangeFromExcel();

                            //                                Log("[OOB2] Co-location range count = " +
                            //                                    oobCoISsweepRanges.Count);

                            //                                foreach (var item in oobCoISsweepRanges)
                            //                                {
                            //                                    oobExRange.Add(new string[]
                            //                                    {
                            //            item[1].ToString(
                            //                System.Globalization.CultureInfo.InvariantCulture),
                            //            item[2].ToString(
                            //                System.Globalization.CultureInfo.InvariantCulture)
                            //                                    });

                            //                                    Log("[OOB2] Exclude Co-location: [" +
                            //                                        item[1] + ", " + item[2] + "] MHz");
                            //                                }
                            //                            }
                            //                            else
                            //                            {
                            //                                Log("[OOB2] WARNING: Co-location exclusion is disabled!",
                            //                                    LogLevel.WARN);
                            //                            }

                            //                            // add Operating band with delta_fOOB to exception range
                            //                           // oobExRange.Add(new string[] { (_isFreqLimitLeft / 1e6).ToString(), (_isFreqLimitRight / 1e6).ToString() });
                            //                           oobExRange.Add(new string[]
                            //{
                            //    _isFreqLimitLeft.ToString(
                            //        System.Globalization.CultureInfo.InvariantCulture),
                            //    _isFreqLimitRight.ToString(
                            //        System.Globalization.CultureInfo.InvariantCulture)
                            //});
                            //                            //for debugging
                            //                            Log("[Out-of-band Measurement] Reading Excluding Range:");
                            //                            foreach (var range in oobExRange)
                            //                            {
                            //                                Log($"[ {range[0]} MHz, {range[1]} MHz]");
                            //                            }
                            //                            //============
                            //                            oobMeasurementInSequence(bbu, vsg2, mode, setPort, step, 800, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, oobMeasRange, oobExRange, Bandwidth, out TP);
                            //                            break;

                            //                        case "OOB1": //oob co-location
                            //                                     //neu su dung smw200a la nguon phat ws => tat bo nguon nhieu o port 2
                            //                            if (vsg1.Model == "SMW200A")
                            //                            {
                            //                                vsg1.OffSignal(2);
                            //                            }
                            //                            getISCoLocationRangeFromExcel();
                            //                            oobMeasurementInSequence(bbu, vsg2, mode, portIndex, step, 800, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, oobCoISsweepRanges, null, Bandwidth, out TP);
                            //                            break;

                            //                        // Add cases for other modes

                            //                        default:
                            //                            // Handle unsupported mode here.
                            //                            break;
                            //                    }
                            //                    vsg2.OnOffSignal(0);

                            //                }
                            //                else
                            //                {
                            //                    //vsg1.OnOffSignal(1);
                            //                    //vsg2.OnOffSignal(1);
                            //                    //Generate inferrence signal
                            //                    switch (mode)
                            //                    {
                            //                        case "ACS":
                            //                            vsg2.SetACSWaveform(isPower, isFreq, IsRBOffset, Bandwidth,TriggerDelay);
                            //                            vsg2.TypeOfSignal = "DFT-s-OFDM";
                            //                            vsg1.setParameter(1, wsPower, isFreq, TriggerDelay);
                            //                            vsg2.setParameter(2, isPower, isFreq, TriggerDelay);
                            //                            bbu.NEW_RXSENS(Bandwidth, WsRBOffset, out throughput);
                            //                            //26.2.2024 - sua code
                            //                            writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                            //                            break;
                            //                        case "ICS":
                            //                            vsg2.SetICSWaveform(isPower, isFreq, IsRBOffset, Bandwidth,TriggerDelay);
                            //                            vsg2.TypeOfSignal = "DFT-s-OFDM";
                            //                            vsg1.setParameter(1, wsPower, isFreq, TriggerDelay); //smw200a 
                            //                            vsg2.setParameter(2, isPower, isFreq, TriggerDelay); //smw200a
                            //                            bbu.NEW_RXSENS(Bandwidth, WsRBOffset, out throughput);
                            //                            //26.2.2024 - sua code
                            //                            writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                            //                            break;
                            //                        case "INB":
                            //                            //vsg2.SetIBBWaveform(isPower, isFreq);
                            //                            vsg2.TypeOfSignal = "DFT-s-OFDM";
                            //                            vsg2.SetIBBWaveform(isPower, isFreq, IsRBOffset, Bandwidth);
                            //                            vsg1.setParameter(1, wsPower, wsFreq,TriggerDelay); //smw200a
                            //                            vsg2.setParameter(2, isPower, isFreq, TriggerDelay); //smw200a
                            //                            bbu.NEW_RXSENS(Bandwidth, WsRBOffset, out throughput);
                            //                            //26.2.2024 - sua code
                            //                            writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                            //                            break;
                            //                        case "INB2":
                            //                            //measuring INB with TM run continuosly in xx (s)
                            //                            vsg2.TypeOfSignal = "DFT-s-OFDM";
                            //                            vsg2.SetIBBWaveform(isPower, isFreq, IsRBOffset, Bandwidth);
                            //                            vsg1.setParameter(1, wsPower, wsFreq,TriggerDelay); //smw200a                      
                            //                            vsg2.setParameter(2, isPower, isFreq, TriggerDelay); //smw200a
                            //                            inbMeasurementInSequence(bbu, vsg2, mode, setPort, step, 10000, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, _isFreqLimitLeft.ToString(), isLowerFreqRange, isUpperFreqRange, _isFreqLimitRight.ToString(), Bandwidth, out TP);
                            //                            break;
                            //                        case "NBB":
                            //                            if (vsg2.Model != "SMW200A")
                            //                            {
                            //                                vsg2.SetNBBWaveform(isPower, isFreq, IsRBOffset, Bandwidth);
                            //                            }
                            //                            else
                            //                            {
                            //                                if (double.Parse(wsFreq) < double.Parse(isFreq)) //Nhieu ben phai
                            //                                {
                            //                                    vsg2.SetNBBWaveform(isPower, isFreq, IsRBOffset, Bandwidth);
                            //                                }
                            //                                else
                            //                                {
                            //                                    vsg2.SetNBBWaveform(isPower, isFreq, IsRBOffset, Bandwidth); //nhieu ben trai
                            //                                }
                            //                            }
                            //                            //vsg2.SetNBBWaveform(isPower, isFreq, IsRBOffset);
                            //                            vsg1.setParameter(1, wsPower, wsFreq,TriggerDelay);
                            //                            //System.Threading.Thread.Sleep(2000);
                            //                            vsg2.setParameter(2, isPower, isFreq, TriggerDelay);
                            //                            vsg2.TypeOfSignal = "DFT-s-OFDM_1RB";
                            //                            bbu.NEW_RXSENS(Bandwidth, WsRBOffset, out throughput);
                            //                            //26.2.2024 - sua code
                            //                            writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, throughput, out TP);
                            //                            break;
                            //                        case "OOB2":
                            //                            //25.3.2024 - viet lai bai do oob                        
                            //                            //neu su dung smw200a la nguon phat ws => tat bo nguon nhieu o port 2
                            //                            if (vsg1.Model == "SMW200A")
                            //                            {
                            //                                vsg1.OffSignal(2); //off nguon 2
                            //                            }
                            //                            //measRange
                            //                            List<string[]> oobMeasRange = new List<string[]>();
                            //                            oobMeasRange.Add(new string[] { "", isLowerFreqRange, isUpperFreqRange });
                            //                            //exceptRange
                            //                            List<string[]> oobExRange = new List<string[]>();
                            //                            //add Co-location bands to exception range if check option
                            //                            if (mainForm.OOBexcludeColocationRange)
                            //                            {
                            //                                getISCoLocationRangeFromExcel();
                            //                                foreach (var item in oobCoISsweepRanges)
                            //                                {
                            //                                    oobExRange.Add(new string[] { item[1].ToString(), item[2].ToString() });
                            //                                }
                            //                            }
                            //                            // add Operating band with delta_fOOB to exception range
                            //                            oobExRange.Add(new string[] { (_isFreqLimitLeft / 1e6).ToString(), (_isFreqLimitRight / 1e6).ToString() });
                            //                            //for debugging
                            //                            Log("[Out-of-band Measurement] Reading Excluding Range:");
                            //                            foreach (var range in oobExRange)
                            //                            {
                            //                                Log($"[ {range[0]} MHz, {range[1]} MHz]");
                            //                            }
                            //                            //============
                            //                            oobMeasurementInSequence(bbu, vsg2, mode, setPort, step, 800, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, oobMeasRange, oobExRange, Bandwidth, out TP);
                            //                            break;

                            //                        case "OOB1": //oob co-location
                            //                                     //neu su dung smw200a la nguon phat ws => tat bo nguon nhieu o port 2
                            //                            if (vsg1.Model == "SMW200A")
                            //                            {
                            //                                vsg1.OffSignal(2);
                            //                            }
                            //                            getISCoLocationRangeFromExcel();
                            //                            oobMeasurementInSequence(bbu, vsg2, mode, portIndex, step, 800, wsFreq, wsPowerStr, WsRBOffset, isFreq, isPowerStr, IsRBOffset, oobCoISsweepRanges, null, Bandwidth, out TP);
                            //                            break;

                            //                        // Add cases for other modes

                            //                        default:
                            //                            // Handle unsupported mode here.
                            //                            break;

                            #endregion
                    }
                    vsg2.OnOffSignal(0);
                }
            }
            catch (Exception ex)
            {
                Log("Cannot find throughput! " + ex.Message, LogLevel.ERROR);
                //stop cacs testmac
                if (L1TestMacV2Start) bbu.STOP_L1TESTMAC();
                if (L1TestMacV3Start) bbu.STOP_L1TESTMAC_V3();

            }

        }
        //bai do inband theo phuong an chay TM lien tuc - 01.03.2024
        #region OldIB
        //private void inbMeasurementInSequence(BBU bbu, VSG vsg2, string mode, int setPort, double step, int timeSequence, string wsFreq, string wsPower, string WsRBOffset, string isFreq, string isPower, string IsRBOffset, string isLowerFreqFrom, string isLowerFreqTo, string isUpperFreqFrom, string isUpperFreqTo, string BW, out double TP)
        //{
        //    TP = -1;
        //    double minTP = -1; //out of range 0-100            
        //    double.TryParse(isLowerFreqFrom, out double isFreqFromL);
        //    double.TryParse(isLowerFreqTo, out double isFreqToL);
        //    double.TryParse(isUpperFreqFrom, out double isFreqFromR);
        //    double.TryParse(isUpperFreqTo, out double isFreqToR);
        //    string throughput;
        //    int testCount = 0;

        //    double freqIS = 0;
        //    //read att of IS:
        //    double sFreqChange = isFreqFromL;
        //    string path = $@"{mainForm.isCSVFilePathRX}/ISRX{setPort}.csv";
        //    double isATT = vsg2.ReadATTfromCSVFile(path, sFreqChange*1e6);
        //    Log($"{vsg2.Alias}: Set ATT_Port{setPort}={isATT} dB at f={sFreqChange}MHz");
        //    Log($"{vsg2.Alias}: CSV Path:{path}");

        //    bool runningMode = true; // mode 
        //    if (runningMode)
        //    {
        //        L1TestMacV2Start = bbu.STR_RXSENS_V2(BW, WsRBOffset, setPort);
        //        //start RxSen_V2 fail => Dung do

        //        //if (!L1TestMacV2Start)
        //        //{
        //        //    bbu.STOP_L1TESTMAC();
        //        //    //waiting for 30s to restart L1TestMac again
        //        //    countdownTimerInSecond(30, "Waiting for L1TestMac restart again");
        //        //    L1TestMacV2Start = bbu.STR_RXSENS_V2(BW, WsRBOffset, currPort.StreamIndex);
        //        //}

        //        if (L1TestMacV2Start)
        //        {
        //            Log("Setting Measurement Environment...");
        //            //waiting for 25s to restart L1TestMac again
        //            countdownTimerInSecond(25, "Waiting for preseting Testmac");
        //            var syncWatch = System.Diagnostics.Stopwatch.StartNew();
        //            bool synchronized = false;

        //            while (syncWatch.ElapsedMilliseconds < 60000)
        //            {
        //                if (mainForm.measurementBackgroundWorker.CancellationPending)
        //                {
        //                    Log("[INB2] Measurement cancelled.");
        //                    bbu.STOP_L1TESTMAC();
        //                    L1TestMacV2Start = false;
        //                    return;
        //                }

        //                if (bbu.VALIDATEFILEOUT_RXSENS_V2())
        //                {
        //                    synchronized = true;
        //                    break;
        //                }

        //                System.Threading.Thread.Sleep(100);
        //            }

        //            if (!synchronized)
        //            {
        //                Log("[INB2] Timeout waiting for RXSENS_V2 synchronization.",
        //                    LogLevel.ERROR);

        //                bbu.STOP_L1TESTMAC();
        //                L1TestMacV2Start = false;
        //                return;
        //            }
        //            //Log("Synchronized Time Period completed: " + DateTime.Now.ToString()); //for debugging                        
        //            Log("Start Measurement!");

        //            //start loop                
        //            DashedLineLog();
        //            //======== Setup Cycle ======
        //            //int stepTime = 10000; // 10s
        //            System.Threading.Thread.Sleep(10);

        //            // INB2 không sử dụng Timer và flagStep nữa.
        //            bool isOAMsyncFail = false;
        //            bool firstStep = true;
        //            int errCount = 0;

        //            var sweepWatch = System.Diagnostics.Stopwatch.StartNew();

        //            // 10 giây giữa các lần đọc throughput.
        //            var stepWatch = System.Diagnostics.Stopwatch.StartNew();

        //            const int sweepTimeoutMs = 300000;
        //            while (sFreqChange <= isFreqToR)
        //            {
        //                if (mainForm.measurementBackgroundWorker.CancellationPending)
        //                {
        //                    Log("[INB2] Measurement cancelled.");
        //                    break;
        //                }

        //                if (sweepWatch.ElapsedMilliseconds > sweepTimeoutMs)
        //                {
        //                    Log("[INB2] Sweep timeout.", LogLevel.ERROR);
        //                    break;
        //                }

        //                // Chờ đủ thời gian trước khi đọc throughput.
        //                if (stepWatch.ElapsedMilliseconds < timeSequence)
        //                {
        //                    System.Threading.Thread.Sleep(10);
        //                    continue;
        //                }

        //                stepWatch.Restart();
        //                //System.Threading.Thread.Sleep(10);
        //                if (flagStep)
        //                {
        //                    if (!firstStep)
        //                    {
        //                        //08.01.2024 - chuyen thong bao len dau vong lap
        //                        Log($"Set frequency of IS to: {sFreqChange / 1e6} MHz");
        //                        //tao chu ky 10s

        //                        //delay = DateTime.Now.AddSeconds(0.05); // 08.01.2024 - test voi chu ky 100ms ???can trong vong lap nay khong???
        //                        isFreq = sFreqChange.ToString();

        //                        //check background worker
        //                        if (mainForm.measurementBackgroundWorker.CancellationPending)
        //                        {
        //                            TP = (minTP <= 100 && minTP >= 0) ? minTP : -1; //minTP out of range 100 ==> TP=0;
        //                            break; //nhay khoi vong lap
        //                        }
        //                        System.Threading.Thread.Sleep(1);

        //                        //Đọc throughput về
        //                        bbu.RXSENS_V2(out throughput); //sua sau khi update OAM
        //                        System.Threading.Thread.Sleep(1);
        //                        double tempTP = GetthroughputFromString(throughput, setPort);
        //                        if ((tempTP >= 95 && tempTP <= 100) || testCount == 3)
        //                        {
        //                            //28.3.2024 - Truong hop van doc dc ve mang throughput
        //                            if (tempTP > 0)
        //                            {
        //                                writeCSVThroughputValue(mode, setPort, wsFreq, wsPower, WsRBOffset, isFreq, isPower, IsRBOffset, throughput, out TP);
        //                                //luu lại giá trị TP nhỏ nhất
        //                                if (minTP >= TP || minTP == -1)
        //                                {
        //                                    minTP = TP;
        //                                    freqIS = sFreqChange;
        //                                }
        //                                Log($"Min throughput = {minTP}% @ NR Interfering Freq. = {freqIS / 1e6} MHz");
        //                                DashedLineLog("-------");
        //                                //09.01.2024 - plot chart
        //                                mainForm.PopulateMeasurementChart($"Inband-Blocking: Port {setPort}", (sFreqChange / 1e6).ToString(), TP.ToString(), "95", "100");
        //                                //==================
        //                                //Change CW frequency for next step                            
        //                                sFreqChange += step;
        //                                //kiem tra tan so nam trong dai tan sweep
        //                                if (sFreqChange > isFreqToL & sFreqChange < isFreqFromR)
        //                                {
        //                                    sFreqChange = isFreqFromR;
        //                                }
        //                                //end loop when freq is up to UpperFreq
        //                                if (sFreqChange > isFreqToR)
        //                                {
        //                                    break;
        //                                }
        //                                isATT = vsg2.ReadATTfromCSVFile(path, sFreqChange);
        //                                Log($"{vsg2.Alias}: Set ATT_Port{setPort}={isATT} dB at f={sFreqChange / 1e6}MHz");
        //                                //Phat nhieu                             
        //                                isFreq = sFreqChange.ToString();
        //                                //vsg2.SetIBBWaveform(isPower, isFreq, IsRBOffset);
        //                                //19.3.2024
        //                                if (vsg2.Model == "SMW200A")
        //                                {
        //                                    //vsg2.setParameter(2, (double.Parse(isPower) + isATT).ToString("N3"), isFreq, TriggerDelay);
        //                                    vsg2.SetFreq(isFreq);
        //                                    vsg2.SetPower((double.Parse(isPower) + isATT).ToString("N3"));
        //                                }
        //                                else
        //                                {
        //                                    vsg2.SetFreq(isFreq);
        //                                    vsg2.SetPower((double.Parse(isPower) + isATT).ToString("N3"));
        //                                }
        //                                //reset testCount ve 0
        //                                testCount = 0;
        //                                errCount = 0;
        //                                isOAMsyncFail = false;
        //                            }
        //                            else   // truong hop khong doc duoc ve mang throughput do loi mat OAM sau khi da test lai 3 lan
        //                            {
        //                                errCount += 1;
        //                                //Console.WriteLine($"\rWaiting for synchronization.....{errCount}");
        //                                if (errCount >= 1000)  //28.3.2024 - Sau 1000 vong lap, neu van loi thi bao loi
        //                                {
        //                                    isOAMsyncFail = true;
        //                                    Log("Server is not responding, measurement stop!");
        //                                    errCount = 0;
        //                                    break;
        //                                }
        //                            }

        //                        }
        //                        else
        //                        {
        //                            Log($"TP = {tempTP}% => Retest: {testCount}");
        //                            testCount += 1;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        firstStep = false;
        //                    }
        //                    //xoa flag
        //                    flagStep = false;
        //                }
        //                if (isOAMsyncFail)  //28.3.2024 - Sau 100 vong lap, neu van loi thi bao loi va thoat bai do
        //                {
        //                    break;
        //                }
        //            }

        //            //Ket thuc chay vong lap
        //            timerSequence.Dispose();  //Stop timer
        //            bbu.STOP_L1TESTMAC();
        //            L1TestMacV2Start = false;
        //            vsg2.OnOffSignal(0);
        //            //gan gia tri throughput nho nhat de in ra ngoai ListView
        //            DashedLineLog("---");
        //            Log("Measurement finished!");
        //            TP = minTP;
        //            if (TP < 95) Log("LOG: Throughput < 95% => FAILED!", LogLevel.ERROR);
        //            else Log("LOG: Throughput > 95% => PASSED!", LogLevel.SUCCESS);
        //            Log("========================================");

        //            if (isOAMsyncFail)
        //            {
        //                MessageBox.Show("OAM is not responding, measurement stop!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            }
        //        }

        //        else
        //        {
        //            bbu.STOP_L1TESTMAC();
        //            L1TestMacV2Start = false;
        //            Log("Start RXSENS_V2 Fail! Wait for a while before next measurement!", LogLevel.ERROR);
        //        }
        //    }
        //}
        // REVIEW PATCH - replace only oobMeasurementInSequence() inside twoVSG.cs.
        // Not compiled against the complete project or tested on RF hardware.
        // Requires: using System.Globalization; using System.Threading;
        #endregion


        private void inbMeasurementInSequence(
            BBU bbu,
            VSG vsg2,
            string mode,
            int setPort,
            double step,
            int timeSequence,
            string wsFreq,
            string wsPower,
            string WsRBOffset,
            string isFreq,
            string isPower,
            string IsRBOffset,
            string isLowerFreqFrom,
            string isLowerFreqTo,
            string isUpperFreqFrom,
            string isUpperFreqTo,
            string BW,
            out double TP)
        {
            TP = -1;

            double minTP = -1;
            double freqIS = 0;
            bool testMacStarted = false;
            bool cancelled = false;
            bool measurementFailed = false;

            var culture =
                System.Globalization.CultureInfo.InvariantCulture;

            // ===== 1. Parse frequency ranges (MHz) =====
            double fromL, toL, fromR, toR;

            if (!double.TryParse(isLowerFreqFrom, out fromL) ||
                !double.TryParse(isLowerFreqTo, out toL) ||
                !double.TryParse(isUpperFreqFrom, out fromR) ||
                !double.TryParse(isUpperFreqTo, out toR))
            {
                Log("[INB2] Invalid frequency configuration.",
                    LogLevel.ERROR);
                return;
            }

            // GUI có thể truyền step theo Hz hoặc MHz.
            // Ví dụ 1000000 Hz = 1 MHz.
            double stepMHz = step >= 1000
                ? step / 1e6
                : step;

            if (stepMHz <= 0 ||
                fromL > toL ||
                fromR > toR ||
                toL >= fromR)
            {
                Log("[INB2] Invalid sweep ranges or step.",
                    LogLevel.ERROR);
                return;
            }

            // ===== 2. Prepare frequency points =====
            var frequencyPoints = new List<double>();

            double[] starts = { fromL, fromR };
            double[] ends = { toL, toR };

            for (int rangeIndex = 0; rangeIndex < 2; rangeIndex++)
            {
                double start = starts[rangeIndex];
                double end = ends[rangeIndex];

                for (int point = 0; ; point++)
                {
                    double frequency = start + point * stepMHz;

                    if (frequency > end + 1e-9)
                        break;

                    if (point > 1000000)
                    {
                        Log("[INB2] Too many sweep points.",
                            LogLevel.ERROR);
                        return;
                    }

                    frequencyPoints.Add(frequency);
                }
            }

            Log(
                $"[INB2] Lower: {fromL} - {toL} MHz; " +
                $"Upper: {fromR} - {toR} MHz");

            Log(
                $"[INB2] Step={stepMHz} MHz; " +
                $"Total points={frequencyPoints.Count}");

            // ===== 3. Correction configuration =====
            string path = System.IO.Path.Combine(
                mainForm.isCSVFilePathRX,
                $"ISRX{setPort}.csv");

            if (!System.IO.File.Exists(path))
            {
                Log("[INB2] Correction file not found: " + path,
                    LogLevel.ERROR);
                return;
            }

            // isPower đã cộng AttenuatorLevel ở Measurement().
            double initialPower;

            if (!double.TryParse(
                isPower,
                System.Globalization.NumberStyles.Float |
                System.Globalization.NumberStyles.AllowThousands,
                System.Globalization.CultureInfo.CurrentCulture,
                out initialPower))
            {
                Log("[INB2] Invalid IS power.", LogLevel.ERROR);
                return;
            }

            double targetISPower =
                initialPower - vsg2.AttenuatorLevel;

            int dwellMs = Math.Max(1, timeSequence);
            const int maxReadAttempts = 4;

            // ===== 4. Start BBU measurement =====
            try
            {
                Log("[INB2] Starting RXSENS_V2...");

                testMacStarted = bbu.STR_RXSENS_V2(
                    BW, WsRBOffset, setPort);

                L1TestMacV2Start = testMacStarted;

                if (!testMacStarted)
                {
                    Log("[INB2] STR_RXSENS_V2 failed.",
                        LogLevel.ERROR);
                    measurementFailed = true;
                    return;
                }

                countdownTimerInSecond(
                    25, "Waiting for preseting Testmac");

                // ===== 5. Wait for synchronization =====
                var syncWatch =
                    System.Diagnostics.Stopwatch.StartNew();

                bool synchronized = false;

                while (syncWatch.ElapsedMilliseconds < 60000)
                {
                    if (mainForm.measurementBackgroundWorker
                        .CancellationPending)
                    {
                        cancelled = true;
                        return;
                    }

                    if (bbu.VALIDATEFILEOUT_RXSENS_V2())
                    {
                        synchronized = true;
                        break;
                    }

                    System.Threading.Thread.Sleep(100);
                }

                if (!synchronized)
                {
                    Log("[INB2] BBU synchronization timeout.",
                        LogLevel.ERROR);

                    measurementFailed = true;
                    return;
                }

                Log("[INB2] BBU synchronized.");
                Log("[INB2] Start Measurement!");

                DashedLineLog();

                // ===== 6. Sequential frequency sweep =====
                foreach (double freqMHz in frequencyPoints)
                {
                    if (mainForm.measurementBackgroundWorker
                        .CancellationPending)
                    {
                        cancelled = true;
                        break;
                    }

                    // SMW200A SetFreq() nhận tần số Hz.
                    double freqHz = freqMHz * 1e6;

                    string freqText = freqHz.ToString(
                        "F0", culture);

                    // Correction file sử dụng Hz.
                    double isATT =
                        vsg2.ReadATTfromCSVFile(path, freqHz);

                    double smwPower = targetISPower + isATT;

                    string powerText = smwPower.ToString(
                        "F3", culture);

                    Log(
                        $"[INB2] IS={freqMHz:F3} MHz; " +
                        $"ATT={isATT:F3} dB; " +
                        $"SMW Power={smwPower:F3} dBm");

                    // ===== 7. Configure interfering signal =====
                    if (vsg2.Model == "SMW200A")
                    {
                        // Chỉ cấu hình Source 2.
                        // Không gọi vsg2.SetFreq() vì hàm đó
                        // thay đổi cả Source 1 và Source 2.
                        vsg2.setParameter(
                            2,
                            powerText,
                            freqText,
                            vsg2.TriggerDelay);
                    }
                    else
                    {
                        vsg2.SetFreq(freqText);
                        vsg2.SetPower(powerText);
                    }

                    // ===== 8. Wait before reading throughput =====
                    var dwellWatch =
                        System.Diagnostics.Stopwatch.StartNew();

                    while (dwellWatch.ElapsedMilliseconds < dwellMs)
                    {
                        if (mainForm.measurementBackgroundWorker
                            .CancellationPending)
                        {
                            cancelled = true;
                            break;
                        }

                        System.Threading.Thread.Sleep(20);
                    }

                    if (cancelled)
                        break;

                    // ===== 9. Read throughput =====
                    bool validTP = false;
                    string throughput = "";
                    double tempTP = -1;

                    for (int attempt = 1;
                         attempt <= maxReadAttempts;
                         attempt++)
                    {
                        if (mainForm.measurementBackgroundWorker
                            .CancellationPending)
                        {
                            cancelled = true;
                            break;
                        }

                        Log(
                            $"[INB2] Reading throughput: " +
                            $"{freqMHz:F3} MHz; " +
                            $"Attempt={attempt}/{maxReadAttempts}");

                        try
                        {
                            bbu.RXSENS_V2(out throughput);

                            tempTP = GetthroughputFromString(
                                throughput, setPort);
                        }
                        catch (Exception ex)
                        {
                            Log(
                                "[INB2] Throughput read error: " +
                                ex.Message,
                                LogLevel.ERROR);

                            tempTP = -1;
                        }

                        if (!double.IsNaN(tempTP) &&
                            !double.IsInfinity(tempTP) &&
                            tempTP >= 0 &&
                            tempTP <= 100)
                        {
                            validTP = true;
                            break;
                        }

                        if (attempt < maxReadAttempts)
                            System.Threading.Thread.Sleep(1000);
                    }

                    if (cancelled)
                        break;

                    if (!validTP)
                    {
                        Log(
                            $"[INB2] Invalid throughput at " +
                            $"{freqMHz:F3} MHz.",
                            LogLevel.ERROR);

                        measurementFailed = true;
                        break;
                    }

                    // TP < 95% là kết quả đo không đạt,
                    // không phải mặc định là lỗi OAM.
                    Log(
                        $"[INB2] Throughput={tempTP:F3}% " +
                        $"at {freqMHz:F3} MHz");

                    // ===== 10. Save CSV =====
                    isFreq = freqText;

                    writeCSVThroughputValue(
                        mode,
                        setPort,
                        wsFreq,
                        wsPower,
                        WsRBOffset,
                        isFreq,
                        isPower,
                        IsRBOffset,
                        throughput,
                        out double csvTP);

                    // Lấy TP đã parse để tránh kết quả
                    // ghi CSV làm sai minTP.
                    TP = tempTP;

                    if (minTP < 0 || tempTP < minTP)
                    {
                        minTP = tempTP;
                        freqIS = freqMHz;
                    }

                    // ===== 11. Update chart =====
                    mainForm.PopulateMeasurementChart(
                        $"Inband-Blocking: Port {setPort}",
                        freqMHz.ToString("F3", culture),
                        tempTP.ToString("F3", culture),
                        "95",
                        "100");

                    Log(
                        $"[INB2] Min TP={minTP:F3}% " +
                        $"at IS={freqIS:F3} MHz");

                    DashedLineLog("-------");
                }

                // ===== 12. Final result =====
                TP = minTP;

                if (cancelled)
                {
                    Log("[INB2] Measurement cancelled.");
                }
                else if (measurementFailed || minTP < 0)
                {
                    Log("[INB2] Measurement incomplete.",
                        LogLevel.ERROR);
                }
                else
                {
                    Log("[INB2] Sweep completed.");

                    if (minTP >= 95)
                        Log("[INB2] PASSED.", LogLevel.SUCCESS);
                    else
                        Log("[INB2] FAILED: TP < 95%.",
                            LogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                measurementFailed = true;

                Log(
                    "[INB2] Measurement exception: " +
                    ex.Message,
                    LogLevel.ERROR);
            }
            finally
            {
                // ===== 13. Cleanup =====
                if (testMacStarted)
                {
                    try
                    {
                        bbu.STOP_L1TESTMAC();
                    }
                    catch (Exception ex)
                    {
                        Log(
                            "[INB2] STOP_L1TESTMAC error: " +
                            ex.Message,
                            LogLevel.ERROR);
                    }
                }

                L1TestMacV2Start = false;

                try
                {
                    vsg2.OnOffSignal(0);
                }
                catch (Exception ex)
                {
                    Log(
                        "[INB2] RF OFF error: " +
                        ex.Message,
                        LogLevel.ERROR);
                }

                if (measurementFailed)
                {
                    Log("[INB2] Stopped due to error.",
                        LogLevel.ERROR);
                }
            }
        }

        #region oobMeasurementInSequence_old
        //private void oobMeasurementInSequence(
        //    BBU bbu, VSG vsg2, string mode, int setPort, double step, int timeSequence,
        //    string wsFreq, string wsPower, string WsRBOffset, string isFreq,
        //    string isPower, string IsRBOffset, List<string[]> measRanges,
        //    List<string[]> exceptRanges, string BW, out double TP)
        //{
        //    TP = -1;
        //    double minTP = -1;
        //    bool testMacStarted = false;
        //    bool completed = false;
        //    const int syncTimeoutMs = 60000;
        //    const int maxReadAttempts = 4; // 1 initial + 3 retries
        //    int dwellMs = Math.Max(100, timeSequence);
        //    var culture = System.Globalization.CultureInfo.InvariantCulture;
        //    var worker = mainForm.measurementBackgroundWorker;

        //    try
        //    {
        //        if (step <= 0 || double.IsNaN(step) || double.IsInfinity(step))
        //            throw new ArgumentOutOfRangeException("step", "Sweep step must be positive (Hz).");
        //        if (worker.CancellationPending) return;

        //        List<string[]> sweepRanges = GetSweepRangeWithID(measRanges, exceptRanges);
        //        if (sweepRanges == null || sweepRanges.Count == 0)
        //        {
        //            Log("OOB: no valid sweep ranges.");
        //            return;
        //        }

        //        vsg2.TypeOfSignal = "CW";
        //        vsg2.LoadUserCorrDataSGS100A(@"/var/user/RxBlocking");

        //        // Configure the first point before starting TestMac.
        //        double firstMHz = double.Parse(sweepRanges[0][1], culture)*1e6;
        //        vsg2.SetCWWaveform(isPower, firstMHz.ToString("F0", culture));
        //        vsg2.LogUserCorrectionSGS100A(isPower, firstMHz);



        //        // Keep existing one-retry startup behavior, but honor Stop.
        //        for (int startAttempt = 0; startAttempt < 2 && !testMacStarted; startAttempt++)
        //        {
        //            if (worker.CancellationPending) return;
        //            if (startAttempt > 0)
        //            {
        //                bbu.STOP_L1TESTMAC();
        //                for (int ms = 0; ms < 30000; ms += 100)
        //                {
        //                    if (worker.CancellationPending) return;
        //                    System.Threading.Thread.Sleep(100);
        //                }
        //            }
        //            testMacStarted = bbu.STR_RXSENS_V2(BW, WsRBOffset, currPort.StreamIndex);
        //            L1TestMacV2Start = testMacStarted;
        //        }
        //        if (!testMacStarted)
        //        {
        //            Log("OOB: cannot start RXSENS_V2.", LogLevel.ERROR);
        //            return;
        //        }

        //        // Wait for BBU file-out with a bounded, cancellable loop.
        //        var syncWatch = System.Diagnostics.Stopwatch.StartNew();
        //        bool synchronized = false;
        //        while (syncWatch.ElapsedMilliseconds < syncTimeoutMs)
        //        {
        //            if (worker.CancellationPending) return;
        //            if (bbu.VALIDATEFILEOUT_RXSENS_V2())
        //            {
        //                synchronized = true;
        //                break;
        //            }
        //            System.Threading.Thread.Sleep(100);
        //        }
        //        if (!synchronized)
        //        {
        //            Log("OOB: timeout waiting for RXSENS_V2 file-out.", LogLevel.ERROR);
        //            return;
        //        }

        //        // One worker owns the sweep. No static flagStep or Timer callback.
        //        foreach (string[] range in sweepRanges)
        //        {
        //            if (worker.CancellationPending) return;
        //            if (range == null || range.Length < 3)
        //                throw new FormatException("Invalid OOB sweep range.");

        //            double fromHz = double.Parse(range[1], culture) * 1e6;
        //            double toHz = double.Parse(range[2], culture) * 1e6;
        //            if (fromHz > toHz) throw new FormatException("OOB range start > end.");
        //            Log(string.Format("OOB sweep: {0} [{1}, {2}] MHz", range[0], range[1], range[2]));

        //            // Index-based calculation avoids cumulative floating-point drift.
        //            for (int point = 0; ; point++)
        //            {
        //                if (worker.CancellationPending) return;
        //                double freqHz = fromHz + point * step;
        //                if (freqHz > toHz + 0.01) break;
        //                if (point > 1000000)
        //                    throw new InvalidOperationException("OOB sweep exceeds 1,000,000 points.");

        //                //string freqText = freqHz.ToString("F0", culture);
        //                //bool skipFrequency = false;

        //                //if (exceptRanges != null)
        //                //{
        //                //    foreach (var exprange in exceptRanges)
        //                //    {
        //                //        double startMHz = double.Parse(
        //                //            range[0],
        //                //            System.Globalization.CultureInfo.InvariantCulture);

        //                //        double endMHz = double.Parse(
        //                //            range[1],
        //                //            System.Globalization.CultureInfo.InvariantCulture);

        //                //        double freqMHz = freqHz / 1e6;

        //                //        if (freqMHz >= startMHz &&
        //                //            freqMHz <= endMHz)
        //                //        {
        //                //            skipFrequency = true;
        //                //            break;
        //                //        }
        //                //    }
        //                //}

        //                //if (skipFrequency)
        //                //{
        //                //    Log("[OOB] SKIP excluded CW = " +
        //                //        (freqHz / 1e6).ToString("F3") + " MHz");

        //                //    continue;
        //                //}
        //                string freqText = freqHz.ToString("F0", culture);

        //                // Check excluded frequency ranges
        //                bool skipFrequency = false;

        //                if (exceptRanges != null)
        //                {
        //                    foreach (string[] exceptRange in exceptRanges)
        //                    {
        //                        if (exceptRange == null || exceptRange.Length < 2)
        //                        {
        //                            throw new FormatException(
        //                                "OOB exceptRange is missing frequency values.");
        //                        }

        //                        double startMHz;
        //                        double endMHz;

        //                        bool validStart = double.TryParse(
        //                            exceptRange[0],
        //                            System.Globalization.NumberStyles.Float,
        //                            System.Globalization.CultureInfo.InvariantCulture,
        //                            out startMHz);

        //                        bool validEnd = double.TryParse(
        //                            exceptRange[1],
        //                            System.Globalization.NumberStyles.Float,
        //                            System.Globalization.CultureInfo.InvariantCulture,
        //                            out endMHz);

        //                        if (!validStart || !validEnd)
        //                        {
        //                            throw new FormatException(
        //                                "Invalid OOB exceptRange: [" +
        //                                exceptRange[0] + ", " +
        //                                exceptRange[1] + "]");
        //                        }

        //                        double freqMHz = freqHz / 1e6;

        //                        if (freqMHz >= Math.Min(startMHz, endMHz) &&
        //                            freqMHz <= Math.Max(startMHz, endMHz))
        //                        {
        //                            skipFrequency = true;
        //                            break;
        //                        }
        //                    }
        //                }

        //                if (skipFrequency)
        //                {
        //                    Log("[OOB] SKIP excluded CW = " +
        //                        (freqHz / 1e6).ToString(
        //                            "F3",
        //                            System.Globalization.CultureInfo.InvariantCulture) +
        //                        " MHz");

        //                    continue;
        //                }
        //                vsg2.SetCWWaveform(isPower, freqText);
        //                Log(string.Format("OOB: CW = {0} MHz", freqHz / 1e6));
        //                vsg2.LogUserCorrectionSGS100A(isPower, freqHz);


        //                // Dwell is cancellable. No busy-spin.
        //                for (int waited = 0; waited < dwellMs; waited += 50)
        //                {
        //                    if (worker.CancellationPending) return;
        //                    System.Threading.Thread.Sleep(Math.Min(50, dwellMs - waited));
        //                }

        //                bool valid = false;
        //                string throughput = "ERROR";
        //                for (int attempt = 0; attempt < maxReadAttempts; attempt++)
        //                {
        //                    if (worker.CancellationPending) return;
        //                    bbu.RXSENS_V2(out throughput);
        //                    double tempTP = GetthroughputFromString(throughput, setPort);
        //                    // Preserve original code's tempTP > 0 acceptance policy.
        //                    // A real 0% throughput needs separate validation in the parser.
        //                    if (tempTP > 0 && tempTP <= 100)
        //                    {
        //                        valid = true;
        //                        break;
        //                    }
        //                    if (attempt + 1 < maxReadAttempts)
        //                    {
        //                        for (int waited = 0; waited < dwellMs; waited += 50)
        //                        {
        //                            if (worker.CancellationPending) return;
        //                            System.Threading.Thread.Sleep(Math.Min(50, dwellMs - waited));
        //                        }
        //                    }
        //                }
        //                if (!valid)
        //                {
        //                    Log("OOB: no valid throughput after retries at " + freqText + " Hz.", LogLevel.ERROR);
        //                    return;
        //                }

        //                string subName = string.IsNullOrEmpty(range[0]) ? "" : "_" + range[0];
        //                writeCSVThroughputValue(mode + "_P" + setPort + subName,
        //                    setPort, wsFreq, wsPower, WsRBOffset, freqText,
        //                    isPower, IsRBOffset, throughput, out double currentTP);

        //                if (currentTP >= 0 && currentTP <= 100 &&
        //                    (minTP < 0 || currentTP < minTP))
        //                    minTP = currentTP;

        //                mainForm.PopulateMeasurementChart("Out-of-band Blocking: Port " + setPort,
        //                    (freqHz / 1e6).ToString(culture), currentTP.ToString(culture), "95", "100");
        //                Log("OOB min throughput = " + minTP + "%");
        //            }
        //        }
        //        completed = true;
        //        TP = minTP;
        //        Log("OOB completed. Min TP = " + minTP + "%",
        //            minTP >= 95 ? LogLevel.SUCCESS : LogLevel.ERROR);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log("OOB stopped: " + ex.Message, LogLevel.ERROR);
        //    }
        //    finally
        //    {
        //        // A failed Stop command must not prevent CW from being turned off.
        //        if (testMacStarted || L1TestMacV2Start)
        //        {
        //            try { bbu.STOP_L1TESTMAC(); }
        //            catch (Exception ex) { Log("STOP_L1TESTMAC: " + ex.Message, LogLevel.ERROR); }
        //        }
        //        L1TestMacV2Start = false;
        //        try { vsg2.OnOffSignal(0); }
        //        catch (Exception ex) { Log("CW OFF: " + ex.Message, LogLevel.ERROR); }
        //        if (!completed) TP = -1;
        //    }
        //}

        #endregion
        private void oobMeasurementInSequence(
    BBU bbu, VSG vsg2, string mode, int setPort, double step, int timeSequence,
    string wsFreq, string wsPower, string WsRBOffset, string isFreq,
    string isPower, string IsRBOffset, List<string[]> measRanges,
    List<string[]> exceptRanges, string BW, out double TP)
        {
            TP = -1;
            double minTP = -1;
            bool testMacStarted = false;
            bool completed = false;
            const int syncTimeoutMs = 60000;
            const int maxReadAttempts = 4;
            int dwellMs = Math.Max(100, timeSequence);
            var culture = System.Globalization.CultureInfo.InvariantCulture;
            var worker = mainForm.measurementBackgroundWorker;
            var exclusions = new List<double[]>();
            var seenHz = new HashSet<long>();
            bool hasInvalidPoint = false;
            int invalidPointCount = 0;

            try
            {
                if (Math.Abs(step - 1e6) > 0.001)
                    throw new ArgumentException("3GPP OOB CW step must be 1 MHz (1000000 Hz); actual step=" + step);
                if (measRanges == null || measRanges.Count == 0)
                    throw new ArgumentException("No OOB measurement ranges configured.");
                if (worker.CancellationPending) return;

                // Parse and validate exclusions BEFORE enabling the interferer.
                if (exceptRanges != null)
                {
                    foreach (string[] exceptRange in exceptRanges)
                    {
                        if (exceptRange == null || exceptRange.Length < 2)
                            throw new FormatException("Invalid OOB exclusion row (needs 2 values).");
                        double low, high;
                        if (!double.TryParse(exceptRange[0], System.Globalization.NumberStyles.Float, culture, out low) ||
                            !double.TryParse(exceptRange[1], System.Globalization.NumberStyles.Float, culture, out high) ||
                            double.IsNaN(low) || double.IsNaN(high) || double.IsInfinity(low) || double.IsInfinity(high))
                            throw new FormatException("Invalid OOB exclusion MHz: [" + exceptRange[0] + ", " + exceptRange[1] + "]");
                        exclusions.Add(new double[] { Math.Min(low, high), Math.Max(low, high) });
                    }
                }

                // Work from ORIGINAL measurement ranges; do not use GetSweepRangeWithID here.
                // This avoids boundary inclusion errors in PopulateRange and preserves the 1-MHz grid.
                var points = new List<Tuple<string, long>>();
                foreach (string[] range in measRanges)
                {
                    if (range == null || range.Length < 3)
                        throw new FormatException("Invalid OOB measurement range (needs ID, start, end).");
                    double fromMHz, toMHz;
                    if (!double.TryParse(range[1], System.Globalization.NumberStyles.Float, culture, out fromMHz) ||
                        !double.TryParse(range[2], System.Globalization.NumberStyles.Float, culture, out toMHz) ||
                        double.IsNaN(fromMHz) || double.IsNaN(toMHz) || fromMHz > toMHz)
                        throw new FormatException("Invalid OOB sweep range: [" + range[1] + ", " + range[2] + "] MHz");
                    Log("OOB source range: " + range[0] + " [" + range[1] + ", " + range[2] + "] MHz");
                    long count = (long)Math.Floor((toMHz - fromMHz) + 1e-9) + 1;
                    if (count < 1 || count > 1000000 || points.Count + count > 1000000)
                        throw new InvalidOperationException("OOB sweep exceeds 1,000,000 points.");
                    for (long point = 0; point < count; point++)
                    {
                        double freqMHz = fromMHz + point; // 1 MHz grid relative to this range start
                        bool excluded = false;
                        foreach (double[] exclusion in exclusions)
                        {
                            if (freqMHz >= exclusion[0] - 1e-9 &&
                                freqMHz <= exclusion[1] + 1e-9)
                            {
                                if (mode == "OOB2" && freqMHz <= 70)
                                {
                                    Log(string.Format(
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        "[OOB2 DEBUG] CW={0:F3} MHz excluded by [{1:F3}, {2:F3}] MHz",
                                        freqMHz,
                                        exclusion[0],
                                        exclusion[1]));
                                }

                                excluded = true;
                                break;
                            }
                        }
                        if (excluded) { Log("[OOB] SKIP excluded CW = " + freqMHz.ToString("F3", culture) + " MHz"); continue; }
                        long freqHz = checked((long)Math.Round(freqMHz * 1e6));
                        if (seenHz.Add(freqHz)) points.Add(Tuple.Create(range[0], freqHz));
                    }
                }
                if (points.Count == 0)
                {
                    Log("OOB: all configured frequencies excluded; no measurement performed.", LogLevel.ERROR);
                    return;
                }
                Log("OOB valid CW points: " + points.Count);

                vsg2.TypeOfSignal = "CW";
                //vsg2.LoadUserCorrDataSGS100A(@"/var/user/RxBlocking");
                // Configure first VALID frequency before starting BBU measurement.
                vsg2.SetCWWaveform(isPower, points[0].Item2.ToString(culture));
              // vsg2.LogUserCorrectionSGS100A(isPower, points[0].Item2);

                for (int startAttempt = 0; startAttempt < 2 && !testMacStarted; startAttempt++)
                {
                    if (worker.CancellationPending) return;
                    if (startAttempt > 0)
                    {
                        bbu.STOP_L1TESTMAC();
                        for (int ms = 0; ms < 30000; ms += 100)
                        { if (worker.CancellationPending) return; System.Threading.Thread.Sleep(500); }
                    }
                    testMacStarted = bbu.STR_RXSENS_V2(BW, WsRBOffset, currPort.StreamIndex);
                    L1TestMacV2Start = testMacStarted;
                }
                if (!testMacStarted) { Log("OOB: cannot start RXSENS_V2.", LogLevel.ERROR); return; }

                var syncWatch = System.Diagnostics.Stopwatch.StartNew();
                bool synchronized = false;
                while (syncWatch.ElapsedMilliseconds < syncTimeoutMs)
                {
                    if (worker.CancellationPending) return;
                    if (bbu.VALIDATEFILEOUT_RXSENS_V2()) { synchronized = true; break; }
                    System.Threading.Thread.Sleep(100);
                }
                if (!synchronized) { Log("OOB: timeout waiting for BBU file-out.", LogLevel.ERROR); return; }

                foreach (var cw in points)
                {
                    if (worker.CancellationPending) return;
                    string freqText = cw.Item2.ToString(culture);
                    double freqMHz = cw.Item2 / 1e6;
                    vsg2.SetCWWaveform(isPower, freqText);
                    vsg2.LogUserCorrectionSGS100A(isPower, cw.Item2);
                    Log("OOB: CW = " + freqMHz.ToString("F3", culture) + " MHz");
                    for (int waited = 0; waited < dwellMs; waited += 500)
                    { if (worker.CancellationPending) return; System.Threading.Thread.Sleep(Math.Min(500, dwellMs - waited)); }

                    bool valid = false;
                    string throughput = "ERROR";
                    for (int attempt = 0; attempt < maxReadAttempts; attempt++)
                    {
                        if (worker.CancellationPending) return;
                        bbu.RXSENS_V2(out throughput);
                       // 131126double tempTP = GetthroughputFromString(throughput, setPort);
                        //if (tempTP > 0 && tempTP <= 100) { valid = true; break; }
                        double tempTP = GetthroughputFromString(throughput, setPort);

                        if (!double.IsNaN(tempTP) &&
                            !double.IsInfinity(tempTP) &&
                            tempTP >= 0 &&
                            tempTP <= 100)
                        {
                            valid = true;
                            break;
                        }
                        if (attempt + 1 < maxReadAttempts)
                            for (int waited = 0; waited < dwellMs; waited += 500)
                            { if (worker.CancellationPending) return; 
                                System.Threading.Thread.Sleep(Math.Min(500, dwellMs - waited)); }
                    }
                    // 131126 if (!valid)
                    //  { Log("OOB: no valid throughput at " + freqMHz.ToString("F3", culture) + " MHz.", LogLevel.ERROR); return; }

                    if (!valid)
                    {
                        hasInvalidPoint = true;
                        invalidPointCount++;

                        Log(
                            "[OOB] INVALID throughput at CW = " +
                            freqMHz.ToString("F3", culture) +
                            " MHz after " + maxReadAttempts +
                            " attempts. Continue sweep.",
                            LogLevel.ERROR);

                        // Bỏ qua điểm không có dữ liệu hợp lệ,
                        // chuyển sang tần số CW tiếp theo.
                        continue;
                    }

                    string subName = string.IsNullOrEmpty(cw.Item1) ? "" : "_" + cw.Item1;
                    double currentTP;
                    writeCSVThroughputValue(mode + "_P" + setPort + subName,
                        setPort, wsFreq, wsPower, WsRBOffset, freqText,
                        isPower, IsRBOffset, throughput, out currentTP);
                    if (currentTP >= 0 && currentTP <= 100 && (minTP < 0 || currentTP < minTP))
                        minTP = currentTP;
                    mainForm.PopulateMeasurementChart("Out-of-band Blocking: Port " + setPort,
                        freqMHz.ToString(culture), currentTP.ToString(culture), "95", "100");
                    Log("OOB min throughput = " + minTP + "%");
                }
                //131126
                //  completed = true;
                // TP = minTP;
                // Log("OOB completed. Min TP = " + minTP + "%", minTP >= 95 ? LogLevel.SUCCESS : LogLevel.ERROR);

                completed = true;

                // Nếu có điểm không đọc được dữ liệu,
                // không được kết luận PASS cho cả dải.
                TP = hasInvalidPoint ? -1 : minTP;

                Log(
                    "[OOB] Sweep completed. " +
                    "Min valid TP = " + minTP + "%, " +
                    "Invalid points = " + invalidPointCount,
                    TP >= 95 ? LogLevel.SUCCESS : LogLevel.ERROR);

            }
            catch (Exception ex) { Log("OOB stopped: " + ex.ToString(), LogLevel.ERROR); }
            finally
            {
                if (testMacStarted || L1TestMacV2Start)
                    try { bbu.STOP_L1TESTMAC(); }
                    catch (Exception ex) { Log("STOP_L1TESTMAC: " + ex.Message, LogLevel.ERROR); }
                L1TestMacV2Start = false;
                try { vsg2.OnOffSignal(0); }
                catch (Exception ex) { Log("CW OFF: " + ex.Message, LogLevel.ERROR); }
                if (!completed) TP = -1;
            }
        }
private List<string[]> BuildSupportedUlExclusionsMHz()
{
    List<string[]> exclusions = new List<string[]>();

    double ulBottomMHz = mainForm.txtOperatingFreqFrom;
    double ulTopMHz = mainForm.txtOperatingFreqTo;
    double deltaFoobMHz = mainForm.deltaFoobValue;

    if (deltaFoobMHz < 0)
        throw new InvalidOperationException(
            "Delta Foob must not be negative.");

    double excludeStartMHz = ulBottomMHz - deltaFoobMHz/1e6;
    double excludeEndMHz = ulTopMHz + deltaFoobMHz/1e6;

    exclusions.Add(new string[]
    {
        excludeStartMHz.ToString(
            System.Globalization.CultureInfo.InvariantCulture),
        excludeEndMHz.ToString(
            System.Globalization.CultureInfo.InvariantCulture)
    });

    Log(string.Format(
        System.Globalization.CultureInfo.InvariantCulture,
        "[OOB] n40 UL exclusion: [{0}, {1}] MHz, DeltaFoob={2} MHz",
        excludeStartMHz,
        excludeEndMHz,
        deltaFoobMHz/1e6));

    return exclusions;
}
        //private void oobMeasurementInSequence(BBU bbu, VSG vsg2, string mode, int setPort, double step, int timeSequence, string wsFreq, string wsPower, string WsRBOffset, string isFreq, string isPower, string IsRBOffset, List<string[]> measRanges, List<string[]> exceptRanges, string BW, out double TP)
        //{
        //    TP = -1;
        //    vsg2.TypeOfSignal = "CW";
        //    double minTP = -1; //out of range 0-100
        //    double sFreqChange = 0;
        //    double freqIS = 0;
        //    int testCount = 0;
        //    int errCount = 0; // 28.3.2024 - counting loop mất đồng bộ
        //    string throughput = "ERROR";
        //    // Get the sweep range
        //    List<string[]> sweepRange = GetSweepRangeWithID(measRanges, exceptRanges);
        //    //printing sweepRange
        //    Log($"Sweeping Frequency Range [{sweepRange.Count}]:");
        //    //       foreach (var item in sweepRange)
        //    //     {
        //    //            Log($"[{item[0]}, {item[1]}, {item[2]}]");
        //    //   }
        //    //Load Ucorr SGS100A
        //    string ucorrPath = @"/var/user/RxBlocking";
        //    vsg2.LoadUserCorrDataSGS100A(ucorrPath);
        //    //initial CW signal
        //    if (sweepRange == null || sweepRange.Count == 0)
        //    {
        //        Log($"No frequency value sweep, Measurement finish!");
        //    }
        //    else
        //    {
        //        sFreqChange = double.Parse(sweepRange[0][1]) * 1e6; //in Hz
        //        vsg2.SetCWWaveform(isPower, sFreqChange.ToString());
        //        //loop
        //        bool oobMode = true; // mode OOB
        //        if (oobMode)
        //        {
        //            //Start RXSens_V2
        //            //bbu.STR_RXSENS_V2();
        //            L1TestMacV2Start = bbu.STR_RXSENS_V2(BW, WsRBOffset, currPort.StreamIndex);
        //            //01.4.2024 - neu start RxSen_V2 fail => chay STP_RXSEN roi cho khoang 30s roi chay lai 1 lan nua, neu khong duoc thi thoat
        //            if (!L1TestMacV2Start)
        //            {
        //                bbu.STOP_L1TESTMAC();
        //                //waiting for 30s to restart L1TestMac again
        //                countdownTimerInSecond(30, "Waiting for L1TestMac restart again");
        //                L1TestMacV2Start = bbu.STR_RXSENS_V2(BW, WsRBOffset, currPort.StreamIndex);
        //            }

        //            if (L1TestMacV2Start)
        //            {
        //                //DateTime delay;
        //                //TimeSpan remainingTime;
        //                //delay = DateTime.Now.AddSeconds(25); //waiting 25s
        //                Log("Setting Measurement Environment...");
        //                //while (DateTime.Now <= delay)
        //                //{
        //                //    remainingTime = delay - DateTime.Now;
        //                //    Console.Write($"\rWaiting for preseting Testmac: {remainingTime.ToString("ss")}s..."); //for debugging                            
        //                //    System.Threading.Thread.Sleep(10); //de do phan giai 10ms
        //                //}
        //                //waiting for 30s to restart L1TestMac again
        //                countdownTimerInSecond(30, "Waiting for preseting Testmac");

        //                //Check if throughut Fileout is existing
        //                Log("Start Synchronizating Time period with Testmac: " + DateTime.Now.ToString()); //for debugging
        //                while (!bbu.VALIDATEFILEOUT_RXSENS_V2())
        //                {
        //                    if (mainForm.measurementBackgroundWorker.CancellationPending)
        //                    {
        //                        TP = (minTP <= 100 && minTP >= 0) ? minTP : -1; //minTP out of range 100 ==> TP=0;
        //                        break;
        //                    }
        //                    System.Threading.Thread.Sleep(10);
        //                }
        //                Log("Synchronized Time Period is ready. Start Measurement!");

        //                //start loop                
        //                Log("------------------------------------------------------------");
        //                //======== Setup Cycle ======
        //                int stepTime = timeSequence; // (in ms) - 21.2.2024
        //                bool firstStep = true; //28.3.2024 - check if first step
        //                timerSequence = new Timer(CWFrequencyLoop, null, 0, stepTime);
        //                //sweep Freq in sweepRange
        //                bool isOAMsyncFail = false; //28.3.2024 - using to detect OAM not responding
        //                int listIndex = 0; //28.3.2024 - store index of range in sweepRang
        //                foreach (var range in sweepRange)
        //                {
        //                    listIndex += 1;
        //                    double sweepFrom = double.Parse(range[1]) * 1e6;
        //                    double sweepTo = double.Parse(range[2]) * 1e6;
        //                    sFreqChange = sweepFrom;
        //                    string isFreqSweep = sFreqChange.ToString();
        //                    errCount = 0;
        //                    //break foreach if measurementBackgroundWorker is canceled
        //                    if (mainForm.measurementBackgroundWorker.CancellationPending)
        //                    {
        //                        TP = (minTP <= 100 && minTP >= 0) ? minTP : -1; //minTP out of range 100 ==> TP=0;
        //                        break;
        //                    }
        //                    Log($"Measurement Frequency Range: {sweepRange[0]}: {sweepRange[1]} MHz - {sweepRange[2]} MHz");
        //                    while (sFreqChange <= sweepTo)
        //                    {
        //                        if (flagStep)
        //                        {
        //                            if (!firstStep) //28.3.2024 - bo first step
        //                            {
        //                                //check background worker Cancelled
        //                                if (mainForm.measurementBackgroundWorker.CancellationPending)
        //                                {
        //                                    TP = (minTP <= 100 && minTP >= 0) ? minTP : -1; //minTP out of range 100 ==> TP=0;
        //                                    break;
        //                                }
        //                                Log($"[{DateTime.Now.ToString("hh:mm:ss:fff")}] LOG: Set frequency of CW signal to: {sFreqChange / 1e6} MHz");
        //                                //Đọc throughput về
        //                                bbu.RXSENS_V2(out throughput);
        //                                System.Threading.Thread.Sleep(1);
        //                                double tempTP = GetthroughputFromString(throughput, setPort);
        //                                if ((tempTP >= 95 && tempTP <= 100) || testCount == 3)
        //                                {
        //                                    //28.3.2024 - Truong hop van doc dc ve mang throughput
        //                                    //5.4.2024 - tạm thời tách TH throughput = 0 coi như là lỗi mất đồng bộ
        //                                    if (tempTP > 0)
        //                                    {
        //                                        string subName = (range[0] == null || range[0] == "") ? "" : ("_" + range[0]);
        //                                        writeCSVThroughputValue($"{mode}_P{setPort}{subName}", setPort, wsFreq, wsPower, WsRBOffset, isFreqSweep, isPower, IsRBOffset, throughput, out TP);
        //                                        //luu lại giá trị TP nhỏ nhất
        //                                        if (minTP >= TP || minTP == -1)
        //                                        {
        //                                            minTP = TP;
        //                                            freqIS = sFreqChange;
        //                                        }
        //                                        Log($"[OVERALL] Min throughput = {minTP}% @ CW Freq. = {freqIS / 1e6} MHz");
        //                                        DashedLineLog("----");
        //                                        mainForm.PopulateMeasurementChart($"Out-of-band Blocking: Port {setPort}", (sFreqChange / 1e6).ToString(), TP.ToString(), "95", "100");
        //                                        //Change CW frequency for next step                            
        //                                        sFreqChange += step;
        //                                        //Kiem tra lai gia tri tan so va Phat nhieu
        //                                        if (sFreqChange <= sweepTo)
        //                                        {
        //                                            isFreqSweep = sFreqChange.ToString();
        //                                            vsg2.SetCWWaveform(isPower, isFreqSweep);
        //                                        }
        //                                        else if (listIndex < sweepRange.Count())//neu gia tri Freq la cuoi cung cua range => Set sang range tiep theo
        //                                        {
        //                                            sFreqChange = double.Parse(sweepRange[listIndex][1]) * 1e6; //nhay toi gia tri tiep theo
        //                                            isFreqSweep = sFreqChange.ToString();
        //                                            vsg2.SetCWWaveform(isPower, isFreqSweep);
        //                                        }
        //                                        //reset testCount ve 0
        //                                        testCount = 0;
        //                                        errCount = 0;
        //                                        isOAMsyncFail = false;
        //                                    }
        //                                    else   // truong hop khong doc duoc ve mang throughput do loi mat OAM sau khi da test lai 3 lan
        //                                    {
        //                                        errCount += 1;
        //                                        //Console.WriteLine($"\rWaiting for synchronization.....{errCount}");
        //                                        if (errCount >= 1000)  //28.3.2024 - Sau 1000 vong lap, neu van loi thi bao loi
        //                                        {
        //                                            isOAMsyncFail = true;
        //                                            Log("Server is not responding, measurement stop!");
        //                                            errCount = 0;
        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    Log($"TP = {tempTP}% => Retest: {testCount}");
        //                                    testCount += 1;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                firstStep = false;
        //                            }
        //                            //xoa flag
        //                            flagStep = false;
        //                        }
        //                    }
        //                    if (isOAMsyncFail)  //28.3.2024 - Sau 100 vong lap, neu van loi thi bao loi va thoat bai do
        //                    {
        //                        break;
        //                    }
        //                }
        //                //Ket thuc chay vong lap
        //                timerSequence.Dispose();  //Stop timer
        //                bbu.STOP_L1TESTMAC();
        //                L1TestMacV2Start = false;
        //                vsg2.OnOffSignal(0);
        //                //gan gia tri throughput nho nhat de in ra ngoai ListView
        //                DashedLineLog("---");
        //                Log("Measurement finished!");
        //                TP = minTP;
        //                if (TP < 95) Log("LOG: Throughput < 95% => FAILED!", LogLevel.FAILED);
        //                else Log("LOG: Throughput > 95% => PASSED!", LogLevel.SUCCESS);
        //                DashedLineLog("---");
        //                //28.3.2024
        //                if (isOAMsyncFail)
        //                {
        //                    MessageBox.Show("OAM is not responding, measurement stop!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //                }
        //            }
        //            else
        //            {
        //                bbu.STOP_L1TESTMAC();
        //                L1TestMacV2Start = false;
        //                Log("Start RXSENS_V2 Fail! Wait for a while before next measurement!");
        //            }
        //        }
        //    }
        //}
        //26.2.2024 - Viet lại hàm
        private void writeCSVThroughputValue(string mode, int port, string wsFreq, string wsPower, string wsRBOffset, string isFreq, string isPower, string isRBOffset, string throughput, out double TP)
        {
            //int index;
            //tmpArr = throughput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ////========== 16.2.2024 - sua lai doan code ======
            //index = (port - 1) % tmpArr.Length;
            //double.TryParse(tmpArr[index], out TP);
            TP = GetthroughputFromString(throughput, port);
            //Data collection
            List<string> data = new List<string> { DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"), mainForm.RruType, mode, port.ToString(), wsFreq, wsPower, wsRBOffset, isFreq, isPower, isRBOffset, TP.ToString() };

            if (TP < 95)
            {
                Log($"Throughput = {TP}% < 95% => FAILED!", LogLevel.FAILED);
            }
            else
            {
                Log($"Throughput = {TP}% > 95% => PASSED!", LogLevel.SUCCESS);
            }

            //Write to CSV
            mainForm.ULCsvRecord(mode, csvHeader, data);
            //Log("-----------------------------------------------------------");
        }
        //private double GetthroughputFromString(string tp, int port)
        //{
        //    double tmpTP = -1;
        //    tmpArr = tp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //    //28.3.2024 - sua lai code xu ly neu tra ve chuoi null
        //    if (tmpArr != null && tmpArr.Length > 0)
        //    {
        //        //int index = (port - 1) % tmpArr.Length;
        //        //double.TryParse(tmpArr[index], out tmpTP);
        //        //Redhat - get throughput
        //        double.TryParse(tmpArr[(currPort.StreamIndex - 1) % tmpArr.Length], out tmpTP);
        //    }
        //    return tmpTP;
        //}

        private double GetthroughputFromString(string tp, int port)
        {
            if (string.IsNullOrWhiteSpace(tp))
                return -1;

            string tpText = tp.Replace("%", "").Trim();

            double tpValue;

            if (double.TryParse(
                tpText,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out tpValue))
            {
                if (tpValue >= 0 && tpValue <= 100)
                    return tpValue;
            }

            Log("[Throughput] Invalid TP: " + tp,
                LogLevel.ERROR);

            return -1;
        }

        private void getISCoLocationRangeFromExcel()
        {
            //xoa list
            oobCoISsweepRanges.Clear();

            string filename = mainForm.directionPathTemplate + "OQC_UL_SETUP_3GPP.xlsx";
            Log($"Read IS Co-location bands information from path:{filename}");
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filename);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[2]; //out-of-band: NR co-location bands are defined in sheet2
            Excel.Range xlRange = xlWorksheet.UsedRange;
            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;
            for (int i = 2; i <= rowCount; i++)
            {
                if (xlRange.Cells[i, 1].Value != null)
                {
                    //Log($"[from Excel]{xlRange.Cells[i, 1].Value}: From {xlRange.Cells[i, 2].Value} MHz to {xlRange.Cells[i, 3].Value} MHz");
                    oobCoISsweepRanges.Add(new string[] { xlRange.Cells[i, 1].Value.ToString(), xlRange.Cells[i, 2].Value.ToString(), xlRange.Cells[i, 3].Value.ToString() });
                }
            }
            //for debugging
            Log("[Out-of-band Measurement] NR bands Co-location:");
            foreach (var range in oobCoISsweepRanges)
            {
                Log($"{range[0]}: From {range[1]} MHz To {range[2]} MHz");
            }
        }

        public List<double[]> GetSweepRange(List<string[]> measRange, List<string[]> exceptRange) //note: measRange element has 3 dimensions, exceptRange's has 2 dimension
        {
            // Sort the ranges
            measRange = measRange.OrderBy(r => r[1]).ToList();
            exceptRange = exceptRange.OrderBy(r => r[0]).ToList();

            //ranges after sorting
            Log("Meas Range after sorting:");
            foreach (var range in measRange)
            {
                Log($"[{range[1]}, {range[2]}]");
            }
            // Print the Except range
            Log("Except Range after sorting:");
            foreach (var range in exceptRange)
            {
                Log($"[{range[0]}, {range[1]}]");
            }

            List<double[]> sweepRange = new List<double[]>();

            if (exceptRange == null || exceptRange.Count == 0)
            {
                // If exceptRange is null or empty, sweep range is the same as measRange
                foreach (var mRange in measRange)
                {
                    sweepRange.Add(new double[] { double.Parse(mRange[1]), double.Parse(mRange[2]) });
                }
                return sweepRange;
            }
            bool resetLoop = false;
            foreach (var eRange in exceptRange)
            {
                bool overlapped = false;

                foreach (var mRange in measRange)
                {
                    if (double.Parse(mRange[1]) <= double.Parse(eRange[1]) && double.Parse(mRange[2]) >= double.Parse(eRange[0]))
                    {
                        // There's an overlap, adjust the range
                        overlapped = true;
                        if (double.Parse(mRange[1]) < double.Parse(eRange[0]))
                            sweepRange.Add(new double[] { double.Parse(mRange[1]), double.Parse(eRange[0]) });
                        if (double.Parse(mRange[2]) > double.Parse(eRange[1]))
                            sweepRange.Add(new double[] { double.Parse(eRange[1]), double.Parse(mRange[2]) });
                    }
                    //if eRange is overlap of all mRange
                    if (sweepRange == null || sweepRange.Count == 0)
                    {
                        Log("excepRange overlapped measRange");
                        resetLoop = true;
                        break;
                    }
                }

                if (!overlapped)
                {
                    // If no overlap, add the entire range to the sweep range
                    sweepRange.Add(new double[] { double.Parse(eRange[0]), double.Parse(eRange[1]) }); //kiem tra code nay
                }
                if (resetLoop) break;
            }

            //debugging - print sweepRange
            Log("Sweep Range:");
            foreach (var range in sweepRange)
            {
                Log($"{range[0]}: From {range[1]} MHz To {range[2]} MHz");
            }

            return sweepRange;
        }


        //get sweeprange with ID beyond ID of measRange
        //public List<string[]> GetSweepRangeWithID(List<string[]> measRange, List<string[]> exceptRange) //note: measRange element has 3 dimensions, exceptRange's has 2 dimension
        //{
        //    List<string[]> sweepRange = new List<string[]>();

        //    if (exceptRange == null || exceptRange.Count == 0)
        //    {
        //        // If exceptRange is null or empty, sweep range is the same as measRange
        //        foreach (var mRange in measRange)
        //        {
        //            sweepRange.Add(new string[] { mRange[0], mRange[1], mRange[2] });
        //        }
        //    }
        //    else
        //    {
        //        foreach (var meas in measRange)
        //        {
        //            sweepRange.Add(new string[] { meas[0], meas[1], meas[2] });
        //            //Console.WriteLine($"Add to sweep range: [{meas[0]}, {meas[1]}, {meas[2]}]");
        //            foreach (var except in exceptRange)
        //            {
        //                PopulateRange(sweepRange, except);
        //            }
        //        }
        //    }
        //    return sweepRange;
        //}


        public List<string[]> GetSweepRangeWithID(List<string[]> measRange, List<string[]> exceptRange)
        {
            List<string[]> sweepRange = new List<string[]>();

            if (measRange == null || measRange.Count == 0)
                return sweepRange;

            // copy all measurement ranges (preserve IDs)
            foreach (var mRange in measRange)
            {
                if (mRange != null && mRange.Length >= 3)
                    sweepRange.Add(new string[] { mRange[0], mRange[1], mRange[2] });
            }

            if (exceptRange == null || exceptRange.Count == 0)
                return sweepRange;

            // apply each except once to the whole sweepRange
            foreach (var except in exceptRange)
            {
                PopulateRange(sweepRange, except);
            }

            return sweepRange;
        }
        //private void PopulateRange(List<string[]> sourceRange, string[] subtract)
        //{
        //    List<string[]> temp = new List<string[]>();
        //    foreach (var item in sourceRange)
        //    {
        //        int startMeas = int.Parse(item[1]);
        //        int endMeas = int.Parse(item[2]);
        //        int startExcept = int.Parse(subtract[0]);
        //        int endExcept = int.Parse(subtract[1]);

        //        if (startMeas >= endExcept || endMeas <= startExcept)
        //        {
        //            temp.Add(new string[] { item[0], item[1], item[2] });
        //        }
        //        else
        //        {
        //            if (startMeas < startExcept)
        //            {
        //                temp.Add(new string[] { item[0], $"{startMeas}", $"{startExcept}" });
        //            }
        //            if (endMeas > endExcept)
        //            {
        //                temp.Add(new string[] { item[0], $"{endExcept}", $"{endMeas}" });
        //            }
        //        }
        //    }
        //    //renew sourceRange
        //    sourceRange.Clear();
        //    foreach (var mRange in temp)
        //    {
        //        sourceRange.Add(new string[] { mRange[0], mRange[1], mRange[2] });
        //    }
        //    ////debugging
        //    //foreach (var xRange in sourceRange)
        //    //{
        //    //    Log($"read sourceRange: [{xRange[0]}, {xRange[1]}, {xRange[2]}]");
        //    //}
        //}

        private void PopulateRange(List<string[]> sourceRange, string[] subtract)
        {
            if (sourceRange == null) return;
            if (subtract == null || subtract.Length < 2) return;

            var invariant = System.Globalization.CultureInfo.InvariantCulture;
            double startExcept, endExcept;
            if (!double.TryParse(subtract[0], System.Globalization.NumberStyles.Any, invariant, out startExcept) ||
                !double.TryParse(subtract[1], System.Globalization.NumberStyles.Any, invariant, out endExcept))
            {
                double.TryParse(subtract[0], out startExcept);
                double.TryParse(subtract[1], out endExcept);
            }

            var temp = new List<string[]>();
            foreach (var item in sourceRange)
            {
                if (item == null || item.Length < 3) continue;

                double startMeas, endMeas;
                if (!double.TryParse(item[1], System.Globalization.NumberStyles.Any, invariant, out startMeas) ||
                    !double.TryParse(item[2], System.Globalization.NumberStyles.Any, invariant, out endMeas))
                {
                    double.TryParse(item[1], out startMeas);
                    double.TryParse(item[2], out endMeas);
                }

                if (startMeas >= endExcept || endMeas <= startExcept)
                {
                    temp.Add(new string[] { item[0], item[1], item[2] });
                }
                else
                {
                    if (startMeas < startExcept)
                        temp.Add(new string[] { item[0], startMeas.ToString(invariant), startExcept.ToString(invariant) });
                    if (endMeas > endExcept)
                        temp.Add(new string[] { item[0], endExcept.ToString(invariant), endMeas.ToString(invariant) });
                }
            }

            sourceRange.Clear();
            foreach (var r in temp) sourceRange.Add(new string[] { r[0], r[1], r[2] });
        }
        private static void CWFrequencyLoop(object state)
        {
            flagStep = true;
        }
        //private static void NRFrequencyLoop(object state)
        //{
        //    flagStep = true;
        //}
        private static int inbTimerTicks = 0;
        private static void NRFrequencyLoop(object state)
        {
            System.Threading.Interlocked.Increment(ref inbTimerTicks);
            flagStep = true;
        }
        private void countdownTimerInSecond(int iSec, string message)
        {
            TimeSpan remainingTime;
            DateTime delay = DateTime.Now.AddSeconds(iSec);
            while (DateTime.Now <= delay)
            {
                remainingTime = delay - DateTime.Now;
                //Console.Write($"\r{message} in {remainingTime.ToString("ss")}s...");
                Log($"\r{message} in {remainingTime.ToString("ss")}s...");
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}