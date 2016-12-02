using System;
using System.Threading;
using GHI.Glide;
using LCD;
using Microsoft.SPOT;
using Be3D.Constants;

namespace _3D_control_v09
{
    class LCDManager
    {
        private static LCDManager _instance;

        public static bool IsActivateNewCartrige = false;
        public static bool IsActivateErrorScreen = false;
        public static bool IsActivateErrorCartrigeScreen = false;

        public static bool IsActivePreheatScreen = false;
        public static bool IsActiveChangeFilamentScreen = false;
        public static bool IsActiveChangeExtrudingScreen = false;
        public static bool IsActiveModelScreen = false;
        public static bool IsActivePrintInfoScreen = false;


        //private static Window _actualWindow;
        private LCDScreen _screen;
        private SDManager _sdManager;
        private GcodeManagere _gCodeManager;

        private readonly DeeControlManager _deeContrManager;

        private LCDManager()
        {
            _deeContrManager = DeeControlManager.GetInstance();
            _sdManager = SDManager.GetInstance();
            _gCodeManager = GcodeManagere.GetInstance();

            Init();
        }

        public static LCDManager GetInstance()
        {
            if (_instance == null)
                _instance = new LCDManager();

            return _instance;
        }

        private void Init()
        {

            GHI.Premium.Hardware.Configuration.LCD.Configurations lcdConfig =
                new GHI.Premium.Hardware.Configuration.LCD.Configurations();

            //TE35
            lcdConfig.Width = 320;
            lcdConfig.Height = 240;
            lcdConfig.OutputEnableIsFixed = true;
            lcdConfig.OutputEnablePolarity = true;
            lcdConfig.HorizontalSyncPolarity = false;
            lcdConfig.VerticalSyncPolarity = false;
            lcdConfig.PixelPolarity = true;
            lcdConfig.HorizontalSyncPulseWidth = 41;
            lcdConfig.HorizontalBackPorch = 29;
            lcdConfig.HorizontalFrontPorch = 51;
            lcdConfig.VerticalSyncPulseWidth = 10;
            lcdConfig.VerticalBackPorch = 3;
            lcdConfig.VerticalFrontPorch = 16;
            lcdConfig.PixelClockRateKHz = 15000;

            GHI.Premium.Hardware.Configuration.LCD.EnableLCDBootupMessages(false);

            if (GHI.Premium.Hardware.Configuration.LCD.Set(lcdConfig))
                Microsoft.SPOT.Hardware.PowerState.RebootDevice(false);


            // Resize any loaded Window to the LCD's size.
            Glide.FitToScreen = true;

            // Activate touch
            GlideTouch.Initialize();

            showLogo();

            _screen = LCDMainScreen.GetInstance();

            if (Program.UPS == Constants.UPS.OK)
            {
                if (Program._basicConfig.GetStrReserve1() != "" && Program._basicConfig.GetStrReserve1() != "null")
                {
                    string msg = Resources.GetString(Resources.StringResources.TextContinePrintingFromOffState);

                    LCDPausePrint.GetInstance().UpdateMessage(msg);
                    _screen = LCDPausePrint.GetInstance();

                }
            }

            // prehodi main screen na stolici
            if (Program.Type == Constants.TYPE_PRINTER.DREStolice)
            {
                _screen = LCDMainScreenStolice.GetInstance();
            }

            if (Program.Type == Constants.TYPE_PRINTER.DGRStolice3)
            {
                _screen = LCDMainScreenStolice.GetInstance();
            }

            ChangeWindow();

        }

        private void showLogo()
        {
            Bitmap LCD = new Bitmap(320, 240);
            Bitmap logo = ResGUI.GetBitmap(ResGUI.BitmapResources.logo1);
            LCD.Clear();
            LCD.DrawImage(0, 0, logo, 0, 0, 320, 240);
            LCD.Flush();
            Thread.Sleep(2000);
        }

        private void ChangeWindow()
        {
            //Tween.SlideWindow(null, _screen.getWindow(), Direction.Up);
            Glide.MainWindow = null;
            Glide.MainWindow = _screen.getWindow();

            //Debug.GC(true);
        }

     /*   public void SaveConfiguration()
        {
            Program.SaveConfig();
        }
        */
        public void UpsDown()
        {
            UpsManager.upsEnable.Write(false);
        }

