using Be3D.Constants;
using System;

namespace _3D_control_v09
{
    [Serializable]
    public class BasicConfiguration
    {
        /*******************************************************************************************************************
         * 6.5.2014 NEUPRAVOVAT!!!! - nepřidávat a nemazat proměnné ani funkce, jinak dojde při updatu FW k přemazání původních dat 
        ********************************************************************************************************************/
        /*
         * 23.7.2014 - upravena funkce ConvertToString - a to o osetreni "" !!! 
         */
        
        public const string StrNull = "null";
        public const char ChDollar = '$';
        public const string StrUnknown = "unknown";

        //Basic variable
        //public Constants.LANGUAGE LangSelected = Constants.LANGUAGE.Czech;
        public Constants.LANGUAGE LangSelected = Constants.LANGUAGE.English;

        

        private string _strSerialNumber = new string(ChDollar, 20); // "unknown";
        private string _strIdCart1 = new string(ChDollar, 20); //"null";
        private string _strIdCart2 = new string(ChDollar, 20); //"null";
        public TimeSpan OperationTime = new TimeSpan(0, 0, 1);

        public int DefTmpForCatGut = 230;   //215 omezeno v kodu
        public int DefTmpRefSpace = 0;
        public int DefTmpRefExtr1 = 180;
        public int DefTmpRefExtr0 = 180;
        public int DefTmpRefBed = 60;
        
        public double[] LatestPositionExtruder = new double[5] {0.0, 0.0, 0.0, 0.0, 0.0};
        public int PostitionsInPrintfileOnSd = 0; 

        // Reserve variable
        private string _strReserve1 = new string(ChDollar, 20);  // zabrano pro UPS Manager na ulozeni nazvu souboru
        private string _strReserve2 = new string(ChDollar, 20);

        public int IntReserve1 = 0;     // zabrano pro UPS Manager - teplota trysky 1
        public int IntReserve2 = 0;     // zabrano pro UPS Manager - teplota trysky 2

        public double DblReserve1 = 0.0; 
        public double DblReserve2 = 0.0;

        public TimeSpan ReserveTime1 = new TimeSpan(0, 0, 0);
        public TimeSpan ReserveTime2 = new TimeSpan(0, 0, 0);

        

        public string GetSerialNumber()
        {
            string newstr = convertToString(_strSerialNumber);
            if (newstr != "")
                return newstr;

            return StrUnknown;
        }

        public void SetSerialNumber(string str)
        {
            _strSerialNumber = convertToBasic(str);
        }

        public string GetIdCart1()
        {
            string newstr = convertToString(_strIdCart1);
            if (newstr != "")
                return newstr;
            return StrNull;
        }

        public void SetIdCart1(string str)
        {
            _strIdCart1 = convertToBasic(str);
        }

        public string GetIdCart2()
        {
            string newstr = convertToString(_strIdCart2);
            if (newstr != "")
                return newstr;
            return StrNull;
        }

        public void SetIdCart2(string str)
        {
            _strIdCart2 = convertToBasic(str);
        }


        public string GetStrReserve1()
        {
            string newstr = convertToString(_strReserve1);
            if (newstr != "")
                return newstr;
            return StrNull;
        }

        public void SetStrReserve1(string str)
        {
            _strReserve1 = convertToBasic(str);
        }

        public string GetStrReserve2()
        {
            string newstr = convertToString(_strReserve2);
            if (newstr != "")
                return newstr;
            return StrNull;
        }

        public void SetStrReserve2(string str)
        {
            _strReserve2 = convertToBasic(str);
        }


        private string convertToBasic(string str)
        {
            string newStr = "";

            int indx = 0;
            while (newStr.Length != 20)
            {
                if (str.Length > indx)
                {
                    if (str[indx] != ChDollar)
                        newStr += str[indx];
                }
                else
                    newStr += ChDollar;
                indx++;
            }
            return newStr;
        }

        private string convertToString(string str)
        {
            string newstr = "";

            if (str == null || str == "")
                return "";

            try
            {
                for (int i = 0; i < 20; i++)
                {
                    if (str[i] != ChDollar)
                        newstr += str[i];
                }
            }
            catch (Exception)
            {
                return "";
            }
            return newstr;
        }
    }
}