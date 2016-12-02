using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using System.Threading;

namespace _3D_control_v09
{
    class LCDSettingsScreen : LCDScreen
    {
        private static LCDSettingsScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinSettings));

        private Button btn_settings_back = (Button)window.GetChildByName("btn_back");
        private Button btn_settings_park = (Button)window.GetChildByName("btn_park");
        private Button btn_settings_another = (Button)window.GetChildByName("btn_another");
        private Button btn_settings_change = (Button)window.GetChildByName("btn_change");
        private TextBlock text_settings_main = (TextBlock)window.GetChildByName("text_main");

        private LCDSettingsScreen()
        {
           setWindow(window);        
           InitScreen();
        }

        public static LCDSettingsScreen GetInstance()
        {
            if(_instance == null)
                _instance = new LCDSettingsScreen();

            return _instance;
        }

        private void InitScreen()
        {
            //***************    Settings window   **************************
            
            btn_settings_back.TapEvent += new OnTap(btn_settings_back_TapEvent);
            btn_settings_back.Font = StateHolder.FontUbuntuMiddle;
           
            btn_settings_park.TapEvent += new OnTap(btn_parkPosition_TapEvent);
            btn_settings_park.Font = StateHolder.FontUbuntuMiddle;
            
            btn_settings_another.TapEvent += new OnTap(btn_settings_another_TapEvent);
            btn_settings_another.Font = StateHolder.FontUbuntuMiddle;
            
            btn_settings_change.TapEvent += new OnTap(btn_settings_change_TapEvent);
            btn_settings_change.Font = StateHolder.FontUbuntuMiddle;
            
            text_settings_main.Font = StateHolder.FontUbuntuBig;

            btn_settings_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            btn_settings_another.Text = Resources.GetString(Resources.StringResources.scrSettingBtAnother);
            btn_settings_change.Text = Resources.GetString(Resources.StringResources.scrSettingBtChange);
            btn_settings_park.Text = Resources.GetString(Resources.StringResources.scrSettingBtPark);
            text_settings_main.Text = Resources.GetString(Resources.StringResources.scrSettingTxMain);

            addUpsBox();

            window.Render();

        }

        private void btn_settings_back_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
        }

        private void btn_parkPosition_TapEvent(object sender)
        {
            if (!(SwitchManager.GetInstance().IsOpenDoor1() || SwitchManager.GetInstance().IsOpenDoor2()))
            {
                StateHolder.GetInstance().ActState = Be3D.Constants.Constants.ACTUAL_STATE.ParkingPosition;
                LCDManager.GetInstance().GoToParkPosition();
            }
            else
            {
                LCDManager.GetInstance().ShowErrorScreenLcd(StateScreen.Settings, Resources.GetString(Resources.StringResources.TextDoorOpen), "",true);
            }
        }

        private void btn_settings_another_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.SettingAnother, "");
        }

        private void btn_settings_change_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.CatGutChange, "");          
        }


       
    }
}
