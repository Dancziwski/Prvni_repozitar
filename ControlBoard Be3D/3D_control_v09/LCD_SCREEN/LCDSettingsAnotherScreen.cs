
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using _3D_control_v09;

namespace _3D_control_v09
{
    class LCDSettingsAnotherScreen : LCDScreen
    {
        private static LCDSettingsAnotherScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinSettingsAnother));

        private Button btn_another_back = (Button)window.GetChildByName("btn_back");
        private Button btn_another_temperature = (Button)window.GetChildByName("btn_temperature");
        private Button btn_another_language = (Button)window.GetChildByName("btn_language");
        private Button btn_setting_offset = (Button)window.GetChildByName("btn_Offset");
        private Button btn_testAxis = (Button)window.GetChildByName("btn_testaxis");
        private TextBlock text_another_main = (TextBlock)window.GetChildByName("text_main");


        private LCDSettingsAnotherScreen()
        {
            setWindow(window);
            InitScreen();
        }

        public static LCDSettingsAnotherScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDSettingsAnotherScreen();

            return _instance;
        }

        private void InitScreen()
        {
            //***************    Another Setting window   *******************
            
            btn_another_back.TapEvent += new OnTap(btn_another_back_TapEvent);
            btn_another_back.Font = StateHolder.FontUbuntuMiddle;
          
            btn_another_temperature.TapEvent += new OnTap(btn_another_temperature_TapEvent);
            btn_another_temperature.Font = StateHolder.FontUbuntuMiddle;
            
            btn_another_language.TapEvent += new OnTap(btn_another_language_TapEvent);
            //btn_another_language.Enabled = false;
            btn_another_language.Font = StateHolder.FontUbuntuMiddle;

            btn_setting_offset.TapEvent += new OnTap(btn_setting_offset_TapEvent);
            //btn_another_language.Enabled = false;
            btn_setting_offset.Font = StateHolder.FontUbuntuMiddle;
         
            btn_testAxis.TapEvent += new OnTap(btn_motionTest_TapEvent);
            btn_testAxis.Font = StateHolder.FontUbuntuMiddle;

            
            text_another_main.Font = StateHolder.FontUbuntuBig;


            btn_another_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            btn_another_language.Text = Resources.GetString(Resources.StringResources.scrSettingAnotherBtLang);
            btn_another_temperature.Text = Resources.GetString(Resources.StringResources.scrSettingAnotherBtTemper);
            btn_setting_offset.Text = Resources.GetString(Resources.StringResources.scrSettingAnotherBtOffset);
            btn_testAxis.Text = Resources.GetString(Resources.StringResources.scrSettingAnotherBtTestAxis);

            addUpsBox();

            text_another_main.Text = Resources.GetString(Resources.StringResources.scrSettingAnotherTxMain);

            if (!ConfigurationPrinter.GetInstance().isOffsetMenu())
                btn_setting_offset.Visible = false;

            window.Render();

        }

        private void btn_another_back_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Settings, "");
        }

      
        private void btn_another_temperature_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.SettingTemp, "");
        }

        private void btn_another_language_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Language, "");

        }

        private void btn_motionTest_TapEvent(object sender)
        {
            if (SwitchManager.GetInstance().IsOpenDoor1() || SwitchManager.GetInstance().IsOpenDoor2())
            {
                LCDManager.GetInstance().ShowErrorScreenLcd(StateScreen.Settings, Resources.GetString(Resources.StringResources.TextDoorOpen), "", true);
                return;
            }

            StateHolder.GetInstance().ActState = Be3D.Constants.Constants.ACTUAL_STATE.MotionTest;

            // zobrazit cekaci smycka na pausu
            LCDWaitingScreen.GetInstance().UpdateScreen(true, Resources.GetString(Resources.StringResources.TextMotionTest), "");
            LCDManager.GetInstance().UpdateScreenLcd(LCD.LCDScreen.StateScreen.WaitingPause, "");
            //LCDWaitingScreen.GetInstance().StartIncrementProgress();
            LCDWaitingScreen.disableIncrement = false;

            if (ConfigurationPrinter.GetInstance().IsPresentAutoBedLeveling())
            {
                LCDManager.GetInstance().GoHome();

                LCDManager.GetInstance().GoToPositionX0Y0();
                LCDManager.GetInstance().GoToMaxPosition();
                LCDManager.GetInstance().GoToPositionX0Y0Z20();
                LCDManager.GetInstance().GoToParkPosition();

            }
            else
            {
                LCDManager.GetInstance().UpdateScreenLcd(StateScreen.CalibrationPark, "");
            }
        }

        private void btn_setting_offset_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.SettingOffset, "");

        }

        
    }
}
