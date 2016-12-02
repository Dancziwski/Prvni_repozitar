using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;


namespace _3D_control_v09
{
    class LCDMainScreenStolice : LCDScreen
    {

        private static LCDMainScreenStolice _instance;

        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinMain));

        private static Button btn_main_print = (Button)window.GetChildByName("btn_print");
        private static Button btn_main_settings = (Button)window.GetChildByName("btn_settings");
        private static Button btn_main_tests = (Button)window.GetChildByName("btn_information");
        
        private static Button btn_main_shutdown = (Button)window.GetChildByName("btn_shutdown");

        private static TextBlock text_main_main = (TextBlock)window.GetChildByName("text_main");

        private LCDMainScreenStolice()
        {
            setWindow(window);             
            InitScreen();
        }

        public static LCDMainScreenStolice GetInstance()
        {
            if (_instance == null)
                _instance = new LCDMainScreenStolice();

            return _instance;
        }

        private void InitScreen()
        {
            //****************   Main window    ****************************
            
            btn_main_tests.TapEvent += new OnTap(btn_tests_TapEvent);
            btn_main_tests.Font = StateHolder.FontUbuntuMiddle;
            
            btn_main_settings.TapEvent += new OnTap(btn_settings_TapEvent);
            btn_main_settings.Font = StateHolder.FontUbuntuMiddle;
            
            btn_main_print.TapEvent += new OnTap(btn_main_print_TapEvent);
            btn_main_print.Font = StateHolder.FontUbuntuMiddle;
            
            btn_main_shutdown.TapEvent += new OnTap(btn_main_shutdown_TapEvent);
            btn_main_shutdown.Font = StateHolder.FontUbuntuMiddle;
            
            text_main_main.Font = StateHolder.FontUbuntuBig;


            btn_main_tests.Text = "Tests";
            btn_main_print.Text = Resources.GetString(Resources.StringResources.scrMainBtPrint);
            btn_main_settings.Text = Resources.GetString(Resources.StringResources.scrMainBtSetting);
            btn_main_shutdown.Text = Resources.GetString(Resources.StringResources.scrMaintBtShutDown);

            btn_main_shutdown.Visible = false;

            text_main_main.Text = Resources.GetString(Resources.StringResources.scrMainTxMain);
   
            addUpsBox();
            window.Render();
      
        }

        private void btn_main_print_TapEvent(object sender)
        {

            StateHolder.GetInstance().FilChangeCorrect = true;
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Model, "");
        }

        private void btn_settings_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Settings, "");
        }

        private void btn_tests_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Tests, "");
        }

        private void btn_main_shutdown_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.ShutDown, "");
        }
    }
}
