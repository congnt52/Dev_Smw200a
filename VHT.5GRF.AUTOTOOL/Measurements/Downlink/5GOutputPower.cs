using System;

namespace _5GAutoTool
{
    class _5GOutputPower
    {
        public Form5GAT mainForm;
        string Name = "Base station output power";
        LogAdapter log = LogAdapter.Instance;
        public _5GOutputPower(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string oldNRTM = "";
        public void Reset()
        {
            oldNRTM = "";
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GOutputPower");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============


        public void OutputPowerMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower, string setBW, out string TXPower)
        {
            TXPower = "ERROR";
            try
            {
                if (NRTM != oldNRTM)
                {
                    vsa.LoadMeasurementSetup(NRTM, setFreq, setBW);
                    vsa.SetCenterFrequency(setFreq);
                    vsa.SetWindowTraceX(setFreq, "EVM");
                    vsa.SetWindowTraceY("30", "EVM");
                    bbu.Generate_NRTM(NRTM, setBW);
                    oldNRTM = NRTM;
                }

                //vsa.SetParameter(setFreq, setAtt);
                vsa.SetExternaAttenuator(setAtt);
                vsa.AutoScale();
                vsa.SingleSweepOnOff(0);
                vsa.OutputPowerMeasurement(port, bbu, out TXPower);

            }
            catch (Exception ex)
            {
                Log("Channel power Measurement: FAIL! " + ex.Message, LogLevel.ERROR);
            }
        }
    }
}
