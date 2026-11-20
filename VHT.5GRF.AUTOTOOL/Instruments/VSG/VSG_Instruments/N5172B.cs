using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keysight.SignalStudio.N7631;

namespace _5GAutoTool
{
    public class N5172B
    {
        public Form5GAT mainForm;
        public string Alias;
        public string Name = "N5172B";
        public double TriggerDelay = 0;
        //public Keysight.SignalStudio.N7631.Api api;
        string currLoadPath = "";
        string currFreq = "";
        string OffsetPosition = "0";

        public N5172B(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 N5172b = new Ivi.Visa.Interop.FormattedIO488();

        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "N5172B");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }

        //// TODO: Define a new api() using for N7631C.
        private Keysight.SignalStudio.N7631.Api api;
        public void Connection(string IP, string Cmd, out string Manufacturer, out string Model, out string Serial)
        {
            //api = new Api();
            Manufacturer = "ERROR";
            Model = "ERROR";
            Serial = "ERROR";
            string ConnectCmd = "TCPIP0::" + IP + "::INSTR";
            //api = new Api();
            //api.New();
            try
            {
                api = new Api();
                api.New();
                N5172b.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP::" + IP + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                //N5172b.WriteString(Cmd, true);
                //System.Threading.Thread.Sleep(100);
                if (!string.IsNullOrWhiteSpace(Cmd))
                {
                    string command = Cmd.Trim();

                    N5172b.WriteString(command, true);

                    if (command.Contains("?"))
                    {
                        string response = N5172b.ReadString();

                        Log("[N5172B] Connect Query: " +
                            command + " => " + response,
                            LogLevel.INFO);
                    }
                }

                System.Threading.Thread.Sleep(100);

                if (api.ConnectInstrument(ConnectCmd))
                {
                    Model = api.SignalGenerator.InstrumentModelNumber.ToString();
                    
                    Manufacturer = "Keysight";
                    //Serial = api.SignalGenerator;
                    Log("Connection: SUCCESS!(N7631C API)", LogLevel.SUCCESS);
                    Name = $"N5172B";
                }
                else
                {
                    Log("Connection => FAIL!", LogLevel.ERROR);
                }
            }
            catch (Exception ex)
            {
                Log("[N5172B] Connection FAIL: " +
        ex.Message, LogLevel.ERROR);
            }
        }
        public void Disconnect()
        {
            try
            {
                //api.Close();
                Log("Connection => CLOSE! (Preset)", LogLevel.WARN);
                api.Preset();
                //N5182b.IO.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        //public void SendCmd(string Cmd, out string respond)
        //{
        //    respond = null;
        //    try
        //    {
        //        N5172b.WriteString(Cmd, true);
        //        object[] tmp;
        //        tmp = (object[])N5172b.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
        //        for (int i = 0; i < tmp.GetLength(0); i++)
        //        {
        //            respond += tmp[i].ToString();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        Log("Connection => FAIL!", LogLevel.ERROR);
        //    }
        //}

        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;

            try
            {
                if (string.IsNullOrWhiteSpace(Cmd))
                    return;

                string command = Cmd.Trim();

                N5172b.WriteString(command, true);

                if (command.Contains("?"))
                {
                    respond = N5172b.ReadString();

                    Log("[N5172B] QUERY: " + command +
                        " => " + respond, LogLevel.INFO);
                }
                else
                {
                    Log("[N5172B] WRITE: " + command,
                        LogLevel.INFO);
                }
            }
            catch (Exception ex)
            {
                Log("[N5172B] SendCmd ERROR: " +
                    ex.Message, LogLevel.ERROR);
            }
        }
        public void SetFreq(string freqSet)
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //N5182b.WriteString("FREQ " + freqSet + "Hz", true);
                //Log("N5172B frequency: " + freqSet);
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Log($"Set frequency: {double.Parse(freqSet) / 1e6} MHz");
                api.SignalGenerator.Frequency = double.Parse(freqSet);
                //api.Download();
                api.UpdateToInstrument();
            }
            catch (Exception)
            {
                Log("Set frequency => FAIL!", LogLevel.ERROR);
            }
        }
        public void SetPower(string setPower)
        {
            try
            {
                //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                //N5182b.WriteString(":POW " + setPower + "DBM", true);
                //Log("N5172B setup power: " + setPower + "dBm");
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Log($"Set power: {setPower} dBm");
                api.SignalGenerator.Amplitude = double.Parse(setPower);
                api.UpdateToInstrument();
            }
            catch (Exception)
            {
                Log("Set power => FAILED!", LogLevel.ERROR);
            }
        }

        // TODO: Load G-FRC-A1-5 reference signal at freqSet Hz
        public void LoadGFR1A15(string freqSet)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\G-FRC-A1-5.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {Alias} load Fail! {err}");
                }
                else
                {
                    Log($"{Alias}: Open file {currLoadPath}");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    api.Generate();
                    api.Download();
                    currLoadPath = positionCmd;
                }
            }
            catch
            {
                Log("ERROR: N5172B can not load file");
            }
        }

        // TODO: Load G-FRC-A1-5 reference signal at freqSet Hz and RBOffset
        public void LoadGFR1A15(string freqSet, string NRULSCHRBOffset)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\G-FRC-A1-5.scp";
            if(positionCmd != currLoadPath || freqSet!=currFreq || NRULSCHRBOffset != OffsetPosition)
            {
                try
                {
                    if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                    System.Threading.Thread.Sleep(100);
                    Log($"[WS] Loaded file G-FRC-A1-5.scp {positionCmd}");
                    api.OpenSetupFile(positionCmd);
                    string err = api.ReadError();
                    if (!String.IsNullOrEmpty(err))
                    {
                        Log($"{err}", LogLevel.ERROR);
                    }
                    else
                    {
                        Log($"LOGLoad file FRC at frequency {freqSet} Hz");
                        api.SignalGenerator.Frequency = double.Parse(freqSet);
                        Log($"LOG: FRC at RBoffset {NRULSCHRBOffset}");
                        SetRBOffset(NRULSCHRBOffset);
                        api.Generate();
                        api.Download();
                        currLoadPath = positionCmd;
                        OffsetPosition = NRULSCHRBOffset;
                        Log($"Load file FRC at frequency {freqSet} Hz");
                        currFreq = freqSet;

                    }
                }
                catch
                {
                    Log($"Loaded file {currLoadPath}");
                }
            }
            else
            {
                Log($"Loaded file {positionCmd}");
            }
            
        }

        public void LoadGFR1A15(string freqSet, string NRULSCHRBOffset, string BW)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\nr5g\\{BW}mhz\\G-FRC-A1-5.scp";
            if (positionCmd != currLoadPath || freqSet != currFreq || NRULSCHRBOffset != OffsetPosition)
            {
                try
                {
                    if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                    System.Threading.Thread.Sleep(100);
                    Log($"LOG:[WS] Loaded file G-FRC-A1-5.scp {positionCmd}");
                    api.OpenSetupFile(positionCmd);
                    string err = api.ReadError();
                    if (!String.IsNullOrEmpty(err))
                    {
                        Log($"{err}", LogLevel.ERROR);
                    }
                    else
                    {
                        Log($"LOG:[WS] Load file FRC at frequency {freqSet} Hz");
                        api.SignalGenerator.Frequency = double.Parse(freqSet);
                        Log($"LOG:[WS] FRC at RBoffset {NRULSCHRBOffset}");
                        SetRBOffset(NRULSCHRBOffset);
                        api.Generate();
                        api.Download();
                        currLoadPath = positionCmd;
                        OffsetPosition = NRULSCHRBOffset;
                        Log($"LOG:[WS] Load file FRC at frequency {freqSet} Hz");
                        currFreq = freqSet;

                    }
                }
                catch
                {
                    Log($"Loaded file {currLoadPath}");
                }
            }
            else
            {
                Log($"Loaded file {positionCmd}");
            }

        }
        // TODO: Tín hiệu nhiêu AWGN signal
        public void SetAWGNSignal()
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                N5172b.WriteString(":RAD:ARB:WAV " + "\"" + "FRC-A2-5-51rb_DDDSU.wfm" + "\"");
                Log("lOG:[IS] N5172B: Load file: FRC-A2-5-51rb_DDDSU.wfm");
                N5172b.WriteString(":SOUR:RAD:ARB:STAT ON");
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }
        
        //public void LoadGRFA25(string freqSet)
        //{
        //    string positionCmd = "C:\\test3gpp\\waveforms\\G-FRC-A2-5.scp";
        //    try
        //    {
        //        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
        //        System.Threading.Thread.Sleep(100);
        //        Log("LOG: N5172B open file G-FRC-A2-5.scp");
        //        api.OpenSetupFile(positionCmd);
        //        string err = api.ReadError();
        //        if (!String.IsNullOrEmpty(err))
        //        {
        //            Log($"ERROR: {err}");
        //        }
        //        else
        //        {
        //            Log("LOG: N5172B load file G-FRC-A2-5.scp");
        //            EnableAWGNSignal(true, "11.0", "18360000", "98280000");
        //            api.SignalGenerator.Frequency = double.Parse(freqSet);
        //            api.Generate();
        //            api.Download();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        Log("ERROR: N5172B load file fail");
        //    }
        //}

        public void LoadGRFA25(string freqSet, string NRULSCHRBOffset, string BW)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\nr5g\\{BW}mhz\\G-FRC-A2-5.scp";
            if(positionCmd!=currLoadPath || freqSet!=currFreq)
            {
                try
                {
                    if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                    System.Threading.Thread.Sleep(100);
                    Log($"LOG:[WS] Loaded file G-FRC-A2-5.scp {positionCmd}");
                    api.OpenSetupFile(positionCmd);
                    string err = api.ReadError();
                    if (!String.IsNullOrEmpty(err))
                    {
                        Log($"ERROR: {err}", LogLevel.ERROR);
                    }
                    else
                    {
                        Log($"{Alias}: N5172B load file G-FRC-A2-5.scp");
                        //doc tham so cau hinh cho AWWGN tu mainForm
                        string awgnCarrierToNoiseRatio = mainForm.ReadValueByHeader(mainForm.CurrentTestcase.Parameters,"CNR");
                        string awgnCarrierBw = mainForm.ReadValueByHeader(mainForm.CurrentTestcase.Parameters, "CarBw");
                        string awgnNoiseBw = mainForm.ReadValueByHeader(mainForm.CurrentTestcase.Parameters, "FatNoiseBw");

                        EnableAWGNSignal(true, awgnCarrierToNoiseRatio, awgnCarrierBw, awgnNoiseBw);

                        Log($"{Alias}: N5172B set FRC at frequency {freqSet} Hz");
                        api.SignalGenerator.Frequency = double.Parse(freqSet);
                        Log($"{Alias}: FRC at RB Offset {NRULSCHRBOffset}");
                        SetRBOffset(NRULSCHRBOffset);
                        api.Generate();
                        api.Download();
                    }
                }
                catch (Exception)
                {
                    Log("ERROR: N5172B load file fail", LogLevel.ERROR);
                }
            }
            else
            {
                Log($"{Alias}: N5172B open file G-FRC-A2-5.scp");
            }
            
        }
        //TODO: Set RBOffset for 5G NR Signal
        public void SetRBOffset(string NRULSCHRBOffset)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"{Alias}: N5172B setup RB offset {NRULSCHRBOffset} for ULSCH channel");
                api.NR5GWaveformSettings.NRCarriers[0].Uplink.ULSCH[0].RBOffset = int.Parse(NRULSCHRBOffset);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //TODO: Enable and set SNR for AWGND signale
        public void EnableAWGNSignal(bool AwgnEnable, string CarriertoNoiseRatio, string CarrierBandwidth, string NoiseBandwidth)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"{Alias}: N5180B enables AWGN signal.");
                api.NR5GWaveformSettings.AwgnEnabled = AwgnEnable;
                Log($"{Alias}: N5172B setups carrier to noise ratio {CarriertoNoiseRatio} dB.");
                api.NR5GWaveformSettings.CarrierToNoiseRatio = double.Parse(CarriertoNoiseRatio); 
                Log($"{Alias}: N5172B setups carrier bandwidth {CarrierBandwidth} MHz.");
                api.NR5GWaveformSettings.CarrierBandwidth = double.Parse(CarrierBandwidth)*1e6; //Chuyen MHz sang Hz
                Log($"{Alias}: N5172B setups noise bandwidth {NoiseBandwidth} MHz.");
                api.NR5GWaveformSettings.NoiseBandwidth = double.Parse(NoiseBandwidth) * 1e6; //Chuyen MHz sang Hz
            }
            catch (Exception)
            {
                Log("ERROR: N5172B setups AWGN fail", LogLevel.ERROR);
                throw;
            }
        }
        // TODO: Load ICS DFT-s-OFDM NR signal, SCS 30 kHz,50 RB at RB Offset
        public void SetICSInterferenceSignal(string freqSet, string RBOffset)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\ICS_DFT-s-OFDM.scp";

            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"Loaded file ICS_DFT-s-OFDM.scp {positionCmd}");
                Log("LOG:[IS] N5172B open file ICS_DFT-s-OFDM_50RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG:[IS] N5172B load file ICS_DFT-s-OFDM.scp");
                    Log($"LOG:[IS] Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log($"LOG:[IS] Intefering signal at RBoffset {RBOffset}");
                    SetRBOffset(RBOffset);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }
        public void SetICSInterferenceSignal(string freqSet, string RBOffset, string BW)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\nr5g\\{BW}mhz\\ICS_DFT-s-OFDM.scp";

            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"Loaded file ICS-DFT-s-OFDM.scp {positionCmd}");
                Log("LOG:[IS] N5172B open file ICS_DFT-s-OFDM_50RB.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG:[IS] N5172B load file ICS_DFT-s-OFDM.scp");
                    Log($"LOG:[IS] Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log($"LOG:[IS] Intefering signal at RBoffset {RBOffset}");
                    SetRBOffset(RBOffset);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }

        // TODO: Load Intefering signal 5G NR in ACS at freqSet Hz
        public void SetACSInterferenceSignal(string freqSet, string BW)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\nr5g\\{BW}mhz\\ACS_DFT-s-OFDM.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"Loaded file ACS_DFT-s-OFDM.scp {positionCmd}");
                Log("LOG:[IS] N5172B open file ACS_DFT-s-OFDM.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}");
                }
                else
                {
                    Log("LOG:[IS] N5172B load file ACS_DFT-s-OFDM_100RB.scp");
                    Log($"LOG:[IS] Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }
        // TODO: Load Intefering signal In-band blocking OFDM 100RB - SCS 15Khz
        public void InbandBlockingSignal(string freqSet)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\INB_DFT-s-OFDM.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"Loaded file INB_DFT-s-OFDM.scp {positionCmd}");
                Log("LOG:[IS] N5172B open file INB_DFT-s-OFDM.scp");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG:[IS] N5172B load file INB_DFT-s-OFDM_100RB.scp");
                    Log($"LOG:[IS] Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }
        public void InbandBlockingSignal(string freqSet, string BW)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\nr5g\\{BW}mhz\\INB_DFT-s-OFDM.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"LOG:[IS] N5172B open file INB_DFT-s-OFDM_100RB.scp {positionCmd}");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG:[IS] N5172B load file INB_DFT-s-OFDM.scp");
                    Log($"LOG:[IS] Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }

        // TODO: Load Narrow-band Blocking Signal 
        public void NarrowbandBlockingSignal(string freqSet, string RBOffset, string BW)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\nr5g\\{BW}mhz\\NAB_DFT-s-OFDM.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"LOG:[IS] N5172B open file NAB_DFT-s-OFDM.scp {positionCmd}");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG:[IS] N5172B load file NAB_DFT-s-OFDM_1RB.scp");
                    Log($"LOG:[IS] Intefering signal at frequency {freqSet} Hz");
                    api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log($"LOG:[IS] Set RB Offset at {RBOffset}");                    
                    SetRBOffset(RBOffset);
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }
        // NarrowBand Blocking Signal Upper edger OFDM 1RB 15kHz QPSK upper edge
        public void NarrowbandUpperBlockingSignal()
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\NAB_DFT-s-OFDM_1RB.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"LOG:[IS] N5172B open file NAB_DFT-s-OFDM_1RB.scp {positionCmd}");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG:[IS] N5172B load file NAB_DFT-s-OFDM_1RB.scp");
                    Log("LOG:[IS] Set RB Offset at 0");
                    // Set RB Offset at 1
                    SetRBOffset("0");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }
        // TODO: Phat song sin
        public void CWSignal()
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\CW_signal.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG:[IS] N5172B load CW_signal successful");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load CW_signal fail", LogLevel.ERROR);
            }
        }

        // TODO: Phát tín hiệu General IMD
        //public void GeneralIMDSignal(string Bw)
        //{
        //    string positionCmd = $"C:\\test3gpp\\waveforms\\nr5g\\{Bw}mhz\\IMD_GEN_DFT-s-OFDM.scp";
        //    try
        //    {
        //        if (mainForm.measurementBackgroundWorker.CancellationPending) return;
        //        System.Threading.Thread.Sleep(100);
        //        Log($"LOG:[IS] N5172B open file IMD_GEN_DFT-s-OFDM.scp {positionCmd}");
        //        api.OpenSetupFile(positionCmd);
        //        string err = api.ReadError();
        //        if (!String.IsNullOrEmpty(err))
        //        {
        //            Log($"ERROR: {err}", LogLevel.ERROR);
        //        }
        //        else
        //        {
        //            Log("LOG:[IS] N5172B load file IMD_GEN_DFT-s-OFDM.scp");
        //            api.Generate();
        //            api.Download();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        Log("ERROR: N5172B load file fail");
        //    }

        //}

        public void GeneralIMDSignal(string Bw)
        {
            string positionCmd =
                $@"C:\test3gpp\waveforms\nr5g\{Bw}mhz\IMD_GEN_DFT-s-OFDM.scp";

            string step = "START";

            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending)
                    return;

                if (api == null)
                    throw new InvalidOperationException(
                        "N7631 API is not initialized.");

                if (!System.IO.File.Exists(positionCmd))
                    throw new System.IO.FileNotFoundException(
                        "General IMD state file not found", positionCmd);

                step = "OpenSetupFile";

                //Log("[N5172B][GenIMD] START Load State: " + positionCmd,
                    //LogLevel.INFO);

                api.OpenSetupFile(positionCmd);

              //  Log("[N5172B][GenIMD] OpenSetupFile completed",
                   // LogLevel.INFO);

                step = "ReadError";

                string err = api.ReadError();

                if (!string.IsNullOrWhiteSpace(err))
                {
                    throw new InvalidOperationException(
                        "N7631 API error: " + err);
                }

                if (mainForm.measurementBackgroundWorker.CancellationPending)
                    return;

                step = "Generate";

              //  Log("[N5172B][GenIMD] START Generate",
                 //  LogLevel.INFO);

                api.Generate();

                Log("[N5172B][GenIMD] Generate completed",
                    LogLevel.INFO);

                if (mainForm.measurementBackgroundWorker.CancellationPending)
                    return;

                step = "Download";

                //Log("[N5172B][GenIMD] START Download",
                //    LogLevel.INFO);

                //api.Download();

                //Log("[N5172B][GenIMD] Download completed",
                //    LogLevel.INFO);

                //Log("[N5172B][GenIMD] Load State sequence completed",
                //    LogLevel.SUCCESS);
            }
            catch (Exception ex)
            {
                Log(
                    "[N5172B][GenIMD] ERROR at " + step +
                    ": " + ex.Message,
                    LogLevel.ERROR);

                throw;
            }
        }

        // phát tín hiệu Narrowband IMD upper edger
        public void NarrowBandUpperIMDSignal(string RBOffset, string Bw)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\nr5g\\{Bw}mhz\\IMD_NAB_DFT-s-OFDM.scp";
            try
            {                
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"LOG: [IS] N5172B open file IMD_NAB_DFT-s-OFDM_1RB.scp {positionCmd}");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG: [IS] N5172B load file IMD_NAB_DFT-s-OFDM_1RB.scp");
                    SetRBOffset(RBOffset);
                    Log($"LOG: [IS] Set RB Offset at {RBOffset}");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: [IS] N5172B load file fail", LogLevel.ERROR);
            }

        }

        // phát tín hiệu Narrowband IMD lower edger
        public void NarrowBandLowerIMDSignal(string RBOffset, string Bw)
        {
            string positionCmd = $"C:\\test3gpp\\waveforms\\nr5g\\{Bw}mhz\\IMD_NAB_DFT-s-OFDM.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(100);
                Log($"LOG: [IS] N5172B open file IMD_NAB_DFT-s-OFDM.scp {positionCmd}");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG: [IS] N5172B load file IMD_NAB_DFT-s-OFDM.scp");
                    SetRBOffset(RBOffset);
                    Log($"LOG: [IS] Set RB Offset at {RBOffset}");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }

        //TODO: Phat tin hieu TX IMD NRTM1.1 BW 10Mhz 
        public void TXIMDSignal()
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\NRTM11_10MHz_FDD.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Log($"LOG: [IS] N5172B set TX IMD NRTM1.1 10 MHz {positionCmd}");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    Log("LOG: [IS] N5172B load file NRTM11_10MHz_FDD.scp");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }
        }

        //TODO: Load NR_TM1.1 signal 100 MHz FDD
        public void FR1_NRTM11_FDD(string freqSet)
        {
            string positionCmd = "C:\\test3gpp\\waveforms\\NRTM11_FDD.scp";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                Log($"LOG: N5172B load NR_TM1.1 signal {positionCmd}");
                api.OpenSetupFile(positionCmd);
                string err = api.ReadError();
                if (!String.IsNullOrEmpty(err))
                {
                    Log($"ERROR: {err}", LogLevel.ERROR);
                }
                else
                {
                    //Log($"LOG: N5172B setups frequency {freqSet} Hz");
                    //api.SignalGenerator.Frequency = double.Parse(freqSet);
                    Log("LOG: N5172B load file NRTM11_FDD.scp");
                    api.Generate();
                    api.Download();
                }
            }
            catch (Exception)
            {
                Log("ERROR: N5172B load file fail", LogLevel.ERROR);
            }

        }
        public void RFOnOff(int ON_OFF)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                N5172b.WriteString(":OUTP " + ON_OFF);
                if (ON_OFF == 1)
                {
                    api.SignalGenerator.RFOutputEnabled = true;
                }
                else
                {
                    api.SignalGenerator.RFOutputEnabled = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void MODOnOff(int ON_OFF)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                N5172b.WriteString(":OUTP " + ON_OFF);
                if (ON_OFF == 1)
                {
                    api.SignalGenerator.RFOutputEnabled = true;
                }
                else
                {
                    api.SignalGenerator.RFOutputEnabled = false;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
