
using Be3D.Constants;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;

namespace _3D_control_v09
{
    class LCDSettingsTempScreen : LCDScreen
    {
        private Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinSettingsTemp));
      
        private RadioButton rdb_temp_ext_1;
        private RadioButton rdb_temp_ext_2;
        private RadioButton rdb_temp_ext_3;
        private RadioButton rdb_temp_bed_1;
        private RadioButton rdb_temp_bed_2;
        private RadioButton rdb_temp_bed_3;
        private RadioButton rdb_temp_space_1;
        private RadioButton rdb_temp_space_2;
        private RadioButton rdb_temp_space_3;


        public LCDSettingsTempScreen()
        {
            setWindow(window);
            InitScreen();

            if(ConfigurationPrinter.GetInstance().IsPresentBedHeat())
                UpdateTempBed();

            UpdateTempExt();
            
            if(ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
                UpdateTempSpace();
        }


        private void InitScreen()
        {

            rdb_temp_ext_1 = (RadioButton) window.GetChildByName("rdb_ext1");
            rdb_temp_ext_2 = (RadioButton) window.GetChildByName("rdb_ext2");
            rdb_temp_ext_3 = (RadioButton) window.GetChildByName("rdb_ext3");
            rdb_temp_bed_1 = (RadioButton) window.GetChildByName("rdb_bed1");
            rdb_temp_bed_2 = (RadioButton) window.GetChildByName("rdb_bed2");
            rdb_temp_bed_3 = (RadioButton) window.GetChildByName("rdb_bed3");
            rdb_temp_space_1 = (RadioButton)window.GetChildByName("rdb_space1");
            rdb_temp_space_2 = (RadioButton)window.GetChildByName("rdb_space2");
            rdb_temp_space_3 = (RadioButton)window.GetChildByName("rdb_space3");


            //***************   Temp settings window   **********************
            Button btn_temp_back = (Button) window.GetChildByName("btn_back");
            btn_temp_back.TapEvent += new OnTap(btn_temp_back_TapEvent);
            btn_temp_back.Font = StateHolder.FontUbuntuMiddle;
            TextBlock text_temp_nozzle = (TextBlock) window.GetChildByName("text_info_nozzle");
            text_temp_nozzle.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_bed = (TextBlock)window.GetChildByName("text_info_bed");
            text_temp_bed.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_space = (TextBlock)window.GetChildByName("text_info_space");
            text_temp_space.Font = StateHolder.FontUbuntuSmall;

            TextBlock text_temp_text3 = (TextBlock) window.GetChildByName("text_info3");
            text_temp_text3.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_text4 = (TextBlock) window.GetChildByName("text_info4");
            text_temp_text4.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_text5 = (TextBlock) window.GetChildByName("text_info5");
            text_temp_text5.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_text6 = (TextBlock) window.GetChildByName("text_info6");
            text_temp_text6.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_text7 = (TextBlock) window.GetChildByName("text_info7");
            text_temp_text7.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_text8 = (TextBlock) window.GetChildByName("text_info8");
            text_temp_text8.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_text9 = (TextBlock) window.GetChildByName("text_info9");
            text_temp_text9.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_text10 = (TextBlock) window.GetChildByName("text_info10");
            text_temp_text10.Font = StateHolder.FontUbuntuSmall;
            TextBlock text_temp_text11 = (TextBlock) window.GetChildByName("text_info11");
            text_temp_text11.Font = StateHolder.FontUbuntuSmall;

            TextBlock text_temp_main = (TextBlock) window.GetChildByName("text_main");
            text_temp_main.Font = StateHolder.FontUbuntuBig;

            addUpsBox();

            rdb_temp_ext_1.TapEvent += new OnTap(rdb_temp_ext1_TapEvent);
            rdb_temp_ext_2.TapEvent += new OnTap(rdb_temp_ext2_TapEvent);
            rdb_temp_ext_3.TapEvent += new OnTap(rdb_temp_ext3_TapEvent);

            rdb_temp_bed_1.TapEvent += new OnTap(rdb_temp_bed1_TapEvent);
            rdb_temp_bed_2.TapEvent += new OnTap(rdb_temp_bed2_TapEvent);
            rdb_temp_bed_3.TapEvent += new OnTap(rdb_temp_bed3_TapEvent);

            rdb_temp_space_1.TapEvent += new OnTap(rdb_temp_space1_TapEvent);
            rdb_temp_space_2.TapEvent += new OnTap(rdb_temp_space2_TapEvent);
            rdb_temp_space_3.TapEvent += new OnTap(rdb_temp_space3_TapEvent);

            btn_temp_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            text_temp_nozzle.Text = Resources.GetString(Resources.StringResources.scrSettingTempNozzle);
            text_temp_bed.Text = Resources.GetString(Resources.StringResources.scrSettingTempBed);
            text_temp_space.Text = Resources.GetString(Resources.StringResources.scrSettingTempSpace);
            text_temp_main.Text = Resources.GetString(Resources.StringResources.scrSettingTempTxMain);

            text_temp_text3.Text = Constants.TempOptionExt1.ToString();
            text_temp_text4.Text = Constants.TempOptionExt2.ToString();
            text_temp_text5.Text = Constants.TempOptionExt3.ToString();

            text_temp_text6.Text = Constants.TempOptionBed1.ToString();
            text_temp_text7.Text = Constants.TempOptionBed2.ToString();
            text_temp_text8.Text = Constants.TempOptionBed3.ToString();

            text_temp_text9.Text = Constants.TempOptionSpace1.ToString();
            if (Constants.TempOptionSpace1 == 0)
                text_temp_text9.Text = "Off";
               
            text_temp_text10.Text = Constants.TempOptionSpace2.ToString();
            text_temp_text11.Text = Constants.TempOptionSpace3.ToString();

            if (!ConfigurationPrinter.GetInstance().IsPresentBedHeat())
            {
                rdb_temp_bed_1.Visible = false;
                rdb_temp_bed_2.Visible = false;
                rdb_temp_bed_3.Visible = false;
                
                text_temp_text6.Visible = false;
                text_temp_text7.Visible = false;
                text_temp_text8.Visible = false;

                text_temp_bed.Visible = false;
            }

            if (!ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
            {
                rdb_temp_space_1.Visible = false;
                rdb_temp_space_2.Visible = false;
                rdb_temp_space_3.Visible = false;
                
                text_temp_text9.Visible = false;
                text_temp_text10.Visible = false;
                text_temp_text11.Visible = false;


                text_temp_space.Visible = false;
            }


            window.Render();

        }

        private void btn_temp_back_TapEvent(object sender)
        {
            LCDManager.GetInstance().UpdateScreenLcd(StateScreen.SettingAnother, "");
        }


        private void rdb_temp_space3_TapEvent(object sender)
        {
            saveDefaultRefTempSpace(Constants.TempOptionSpace3);
            UpdateTempSpace();
        }

        private void rdb_temp_space2_TapEvent(object sender)
        {
            saveDefaultRefTempSpace(Constants.TempOptionSpace2);
            UpdateTempSpace();
        }

        private void rdb_temp_space1_TapEvent(object sender)
        {
            saveDefaultRefTempSpace(Constants.TempOptionSpace1);
            UpdateTempSpace();
        }

        private void saveDefaultRefTempSpace(int temp)
        {
            StateHolder.GetInstance().ActSetTempSpace = temp;
            Program._basicConfig.DefTmpRefSpace = temp;
            LCDManager.GetInstance().SaveConfiguration();
        }
      

        private void rdb_temp_bed3_TapEvent(object sender)
        {
            saveDefaultTempRefBed(Constants.TempOptionBed3);
            UpdateTempBed();
        }

        private void rdb_temp_bed2_TapEvent(object sender)
        {
            saveDefaultTempRefBed(Constants.TempOptionBed2);
            UpdateTempBed();
            
        }

        private void rdb_temp_bed1_TapEvent(object sender)
        {
            saveDefaultTempRefBed(Constants.TempOptionBed1);
            UpdateTempBed();
        }

        private void saveDefaultTempRefBed(int temp)
        {
            StateHolder.GetInstance().ActSetTempBed = temp;
            Program._basicConfig.DefTmpRefBed = StateHolder.GetInstance().ActSetTempBed;
            LCDManager.GetInstance().SaveConfiguration();
        
        }


        private void rdb_temp_ext3_TapEvent(object sender)
        {
            saveDefaultTempRefExtr(Constants.TempOptionExt3);
            UpdateTempExt();
        }

        private void rdb_temp_ext2_TapEvent(object sender)
        {
            saveDefaultTempRefExtr(Constants.TempOptionExt2);

            UpdateTempExt();
        }

        private void rdb_temp_ext1_TapEvent(object sender)
        {
            saveDefaultTempRefExtr(Constants.TempOptionExt1);
            UpdateTempExt();
        }

        private void saveDefaultTempRefExtr(int temp)
        {
            StateHolder.GetInstance().ActSetTempPrimary = temp;
            StateHolder.GetInstance().ActSetTempSecondary = temp;

            Program._basicConfig.DefTmpRefExtr0 = StateHolder.GetInstance().ActSetTempPrimary;
            Program._basicConfig.DefTmpRefExtr1 = StateHolder.GetInstance().ActSetTempSecondary;
            LCDManager.GetInstance().SaveConfiguration();
        }



        private void UpdateTempSpace()
        {
            switch (StateHolder.GetInstance().ActSetTempSpace)
            {
                case Constants.TempOptionSpace1:
                    rdb_temp_space_1.Checked = true;
                    rdb_temp_space_2.Checked = false;
                    rdb_temp_space_3.Checked = false;
                    break;
                case Constants.TempOptionSpace2:
                    rdb_temp_space_1.Checked = false;
                    rdb_temp_space_2.Checked = true;
                    rdb_temp_space_3.Checked = false;
                    break;
                case Constants.TempOptionSpace3:
                    rdb_temp_space_1.Checked = false;
                    rdb_temp_space_2.Checked = false;
                    rdb_temp_space_3.Checked = true;
                    break;
            }
        }

        private void UpdateTempBed()
        {
            switch (StateHolder.GetInstance().ActSetTempBed)
            {
                case Constants.TempOptionBed1:
                    rdb_temp_bed_1.Checked = true;
                    rdb_temp_bed_2.Checked = false;
                    rdb_temp_bed_3.Checked = false;
                    break;
                case Constants.TempOptionBed2:
                    rdb_temp_bed_1.Checked = false;
                    rdb_temp_bed_2.Checked = true;
                    rdb_temp_bed_3.Checked = false;
                    break;
                case Constants.TempOptionBed3:
                    rdb_temp_bed_1.Checked = false;
                    rdb_temp_bed_2.Checked = false;
                    rdb_temp_bed_3.Checked = true;
                    break;
            }
            
        }

        private void UpdateTempExt()
        {
            switch (StateHolder.GetInstance().ActSetTempPrimary)
            {
                case Constants.TempOptionExt1:
                    rdb_temp_ext_1.Checked = true;
                    rdb_temp_ext_2.Checked = false;
                    rdb_temp_ext_3.Checked = false;
                    break;
                case Constants.TempOptionExt2:
                    rdb_temp_ext_1.Checked = false;
                    rdb_temp_ext_2.Checked = true;
                    rdb_temp_ext_3.Checked = false;
                    break;
                case Constants.TempOptionExt3:
                    rdb_temp_ext_1.Checked = false;
                    rdb_temp_ext_2.Checked = false;
                    rdb_temp_ext_3.Checked = true;
                    break;
            }
       
        }

    }
}
