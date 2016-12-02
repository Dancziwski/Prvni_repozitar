
using System;
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;
using LCD;
using _3D_control_v09;
using Be3D.Constants;

namespace _3D_control_v09
{
    class LCDInfoScreen : LCDScreen
    {
        private static LCDInfoScreen _instance;

        private static Window window = GlideLoader.LoadWindow(ResGUI.GetString(ResGUI.StringResources.WinInfo));

        private static Button btn_info_back = (Button)window.GetChildByName("btn_back");
        private static TextBlock text_info_main = (TextBlock)window.GetChildByName("text_main");
        private static TextBlock text_nameCompany = (TextBlock)window.GetChildByName("text_nameCompany");
        private static TextBlock text_company = (TextBlock)window.GetChildByName("text_company");

        private TextBlock text_nameProduct = (TextBlock)window.GetChildByName("text_nameProduct");
        private TextBlock text_valueNameProduct = (TextBlock)window.GetChildByName("text_valueNameProduct");
        
        private static TextBlock text_typeProduct = (TextBlock)window.GetChildByName("text_typeProduct");
        private static TextBlock text_valueTypeProduct = (TextBlock)window.GetChildByName("text_valueTypeProduct");

        private static TextBlock text_nameTrafficTime = (TextBlock)window.GetChildByName("text_nameTrafficTime");
        private static TextBlock text_trafficTime = (TextBlock)window.GetChildByName("text_trafficTime");
        private static TextBlock text_firmware = (TextBlock)window.GetChildByName("text_firmware");
        private static TextBlock text_urlWeb = (TextBlock)window.GetChildByName("text_urlWeb");


        private LCDInfoScreen()
        {
            setWindow(window); 
            InitScreen();        
        }

        public static LCDInfoScreen GetInstance()
        {
            if (_instance == null)
                _instance = new LCDInfoScreen();

            return _instance;

        }

        private void InitScreen()
        {
            btn_info_back.TapEvent += new OnTap(btn_back_info_TapEvent);
            btn_info_back.Font = StateHolder.FontUbuntuMiddle;

            text_info_main.Font = StateHolder.FontUbuntuBig;
            text_nameCompany.Font = StateHolder.FontUbuntuSmall;            
            text_company.Font = StateHolder.FontUbuntuSmall;

            text_nameProduct.Font = StateHolder.FontUbuntuSmall;
            text_valueNameProduct.Font = StateHolder.FontUbuntuSmall;      
            text_typeProduct.Font = StateHolder.FontUbuntuSmall;           
            text_valueTypeProduct.Font = StateHolder.FontUbuntuSmall; 
          
            text_nameTrafficTime.Font = StateHolder.FontUbuntuSmall;           
            text_trafficTime.Font = StateHolder.FontUbuntuSmall;          
            text_firmware.Font = StateHolder.FontUbuntuSmall;           
            text_urlWeb.Font = StateHolder.FontUbuntuSmall;


            btn_info_back.Text = Resources.GetString(Resources.StringResources.TextBack);
            text_info_main.Text = Resources.GetString(Resources.StringResources.scrtInfoTxMain);
            text_nameCompany.Text = Resources.GetString(Resources.StringResources.scrInfoNameCompany);

            text_nameProduct.Text = Resources.GetString(Resources.StringResources.scrInfoNameProduct);
            text_typeProduct.Text = Resources.GetString(Resources.StringResources.scrInfoTypeProduct);
            

            text_company.Text = ConfigurationPrinter.GetInstance().GetNameOfManufacterLong();
            text_valueNameProduct.Text = ConfigurationPrinter.GetInstance().GetNameOfPrinter();
            text_valueTypeProduct.Text = ConfigurationPrinter.GetInstance().GetTypePrinter();
            
            text_firmware.Text = Resources.GetString(Resources.StringResources.srcInfoFirmware) + " " + ConfigurationPrinter.GetInstance().verisonFWControl + " / " + ConfigurationPrinter.GetInstance().verisonFWPower;
            text_urlWeb.Text = Resources.GetString(Resources.StringResources.srcInfoUrlWeb);

            addUpsBox();

        }


        public void UpdateData(TimeSpan time)
        {
            text_nameTrafficTime.Text = Resources.GetString(Resources.StringResources.scrInfoNameTraffic);
            text_trafficTime.Text = ((time.Days * 24) + time.Hours) + "h " + time.Minutes + " m";

            window.Invalidate();
        }

        private void btn_back_info_TapEvent(object sender)
        {

            LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");

        }
    }
}
