using System;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using _3D_control_v09;
using Be3D.Constants;

namespace _3D_control_v09
{
    class LCDPrintInfoScreen : LCDScreen
    {

        private static LCDPrintInfoScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinPrintInfo));
        
        private TextBlock text_printinfo_state = (TextBlock)window.GetChildByName("text_state");
        private ProgressBar progress_printinfo_state = (ProgressBar)window.GetChildByName("progress");
        private TextBlock text_printinfo_tempext1 = (TextBlock)window.GetChildByName("text_tempext1");
        private TextBlock text_printinfo_tempext2 = (TextBlock)window.GetChildByName("text_tempext2");
        private TextBlock text_printinfo_tempbed = (TextBlock)window.GetChildByName("text_tempbed");
        private TextBlock text_printinfo_model = (TextBlock)window.GetChildByName("text_model");
        private TextBlock text_printinfo_tempspace = (TextBlock)window.GetChildByName("text_tempspace");
        private Button btn_printinfo_pause = (Button)window.GetChildByName("btn_pause");
        private Button btn_printinfo_stop = (Button)window.GetChildByName("btn_stop");
        private TextBlock text_printinfo_main = (TextBlock)window.GetChildByName("text_main");
        private TextBlock text_printinfo_text1 = (TextBlock)window.GetChildByName("text_info1");
        private TextBlock text_printinfo_text2 = (TextBlock)window.GetChildByName("text_info2");
        private TextBlock text_printinfo_text3 = (TextBlock)window.GetChildByName("text_info3");
        private TextBlock text_printinfo_text4 = (TextBlock)window.GetChildByName("text_info4");
        private TextBlock text_printinfo_text5 = (TextBlock)window.GetChildByName("text_info5");
        private TextBlock text_printinfo_text6 = (TextBlock)window.GetChildByName("text_info6");
        private TextBlock text_printinfo_text7 = (TextBlock)window.GetChildByName("text_info7");

        private LCDPrintInfoScreen()
        {
            setWindow(window);
            InitScreen();
        }

        public static LCDPrintInfoScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDPrintInfoScreen();

            return _instance;
        }
 
        private void InitScreen()
        {

            text_printinfo_state.Font = StateHolder.FontUbuntuSmall;
            text_printinfo_model.Font = StateHolder.FontUbuntuSmall;

            text_printinfo_tempbed.Font = StateHolder.FontUbuntuSmall;
            text_printinfo_tempext1.Font = StateHolder.FontUbuntuSmall;
            text_printinfo_tempext2.Font = StateHolder.FontUbuntuSmall;         
            text_printinfo_tempspace.Font = StateHolder.FontUbuntuSmall;

            //***************   Print info   ********************************

            btn_printinfo_stop.TapEvent += new OnTap(btn_printinfo_stop_TapEvent);
            btn_printinfo_stop.Font = StateHolder.FontUbuntuMiddle;
            btn_printinfo_stop.Enabled = true;
            btn_printinfo_pause.TapEvent += new OnTap(btn_printinfo_pause_TapEvent);
            btn_printinfo_pause.Font = StateHolder.FontUbuntuMiddle;
            btn_printinfo_pause.Enabled = false;


            text_printinfo_main.Font = StateHolder.FontUbuntuBig;           
            text_printinfo_text1.Font = StateHolder.FontUbuntuSmall;           
            text_printinfo_text2.Font = StateHolder.FontUbuntuSmall;           
            text_printinfo_text3.Font = StateHolder.FontUbuntuSmall;           
            text_printinfo_text4.Font = StateHolder.FontUbuntuSmall;           
            text_printinfo_text5.Font = StateHolder.FontUbuntuSmall;          
            text_printinfo_text6.Font = StateHolder.FontUbuntuSmall;         
            text_printinfo_text7.Font = StateHolder.FontUbuntuSmall;

            addUpsBox();

            btn_printinfo_pause.Text = Resources.GetString(Resources.StringResources.TextPause);
            btn_printinfo_stop.Text = Resources.GetString(Resources.StringResources.TextStop);

            text_printinfo_main.Text = Resources.GetString(Resources.StringResources.scrPrintInfoTxMain);
            text_printinfo_text1.Text = Resources.GetString(Resources.StringResources.scrPrintInfoTx1);
            text_printinfo_text2.Text = Resources.GetString(Resources.StringResources.scrPrintInfoTx2);
            text_printinfo_text3.Text = Resources.GetString(Resources.StringResources.scrPrintInfoTx3);
            text_printinfo_text4.Text = Resources.GetString(Resources.StringResources.scrPrintInfoTx4);
            text_printinfo_text5.Text = Resources.GetString(Resources.StringResources.scrPrintInfoTx5);

            //window.Render();
        }

        public void UpdateVisibleComponent(bool progres,bool pause) 
        {
            progress_printinfo_state.Visible = progres;
            window.FillRect(progress_printinfo_state.Rect);
            progress_printinfo_state.Invalidate();

            btn_printinfo_pause.Visible = pause;
            window.FillRect(btn_printinfo_pause.Rect);
            btn_printinfo_pause.Invalidate();

            if (!ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
            {
                text_printinfo_text5.Visible = false;
                window.FillRect(text_printinfo_text5.Rect);
                text_printinfo_text5.Invalidate();

                text_printinfo_tempspace.Visible = false;
                window.FillRect(text_printinfo_tempspace.Rect);
                text_printinfo_tempspace.Invalidate();
            }

            if (!ConfigurationPrinter.GetInstance().IsPresentBedHeat())
            {
                text_printinfo_text4.Visible = false;
                window.FillRect(text_printinfo_text4.Rect);
                text_printinfo_text4.Invalidate();

                text_printinfo_tempbed.Visible = false;
                window.FillRect(text_printinfo_tempbed.Rect);
                text_printinfo_tempbed.Invalidate();
            }

            if (!ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder() || Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || 
                Program.Type == Constants.TYPE_PRINTER.DeeRed2_1 || Program.Type == Constants.TYPE_PRINTER.DREStolice)
            {
                
                text_printinfo_text7.Visible = false;
                window.FillRect(text_printinfo_text7.Rect);
                text_printinfo_text7.Invalidate();

                text_printinfo_tempext2.Visible = false;
                window.FillRect(text_printinfo_tempext2.Rect);
                text_printinfo_tempext2.Invalidate();
            }
        }

        public void StartPrint()
        {
            //pokud nebezi tisk spust tisk
            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Idle)
                LCDManager.GetInstance().StartPrint();
        }

        #region Update
        public void UpdateAll()
        {
            UpdatePrintInfoModelName(StateHolder.GetInstance().PrintFile);
            UpdatePrintInfoState();
            UpdatePrintInfoTempExt(StateHolder.GetInstance().ActTempPrimaryExt, StateHolder.GetInstance().ActTempSecundaryExt);
            UpdatePrintInfoTempBed(StateHolder.GetInstance().ActTempBed);
            UpdatePrintInfoTempSpace(StateHolder.GetInstance().ActTempSpace);
            UpdatePrintInfoProgressBar();

        }

        private void UpdatePrintInfoState()
        {
            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Init)
                text_printinfo_state.Text = Resources.GetString(Resources.StringResources.TextStatePrintInit);
           
            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Print)
                text_printinfo_state.Text = Resources.GetString(Resources.StringResources.TextStatePrintPrint);

            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Done)
            {
                text_printinfo_state.Text = Resources.GetString(Resources.StringResources.TextStatePrintDone);
                btn_printinfo_stop.Text = Resources.GetString(Resources.StringResources.TextEnd);
            }

            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Print)
                btn_printinfo_pause.Enabled = true;
            else
            {
                btn_printinfo_pause.Enabled = false;
            }
            
            window.FillRect(text_printinfo_state.Rect);
            text_printinfo_state.Invalidate();

            window.FillRect(btn_printinfo_pause.Rect);
            btn_printinfo_pause.Invalidate();

            window.FillRect(btn_printinfo_stop.Rect);
            btn_printinfo_stop.Invalidate();
        }

        private void UpdatePrintInfoProgressBar()
        {
            double value = StateHolder.GetInstance().GETValuePrint();
            double maxValue = StateHolder.GetInstance().GETValuePrintMax();

            if (value == 0 || value < 0)
                return;
            if (maxValue == 0 || maxValue < 0)
                return;

            progress_printinfo_state.Value = (int)((value / maxValue) * 100);

            window.FillRect(progress_printinfo_state.Rect);
            progress_printinfo_state.Invalidate();
        }

        private void UpdatePrintInfoTempBed(int temp)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentBedHeat())
                return;

            if (temp > 20 && temp < 150)
                text_printinfo_tempbed.Text = temp.ToString() + Constants.strStC;
            else
                text_printinfo_tempbed.Text = "--" + Constants.strStC;

            window.FillRect(text_printinfo_tempbed.Rect);
            text_printinfo_tempbed.Invalidate();
        }

        private void UpdatePrintInfoTempExt(double tempExt1, int tempExt2)
        {
            if (tempExt1 > 20 && tempExt1 < 300)
                text_printinfo_tempext1.Text = tempExt1.ToString() + Constants.strStC;
            else
                text_printinfo_tempext1.Text = "--" + Constants.strStC;

            window.FillRect(text_printinfo_tempext1.Rect);
            text_printinfo_tempext1.Invalidate();


            if (ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder() )
            {
                if ((Program.Type != Constants.TYPE_PRINTER.DeeRed1_3) || (Program.Type != Constants.TYPE_PRINTER.DeeRed2_1) || 
                    Program.Type == Constants.TYPE_PRINTER.DREStolice)
                {
                    if (tempExt2 > 20 && tempExt2 < 300)
                        text_printinfo_tempext2.Text = tempExt2.ToString() + Constants.strStC;
                    else
                        text_printinfo_tempext2.Text = "--" + Constants.strStC;

                    window.FillRect(text_printinfo_tempext2.Rect);
                    text_printinfo_tempext2.Invalidate();
                }
            }
        }

        private void UpdatePrintInfoTempSpace(int tempSpace)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
                return;

            if(tempSpace > 0 && tempSpace < 125)
                text_printinfo_tempspace.Text = tempSpace.ToString() + Constants.strStC;
            else
                text_printinfo_tempspace.Text ="--" + Constants.strStC;

            window.FillRect(text_printinfo_tempspace.Rect);
            text_printinfo_tempspace.Invalidate();
        }

        private void UpdatePrintInfoModelName(string name)
        {
            string[] strarr = name.Split('.');

            if (strarr != null && strarr.Length > 1)
            {
                if (Constants.IsCharInName(strarr[0], '~'))
                {
                    text_printinfo_model.Text = strarr[0].Substring(0, strarr[0].Length - 2);
                }
                else
                {
                    text_printinfo_model.Text = strarr[0];
                }
            }
            else
                text_printinfo_model.Text = "";

            window.FillRect(text_printinfo_model.Rect);
            text_printinfo_model.Invalidate();
        }

        #endregion

        private void btn_printinfo_stop_TapEvent(object sender)
        {
            LCDManager.IsActivePrintInfoScreen = false;

            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Done)
            {
                LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
                return;
            }

            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Stop, "");
        }

        private void btn_printinfo_pause_TapEvent(object sender)
        {
            LCDManager.IsActivePrintInfoScreen = false;

            LCDManager.GetInstance().PausePrint();
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.PausePrint, "");
        }


        
    }
}
