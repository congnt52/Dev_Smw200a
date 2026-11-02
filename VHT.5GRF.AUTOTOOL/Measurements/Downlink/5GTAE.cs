using System;

namespace _5GAutoTool
{
    class _5GTAE
    {
        public Form5GAT mainForm;
        public _5GTAE(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "";
        public void Reset()
        {
            sameMode = "";
        }
        double[] tmp = new double[14];

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GTAE");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void TAEMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, string setFreq, string setBW, out string tae)
        {
            tae = "ERROR";
            Mode = "TAE_7D1S2U_100MHz";
            try
            {
                if (Mode != sameMode)
                {
                    vsa.LoadMeasurementSetup("TAE_7D1S2U_100MHz", setFreq);
                    vsa.SetCenterFrequency(setFreq);
                    vsa.SetWindowTraceX(setFreq, "EVM");
                    vsa.SetWindowTraceY("30", "EVM");                    
                    bbu.Generate_NRTM(NRTM, setBW);
                    sameMode = Mode;
                }
                vsa.SetExternaAttenuator(setAtt);
                vsa.AutoScale();
                vsa.SingleSweepOnOff(0);
                vsa.TAEMeasurement(setFreq, out tae);
            }
            catch (Exception ex)
            {
                Log("TAE Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }

        public void NewTAEMeasurement(VSA vsa, BBU bbu, string NRTM, int port, string att, string setFreq, string setBW, ref string taeref, ref string[] tae)
        {
            //taeref = "0";
            tae = new string[] { };
            Mode = "new_Time Aligment Error";
            try
            {
                if (Mode != sameMode)
                {
                    vsa.LoadMeasurementSetup(NRTM, setFreq);
                    bbu.Generate_NRTM(NRTM, setBW);
                    sameMode = Mode;
                }

                vsa.SetParameter(setFreq, att);
                vsa.AutoScale();
                vsa.SingleSweepOnOff(0);
                vsa.NewTAEMeasurement(Mode, NRTM, port, ref taeref, ref tae);
                //vsa.SingleSweepOnOff(1);
            }
            catch (Exception ex)
            {
                Log("TAE Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}