        public void UpsUp()
        {
            UpsManager.upsEnable.Write(true);
        }

        public void ShowErrorScreenLcd(LCDScreen.StateScreen previousState, String message, String messMain, bool isBackButton)
        {
            if (!IsActivateErrorScreen)
            {
                _screen = new LCDErrorScreen(previousState, message, messMain, isBackButton);
                ChangeWindow();
                IsActivateErrorScreen = true;
            }
        }

        public void SaveConfiguration()
        {
            Program.SaveConfigToEEprom();
        }

        #region update

        public void UpdateScreenLcd(LCDScreen.StateScreen stateScreen, String message)
        {

            switch (stateScreen)
            {
                case LCDScreen.StateScreen.Main:
                    {
                        #region main

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        //musi byt jinak hrozi to ze nepujde opakovane spustit tisk 
                        StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Idle; 

                        _screen = LCDMainScreen.GetInstance();

                        ChangeWindow();

                        break;

                        #endregion
                    }
                case LCDScreen.StateScreen.MainStolice:
                    {
                        #region mainStolice

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        //musi byt jinak hrozi to ze nepujde opakovane spustit tisk 
                        StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Idle;

                        _screen = LCDMainScreenStolice.GetInstance();

                        ChangeWindow();

                        break;

                        #endregion
                    }
                case LCDScreen.StateScreen.Info:
                    {
                        #region info

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        LCDInfoScreen.GetInstance().UpdateData(Program._basicConfig.OperationTime);
                        _screen = LCDInfoScreen.GetInstance();

                        ChangeWindow();
                        break;

                        #endregion
                    }
                case LCDScreen.StateScreen.Settings:
                    {
                        #region settings

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        _screen = LCDSettingsScreen.GetInstance();

                        ChangeWindow();
                        break;

                        #endregion
                    }
                case LCDScreen.StateScreen.ShutDown:
                    {
                        #region shutdown

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        _screen = LCDShutdownScreen.GetInstance();

                        ChangeWindow();
                        break;

                        #endregion
                    }
                case LCDScreen.StateScreen.Model:
                    {
                        #region model

                        IsActivePreheatScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActiveModelScreen = true;

                        LCDModelScreen.GetInstance().ClearFiles();
                        _screen = LCDModelScreen.GetInstance();

                        ChangeWindow();

                        break;

                        #endregion
                    }

                case LCDScreen.StateScreen.CatGutChange:
                    {
                        #region cat

                        IsActiveModelScreen = false;
                        IsActivePreheatScreen = false;

                        IsActiveChangeFilamentScreen = true;

                        if (Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Constants.TYPE_PRINTER.DeeRed2_1)
                        {
                            LCDCatGutChangeSelector.GetInstance().LoadPrintTemper();
                            _screen = LCDCatGutChangeSelector.GetInstance();
                        }
                        else
                        { 
                            LCDCatGutChange.GetInstance().LoadPrintTemper();
                            _screen = LCDCatGutChange.GetInstance();
                        }

                        ChangeWindow();

                        #endregion

                        break;
                    }
                case LCDScreen.StateScreen.CatGutChangeTestExtruding:
                    {
                        #region cat

                        IsActiveModelScreen = false;
                        IsActivePreheatScreen = false;

                        IsActiveChangeExtrudingScreen = true;

                        LCDCatGutChangeTestExtruding.GetInstance().LoadPrintTemper();
                        _screen = LCDCatGutChangeTestExtruding.GetInstance();

                        ChangeWindow();

                        #endregion

                        break;
                    }

                case LCDScreen.StateScreen.PausePrint:
                    {
                        #region pausePrint

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        LCDPausePrint.GetInstance().UpdateMessage(message);
                        _screen = LCDPausePrint.GetInstance();

                        ChangeWindow();

                        #endregion

                        break;
                    }
                case LCDScreen.StateScreen.Preheat:
                    {
                        #region preheat
                        //pouze nacteni screen pro spusteni predehrevu

                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActiveModelScreen = false;
                        IsActivePreheatScreen = true;
                        LCDPreheatScreen.GetInstance().UpdateState(StateHolder.GetInstance().StateHeating);
                        _screen = LCDPreheatScreen.GetInstance();
                        ChangeWindow();

                        #endregion

                        break;
                    }
                case LCDScreen.StateScreen.PrintInfo:
                    {
                        #region printinfo

                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;
                        IsActiveModelScreen = false;
                        IsActivateErrorScreen = false;
                        
                        IsActivePrintInfoScreen = true;

                        LCDPrintInfoScreen.GetInstance().StartPrint();
                        LCDPrintInfoScreen.GetInstance().UpdateAll();

                        if (Program.PrintThreadBufferActive)
                            LCDPrintInfoScreen.GetInstance().UpdateVisibleComponent(false, false);
                        else
                            LCDPrintInfoScreen.GetInstance().UpdateVisibleComponent(true, true);

                        _screen = LCDPrintInfoScreen.GetInstance();
                        ChangeWindow();

                        #endregion

                        break;
                    }
                case LCDScreen.StateScreen.TestServo:
                    {
                        #region Tests

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        _screen = LCDTestServoScreen.GetInstance();

                        ChangeWindow();
                        break;

                        #endregion

                        break;
                    }
                case LCDScreen.StateScreen.Tests:
                    {
                        #region Tests

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        _screen = LCDTestScreen.GetInstance();

                        ChangeWindow();
                        break;

                        #endregion

                        break;
                    }
                case LCDScreen.StateScreen.SettingAnother:
                    {
                        #region settinganother

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        _screen = LCDSettingsAnotherScreen.GetInstance();

                        ChangeWindow();

                        #endregion

                        break;
                    }

                case LCDScreen.StateScreen.SettingTemp:
                    {
                        #region settingtemp

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        _screen = new LCDSettingsTempScreen();

                        ChangeWindow();

                        #endregion

                        break;
                    }
                case LCDScreen.StateScreen.WaitingPause:
                    {
                        #region waitingPause

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        _screen = LCDWaitingScreen.GetInstance();

                        ChangeWindow();

                        #endregion

                        break;
                    }
                case LCDScreen.StateScreen.SettingOffset:
                    {
                        #region settingOffset

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        LCDOffsetScreen.GetInstance().RefreshOffset();
                        _screen = LCDOffsetScreen.GetInstance();

                        ChangeWindow();

                        #endregion

                        break;
                    }

                case LCDScreen.StateScreen.Stop:
                    {
                        #region stop

                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        _screen = new LCDStopScreen();

                        ChangeWindow();

                        #endregion

                        break;
                    }
                case LCDScreen.StateScreen.Language:
                    {
                        #region language
                        IsActiveModelScreen = false;
                        IsActiveChangeFilamentScreen = false;
                        IsActiveChangeExtrudingScreen = false;
                        IsActivePreheatScreen = false;

                        LCDLanguageScreen.GetInstance().UpdateScreenLang();
                        _screen = LCDLanguageScreen.GetInstance();

                        ChangeWindow();

                        #endregion

                        break;
                    }
            }
        }

