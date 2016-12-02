using System;
using Microsoft.SPOT;
using Be3D.Constants;
using System.Threading;

namespace _3D_control_v09
{
    class GcodeManagere
    {
        private static GcodeManagere _instance;

        private static int _countInstruction = 1;
        private static int _cs = 0;
        private static char[] _arr = new char[96];
        private static char _ch = ' ';

        private GcodeManagere()
        {

        }

        public static GcodeManagere GetInstance()
        {
            if (_instance == null)
                _instance = new GcodeManagere();

            return _instance;
        }

        public static string CreateGcodeForMarlin(string gcode)
        {
            gcode = "N" + _countInstruction + " " + gcode + "*";

            _cs = 0;
            _arr = gcode.ToCharArray();

            for (int i = 0; i < _arr.Length; i++)
            {
                _ch = _arr[i];
                if (_ch != '*')
                    _cs = _cs ^ _ch;
            }

            _cs &= 0xff;

            _countInstruction++;

            return gcode + _cs.ToString();
        }

        public static int GetCountInstruction()
        {
            return _countInstruction;
        }

        public static void SetCountInstruction(int value)
        {
            _countInstruction = value;
        }


        public void InitPowerBoard()
        {
            //set units to Millimeters
            string gCode = "G21";
            Program.SendDataToPrinter(gCode);

            SetAbsolutePosition();

        }

        private void SetAbsolutePosition()
        {
            // set to Absolute Position
            string gCode = "G90";
            Program.SendDataToPrinter(gCode);
        }

        private void SetRelativePosition()
        {
            // set to relative Position
            string gCode = "G91";
            Program.SendDataToPrinter(gCode);
        }

        public void SetCurrentPosition(double[] position)
        {
            string gCode = "G92 " + "X" + position[0] + " Y" + position[1] + " Z" + position[2];
            
            if(position[3] != 0.0)
                gCode += " E" + position[3];

            if(position[4] != 0.0)
                gCode += " E" + position[4];

            Program.SendDataToPrinter(gCode);
        }

        public void GetCurrentPosition()
        {
            //M114
            string gCode = "M114";
            Program.SendDataToPrinter(gCode);
        }

        public void GetTachoFilament()
        {
            //M123
            string gCode = "M123";
            Program.SendDataToPrinter(gCode);
        }

        #region TOOLS
        public void GetExtrudersAndBedTemp()
        {
            //M105
            string code = "M105";
            Program.SendDataToPrinter(code);
        }

        public void SetActExtruder(Constants.EXTRUDER extr)
        {
            string code = "";
            if (extr == Constants.EXTRUDER.ExtruderPrimary)
                code = "T0";
            if (extr == Constants.EXTRUDER.ExtruderSecondary)
                code = "T1";

            Program.SendDataToPrinter(code);
        }

        public void SetExtruderTempAndWait(Constants.EXTRUDER extr, int temp)
        {
            //M109 S190 
            string code = "";

            if (extr == Constants.EXTRUDER.ExtruderPrimary)
                code = "M109 T0 S" + temp;

            if (extr == Constants.EXTRUDER.ExtruderSecondary)
                code = "M109 T1 S" + temp;

            Program.SendDataToPrinter(code);
        }

        public void SetExtruderTemp(Constants.EXTRUDER extr, int temp)
        {
            //M104 S190 
            string code = "";

            if (extr == Constants.EXTRUDER.ExtruderPrimary)
                code = "M104 T0 S" + temp;

            if (extr == Constants.EXTRUDER.ExtruderSecondary)
                code = "M104 T1 S" + temp;

            if (extr == Constants.EXTRUDER.ExtruderThird)
                code = "M104 T2 S" + temp;

            Program.SendDataToPrinter(code);
        }

        public void SetBedTemp(int temp)
        {
            //M140 S55
            string code = "M140 S" + temp;
            Program.SendDataToPrinter(code);
        }
        
