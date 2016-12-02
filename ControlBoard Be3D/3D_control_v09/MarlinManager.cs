using System;
using Microsoft.SPOT;
using Be3D.Constants;
using System.Threading;
using GHI.Glide;
using GHI.Premium.System;

namespace _3D_control_v09
{
    class MarlinManager
    {
        private static MarlinManager _instance;

        private SDManager _sdManager;
     
        private MarlinManager()
        {
            _sdManager = SDManager.GetInstance();
        }

        public static MarlinManager GetInstace()
        {
            if (_instance == null)
                _instance = new MarlinManager();

            return _instance;
        }
        
        private double[] oldActPoz = new double[5];

        public void CommandWorkerUart(string command)
        {
            if (command == "")
                return;

            if (command.IndexOf("ok") > -1)
            {
                Program.dataOK = true;
                Program.resendData = false;
                //return    // obcas se prijde nesmysl a spoji ok dohromady s jinou instrukci
            }

            if (command == "no")
            {
                Program.dataOK = false;
                Program.resendData = true;
                return;
            }

            //Done printing file
            if (command == "Done printing file")
            {
                LCDManager.GetInstance().DonePrint();

                Debug.Print("PWB: " + command);
                return;
            }

            if (command.IndexOf("actual Z offset:") > -1)
            {
                string[] strArr = command.Split(':');
                if (strArr != null)
                {
                    if (strArr.Length == 2)
                    {
                        strArr[1] = strArr[1].Trim();
                        StateHolder.GetInstance().offsetZ = System.Math.Truncate(Convert.ToDouble(strArr[1]) * 100) / 100; // zaokrouhleni 0.xx 
                        LCDOffsetScreen.GetInstance().UpdateOffset();
                    }
                }
            }

            #region SD init and File managing

            if (command == "echo:SD card ok")
            {
                Program._waitonSdInit = false;
                StateHolder.GetInstance().IsMountingSD = true;

                if ((StateHolder.GetInstance().GetListFiles() == null || StateHolder.GetInstance().GetListFiles().Count == 0) &&
                    StateHolder.GetInstance().ReadingFileListFromSD == false)
                {
                    StateHolder.GetInstance().ReadingFileListFromSD = true;

                    if (!StateHolder.GetInstance().DataLoadingFromSD)
                    {
                        _sdManager.GetNamesOfFilesFromSd();
                    }

                    SendDataToPc(DeeControlManager.GetInstance().StsSdMount());
                }
                return;
            }

            if (command == "echo:SD init fail")
            {
                Program._waitonSdInit = false;

                StateHolder.GetInstance().FreeSpaceSDCard = "null";

                StateHolder.GetInstance().IsMountingSD = false;
                StateHolder.GetInstance().ReadingFileListFromSD = false;
                StateHolder.GetInstance().ClearFilesFromListFiles();
                StateHolder.GetInstance().DataLoadingFromSD = false;

                if (LCDManager.IsActiveModelScreen)
                    LCDManager.GetInstance().UpdateModelScreen();

                SendDataToPc(DeeControlManager.GetInstance().StsSdUnMount());

                return;
            }

            if (command == "Begin file list")
            {
                StateHolder.GetInstance().ReadingFileListFromSD = true;
                return;
            }

            if (command == "End file list")
            {
                StateHolder.GetInstance().ReadingFileListFromSD = false;

                if (LCDManager.IsActiveModelScreen)
                    LCDManager.GetInstance().UpdateModelScreen();

                StateHolder.GetInstance().DataLoadingFromSD = true;
                return;
            }

            if (StateHolder.GetInstance().ReadingFileListFromSD && !Program.dataOK)
            {
                if (command.IndexOf(Constants.Prefix1) > -1) //Prefix
                {
                    StateHolder.GetInstance().AddFileToListFiles(command);
                    return;
                }
            }

            if (command == "File selected")
            {
                Debug.Print(command);
                return;
            }

            //Not SD printing
            if (command == "Not SD printing")
            {
                Program._waitOnSdPrintStatus = false;

                SendDataToPc(DeeControlManager.GetInstance().StsPrintSdNotPrinting());
                return;
            }

            #endregion

            #region SD load parametr from file

            if (command == "Begin file parameters")
            {
                StateHolder.GetInstance().ReadingParametrsFromFile = true;
                return;
            }

            if (command == "End file parameters")
            {
                StateHolder.GetInstance().ReadingParametrsFromFile = false;
                StateHolder.GetInstance().DataParametersLoadingFromSD = true;
                return;
            }

            if (StateHolder.GetInstance().ReadingParametrsFromFile)
            {
                StateHolder.GetInstance().AddParametersToListFiles(command);
                return;
            }

            #endregion

            #region SD kapacita
            // SDCard Free/Capacity: 72510/3850240 KiB

            if (command.IndexOf("SDCard Free/Capacity:") > -1)
            {
                string[] strArr = command.Split(' ');
                if (strArr != null)
                {
                    if (strArr.Length == 4)
                    {
                        string kapacite = strArr[2].Trim();
                        string[] strCap = kapacite.Split('/');
                        try
                        {
                            //int celk = Convert.ToInt32(strCap[1]);
                            //int free = Convert.ToInt32(strCap[0]);
                            Int64 free = Convert.ToInt64(strCap[0]);
                            StateHolder.GetInstance().FreeSpaceSDCard = ((free)*1000).ToString();
                        }
                        catch (Exception)
                        {
                            StateHolder.GetInstance().FreeSpaceSDCard = "null";
                        }
                    }
                }
            }


            #endregion
        
            #region ECHO

            if (command.Length > 5 && command.Substring(0, 5) == "echo:")
            {
                //echo:Marlin 1.4.2
                if (command.IndexOf("echo:Marlin") > -1)
                {
                    string[] str = command.Split(' ');
                    if (str != null)
                        ConfigurationPrinter.GetInstance().verisonFWPower = str[1];
                }

                //echo:endstops hit:  Z:0.00
                if (command.IndexOf("echo:endstops hit:  Z:") > -1)
                {
                    if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.MotionTest)
                    {
                        LCDWaitingScreen.disableIncrement = true;
                        LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
                    }

                    StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Idle;
                }

                if (command.IndexOf("echo:Active Extruder: 0") > -1 || command.IndexOf("echo:Active EXTRUDER: 0") > -1)
                {
                    Program._actSelectExtrOnPowerboard = Constants.EXTRUDER.ExtruderPrimary;
                    // Program.dataOK = true;
                    Debug.Print("PWB: " + command);
                    Debug.Print("G120: akt. extr: " + Program._actSelectExtrOnPowerboard);
                }

                if (command.IndexOf("echo:Active Extruder: 1") > -1 || command.IndexOf("echo:Active EXTRUDER: 1") > -1)
                {
                    Program._actSelectExtrOnPowerboard = Constants.EXTRUDER.ExtruderSecondary;
                    //Program.dataOK = true;
                    Debug.Print("PWB: " + command);
                    Debug.Print("G120: akt. extr: " + Program._actSelectExtrOnPowerboard);

                }

                Debug.Print("PWB: " + command);
                return;
            }

