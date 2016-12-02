using Be3D.Constants;

namespace _3D_control_v09
{
    class ConfigurationPrinter
    {
        private static ConfigurationPrinter _instance;

        private ConfigurationPrinter()
        {
            
        }

        public static ConfigurationPrinter GetInstance()
        {
            if(_instance == null)
                _instance = new ConfigurationPrinter();

            return _instance;
        }

        public string verisonFWControl = "";
        public string verisonFWPower = "";


        public bool isOffsetMenu()
        {
            return true;
        }

        public string GetNameOfPrinter()
        {
            string result = "";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = "3D printer DeeGreen";
                    break;
                case Constants.TYPE_PRINTER.DeeGreenSer3:
                    result = "3D printer DeeGreen.";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = "3D printer Present DeeGreen";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "3D printer DeeRed";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "3D printer DeeRed";
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "3D printer DeeRed 2";
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = "3D printer DREStolice";
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = "3D printer DGRtolice 3 extr";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = "3D printer Present DeeRed";
                    break;
            
            }
            return result;
        }

        public string GetNameOfManufacterLong()
        {
            return "be3d s.r.o.";          
        }

        public string GetNameOfManufacter()
        {
            return "do-it";
        }

        public string GetNameOfManufacter2()
        {
            return "be3D";
        }

        public string GetLenghtMoveFilUp()
        {
            string result = "-100";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "-80";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "-80";
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "-80";
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = "-90";
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = "-90";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = "-80";
                    break;

            }
            return result;
        }

        public string GetLenghtMoveFilDown()
        {
            string result = "75";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "80";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "20";
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "20";
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = "90";
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = "90";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = "20";
                    break;

            }
            return result;
        }

        public string GetLenghtMoveFilRepeat()
        {
            string result = "5";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "10";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "10";
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "10";
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = "10";
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = "10";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = "10";
                    break;

            }
            return result;
        }

        public string GetTypePrinter()
        {
            string result = "";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = "1.2";
                    break;
                case Constants.TYPE_PRINTER.DeeGreenSer3:
                    result = "1.3";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = "present";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "1.2";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "1.3";
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "2.1";
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = "2.1";
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = "1.3";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = "present";
                    break;

            }
            return result;
        }

        public string GetNameOfTypePrinter()
        {
            string result = "";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = "green";
                    break;
                case Constants.TYPE_PRINTER.DeeGreenSer3:
                    result = "green.";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = "green";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "red";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "red1_3";
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "red2_1";
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = "red2_1";
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = "green.";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = "red";
                    break;
            
            }
            return result;
        }
        
        public Constants.DEVICE_PRINTER GetConnectWithPrinter()
        {
            var result = Constants.DEVICE_PRINTER.UART1;

            return result;
        }

        public Constants.PC GetConnectWithPc()
        {
            var result = Constants.PC.RS232;

            return result;
        }

        public double[] GetPrinterSize()
        {
            double[] result = new double[5] { 150, 150, 150, 0.0, 0.0 };

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = new double[5] { 145, 145, 140, 0.0, 0.0 };
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = new double[5] { 400, 400, 800, 0.0, 0.0 };
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = new double[5] { 400, 400, 800, 0.0, 0.0 };
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = new double[5] { 600, 400, 800, 0.0, 0.0 };
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = new double[5] { 0, 0, 0, 0.0, 0.0 };
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = new double[5] { 0, 0, 0, 0.0, 0.0 };
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = new double[5] { 400, 400, 770, 0.0, 0.0 };
                    break;
                
            }
            return result;
        }

        public string GetHomeX()
        {
            string result = "G1 X5 F3000";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "G1 X395 F3000";   // koncak X je v MAX
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "G1 X395 F3000";   // koncak X je v MAX
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "G1 X595 F3000";   // koncak X je v MAX
                    break;
            }
            return result;
        }

        public string GetHomeY()
        {
            string result = "G1 Y0 F3000";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "G1 Y0 F3000";   
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "G1 Y0 F3000";   
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "G1 Y0 F3000";   
                    break;
            }
            return result;
        }

        public string GetHomeXY()
        {
            string result = "G1 X0 Y0 F3000";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "G1 X400 Y0 F3000";   // koncak X je v MAX
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "G1 X400 Y0 F3000";   // koncak X je v MAX
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "G1 X600 Y0 F3000";   // koncak X je v MAX
                    break;
            }
            return result;
        }

        public string GetHomeZ()
        {
            string result = "G1 Z150 F2400";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "G1 Z350 F2400";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "G1 Z350 F2400";
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "G1 Z350 F2400";
                    break;
            }
            return result;
        }

        public string GetParkPosition()
        {
            string result = "G1 Z150 F3000";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = "G1 Z145 F3000";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "G1 Z400 F3000";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "G1 Z400 F3000";
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "G1 Z400 F3000";
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = "G1 Z400 F3000";
                    break;
           
            }
            return result;
        }

        public string GetMaxPosition()
        {
            string result = "G1 X150 Y150 Z150 F2500";
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = "G1 X145 Y145 Z140 F2500";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = "G1 X390 Y390 Z700 F3000";
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = "G1 X400 Y400 Z770 F3000";
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = "G1 X600 Y400 Z800 F3000";
                    break;
              
            }
            return result;
        }

        public bool IsPresentSwitchDoor1()
        {
            var result = true;

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DREStolice:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = false;
                    break;              
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = false;
                    break;          
            }
            return result;
        }

        public bool IsPresentSwitchDoor2()
        {
            var result = true;

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = false;
                    break;          
            }
            return result;
        }

        public bool IsPresentSwitchFilament1()
        {
            var result = true;

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DREStolice:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = false;
                    break;                
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = false;
                    break;
            }
            return result;
        }

        public bool IsPresentSwitchFilament2()
        {
            var result = false;

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = true;
                    break;

            }

            return result;
        }

        public  bool IsPresentAutoBedLeveling()
        {
            if (Program.Type == Constants.TYPE_PRINTER.DeeRed1_3)
            {
                return false;
            }

            return true;
        }

        public bool IsPresentBedHeat()
        {
            var result = false;
            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DREStolice:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeGreenSer3:
                    result = true;
                    break;
            }

            return result;
        }

        public bool IsPresentSpaceHeat()
        {
            var result = false;

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = true;
                    break;

            }
            return result;
        }

        public bool IsPresentSecondaryExtruder()
        {
            var result = false;

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeGreenSer3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice5:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = true;
                    break;
            }
            return result;
        }

        public bool IsPresentThirdExtruder()
        {
            var result = false;

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeGreenSer3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice5:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = false;
                    break;
            }
            return result;
        }

        public bool IsPresentFourthExtruder()
        {
            var result = false;

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeGreenSer3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice5:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = false;
                    break;
            }
            return result;
        }

        public bool IsPresentFithtExtruder()
        {
            var result = false;

            switch (Program.Type)
            {
                case Constants.TYPE_PRINTER.DeeGreenSer1Ser2:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeGreenSer3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeGreen:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice3:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DGRStolice5:
                    result = true;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_2:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed1_3:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DeeRed2_1:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.DREStolice:
                    result = false;
                    break;
                case Constants.TYPE_PRINTER.PresentDeeRed:
                    result = false;
                    break;
            }
            return result;
        }
    }
}
