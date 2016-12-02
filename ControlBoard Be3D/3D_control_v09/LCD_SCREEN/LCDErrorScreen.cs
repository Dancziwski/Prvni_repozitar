using System;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;

namespace _3D_control_v09
{
    class LCDErrorScreen : LCDScreen
    {
        private Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinError));
       
        private readonly string _mess = "";
        private string _messMain = "";
        private StateScreen _previousState;

        public LCDErrorScreen(StateScreen actStateScreen, String message, String messMain, bool isButtonBack)
        {
            if(messMain != "")
                _messMain = messMain;
            
            _mess = message;
            _previousState = actStateScreen;
            setWindow(window);
           
            InitScreen(isButtonBack);
        }

        private void InitScreen(bool isButtonBack)
        {
            TextBlock text_error_text1 = (TextBlock)window.GetChildByName("text_info1");
            TextBlock text_error_text2= (TextBlock)window.GetChildByName("text_info2");
            TextBlock text_error_text3 = (TextBlock)window.GetChildByName("text_info3");
            TextBlock text_error_text4 = (TextBlock)window.GetChildByName("text_info4");

            Button btn_error_back = (Button)window.GetChildByName("btn_continue");
            TextBlock text_error_main = (TextBlock)window.GetChildByName("text_main");

            //***************   Error window  *******************************
             btn_error_back.TapEvent += new OnTap(btn_error_back_TapEvent);
            btn_error_back.Font = StateHolder.FontUbuntuMiddle;
            text_error_text1.Font = StateHolder.FontUbuntuSmall;
            text_error_text2.Font = StateHolder.FontUbuntuSmall;
            text_error_text3.Font = StateHolder.FontUbuntuSmall;
            text_error_text4.Font = StateHolder.FontUbuntuSmall;

            text_error_main.Font = StateHolder.FontUbuntuBig;

            btn_error_back.Text = Resources.GetString(Resources.StringResources.TextBack);

            addUpsBox();

            if(_messMain == "")
                text_error_main.Text = Resources.GetString(Resources.StringResources.TextError);
            else
            {
                text_error_main.Text = _messMain;
            }

            if (_mess != "")
            {
                string[] sepMess = StateHolder.GetInstance().SeparateTextTo4Line(38, _mess);
                text_error_text1.Text = sepMess[0];
                text_error_text2.Text = sepMess[1];
                text_error_text3.Text = sepMess[2];
                text_error_text4.Text = sepMess[3];
            }
            else
            {
                text_error_text1.Text = "";
                text_error_text2.Text = "";
                text_error_text3.Text = "";
                text_error_text4.Text = "";
            }

            if (isButtonBack == false)
                btn_error_back.Visible = false;
            else
                btn_error_back.Visible = true;

            
           window.Render();

        }

        private void btn_error_back_TapEvent(object sender)
        {
            LCDManager.IsActivateErrorScreen = false;
            LCDManager.GetInstance().UpdateScreenLcd(_previousState, "");
        }


        
    }
}
