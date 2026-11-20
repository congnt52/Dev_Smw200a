using _5GAutoTool.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _5GAutoTool
{
    class threeVSG
    {
        public Form5GAT mainForm;
        public threeVSG(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "";
        public void Reset()
        {
            Mode = "";
        }
        string[] tmpArr = null;
        List<string> csvHeader = new List<string> { "DATE", "TIME", "TYPE", "SPEC", "PORT", "WS FREQ", "WS PWR LVL", "IS1 FREQ", "IS1 PWR LVL", "IS2 FREQ", "IS2 PWR LVL", "THROUGHPUT" };
        PortMapping currPort;

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "threeVSG");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void Measurement(VSG vsg1, VSG vsg2, VSG vsg3, BBU bbu, string mode, string WsRBOffset, string IsRBOffset, double step, string setAtt, int setPort, string wsPowerStr, string wsFreq, string isPowerStr, string isFreq, string cwFreq, string Bandwidth,int rf, out double TP)

        {
           // TP = 0;
           // string throughput;

            TP = double.NaN;
            string throughput = null;
            //ServerRedhat - finding PortMapping
            currPort = mainForm.PortMappings.FirstOrDefault(item => item.Port == setPort);
            //old Switch Group
            //bbu.SwitchGroup(currPort.XRanIndex.ToString());
            //

            //Test conection no devices
            if (currPort == null)
                throw new InvalidOperationException(
                    "Port mapping not found: " + setPort);

            try
            {
                bbu.SwitchGroup(currPort.XRanIndex.ToString());
            }
            catch (Exception ex)
            {
                if (!mainForm.AllowImdNoDevice)
                    throw;

                Log("[NO DEVICE] SwitchGroup: " + ex.Message, LogLevel.WARN);
            }

            //double _setAtt = double.Parse(setAtt);
            double _wsPower = double.Parse(wsPowerStr);
            double _isPower = double.Parse(isPowerStr);
            // Cw signal power equal Interferting signal power
            double _cwPower = double.Parse(isPowerStr);
            double _wsATT = vsg1.AttenuatorLevel;
            double _isATT = vsg2.AttenuatorLevel;
            double _cwATT = vsg3.AttenuatorLevel;
            //vsg1.SetWSIMD(wsPowerStr, isFreq, IsRBOffset, Bandwidth);
            Log($"{vsg1.Name}: Expected Power Level={_wsPower} dBm");
          Log($"{vsg2.Name}: Expected Power Level={_isPower} dBm");
           Log($"{vsg3.Name}: Expected Power Level={_cwPower} dBm");
            // vsg1.SetWSACSWaveform(wsPowerStr, wsFreq, WsRBOffset, Bandwidth);
            // vsg1.SetWSIMD(wsPowerStr, wsFreq, WsRBOffset, Bandwidth);
           // vsg1.SetWSACSWaveform(wsPowerStr, wsFreq, WsRBOffset, Bandwidth);
            string wsPower = (_wsPower + _wsATT).ToString(); //công suất tai cong output cua VSG1 WS
            string isPower = (_isPower + _isATT).ToString(); //công suất tai cong output cua VSG2 IS
            // Set cw power to VSG3
            string cwPower = (_cwPower + _cwATT).ToString();
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending)
                    return;

                string direction =
                    double.Parse(isFreq) > double.Parse(wsFreq)
                        ? "UPPER"
                        : "LOWER";

                Log($"[{mode}] PORT{setPort} {direction}: START");

                Log($"[{mode}] WS={wsFreq} Hz, " +
                    $"IS={isFreq} Hz, CW={cwFreq} Hz");

                // Configure interfering signal
                //try
                //{
                //    if (mode == "GenIMD")
                //    {
                //        if (direction == "UPPER")

                //        vsg2.SetGeneralIMDWaveformUpper(
                //                isPower, isFreq, Bandwidth);
                //        else
                //            vsg2.SetGeneralIMDWaveformLower(
                //                isPower, isFreq, Bandwidth);
                //    }
                //    else if (mode == "NBIMD")
                //    {
                //        if (direction == "UPPER")
                //            vsg2.SetNBUpperIMDWaveform(
                //                isPower, isFreq, IsRBOffset, Bandwidth);
                //        else
                //            vsg2.SetNBLowerIMDWaveform(
                //                isPower, isFreq, IsRBOffset, Bandwidth);
                //    }

                //    Log($"[{mode}] PORT{setPort} {direction}: VSG2 configured");
                //}
                //catch (Exception ex)
                //{
                //    Log($"[{mode}] PORT{setPort} VSG2 ERROR: " +
                //        ex.Message, LogLevel.ERROR);

                //    if (!mainForm.AllowImdNoDevice)
                //        return;
                //}
                // Configure WS and IS
                try
                {
                    if (direction == "UPPER")
                        vsg1.LoadIMDUpper();   // WS RB Offset = 82
                    else
                        vsg1.LoadIMDLower();   // WS RB Offset = 0

                    if (mode == "GenIMD")
                    {
                        if (direction == "UPPER")
                            vsg2.SetGeneralIMDWaveformUpper(
                                isPower, isFreq, Bandwidth);
                        else
                            vsg2.SetGeneralIMDWaveformLower(
                                isPower, isFreq, Bandwidth);
                    }
                    else if (mode == "NBIMD")
                    {
                        if (direction == "UPPER")
                            vsg2.SetNBUpperIMDWaveform(
                                isPower, isFreq, IsRBOffset, Bandwidth);
                        else
                            vsg2.SetNBLowerIMDWaveform(
                                isPower, isFreq, IsRBOffset, Bandwidth);
                    }

                    Log($"[{mode}] PORT{setPort} {direction}: VSG1 and VSG2 configured");
                }
                catch (Exception ex)
                {
                    Log($"[{mode}] PORT{setPort} ERROR: {ex.Message}", LogLevel.ERROR);

                    if (!mainForm.AllowImdNoDevice)
                        return;
                }

                if (mainForm.measurementBackgroundWorker.CancellationPending)
                    return;

                // Configure CW
                try
                {
                    vsg3.SetCWWaveform(cwPower, cwFreq, rf);

                    Log($"[{mode}] PORT{setPort} {direction}: VSG3 configured");
                }
                catch (Exception ex)
                {
                    Log($"[{mode}] PORT{setPort} VSG3 ERROR: " +
                        ex.Message, LogLevel.ERROR);

                    if (!mainForm.AllowImdNoDevice)
                        return;
                }

                if (mainForm.measurementBackgroundWorker.CancellationPending)
                    return;

                // Read actual throughput from BBU
                try
                {
                    Log($"[{mode}] PORT{setPort} {direction}: Reading TP");

                    bbu.NEW_RXSENS(
                        Bandwidth,
                        WsRBOffset,
                        currPort.StreamIndex,
                        out throughput);

                    Log($"[{mode}] PORT{setPort} {direction}: " +
                        $"Raw TP = [{throughput ?? "NULL"}]");
                }
                catch (Exception ex)
                {
                    Log($"[{mode}] PORT{setPort} {direction}: " +
                        $"BBU ERROR: {ex.Message}", LogLevel.ERROR);
                }

                if (!string.IsNullOrWhiteSpace(throughput))
                {
                    try
                    {
                        writeCSVThroughputValue(
                            mode, setPort,
                            wsFreq, wsPowerStr,
                            isFreq, isPowerStr,
                            cwFreq, isPowerStr,
                            throughput, out TP);
                    }
                    catch (Exception ex)
                    {
                        TP = double.NaN;

                        Log($"[{mode}] PORT{setPort} {direction}: " +
                            $"TP parse ERROR: {ex.Message}",
                            LogLevel.ERROR);
                    }
                }

                if (double.IsNaN(TP))
                {
                    Log($"[{mode}] PORT{setPort} {direction}: " +
                        "NOT MEASURED", LogLevel.ERROR);
                }
                else
                {
                    Log($"[{mode}] PORT{setPort} {direction}: " +
                        $"TP={TP:F2}%",
                        TP >= 95 ? LogLevel.SUCCESS : LogLevel.FAILED);
                }
            }
            catch (Exception ex)
            {
                TP = double.NaN;

                Log($"[{mode}] PORT{setPort}: " +
                    $"Measurement ERROR: {ex.Message}",
                    LogLevel.ERROR);
            }


            //        try
            //        {
            //            //int port = 1;
            //            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            //            System.Threading.Thread.Sleep(1);
            //            //doc gia tri suy hao cua VSGs

            //            // vsg1.SetPsenWaveform(wsPower, wsFreq, WsRBOffset, Bandwidth,TriggerDelay);
            //            if (mode == "GenIMD" && double.Parse(isFreq) < double.Parse(wsFreq))
            //            {
            //                vsg2.SetGeneralIMDWaveformLower(isPower, isFreq, Bandwidth);
            //            }
            //            else if (mode == "GenIMD" && double.Parse(isFreq) > double.Parse(wsFreq))
            //            {
            //                vsg2.SetGeneralIMDWaveformUpper(isPower, isFreq, Bandwidth);
            //            }
            //            else if (mode == "NBIMD" && double.Parse(isFreq) > double.Parse(wsFreq))
            //            {
            //                vsg2.SetNBUpperIMDWaveform(isPower, isFreq, IsRBOffset, Bandwidth);
            //            }
            //            else if (mode == "NBIMD" && double.Parse(isFreq) < double.Parse(wsFreq))
            //            {
            //                vsg2.SetNBLowerIMDWaveform(isPower, isFreq, IsRBOffset, Bandwidth);
            //            }
            //            //vsg1.setParameter(1, wsPower, wsFreq);
            //            // vsg2.setParameter(2, isPower, isFreq,TriggerDelay);
            //            vsg3.SetCWWaveform(cwPower, cwFreq);
            //            //bbu.NEW_RXSENS(out throughput);
            //            /*   bbu.NEW_RXSENS(Bandwidth, WsRBOffset,currPort.StreamIndex, out throughput);
            //               writeCSVThroughputValue(mode, setPort, wsFreq, wsPowerStr, isFreq, isPowerStr, cwFreq, isPowerStr, throughput, out TP);
            //               //tmpArr = throughput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //               //int index = (setPort-1) % tmpArr.Length;
            //               //TP=double.Parse(tmpArr[index]);
            //               ////if (setPort < 9)
            //               ////{
            //               ////    TP = double.Parse(tmpArr[setPort - 1]);
            //               ////}
            //               ////else if (setPort >= 9 && setPort < 17)
            //               ////{
            //               ////    TP = double.Parse(tmpArr[setPort - 9]);
            //               ////}
            //               ////else if (setPort >= 17 && setPort < 25)
            //               ////{
            //               ////    TP = double.Parse(tmpArr[setPort - 17]);
            //               ////}
            //               ////else
            //               ////{
            //               ////    TP = double.Parse(tmpArr[setPort - 25]);
            //               ////}
            //               //string tmp1 = (double.Parse(wsPower) - double.Parse(setAtt)).ToString();
            //               //string tmp2 = (double.Parse(isPower) - double.Parse(setAtt)).ToString();
            //               //Log("LOG: Thoughtput = " + TP + "%" + " WS Power =" + tmp1 + " IS Power =" + tmp2);
            //               //if (TP < 95) Log("LOG: Throughput < 95% => FAILED!");
            //               //else Log("LOG: Throughput > 95% => PASSED!");
            //               //Log("========================================");
            //               //mainForm.csvRecord(mode, setPort.ToString(), wsFreq, tmp1, isFreq, tmp2, cwFreq, tmp2, TP.ToString());

            //               */
            //            Log($"[{mode}] PORT{setPort}: Start reading throughput",
            //LogLevel.INFO);

            //            bbu.NEW_RXSENS(
            //                Bandwidth,
            //                WsRBOffset,
            //                currPort.StreamIndex,
            //                out throughput);

            //            Log($"[{mode}] PORT{setPort}: Raw TP = [{throughput ?? "NULL"}]",
            //                LogLevel.INFO);

            //            if (string.IsNullOrWhiteSpace(throughput))
            //            {
            //                Log($"[{mode}] PORT{setPort}: TP NOT MEASURED - empty response",
            //                    LogLevel.ERROR);
            //            }
            //            else
            //            {
            //                writeCSVThroughputValue(
            //                    mode, setPort, wsFreq, wsPowerStr,
            //                    isFreq, isPowerStr, cwFreq,
            //                    isPowerStr, throughput, out TP);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Log("Cannot find throughput! " + ex.Message, LogLevel.ERROR);
            //        }
        }

        //private void writeCSVThroughputValue(string mode, int port, string wsFreq, string wsPower, string wsRBOffset, string isFreq, string isPower, string isRBOffset, string throughput, out double TP)
        //{
        //    int index;
        //    tmpArr = throughput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //    //========== 16.2.2024 - sua lai doan code ======
        //    //index = (port - 1) % tmpArr.Length;
        //    //double.TryParse(tmpArr[index], out TP);

        //    //Redhat - get throughput
        //    double.TryParse(tmpArr[(currPort.StreamIndex - 1) % tmpArr.Length], out TP);

        //    //Data collection
        //    List<string> data = new List<string> { DateTime.Now.ToString("ddMMyyyy"), DateTime.Now.ToString("HH:mm:ss"), mainForm.RruType, mode, port.ToString(), wsFreq, wsPower, wsRBOffset, isFreq, isPower, isRBOffset, TP.ToString() };

        //    if (TP < 95)
        //    {
        //        Log($"Throughput = {TP}% < 95% => FAILED!", LogLevel.FAILED);
        //    }
        //    else
        //    {
        //        Log($"Throughput = {TP}% > 95% => PASSED!", LogLevel.SUCCESS);
        //    }

        //    //Write to CSV
        //    mainForm.ULCsvRecord(mode, csvHeader, data);
        //}
        private void writeCSVThroughputValue(
    string mode,
    int port,
    string wsFreq,
    string wsPower,
    string wsRBOffset,
    string isFreq,
    string isPower,
    string isRBOffset,
    string throughput,
    out double TP)
        {
            TP = double.NaN;

            if (string.IsNullOrWhiteSpace(throughput))
                throw new FormatException("Empty throughput response");

            tmpArr = throughput.Split(
                new char[] { ' ', '\t', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            if (tmpArr.Length == 0)
                throw new FormatException("No throughput values found");

            if (currPort == null || currPort.StreamIndex <= 0)
                throw new InvalidOperationException(
                    "Invalid port mapping or StreamIndex");

            int index = (currPort.StreamIndex - 1) % tmpArr.Length;

            string tpText = tmpArr[index].Trim().TrimEnd('%');

            double parsedTP;

            bool parsed = double.TryParse(
                tpText,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out parsedTP);

            if (!parsed ||
                double.IsNaN(parsedTP) ||
                double.IsInfinity(parsedTP))
            {
                throw new FormatException(
                    "Invalid throughput value: " + tpText);
            }

            TP = parsedTP;

            List<string> data = new List<string>
    {
        DateTime.Now.ToString("ddMMyyyy"),
        DateTime.Now.ToString("HH:mm:ss"),
        mainForm.RruType,
        mode,
        port.ToString(),
        wsFreq,
        wsPower,
        wsRBOffset,
        isFreq,
        isPower,
        isRBOffset,
        TP.ToString(
            System.Globalization.CultureInfo.InvariantCulture)
    };

            if (TP < 95)
            {
                Log($"Throughput = {TP:F2}% < 95% => FAILED!",
                    LogLevel.FAILED);
            }
            else
            {
                Log($"Throughput = {TP:F2}% >= 95% => PASSED!",
                    LogLevel.SUCCESS);
            }

            mainForm.ULCsvRecord(mode, csvHeader, data);
        }

    }
}