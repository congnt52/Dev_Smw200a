using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
//using Agilent.SA.Vsa;
using Newtonsoft.Json.Linq;

namespace _5GAutoTool
{
    public class SMB100A
    {
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 smb100a = new Ivi.Visa.Interop.FormattedIO488();

        public Form5GAT mainForm;
        public string Alias;
        public string Name = "SMB100A";
        private double currFreq = 0;
        private double currPowerLevel = 0;
        public SMB100A(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        object[] tmp;
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "SMB100A");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }
        public void Connection(string IP, string Cmd, out string Manufacturer, out string Model, out string Serial)
        {
            Manufacturer = "ERROR";
            Model = "ERROR";
            Serial = "ERROR";
            try
            {
                smb100a.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP::" + IP + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                smb100a.WriteString(Cmd, true);
                smb100a.IO.Timeout = 2000;
                tmp = (object[])smb100a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                Manufacturer = tmp[0].ToString();
                Model = tmp[1].ToString();
                Serial = tmp[2].ToString();
                if (tmp[1].ToString() == "SMB100A")
                {
                    Log("SMB100A: Connected IP: " + IP, LogLevel.SUCCESS);
                }
                else
                {
                    Log("ERROR: SMB100A Connection fail", LogLevel.ERROR);
                }
                Thread.Sleep(500);
                smb100a.WriteString("*RST;", true);
                smb100a.WriteString("*CLS;", true); // *CLS; *OPC?
                Name += $"-IP:{IP}";
            }
            catch (Exception e)
            {
                Log("ERROR:<SMB100A> Connection Request error! " + e.Message, LogLevel.ERROR);
            }
        }
        public void Disconnect()
        {
            try
            {
                if (smb100a != null)
                {
                    smb100a.IO.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SMB100A> Disconnection Request error! " + e.Message);
            }

        }
        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;
            try
            {
                smb100a.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])smb100a.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch (Exception e)
            {
                Log("ERROR:<SMB100A> Cannot send command to device! " + e.Message, LogLevel.ERROR);
            }
        }
        public void SetFreq(string freqSet)
        {
            try
            {
                double.TryParse(freqSet, out double tmp);
                string freqGHz = (tmp / 1e9).ToString(
    "G17",
    System.Globalization.CultureInfo.InvariantCulture);
                if (tmp!=currFreq)
                {
                    //send command to setup frequency value and get response
                    smb100a.WriteString(
    $":SOUR:FREQ:CW {freqGHz} GHz;*OPC?", true);

                    double response = smb100a.ReadNumber();

                    if (response == 1)
                    {
                        Log($"SMB100A: Source frequency= {tmp / 1e6} MHz");
                        currFreq = tmp;
                    }
                    else
                    {
                        Log($"SMB100A: isBusy!", LogLevel.WARN);
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SMB100A> cannot set frequency! " + e.Message);
            }
        }
        public void SetPower(string setPower)
        {
            try
            {
                double.TryParse(setPower, out double tmp);
                if (tmp != currPowerLevel)
                {
                    //send command to setup frequency value and get response
                    smb100a.WriteString(
     $":SOUR:POW:LEV:IMM:AMPL {setPower};*OPC?", true);

                    double response = smb100a.ReadNumber();

                    if (response == 1)
                    {
                        Log($"SMB100A: Source power level = {setPower} dBm");
                        currPowerLevel = tmp;
                    }
                    else
                    {
                        Log($"SMB100A: isBusy!", LogLevel.WARN);
                    }
                }                    
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SMB100A> cannot set power level! " + e.Message);
            }
        }
        //public void LogUserCorrectionSMB100A(string isPower, double sFreqChange)
        //{
        //    try
        //    {
        //        smb100a.WriteString(":SOUR:CORR:VAL?", true);
                
        //        double correction = smb100a.ReadNumber();
        //        double standardPower;
        //        if (!double.TryParse(isPower, System.Globalization.NumberStyles.Float,
        //            System.Globalization.CultureInfo.InvariantCulture, out standardPower))
        //        {
        //            Log("SMB100A: Invalid OOB standard power: " + isPower, LogLevel.WARN);
        //            return;
        //        }
        //        double requiredSetPower = standardPower + correction;
        //        smb100a.WriteString($":SOUR:POW:LEV:IMM:AMPL {requiredSetPower}", true);
        //        Log(string.Format(System.Globalization.CultureInfo.InvariantCulture,
        //            "[OOB] CW={0:F3} MHz | Standard={1:F3} dBm | UCOR={2:+0.000;-0.000;0.000} dB | Required SGS Set={3:F3} dBm | Current commanded Level={4:F3} dBm",
        //            sFreqChange / 1e6, standardPower, correction, requiredSetPower, standardPower));
        //        // IMPORTANT: Query/log only. Current SetCWWaveform(isPower, ...)
        //        // still sets 'isPower' directly, NOT 'requiredSetPower'.
        //    }
        //    catch (Exception e)
        //    {
        //        Log("SMB100A: Cannot query current UCOR: " + e.Message, LogLevel.WARN);
        //    }
        //}
        public void LogUserCorrectionSMB100A(string isPower, double sFreqChange)
        {
            try
            {
                //smb100a.WriteString(":SOUR:CORR:VAL?", true);

                // double correction = smb100a.ReadNumber();
                double correction = 0;
                double standardPower;
                if (!double.TryParse(isPower, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out standardPower))
                {
                    Log("SMB100A: Invalid OOB standard power: " + isPower, LogLevel.WARN);
                    return;
                }
                double requiredSetPower = standardPower + correction;
                smb100a.WriteString($":SOUR:POW:LEV:IMM:AMPL {requiredSetPower}", true);
                
              //  Log(string.Format(System.Globalization.CultureInfo.InvariantCulture,
              //      "[OOB] CW={0:F3} MHz | Standard={1:F3} dBm | UCOR={2:+0.000;-0.000;0.000} dB | Required SGS Set={3:F3} dBm | Current commanded Level={4:F3} dBm",
               //     sFreqChange / 1e6, standardPower, correction, requiredSetPower, standardPower));
                // IMPORTANT: Query/log only. Current SetCWWaveform(isPower, ...)
                // still sets 'isPower' directly, NOT 'requiredSetPower'.
            }
            catch (Exception e)
            {
                Log("SMB100A: Cannot query current UCOR: " + e.Message, LogLevel.WARN);
            }
        }
        public void RFOnOff(int ON_OFF)
        {
            try
            {
                smb100a.WriteString(
    $":OUTP:STAT {ON_OFF};*OPC?", true);

                double response = smb100a.ReadNumber();
                if (response == 1)
                {
                    Log($"SMB100A: RF State = {ON_OFF}");
                }
                else
                {
                    Log($"SMB100A: isBusy!", LogLevel.WARN);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SMB100A> cannot set output port ON/OFF! " + e.Message);
            }
        }
        public void LoadUserCorrectionSettingFile(string corrPath)
        {
            try                
            {                
                smb100a.WriteString($":SOUR:CORR:CSET '{corrPath}'", true);
                Log($"SMB100A: Load User Correction Data from Path: {corrPath}");
                //Activates user correction
                smb100a.WriteString($":SOUR:CORR:STAT ON; *OPC?", true);

                double response = smb100a.ReadNumber();
                if (response == 1)
                {
                    Log($"SMB100A: UCOR State is: ON ");
                }
                else
                {
                    Log($"SMB100A: isBusy!", LogLevel.WARN);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:<SMB100A> cannot call User Correction Setting! "+ e.Message);
            }
        }
    }
}