        public void UpdateStateUps(bool warning)
        {
            _screen.UpdateStateUPS(warning);
        }

        public void UpdatePreheatScreen()
        {
            LCDPreheatScreen.GetInstance().UpdateActTemp();
        }

        public void UpdateProgresBarCutGut()
        {
            if (Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Constants.TYPE_PRINTER.DeeRed2_1)
                LCDCatGutChangeSelector.GetInstance().UpdateProgressBar();
            else
                LCDCatGutChange.GetInstance().UpdateProgressBar();
        }

        public void UpdateProgresBarExtruding()
        {
            if (Program.Type == Constants.TYPE_PRINTER.DGRStolice3 || Program.Type == Constants.TYPE_PRINTER.DGRStolice5)
                LCDCatGutChangeTestExtruding.GetInstance().UpdateIndividualProgressBar();
            else
                LCDCatGutChangeTestExtruding.GetInstance().UpdateProgressBar();
        }

        public void UpdatePrintInfoScreen()
        {
            LCDPrintInfoScreen.GetInstance().UpdateAll();
        }
       
        public void UpdateModelScreen()
        {
            // pokud je nacten 
            LCDModelScreen.GetInstance().LoadPrintsFiles();
            LCDModelScreen.GetInstance().UpdateChooseModel();
        }

        #endregion

        public LCDScreen.StateScreen GetMainScreen()
        {
            if (Program.Type == Constants.TYPE_PRINTER.DREStolice || Program.Type == Constants.TYPE_PRINTER.DGRStolice3)
                return LCDScreen.StateScreen.MainStolice;
            else
                return LCDScreen.StateScreen.Main;
        }

