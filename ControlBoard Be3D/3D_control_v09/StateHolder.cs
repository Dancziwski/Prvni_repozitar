using System;
using System.Collections;
using Microsoft.SPOT;
using Be3D.Constants;

namespace _3D_control_v09
{
    
    public class StateHolder
    {
        private static StateHolder _instance;

        #region font

       // public static Font FontUbuntu = Resources.GetFont(Resources.FontResources.ubuntu2);
       // public static Font FontUbuntuBig = Resources.GetFont(Resources.FontResources.ubuntu_big2);
       // public static Font FontUbuntuSmall = Resources.GetFont(Resources.FontResources.ubuntu_small2);


        public static Font FontUbuntuBig = ResGUI.GetFont(ResGUI.FontResources.ubuntu_bold_20_utf8);
        public static Font FontUbuntuMiddle = ResGUI.GetFont(ResGUI.FontResources.ubuntu_bold_17_utf8);
        public static Font FontUbuntuSmall = ResGUI.GetFont(ResGUI.FontResources.ubuntu_medium_16_utf8);


        #endregion

        public static StateHolder GetInstance()
        {
            if(_instance == null) 
                _instance = new StateHolder();

            return _instance;
        }

        private StateHolder()
        {
           
        }

        public string PrintFile = "";
  

        public Constants.ACTUAL_STATE ActState = Constants.ACTUAL_STATE.Idle;
        public Constants.PRINT_STATE ActPrintState = Constants.PRINT_STATE.Stop;

        public bool StateHeating = false;
        //public bool ConnectionsPowerBoard = false;
        public bool StatePauseFromPrinting = false;
        public bool FileDataTransfer = false;

        public string FreeSpaceSDCard = "null";
        
        //nastavene hodnoty teploty v systemu
        //public int ActSetTempForCatGut = Program.BasicConfig.DefTmpForCatGut;
        public int ActSetTempForCatGut = 215;

        public int ActSetTempPrimary = Program._basicConfig.DefTmpRefExtr0;
        public int ActSetTempSecondary = Program._basicConfig.DefTmpRefExtr1;
        public int ActSetTempThird = Program._basicConfig.DefTmpRefExtr1; // každý další extruder má stejnou hodnotu teploty jako extruder 2
        public int ActSetTempSpace = Program._basicConfig.DefTmpRefSpace;
        public int ActSetTempBed = Program._basicConfig.DefTmpRefBed;
        
        public int ActSetTempInPowerBordPrimExt = 0;
        public int ActSetTempInPowerBordSecundExt = 0;
        public int ActSetTempInPowerBordThirdExt = 0;

        //aktualni hodnoty teploty
        public int ActTempPrimaryExt = 0;
        public int ActTempSecundaryExt = 0;
        public int ActTempThirdExt = 0;
        public int ActTempSpace = 0;
        public int ActTempBed = 0;

        public bool IsMountingSD = false;
        public bool ReadingFileListFromSD = false;
        public bool ReadingParametrsFromFile = false;
        public bool DataLoadingFromSD = false;      // seznam souboru nacten a ukoncen
        public bool DataParametersLoadingFromSD = false; // parametry Gcode souboru nacteny
        public bool FilChangeCorrect = false;


        private int _valuePrint = 0;
        private int _valuePrintMax = 0;

        private ArrayList _listFiles = null;     //seznam souboru
        private ArrayList _listParametersFile = null;    // seznam parametru y Gcode souboru

        private string _capacite = "";
        private string _freeCapacite = "";

        public double offsetZ = 0.0;

        public void AddParametersToListFiles(string data)
        {
            if (_listParametersFile == null)
                _listParametersFile = new ArrayList();

            _listParametersFile.Add(data);

        }

        public void ClearParametersFromListFiles()
        {
            if (_listParametersFile == null)
                _listParametersFile = new ArrayList();

            _listParametersFile.Clear();
        }

        public void SetListFiles(ArrayList list)
        {
            _listFiles = list;
        }

        public void AddFileToListFiles(string name)
        {
            if (_listFiles == null)
                _listFiles = new ArrayList();

            if (name == null || name == "")
                return;

            string[] file;

            file = name.Split('.');
            if (file.Length == 2)
            {
                if (file[0].IndexOf('/') > -1)
                    return;

                if (file[1] == Constants.Prefix1)
                {
                    _listFiles.Add(file[0]);
                }
            }
        }

        public void ClearFilesFromListFiles()
        {
            if (_listFiles == null)
                return;

            _listFiles.Clear();
        }

        public ArrayList GetListParametersFile()
        {
            return _listParametersFile;
        }

        public ArrayList GetListFiles()
        {
            return _listFiles;
        }

