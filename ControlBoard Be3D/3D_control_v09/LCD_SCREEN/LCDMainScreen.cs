using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;


namespace _3D_control_v09
{
    class LCDMainScreen : LCDScreen
    {

        private static LCDMainScreen _instance;

        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinMain));

        private static Button btn_main_info = (Button)window.GetChildByName("btn_information");
        private static Button btn_main_settings = (Button)window.GetChildByName("btn_settings");
        private static Button btn_main_print = (Button)window.GetChildByName("btn_print");
        private static Button btn_main_shutdown = (Button)window.GetChildByName("btn_shutdown");
        private static TextBlock text_main_main = (TextBlock)window.GetChildByName("text_main");

        private LCDMainScreen()
        {
            setWindow(window);             
            InitScreen();
        }

        public static LCDMainScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDMainScreen();

            return _instance;
        }

        private void InitScreen()
        {
            //****************   Main window    ****************************
            
            btn_main_info.TapEvent += new OnTap(btn_info_TapEvent);
            btn_main_info.Font = StateHolder.FontUbuntuMiddle;
            
            btn_main_settings.TapEvent += new OnTap(btn_settings_TapEvent);
            btn_main_settings.Font = StateHolder.FontUbuntuMiddle;
            
            btn_main_print.TapEvent += new OnTap(btn_main_print_TapEvent);
            btn_main_print.Font = StateHolder.FontUbuntuMiddle;
            
            btn_main_shutdown.TapEvent += new OnTap(btn_main_shutdown_TapEvent);
            btn_main_shutdown.Font = StateHolder.FontUbuntuMiddle;
            
            text_main_main.Font = StateHolder.FontUbuntuBig;


            btn_main_info.Text = Resources.GetString(Resources.StringResources.scrMainBtInfo);
            btn_main_print.Text = Resources.GetString(Resources.StringResources.scrMainBtPrint);
            btn_main_settings.Text = Resources.GetString(Resources.StringResources.scrMainBtSetting);
            btn_main_shutdown.Text = Resources.GetString(Resources.StringResources.scrMaintBtShutDown);
            text_main_main.Text = Resources.GetString(Resources.StringResources.scrMainTxMain);
   
            addUpsBox();
            window.Render();
      
        }

        private void btn_info_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Info, "");
        }

        private void btn_settings_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Settings, "");
        }

        private void btn_main_print_TapEvent(object sender)
        {
            if (StateHolder.GetInstance().FileDataTransfer)
                LCDManager.GetInstance().ShowErrorScreenLcd(LCDManager.GetInstance().GetMainScreen(), Resources.GetString(Resources.StringResources.TextDataTransferText), "",false);

            if ((Program.Type == Be3D.Constants.Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Be3D.Constants.Constants.TYPE_PRINTER.DeeRed2_1) && StateHolder.GetInstance().FilChangeCorrect == false)
            {
                LCDManager.GetInstance().UpdateScreenLcd(StateScreen.CatGutChange, "");
                return;
            }

            StateHolder.GetInstance().FilChangeCorrect = true;
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Model, "");

        }

        private void btn_main_shutdown_TapEvent(object sender)
        {
            if (Program.UPS == Be3D.Constants.Constants.UPS.OK)
            {
                LCDManager.GetInstance().UpsDown();
            }
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.ShutDown, "");
        }
    }
}
