
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using _3D_control_v09;

namespace _3D_control_v09
{
    class LCDShutdownScreen : LCDScreen
    {
        private static LCDShutdownScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinShutDown));

        private Button btn_shutdown_back = (Button)window.GetChildByName("btn_back");
        private TextBlock text_shutdown_1 = (TextBlock)window.GetChildByName("text_info1");
        private TextBlock text_shutdown_2 = (TextBlock)window.GetChildByName("text_info2");
        private TextBlock text_shutdown_3 = (TextBlock)window.GetChildByName("text_info3");
        private TextBlock text_shutdown_4 = (TextBlock)window.GetChildByName("text_info4");
        private TextBlock text_shutdown_5 = (TextBlock)window.GetChildByName("text_info5");
        private TextBlock text_shutdown_main = (TextBlock)window.GetChildByName("text_main");


        private LCDShutdownScreen()
        {
            setWindow(window);
            InitScreen();
        }

        public static LCDShutdownScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDShutdownScreen();

            return _instance;

        }

        private void InitScreen()
        {
            //***************   Shutdown window    **************************
            
            btn_shutdown_back.TapEvent += new OnTap(btn_shutdown_back_TapEvent);
            btn_shutdown_back.Font = StateHolder.FontUbuntuMiddle;
            
            text_shutdown_1.Font = StateHolder.FontUbuntuSmall;
            
            text_shutdown_2.Font = StateHolder.FontUbuntuSmall;
            
            text_shutdown_3.Font = StateHolder.FontUbuntuSmall;
            
            text_shutdown_4.Font = StateHolder.FontUbuntuSmall;
            
            text_shutdown_5.Font = StateHolder.FontUbuntuSmall;
            
            text_shutdown_main.Font = StateHolder.FontUbuntuBig;


            btn_shutdown_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            text_shutdown_1.Text = Resources.GetString(Resources.StringResources.scrShutDownTx1);
            text_shutdown_2.Text = Resources.GetString(Resources.StringResources.scrShutDownTx2);
            text_shutdown_3.Text = Resources.GetString(Resources.StringResources.scrShutDownTx3);
            text_shutdown_4.Text = Resources.GetString(Resources.StringResources.scrShutDownTx4);
            text_shutdown_5.Text = Resources.GetString(Resources.StringResources.scrShutDownTx5);
            text_shutdown_main.Text = Resources.GetString(Resources.StringResources.scrShutDownTxMain);

            addUpsBox();

            window.Render();

        }

        private void btn_shutdown_back_TapEvent(object sender)
        {
            
            if(Program.UPS == Be3D.Constants.Constants.UPS.OK)
                LCDManager.GetInstance().UpsUp();

            Program.HardwareResetPrinter1();
            Program.HardwareResetPrinter2();

            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
        }


      
    }
}
