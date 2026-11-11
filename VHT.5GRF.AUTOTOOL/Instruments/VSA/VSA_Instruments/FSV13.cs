using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5GAutoTool
{
    public class FSV13
    {
        public Form5GAT mainForm;
        public string Name = "FSV13";
        public FSV13(Form5GAT mMainForm)
        {
            mainForm = mMainForm;
        }
        Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
        Ivi.Visa.Interop.FormattedIO488 fsv = new Ivi.Visa.Interop.FormattedIO488();
        LogAdapter log = LogAdapter.Instance;
        public void Log(string text, LogLevel logLevel = LogLevel.INFO)
        {
            log.Log(text, logLevel, Name, "FSV13");
        }
        public void DashedLineLog(string title = "")
        {
            log.Log(title, LogLevel.DASH);
        }

        public void takephoto()
        {
            string name;
            name = "_ACLR_P" + "_" + DateTime.Now.ToString("MMdd_hhmm").ToString();
            fsv.WriteString("MMEM:NAME " + "\"" + "C:/R_S/Instr/user/NR5G/images/" + name + ".png" + "\"");
        }

        public void Connection(string IP, string Cmd, string Instrument, out string manufacturer, out string model, out string serial)
        {
            manufacturer = "ERROR";
            model = "ERROR";
            serial = "ERROR";
            try
            {
                //Disconnect();
                fsv.IO = (Ivi.Visa.Interop.IMessage)rm.Open("TCPIP::" + IP + "::INSTR", Ivi.Visa.Interop.AccessMode.NO_LOCK, 5, " ");
                fsv.IO.Timeout = 3000;
                fsv.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                manufacturer = tmp[0].ToString();
                model = tmp[1].ToString();
                serial = tmp[2].ToString();
                if (tmp[1].ToString() == Instrument)
                {
                    Log("FSV13: connected to " + IP, LogLevel.SUCCESS);
                }
                else
                {
                    Log("ERROR: FSV13 connection fail", LogLevel.ERROR);
                }
            }
            catch
            {
                Log("ERROR: FSV13 connection fail", LogLevel.ERROR);
            }
        }

        public void Disconnect()
        {
            if (fsv != null)
            {
                fsv.IO.Close();
            }
        }

        public void SendCmd(string Cmd, out string respond)
        {
            respond = null;
            try
            {
                fsv.WriteString(Cmd, true);
                object[] tmp;
                tmp = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                for (int i = 0; i < tmp.GetLength(0); i++)
                {
                    respond += tmp[i].ToString();
                }
            }
            catch
            {
                Log("fsv13 connect: fail", LogLevel.ERROR);
            }
        }

        public void FreqCent(string freq)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsv.WriteString("FREQ:CENT " + freq + "Hz", true);
                Log("FSV13: select center frequency: " + freq);

            }
            catch
            {
                Log("ERROR: FSV13 select center frequency fail", LogLevel.ERROR);
            }
        }

        public void RLEV(string reflever_dBm)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsv.WriteString("DISP:TRAC:Y:RLEV " + reflever_dBm + " dBm", true);
                Log("FSV13: ref lever: " + reflever_dBm);
            }
            catch
            {
                Log("ERROR: FSV13 ref lever fail", LogLevel.ERROR);
            }
        }

        public void AutoScale()
        {
            try
            {
                fsv.WriteString(":SENS:ADJ:LEV;*WAI", true); // auto scale window
                System.Threading.Thread.Sleep(10000);
            }
            catch
            {

            }
        }

        public void RLEVOFFS(string refLeverOffSetDB)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsv.WriteString("DISP:TRAC:Y:RLEV:OFFS " + refLeverOffSetDB + " dB", true);
                Log("FSV13: reference lever Offset: " + refLeverOffSetDB);
            }
            catch
            {
                Log("ERROR: FSV13 ref lever Offset fail", LogLevel.ERROR);
            }
        }

        public void PHASECOMP(string ONorOFF)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString("CONF:UL:CC1:RFUC:STAT " + ONorOFF, true);
            }
            catch
            {
                Log("FSV13 Phase Compensation: FAIL", LogLevel.ERROR);
            }
        }

        public void Swetime(string time)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString("SENS:SWE:TIME " + time + "ms", true);
                Log("FSV13: sweep time " + time + "ms");
            }
            catch
            {
                Log("ERROR: FSV13 sweep time fail", LogLevel.ERROR);
            }
        }

        public void LoadNRTMSetup(string NRTMxx)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString("INST:DEL '5G NR'");
                fsv.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/NRTM/7D1S2U/" + NRTMxx + "_7D1S2U'", true);
                Log("FSV13 load NRTM measurement: " + NRTMxx);
            }
            catch
            {
                Log("FSV13 Load NRTM measurement: FAIL", LogLevel.ERROR);
            }
        }
        public void LoadNRTMSetup(string NRTMxx, string Bw)
        {
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                fsv.WriteString("INST:DEL '5G NR'");
                fsv.WriteString($"MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/NRTM/{Bw}/7D1S2U/" + NRTMxx + "_7D1S2U'", true);
                Log("FSV13 load NRTM measurement: " + NRTMxx);
            }
            catch
            {
                Log("FSV13 Load NRTM measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void OFDMPower(out string power)
        {
            power = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                fsv.WriteString(":SENS:ADJ:LEV;*WAI", true); // auto scale window
                System.Threading.Thread.Sleep(10000);
                fsv.WriteString(":FETC:CC1:ISRC:FRAM:SUMM:OSTP:AVER?", true);
                object[] fsv13_Power;
                fsv13_Power = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                power = fsv13_Power[0].ToString();
                Log("OFDM Power: " + power);
            }
            catch
            {
                Log("FSV13 Total power dynamic range Measurement: FAIL", LogLevel.ERROR);
            }
        }

        public void ACLRPower(int port, string TM, out string power, out string aclrAdjLower, out string aclrAdjUpper, out string aclrAlt1Lower, out string aclrAlt1Upper)
        {
            string[] data = new string[] { "TXPOWER", "LOWER ABS ADJ", "UPPER ABS ADJ", "LOWER ADJ", "UPPER ADJ", "LOWER ABS ALT", "UPPER ABS ALT", "LOWER ALT", "UPPER ALT", "ACLR", "Worst Abs Power" };
            power = "ERROR";
            aclrAdjLower = "ERROR";
            aclrAdjUpper = "ERROR";
            aclrAlt1Lower = "ERROR";
            aclrAlt1Upper = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(1);
                //    TakePhoto(mode); // luu anh

                //   System.Threading.Thread.Sleep(500);
                fsv.WriteString("CALC:MARK:FUNC:POW:RES:DET? ACP", true);
                object[] fsv13_aclr;
                fsv13_aclr = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");

                power = fsv13_aclr[0].ToString();
                string aclrAdjabsLower = fsv13_aclr[1].ToString();
                string aclrAdjabsUpper = fsv13_aclr[2].ToString();
                string aclrAdtabsLower = fsv13_aclr[5].ToString();
                string aclrAdtabsUpper = fsv13_aclr[6].ToString();
                aclrAdjLower = fsv13_aclr[3].ToString();
                aclrAdjUpper = fsv13_aclr[4].ToString();
                aclrAlt1Lower = fsv13_aclr[7].ToString();
                aclrAlt1Upper = fsv13_aclr[8].ToString();
                Log("FSV13: ACLR measurement" + "\n"
                    + "ADJ lower: " + aclrAdjLower + "\n"
                    + "ADJ upper: " + aclrAdjUpper + "\n"
                    + "ALT lower: " + aclrAlt1Lower + "\n"
                    + "ALT upper: " + aclrAlt1Upper + "\n");
                double tmp1 = Math.Max(double.Parse(aclrAdjabsLower), double.Parse(aclrAdjabsUpper));
                double tmp2 = Math.Max(double.Parse(aclrAdtabsLower), double.Parse(aclrAdtabsUpper));
                double tmp3 = Math.Max(double.Parse(aclrAdjabsLower), double.Parse(aclrAdjabsUpper));
                double tmp4 = Math.Max(double.Parse(aclrAdjabsLower), double.Parse(aclrAdjabsUpper));
                string[] result = new string[] { Math.Max(tmp1, tmp2).ToString(), Math.Max(tmp3, tmp4).ToString() };
                mainForm.DLCsvRecord(data, fsv13_aclr, result, "ACLR", TM, fsv13_aclr.Length, result.Length, port);
            }
            catch
            {
                Log("FSV13 ACLR Measurement: FAIL", LogLevel.ERROR);
            }
        }

        /*........................................................OBW mesurement.....................................................................*/
        public void LoadOBWSetup(string BW, string TDD_or_FDD)
        {
            fsv.WriteString("INST:DEL '5G NR'");
            fsv.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/OBW/OBW_" + BW + "MHz_" + TDD_or_FDD + "'", true);
            Log("FSV13 load OBW measurement: OBW_" + BW + "_" + TDD_or_FDD);
        }
        public void OBWMeas(int port, out string OBWMeas)
        {
            OBWMeas = "ERROR";
            try
            {
                if (mainForm.measurementBackgroundWorker.CancellationPending) return;
                System.Threading.Thread.Sleep(4000);
                fsv.WriteString("CALC:MARK:FUNC:POW:RES? OBW", true);
                object[] fsv13_obw;
                fsv13_obw = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                OBWMeas = fsv13_obw[0].ToString();
                Log(OBWMeas);
            }
            catch
            {
                Log("FSV13 load OBW measurement: FAIL", LogLevel.ERROR);
            }
        }

        /*.................................................................Operating band Unwanted emissions (SEM) ...........................................................................*/
        public void LoadSEMSetup(string BW, string TDDFDD)
        {
            fsv.WriteString("INST:DEL '5G NR'");
            fsv.WriteString("MMEM:LOAD:STAT 1,'C:/R_S/Instr/user/NR5G/SEM/SEM_" + BW + "MHz_" + TDDFDD + "'", true);
            Log("FSV3030 load SEM measurement: SEM_" + BW + "_" + TDDFDD);
        }

        public void SEMMeas(int port, string TM, out double absPwr1, out double absPwr2, out double absPwr3, out double semLimit)
        {
            //semLimit = "ERROR";
            //if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            //System.Threading.Thread.Sleep(1);
            //try
            //{
            //    string limit1;
            //    string limit2;
            //    string limit3;
            //    string limit4;
            //    string limit5;
            //    string limit6;
            //    System.Threading.Thread.Sleep(2000);
            //    fsv.WriteString("TRAC:DATA? LIST", true);
            //    object[] fsv3030_SEM;
            //    fsv3030_SEM = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
            //    limit1 = fsv3030_SEM[7].ToString();
            //    limit2 = fsv3030_SEM[18].ToString();
            //    limit3 = fsv3030_SEM[29].ToString();
            //    limit4 = fsv3030_SEM[40].ToString();
            //    limit5 = fsv3030_SEM[51].ToString();
            //    limit6 = fsv3030_SEM[62].ToString();
            //    string tmp1 = (Math.Max(double.Parse(limit1), double.Parse(limit2))).ToString();
            //    string tmp2 = (Math.Max(double.Parse(limit3), double.Parse(limit4))).ToString();
            //    string tmp3 = (Math.Max(double.Parse(limit5), double.Parse(limit6))).ToString();
            //    string tmp4 = (Math.Max(double.Parse(tmp1), double.Parse(tmp2))).ToString();
            //    semLimit = (Math.Max(double.Parse(tmp4), double.Parse(tmp3))).ToString();
            //    Log("FSV3030 measement: " + "\n"
            //        + "limit1: " + limit1 + "\n"
            //        + "limit2: " + limit2 + "\n"
            //        + "limit3: " + limit2 + "\n"
            //        + "limit4: " + limit2 + "\n"
            //        + "limit5: " + limit2 + "\n"
            //        + "limit6: " + limit2 + "\n");
            absPwr1 = 0;
            absPwr2 = 0;
            absPwr3 = 0;
            semLimit = 1;
            if (mainForm.measurementBackgroundWorker.CancellationPending) return;
            System.Threading.Thread.Sleep(1);
            try
            {
                System.Threading.Thread.Sleep(4000);
                fsv.WriteString("TRAC:DATA? LIST", true);
                object[] fsw13_SEM;
                fsw13_SEM = (object[])fsv.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                string limit1 = fsw13_SEM[7].ToString();
                string limit2 = fsw13_SEM[18].ToString();
                string limit3 = fsw13_SEM[29].ToString();
                string limit4 = fsw13_SEM[40].ToString();
                string limit5 = fsw13_SEM[51].ToString();
                string limit6 = fsw13_SEM[62].ToString();

                string absPower1 = fsw13_SEM[5].ToString();
                string absPower2 = fsw13_SEM[16].ToString();
                string absPower3 = fsw13_SEM[27].ToString();
                string absPower4 = fsw13_SEM[38].ToString();
                string absPower5 = fsw13_SEM[49].ToString();
                string absPower6 = fsw13_SEM[60].ToString();

                double tmp1 = Math.Max(double.Parse(limit1), double.Parse(limit2));
                double tmp2 = Math.Max(double.Parse(limit3), double.Parse(limit4));
                double tmp3 = Math.Max(double.Parse(limit5), double.Parse(limit6));
                double tmp4 = Math.Max(tmp1, tmp2);
                semLimit = Math.Max(tmp4, tmp3);
                Log("FSV13 measement: " + "\n" + "absPower1: " + absPower1 + "\n" + "absPower2: " + absPower2 + "\n" + "absPower3: " + absPower3 + "\n" + "absPower4: " + absPower4 + "\n" + "absPower5: " + absPower5 + "\n" + "absPower6: " + absPower6);

                absPwr1 = Math.Round(Math.Max(double.Parse(absPower3), double.Parse(absPower4)), 1);
                absPwr2 = Math.Round(Math.Max(double.Parse(absPower2), double.Parse(absPower5)), 1);
                absPwr3 = Math.Round(Math.Max(double.Parse(absPower1), double.Parse(absPower6)), 1);
            }
            catch
            {
                Log("FSV13: SEM measurement: FAIL", LogLevel.ERROR);
            }
        }
    }
}
