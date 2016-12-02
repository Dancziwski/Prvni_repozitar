
namespace Be3D.Constants
{

    public class Constants
    {
        public const int TempOptionSpace1 = 0;
        public const int TempOptionSpace2 = 40;
        public const int TempOptionSpace3 = 60;

        public const int TempOptionBed1 = 40;
        public const int TempOptionBed2 = 50;
        public const int TempOptionBed3 = 70;

        public const int TempOptionExt1 = 180;
        public const int TempOptionExt2 = 200;
        public const int TempOptionExt3 = 220;

        public const string Prefix1 = "GCO";
        public const string StrNull = "null";

        public const char EndCharRs232 = ';';

        public const char chNln = '\n';
        public const char chDlr = '$';

        public const string strStC = "°C";

        public const string strFailTemp = "--°C";
        public const string strNoOffset = "--";



        #region ENUM CONSTANT


        public enum UPS
        {
            OK = 0,
            NO = 1,
        }

        public enum TYPE_PRINTER
        {
            DeeRed1_2 = 1,
            DeeGreenSer1Ser2 = 2,
            DeeGreenSer3 = 3,
            PresentDeeGreen = 5,
            PresentDeeRed = 6,
            DeeRed1_3 = 7,
            DeeRed2_1 = 8,
            DREStolice = 9,
            DGRStolice3 = 10,
            DGRStolice5 =11,
        }

        public enum ACTUAL_STATE
        {
            Idle = 1,
            Print = 2,
            ParkingPosition = 3,
            MotionTest = 4,
            ShutDown = 5
        }

        public enum CARTRIGE
        {
            NULL = 0,
            CartExtrRight = 1,
            CartExtrLeft = 2
        }
       
        public enum PRINT_STATE
        {
            Init = 1,
            Print = 2,
            Pause = 3,
            Stop = 4,
            Done = 5,      
        }
    
        public enum EXTRUDER
        {
            ExtruderPrimary = 0,
            ExtruderSecondary = 1,
            ExtruderThird = 2,
        }

        public enum MATERIAL
        {
            ABS = 1,
            PLA = 2,
            PVA = 3,
            WOOD = 4,
        }

        public enum ENDSWITCH
        {
            DOOR = 1,
            CEILING = 2,
        }

        public enum DEVICE_PRINTER
        {
            USB = 1,
            UART = 2,
        }

        public enum PC
        {
            RS232 = 1,
            ETH = 2,
        }

      
        public enum LANGUAGE
        {
            Czech = 0,
            English = 1,
            Deutsh = 2,
            Espanol = 3,
            Francais = 4,
            Italiano = 5,
        }

        public enum FILCHANGE_STATE
        {
            SelectExtr = 1,
            Heater = 2,
            IsFilament = 3,
            
            UpFilament = 4,
            UpPrimaryFil = 9,
            UpSecundaryFil=10,

            DownFilament = 6,
            DownPrimaryFil = 7,
            DownSecundaryFil = 8,

            DrainOutPrimFil = 11,
            DrainOutSecFil = 12,

            Done = 13

        }

        #endregion

        public static bool IsCharInName(string name, char ch)
        {
            if (name == "")
                return false;

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == ch)
                    return true;
            }
            return false;
        }

    }

}