        public void StartPrint()
        {
            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print && StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Init)
                return;

            StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Print;
            StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Init;

            Program.StartWatherPrint();

            StateHolder.GetInstance().SETValuePrint(1);
            StateHolder.GetInstance().SETValuePrintMax(100);

            //sekvence prikazu k tisku
            if (Program.PrintThreadBufferActive)
                return;

            _sdManager.SelectSdFileForPrint(StateHolder.GetInstance().PrintFile);
            _sdManager.StartResumeSdPrint();
        }

        public void StopPrint()
        {
            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Idle && StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Stop)
                return;

            StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Idle;
            StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Stop;

            // zobrazit cekaci smycka na pausu
            LCDWaitingScreen.GetInstance().UpdateScreen(false, Resources.GetString(Resources.StringResources.TextStopPrint), "");
            LCDManager.GetInstance().UpdateScreenLcd(LCD.LCDScreen.StateScreen.WaitingPause, "");
            //LCDWaitingScreen.GetInstance().StartIncrementProgress();
            LCDWaitingScreen.disableIncrement = false;

            if (Program.PrintThreadBufferActive)    // nutne vycistit buffer
            {
                Program.PrintThreadBufferActive = false;
                Program.GetBufferUartSenderPrinter1().Clear();
            }

            Program.StopWatcherPrint();

            //nutne vysunout posledni strunu do default polohy
            int lenght = 0;
            int speed = 800;
            
            if (Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Constants.TYPE_PRINTER.DeeRed2_1) // posouva relativne
                lenght = -60;
            
            Program.SendDataToPc(_deeContrManager.StsPrintStop());
            Program.HardwareResetPrinter1();

            LCDManager.GetInstance().MoveString(Program._actSelectExtrOnPowerboard, "" + lenght, "" + speed);
            freezScreen(lenght, speed);

