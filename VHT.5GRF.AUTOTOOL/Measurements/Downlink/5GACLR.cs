namespace _5GAutoTool
{
    class _5GACLR
    {
        public Form5GAT mainForm;
        public _5GACLR(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string Mode = "", sameMode = "";
        public void Reset()
        {
            sameMode = "";
        }
        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GACLR");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void ACLRMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower, string setBW, out string outpMeas,
            out string aclrAdjLower, out string aclrAdjUpper, out string aclrAlt1Lower, out string aclrAlt1Upper)
        {
            aclrAdjLower = "ERROR";
            aclrAdjUpper = "ERROR";
            aclrAlt1Lower = "ERROR";
            aclrAlt1Upper = "ERROR";
            outpMeas = "ERROR";
            Mode = "ACLR";
            try
            {
                System.Threading.Thread.Sleep(1000);
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;

                if (sameMode != Mode)
                {
                    vsa.LoadMeasurementSetup(Mode, setFreq, setBW);
                    vsa.SetCenterFrequency(setFreq);                    
                    vsa.SetWindowTraceY("30", "ACP");
                    bbu.Generate_NRTM(NRTM, setBW);
                    sameMode = Mode;
                }

                //vsa.SetParameter(setFreq, setAtt);
                vsa.SetExternaAttenuator(setAtt);
                vsa.AutoScale();
                vsa.SingleSweepOnOff(0);
                vsa.ACLRPowerMeasurement(port, NRTM, out outpMeas, out aclrAdjLower, out aclrAdjUpper, out aclrAlt1Lower, out aclrAlt1Upper);
            }
            catch
            {
                Log("ACLR Measurement: FAIL!", LogLevel.ERROR);
            }
        }
    }
}