        public void SetFanSpeed(int speed) 
        {
            string code = "M106 S" + speed;
            Program.SendDataToPrinter(code);
        }

        #endregion

        #region MOVE

        public void MoveStop()
        {
            string gCode = "M124";

            Program.SendDataToPrinter(gCode);

        }

        public void MoveEmergencyStop()
        {
            string gCode = "M112";
            Program.SendDataToPrinter(gCode);

        }

        public void MoveToHome()
        {
            SetAbsolutePosition();
            string code = "G28";
            Program.SendDataToPrinter(code);

        }

        public void GoToPositionChangeFilament()
        {
            //nastaveni Z
            double x = ConfigurationPrinter.GetInstance().GetPrinterSize()[0];
            double y = ConfigurationPrinter.GetInstance().GetPrinterSize()[1];

            double moveZ = Program._basicConfig.LatestPositionExtruder[2] + 20;
            if (moveZ > ConfigurationPrinter.GetInstance().GetPrinterSize()[2])
                moveZ = ConfigurationPrinter.GetInstance().GetPrinterSize()[2];

            string code = "G1 X" + x + " Y"+ y + " Z" + moveZ + " F4000";

            if (Program.Type == Constants.TYPE_PRINTER.DeeRed1_3)
            {
                code = code = "G1 X-35 Y250 Z" + moveZ + " F4000";
            }

            if (Program.Type == Constants.TYPE_PRINTER.DeeRed2_1)
            {
                code = code = "G1 X-35 Y250 Z" + moveZ + " F4000";
            }

            Program.SendDataToPrinter(code);
        }

        public void GoToLastPositionPrint(Constants.EXTRUDER actExtr)
        {
            //struktura double[] { x, y, z, e0, e1 };
            double x = Program._basicConfig.LatestPositionExtruder[0];
            double y = Program._basicConfig.LatestPositionExtruder[1];
            double z = Program._basicConfig.LatestPositionExtruder[2];

            //nastavit extruder
            if (actExtr == Constants.EXTRUDER.ExtruderPrimary)
                Program.SendDataToPrinter("G92 E" + Program._basicConfig.LatestPositionExtruder[3]);
            
            if (actExtr == Constants.EXTRUDER.ExtruderSecondary)
                Program.SendDataToPrinter("G92 E" + Program._basicConfig.LatestPositionExtruder[4]);


            //vratit se na posledni pozici
            string code = "G1 X" + x + " Y" + y + " Z" + z +" F4000";
            Program.SendDataToPrinter(code);




        }

        public void MoveToBedDownInMM(double mm)
        {
            //struktura double[] { x, y, z, e0, e1 };

            Program._basicConfig.LatestPositionExtruder[2] = Program._basicConfig.LatestPositionExtruder[2] + mm;

            if (Program._basicConfig.LatestPositionExtruder[2] > ConfigurationPrinter.GetInstance().GetPrinterSize()[2])
                Program._basicConfig.LatestPositionExtruder[2] = ConfigurationPrinter.GetInstance().GetPrinterSize()[2];


            string code = "G1 Z" + Program._basicConfig.LatestPositionExtruder[2] + " F4000";
            Program.SendDataToPrinter(code);     
        }

        public void MoveToBedUpInMM(double mm)
        {
            //struktura double[] { x, y, z, e0, e1 };

            Program._basicConfig.LatestPositionExtruder[2] = Program._basicConfig.LatestPositionExtruder[2] - mm;
            if (Program._basicConfig.LatestPositionExtruder[2] < 0)
                Program._basicConfig.LatestPositionExtruder[2] = 0;

            string code = "G1 Z" + Program._basicConfig.LatestPositionExtruder[2] + " F4000";
            Program.SendDataToPrinter(code); 
           
        }



        public void MoveToX0Y0Z20()
        {
            string code = "G1 X0 Y0 Z20 F3000";
            Program.SendDataToPrinter(code);
        }

