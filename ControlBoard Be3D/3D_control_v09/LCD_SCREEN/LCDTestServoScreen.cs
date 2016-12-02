
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using _3D_control_v09;
using System.Threading;

namespace _3D_control_v09
{
    class LCDTestServoScreen : LCDScreen
    {
        private static LCDTestServoScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinTestServo));

        private Button btn_back = (Button)window.GetChildByName("btn_back");

        private Button btn_up = (Button)window.GetChildByName("btn_up");
        private Button btn_down = (Button)window.GetChildByName("btn_down");
        private Button btn_50cycles = (Button)window.GetChildByName("btn_50cycles");
        private Button btn_infinity = (Button)window.GetChildByName("btn_infinity");

        private TextBlock txb_50cycles = (TextBlock)window.GetChildByName("txb_50cycles");
        private TextBlock txb_infinity = (TextBlock)window.GetChildByName("txb_infinity");
        private TextBlock txb_main = (TextBlock)window.GetChildByName("txb_main");


        private LCDTestServoScreen()
        {
            setWindow(window);
            InitScreen();
        }

        public static LCDTestServoScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDTestServoScreen();

            return _instance;

        }

        private void InitScreen()
        {
            //***************   Shutdown window    **************************

            btn_back.TapEvent += new OnTap(btn_back_TapEvent);
            btn_back.Font = StateHolder.FontUbuntuMiddle;

            btn_up.TapEvent += new OnTap(btn_up_TapEvent);
            btn_up.Font = StateHolder.FontUbuntuMiddle;

            btn_down.TapEvent += new OnTap(btn_down_TapEvent);
            btn_down.Font = StateHolder.FontUbuntuMiddle;

            btn_50cycles.TapEvent += new OnTap(btn_50cycles_TapEvent);
            btn_50cycles.Font = StateHolder.FontUbuntuMiddle;

            btn_infinity.TapEvent += new OnTap(btn_infinity_TapEvent);
            btn_infinity.Font = StateHolder.FontUbuntuMiddle;

            txb_50cycles.Text = "0";
            txb_infinity.Text = "0";

            txb_50cycles.Visible = false;
            txb_infinity.Visible = false;

            txb_50cycles.Font = StateHolder.FontUbuntuSmall;
            txb_infinity.Font = StateHolder.FontUbuntuSmall;
            txb_main.Font = StateHolder.FontUbuntuBig;


            btn_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            
            addUpsBox();
            window.Render();
        }

        private void btn_up_TapEvent(object sender)
        {
            GcodeManagere.GetInstance().GoServoAngle("90");
        }
        private void btn_down_TapEvent(object sender)
        {
            GcodeManagere.GetInstance().GoServoAngle("180");
        }
        private void btn_50cycles_TapEvent(object sender)
        {
            if (runCycle == true)
            {
                btn_infinity.Text = "infinity";
                btn_50cycles.Text = "50x";
                runCycle = false;

                btn_50cycles.Invalidate();
                btn_infinity.Invalidate();

                return;
            }

            runCycle = true;
            countCycle = 0;
            numberOfCycle = 50;
            new Thread(runThreadCycleServo).Start();

            btn_50cycles.Text = "STOP";
            btn_infinity.Text = "STOP";


            btn_50cycles.Invalidate();
            btn_infinity.Invalidate();
        }
        private void btn_infinity_TapEvent(object sender)
        {
            if (runCycle == true)
            {
                btn_infinity.Text = "infinity";
                btn_50cycles.Text = "50x";
                runCycle = false;


                btn_50cycles.Invalidate();
                btn_infinity.Invalidate();
                return;
            }

            runCycle = true;
            countCycle = 0;
            numberOfCycle = int.MaxValue;
            new Thread(runThreadCycleServo).Start();

            btn_50cycles.Text = "STOP";
            btn_infinity.Text = "STOP";

            btn_50cycles.Invalidate();
            btn_infinity.Invalidate();
        }

        private void btn_back_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(LCDScreen.StateScreen.Tests, "");
        }

        bool runCycle = false;
        int numberOfCycle = 50;
        int countCycle = 0;

        private void runThreadCycleServo() {

            while(runCycle) {
                if (numberOfCycle == 0)
                    runCycle = false;
                GcodeManagere.GetInstance().runServoCycle();
                Thread.Sleep(4000);
                numberOfCycle--;
                countCycle++;

                txb_50cycles.Text = countCycle.ToString();
                txb_infinity.Text = countCycle.ToString();

            }

            runCycle = false;
            Program.HardwareResetPrinter1();
            Program.HardwareResetPrinter2();
        }

      
    }
}
