namespace _5GAutoTool
{
    class _5GEVMFreqErr
    {
        public Form5GAT mainForm;
        public _5GEVMFreqErr(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        string[] EVM = new string[13];
        string oldNRTM = "";
        public void Reset()
        {
            oldNRTM = "";
        }

        /// <summary>
        /// Log Adapter
        /// </summary>
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel,"", "_5GEVMFreqErr");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        //==============
        public void EVMFreqErrMeasurement(VSA vsa, BBU bbu, string NRTM, string setAtt, int port, string setFreq, string setPower, string setBW, out string[] evm, out string freqErrMeas)
        {
            freqErrMeas = "ERROR";
            EVM[0] = "ERROR";
            EVM[1] = "ERROR";
            EVM[2] = "ERROR";
            EVM[3] = "ERROR";
            EVM[4] = "ERROR";
            EVM[5] = "ERROR";
            EVM[6] = "ERROR";
            EVM[7] = "ERROR";
            EVM[8] = "ERROR";
            EVM[9] = "ERROR";
            EVM[10] = "ERROR";
            EVM[11] = "ERROR";
            EVM[12] = "ERROR";
            evm = EVM;
            try
            {
                System.Threading.Thread.Sleep(1000);
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;

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
                vsa.EVMFreqErrMeasurement(port, NRTM, out evm, out freqErrMeas); //Setup VSA do EVM 
            }
            catch
            {
                Log("EVM/ Frequency Error Measurement: FAIL!", LogLevel.ERROR);
            }

        }
    }
}