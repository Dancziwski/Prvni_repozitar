
using System.Collections;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using Microsoft.SPOT.Hardware;
using Button = GHI.Glide.UI.Button;
using Be3D.Constants;
using System.Reflection;

namespace _3D_control_v09
{
    class LCDLanguageScreen : LCDScreen
    {
        private static LCDLanguageScreen _instance;
        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinLanguageChange));

        private static Button btn_back = (Button)window.GetChildByName("btn_back");
        private static Button btn_ok = (Button)window.GetChildByName("btn_ok");

        private static TextBlock text_model_main = (TextBlock)window.GetChildByName("text_main");


        private static Button btn_model_prev = (Button)window.GetChildByName("btn_prev");
        private static Button btn_model_next = (Button)window.GetChildByName("btn_next");

        private static RadioButton rdb_model_file_1 = (RadioButton)window.GetChildByName("rdb_file1");
        private static RadioButton rdb_model_file_2 = (RadioButton)window.GetChildByName("rdb_file2");
        private static RadioButton rdb_model_file_3 = (RadioButton)window.GetChildByName("rdb_file3");

        private static TextBlock text_model_file_1 = (TextBlock)window.GetChildByName("text_file1");
        private static TextBlock text_model_file_2 = (TextBlock)window.GetChildByName("text_file2");
        private static TextBlock text_model_file_3 = (TextBlock)window.GetChildByName("text_file3");

        //Aktualni zobrazena obrazovka
        private static int _screenIndex = 0;
        //Zavisi na poctu souboru ulozenych na SD karte
        private static int _screenNumber = 0;

        private static ArrayList _listLangKey; 

        private Constants.LANGUAGE _actLangSelect;

        private LCDLanguageScreen()
        {
            _listLangKey = new ArrayList();

            setWindow(window);
            InitScreen();

            LoadLanguages();

        }

        public static LCDLanguageScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDLanguageScreen();

            return _instance;
        }

        private void InitScreen()
        {
            
            btn_back.TapEvent += new OnTap(btn_model_back_TapEvent);
            btn_back.Font = StateHolder.FontUbuntuMiddle;

            btn_ok.TapEvent += new OnTap(btn_model_ok_TapEvent);
            btn_ok.Font = StateHolder.FontUbuntuMiddle;

            text_model_main.Font = StateHolder.FontUbuntuBig;
            text_model_file_1.Font = StateHolder.FontUbuntuSmall;
            text_model_file_2.Font = StateHolder.FontUbuntuSmall;
            text_model_file_3.Font = StateHolder.FontUbuntuSmall;

            btn_model_prev.TapEvent += new OnTap(btn_model_prev_TapEvent);
            btn_model_prev.Font = StateHolder.FontUbuntuSmall;
            btn_model_prev.Enabled = false;
            btn_model_next.TapEvent += new OnTap(btn_model_next_TapEvent);
            btn_model_next.Font = StateHolder.FontUbuntuSmall;
            btn_model_next.Enabled = false;

            rdb_model_file_1.TapEvent += new OnTap(radio_btn_model_file_1_TapEvent);
            rdb_model_file_2.TapEvent += new OnTap(radio_btn_model_file_2_TapEvent);
            rdb_model_file_3.TapEvent += new OnTap(radio_btn_model_file_3_TapEvent);

            btn_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            btn_ok.Text = Resources.GetString(Resources.StringResources.TextOk);
            text_model_main.Text = Resources.GetString(Resources.StringResources.scrLanguageTxMain);
            btn_model_prev.Text = Resources.GetString(Resources.StringResources.TextPrevious);
            btn_model_next.Text = Resources.GetString(Resources.StringResources.TextNext);

            addUpsBox();

        }

        private void btn_model_back_TapEvent(object sender)
        {
            Program._basicConfig.LangSelected = _actLangSelect;   // nutne vratit do puvodniho stavu

            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");
        }

        private void btn_model_ok_TapEvent(object sender)
        {
            if (Program._basicConfig.LangSelected != _actLangSelect)
            {
                //Program.basicConfig.LangSelected = _actLangSelect;
                LCDManager.GetInstance().SaveConfiguration();
                PowerState.RebootDevice(true);

            }
            else
            {
                Program._basicConfig.LangSelected = _actLangSelect;   // nutne vratit do puvodniho stavu
                LCDManager.GetInstance().UpdateScreenLcd(StateScreen.SettingAnother, ""); 
            }
            
        }

        private void btn_model_prev_TapEvent(object sender)
        {
            //Tlacitko predchozi v okne Vyber modelu
            _screenIndex--;
            if (_screenIndex <= 0)
                _screenIndex = 0;

            UpdateScreenLang();
        }

        private void btn_model_next_TapEvent(object sender)
        {
            //Tlacitko Nasledujici v okne Vyber modelu
            _screenIndex++;
            if (_screenIndex == _screenNumber)
               _screenIndex = _screenNumber;

            UpdateScreenLang();   
        }

        private void radio_btn_model_file_1_TapEvent(object sender)
        {
            if ((_screenIndex*3) < _listLangKey.Count)
            {
                Program._basicConfig.LangSelected = (Constants.LANGUAGE)_listLangKey[(_screenIndex * 3)];
            }

            rdb_model_file_2.Checked = false;
            rdb_model_file_3.Checked = false;
        }

        private void radio_btn_model_file_2_TapEvent(object sender)
        {
            if (((_screenIndex * 3) + 1) < _listLangKey.Count)
            {
                Program._basicConfig.LangSelected = (Constants.LANGUAGE)_listLangKey[(_screenIndex * 3) + 1];
            }

            rdb_model_file_1.Checked = false;
            rdb_model_file_3.Checked = false;
        }

        private void radio_btn_model_file_3_TapEvent(object sender)
        {
            if (((_screenIndex * 3) + 2) < _listLangKey.Count)
            {
                Program._basicConfig.LangSelected = (Constants.LANGUAGE)_listLangKey[(_screenIndex * 3) + 2];
            }

            rdb_model_file_1.Checked = false;
            rdb_model_file_2.Checked = false;
        }


    
        private void LoadLanguages()
        {
            _listLangKey.Clear();

            _actLangSelect = Program._basicConfig.LangSelected;

            _listLangKey.Add(_actLangSelect);

            if ((StateHolder.GetInstance().TabLangNameOfPrefix.Count % 3) == 0)
                _screenNumber = (StateHolder.GetInstance().TabLangNameOfPrefix.Count / 3);
            else
                _screenNumber = (StateHolder.GetInstance().TabLangNameOfPrefix.Count / 3) + 1;

            foreach (DictionaryEntry lang in StateHolder.GetInstance().TabLangNameOfPrefix)
            {
                if ((Constants.LANGUAGE) lang.Key != _actLangSelect)
                {
                    _listLangKey.Add(lang.Key);
                }
            }

            _screenIndex = 0;  
        }

        public void UpdateScreenLang()
        {
            #region update button
            if (_screenIndex == 0)
                btn_model_prev.Enabled = false;           
            else
                btn_model_prev.Enabled = true;               

            if ((_screenIndex + 1) == _screenNumber)
                btn_model_next.Enabled = false;              
            else
                btn_model_next.Enabled = true;              

            #endregion

            if ((_screenIndex*3) < _listLangKey.Count)
            {
                rdb_model_file_1.Enabled = true;
                text_model_file_1.Text = StateHolder.GetInstance().TabLangNameOfPrefix[(Constants.LANGUAGE)_listLangKey[(_screenIndex * 3)]].ToString();
            }
            else
            {
                rdb_model_file_1.Enabled = false;
                text_model_file_1.Text = "--";
            }

            //oznaci aktualni zvoleny model
            if (_screenIndex == 0)
            {
                rdb_model_file_1.Checked = true;
                rdb_model_file_2.Checked = false;
                rdb_model_file_3.Checked = false;
            }
            else
            {
                rdb_model_file_1.Checked = false;
                rdb_model_file_2.Checked = false;
                rdb_model_file_3.Checked = false;
            }

            if (((_screenIndex*3) + 1) < _listLangKey.Count)
            {
                rdb_model_file_2.Enabled = true;
                text_model_file_2.Text = StateHolder.GetInstance().TabLangNameOfPrefix[(Constants.LANGUAGE)_listLangKey[(_screenIndex * 3) + 1]].ToString();
            }
            else
            {
                rdb_model_file_2.Enabled = false;
                text_model_file_2.Text = "--";
            }

            if (((_screenIndex*3) + 2) < _listLangKey.Count)
            {
                rdb_model_file_3.Enabled = true;
                text_model_file_3.Text = StateHolder.GetInstance().TabLangNameOfPrefix[(Constants.LANGUAGE)_listLangKey[(_screenIndex * 3) + 2]].ToString();
            }
            else
            {
                rdb_model_file_3.Enabled = false;
                text_model_file_3.Text = "--";
            }

            window.Invalidate();
        }
    
    }
}