            #endregion

            #region ERROR
            if (command.IndexOf("Error:") > -1)
            {
                Thread.Sleep(200);
                Program._waitOnSdPrintStatus = false;
                Program._waitontemp = false;

                string id = getIDError(command);

                switch (id)
                {
                    case "":
                        return;
                    case "ERR-0101": //Extruder overheated.
                        break;
                    case "ERR-0102": //Extruder heater sensor fail/disconnected. Check cable and restart printer.:
                        break;
                    case "ERR-0103": //cold extrusion prevented
                        break;
                    case "ERR-0104": //too long extrusion prevented
                        break;
                    case "ERR-0105": //No thermistors - no temperature
                        break;
                    case "ERR-0201": //Endstop/motor fail. 
                        break;
                    case "ERR-0301": //Printer halted. kill() called!
                        break;
                    case "ERR-0302": //Printer stopped due to errors.
                        break;
                    case "ERR-0401": //Heatbed overheated.
                        break;
                    case "ERR-0402": //Heatbed sensor fail/disconnected. Check cable and restart printer.
                        break;
                    case "ERR-0501": //error writing to file
                        break;

                    case "ERR-0601": //Line Number is not Last Line Number+1, Last Line
                        return;
                    case "ERR-0602": //checksum mismatch, Last Line: xxx
                        return;
                    case "ERR-0603": //No Checksum with line number, Last Line:
                        return;
                    case "ERR-0604": //No Line Number with checksum, Last Line:
                        return;

                    case "ERR-0999":
                        break;

                    default:
                        return;
                }

                //ukonci tisk pokud se tiskne
                if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print)
                {
                    LCDManager.GetInstance().StopPrint();
                    LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
                }

