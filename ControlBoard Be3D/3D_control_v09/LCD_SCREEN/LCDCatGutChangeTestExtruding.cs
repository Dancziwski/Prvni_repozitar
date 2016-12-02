using System.Threading;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using Be3D.Constants;

namespace _3D_control_v09
{
    class LCDCatGutChangeTestExtruding : LCDScreen
    {
        private static LCDCatGutChangeTestExtruding _instance;

        //private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinCutChange));
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinCutChangeStoliceDGR));

        private Button btn_continue;
        private Button btn_repeat;
        private Button btn_end;

        private ProgressBar progress_change_state;
        private ProgressBar progress_change_state2;
        private ProgressBar progress_change_state3;
        private ProgressBar progress_change_state4;
        private ProgressBar progress_change_state5;


        private TextBlock text_info;
        private TextBlock text_change_main;

        private RadioButton rdb_extr1_yes;
        private RadioButton rdb_extr2_no;

        private TextBlock text_extr1_yes;
        private TextBlock text_extr2_no;

        private Constants.FILCHANGE_STATE _stateCatGut;

        private Constants.EXTRUDER _actExt = Constants.EXTRUDER.ExtruderPrimary;
        //private  Constants.MATERIAL _actMat = Constants.MATERIAL.ABS;

        private int oldTempExt1 = 0;
        private int oldTempExt2 = 0;
        private int oldTempExt3 = 0;

        public static bool disableUpdate = false;  // zabranuje interni refresh progresbaru po prepnuti sync vlaken

        private LCDCatGutChangeTestExtruding()
        {
            setWindow(window);
            Init();
           
        }
        public static LCDCatGutChangeTestExtruding GetInstance()
        {
             if(_instance == null)
                 _instance = new LCDCatGutChangeTestExtruding();

            return _instance;
        }

        public void LoadPrintTemper()
        {
            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Pause)
            {
                oldTempExt1 = StateHolder.GetInstance().ActSetTempInPowerBordPrimExt;
                oldTempExt2 = StateHolder.GetInstance().ActSetTempInPowerBordSecundExt;
                oldTempExt3 = StateHolder.GetInstance().ActSetTempInPowerBordThirdExt;
            }

            //GetActExtruder();
            //LCDManager.GetInstance().StartHeatingExt(_actExt, StateHolder.GetInstance().ActSetTempForCatGut);
            
            LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderPrimary, StateHolder.GetInstance().ActSetTempForCatGut);

            if (ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder())
                LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderSecondary, StateHolder.GetInstance().ActSetTempForCatGut);
            
            if(ConfigurationPrinter.GetInstance().IsPresentThirdExtruder())
                LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderThird, StateHolder.GetInstance().ActSetTempForCatGut);
            
            // ma se spustit pruvodce volbou extruderu ? 
            if(Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Constants.TYPE_PRINTER.DeeRed2_1 || Program.Type == Constants.TYPE_PRINTER.DREStolice) {
                rdb_extr1_yes.Checked = true;
                rdb_extr2_no.Checked = false;

                InitScreenSelExtr();
                return;
            }

            disableUpdate = true;
            InitScreenHeating();
        }

        private void Init()
        {
            btn_continue = (Button) window.GetChildByName("btn_continue");
            btn_continue.PressEvent += btn_change_continue_TapEvent;
            btn_continue.Font = StateHolder.FontUbuntuMiddle;

            btn_repeat = (Button) window.GetChildByName("btn_repeat");
            btn_repeat.PressEvent += btn_change_repeat_PressEvent;
            btn_repeat.Font = StateHolder.FontUbuntuSmall;

            btn_end = (Button) window.GetChildByName("btn_end");
            btn_end.PressEvent += btn_change_end_TapEvent;
            btn_end.Font = StateHolder.FontUbuntuMiddle;

            text_change_main = (TextBlock) window.GetChildByName("text_main");
            text_change_main.Font = StateHolder.FontUbuntuBig;

            text_info = (TextBlock) window.GetChildByName("text_info");
            //text_info.Font = StateHolder.FontUbuntuSmall;
            text_info.Font = StateHolder.FontUbuntuSmall;
            
            progress_change_state = (ProgressBar) window.GetChildByName("progressBar");
            progress_change_state2 = (ProgressBar)window.GetChildByName("progressBar2");
            progress_change_state3 = (ProgressBar)window.GetChildByName("progressBar3");
            progress_change_state4 = (ProgressBar)window.GetChildByName("progressBar4");
            progress_change_state5 = (ProgressBar)window.GetChildByName("progressBar5");

            text_extr1_yes = (TextBlock) window.GetChildByName("text_catgut1");
            text_extr1_yes.Font = StateHolder.FontUbuntuSmall;
            text_extr2_no = (TextBlock) window.GetChildByName("text_catgut2");
            text_extr2_no.Font = StateHolder.FontUbuntuSmall;
           

            rdb_extr1_yes = (RadioButton) window.GetChildByName("rdb_catgut1");
            rdb_extr1_yes.TapEvent += new OnTap(rdb_1_TapEvent);

            rdb_extr2_no = (RadioButton) window.GetChildByName("rdb_catgut2");
            rdb_extr2_no.TapEvent += new OnTap(rdb_2_TapEvent);

            addUpsBox();

            text_change_main.Text = Resources.GetString(Resources.StringResources.scrCatGutChTxMain);
            
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextContinue);
            btn_end.Text = Resources.GetString(Resources.StringResources.TextEnd);
            btn_repeat.Text = "150";
        }

        private void InitScreenSelExtr() // vyber extruderu
        {
            _stateCatGut = Constants.FILCHANGE_STATE.SelectExtr;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCatGutChScr0Tx1);
            text_extr1_yes.Text = Resources.GetString(Resources.StringResources.scrCatGutChScr0Cat1);
            text_extr2_no.Text = Resources.GetString(Resources.StringResources.scrCatGutChScr0Cat2);

            progress_change_state.Visible = false;
            progress_change_state2.Visible = false;
            progress_change_state3.Visible = false;
            progress_change_state4.Visible = false;
            progress_change_state5.Visible = false;

            text_extr1_yes.Visible = true;
            text_extr2_no.Visible = false;
            rdb_extr1_yes.Visible = true;
            rdb_extr2_no.Visible = false;

            btn_repeat.Visible = false;
            btn_continue.Enabled = true;
            btn_end.Visible = true;

            btn_continue.Text = Resources.GetString(Resources.StringResources.TextContinue);

            if (ConfigurationPrinter.GetInstance().IsPresentSecondaryExtruder())
            {
                text_extr2_no.Visible = true;
                rdb_extr2_no.Visible = true;
            }
 
            window.Invalidate();

        }

        private void InitScreenHeating() // ohrev
        {
            
            _stateCatGut = Constants.FILCHANGE_STATE.Heater;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCatGutChScr1Tx1);
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextContinue);

            btn_repeat.Visible = false;
            btn_continue.Enabled = true;
            btn_end.Visible = true;

            progress_change_state.Visible = true;
            progress_change_state2.Visible = true;
            progress_change_state3.Visible = true;
            progress_change_state4.Visible = true;
            progress_change_state5.Visible = true;

            text_extr1_yes.Visible = false;
            text_extr2_no.Visible = false;
            
            rdb_extr1_yes.Visible = false;
            rdb_extr2_no.Visible = false;

            if (Program.Type == Constants.TYPE_PRINTER.DGRStolice3 || Program.Type == Constants.TYPE_PRINTER.DGRStolice5)
                UpdateIndividualProgressBar();
            else
                UpdateProgressBar();
            
            window.Invalidate();


        }

        private void InitScreenIsFilam() // je v tiskarne struna
        {
            _stateCatGut = Constants.FILCHANGE_STATE.IsFilament;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCatGutChScr2Tx1);
            
            //text_extr1_yes.Text = Resources.GetString(Resources.StringResources.TextYes);
            //text_extr2_no.Text = Resources.GetString(Resources.StringResources.TextNo); ;

            text_extr1_yes.Text = Resources.GetString(Resources.StringResources.TextRemove);
            text_extr2_no.Text = Resources.GetString(Resources.StringResources.TextFeedIn);
              

            progress_change_state.Visible = false;
            progress_change_state2.Visible = false;
            progress_change_state3.Visible = false;
            progress_change_state4.Visible = false;
            progress_change_state5.Visible = false;


            text_extr1_yes.Visible = true;
            text_extr2_no.Visible = true;
            rdb_extr1_yes.Visible = true;
            rdb_extr2_no.Visible = true;

            rdb_extr1_yes.Checked = true;
            rdb_extr2_no.Checked = false;

            window.Invalidate();

            
        }

        private void InitScreenUpFil() // ano je struna vysouvam
        {
            _stateCatGut = Constants.FILCHANGE_STATE.UpFilament;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCatGutChScr3Tx1);

            //***************   Change catgut window 1 **********************
            progress_change_state.Visible = false;
            progress_change_state2.Visible = false;
            progress_change_state3.Visible = false;
            progress_change_state4.Visible = false;
            progress_change_state5.Visible = false;

            text_extr1_yes.Visible = false;
            text_extr2_no.Visible = false;
            rdb_extr1_yes.Visible = false;
            rdb_extr2_no.Visible = false;

            window.Invalidate();


            string speedFill = "200";
            //TODO prasarna pøedìlat do rozumného stavu
            if (Program.Type == Constants.TYPE_PRINTER.DREStolice || Program.Type == Constants.TYPE_PRINTER.DGRStolice3)
                speedFill = "300";

            if (Program.Type == Constants.TYPE_PRINTER.DGRStolice3)
            {
                _actExt = Constants.EXTRUDER.ExtruderPrimary;
                LCDManager.GetInstance().MoveString(_actExt, "-0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, "-0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, ConfigurationPrinter.GetInstance().GetLenghtMoveFilUp(), speedFill);

                _actExt = Constants.EXTRUDER.ExtruderSecondary;
                LCDManager.GetInstance().MoveString(_actExt, "-0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, "-0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, ConfigurationPrinter.GetInstance().GetLenghtMoveFilUp(), speedFill);

                _actExt = Constants.EXTRUDER.ExtruderThird;
                LCDManager.GetInstance().MoveString(_actExt, "-0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, "-0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, ConfigurationPrinter.GetInstance().GetLenghtMoveFilUp(), speedFill);
            }
            else
            {
                LCDManager.GetInstance().MoveString(_actExt, "-0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, "-0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, ConfigurationPrinter.GetInstance().GetLenghtMoveFilUp(), speedFill);

            }
        }

        private void InitScreenDownFil()  // natahuji material
        {
            _stateCatGut = Constants.FILCHANGE_STATE.DownFilament;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCatGutChScr5Tx1);

            progress_change_state.Visible = false;
            progress_change_state2.Visible = false;
            progress_change_state3.Visible = false;
            progress_change_state4.Visible = false;
            progress_change_state5.Visible = false;

            text_extr1_yes.Visible = false;
            text_extr2_no.Visible = false;
            rdb_extr1_yes.Visible = false;
            rdb_extr2_no.Visible = false;


            //TODO prasarna predela globalne
            if (Program.Type == Constants.TYPE_PRINTER.DREStolice || Program.Type == Constants.TYPE_PRINTER.DGRStolice3)
            {
                btn_repeat.Text = "150"; 
            }

            btn_repeat.Visible = true;
            btn_end.Visible = false;
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextEnd);

            window.Invalidate();

            string speedFill = "200";
            //TODO prasarna pøedìlat globalnì
            if (Program.Type == Constants.TYPE_PRINTER.DREStolice || Program.Type == Constants.TYPE_PRINTER.DGRStolice3)
                speedFill = "300";

           

            if (Program.Type == Constants.TYPE_PRINTER.DGRStolice3)
            {
                _actExt = Constants.EXTRUDER.ExtruderPrimary;
                LCDManager.GetInstance().MoveString(_actExt, "0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, "0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, ConfigurationPrinter.GetInstance().GetLenghtMoveFilDown(), speedFill);

                _actExt = Constants.EXTRUDER.ExtruderSecondary;
                LCDManager.GetInstance().MoveString(_actExt, "0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, "0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, ConfigurationPrinter.GetInstance().GetLenghtMoveFilDown(), speedFill);

                _actExt = Constants.EXTRUDER.ExtruderThird;
                LCDManager.GetInstance().MoveString(_actExt, "0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, "0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, ConfigurationPrinter.GetInstance().GetLenghtMoveFilDown(), speedFill);
            }
            else
            {
                LCDManager.GetInstance().MoveString(_actExt, "0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, "0.1", speedFill);
                LCDManager.GetInstance().MoveString(_actExt, ConfigurationPrinter.GetInstance().GetLenghtMoveFilDown(), speedFill);

            }



        }

        private void rdb_1_TapEvent(object sender)
        {

            rdb_extr1_yes.Checked = true;
            rdb_extr2_no.Checked = false;
        }

        private void rdb_2_TapEvent(object sender)
        {

            rdb_extr1_yes.Checked = false;
            rdb_extr2_no.Checked = true;
        }

        public void UpdateProgressBar()
        {
            if (disableUpdate == false)
                return;

            double actTemp = 0;
            double setTemp = 0;
            
            if (_actExt == Constants.EXTRUDER.ExtruderPrimary) // T0
            {
                actTemp = StateHolder.GetInstance().ActTempPrimaryExt;
                setTemp = StateHolder.GetInstance().ActSetTempPrimary;

            }

            if (_actExt == Constants.EXTRUDER.ExtruderSecondary) // T1
            {
                actTemp = StateHolder.GetInstance().ActTempSecundaryExt;
                setTemp = StateHolder.GetInstance().ActSetTempSecondary;
            
            }

            if (setTemp == 0 || setTemp < 0)
            {
                btn_continue.Enabled = false;
                return;
            }

            progress_change_state.Value = (int)((actTemp / setTemp) * 100);
            
            if (progress_change_state.Value > 100)
                progress_change_state.Value = 100;

            //+-5~tolerantni vysledek
            if (actTemp >= setTemp-5)
                btn_continue.Enabled = true;
            else
                btn_continue.Enabled = false;

            window.FillRect(btn_continue.Rect);
            btn_continue.Invalidate();

            window.FillRect(progress_change_state.Rect);
            progress_change_state.Invalidate();

        }

        public void UpdateIndividualProgressBar()
        {
            if (disableUpdate == false)
                return;

            double actTemp = 0;
            double setTemp = 0;

            //---------------------------------------------------------------------------------------
            actTemp = StateHolder.GetInstance().ActTempPrimaryExt;
            setTemp = StateHolder.GetInstance().ActSetTempPrimary;

            progress_change_state.Value = (int)((actTemp / setTemp) * 100);

            if (progress_change_state.Value > 100)
                progress_change_state.Value = 100;

            if (setTemp == 0 || setTemp < 0)
            {
                btn_continue.Enabled = false;
                return;
            }

            //+-5~tolerantni vysledek
            if (actTemp >= setTemp - 5)
                btn_continue.Enabled = true;
            else
                btn_continue.Enabled = false;
            
            //---------------------------------------------------------------------------------------
            actTemp = StateHolder.GetInstance().ActTempSecundaryExt;
            setTemp = StateHolder.GetInstance().ActSetTempSecondary;

            progress_change_state2.Value = (int)((actTemp / setTemp) * 100);

            if (progress_change_state2.Value > 100)
                progress_change_state2.Value = 100;

            if (setTemp == 0 || setTemp < 0)
            {
                btn_continue.Enabled = false;
                return;
            }

            //+-5~tolerantni vysledek
            if (actTemp >= setTemp - 5)
                btn_continue.Enabled = true;
            else
                btn_continue.Enabled = false;
            
            //---------------------------------------------------------------------------------------
            actTemp = StateHolder.GetInstance().ActTempThirdExt;
            setTemp = StateHolder.GetInstance().ActSetTempThird;

            progress_change_state3.Value = (int)((actTemp / setTemp) * 100);

            if (progress_change_state3.Value > 100)
                progress_change_state3.Value = 100;

            if (setTemp == 0 || setTemp < 0)
            {
                btn_continue.Enabled = false;
                return;
            }

            //+-5~tolerantni vysledek
            if (actTemp >= setTemp - 5)
                btn_continue.Enabled = true;
            else
                btn_continue.Enabled = false;
            
            
            window.FillRect(btn_continue.Rect);
            btn_continue.Invalidate();

            window.FillRect(progress_change_state.Rect);
            progress_change_state.Invalidate();

            window.FillRect(progress_change_state2.Rect);
            progress_change_state2.Invalidate();

            window.FillRect(progress_change_state3.Rect);
            progress_change_state3.Invalidate();

        }

        private void GetActExtruder()
        {
            if (rdb_extr1_yes.Checked)
                _actExt = Constants.EXTRUDER.ExtruderPrimary;

            if (rdb_extr2_no.Checked)
                _actExt = Constants.EXTRUDER.ExtruderSecondary;
        }

        private void btn_change_repeat_PressEvent(object sender)
        {

            LCDManager.GetInstance().MoveString(_actExt, "150", "300");
            btn_repeat.Enabled = false;

            window.FillRect(btn_repeat.Rect);
            btn_repeat.Invalidate();

            new Thread(new ThreadStart(waitingInDownFil2sBtnRepeat)).Start();

        }

        private void waitingInDownFil2sBtnRepeat()
        {
            Thread.Sleep(2000);
            btn_repeat.Enabled = true;

            window.FillRect(btn_repeat.Rect);
            btn_repeat.Invalidate();
        }

        private void btn_change_continue_TapEvent(object sender)
        {
            Thread.Sleep(200);  // umele spozdeni refreshe okna
            switch (_stateCatGut)
            {
                case Constants.FILCHANGE_STATE.SelectExtr:
                    {
                        GetActExtruder();

                        //TODO asi duplicita kodu
                        switch (Program.Type)
                        {
                            case Constants.TYPE_PRINTER.DeeRed1_2:
                                LCDManager.GetInstance().StartHeatingExt(_actExt, StateHolder.GetInstance().ActSetTempForCatGut);
                                break;
                            case Constants.TYPE_PRINTER.DeeRed1_3:
                                {
                                    LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderPrimary, StateHolder.GetInstance().ActSetTempForCatGut);
                                    LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderSecondary, StateHolder.GetInstance().ActSetTempForCatGut);
                                }
                                break;
                            case Constants.TYPE_PRINTER.DeeRed2_1:
                                LCDManager.GetInstance().StartHeatingExt(_actExt, StateHolder.GetInstance().ActSetTempForCatGut);
                                break;
                            case Constants.TYPE_PRINTER.DREStolice:
                                {
                                    LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderPrimary, StateHolder.GetInstance().ActSetTempForCatGut);
                                    LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderSecondary, StateHolder.GetInstance().ActSetTempForCatGut);
                                }
                                break;
                            case Constants.TYPE_PRINTER.DGRStolice3:
                                {
                                    LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderPrimary, StateHolder.GetInstance().ActSetTempForCatGut);
                                    LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderSecondary, StateHolder.GetInstance().ActSetTempForCatGut);
                                    LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderThird, StateHolder.GetInstance().ActSetTempForCatGut);
                                }
                                break;
                            case Constants.TYPE_PRINTER.PresentDeeRed:
                                LCDManager.GetInstance().StartHeatingExt(_actExt, StateHolder.GetInstance().ActSetTempForCatGut);
                                break;
                            default:
                                LCDManager.GetInstance().StartHeatingExt(_actExt, StateHolder.GetInstance().ActSetTempForCatGut);
                                break;
                        }

                        disableUpdate = true;
                        InitScreenHeating();
                        break;
                    }
                case Constants.FILCHANGE_STATE.Heater:
                    {
                        disableUpdate = false;
                        InitScreenIsFilam();
                        break;
                    }
                case Constants.FILCHANGE_STATE.IsFilament:
                    {
                        LCDManager.GetInstance().StartHeatingExt(_actExt, StateHolder.GetInstance().ActSetTempForCatGut);
                       
                        if (rdb_extr1_yes.Checked) 
                            InitScreenUpFil();

                        if (rdb_extr2_no.Checked)
                            InitScreenDownFil();

                        break;
                    }
                case Constants.FILCHANGE_STATE.UpFilament:
                    {
                        LCDManager.GetInstance().MoveStop();
                        LCDManager.GetInstance().StartHeatingExt(_actExt, StateHolder.GetInstance().ActSetTempForCatGut);

                        InitScreenDownFil();
                        break;
                    }
                
                case Constants.FILCHANGE_STATE.DownFilament:
                    {
                        LCDManager.GetInstance().MoveStop();
                        StateHolder.GetInstance().FilChangeCorrect = true;
                        EndProccesCut();
                        break;
                    }         
            }
        }

        private void btn_change_end_TapEvent(object sender)
        {
            EndProccesCut();
        }

        private void EndProccesCut()
        {
             disableUpdate = false;
            _stateCatGut = Constants.FILCHANGE_STATE.SelectExtr;
           
            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Pause)
            {
                LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderPrimary, oldTempExt1);
                LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderSecondary, oldTempExt2);
                LCDManager.GetInstance().UpdateScreenLcd(StateScreen.PausePrint, "");
                return;
            }

            LCDManager.GetInstance().MoveStop();
            LCDManager.GetInstance().StopHeatingExt(Constants.EXTRUDER.ExtruderPrimary);
            LCDManager.GetInstance().StopHeatingExt(Constants.EXTRUDER.ExtruderSecondary);

            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Tests, "");
            
        }



    }
}
