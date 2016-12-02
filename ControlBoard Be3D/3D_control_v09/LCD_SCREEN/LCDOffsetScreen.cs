using Be3D.Constants;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using System;
using System.Threading;


namespace _3D_control_v09
{
    internal class LCDOffsetScreen : LCDScreen
    {
        private static LCDOffsetScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinSettingOffset));

        private Button bt_back = (Button)window.GetChildByName("btn_back");
        private TextBlock text_shutdown_main = (TextBlock)window.GetChildByName("text_main");
        private TextBlock text_info1 = (TextBlock)window.GetChildByName("text_info1");
        private TextBlock text_offsetScreen = (TextBlock)window.GetChildByName("text_offset");

        private Button bt_save = (Button)window.GetChildByName("bt_save");
        private Button bt_testPrint = (Button)window.GetChildByName("bt_testPrint");
        private Button bt_reload = (Button)window.GetChildByName("bt_reload");
        private Button bt_p0_01 = (Button)window.GetChildByName("bt_+0_01");
        private Button bt_m0_01 = (Button)window.GetChildByName("bt_-0_01");
        private Button bt_p0_1 = (Button)window.GetChildByName("bt_+0_1");
        private Button bt_m0_1 = (Button)window.GetChildByName("bt_-0_1");

        private LCDOffsetScreen()
        {
            setWindow(window);
            InitScreen();
        }

        public static LCDOffsetScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDOffsetScreen();

            return _instance;
        }

        private void InitScreen()
        {
            //***************   Shutdown window    **************************         
            bt_back.TapEvent += new OnTap(btn_shutdown_back_TapEvent);
            bt_back.Font = StateHolder.FontUbuntuMiddle;

            text_shutdown_main.Font = StateHolder.FontUbuntuBig;
            text_info1.Font = StateHolder.FontUbuntuSmall;
            text_offsetScreen.Font = StateHolder.FontUbuntuSmall;

            bt_save.TapEvent += new OnTap(btn_save_TapEvent);
            bt_save.Font = StateHolder.FontUbuntuMiddle;
         
            bt_testPrint.TapEvent += new OnTap(btn_testPrint_TapEvent);
            bt_testPrint.Font = StateHolder.FontUbuntuMiddle;

            
            bt_reload.TapEvent += new OnTap(btn_reload_TapEvent);
            bt_reload.Font = StateHolder.FontUbuntuMiddle;
  
            bt_p0_01.TapEvent += new OnTap(btn_p0_01_TapEvent);
            bt_p0_01.Font = StateHolder.FontUbuntuMiddle;
            
            bt_m0_01.TapEvent += new OnTap(btn_m0_01_TapEvent);
            bt_m0_01.Font = StateHolder.FontUbuntuMiddle;
           
            bt_p0_1.TapEvent += new OnTap(btn_p0_1_TapEvent);
            bt_p0_1.Font = StateHolder.FontUbuntuMiddle;
           
            bt_m0_1.TapEvent += new OnTap(btn_m0_1_TapEvent);
            bt_m0_1.Font = StateHolder.FontUbuntuMiddle;

            bt_back.Text = Resources.GetString(Resources.StringResources.TextBack);           
            text_shutdown_main.Text = Resources.GetString(Resources.StringResources.scrSettingOffsetTxMain);
            text_info1.Text = Resources.GetString(Resources.StringResources.scrSettingOffsetInfo1);
           
            bt_save.Text = Resources.GetString(Resources.StringResources.scrSettingOffsetBtSave);
            bt_testPrint.Text = "Test offset";
            bt_reload.Text = Resources.GetString(Resources.StringResources.scrSettingOffsetBtReload);

            addUpsBox();

            window.Render();

        }

        public void RefreshOffset()
        {
            Program.SendDataToPrinter("M565");
        }

        public void UpdateOffset()
        {
            UpdateActOffset(StateHolder.GetInstance().offsetZ.ToString("F2") + "");       
        }

        private void UpdateActOffset(string offset)
        {
            text_offsetScreen.Text = offset;
           
            window.FillRect(text_offsetScreen.Rect);
            text_offsetScreen.Invalidate();
        }

        private void btn_reload_TapEvent(object sender)
        {
            Program.SendDataToPrinter("M565");
        }

        private void btn_save_TapEvent(object sender)
        {
            if (text_offsetScreen.Text == Constants.strNoOffset)
                return;

            Program.SendDataToPrinter("M565 Z" + StateHolder.GetInstance().offsetZ);
            Program.SendDataToPrinter("M500");

            StateHolder.GetInstance().offsetZ = 0.0;
            UpdateOffset();

        }

        private void btn_testPrint_TapEvent(object sender)
        {
            //ulozi hodnotu do eeprom
            if (text_offsetScreen.Text != "--")
            {
                Program.SendDataToPrinter("M565 Z" + StateHolder.GetInstance().offsetZ);
                Program.SendDataToPrinter("M500");
            }

            // jsou uzavrene dvere1 nebo 2
            if (SwitchManager.GetInstance().IsOpenDoor1() || SwitchManager.GetInstance().IsOpenDoor2())
            {
                string message = Resources.GetString(Resources.StringResources.TextOpenDoorOrCover);
                ModalResult result = Glide.MessageBoxManager.Show(message, "Warning", ModalButtons.Ok);
                Thread.Sleep(1);

                if (result == ModalResult.Ok)
                {
                    return;
                }
            }

            //spusti tisk
            StateHolder.GetInstance().ActState = Be3D.Constants.Constants.ACTUAL_STATE.Print;
            StateHolder.GetInstance().ActPrintState = Be3D.Constants.Constants.PRINT_STATE.Init;    // pri otevreni tveri hard reset elektroniky

            StateHolder.GetInstance().PrintFile = "Test offset.gco";
            Program.PrintThreadBufferActive = true;

            LCDManager.GetInstance().UpdateScreenLcd(LCDScreen.StateScreen.PrintInfo, "");

            Thread.Sleep(500);

            Program.GetBufferUartSenderPrinter().Enqueue("G21");
            Program.GetBufferUartSenderPrinter().Enqueue("G90");
            Program.GetBufferUartSenderPrinter().Enqueue("M104 S220");
            Program.GetBufferUartSenderPrinter().Enqueue("G28");
            Program.GetBufferUartSenderPrinter().Enqueue("G29");
            Program.GetBufferUartSenderPrinter().Enqueue("M109 S220");
            Program.GetBufferUartSenderPrinter().Enqueue("T0");
            Program.GetBufferUartSenderPrinter().Enqueue("G92 E0");


            double[] maxPlatformSizeXYZ = ConfigurationPrinter.GetInstance().GetPrinterSize();
            double uhlopricka = Math.Sqrt((maxPlatformSizeXYZ[0] * maxPlatformSizeXYZ[0]) + (maxPlatformSizeXYZ[1] * maxPlatformSizeXYZ[1]));
            double magicConstant = 0.03018224359807617;


            string x = "0";
            string y = "0";
            string z = "0.1";
            string e = "0";
            string f = "2400";

            //X0,Y0
            Program.GetBufferUartSenderPrinter().Enqueue("G1 X" + x + " Y" + y + " Z" + z + " E" + e + " F" + f);

            //X150 Y150
            x = "" + maxPlatformSizeXYZ[0];
            y = "" + maxPlatformSizeXYZ[1];
            e = "" + (uhlopricka * magicConstant);

            Program.GetBufferUartSenderPrinter().Enqueue("G1 X" + x + " Y" + y + " Z" + z + " E" + e + " F" + f);
            Program.GetBufferUartSenderPrinter().Enqueue("G92 E0");


            //x150 y0
            x = "" + maxPlatformSizeXYZ[0];
            y = "0";
            e = "" + (maxPlatformSizeXYZ[1] * magicConstant);

            Program.GetBufferUartSenderPrinter().Enqueue("G1 X" + x + " Y" + y + " Z" + z + " E" + e + " F" + f);
            Program.GetBufferUartSenderPrinter().Enqueue("G92 E0");

            //x0 y150
            x = "0";
            y = "" + maxPlatformSizeXYZ[1];
            e = "" + (uhlopricka * magicConstant); ;

            Program.GetBufferUartSenderPrinter().Enqueue("G1 X" + x + " Y" + y + " Z" + z + " E" + e + " F" + f);
            Program.GetBufferUartSenderPrinter().Enqueue("G92 E0");

            //x0 y0
            x = "0";
            y = "0";
            e = "" + (maxPlatformSizeXYZ[1] * magicConstant);

            Program.GetBufferUartSenderPrinter().Enqueue("G1 X" + x + " Y" + y + " Z" + z + " E" + e + " F" + f);
            Program.GetBufferUartSenderPrinter().Enqueue("G92 E0");


            Program.GetBufferUartSenderPrinter().Enqueue("G1 Z100");
            Program.GetBufferUartSenderPrinter().Enqueue("M104 S0");

        }


        private void btn_p0_01_TapEvent(object sender)
        {
            if (text_offsetScreen.Text == Constants.strNoOffset)
                return;

            StateHolder.GetInstance().offsetZ += 0.01;
            UpdateActOffset("" + StateHolder.GetInstance().offsetZ.ToString("F2"));
        }

        private void btn_m0_01_TapEvent(object sender)
        {
            if (text_offsetScreen.Text == Constants.strNoOffset)
                return;

            StateHolder.GetInstance().offsetZ -= 0.01;
            UpdateActOffset("" + StateHolder.GetInstance().offsetZ.ToString("F2"));
        }

        private void btn_p0_1_TapEvent(object sender)
        {
            if (text_offsetScreen.Text == Constants.strNoOffset)
                return;

            StateHolder.GetInstance().offsetZ += 0.1;
            UpdateActOffset("" + StateHolder.GetInstance().offsetZ.ToString("F2"));
        }

        private void btn_m0_1_TapEvent(object sender)
        {
            if (text_offsetScreen.Text == Constants.strNoOffset)
                return;

            StateHolder.GetInstance().offsetZ -= 0.1;
            UpdateActOffset("" + StateHolder.GetInstance().offsetZ.ToString("F2"));
        }

        private void btn_shutdown_back_TapEvent(object sender)
        {
            Program.HardwareResetPrinter();

            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
        }
    }
}
