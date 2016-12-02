using System.Threading;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using Be3D.Constants;
using System;
using Microsoft.SPOT;

namespace _3D_control_v09
{
    class LCDCatGutChangeSelector : LCDScreen
    {
        private static LCDCatGutChangeSelector _instance;

        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinCutChangeSelector));

        private Button btn_continue;
        private Button btn_repeat;
        private Button btn_end;

        private ProgressBar progress_change_state;

        private TextBlock text_info;
        private TextBlock text_change_main;

        private Constants.FILCHANGE_STATE _stateCatGut;

        private Constants.EXTRUDER _actExt = Constants.EXTRUDER.ExtruderPrimary;
      
        private int oldTempExtPrimar = 0;
        private int oldTempExtSecundar = 0;

        public static bool updateProgTemp = false;  // zabranuje interni refresh progresbaru po prepnuti sync vlaken

        private LCDCatGutChangeSelector()
        {
            setWindow(window);
            Init();
           
        }


        public static LCDCatGutChangeSelector GetInstance()
        {
             if(_instance == null)
                 _instance = new LCDCatGutChangeSelector();

            return _instance;
        }

        public void LoadPrintTemper()
        {
            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Pause)
            {
                oldTempExtPrimar = StateHolder.GetInstance().ActSetTempInPowerBordPrimExt;
                oldTempExtSecundar = StateHolder.GetInstance().ActSetTempInPowerBordSecundExt;
            }

            _actExt = Constants.EXTRUDER.ExtruderPrimary;

            StateHolder.GetInstance().FilChangeCorrect = false;

            InitScreenHeating();

        }

        private void Init()
        {
            btn_continue = (Button) window.GetChildByName("btn_continue");
            btn_continue.PressEvent += btn_change_continue_TapEvent;
            btn_continue.Font = StateHolder.FontUbuntuMiddle;

            btn_repeat = (Button) window.GetChildByName("btn_repeat");
            btn_repeat.PressEvent += btn_change_repeat_PressEvent;
            btn_repeat.Font = StateHolder.FontUbuntuMiddle;

            btn_end = (Button) window.GetChildByName("btn_end");
            btn_end.PressEvent += btn_change_end_TapEvent;
            btn_end.Font = StateHolder.FontUbuntuMiddle;

            text_change_main = (TextBlock) window.GetChildByName("text_main");
            text_change_main.Font = StateHolder.FontUbuntuBig;

            text_info = (TextBlock) window.GetChildByName("text_info");
            //text_info.Font = StateHolder.FontUbuntuSmall;
            text_info.Font = StateHolder.FontUbuntuSmall;
            
            progress_change_state = (ProgressBar) window.GetChildByName("progressBar");

            addUpsBox();

            text_change_main.Text = Resources.GetString(Resources.StringResources.scrCatGutChTxMain);
            
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextContinue);
            btn_end.Text = Resources.GetString(Resources.StringResources.TextEnd);
            btn_repeat.Text = Resources.GetString(Resources.StringResources.TextRepeat);
        }

        private void InitScreenHeating() // ohrev
        {

            _stateCatGut = Constants.FILCHANGE_STATE.Heater;
   
            LCDManager.GetInstance().StartHeatingExt(_actExt, StateHolder.GetInstance().ActSetTempForCatGut);
            
            //text_info.Text = Resources.GetString(Resources.StringResources.scrCatGutChScr1Tx1);
            text_info.Text = Resources.GetString(Resources.StringResources.scrCutPreparePrintHeating);
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextContinue);


            btn_repeat.Visible = false;
            btn_continue.Enabled = false;
            btn_end.Visible = true;

            progress_change_state.Visible = true;   
            UpdateProgressBarTemp();
            
            window.Invalidate();

            updateProgTemp = true;
   
        }

        private void InitScreenUpPrimaryFil()
        {
            _stateCatGut = Constants.FILCHANGE_STATE.UpPrimaryFil;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCutEjectPrimaryFil);
            btn_continue.Visible = false;
            btn_end.Visible = false;

            progress_change_state.Value = 0;

            window.Invalidate();

            _actExt = Constants.EXTRUDER.ExtruderPrimary;

            updateProgFilUp = true;
            ChangeFillProgress(_actExt, "-80","800",true);
            
            //InitScreenUpSecundaryFil();

        }

        private void InitScreenUpSecondaryFil()
        {
            _stateCatGut = Constants.FILCHANGE_STATE.UpSecundaryFil;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCutEjectSecondFil);
            btn_continue.Visible = false;
            btn_end.Visible = false;

            progress_change_state.Value = 0;

            window.Invalidate();

            _actExt = Constants.EXTRUDER.ExtruderSecondary;

            updateProgFilUp = true;
            ChangeFillProgress(_actExt, "-80", "800",true);

        }

        private void InitScreenInsertPrimFil()
        {
            _stateCatGut = Constants.FILCHANGE_STATE.DownPrimaryFil;
            _actExt = Constants.EXTRUDER.ExtruderPrimary;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCutInsertPrimaryFil);
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextOk);
            btn_repeat.Enabled = true;
            btn_repeat.Visible = false;
            progress_change_state.Visible = false;
            btn_end.Visible = true;
            btn_continue.Enabled = true;
            btn_continue.Visible = true;

            progress_change_state.Value = 0;
            

            window.Invalidate();

            
        }

        private void InitScreenDrainOutPrimFil()
        {
            _stateCatGut = Constants.FILCHANGE_STATE.DrainOutPrimFil;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCutFlowsFilFromNozzle);
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextYes);
            btn_repeat.Visible = true;
            btn_repeat.Text = Resources.GetString(Resources.StringResources.TextNo);

            progress_change_state.Visible = true;
            progress_change_state.Value = 0;

            btn_continue.Enabled = false;
            btn_repeat.Enabled = false;

            window.Invalidate();

            _actExt = Constants.EXTRUDER.ExtruderPrimary;

            updateProgFilUp = true;
            ChangeFillProgress(_actExt, "150", "200", true);

        }

        private void InitScreenInsertSecFil()
        {
            _stateCatGut = Constants.FILCHANGE_STATE.DownSecundaryFil;
            _actExt = Constants.EXTRUDER.ExtruderSecondary;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCutInsertSecundFil);
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextOk);

            btn_repeat.Visible = false;

            progress_change_state.Value = 0;
            progress_change_state.Visible = false;

            window.Invalidate();


        }

        private void InitScreenDrainOutSecFil()
        {
            _stateCatGut = Constants.FILCHANGE_STATE.DrainOutSecFil;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCutFlowsFilFromNozzle);
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextYes);
            btn_repeat.Visible = true;
            btn_repeat.Text = Resources.GetString(Resources.StringResources.TextNo);

            progress_change_state.Visible = true;
            progress_change_state.Value = 0;

            btn_continue.Enabled = false;
            btn_repeat.Enabled = false;

            window.Invalidate();

            _actExt = Constants.EXTRUDER.ExtruderSecondary;

            updateProgFilUp = true;
            ChangeFillProgress(_actExt, "150", "200", true);
        }

        private void InitScreenChangeDone()
        {
            _stateCatGut = Constants.FILCHANGE_STATE.Done;

            text_info.Text = Resources.GetString(Resources.StringResources.scrCutExchangeDone);
            btn_continue.Text = Resources.GetString(Resources.StringResources.TextFinish);
            btn_repeat.Visible = false;
            btn_continue.Visible = true;
            btn_end.Visible = false;
            progress_change_state.Visible = false;

            window.Invalidate();
        }



        public void UpdateProgressBar()
        {
            if(_stateCatGut == Constants.FILCHANGE_STATE.Heater)
                UpdateProgressBarTemp();

            if (_stateCatGut == Constants.FILCHANGE_STATE.UpPrimaryFil || _stateCatGut == Constants.FILCHANGE_STATE.UpSecundaryFil || _stateCatGut == Constants.FILCHANGE_STATE.DrainOutPrimFil || _stateCatGut == Constants.FILCHANGE_STATE.DrainOutSecFil)
                UpdateProgressBarFilUp();
           
        }

        private void UpdateProgressBarTemp()
        {
            if (updateProgTemp == false)
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
                actTemp = StateHolder.GetInstance().ActTempSecondaryExt;
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

        private bool updateProgFilUp = false;
        private void UpdateProgressBarFilUp()
        {
            if (updateProgFilUp == false)
                return;

            if (progress_change_state.Value < 100)
            {
                Debug.Print(progress_change_state.Value.ToString());

                double proc = ((double) countTime / countSecTimeFillUp) * 100;

                progress_change_state.Value = (int)proc;

                if (progress_change_state.Value > 100)
                    progress_change_state.Value = 100;

                countTime++;

                window.FillRect(progress_change_state.Rect);
                progress_change_state.Invalidate();

                if (progress_change_state.Value > 50)
                {
                    if (_stateCatGut == Constants.FILCHANGE_STATE.DrainOutPrimFil)
                    {
                        btn_continue.Enabled = true;
                        btn_repeat.Enabled = true;

                        window.FillRect(btn_continue.Rect);
                        btn_continue.Invalidate();

                        window.FillRect(btn_repeat.Rect);
                        btn_repeat.Invalidate();
                        return;
                    }

                    if (_stateCatGut == Constants.FILCHANGE_STATE.DrainOutSecFil)
                    {
                        btn_continue.Enabled = true;
                        btn_repeat.Enabled = true;

                        window.FillRect(btn_continue.Rect);
                        btn_continue.Invalidate();

                        window.FillRect(btn_repeat.Rect);
                        btn_repeat.Invalidate();
                        return;
                    }
                }

                if (progress_change_state.Value == 100)
                {
                    // dalsi stav
                    if (_stateCatGut == Constants.FILCHANGE_STATE.UpPrimaryFil)
                    {
                        InitScreenUpSecondaryFil();
                        return;
                    }

                    if (_stateCatGut == Constants.FILCHANGE_STATE.UpSecundaryFil)
                    {
                        InitScreenInsertPrimFil();
                        updateProgFilUp = false;
                        return;
                    }

                    if (_stateCatGut == Constants.FILCHANGE_STATE.DownPrimaryFil)
                    {
                        updateProgFilUp = false;
                        return;
                    }

                    if (_stateCatGut == Constants.FILCHANGE_STATE.DownSecundaryFil)
                    {
                        updateProgFilUp = false;
                        return;
                    }
        
                }
            }

        }

       
        private int countSecTimeFillUp = 0;
        private int countTime = 0;

        private void ChangeFillProgress(Constants.EXTRUDER actExt, string lenghtMm, string speed, bool move)
        {
            countSecTimeFillUp = 0;
            countTime = 0;

            if (move)
            {
                LCDManager.GetInstance().MoveString(actExt, "0.1", speed);
                LCDManager.GetInstance().MoveString(actExt, "0.1", speed);
                LCDManager.GetInstance().MoveString(actExt, lenghtMm, speed);
            }
            
            int lenght = System.Math.Abs((int)(Convert.ToInt16(lenghtMm)));
            int speedMMss = Convert.ToInt16(speed) / 60;

            double time = lenght / speedMMss;
            countSecTimeFillUp = (int) time;

        }

        private void btn_change_repeat_PressEvent(object sender)
        {
            LCDManager.GetInstance().MoveString(_actExt, "50", "200");
        }

        private void btn_change_continue_TapEvent(object sender)
        {
            Thread.Sleep(200);  // umele spozdeni refreshe okna
            switch (_stateCatGut)
            {
                case Constants.FILCHANGE_STATE.Heater:
                    {
                        updateProgTemp = false;
                        InitScreenUpPrimaryFil();
                        break;
                    }

                case Constants.FILCHANGE_STATE.DownPrimaryFil:
                    {
                        LCDManager.GetInstance().MoveString(_actExt, "0.1", "200");
                        LCDManager.GetInstance().MoveString(_actExt, "0.1", "200");
                        LCDManager.GetInstance().MoveString(_actExt, "150", "200");

                        InitScreenDrainOutPrimFil();

                        break;
                    }
                
                case Constants.FILCHANGE_STATE.DrainOutPrimFil:
                    {
                        LCDManager.GetInstance().MoveStop();
                        LCDManager.GetInstance().MoveString(_actExt, "-60", "800");

                        InitScreenInsertSecFil();

                        break;
                    }

                case Constants.FILCHANGE_STATE.DownSecundaryFil:
                    {
                        LCDManager.GetInstance().MoveString(_actExt, "0.1", "200");
                        LCDManager.GetInstance().MoveString(_actExt, "0.1", "200");
                        LCDManager.GetInstance().MoveString(_actExt, "150", "200");

                        InitScreenDrainOutSecFil();

                        break;
                    }

                case Constants.FILCHANGE_STATE.DrainOutSecFil:
                    {

                        LCDManager.GetInstance().MoveStop();
                        LCDManager.GetInstance().MoveString(_actExt, "-60", "800");

                        InitScreenChangeDone();

                        break;
                    }
                case Constants.FILCHANGE_STATE.Done:
                    {
                        StateHolder.GetInstance().FilChangeCorrect = true;
                        EndProccesCut();
                        break;
                    }
            }
        }

        private void btn_change_end_TapEvent(object sender)
        {
            //countThreadChange = 0;
            //timeSleepChange = 10;

            LCDManager.GetInstance().MoveStop();
            EndProccesCut();
        }

        private void EndProccesCut()
        {
             updateProgTemp = false;
             updateProgFilUp = false;

            if (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Pause)
            {
                LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderPrimary, oldTempExtPrimar);
                LCDManager.GetInstance().StartHeatingExt(Constants.EXTRUDER.ExtruderSecondary, oldTempExtSecundar);
                LCDManager.GetInstance().UpdateScreenLcd(StateScreen.PausePrint, "");
                return;
            }

            //vysunuti se musi dokoncit korektne - move stop volano behem vymeny nikoliv korektnim ukonceni
            //LCDManager.GetInstance().MoveStop();
            LCDManager.GetInstance().StopHeatingExt(Constants.EXTRUDER.ExtruderPrimary);
            LCDManager.GetInstance().StopHeatingExt(Constants.EXTRUDER.ExtruderSecondary);

            if ((Program.Type == Constants.TYPE_PRINTER.DeeRed1_3 || Program.Type == Constants.TYPE_PRINTER.DeeRed2_1 ) && StateHolder.GetInstance().FilChangeCorrect)
            {
                LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Model, "");
                return;
            }

            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Settings, "");
                

        }

    }
}
