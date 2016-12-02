
using System;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using _3D_control_v09;
using Be3D.Constants;
using System.Threading;

namespace _3D_control_v09
{
    class LCDWaitingScreen : LCDScreen
    {
        private static LCDWaitingScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinWaitingScreen));
        
        private TextBlock text_info_main = (TextBlock)window.GetChildByName("text_info");
        private TextBlock text_progress = (TextBlock)window.GetChildByName("text_progress");
        private Button btn_cancel = (Button)window.GetChildByName("btn_cancel");

        public static bool disableIncrement = false;

        private LCDWaitingScreen()
        {
            setWindow(window); 
            InitScreen();        
        }

        public static LCDWaitingScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDWaitingScreen();

            return _instance;

        }

        private void InitScreen()
        {
            text_info_main.Font = StateHolder.FontUbuntuMiddle;
            text_progress.Font = StateHolder.FontUbuntuBig;
            btn_cancel.Font = StateHolder.FontUbuntuMiddle;

            btn_cancel.Text = Resources.GetString(Resources.StringResources.TextCancel);

            btn_cancel.TapEvent += btn_cancel_TapEvent;

        }

        private void btn_cancel_TapEvent(object sender)
        {
            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.MotionTest)
                Program.HardwareResetPrinter1();
                
            else
                LCDManager.GetInstance().MoveStop();

            disableIncrement = true;
            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
        }

        public void UpdateScreen(bool visibleBtn, string textInfo, string textProgres)
        {
            if (visibleBtn)
                btn_cancel.Visible = true;
            else
                btn_cancel.Visible = false;

            text_info_main.Text = textInfo;
            text_progress.Text = textProgres;

            window.FillRect(btn_cancel.Rect);
            btn_cancel.Invalidate();

            window.FillRect(text_info_main.Rect);
            text_info_main.Invalidate();

            window.FillRect(text_progress.Rect);
            text_progress.Invalidate();

        }

        /*
        private void StartIncrementProgress() 
        {
            disableIncrement = false;
            new Thread(incrementProgress).Start();

        }
        */

        private void incrementProgress()
        {
            while (!disableIncrement)
            {
                if (text_progress.Text == "" || text_progress.Text == ".....................")
                    text_progress.Text = ".";

                window.FillRect(text_progress.Rect);
                text_progress.Invalidate();

                text_progress.Text = text_progress.Text + ".";

                Thread.Sleep(600);
            }
        }


        public void IncrementProgress()
        {
            if (disableIncrement == true)
                return;

            if (text_progress.Text == "" || text_progress.Text == ".....................")
                text_progress.Text = ".";

            window.FillRect(text_progress.Rect);
            text_progress.Invalidate();

            text_progress.Text = text_progress.Text + ".";

            Thread.Sleep(600);
        }

    }
}
