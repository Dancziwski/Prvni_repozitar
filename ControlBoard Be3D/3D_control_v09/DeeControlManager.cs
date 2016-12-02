using System;
using System.Collections;
using System.Text;
using System.Threading;
using LCD;
using Microsoft.SPOT;
using Be3D.Constants;
using GHI.Premium.System;

namespace _3D_control_v09
{

    class DeeControlManager
    {
        private static DeeControlManager _instance;

        private StringBuilder sb;

        private SDManager _sdManager;

        private Thread _threadWatchdog;
        private bool _checkWatchdog = true;

        private DeeControlManager()
        {
            sb = new StringBuilder();
            _sdManager = SDManager.GetInstance();
        }

        public static DeeControlManager GetInstance()
        {
            if (_instance == null)
                _instance = new DeeControlManager();

            return _instance;
        }

        public void CommandWorker(string command)
        {
            if (command == "")
            {
                SendDataToPc("RET:CMD_PARSE_EMPTY;");
                return;
            }

            string[] cmd = command.Split(':');


            if (!CheckValidCmd(cmd))
            {
                SendDataToPc("RET:CMD_PARSE_FAILED;");
                return;
            }


            switch (cmd[0]) //SET,GET,FIL atd.
            {
                #region GET

                case "GET":
                    switch (cmd[1]) //PRINTINFO, STRINGTABLE atd.....
                    {
                        case "PRINTINFO":
                            SendDataToPc(GetPrintInfo());
                            break;
                        case "SDSPACE":
                            SendDataToPc(GetSdSpace());
                            break;
                        case "EXTTEMP":
                            SendDataToPc(GetExtruderTemp());
                            break;
                        case "BEDTEMP":
                            SendDataToPc(GetBetTemp());
                            break;
                        case "SPACETEMP":
                            SendDataToPc(GetSpaceTemp());
                            break;
                        case "PRINTSTATE":
                            SendDataToPc(GetPrintState());
                            break;
                        case "LOADFILE":
                            SendDataToPc(GetLoadFile());
                            break;
                        case "CARTRIGEINFO":
                            SendDataToPc(GetCartrigeInfo());
                            break;
                        case "ETHSETTING":
                            SendDataToPc(GetEthSettings());
                            break;
                        default:
                            //Chyba, neznamy GET prikaz 
                            SendDataToPc("RET:UNKNOWN_GET_COMMAND|" + cmd[1] + ";");
                            break;
                    }
                    break;

                #endregion

                #region SET

                case "SET":

                    string[] cmdNext = cmd[1].Split('|');

                    if (!CheckValidCmdNext(cmdNext))
                    {
                        SendDataToPc("RET:CMD_SET_PARSE_FAILED|" + cmd[1] + ";");
                        return;
                    }

                    switch (cmdNext[0])
                    {
                        //SET:asdf5628gethydfshtgfgjyu;     //resetuje pocitaldo motohodin  
                        case "asdf5628gethydfshtgfgjyu":
                            SendDataToPc(ResetOperatingTimeInMemory());
                            break;
                        case "SERIALNUMBER":
                            SendDataToPc(SetSerialNumber(cmdNext[1]));
                            break;
                        case "EXTTEMP_1":
                            SendDataToPc(SetTempExtrRight(cmdNext[1]));
                            break;

                        case "EXTTEMP_2":
                            SendDataToPc(SetTempExtrLeft(cmdNext[1]));
                            break;

                        case "BEDTEMP":
                            SendDataToPc(SetTempBed(cmdNext[1]));
                            break;

                        case "SPACETEMP":
                            SendDataToPc(SetTempSpace(cmdNext[1]));
                            break;

                        case "STARTPRINTFILE":
                            SendDataToPc(StartPrintFile(cmdNext[1]));
                            break;
                        case "PRINTBUFFER":
                            if (cmdNext.Length == 3)    //SET:PRINTBUFFER|START|nazevsouboru;
                            {
                                SendDataToPc(PrintBuffer(cmdNext[1], cmdNext[2]));
                                break;
                            }
                            if (cmdNext.Length == 2)    //SET:PRINTBUFFER|START;
                            {
                                SendDataToPc(PrintBuffer(cmdNext[1], ""));
                                break;
                            }
                            SendDataToPc("RET:PRINTBUFFER|FAIL;");
                            break;

                        case "STOPPRINT":
                            SendDataToPc(StopPrint());
                            break;

                        case "PREHEAT":
                            SendDataToPc(Preheat(cmdNext[1]));
                            break;

                        case "UPDATECOPY":
                            SendDataToPc(UpdateCopy(cmdNext[1]));
                            break;

                        case "UPDATE":
                            SendDataToPc(Update(cmdNext[1]));
                            break;
                        //SET:RESTART|RUN;
                        case "RESTART":
                            SendDataToPc(Update("RET:RESTART;"));
                            Microsoft.SPOT.Hardware.PowerState.RebootDevice(false);
                            break;

                        default:
                            SendDataToPc("RET:UNKNOWN_SET_CMD|" + cmd[1] + ";");
                            break;
                    }
                    break;

                #endregion

                case "GCD":
                    SendDataToPc(Gcd(cmd[1]));
                    break;

                #region FIL

                case "FIL":

                    string[] cmdFil = cmd[1].Split('|');

                    if (!CheckValidCmdNext(cmdFil))
                    {
                        SendDataToPc("RET:CMD_FIL_PARSE_FAILED|" + cmd[1] + ";");
                        return;
                    }

                    switch (cmdFil[0])
                    {
                        case "MKD":
                            if (cmdFil[1] == "")
                            {
                                Debug.Print("RET:FIL|FILENAME_EMPTY;");
                                SendDataToPc("RET:FIL|FILENAME_EMPTY;");
                                break;
                            }

                            if (!StateHolder.GetInstance().IsMountingSD)
                            {
                                Debug.Print("RET:MKD|SD_ERROR;");
                                SendDataToPc("RET:MKD|SD_ERROR;");
                                break;
                            }

                            Program.DiscartUartPrinter1();
                            Program.EndThreadRefreshState();

                            //Thread.Sleep(500);

                            SendDataToPc(FilMKD(cmdFil[1]));
                            break;
                        case "DATA":
                            SendDataToPc(FilData(cmdFil[1]));
                            break;

                        case "ENDWRITE":               
                            SendDataToPc(FilEndWrite());
                            break;
                        case "MOUNTSD":
                            SendDataToPc(FilMountSd());
                            break;
                        case "UPDATEDATA":
                            SendDataToPc(UpdateData(cmdFil[1]));
                            break;

                    }
                    break;

                #endregion

                default:
                    SendDataToPc("RET:UNKNOWN_CMD|" + cmd[1] + ";");
                    break;
            }
        }

