using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using System.Threading;

namespace _3D_control_v09
{
    class LCDTestScreen : LCDScreen
    {
        private static LCDTestScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinSettings));

        private Button btn_settings_back = (Button)window.GetChildByName("btn_back");

        private Button btn_tests_testservo = (Button)window.GetChildByName("btn_park");
        private Button btn_tests_extruding = (Button)window.GetChildByName("btn_change");

        private Button btn_settings_another = (Button)window.GetChildByName("btn_another");
        private TextBlock text_settings_main = (TextBlock)window.GetChildByName("text_main");

        private LCDTestScreen()
        {
           setWindow(window);        
           InitScreen();
        }

        public static LCDTestScreen GetInstance()
        {
            if(_instance == null)
                _instance = new LCDTestScreen();

            return _instance;
        }

        private void InitScreen()
        {
            //***************    Settings window   **************************
            
            btn_settings_back.TapEvent += new OnTap(btn_settings_back_TapEvent);
            btn_settings_back.Font = StateHolder.FontUbuntuMiddle;
           
            btn_tests_testservo.TapEvent += new OnTap(btn_tests_testservo_TapEvent);
            btn_tests_testservo.Font = StateHolder.FontUbuntuMiddle;
        
            btn_tests_extruding.TapEvent += new OnTap(btn_test_extruding_TapEvent);
            btn_tests_extruding.Font = StateHolder.FontUbuntuMiddle;

            btn_settings_another.TapEvent += new OnTap(btn_settings_another_TapEvent);
            btn_settings_another.Font = StateHolder.FontUbuntuMiddle;

           
        
            text_settings_main.Font = StateHolder.FontUbuntuBig;

            btn_settings_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            btn_settings_another.Text = Resources.GetString(Resources.StringResources.scrSettingBtAnother);
            btn_tests_extruding.Text = "Extruding";
            btn_tests_testservo.Text = "Test servo";

            text_settings_main.Text = Resources.GetString(Resources.StringResources.scrSettingTxMain);

            btn_settings_another.Visible = false;
            addUpsBox();
            window.Render();

        }

        private void btn_settings_back_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
        }

        private void btn_tests_testservo_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.TestServo, "");
        }

        private void btn_test_extruding_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.CatGutChangeTestExtruding, "");         
        }

        private void btn_settings_another_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.SettingAnother, "");
        }
       
    }
}
