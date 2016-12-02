
using Be3D.Constants;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using System.Threading;

namespace _3D_control_v09
{
    class LCDStopScreen : LCDScreen
    {
        private Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinStop));
    
        public LCDStopScreen()
        {
            setWindow(window);
            InitScreen();
        }

        private void InitScreen()
        {
            //***************   Stop model   *********************************
            Button btn_stop_yes = (Button) window.GetChildByName("btn_yes");
            btn_stop_yes.TapEvent += new OnTap(btn_stop_yes_TapEvent);
            btn_stop_yes.Font = StateHolder.FontUbuntuMiddle;
            Button btn_stop_no = (Button) window.GetChildByName("btn_no");
            btn_stop_no.TapEvent += new OnTap(btn_stop_no_TapEvent);
            btn_stop_no.Font = StateHolder.FontUbuntuMiddle;
            TextBlock text_stop_text1 = (TextBlock) window.GetChildByName("text_info1");
            text_stop_text1.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_stop_text2 = (TextBlock) window.GetChildByName("text_info2");
            text_stop_text2.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_stop_text3 = (TextBlock) window.GetChildByName("text_info3");
            text_stop_text3.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_stop_text4 = (TextBlock) window.GetChildByName("text_info4");
            text_stop_text4.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_stop_main = (TextBlock) window.GetChildByName("text_main");
            text_stop_main.Font = StateHolder.FontUbuntuSmall;

            addUpsBox();


            btn_stop_no.Text = Resources.GetString(Resources.StringResources.TextNo);
            btn_stop_yes.Text = Resources.GetString(Resources.StringResources.TextYes);
            text_stop_text1.Text = Resources.GetString(Resources.StringResources.scrStopTx1);
            text_stop_text2.Text = Resources.GetString(Resources.StringResources.scrStopTx2);
            text_stop_text3.Text = Resources.GetString(Resources.StringResources.scrStopTx3);
            text_stop_text4.Text = Resources.GetString(Resources.StringResources.scrStopTx4);
            text_stop_main.Text = Resources.GetString(Resources.StringResources.scrStopTxMain);

           
            window.Render();
        }

        private void btn_stop_yes_TapEvent(object sender)
        {
            LCDManager.GetInstance().StopPrint();

            if(!SwitchManager.GetInstance().IsOpenDoor1() && !SwitchManager.GetInstance().IsOpenDoor2())             
                    LCDManager.GetInstance().GoToParkPosition();

            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
        }

        private void btn_stop_no_TapEvent(object sender)
        {
            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Pause)
            {
                LCDManager.GetInstance().UpdateScreenLcd(StateScreen.PausePrint, "");
                return;
            }

            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.PrintInfo, "");
        }


     
    }
}
