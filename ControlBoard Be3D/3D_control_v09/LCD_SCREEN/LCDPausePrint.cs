using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using _3D_control_v09;

namespace _3D_control_v09
{
    class LCDPausePrint : LCDScreen
    {
        private static LCDPausePrint _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinPausePrint));
        
        private Button btn_continue;
        private Button btn_stop = (Button)window.GetChildByName("btn_stop");
        private Button btn_changeString = (Button)window.GetChildByName("btn_changeString");
        private TextBlock text_info = (TextBlock)window.GetChildByName("text_info");
        private TextBlock text_main = (TextBlock)window.GetChildByName("text_main");

       // private string mess = "";

        private LCDPausePrint()
        {
            setWindow(window);
            InitScreen();
        }

        public static LCDPausePrint GetInstance()
        {
            if (_instance == null)
                _instance = new LCDPausePrint();

            return _instance;

        }

        private void InitScreen()
        {

            //***************   Pause catgut window   ***********************

            btn_continue = (Button) window.GetChildByName("btn_continue");
            btn_continue.TapEvent += new OnTap(btn_continue_TapEvent);
            btn_continue.Font = StateHolder.FontUbuntuSmall;

            
            btn_stop.TapEvent += new OnTap(btn_stop_TapEvent);
            btn_stop.Font = StateHolder.FontUbuntuSmall;

            
            btn_changeString.TapEvent += new OnTap(btn_changeString_TapEvent);
            btn_changeString.Font = StateHolder.FontUbuntuSmall;

            text_info.Font = StateHolder.FontUbuntuSmall;
          
            text_main.Font = StateHolder.FontUbuntuBig;

            btn_continue.Text = Resources.GetString(Resources.StringResources.TextContinue);
            btn_stop.Text = Resources.GetString(Resources.StringResources.scrPauseBtStop);
            btn_changeString.Text = Resources.GetString(Resources.StringResources.scrSettingBtChange);

            text_main.Text = Resources.GetString(Resources.StringResources.scrPauseTxMain);

            addUpsBox();

        }

        public void UpdateMessage(string _mess)
        {
            if (_mess != "")
                text_info.Text = _mess;
            else
                text_info.Text = Resources.GetString(Resources.StringResources.scrPauseTxInfo);

            window.FillRect(text_info.Rect);
            text_info.Invalidate();
        }


        private void btn_changeString_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.CatGutChange, "");
        }

        private void btn_stop_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Stop, "");
        }

        private void btn_continue_TapEvent(object sender)
        {
            string mainMess = "";
            string message = "";

            // jsou uzavrene dvere1 nebo 2
            if (SwitchManager.GetInstance().IsOpenDoor1() || SwitchManager.GetInstance().IsOpenDoor2())
            {
                message = Resources.GetString(Resources.StringResources.TextOpenDoorOrCover);
                LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.PausePrint, message, mainMess, true);
                return;
            }

            //nekorektne udelana vymena struny
            if (StateHolder.GetInstance().FilChangeCorrect == false)
            {
                message = Resources.GetString(Resources.StringResources.scrCutIncorectChangeFil);
                LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.PausePrint, message, mainMess, true);
                return;
            }

            #region UPS
            // je instalovana UPS 
            if (Program.UPS == Be3D.Constants.Constants.UPS.OK)
            {
                // jedeme na baterie
                if (Ups.actState == Ups.STATE.LowBatt || Ups.actState == Ups.STATE.TrafficBatt)
                {
                    message = Resources.GetString(Resources.StringResources.TextStateUpsTraffic);
                    LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.PausePrint, message, mainMess, true);
                    return;
                }

                // nacti parametry pokud byl pozastaven tisk pomoci UPS
                if (Program._basicConfig.GetStrReserve1() != "" && Program._basicConfig.GetStrReserve1() != "null")
                {
                    UpsManager.LoadPrintsParametrFromMemory();
                }
            }

            #endregion

            // get A, B, AB, kterym extruderem se bude tisknout
            string printingExt = "A";

            if (ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder())
            {
                printingExt = LCDManager.GetInstance().GetPrintingExtruder();
            }

            #region print A
            if (printingExt == "A")
            {
                if (SwitchManager.GetInstance().IsFilamentExt1() == false)
                {
                    message = Resources.GetString(Resources.StringResources.TextNoMaterialExtr1);
                    LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.PausePrint, message, mainMess, true);
                    return;
                }
            }
            #endregion

            #region print B
            if (printingExt == "B")
            {
                if (SwitchManager.GetInstance().IsFilamentExt2() == false)
                {
                    message = Resources.GetString(Resources.StringResources.TextNoMaterialExtr2); 
                    LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.PausePrint, message, mainMess, true);
                    return;
                }
            }
            #endregion

            #region print AB
            if (printingExt == "AB")
            {
                if (SwitchManager.GetInstance().IsFilamentExt1() == false)
                {
                    message = Resources.GetString(Resources.StringResources.TextNoMaterialExtr1);
                    LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.PausePrint, message, mainMess, true);
                    return;
                }

                if (SwitchManager.GetInstance().IsFilamentExt2() == false)
                {
                    message = Resources.GetString(Resources.StringResources.TextNoMaterialExtr2);
                    LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.PausePrint, message, mainMess, true);
                    return;
                }
            }
            #endregion

            
            LCDManager.GetInstance().PreviousFromPause();
        }
    }
}