            LCDWaitingScreen.disableIncrement = true;
            StateHolder.GetInstance().FilChangeCorrect = true;
        }

        private void freezScreen(int lenght, int speedRetract)
        {
            if (lenght == 0 || speedRetract == 0)
                return;

            int x = (int)(System.Math.Abs(lenght) / (speedRetract / 60)) * 1000 ; 

            Thread.Sleep(x);
        }

        public void DonePrint()
        {
            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print && StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Done)
                return;

            StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Print;
            StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Done;

            //TODO mozna neni potreba udelat - marlin pravdepodobne zasle konecnou hotnotu tisku
            StateHolder.GetInstance().SETValuePrint(StateHolder.GetInstance().GETValuePrintMax());

            Program.StopWatcherPrint();

            string lenght = "0";
            if (Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Constants.TYPE_PRINTER.DeeRed2_1) // posouva relativne
                lenght = "-60";
            LCDManager.GetInstance().MoveString(Program._actSelectExtrOnPowerboard, lenght, "800");

            LCDManager.GetInstance().GoToParkPosition();

            StateHolder.GetInstance().FilChangeCorrect = true;

            Program.SendDataToPc(_deeContrManager.StsPrintDone());
        }
  
        public void PausePrint()
        {
            //prekmit displeje
            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print && StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Pause)
                return;

            StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Print;
            StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Pause;


            Debug.Print("G120: Pausuji tisk. akt. extr:" + Program._actSelectExtrOnPowerboard);

            // zobrazit cekaci smycka na pausu
            LCDWaitingScreen.GetInstance().UpdateScreen(false, Resources.GetString(Resources.StringResources.TextPausePrint), "");
            LCDManager.GetInstance().UpdateScreenLcd(LCD.LCDScreen.StateScreen.WaitingPause, "");
            LCDWaitingScreen.disableIncrement = false;

            //1, pozastaveni tisku
            _sdManager.PauseSdPrint();
            
            Program._basicConfig.LatestPositionExtruder = new double[5] { 0.0, 0.0, 0.0, 0.0, 0.0 };
            UpsManager.correctReadPositonAxis = false;

            //2, zjistovani polohy
            Debug.Print("G120: Cekam na zastaveni");
            while (!UpsManager.correctReadPositonAxis)
            {
                Thread.Sleep(1000);
                Debug.Print("G120: nacitam polohu");
                _gCodeManager.GetCurrentPosition();
            }

            //5, odjeti na polohu ublinkavace nebo max pozici
            Debug.Print("G120: Jedu na polohu ublinkavani");
            _gCodeManager.GoToPositionChangeFilament();


            //3, vysunuti vlakna (kazda tiskarna jinak)
            Debug.Print("G120: Vysouvam vlakno akt. extr:" + Program._actSelectExtrOnPowerboard);

            string lenght = "0";
            if (Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Constants.TYPE_PRINTER.DeeRed2_1) // posouva relativne
                lenght = "-60";
            LCDManager.GetInstance().MoveString(Program._actSelectExtrOnPowerboard, lenght, "800");

            //4, vypnuti nahrivani
            Debug.Print("G120: Vypinam vyhrev akt. extr:" + Program._actSelectExtrOnPowerboard);

            Program._basicConfig.IntReserve1 = StateHolder.GetInstance().ActSetTempInPowerBordPrimExt;
            Program._basicConfig.IntReserve2 = StateHolder.GetInstance().ActSetTempInPowerBordSecundExt;
            LCDManager.GetInstance().StopHeatingExt(Program._actSelectExtrOnPowerboard);

            if (Program.UPS == Constants.UPS.OK)
            {
                UpsManager.SavePrintsParametrToMemory();
            }

            Program.SendDataToPc(_deeContrManager.StsPrintPause());
            LCDWaitingScreen.disableIncrement = true;
        }

        public void PreviousFromPause()
        {
            //prekmit displeje
            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print && StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Print)
                return;

            StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Print;
            StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Print;

            int setTempPrimEx = Program._basicConfig.IntReserve1;
            int setTempSecEx = Program._basicConfig.IntReserve2;
            double actTempPrimEx = StateHolder.GetInstance().ActTempPrimaryExt;
            int actTempSecEx = StateHolder.GetInstance().ActTempSecondaryExt;

            //1, opetovne nahrati trysky
            Debug.Print("G120: Zapinam vyhrev PRIM: " + setTempPrimEx + "SEC:" + setTempSecEx);
            LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderPrimary,setTempPrimEx);
            LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderSecondary,setTempSecEx);

            // zobrazit cekaci smycka na pausu
            LCDWaitingScreen.GetInstance().UpdateScreen(false, Resources.GetString(Resources.StringResources.scrCutPreparePrintHeating), "");
            LCDManager.GetInstance().UpdateScreenLcd(LCD.LCDScreen.StateScreen.WaitingPause, "");
            //LCDWaitingScreen.GetInstance().StartIncrementProgress();
            LCDWaitingScreen.disableIncrement = false;

            while ((actTempPrimEx < setTempPrimEx))
            {
                Thread.Sleep(500);
                actTempPrimEx = StateHolder.GetInstance().ActTempPrimaryExt;
            }

            while ((actTempSecEx < setTempSecEx))
            {
                Thread.Sleep(500);
                //actTempPrimEx = StateHolder.GetInstance().ActTempPrimaryExt;
                actTempPrimEx = StateHolder.GetInstance().ActTempSecondaryExt;
            }


            //nastavit spravny nastroj T0 T1
            if (Program._basicConfig.LatestPositionExtruder[3] != 0.0)
                Program._actSelectExtrOnPowerboard = Constants.EXTRUDER.ExtruderPrimary;

            if (Program._basicConfig.LatestPositionExtruder[4] != 0.0)
                Program._actSelectExtrOnPowerboard = Constants.EXTRUDER.ExtruderSecondary;

            if (Program._basicConfig.LatestPositionExtruder[5] != 0.0)
                Program._actSelectExtrOnPowerboard = Constants.EXTRUDER.ExtruderThird;

            if (Program._basicConfig.LatestPositionExtruder[6] != 0.0)
                Program._actSelectExtrOnPowerboard = Constants.EXTRUDER.ExtruderFourth;

            if (Program._basicConfig.LatestPositionExtruder[7] != 0.0)
                Program._actSelectExtrOnPowerboard = Constants.EXTRUDER.ExtruderFifth;


            //2, nasunuti vlakna (kazda tiskarna jinak)
            Debug.Print("G120: Nasouvam vlakno akt. extr:" + Program._actSelectExtrOnPowerboard);
            
            string lenght = "0";
            if (Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Constants.TYPE_PRINTER.DeeRed2_1) // posouva relativne
                lenght = "60";
            LCDManager.GetInstance().MoveString(Program._actSelectExtrOnPowerboard, lenght, "200");

            //3, presun na pùvodni polohu
            //F4000
            Debug.Print("G120: Jedu na posledni ulozenou pozici");
            _gCodeManager.GoToLastPositionPrint(Program._actSelectExtrOnPowerboard);
   
            //F2400
            _gCodeManager.MoveSetSpeed40mm();
           
            //4, spuštìni tisku z PW
            Debug.Print("G120: Start tisku");
            _sdManager.StartResumeSdPrint();

            //5, návrat na print info screen
            LCDManager.GetInstance().UpdateScreenLcd(LCD.LCDScreen.StateScreen.PrintInfo, "");
            UpdatePrintInfoScreen();

            Program.SendDataToPc(_deeContrManager.StsPrintStart());

        }

        public bool IsCorrectManufaturAndTypPrinter()
        {
            if (StateHolder.GetInstance().GetListParametersFile() == null || StateHolder.GetInstance().GetListParametersFile().Count == 0)
                return false;

            string manufac = ConfigurationPrinter.GetInstance().GetNameOfManufacter();
            string manufac2 = ConfigurationPrinter.GetInstance().GetNameOfManufacter2();
            string typ = ConfigurationPrinter.GetInstance().GetNameOfTypePrinter();
            string param = (string)StateHolder.GetInstance().GetListParametersFile()[0];

            param = param.Trim('\r');
            param = param.Trim('\n');

            if (Program.Type == Constants.TYPE_PRINTER.DeeGreenSer1Ser2)
            {
                return (param == (manufac + "@" + typ));
            }

            // pridava moznost spustit gcode z II serie na tiskarne III serie
            if (Program.Type == Constants.TYPE_PRINTER.DeeGreenSer3)
            {
                //vyrobce doit
                if (param.IndexOf(manufac) > -1)
                {
                    return (param.IndexOf(manufac + "@" + typ) > -1) || (param.IndexOf(manufac + "@" + "green") > -1);
                }

                //vyrobce be3D
                if (param.IndexOf(manufac2) > -1)
                {
                    return (param.IndexOf(manufac2 + "@" + typ) > -1) || (param.IndexOf(manufac2 + "@" + "green") > -1);
                }
            }

            return param.IndexOf(manufac + "@" + typ) > -1;
        }

        // get A, B, AB
        public string GetPrintingExtruder()
        {
            try
            {
                string[] paramA = ((string)StateHolder.GetInstance().GetListParametersFile()[1]).Split('@');
                string[] paramB = ((string)StateHolder.GetInstance().GetListParametersFile()[2]).Split('@');

                string matA = paramA[0];
                string lenghtA = paramA[1].Trim('\r');
                string matB = paramB[0];
                string lenghtB = paramB[1].Trim('\r');

                //chyba nebo nic se nicim netiskne
                if (matA == "" || matB == "" || lenghtA == "" || lenghtB == "")
                    return "";

                //chyba nebo nic se nicim netiskne
                if ((matA == "null" && matB == "null") || (lenghtA == "0" && lenghtB == "0"))
                    return "";

                // tiskne A
                if ((matA != "null" && lenghtA != "0") && (matB == "null" || lenghtB == "null" || lenghtB == "0"))
                    return "A";

                // tiskne B
                if ((matB != "null" && lenghtB != "0") && (matA == "null" || lenghtA == "null" || lenghtA == "0"))
                    return "B";

                // tiskne AB
                if ((matA != "null" && lenghtA != "0") && (matB != "null" && lenghtB != "0"))
                    return "AB";

                return "";
            }
            catch (Exception)
            {
                return "";
            }

        }

        #region HEATING

        public void StartPreHeating()
        {
            if (StateHolder.GetInstance().StateHeating == true)
                return;

            StateHolder.GetInstance().StateHeating = true;
            StartHeatingExt(Constants.EXTRUDER.ExtruderPrimary, StateHolder.GetInstance().ActSetTempPrimary);

            if (ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder())
                StartHeatingExt(Constants.EXTRUDER.ExtruderSecondary, StateHolder.GetInstance().ActSetTempSecondary);

            if (ConfigurationPrinter.GetInstance().IsPresentBedHeat())
                StartHeatingBed();
            
            if (ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
                StartHeatingSpace();

            Program.SendDataToPc(_deeContrManager.StsPreheatStart());

        }

        public void StopPreheating()
        {
            if (StateHolder.GetInstance().StateHeating == false)
                return;

            StateHolder.GetInstance().StateHeating = false;
            StopHeatingExt(Constants.EXTRUDER.ExtruderPrimary);

            if (ConfigurationPrinter.GetInstance().IsPresentBedHeat())
                StopHeatingBed();
            if (ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
                StopHeatingSpace();
            if (ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder())
                StopHeatingExt(Constants.EXTRUDER.ExtruderSecondary);

            Program.SendDataToPc(_deeContrManager.StsPreheatStop());

        }

        private void StartHeatingBed()
        {
            // ochrana proti zapnuti vyhrevu u DGR - kdyz neni podlozka nebo je n2jak8 chyba
            if (StateHolder.GetInstance().ActTempBed > 10 && StateHolder.GetInstance().ActTempBed < 150)
            {
                _gCodeManager.SetBedTemp(StateHolder.GetInstance().ActSetTempBed);
            }
        }

        private void StopHeatingBed()
        {
            _gCodeManager.SetBedTemp(0);
        }

        public void StartHeatingExt(Constants.EXTRUDER ext, int temp)
        {
            StateHolder.GetInstance().StateHeating = true;

            if (ext == Constants.EXTRUDER.ExtruderPrimary)
            {
                StateHolder.GetInstance().ActSetTempPrimary = temp;
                _gCodeManager.SetExtruderTemp(ext, StateHolder.GetInstance().ActSetTempPrimary);
            }
            if (ext == Constants.EXTRUDER.ExtruderSecondary)
            {
                StateHolder.GetInstance().ActSetTempSecondary = temp;
                _gCodeManager.SetExtruderTemp(ext, StateHolder.GetInstance().ActSetTempSecondary);
            }
            if (ext == Constants.EXTRUDER.ExtruderThird)
            {
                StateHolder.GetInstance().ActSetTempFifth = temp;
                _gCodeManager.SetExtruderTemp(ext, StateHolder.GetInstance().ActSetTempFifth);
            }

        }

        public void StopHeatingExt(Constants.EXTRUDER ext)
        {
            //vypni topeni pouze v pripade, ze je stav IDLE, duvod pausa pri tisku a vymene struny

            StateHolder.GetInstance().StateHeating = false;

            if (ext == Constants.EXTRUDER.ExtruderPrimary)
                StateHolder.GetInstance().ActSetTempPrimary = Program._basicConfig.DefTmpRefExtr0;

            if (ext == Constants.EXTRUDER.ExtruderSecondary)
                StateHolder.GetInstance().ActSetTempSecondary = Program._basicConfig.DefTmpRefExtr1;

            _gCodeManager.SetExtruderTemp(ext, 0);

        }

        private void StartHeatingSpace()
        {
            //TODO zapnout topeni prostoru pomoci ControlBoard
        }

        private void StopHeatingSpace()
        {
            //TODO vypnout topeni prostoru pomoci ControlBoard
        }

        #endregion

        #region MOVE

        public void EmergencyStop()
        {
            _gCodeManager.MoveEmergencyStop();
        }

        public void MoveStop()
        {
            _gCodeManager.MoveStop();
        }

        public void GoToPositionX0Y0()
        {
            _gCodeManager.MoveToX0Y0();
        }

        public void GoToPositionX0Y0Z20()
        {
            _gCodeManager.MoveToX0Y0Z20();
        }

        public void GoToParkPosition()
        {
            _gCodeManager.MoveParkPosition();
        }

        public void GoToHomePosition()
        {
            _gCodeManager.MoveHomePosition();
        }

        public void GoHome()
        {
            _gCodeManager.MoveToHome();
        }

        public void GoToMaxPosition()
        {
            _gCodeManager.MoveToMaxPosition();
        }

        public void MoveString(Constants.EXTRUDER ext, string mm, string speed)
        {
            _gCodeManager.MotorEnable();
            _gCodeManager.MoveSpring(ext,mm,speed);
        }

        public void MoveSpeedString(Constants.EXTRUDER ext, string mm)
        {
            _gCodeManager.MotorEnable();
            _gCodeManager.MoveSpeedSpringUp(ext, mm);
        }

        #endregion

    }
}