        private bool CheckValidCmd(string[] cmd)
        {
            if (cmd == null)
                return false;
            if (cmd.Length != 2)
                return false;

            return true;
        }

        private bool CheckValidCmdNext(string[] cmdNext)
        {
            if (cmdNext == null)
                return false;
            if (cmdNext.Length == 0)
                return false;

            return true;
        }

        private void SendDataToPc(string data)
        {
            Program.SendDataToPc(data);
        }

        private string ResetOperatingTimeInMemory()
        {
            Program.ResetOperatingTime();

            return "RET:RESETOPERATIONTIME|OK;";
        }


        #region GET command

        public string GetPrintInfo()
        {
            sb.Clear();
            //  sb.Append("GET:PRINTINFO;");
            sb.Append("RET:SERIALNUMBER|" + Program._basicConfig.GetSerialNumber() + ";");
            sb.Append("RET:NAME|" + ConfigurationPrinter.GetInstance().GetNameOfPrinter() + ";");
            sb.Append("RET:MANUFACTURER|" + ConfigurationPrinter.GetInstance().GetNameOfManufacter() + ";");
            sb.Append("RET:TYPE|" + ConfigurationPrinter.GetInstance().GetNameOfTypePrinter() + ";");
            sb.Append("RET:FWCV|" + ConfigurationPrinter.GetInstance().verisonFWControl + ";");
            sb.Append("RET:FWPV|" + ConfigurationPrinter.GetInstance().verisonFWPower + ";");

            sb.Append("END:PRINTINFO;");
            return sb.ToString();
        }

        public string GetSdSpace()
        {
            StateHolder.GetInstance().FreeSpaceSDCard = "null";

            if (StateHolder.GetInstance().IsMountingSD == false) // nemuze byt zaroven nevlozena karta a nejake volne misto....
                return "RET:SDSPACE|0;";

            _sdManager.GetStateSpaceSD();

            sb.Clear();
            string hodn = StateHolder.GetInstance().FreeSpaceSDCard;

            for (int i = 0; i < 15; i++)
            {
                if (hodn == "null" || hodn == "NULL")
                {
                    Thread.Sleep(200);
                    hodn = StateHolder.GetInstance().FreeSpaceSDCard;
                }
            }

            if (hodn == "null" || hodn == "NULL")
                return "RET:SDSPACE|0;";

            sb.Append("RET:SDSPACE|" + StateHolder.GetInstance().FreeSpaceSDCard + ";");

            Debug.Print(sb.ToString());

            return sb.ToString();
        }