        public void MoveToX0Y0()
        {
            string code = "G1 X0 Y0 F3000";
            Program.SendDataToPrinter(code);
        }

        public void MoveToMaxPosition()
        {
            string code = StateHolder.GetInstance().GetMoveMaxPosition();
            Program.SendDataToPrinter(code);
        }

        public void MotorEnable()
        {
            //M17
            string gCode = "M17";
            Program.SendDataToPrinter(gCode);
        }

        public void MotorDisable()
        {
            //M18
            string gCode = "M18";
            Program.SendDataToPrinter(gCode);
        }

        public void MoveSpeedSpringUp(Constants.EXTRUDER ext, string mm)
        {
            //TODO posun struny ve spravnem extruderu
            SetRelativePosition();

            if (ext == Constants.EXTRUDER.ExtruderPrimary)
            {
                string gCode = "T0";
                Program.SendDataToPrinter(gCode);
            }

            if (ext == Constants.EXTRUDER.ExtruderSecondary)
            {
                string gCode = "T1";
                Program.SendDataToPrinter(gCode);
            }

            if (ext == Constants.EXTRUDER.ExtruderThird)
            {
                string gCode = "T2";
                Program.SendDataToPrinter(gCode);
            }
            //string code = "G1 E-60 F180"; // orig 60 na prani Toma 80
            //string code = "G1 E-80 F200";
            string code = "G1 E" + mm + " F4000";

            Program.SendDataToPrinter(code);

            SetAbsolutePosition();
        }


        public void MoveSpring(Constants.EXTRUDER ext, string mm,string speed)
        {
            SetRelativePosition();

            if (ext == Constants.EXTRUDER.ExtruderPrimary)
            {
                string gCode = "T0";
                Program.SendDataToPrinter(gCode);
            }

            if (ext == Constants.EXTRUDER.ExtruderSecondary)
            {
                string gCode = "T1";
                Program.SendDataToPrinter(gCode);
            }

            if (ext == Constants.EXTRUDER.ExtruderThird)
            {
                string gCode = "T2";
                Program.SendDataToPrinter(gCode);
            }

            string code = "G1 E"+ mm +" F" + speed;
    
            Program.SendDataToPrinter(code);
            SetAbsolutePosition();
        }

        public void MoveParkPosition()
        {
            //sjede smerem dolu o danou delku
            
            string code = "G92 Z0"; //pokud jedeme proti koncaku, muzeme si dovolit, jinak si pøi pøerušeni M124 tiskarna tiskarna mysli ze je na pozici
            Program.SendDataToPrinter(code);

            code = StateHolder.GetInstance().GetMoveParkPosition();   
            Program.SendDataToPrinter(code);

        }

        public void MoveSetSpeed40mm()
        {
            string gCode = "G1 F2400";
            Program.SendDataToPrinter(gCode);
        }

        public void MoveHomePosition()
        {
            SetRelativePosition();

            string code = StateHolder.GetInstance().GetMoveHomeZ();
            Program.SendDataToPrinter(code);

            //nevime v jake poloze jsme, nutne ji nastavit maximalni a zajet do 0,0 - duvodem je nemoznost stopnout G28 pomoci M124
            SetAbsolutePosition();

            GcodeManagere.GetInstance().SetCurrentPosition(ConfigurationPrinter.GetInstance().GetPrinterSize());

            code = StateHolder.GetInstance().GetMoveHomeX();
            Program.SendDataToPrinter(code);

            code = StateHolder.GetInstance().GetMoveHomeY();
            Program.SendDataToPrinter(code);
        }


        #endregion


        public void GoServoAngle(string angle)
        {
            // set to Absolute Position
            string gCode = "M280 P0 S" + angle;
            Program.SendDataToPrinter(gCode);
        }

        public void runServoCycle()
        {
            // set to Absolute Position
            string gCode = "G30";
            Program.SendDataToPrinter(gCode);
        }

    }
}