        public bool ComparePositionXY(double[] newPoz, double[] oldPoz)
        {
            int xnew = (int)(newPoz[0] * 10);            
            int xold = (int)(oldPoz[0] * 10);
            
            if (xnew == xold)
            {
                int ynew = (int)(newPoz[1] * 10);
                int yold = (int)(oldPoz[1] * 10);

                if (ynew == yold)
                    return true;
            }

            return false;
        }

        public bool ComparePosition(double[] newPoz, double[] oldPoz) 
        {
            if (newPoz.Length != oldPoz.Length)
                return false;

            for (int i = 0; i < newPoz.Length; i++)
            {
                if (newPoz[i] != oldPoz[i])
                    return false;
            }

            return true;
        }

        public string GetCapacitySd()
        {
            return _capacite + " MB";
        }

        public string GetFreeSapceSd()
        {
            return _freeCapacite + " MB";
        }

        public string GetMoveHomeX()
        {
            return ConfigurationPrinter.GetInstance().GetHomeX();
        }

        public string GetMoveHomeY()
        {
            return ConfigurationPrinter.GetInstance().GetHomeY();
        }

        public string GetMoveHomeXY()
        {
            return ConfigurationPrinter.GetInstance().GetHomeXY();
        }

        public string GetMoveHomeZ()
        {
            return ConfigurationPrinter.GetInstance().GetHomeZ();
        }

        public string GetMoveParkPosition()
        {
            return ConfigurationPrinter.GetInstance().GetParkPosition();
        }

        public string GetMoveMaxPosition()
        {
            return ConfigurationPrinter.GetInstance().GetMaxPosition();
        }

        #region language

        public Hashtable TabLangNameOfPrefix = new Hashtable()
            {
                {Constants.LANGUAGE.Czech, "Czech"},
                {Constants.LANGUAGE.English, "English"},
                {Constants.LANGUAGE.Deutsh, "Deutsch"},
                {Constants.LANGUAGE.Espanol, "Espanol"},
                {Constants.LANGUAGE.Francais, "Francais"},
                {Constants.LANGUAGE.Italiano, "Italiano"},
              
            };

        public Hashtable TabPrefixOfLang = new Hashtable()
            {
                {Constants.LANGUAGE.Czech, "cz"},
                {Constants.LANGUAGE.English, "en"},
                {Constants.LANGUAGE.Deutsh, "de"},
                {Constants.LANGUAGE.Espanol, "es"},
                {Constants.LANGUAGE.Francais, "fr"},
                {Constants.LANGUAGE.Italiano, "it"},

            };

        public string GETCulturePrefix()
        {
            if (TabPrefixOfLang.Contains(Program._basicConfig.LangSelected))
                return (string)TabPrefixOfLang[Program._basicConfig.LangSelected];

            return null;

        }


        #endregion

        public void SETValuePrint(int value)
        {
            _valuePrint = value;
        }

        public int GETValuePrint()
        {
            return _valuePrint;
        }

        public void SETValuePrintMax(int value)
        {
            _valuePrintMax = value;
        }

        public int GETValuePrintMax()
        {
            return _valuePrintMax;
        }

        public string[] SeparateTextTo4Line(int numOfChar, string mess)
        {
            int numDiv = mess.Length/numOfChar;
            string[] separate = new string[4];

            if (numDiv == 0)
            {
                separate[0] = mess;
                separate[1] = "";
                separate[2] = "";
                separate[3] = "";
            }

            if (numDiv >= 1)
                {
                    separate[0] = mess.Substring(0, numOfChar);
                    if (mess.Length > numDiv)
                        separate[1] = mess.Substring(numOfChar);
                    else
                        separate[1] = "";

                    separate[2] = "";
                    separate[3] = "";
                }
                if (numDiv >= 2)
                {
                    separate[0] = mess.Substring(0, numOfChar);
                    separate[1] = mess.Substring(numOfChar, mess.Length - numOfChar);
                    if (mess.Length > numDiv*2)
                        separate[2] = mess.Substring(2 * numOfChar);
                    else
                        separate[2] = "";

                    separate[3] = "";
                }
                if (numDiv >= 3)
                {
                    separate[0] = mess.Substring(0, numOfChar);
                    separate[1] = mess.Substring(numOfChar, mess.Length - numOfChar);
                    separate[2] = mess.Substring(2 * numOfChar, mess.Length - numOfChar * 2);                  
                    if(mess.Length > numDiv*2)
                        separate[3] = mess.Substring(3 * numOfChar);
                    else
                        separate[3] = "";
                }
                if (numDiv >= 4)
                {
                    separate[0] = mess.Substring(0, numOfChar);
                    separate[1] = mess.Substring(numOfChar, mess.Length - numOfChar);
                    separate[2] = mess.Substring(2 * numOfChar, mess.Length - numOfChar * 2);
                    separate[3] = mess.Substring(3 * numOfChar, mess.Length - numOfChar * 3);
                }
            return separate;
        }
    }
}
