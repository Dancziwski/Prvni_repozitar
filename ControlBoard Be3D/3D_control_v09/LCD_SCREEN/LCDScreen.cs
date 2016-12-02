using _3D_control_v09;
using GHI.Glide.Display;
using GHI.Glide.UI;

namespace LCD
{
    abstract class LCDScreen
    {
        private Window _window = null;
        public TextBlock text_ups;
     
        public enum StateScreen
        {
            Main = 1,
            Info = 2,
            Settings = 3,
            ShutDown = 4,
            Model = 5,
            SdMissing = 6,
            Language = 7,
            CalibrationPark = 9,           
            CartrigeInfo = 10,
            CatGutChange = 14,
            PausePrint = 20,
            Preheat = 22,
            PrintInfo = 23,
            SettingAnother = 24,
            SettingState = 25,
            SettingTemp = 26,
            Stop = 27,
            SettingOffset = 28,
            UpdateFw = 29,
            WaitingPause = 30,
            MainStolice = 31,
            CatGutChangeTestExtruding = 32,
            Tests = 33,
            TestServo = 34,
        }

        public Window getWindow()
        {
            return _window;
        }

        public void setWindow(Window window)
        {
            _window = window;
        }

        public void addUpsBox()
        {
            if (_window == null)
                return;
            text_ups = new TextBlock("logo_ups",0xff,270,0,50,28);
            text_ups.Name = "logo_ups";
            text_ups.X = 260;
            text_ups.Y = 10;
            text_ups.Width = 50;
            text_ups.Height = 28;
            text_ups.Text = "UPS";
            text_ups.TextAlign = GHI.Glide.HorizontalAlignment.Center;
            text_ups.TextVerticalAlign = GHI.Glide.VerticalAlignment.Middle;
            text_ups.Font = StateHolder.FontUbuntuSmall;
            text_ups.FontColor = 0;
            text_ups.BackColor = Microsoft.SPOT.Presentation.Media.ColorUtility.ColorFromRGB(0xff, 0x00, 0x00);
            text_ups.ShowBackColor = true;
            text_ups.Visible = false;

            _window.AddChild(text_ups);
        }

        public void UpdateStateUPS(bool warning)
        {
            if (_window == null || text_ups == null )
                return;

            if (warning)
            {
                text_ups.Visible = true;
                _window.Invalidate();
            }
            else
            {
                if (text_ups.Visible != false)
                {
                    text_ups.Visible = false;
                    _window.Invalidate();
                }
            }
        }

       
    }

}