        public string GetCartrigeInfo()
        {
            sb.Clear();
            sb.Append("RET:CRT_1_EMPTY;");
            sb.Append("RET:CRT_2_EMPTY;");

         /*   Cartrige cart = CartrigeManager.GetInstance().GetCartrige(Constants.CARTRIGE.CartExtrRight);

            if (cart.Id != "null")
            {

                sb.Append("RET:CRT_1_ID|" + cart.Id + ";");
                sb.Append("RET:CRT_1_BRAND|" + cart.Brand + ";");
                sb.Append("RET:CRT_1_TYPE|" + cart.Material + ";");
                sb.Append("RET:CRT_1_COLOR|" + cart.Color + ";");
                sb.Append("RET:CRT_1_QUANTITY|" + cart.Quantity + ";");
                sb.Append("RET:CRT_1_QUANTITYMAX|" + cart.QuantityMax + ";");

                if (cart.QuantityFullWeight > 0 && cart.QuantityFullWeight <= 750)
                    sb.Append("RET:CRT_1_WEIGHTMAX|" + cart.QuantityFullWeight + ";");
            }
            else
            {
                sb.Append("RET:CRT_1_EMPTY;");
            }

            cart = CartrigeManager.GetInstance().GetCartrige(Constants.CARTRIGE.CartExtrLeft);

            if (cart.Id != "null")
            {
                sb.Append("RET:CRT_2_ID|" + cart.Id + ";");
                sb.Append("RET:CRT_2_BRAND|" + cart.Brand + ";");
                sb.Append("RET:CRT_2_TYPE|" + cart.Material + ";");
                sb.Append("RET:CRT_2_COLOR|" + cart.Color + ";");
                sb.Append("RET:CRT_2_QUANTITY|" + cart.Quantity + ";");
                sb.Append("RET:CRT_2_QUANTITYMAX|" + cart.QuantityMax + ";");

                if (cart.QuantityFullWeight > 0 && cart.QuantityFullWeight <= 750)
                    sb.Append("RET:CRT_2_WEIGHTMAX|" + cart.QuantityFullWeight + ";");
            }
            else
            {
                sb.Append("RET:CRT_2_EMPTY;");
            }*/

            sb.Append("END:CARTRIGEINFO;");
            return sb.ToString();

        }

        public string GetExtruderTemp()
        {
            sb.Clear();
            sb.Append("RET:EXTTEMP_1|" + StateHolder.GetInstance().ActTempSecondaryExt + "," + StateHolder.GetInstance().ActSetTempSecondary + ";");

            if (ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder())
                sb.Append("RET:EXTTEMP_2|" + StateHolder.GetInstance().ActTempPrimaryExt + "," + StateHolder.GetInstance().ActSetTempPrimary + ";");
            else
                sb.Append("RET:EXTTEMP_2|FAIL_NOTEXT;");

            sb.Append("END:EXTTEMP;");
            return sb.ToString();
        }

        public string GetBetTemp()
        {
            sb.Clear();

            if (!ConfigurationPrinter.GetInstance().IsPresentBedHeat())
                return "RET:BEDTEMP|FAIL_NOTBED;";

            sb.Append("RET:BEDTEMP|" + StateHolder.GetInstance().ActTempBed + "," + StateHolder.GetInstance().ActSetTempBed + ";");

            return sb.ToString();
        }

        public string GetSpaceTemp()
        {
            sb.Clear();
            return "RET:SPACETEMP|FAIL_NOTSPACE;";
        }

