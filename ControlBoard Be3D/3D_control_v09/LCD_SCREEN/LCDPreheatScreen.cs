using Be3D.Constants;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;

namespace _3D_control_v09
{
    internal class LCDPreheatScreen : LCDScreen
    {
        private static LCDPreheatScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinPreheater));

        private TextBlock text_preheat_tempext1_set = (TextBlock)window.GetChildByName("text_tempext1_set");
        private TextBlock text_preheat_tempbed_set = (TextBlock)window.GetChildByName("text_tempbed_set");
        private TextBlock text_preheat_tempext1_act = (TextBlock)window.GetChildByName("text_tempext1_act");
        private TextBlock text_preheat_tempbed_act = (TextBlock)window.GetChildByName("text_tempbed_act");
        private TextBlock text_preheat_tempext2_set = (TextBlock)window.GetChildByName("text_tempext2_set");
        private TextBlock text_preheat_tempspace_set = (TextBlock)window.GetChildByName("text_tempspace_set");
        private TextBlock text_preheat_tempext2_act = (TextBlock)window.GetChildByName("text_tempext2_act");
        private TextBlock text_preheat_tempspace_act = (TextBlock)window.GetChildByName("text_tempspace_act");
        private TextBlock text_preheat_state = (TextBlock)window.GetChildByName("text_info3");
        private Button btn_preheat_preheat = (Button)window.GetChildByName("btn_preheat");
        private Button btn_preheat_back = (Button)window.GetChildByName("btn_back");
        private TextBlock text_preheat_text1 = (TextBlock)window.GetChildByName("text_info1");
        private TextBlock text_preheat_text2 = (TextBlock)window.GetChildByName("text_info2");
        private TextBlock text_preheat_text4 = (TextBlock)window.GetChildByName("text_info4");
        private TextBlock text_preheat_text5 = (TextBlock)window.GetChildByName("text_info5");
        private TextBlock text_preheat_text6 = (TextBlock)window.GetChildByName("text_info6");
        private TextBlock text_preheat_text7 = (TextBlock)window.GetChildByName("text_info7");
        private TextBlock text_preheat_main = (TextBlock)window.GetChildByName("text_main");

        private LCDPreheatScreen()
        {
            setWindow(window);
            InitScreen();
        }

        public static LCDPreheatScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDPreheatScreen();

            return _instance;
        }

        private void InitScreen()
        {
            //***************   Preheater window   **************************
            
            btn_preheat_back.TapEvent += new OnTap(btn_preheat_back_TapEvent);
            btn_preheat_back.Font = StateHolder.FontUbuntuMiddle;
            btn_preheat_preheat.TapEvent += new OnTap(btn_preheat_preheat_TapEvent);
            btn_preheat_preheat.Font = StateHolder.FontUbuntuMiddle;
            
            text_preheat_text1.Font = StateHolder.FontUbuntuSmall;
            
            text_preheat_text2.Font = StateHolder.FontUbuntuSmall;
            text_preheat_state.Font = StateHolder.FontUbuntuMiddle;
            text_preheat_state.FontColor = Colors.Green;
            
            text_preheat_text4.Font = StateHolder.FontUbuntuSmall;
            
            text_preheat_text5.Font = StateHolder.FontUbuntuSmall;
            
            text_preheat_text6.Font = StateHolder.FontUbuntuSmall;
            
            text_preheat_text7.Font = StateHolder.FontUbuntuSmall;
            
            text_preheat_main.Font = StateHolder.FontUbuntuBig;

            text_preheat_tempbed_set.Font = StateHolder.FontUbuntuSmall;
            text_preheat_tempext1_set.Font = StateHolder.FontUbuntuSmall;
            text_preheat_tempbed_act.Font = StateHolder.FontUbuntuSmall;
            text_preheat_tempext1_act.Font = StateHolder.FontUbuntuSmall;
            text_preheat_tempext2_set.Font = StateHolder.FontUbuntuSmall;
            text_preheat_tempext2_act.Font = StateHolder.FontUbuntuSmall;
            text_preheat_tempspace_act.Font = StateHolder.FontUbuntuSmall;
            text_preheat_tempspace_set.Font = StateHolder.FontUbuntuSmall;


            btn_preheat_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            btn_preheat_preheat.Text = Resources.GetString(Resources.StringResources.scrPreheatBtOn);

            text_preheat_text1.Text = Resources.GetString(Resources.StringResources.scrPreheatTx1);
            text_preheat_text2.Text = Resources.GetString(Resources.StringResources.scrPreheatTx2);
            text_preheat_text4.Text = Resources.GetString(Resources.StringResources.scrPreheatTx4);
            text_preheat_text5.Text = Resources.GetString(Resources.StringResources.scrPreheatTx5);
            text_preheat_text6.Text = Resources.GetString(Resources.StringResources.scrPreheatTx6);
            text_preheat_text7.Text = Resources.GetString(Resources.StringResources.scrPreheatTx7);
            text_preheat_main.Text = Resources.GetString(Resources.StringResources.scrPreheatTxMain);
            text_preheat_state.Text = Resources.GetString(Resources.StringResources.scrPreheatTxStateOff);

            //zneviditelneni polozek v menu v pripade absence prvku
            if (!ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder() || Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 ||
                Program.Type == Constants.TYPE_PRINTER.DeeRed2_1 || Program.Type == Constants.TYPE_PRINTER.DREStolice)
            {
                text_preheat_text1.Text = Resources.GetString(Resources.StringResources.scrPrintInfoTx3);
                text_preheat_tempext2_set.Visible = false;
                text_preheat_tempext2_act.Visible = false;
                text_preheat_text2.Visible = false;
            }

            if (!ConfigurationPrinter.GetInstance().IsPresentBedHeat())
            {
                text_preheat_tempbed_set.Visible = false;
                text_preheat_tempbed_act.Visible = false;
                text_preheat_text6.Visible = false;
            }

            if (!ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
            {
                text_preheat_tempspace_set.Visible = false;
                text_preheat_tempspace_act.Visible = false;
                text_preheat_text7.Visible = false;
            }

            addUpsBox();

            window.Render();

        }

        public void UpdateState(bool state)
        {
            StateHolder.GetInstance().ActSetTempBed = Program._basicConfig.DefTmpRefBed;
            StateHolder.GetInstance().ActSetTempPrimary = Program._basicConfig.DefTmpRefExtr0;
            StateHolder.GetInstance().ActSetTempSecondary = Program._basicConfig.DefTmpRefExtr1;
            StateHolder.GetInstance().ActSetTempSpace = Program._basicConfig.DefTmpRefSpace;

            UpdateTempBedSet(StateHolder.GetInstance().ActSetTempBed.ToString() + Constants.strStC);
            UpdateTempExt1Set(StateHolder.GetInstance().ActSetTempPrimary.ToString() + Constants.strStC);
            UpdateTempExt2Set(StateHolder.GetInstance().ActSetTempSecondary.ToString() + Constants.strStC);
            UpdateTempSpaceSet(StateHolder.GetInstance().ActSetTempSpace.ToString() + Constants.strStC);

            if (state)
            {
                updateStateHeatON();
                LCDManager.GetInstance().StartPreHeating();
            }
            else
            {
                updateStatHeatOFF();
                LCDManager.GetInstance().StopPreheating();
            }
        }

        private void updateStateHeatON()
        {
            text_preheat_state.Text = Resources.GetString(Resources.StringResources.scrPreheatTxStateOn);
            //text_preheat_state.FontColor = Colors.Red;
            text_preheat_state.FontColor = Colors.Green;
            btn_preheat_preheat.Text = Resources.GetString(Resources.StringResources.scrPreheatBtOff);

            window.FillRect(text_preheat_state.Rect);
            text_preheat_state.Invalidate();
            window.FillRect(btn_preheat_preheat.Rect);
            btn_preheat_preheat.Invalidate();
        }

        private void updateStatHeatOFF()
        {
            text_preheat_state.Text = Resources.GetString(Resources.StringResources.scrPreheatTxStateOff);
            //text_preheat_state.FontColor = Colors.Green;
            text_preheat_state.FontColor = Colors.Red;
            btn_preheat_preheat.Text = Resources.GetString(Resources.StringResources.scrPreheatBtOn);

            window.FillRect(text_preheat_state.Rect);
            text_preheat_state.Invalidate();
            window.FillRect(btn_preheat_preheat.Rect);
            btn_preheat_preheat.Invalidate();
        }


        public void UpdateActTemp()
        {
            if (StateHolder.GetInstance().ActTempBed > 10 && StateHolder.GetInstance().ActTempBed < 150)
                UpdateTempBedAct(StateHolder.GetInstance().ActTempBed.ToString() + Constants.strStC);
            else
                UpdateTempBedAct(Constants.strFailTemp);

            UpdateTempExt1Act(StateHolder.GetInstance().ActTempPrimaryExt.ToString() + Constants.strStC);
            UpdateTempExt2Act(StateHolder.GetInstance().ActTempSecundaryExt.ToString() + Constants.strStC);
            UpdateTempExt3Act(StateHolder.GetInstance().ActTempThirdExt.ToString() + Constants.strStC);

            if (StateHolder.GetInstance().ActTempSpace > 10 && StateHolder.GetInstance().ActTempSpace < 150)
                UpdateTempSpaceAct(StateHolder.GetInstance().ActTempSpace.ToString() + Constants.strStC);
            else
                UpdateTempSpaceAct(Constants.strFailTemp);

        }

        private void UpdateTempExt1Set(string temp)
        {
            text_preheat_tempext1_set.Text = temp;

            window.FillRect(text_preheat_tempext1_set.Rect);
            text_preheat_tempext1_set.Invalidate();
        }
        private void UpdateTempExt1Act(string temp)
        {
            text_preheat_tempext1_act.Text = temp;

            window.FillRect(text_preheat_tempext1_act.Rect);
            text_preheat_tempext1_act.Invalidate();
        }

        private void UpdateTempExt2Set(string temp)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder())
                return;

            text_preheat_tempext2_set.Text = temp;

            window.FillRect(text_preheat_tempext2_set.Rect);
            text_preheat_tempext2_set.Invalidate();
        }
        private void UpdateTempExt2Act(string temp)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder())
                return;

            if (Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Constants.TYPE_PRINTER.DeeRed2_1 || Program.Type == Constants.TYPE_PRINTER.DREStolice)
                return;

            text_preheat_tempext2_act.Text = temp;

            window.FillRect(text_preheat_tempext2_act.Rect);
            text_preheat_tempext2_act.Invalidate();
        }

        private void UpdateTempExt3Set(string temp)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentThirdExtruder())
                return;

            //text_preheat_tempext2_act.Text = temp;

            //window.FillRect(text_preheat_tempext2_act.Rect);
            //text_preheat_tempext2_act.Invalidate();
        }
        private void UpdateTempExt3Act(string temp)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentThirdExtruder())
                return;

            //text_preheat_tempext2_act.Text = temp;

            //window.FillRect(text_preheat_tempext2_act.Rect);
            //text_preheat_tempext2_act.Invalidate();
        }


        private void UpdateTempBedSet(string temp)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentBedHeat())
                return;
            
            text_preheat_tempbed_set.Text = temp;

            window.FillRect(text_preheat_tempbed_set.Rect);
            text_preheat_tempbed_set.Invalidate();
        }

        private void UpdateTempBedAct(string temp)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentBedHeat())
                return;

            text_preheat_tempbed_act.Text = temp;

            window.FillRect(text_preheat_tempbed_act.Rect);
            text_preheat_tempbed_act.Invalidate();
        }

        private void UpdateTempSpaceSet(string temp)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
                return;

            text_preheat_tempspace_set.Text = temp;

            window.FillRect(text_preheat_tempspace_set.Rect);
            text_preheat_tempspace_set.Invalidate();
        }

        private void UpdateTempSpaceAct(string temp)
        {
            if (!ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
                return;

            text_preheat_tempspace_act.Text = temp;

            window.FillRect(text_preheat_tempspace_act.Rect);
            text_preheat_tempspace_act.Invalidate();
        }


        private void btn_preheat_back_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Model, "");           
        }
    
        private void btn_preheat_preheat_TapEvent(object sender)
        {
            if (!StateHolder.GetInstance().StateHeating)
            {
                LCDManager.GetInstance().StartPreHeating();
                updateStateHeatON();
            }
            else
            {
                LCDManager.GetInstance().StopPreheating();
                updateStatHeatOFF();
            }
        }
    }
}
