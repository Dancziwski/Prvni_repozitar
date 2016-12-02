
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using Be3D.Constants;
using System;
using System.Collections;

namespace _3D_control_v09
{
    class LCDModelScreen : LCDScreen
    {
        private static LCDModelScreen _instance;

        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinModel));

        private static Button btn_model_prev = (Button)window.GetChildByName("btn_prev");
        private static Button btn_model_next = (Button)window.GetChildByName("btn_next");
        private static TextBlock text_model_file_1 = (TextBlock)window.GetChildByName("text_file1");
        private static TextBlock text_model_file_2 = (TextBlock)window.GetChildByName("text_file2");
        private static TextBlock text_model_file_3 = (TextBlock)window.GetChildByName("text_file3");
        private static TextBlock text_infoSD = (TextBlock)window.GetChildByName("textBox_infoSD");

        private static RadioButton rdb_model_file_1 = (RadioButton)window.GetChildByName("rdb_file1");
        private static RadioButton rdb_model_file_2 = (RadioButton)window.GetChildByName("rdb_file2");
        private static RadioButton rdb_model_file_3 = (RadioButton)window.GetChildByName("rdb_file3");
        private static Button btn_model_back = (Button)window.GetChildByName("btn_back");
        private static Button btn_model_print = (Button)window.GetChildByName("btn_print");
        private static Button btn_model_preheat = (Button)window.GetChildByName("btn_preheat");
        private static TextBlock text_model_main = (TextBlock)window.GetChildByName("text_main");

        //Aktualni zobrazena obrazovka
        private int _screenIndex = 0;
        //Zavisi na poctu souboru ulozenych na SD karte
        private int _screenNumber = 0;

        private ArrayList _listFiles = new ArrayList();

        private string _printFile;
        private SDManager _sdManager;

        private LCDModelScreen()
        {
            _sdManager = SDManager.GetInstance();

            setWindow(window);
            InitScreen();

        }

        public static LCDModelScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDModelScreen();

            return _instance;
        }

        public void ClearFiles()
        {
            StateHolder.GetInstance().ClearFilesFromListFiles();
            StateHolder.GetInstance().DataLoadingFromSD = false;
            VisibleLoadSd();
        }

        private void InitScreen()
        {

            btn_model_back.TapEvent += new OnTap(btn_model_back_TapEvent);
            btn_model_back.Font = StateHolder.FontUbuntuMiddle;

            btn_model_print.TapEvent += new OnTap(btn_model_print_TapEvent);
            btn_model_print.Font = StateHolder.FontUbuntuMiddle;

            btn_model_preheat.TapEvent += new OnTap(btn_model_preheat_TapEvent);
            btn_model_preheat.Font = StateHolder.FontUbuntuMiddle;

            text_model_main.Font = StateHolder.FontUbuntuBig;
            text_model_file_1.Font = StateHolder.FontUbuntuSmall;
            text_model_file_2.Font = StateHolder.FontUbuntuSmall;
            text_model_file_3.Font = StateHolder.FontUbuntuSmall;
            text_infoSD.Font = StateHolder.FontUbuntuSmall;

            btn_model_prev.TapEvent += new OnTap(btn_model_prev_TapEvent);
            btn_model_prev.Font = StateHolder.FontUbuntuSmall;
            btn_model_prev.Enabled = false;
            btn_model_next.TapEvent += new OnTap(btn_model_next_TapEvent);
            btn_model_next.Font = StateHolder.FontUbuntuSmall;
            btn_model_next.Enabled = false;

            rdb_model_file_1.TapEvent += new OnTap(radio_btn_model_file_1_TapEvent);
            rdb_model_file_2.TapEvent += new OnTap(radio_btn_model_file_2_TapEvent);
            rdb_model_file_3.TapEvent += new OnTap(radio_btn_model_file_3_TapEvent);

            btn_model_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            btn_model_print.Text = Resources.GetString(Resources.StringResources.scrModelBtPrint);
            btn_model_preheat.Text = Resources.GetString(Resources.StringResources.scrModelBtPreheat);
            btn_model_prev.Text = Resources.GetString(Resources.StringResources.TextPrevious);
            btn_model_next.Text = Resources.GetString(Resources.StringResources.TextNext);

            text_model_main.Text = Resources.GetString(Resources.StringResources.scrModelTxMain);
            text_infoSD.Text = Resources.GetString(Resources.StringResources.TextNoSdLoad);

            addUpsBox();

        }

        private void btn_model_back_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
        }

        private void StartPrint()
        {
            string message = "";
            string mainMessage = "";

            LCDWaitingScreen.GetInstance().UpdateScreen(false, Resources.GetString(Resources.StringResources.TextStartPrint), "");
            LCDManager.GetInstance().UpdateScreenLcd(LCD.LCDScreen.StateScreen.WaitingPause, "");

            //LCDWaitingScreen.GetInstance().StartIncrementProgress();
            LCDWaitingScreen.disableIncrement = false;

            // je vybran tiskovy soubor ?
            if (StateHolder.GetInstance().PrintFile == "")
            {
                mainMessage = "";
                message = Resources.GetString(Resources.StringResources.TextNoModelSelection);
                LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.Model, message, mainMessage, true);
                return;
            }

            // jsou uzavrene dvere1 nebo 2
            if (SwitchManager.GetInstance().IsOpenDoor1() || SwitchManager.GetInstance().IsOpenDoor2())
            {
                mainMessage = "";
                message = Resources.GetString(Resources.StringResources.TextOpenDoorOrCover);
                LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.Model, message, mainMessage, true);
                return;
            }

            // je instalovana UPS 
            if (Program.UPS == Be3D.Constants.Constants.UPS.OK)
            {
                // jedeme na baterie
                if (Ups.actState == Ups.STATE.LowBatt || Ups.actState == Ups.STATE.TrafficBatt)
                {
                    message = Resources.GetString(Resources.StringResources.TextStateUpsTraffic);
                    LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.Model, message, mainMessage, true);
                    return;
                }
            }

            _sdManager.GetParametersFromFile();

            // get A, B, AB, kterym extruderem se bude tisknout
            string printingExt = LCDManager.GetInstance().GetPrintingExtruder();

            // test kompatibily gcode na typu tiskarny
            if (LCDManager.GetInstance().IsCorrectManufaturAndTypPrinter() == false || printingExt == "")
            {
                mainMessage = "";
                message = Resources.GetString(Resources.StringResources.Text3DmodelFalse);
                LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.Model, message, mainMessage, true);
                return;
            }

            #region print A
            if (printingExt == "A")
            {
                if (SwitchManager.GetInstance().IsFilamentExt1() == false)
                {
                    showScreenInCorrectMatA();
                    return;
                }
            }
            #endregion

            #region print B
            if (printingExt == "B")
            {
                if (SwitchManager.GetInstance().IsFilamentExt2() == false)
                {
                    showScreenInCorrectMatB();
                    return;
                }
            }
            #endregion

            #region print AB
            if (printingExt == "AB")
            {
                if (SwitchManager.GetInstance().IsFilamentExt1() == false)
                {
                    showScreenInCorrectMatA();
                    return;
                }

                if (SwitchManager.GetInstance().IsFilamentExt2() == false)
                {
                    showScreenInCorrectMatB();
                    return;
                }
            }
            #endregion

            LCDWaitingScreen.disableIncrement = true;
            // START TISK :)
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.PrintInfo, "");
        }

        private void btn_model_print_TapEvent(object sender)
        {
            

            StartPrint();
        }

        private void showScreenInCorrectMatA()
        {
            string mainMessage = "";
            string message = Resources.GetString(Resources.StringResources.TextNoMaterialExtr1);

            LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.Model, message, mainMessage, true);
        }

        private void showScreenInCorrectMatB()
        {
            string mainMessage = "";
            string message = Resources.GetString(Resources.StringResources.TextNoMaterialExtr2);

            LCDManager.GetInstance().ShowErrorScreenLcd(LCDScreen.StateScreen.Model, message, mainMessage, true);

        }

        private void btn_model_preheat_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.Preheat, "");
        }

        private void btn_model_prev_TapEvent(object sender)
        {
            //Tlacitko predchozi v okne Vyber modelu
            _screenIndex--;
            if (_screenIndex <= 0)
                _screenIndex = 0;

            UpdateChooseModel();
        }

        private void btn_model_next_TapEvent(object sender)
        {
            //Tlacitko Nasledujici v okne Vyber modelu
            _screenIndex++;
            if (_screenIndex == _screenNumber)
                _screenIndex = _screenNumber;

            UpdateChooseModel();
        }

        private void radio_btn_model_file_1_TapEvent(object sender)
        {
            if ((_screenIndex * 3) < _listFiles.Count)
            {
                _printFile = _listFiles[(_screenIndex * 3)].ToString() + "." + Constants.Prefix1;
                savePrintFile(_printFile);
            }
            rdb_model_file_2.Checked = false;
            rdb_model_file_3.Checked = false;
        }

        private void radio_btn_model_file_2_TapEvent(object sender)
        {
            if (((_screenIndex * 3) + 1) < _listFiles.Count)
            {
                _printFile = _listFiles[(_screenIndex * 3) + 1].ToString() + "." + Constants.Prefix1;
                savePrintFile(_printFile);
            }
            rdb_model_file_1.Checked = false;
            rdb_model_file_3.Checked = false;
        }

        private void radio_btn_model_file_3_TapEvent(object sender)
        {
            if (((_screenIndex * 3) + 2) < _listFiles.Count)
            {
                _printFile = _listFiles[(_screenIndex * 3) + 2].ToString() + "." + Constants.Prefix1;
                savePrintFile(_printFile);
            }
            rdb_model_file_1.Checked = false;
            rdb_model_file_2.Checked = false;
        }

        private void savePrintFile(string name)
        {
            StateHolder.GetInstance().PrintFile = name;
            LCDManager.GetInstance().SaveConfiguration();
        }



        private void VisibleLoadSd()
        {
            text_infoSD.Text = Resources.GetString(Resources.StringResources.TextSDLoad);

            text_infoSD.Visible = true;
            text_model_file_1.Visible = false;
            text_model_file_2.Visible = false;
            text_model_file_3.Visible = false;
            rdb_model_file_1.Visible = false;
            rdb_model_file_2.Visible = false;
            rdb_model_file_3.Visible = false;
            btn_model_print.Enabled = false;
            btn_model_prev.Enabled = false;
            btn_model_next.Enabled = false;

            if (LCDManager.IsActiveModelScreen)
                window.Invalidate();
        }

        private void VisibleNoLoadSd()
        {
            text_infoSD.Text = Resources.GetString(Resources.StringResources.TextNoSdLoad);

            text_infoSD.Visible = true;
            text_model_file_1.Visible = false;
            text_model_file_2.Visible = false;
            text_model_file_3.Visible = false;
            rdb_model_file_1.Visible = false;
            rdb_model_file_2.Visible = false;
            rdb_model_file_3.Visible = false;
            btn_model_print.Enabled = false;
            btn_model_prev.Enabled = false;
            btn_model_next.Enabled = false;

            if (LCDManager.IsActiveModelScreen)
                window.Invalidate();

        }

        public void LoadPrintsFiles()
        {
            ArrayList listFiles = null;

            if (!(StateHolder.GetInstance().IsMountingSD))
            {
                StateHolder.GetInstance().ClearFilesFromListFiles();

                return;
            }

            _printFile = StateHolder.GetInstance().PrintFile.Split('.')[0];

            if (StateHolder.GetInstance().IsMountingSD)
                if (StateHolder.GetInstance().GetListFiles() != null)
                    listFiles = (ArrayList)StateHolder.GetInstance().GetListFiles().Clone();


            if (listFiles == null || listFiles.Count == 0)
            {
                return;
            }

            _listFiles.Clear();

            Collections.quickSort(ref listFiles, 0, listFiles.Count-1);

            // smazeme ze seznamu soubor ktery byl jiz drive vybran         
            for (int i = 0; i < listFiles.Count; i++)
            {
                if ((string)listFiles[i] == _printFile)
                {
                    //pridame drive vybrany soubor jako prvni
                    _listFiles.Add(_printFile);
                    listFiles.RemoveAt(i);
                    break;
                }
            }

            //zkopirujeme nove pole ve spravnem poradi
            for (int i = 0; i < listFiles.Count; i++)
            {
                _listFiles.Add(listFiles[i]);
            }

            if ((_listFiles.Count % 3) == 0)
            {
                _screenNumber = (_listFiles.Count / 3);
            }
            else
            {
                _screenNumber = (_listFiles.Count / 3) + 1;
            }

            _screenIndex = 0;

        }

        public void UpdateChooseModel()
        {
            #region sd hlaseni

            //zobraz hlasku o chybe SD
            if (StateHolder.GetInstance().GetListFiles() == null || StateHolder.GetInstance().GetListFiles().Count == 0)
            {
                VisibleNoLoadSd();
                return;
            }

            text_infoSD.Visible = false;
            text_model_file_1.Visible = true;
            text_model_file_2.Visible = true;
            text_model_file_3.Visible = true;
            rdb_model_file_1.Visible = true;
            rdb_model_file_2.Visible = true;
            rdb_model_file_3.Visible = true;
            btn_model_print.Enabled = true;

            #endregion

            #region update button

            if (_screenIndex == 0)
            {
                btn_model_prev.Enabled = false;
            }
            else
            {
                btn_model_prev.Enabled = true;
            }

            if ((_screenIndex + 1) == _screenNumber)
            {
                btn_model_next.Enabled = false;
            }
            else
            {
                btn_model_next.Enabled = true;
            }

            #endregion

            string[] prnt = _printFile.Split('.');
            if (prnt == null || prnt.Length < 1)
                prnt = new string[] { "" };

            if ((_screenIndex * 3) < _listFiles.Count)
            {
                rdb_model_file_1.Enabled = true;
                string str1 = _listFiles[(_screenIndex * 3)].ToString();

                if (Constants.IsCharInName(str1, '~'))
                    text_model_file_1.Text = str1.Substring(0, str1.Length - 2); // orizne posledni dva znaky xxxx~1
                else
                    text_model_file_1.Text = str1;
            }
            else
            {
                rdb_model_file_1.Enabled = false;
                text_model_file_1.Text = "--";
            }

            if (text_model_file_1.Text == prnt[0])
                rdb_model_file_1.Checked = true;
            else
                rdb_model_file_1.Checked = false;


            if (((_screenIndex * 3) + 1) < _listFiles.Count)
            {
                rdb_model_file_2.Enabled = true;
                string str2 = _listFiles[(_screenIndex * 3) + 1].ToString();

                if (Constants.IsCharInName(str2, '~'))
                    text_model_file_2.Text = str2.Substring(0, str2.Length - 2); // orizne posledni dva znaky xxxx~1
                else
                    text_model_file_2.Text = str2;


            }
            else
            {
                rdb_model_file_2.Enabled = false;
                text_model_file_2.Text = "--";
            }

            if (text_model_file_2.Text == prnt[0])
                rdb_model_file_2.Checked = true;
            else
                rdb_model_file_2.Checked = false;

            if (((_screenIndex * 3) + 2) < _listFiles.Count)
            {
                rdb_model_file_3.Enabled = true;
                string str3 = _listFiles[(_screenIndex * 3) + 2].ToString();

                if (Constants.IsCharInName(str3, '~'))
                    text_model_file_3.Text = str3.Substring(0, str3.Length - 2); // orizne posledni dva znaky xxxx~1
                else
                    text_model_file_3.Text = str3;

            }
            else
            {
                rdb_model_file_3.Enabled = false;
                text_model_file_3.Text = "--";
            }
            if (text_model_file_3.Text == prnt[0])
                rdb_model_file_3.Checked = true;
            else
                rdb_model_file_3.Checked = false;

            if (LCDManager.IsActiveModelScreen)
                window.Invalidate();

        }

    }
}