        public string GetPrintState()
        {
            sb.Clear();

            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print)
            {
                if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Init)
                    sb.Append("RET:STATEINITPRINT;");
                if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Print)
                    sb.Append("RET:STATEPRINT|" + StateHolder.GetInstance().GETValuePrint() + "," + StateHolder.GetInstance().GETValuePrintMax() + ";");
                if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Pause)
                    sb.Append("RET:STATEPAUSE;");
                if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Stop)
                    sb.Append("RET:STATESTOP|xxxxxxxx;");
            }
            else
            {
                sb.Append("RET:STATEIDLE;");
            }


            return sb.ToString();
        }

        public string GetLoadFile()
        {
            sb.Clear();
            ArrayList listOfFiles = null;

            if (StateHolder.GetInstance().IsMountingSD)
                listOfFiles = StateHolder.GetInstance().GetListFiles();

            if (listOfFiles != null)
            {
                for (int i = 0; i < listOfFiles.Count; i++)
                {
                    sb.Append("RET:ITEM|" + ((string)listOfFiles[i]).Substring(4) + ";");
                }

                sb.Append("END:LOADFILE;");
            }
            else
            {
                sb.Append("RET:LOADFILE|SD_FAIL;");
            }

            return sb.ToString();
        }

        public string GetEthSettings()
        {
            sb.Clear();
            sb.Append("RET:ETHSETTING|NOT DEFINED;");

            return sb.ToString();
        }

        #endregion

        #region SET command

        private string SetSerialNumber(string num)
        {
            sb.Clear();
            if (Program._basicConfig.GetSerialNumber() == "unknown")
            {
                Program._basicConfig.SetSerialNumber(num);
                Program.SaveConfigToEEprom();

                if (Program._basicConfig.GetSerialNumber() == num)
                    sb.Append("RET:SERIALNUMBER|OK;");
                else
                    sb.Append("RET:SERIALNUMBER|FAIL;");
            }
            else
                sb.Append("RET:SERIALNUMBER|READ_ONLY;");

            return sb.ToString();
        }

        private string SetTempExtrRight(string temp)
        {
            const string failmsg = "RET:EXTTEMP_1|FAIL_VALUE;";

            if (temp == "")
                return failmsg;
            try
            {
                int iTemp = Int32.Parse(temp);
                StateHolder.GetInstance().ActSetTempSecondary = iTemp;
            }
            catch (Exception)
            {
                return failmsg;
            }

            return "RET:EXTTEMP_1|OK;";
        }

        private string SetTempExtrLeft(string temp)
        {
            const string failmsg = "RET:EXTTEMP_2|FAIL_VALUE;";

            if (!ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder())
                return "RET:EXTTEMP_2|FAIL_NOTEXT;";

            if (temp == "")
                return failmsg;
            try
            {
                int iTemp = Int32.Parse(temp);
                StateHolder.GetInstance().ActSetTempPrimary = iTemp;
            }
            catch (Exception)
            {
                return failmsg;
            }

            return "RET:EXTTEMP_2|OK;";
        }

        private string SetTempBed(string temp)
        {
            const string failmsg = "RET:BEDTEMP|FAIL_VALUE;";

            if (!ConfigurationPrinter.GetInstance().IsPresentBedHeat())
                return "RET:BEDTEMP|FAIL_NOTBED;";

            if (temp == "")
                return failmsg;
            try
            {
                int iTemp = Int32.Parse(temp);
                StateHolder.GetInstance().ActSetTempBed = iTemp;
            }
            catch (Exception)
            {
                return failmsg;
            }

            return "RET:BEDTEMP|OK;";
        }

        private string SetTempSpace(string temp)
        {
            return "RET:SPACETEMP|FAIL_NOTSPACE;";
        }

        public string StartPrintFile(string name)
        {
            Debug.Print("PC: Start print file...");

            if (!StateHolder.GetInstance().IsMountingSD)
                return "RET:STARTPRINTFILE|SD_FAIL;";

            if (name == "")
                return "RET:STARTPRINTFILE|FILE;";

            StateHolder.GetInstance().PrintFile = name;
            Program.SaveConfigToEEprom();

            SDManager.GetInstance().GetParametersFromFile();


            if ((SwitchManager.GetInstance().IsOpenDoor1() || SwitchManager.GetInstance().IsOpenDoor2()))
            {
                return "RET:STARTPRINTFILE|DOOR;";
            }

            if (!(SwitchManager.GetInstance().IsFilamentExt1() && SwitchManager.GetInstance().IsFilamentExt2()))
            {
                return "RET:STARTPRINTFILE|CATGUT;";
            }

            LCDManager.GetInstance().UpdateScreenLcd(LCDScreen.StateScreen.PrintInfo, "");

            return "RET:STARTPRINTFILE|OK;";
        }

        public string PrintBuffer(string state, string namePrintFile)
        {
            if ((SwitchManager.GetInstance().IsOpenDoor1() || SwitchManager.GetInstance().IsOpenDoor2()))
                return "RET:PRINTBUFFER|DOOR;";

            if (!(SwitchManager.GetInstance().IsFilamentExt1() && SwitchManager.GetInstance().IsFilamentExt2()))
                return "RET:PRINTBUFFER|CATGUT;";

            StateHolder.GetInstance().PrintFile = namePrintFile;
            Program.SaveConfigToEEprom();

            if (state == "START")
            {
                Program.PrintThreadBufferActive = true;
                StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Print;

                LCDManager.GetInstance().UpdateScreenLcd(LCDScreen.StateScreen.PrintInfo, "");
                Program.StartThreadBufferPrint();

            }

            if (state == "STOP")
            {
                LCDManager.GetInstance().StopPrint();              
                LCDManager.GetInstance().GoToHomePosition();

                LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(),"");
            }

            if (state == "DONE")
            {
                Program.PrintThreadBufferActive = false;
                while (Program.GetBufferUartSenderPrinter1().Count == 0)
                {
                    Thread.Sleep(100);
                }

                Program.SendDataFromPC = true;
                LCDManager.GetInstance().DonePrint();
            }

            return "RET:PRINTBUFFER|OK;";
        }

        private string StopPrint()
        {
            Debug.Print("PC: Stop print...");

            if (StateHolder.GetInstance().ActState != Constants.ACTUAL_STATE.Print)
            {
                //vola se kdyz se ma prerusit prenos souboru
                _checkWatchdog = false;
                return "RET:STOPPRINT|FAIL;";
            }

            StateHolder.GetInstance().FileDataTransfer = false;
            
            LCDManager.GetInstance().StopPrint();
            
            LCDManager.GetInstance().GoToHomePosition();

            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");

            return "RET:STOPPRINT|OK;";
        }

        private string Preheat(string state)
        {
            if (state == "START")
                LCDManager.GetInstance().StartPreHeating();
            if (state == "STOP")
                LCDManager.GetInstance().StopPreheating();

            return "RET:PREHEAT|OK;";
        }

        public string UpdateCopy(string state)
        {
            if (state == "START")
            {
                Debug.Print("Start update FW");

                LCDManager.GetInstance().StopPreheating();

                Program.EndThreadRefreshState();
                Program.EndThreadSenderReceiverDataForPrinter();

                // zobrazit cekaci smycka na pausu
                LCDWaitingScreen.GetInstance().UpdateScreen(false, Resources.GetString(Resources.StringResources.TextUpdateFW), "");
                LCDManager.GetInstance().UpdateScreenLcd(LCD.LCDScreen.StateScreen.WaitingPause, "");

                //LCDWaitingScreen.GetInstance().StartIncrementProgress();
                LCDWaitingScreen.disableIncrement = false;

                StateHolder.GetInstance().FileDataTransfer = true;
                UpdateManager.GetInstance().ActivateUpdateOnTheFlight(GHI.Premium.System.SystemUpdate.SystemUpdateType.Deployment);

            }

            if (state == "DONE")
            {
                Debug.Print("Done update FW");
            }

            return "RET:UPDATECOPY|OK;";

        }

        private static byte[] bData = new byte[1024];
        private static string crc = "";

        public string UpdateData(string data)
        {
            sb.Clear();
            bData = Convert.FromBase64String(data);
            crc = UpdateManager.calculateCRC(bData);


            if (UpdateManager.GetInstance().AddDataToFlash(GHI.Premium.System.SystemUpdate.SystemUpdateType.Deployment, bData))
            {
                sb.Append("RET:CHECK|" + crc + ";");
            }
            else
            {
                sb.Append("RET:CHECK|FAIL;");
            }

            sb.Append("END:UPDATEDATA;");
            return sb.ToString();
        }

        public string Update(string state)
        {
            if (state == "RUN")
            {
                if (UpdateManager.GetInstance().RunUpdateOnTheFlight())
                {
                    return "RET:UPDATE|OK;";
                }
                else
                {
                    Program.SendDataToPc("RET:UPDATE|FAIL;");
                }
            }

            return "RET:UPDATE|FAIL;";
        }

        #endregion

        #region GCD

        public string Gcd(string code)
        {
            if (code == "")
                return "RET:GCD|FAIL_EMPTY;";

                Program.SendDataToPrinter1(code);
                return "RET:GCD|OK;";
        }

        #endregion

        #region FILE command

        public string ActFileName = "";

        public string FilMKD(string fileName)
        {
            Debug.Print("PC: Create file on SD and start transfer...");

            ActFileName = fileName;

            _sdManager.CreateFileAndStartWriteDataToFile(ActFileName);

            _threadWatchdog = new Thread(Watchdog);
            _threadWatchdog.Start();


            LCDManager.GetInstance().ShowErrorScreenLcd(LCDManager.GetInstance().GetMainScreen(),
                                          Resources.GetString(Resources.StringResources.TextTransferFile), "Warning", false);

            return "RET:MKD|OK;";
        }

        private void Watchdog()
        {
            _checkWatchdog = true;

            while (_checkWatchdog)
            {
                _checkWatchdog = false;
                Thread.Sleep(5000);
            }

            if (StateHolder.GetInstance().FileDataTransfer) // pokud je true - data by mela stale bezet proto resetni
            {
                Debug.Print("Restart watchdog");
                StateHolder.GetInstance().FileDataTransfer = false;
                Program.HardwareResetPrinter1();
                LCDManager.IsActivateErrorScreen = false;

                LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
                
            }
        }

        public string FilData(string gcode)
        {
            if (Program.PrintThreadBufferActive)
            {
                Program.SendDataFromPC = true;

                char[] sep = new char[] { '\r', '\n' };
                string[] arr = gcode.Split(sep);

                for (int i = 0; i < arr.Length; i++)
                {
                    Program.GetBufferUartSenderPrinter1().Enqueue(arr[i]);
                }
            }
            else
            {
                _checkWatchdog = true;
                _sdManager.AddDataToFile(gcode);
                //Debug.Print("Data add to Buff");
            }
            return "RET:DATA|OK;";
        }

        public string FilEndWrite()
        {
            Debug.Print("PC: End transfer file...");

            _sdManager.EndWriteDataToFile(ActFileName);

            while (Program.GetBufferUartSenderPrinter1().Count != 0)
            {
                _checkWatchdog = true;
                Thread.Sleep(100);
            }

            ActFileName = "";
            _checkWatchdog = false; // ukonci vlakno watchdog

            Program.CreateThreadRefreshState();

            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
            return "RET:ENDWRITE|OK;";
        }

        private const string retMOUNT_SD_OK = "RET:MOUNTSD|OK;";
        private const string retMOUNT_SD_FAIL = "RET:MOUNTSD|FAIL;";

        public string FilMountSd()
        {
            Debug.Print("PC: Mount SD...");

            _sdManager.InitSD();

            for (int i = 0; i < 30; i++)
            {
                if (StateHolder.GetInstance().IsMountingSD)
                    return retMOUNT_SD_OK;

                Thread.Sleep(100);
            }

            Debug.Print(retMOUNT_SD_FAIL);

            /*   if (StateHolder.GetInstance().IsMountingSD)
                   return retMOUNT_SD_OK;*/

            return retMOUNT_SD_FAIL;
        }

        #endregion

        #region STATUS command

        public string StsPreheatStart()
        {
            return "STS:PREHEATER;";
        }

        public string StsPreheatStop()
        {
            return "STS:PREHEATER|STOP;";
        }

        public string StsPrintStart()
        {
            return "STS:PRINT|START;";
        }

        public string StsPrintPause()
        {
            return "STS:PRINT|PAUSE;";
        }

        public string StsPrintStop()
        {
            return "STS:PRINT|STOP;";
        }

        public string StsPrintDone()
        {
            return "STS:PRINT|DONE;";
        }

        public string StsPrintSdNotPrinting()
        {
            return "STS:PRINT|SDNOTPRINTING;";
        }

        public string StsCartrigeDown(Constants.CARTRIGE cart)
        {
            if (cart == Constants.CARTRIGE.CartExtrRight)
                return "STS:INFO|CARTRIGE_DOWN|1;";

            if (cart == Constants.CARTRIGE.CartExtrLeft)
                return "STS:INFO|CARTRIGE_DOWN|2;";

            //asi nikdy nenastane
            return "";
        }

        public string StsCartrigeUp(Constants.CARTRIGE cart)
        {
            if (cart == Constants.CARTRIGE.CartExtrRight)
                return "STS:INFO|CARTRIGE_UP|1;";

            if (cart == Constants.CARTRIGE.CartExtrLeft)
                return "STS:INFO|CARTRIGE_UP|2;";

            //asi nikdy nenastane
            return "";
        }

        public string StsSdMount()
        {
            return "STS:SD|MOUNT;";
        }

        public string StsSdUnMount()
        {
            return "STS:SD|UNMOUNT;";
        }

        public string StsSdWriteError()
        {
            return "STS:SD|WRITEERROR;";
        }


        #endregion

    }
}