                ModalResult result = Glide.MessageBoxManager.Show(id, "Error", ModalButtons.Ok);
                Thread.Sleep(1);

                if (result == ModalResult.Ok)
                {
                    Program.HardwareResetPrinter1();
                }

                Debug.Print("PWB: " + command);
                return;
            }

            #endregion
       
            #region Resend
            //Resend: 25
            if (command.Length > 7 && command.Substring(0, 7) == "Resend:")
            {
                string[] str = command.Split(' ');
                if (str.Length > 1)
                    GcodeManagere.SetCountInstruction(Convert.ToInt32(str[1]));

                Debug.Print("PWB: " + command);
                return;
            }
            #endregion

            #region Position read by pause
            //command=X:0.00Y:0.00Z:0.00E:0.00 Count X: 0.00Y:0.00Z:0.00
            if (command.Length > 2 && command.Substring(0, 2) == "X:")
            {
                Debug.Print(command);

                //command=X:0.00Y:0.00Z:0.00E:0.00 Count X: 0.00Y:0.00Z:0.00
                string[] str = command.Split(' ');

                if (str.Length != 4)
                    return;

                //str[3] = 0.00Y:0.00Z:0.00
                string[] param = str[3].Split(':');

                //str[0] = command=X:0.00Y:0.00Z:0.00E:0.00
                string[] paramPlan = str[0].Split(':');

                if (param.Length != 3 || paramPlan.Length != 5)
                    return;
                
                double x = 0.0, y = 0.0, z = 0.0;
                double xPlan = 0.0, yPlan = 0.0, zPlan = 0.0, e0Plan = 0.0, e1Plan = 0.0;

                //param
                x = Convert.ToDouble(param[0].Substring(0, param[0].Length - 1));
                y = Convert.ToDouble(param[1].Substring(0, param[1].Length - 1));
                z = Convert.ToDouble(param[2].Substring(0, param[2].Length));

                //paramPlan
                xPlan = Convert.ToDouble(paramPlan[1].Substring(0, paramPlan[1].Length - 1));
                yPlan = Convert.ToDouble(paramPlan[2].Substring(0, paramPlan[2].Length - 1));
                zPlan = Convert.ToDouble(paramPlan[3].Substring(0, paramPlan[3].Length - 1));

                if (Program._actSelectExtrOnPowerboard == Constants.EXTRUDER.ExtruderPrimary)
                    e0Plan = Convert.ToDouble(paramPlan[4]);

                if (Program._actSelectExtrOnPowerboard == Constants.EXTRUDER.ExtruderSecondary)
                    e1Plan = Convert.ToDouble(paramPlan[4]);

                double[] newActPoz = new double[5] { x, y, z, e0Plan, e1Plan };
                double[] planPoz = new double[5] { xPlan, yPlan, zPlan, e0Plan, e1Plan };

                bool state = StateHolder.GetInstance().ComparePositionXY(newActPoz, oldActPoz);

                if (!UpsManager.correctReadPositonAxis)
                {
                    Program._basicConfig.LatestPositionExtruder = planPoz;
                    oldActPoz = newActPoz;
                }

                if (state)
                {
                    UpsManager.correctReadPositonAxis = true;
                }

                return;
            }
            #endregion

