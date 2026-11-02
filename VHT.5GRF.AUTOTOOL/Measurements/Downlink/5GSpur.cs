using System;

namespace _5GAutoTool
{
    class _5GSpur
    {
        public Form5GAT mainForm;
        public _5GSpur(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "", RBW = "", NRTMcheck = "";
        public void Reset()
        {
            sameMode = "";
            RBW = "";
            NRTMcheck = "";
        }
        double?[] tmp = new double?[20];

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GSpur");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void SpurMeasurement(VSA vsa, BBU bbu, string mode, string NRTM, string setAtt, int port, string setFreq, string setBW, string setRBW, out double?[] spur)
        {
            spur = tmp;
            try
            {
                if (mode == "Rx-Spurious")
                {
                    vsa.LoadMeasurementSetup("Spur_100MHz_TDD", setFreq, mode, setRBW);
                    System.Threading.Thread.Sleep(5000);
                    vsa.SingleSweepOnOff(0);
                    System.Threading.Thread.Sleep(1000);
                    vsa.SpurMeasurement(port, setAtt, setFreq, out spur);
                }
                else
                {
                    if (NRTM == NRTMcheck && setRBW == RBW && mode == sameMode)
                    {
                        System.Threading.Thread.Sleep(5000);
                        vsa.SingleSweepOnOff(0);
                        System.Threading.Thread.Sleep(1000);
                        vsa.SpurMeasurement(port, setAtt, setFreq, out spur);
                    }
                    else if (NRTM != NRTMcheck)
                    {
                        vsa.LoadMeasurementSetup("Spur_100MHz_TDD", setFreq, mode, setRBW);
                        System.Threading.Thread.Sleep(5000);
                        bbu.Generate_NRTM(NRTM, setBW);
                        vsa.SingleSweepOnOff(0);
                        System.Threading.Thread.Sleep(1000);
                        vsa.SpurMeasurement(port, setAtt, setFreq, out spur);
                        NRTMcheck = NRTM;
                    }
                    else if (mode != sameMode || setRBW != RBW)
                    {
                        System.Threading.Thread.Sleep(5000);
                        vsa.LoadMeasurementSetup("Spur_100MHz_TDD", setFreq, mode, setRBW);
                        vsa.SingleSweepOnOff(0);
                        System.Threading.Thread.Sleep(1000);
                        vsa.SpurMeasurement(port, setAtt, setFreq, out spur);
                        RBW = setRBW;
                        sameMode = mode;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Spurious measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}