            #region Read printing byte
            //SD printing byte 6305/655157
            if (command.IndexOf("SD printing byte") > -1)
            {
                Program._waitOnSdPrintStatus = false;
                try
                {
                    //SD printing byte 6305/655157
                    string[] data = command.Split(' ');
                    if (data == null || data.Length != 4)
                        return;

                    //6305/655157
                    string[] hodn = data[3].Split('/');
                    if (hodn == null || hodn.Length != 2)
                        return;

                    if (StateHolder.GetInstance().ActPrintState != Constants.PRINT_STATE.Done && StateHolder.GetInstance().ActPrintState != Constants.PRINT_STATE.Pause)
                    {
                        StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Print;
                        SendDataToPc(DeeControlManager.GetInstance().StsPrintStart());
                    }

                    int value = Convert.ToInt32(hodn[0]);

                    StateHolder.GetInstance().SETValuePrint(value);
                    StateHolder.GetInstance().SETValuePrintMax(Convert.ToInt32(hodn[1]));

                    Program._basicConfig.PostitionsInPrintfileOnSd = value;
                    UpsManager.correctReadSDPosition = true;

                    Debug.Print("PWB: " + command);

                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                }

                return;
            }
            #endregion

            #region Temp read
            //T:234.3 E:0 W: 0
            // ok T:63.8 /0.0 B:0.0 /0.0 T0:63.8 /0.0 T1:64.0 /0.0 @:0 B@:0

            if ((command.IndexOf("ok T:") > -1) || (command.IndexOf("T:") > -1))
            //if ((command.Length > 2 && command.Substring(0, 2) == "T:") || (command.Length > 4 && command.Substring(0, 4) == "ok T"))
            // vraci teplotu
            {
                Program._waitontemp = false;
                Program.dataOK = true;

                Debug.Print("PWB: " + command);

                string[] tempExt = command.Split(' ');

                for (int i = 0; i < tempExt.Length; i++)
                {
                    string[] str = tempExt[i].Split(':');
                    if (str == null || str.Length == 0)
                        continue;
                    try
                    {
                        switch (str[0])
                        {
                            case "T":
                                {
                                    if (Program._actSelectExtrOnPowerboard == Constants.EXTRUDER.ExtruderPrimary)
                                        StateHolder.GetInstance().ActTempPrimaryExt = (int)Convert.ToDouble(str[1]);
                                    if (Program._actSelectExtrOnPowerboard == Constants.EXTRUDER.ExtruderSecondary)
                                        StateHolder.GetInstance().ActTempSecondaryExt = (int)Convert.ToDouble(str[1]);
                                    break;
                                }
                            case "T0":
                                {
                                    StateHolder.GetInstance().ActTempPrimaryExt = (int)Convert.ToDouble(str[1]);
                                    StateHolder.GetInstance().ActSetTempInPowerBordPrimExt = (int)Convert.ToDouble(tempExt[i + 1].Substring(1, tempExt[i + 1].Length - 1)); //uklada teplotu nastavenou /0.0
                                    //Program.dataOK = true;
                                    break;
                                }
                            case "T1":
                                {
                                    StateHolder.GetInstance().ActTempSecondaryExt = (int)Convert.ToDouble(str[1]);
                                    StateHolder.GetInstance().ActSetTempInPowerBordSecundExt = (int)Convert.ToDouble(tempExt[i + 1].Substring(1, tempExt[i + 1].Length - 1)); //uklada teplotu nastavenou /0.0
                                    //Program.dataOK = true;
                                    break;
                                }
                            case "T2":
                                {
                                    StateHolder.GetInstance().ActTempThirdExt = (int)Convert.ToDouble(str[1]);
                                    StateHolder.GetInstance().ActSetTempInPowerBordThirdExt = (int)Convert.ToDouble(tempExt[i + 1].Substring(1, tempExt[i + 1].Length - 1)); //uklada teplotu nastavenou /0.0
                                    //Program.dataOK = true;
                                    break;
                                }
                            case "T3":
                                {
                                    StateHolder.GetInstance().ActTempFourthExt = (int)Convert.ToDouble(str[1]);
                                    StateHolder.GetInstance().ActSetTempInPowerBordFourthExt = (int)Convert.ToDouble(tempExt[i + 1].Substring(1, tempExt[i + 1].Length - 1)); //uklada teplotu nastavenou /0.0
                                    //Program.dataOK = true;
                                    break;
                                }
                            case "T4":
                                {
                                    StateHolder.GetInstance().ActTempFifthExt = (int)Convert.ToDouble(str[1]);
                                    StateHolder.GetInstance().ActSetTempInPowerBordFifthExt = (int)Convert.ToDouble(tempExt[i + 1].Substring(1, tempExt[i + 1].Length - 1)); //uklada teplotu nastavenou /0.0
                                    //Program.dataOK = true;
                                    break;
                                }
                            case "@":
                                {

                                    break;
                                }
                            case "B":
                                {
                                    StateHolder.GetInstance().ActTempBed = (int)Convert.ToDouble(str[1]);
                                    break;
                                }
                            case "W":
                                {
                                    //pokud je W:0 start tisk
                                    if (str[1] == "0")
                                        Program.dataOK = true;
                                    break;
                                }

                        }
                    }
                    catch (Exception)
                    {
                        //Program.dataOK = true;
                        break;
                    }
                }

                return;

            }
            #endregion

            if (command.IndexOf("ok") <= -1)
                Debug.Print("PWB: " + command);
        }

        private string getIDError(string errorStr)
        {
            string id = "";
            string[] msgError = errorStr.Split(':');
           
            if (msgError == null || msgError.Length < 2)
                return id;

            msgError[1].Trim();

            if (msgError[1] == "Extruder")
                return "";

            if (msgError[1].IndexOf("Extruder overheated.") > -1)
            {
                id = "ERR-0101";
                return id;
            }
            if (msgError[1].IndexOf("Extruder heater sensor fail/disconnected. Check cable and restart printer.") > -1)
            {
                id = "ERR-0102";
                return id;
            }

            if (msgError[1].IndexOf("cold extrusion prevented") > -1)
            {
                id = "ERR-0103";
                return id;
            }
            if (msgError[1].IndexOf("too long extrusion prevented") > -1)
            {
                id = "ERR-0104";
                return id;
            }
            if (msgError[1].IndexOf("No thermistors - no temperature") > -1)
            {
                id = "ERR-0105";
                return id;
            }
            if (msgError[1].IndexOf("Endstop/motor fail.") > -1)
            {
                id = "ERR-0201";
                return id;
            }
            if (msgError[1].IndexOf("Printer halted. kill() called!") > -1)
            {
                id = "ERR-0301";
                return id;
            }
            if (msgError[1].IndexOf("Printer stopped due to errors.") > -1)
            {
                id = "ERR-0302";
                return id;
            }
            if (msgError[1].IndexOf("Heatbed overheated.") > -1)
            {
                id = "ERR-0401";
                return id;
            }
            if (msgError[1].IndexOf("Heatbed sensor fail/disconnected. Check cable and restart printer.") > -1)
            {
                id = "ERR-0402";
                return id;
            }
            if (msgError[1].IndexOf("error writing to file") > -1)
            {
                id = "ERR-0501";
                return id;
            }

            if (msgError[1].IndexOf("Line Number is not Last Line Number+1, Last Line") > -1)
            {
                id = "ERR-0601";
                return id;
            }

            if (msgError[1].IndexOf("checksum mismatch, Last Line: ") > -1 || msgError[1].IndexOf("checksum mismatch") > -1)
            {
                id = "ERR-0602";
                return id;
            }

            if (msgError[1].IndexOf("No Checksum with line number, Last Line: ") > -1)
            {
                id = "ERR-0603";
                return id;
            }

            if (msgError[1].IndexOf("No Line Number with checksum, Last Line: ") > -1)
            {
                id = "ERR-0604";
                return id;
            }

            return "ERR-0999";

        }

        private void SendDataToPc(string data)
        {
            Program.SendDataToPc(data);
        }
    }
}
