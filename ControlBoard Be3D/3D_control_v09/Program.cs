using System;
using System.Collections;
using System.Globalization;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Threading;
using GHI.Hardware.G120;
using GHI.Premium.Hardware.LowLevel;
using GHI.Premium.System;
using GHI.Premium.USBHost;
using LCD;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO;
using Be3D.Constants;

namespace _3D_control_v09
{
    
    public class Program
    {
        /*
         * 
         * Change log 1.4.7
         * 34, přidana tiskarna 50um DeeGreen+
         * 35, opravit problem, prenos souboru otevřeni dveri zakazat Thread create
         * 36, přidat při spuštění tisku z DeeControl načtení parametrů ze souboru
         * 37, opraven problem s upsUp při back v shutdown
         * 38, zrychleni menu pomoci Static 
         * 
         * Change log 1.4.8
         * 39, problem other setting
         * 
         * Change log 1.4.9
         * 40, problem pausa 
         * 
         * Change log 1.4.10
         * 41, problem UART - opraveno problem se statickymi screen ktere si vojali pri vzniku novy objekt.
         * 42, Proverit jeste dalsi screen zda pujdou predelat na singleton
         * 43, upravena UART komunikace dle EBP FW
         * 44, odstranen discard buffer ze Serial buffer - zahazuje data 
         * 45, opraven problem s kapacitou SD karty
         * 
         * Change log 1.4.11
         * 46, pridana moznost spustit gcode z predesle serie
         * 
         * Change log 1.4.12
         * 47, zruseno Resume a Suspend vlaken
         * 48, predelano end vlaken vzdy se ceka na dokonceni
         * 
         * Change log 1.4.13
         * 49, implementace opravy chyb s plnou SD a MountSD když není v tiskárně
         * 
         * Change log 1.4.14 
         * 50, změna názvu firmy be3D s.r.o.
         * 51, chyba oriznutého textu u výměny struny
         * 52, změna natahováni struny první krok 65mm, repeat 5mm
         * 53, opraveno přepínání jazyků
         * 54, stopovani pri moution test - natvrdo restart PW
         * 55, změna sekvence při vývměně struny, pokud je jeden extruder ruší se obrazovka s volbou extruderu
         * 56, pri vymene struny nefungoval stopnuti struny. 
         * 57, změna sekvence motion test
         * 58, čekání na ukončení pohybu při pause tisku a opetovne navazani 
         * 59, snad opraveno - po ukončeném tisku nejde výměna struny / testovano s 1.4.13 a funkční
         * 60, - zastavovani pojezdu pri pohybu osou Z po ukonceni tisku
         * 61, uprava refreshe preheat, printinfo screen atd.
         * 62, 
         * 
         * 
         * Change log 1.4.15
         * 64, vraceni hardwarereset do shutdown
         * 65, pridani 2x0.1mm pro vymenu struny
         * 
         * Change log 1.4.16
         * 66, pridana podpora DeeRed 2_1
         * 
         * Change log 1.4.17
         * 67, pridan timer pro hromadny update grafiky, naveseno wait okno,pause,printinfo,fil change
         * 68, pridana vymena struny selector DeeRed 1.3
         * 
         * Change log 1.4.18
         * 69, opr bug - chyba v popisu sekundar/primar 
         * 70), unvisible teplota druhe tryskz 
         * 
         * Change log 1.4.19
         * 71, odstraneno SD G120 board
         * 72, uprava sd print status aby se delal refresh pouze pri ini nebo print
         * 73, blokace znovu volani StartPrint,StopPrint,DonePrint,PausePrint
         * 74, blokace znovu volani StartPreheating, StopPreheating
         * 75, snad vyresen problem pausa
         * 76, opakovany tisk - problem se spatnem stavu po stisku end
         * 
         * 
         * Change log 1.4.20
         * 77, testovaci kriz universal vzavislosti na velikosti podlozky tiskarny
         * 78, upraveno operating time predpoklad ze tiskarna nepojede dele jak 5let + double zaloha casu
         * 79, vysunuti struny pri HW reset pri stop, cekam az se vysune struna pak restart
         * 80, opraven problem GCD:xxxx
         * 
         * 
         * Change log 1.4.21
         * 81, změněna pauza dle Tomáše P.  
         * 82, oprava chyby s výměnou struny během pausy tisku nechce se čeknout
         * 
         * 
         * Change log 1.4.22
         * 83, dodelana podpora 2_1
         * 84, opraveno stopnuti tisku
         * 85, upraven algoritmus pausy interval cteni cca 1s
         * 86, upraven format zobrazeneho casu
         * 
         * Change log 1.4.23
         * 
         * 87, oprava pauzy dle Tomáše P. primarne testováno DGRv5
         * 88, upravena sekce zpracovani error hlaseni z PWB
         * 89, prodlouzen feed in default na 75
         * 70, vylepseno ukladani a prvotni ini EEPROM
         * 
         * Change log 1.4.24
         * 91, problem pri vyhrevu podlozky a error 9999 pri line +1 - opraveno
         * 92, opraven problem s EEPROM pri uplne nove desce
         * 
         * Change log 1.4.25
         * 93, odchycen error Endstop/motor fail. " na konci mezera
         * 94, upraven error odchytavani - pridany dalsi - nutné více protestovat
         * 95, disablovani tlacitka Repeat 5mm po stisku na 2s
         * 
         * Change log 1.4.26
         * 96, vizualizace teploty pro DGRv5 a eDee
         * 97, oprava nazvu trysek a serazeni na print info a preheat screen 
         * 98, oprava Error checksum - ignorovan - vyresen problem pri dlouhem nahrivani
         * 100, nove jazyky
         * 
         * Change log 1.4.27
         * 101, oprava problemu s offset a jazyky - neexistoval NumberFormat
         * 102, zporovzneno automaticke nacitani offsetu
         * 103, upravena LCD offset na singleton
         * 
         * Change log 1.4.28
         * 104, přidana podpora DREStolice
         * 
         * Change log 1.4.29
         * - přidání DGRstolice3
         * 
         * TODO  
         *      - pri error vypnout update tecek překryjí obrazovku
         *      - dvoubarevny tisk a prohozeni postupu kvuli barevnosti materialu
         *      - kouknout na zapnuti predehrevu space heating 
         */

        public static string VersionNumber = "1.4.29";
       
        //GREEN
       // public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.DeeGreenSer1Ser2;
         // public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.DeeGreenSer3;
       // public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.PresentDeeGreen;


        // RED 
        //public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.DeeRed1_2;
        //public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.DeeRed1_3;
        //public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.PresentDeeRed;
        //public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.DeeRed2_1;
        //public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.DREStolice;
        public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.DGRStolice3;
        //public static Constants.TYPE_PRINTER Type = Constants.TYPE_PRINTER.DGRStolice5;

        // public static Constants.UPS UPS = Constants.UPS.OK;     
         public static Constants.UPS UPS = Constants.UPS.NO;
        

        private static OutputPort PinResetPowerBoard = new OutputPort(Pin.P0_11, true);


        #region VARIABLE

        public static BasicConfiguration _basicConfig;      
        private static MemoryConfigManager _configManagere;

        private static SDManager _sdManager;        
        private static LCDManager _lcdManager;
        private static DeeControlManager _deeManager;
        private static UpsManager _upsManager;
       
        private static SerialPort _uartPWB1;
        private static SerialPort _uartPWB2;
        private static SerialPort _rs232Pc;
        

        private static DateTime Date = new DateTime();
        private static TimeSpan StartTime = new TimeSpan();
        private static TimeSpan StopTime = new TimeSpan();

        public static Constants.EXTRUDER _actSelectExtrOnPowerboard = Constants.EXTRUDER.ExtruderPrimary;

        public static Timer RefreshTimer;

        #endregion

        #region THREAD SETTING

        private static Thread _ThreadRefreshStatePrinter = null;
       
        private const ThreadPriority PriorityRefreshPrintStateThread = ThreadPriority.Lowest;
       
        // cteni RS232 komunikace
        private const int TimoutSerialRs232 = 1;
        private const int TimeoutSerialUart = 1;
        

        private static Thread _ThreadReceiveSerialRs232Thread;
        private const ThreadPriority PrioritySerialRs232Thread = ThreadPriority.Normal;

        public static Thread _ThreadSenderReceiverSerialUart1;
        public static Thread _ThreadSenderReceiverSerialUart2;
        
        private const ThreadPriority PrioritySerialUartThread1 = ThreadPriority.Normal;
        private const ThreadPriority PrioritySerialUartThread2 = ThreadPriority.Normal;


        public static Thread _ThreadPrint;

        #endregion

        public static void Main()
        {
            InitConfiguration();   
            InitPowerManagement();
            InitLanguage();
            InitLcd();
            InitCommunication();
            InitEndSwitch();
            InitSd();
            InitUps();

            
            RefreshTimer = new Timer(new TimerCallback(GraphicsStatusRefresh),null,1000,1000);

            _deeManager = DeeControlManager.GetInstance();

            ConfigurationPrinter.GetInstance().verisonFWControl = VersionNumber; // nutne kvuli posilani Deecontrol

            HardwareResetPrinter1();
            //HardwareResetPrinter2();

            Debug.GC(true);

            Thread.Sleep(Timeout.Infinite);
        }

       
        #region INIT

        public static void InitConfiguration()
        {
            //load config
            _configManagere = new MemoryConfigManager();
            _basicConfig = new BasicConfiguration();

            byte[] serConfig = Reflection.Serialize(_basicConfig, typeof(BasicConfiguration));
            byte[] readData = _configManagere.Read(0, 0, (uint)serConfig.Length);

            Thread.Sleep(300);

            try
            {
                _basicConfig = (BasicConfiguration)Reflection.Deserialize(readData, typeof(BasicConfiguration));
            }
            catch (Exception)
            {
                _basicConfig = null;
                CreateNewBasicConfig(serConfig);
            }

            if (_basicConfig != null)
                Debug.Print("G120: EEPROM basicConfig read OK");
            else
            {
                Debug.Print("G120: EEPROM basicConfig create new basicConfig");
                _basicConfig = new BasicConfiguration();
            }

        }

        public static void InitPowerManagement()
        {
            var PCONP = new Register(0x400fc0c4);
            var PLL0CFG = new Register(0x400FC084);
            var PLL0FEED = new Register(0x400FC08C);

            PLL0CFG.Write(0x09); //Set MSEL = 6, M = 5,PSEL = 0(div by 1) CLK = 10*12/= 120MHz  PLL0FEED.Write(0xAA);
            PLL0FEED.Write(0x55); //Make it happen
            PCONP.Write(0xFFFEFFFF); //Back to normal

        }
        
       
        private static void InitLcd()
        {
            _lcdManager = LCDManager.GetInstance();
           
        }

        // init USB,RS232,ETH atd...
        private static void InitCommunication()
        {
            // PRINTER is UART1
            if (ConfigurationPrinter.GetInstance().GetConnectWithPrinter() == Constants.DEVICE_PRINTER.UART1)
            {

                //_uartPrinter = new SerialPort("COM4", 115200)
                _uartPWB1 = new SerialPort("COM4", 250000)
                    {
                        ReadTimeout = TimeoutSerialUart, 
                        WriteTimeout = TimeoutSerialUart,
                        DataBits = 8,
                        Parity = Parity.None,
                        StopBits = StopBits.One,
                        Handshake = Handshake.None
                    };
             
                _uartPWB1.ErrorReceived += _uartPrinter_ErrorReceived;
                _uartPWB1.Open();
            }

            // PRINTER is UART2
            if (ConfigurationPrinter.GetInstance().GetConnectWithPrinter() == Constants.DEVICE_PRINTER.UART2)
            {

                //_uartPrinter = new SerialPort("COM4", 115200)
                _uartPWB2 = new SerialPort("COM1", 250000)
                {
                    ReadTimeout = TimeoutSerialUart,
                    WriteTimeout = TimeoutSerialUart,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None
                };

                _uartPWB2.ErrorReceived += _uartPrinter_ErrorReceived;
                _uartPWB2.Open();
            }

            //PC is RS232
            if (ConfigurationPrinter.GetInstance().GetConnectWithPc() == Constants.PC.RS232)
            {
                _rs232Pc = new SerialPort("COM2", 115200) //puvodne com 1
                //_rs232Pc = new SerialPort("COM1", 57600)
                     {
                         ReadTimeout = TimoutSerialRs232,
                         WriteTimeout = TimoutSerialRs232,
                         DataBits = 8,
                         Parity = Parity.None,
                         StopBits = StopBits.One,
                         Handshake = Handshake.None              
                     };

                _rs232Pc.ErrorReceived += _rs232Pc_ErrorReceived;
                _rs232Pc.Open();

                _ThreadSenderReceiverSerialUart1 = new Thread(ThreadSenderReceiverDataForPrinterUart1);
                _ThreadSenderReceiverSerialUart1.Priority = PrioritySerialUartThread1;
                _ThreadSenderReceiverSerialUart1.Start();

                //_ThreadSenderReceiverSerialUart2 = new Thread(ThreadSenderReceiverDataForPrinterUart2);
                //_ThreadSenderReceiverSerialUart2.Priority = PrioritySerialUartThread2;
                //_ThreadSenderReceiverSerialUart2.Start();

                _ThreadReceiveSerialRs232Thread = new Thread(ThreadRs232ReceiveSerialData);
                _ThreadReceiveSerialRs232Thread.Priority = PrioritySerialRs232Thread;
                _ThreadReceiveSerialRs232Thread.Start();
                
            }
        }

        static void _rs232Pc_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Debug.Print("rs232 PC: " + e.EventType.ToString());

            if (StateHolder.GetInstance().FileDataTransfer)
            {
                SendDataToPc(DeeControlManager.GetInstance().StsSdWriteError());   
            }
        }

        static void _uartPrinter_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Debug.Print("UART PWB: " + e.EventType.ToString()); 
        }
        

        private static void InitEndSwitch()
        {
            SwitchManager.GetInstance();
        }

        private static void InitLanguage()
        {
            string prefix = StateHolder.GetInstance().GETCulturePrefix();
            if (prefix == null || prefix == "")
            {
                prefix = "en";
                SaveConfigToEEprom();
            }

            SelectCulture(prefix);

        }

        private static void InitSd()
        {
            _sdManager = SDManager.GetInstance();
            _sdManager.Init();
        }

        private static void InitUps()
        {
            if(UPS == Constants.UPS.OK)
                _upsManager = new UpsManager();
        }

        //nastavi jazykovou mutaci
        private static void ResetResourceManager()
        {
            FieldInfo fieldInfo =
                typeof(Resources).GetField("manager",
                                            BindingFlags.NonPublic | BindingFlags.Static);
            fieldInfo.SetValue(null, null);
        }

        private static CultureInfo SelectCulture(string name)
        {
            int i;
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

            CultureInfo ci = null;
            CultureInfo ciSelected = null;
            CultureInfo ciRoot = null;

            // parse: assume de-AT is correct format?
            string lang = "";
            string culture = null;
            if (name.Length >= 2)
            {
                lang = name.Substring(0, 2);
            }
            if (name.Length == 5)
            {
                culture = name.Substring(3, 2);
            }

            for (i = 0; i < cultures.Length; i++)
            {
                ci = cultures[i];

                //Debug.Print(i.ToString()+"] ci.Name=\"" + ci.Name + "\"\r\n");

                if (ci.Name.Equals(""))
                {
                    ciRoot = cultures[i];
                }

                if ((ciSelected == null) && (ci.Name.Length == 2) && (ci.Name.Equals(lang)))
                {
                    ciSelected = ci;
                }

                if (ci.Name == name)
                {
                    ciSelected = ci;
                    break;
                }
            }
            if (ciSelected == null) 
                ciSelected = ciRoot;

            ResourceUtility.SetCurrentUICulture(ciSelected);

            //we need to reset the resource manager because language was changed during runtime
            ResetResourceManager();

            return ciSelected;
        }
        
        #endregion

        public static bool _waitontemp = true;
        public static bool _waitonSdInit = true;
        public static bool _waitOnSdPrintStatus = true;

        #region THREAD

        private static bool _endThreadStateAndTemp = true;
        private static bool _endThreadUartPowerBoard1 = true;    // pouziti pri aktualizaci FW
        private static bool _endThreadUartPowerBoard2 = true; 

        private static int sleepWaitThreadState500ms = 500;
        private static int sleepWaitThreadState2000ms = 2000;

       // private static bool hasTemp = false;

        private static void ThreadRefreshStatePrinter()
        {
            _endThreadStateAndTemp = true;

            sleepWaitThreadState500ms = 500;
            sleepWaitThreadState2000ms = 2000;

            int counter = 0;
            const int maxcounter = 5;

            while (_endThreadStateAndTemp)
            {
                #region update temp nozzle and bed

                _waitontemp = true;

                GcodeManagere.GetInstance().GetExtrudersAndBedTemp();
                StateHolder.GetInstance().ActTempSpace = (int)TempManagere.GetInstance().GetAndRegulTempSpace();
               
                counter = 0;
                while (_waitontemp)
                {
                    Thread.Sleep(sleepWaitThreadState500ms);
                    counter++;
                    if (counter >= maxcounter)
                    {
                        _waitontemp = false;
                    }
                }

                #endregion

                #region update SD print status

                if (LCDManager.IsActivePrintInfoScreen && 
                    (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print && 
                    (StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Init || StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Print)))
                {
                    _waitOnSdPrintStatus = true;
                    _sdManager.GetSdPrintStatus();

                    while (_waitOnSdPrintStatus)
                    {
                        Thread.Sleep(sleepWaitThreadState500ms);
                        counter++;
                        if (counter >= maxcounter)
                            _waitOnSdPrintStatus = false;
                    }
                }

                #endregion

                #region SDCard files init

                if (LCDManager.IsActiveModelScreen) 
                {
                    _waitonSdInit = true;
                    _sdManager.InitSD();
                    
                    counter = 0;
                    while (_waitonSdInit)
                    {
                        Thread.Sleep(sleepWaitThreadState500ms);
                        counter++;
                        if (counter >= maxcounter)
                            _waitonSdInit = false;
                    }
                }

                #endregion

                #region state UPS
                
                if (UPS == Constants.UPS.OK && _upsManager != null)
                {
                   Ups.STATE stateUps = _upsManager.GetState();

                   if (stateUps == Ups.STATE.TrafficBatt)
                   {
                       LCDManager.GetInstance().UpdateStateUps(true);
                       //timer pro uspani tiskarny
                       _upsManager.runTimerPowerDown();
                   }

                   if (stateUps == Ups.STATE.LowBatt)
                   {
                       //co nejrychleji vypnout system
                       _upsManager.PowerDown();
                   }

                   if (stateUps == Ups.STATE.PowerFull)
                   {
                       LCDManager.GetInstance().UpdateStateUps(false);
                       _upsManager.stopTimerPowerDown();
                   }

                }

                #endregion

                if(_endThreadStateAndTemp)
                    Thread.Sleep(sleepWaitThreadState2000ms);

            }
        }

        public static void CreateThreadRefreshState()
        {
            Debug.Print("Creating thread RefreshState...");

            if(_ThreadRefreshStatePrinter != null)
                EndThreadRefreshState();

            _ThreadRefreshStatePrinter = new Thread(ThreadRefreshStatePrinter);
            _ThreadRefreshStatePrinter.Priority = PriorityRefreshPrintStateThread;
            _ThreadRefreshStatePrinter.Start();

            //pockej na ukonceni vlakna
            Thread.Sleep(1);         
        }

        public static void EndThreadRefreshState()
        {
            _endThreadStateAndTemp = false;
            sleepWaitThreadState500ms = 1;
            sleepWaitThreadState2000ms = 1;

            //pockej na ukonceni vlakna
            while (_ThreadRefreshStatePrinter != null && _ThreadRefreshStatePrinter.IsAlive)
            {
                _waitontemp = false;
                _waitonSdInit = false;
                _waitOnSdPrintStatus = false;

                Thread.Sleep(10);
            }

            _ThreadRefreshStatePrinter = null;
            Debug.Print("Thread RefreshState was ending...");
        }


        private static readonly Queue BufferUartSenderPrinter1 = new Queue();
        private static readonly Queue BufferUartSenderPrinter2 = new Queue();
        public static bool dataOK = false;
        public static bool resendData = false;    // preposle stary prikaz

        public static Queue GetBufferUartSenderPrinter1()
        {
            return BufferUartSenderPrinter1;
        }

        public static Queue GetBufferUartSenderPrinter2()
        {
            return BufferUartSenderPrinter2;
        }

        private static int watchdogCount = 0;

        private static void ThreadSenderReceiverDataForPrinterUart1()
        {
            const int numberOfData = 300;

            string gcode = "";
            bool isG1G0 = false;

            while (_endThreadUartPowerBoard1)
            {
                //Send data PWB1
                #region SendDataPowerBoard
                if (BufferUartSenderPrinter1.Count > 0 || resendData)
                {
                    if (!resendData)
                    {
                        #region filtrace dat

                        lock (BufferUartSenderPrinter1)
                        {
                            // pokud je resend nevyvedavej dalsi data
                            gcode = (string)BufferUartSenderPrinter1.Dequeue();
                        }

                        //filtr - pokud radek zacina ; zahod je to komentar
                        if (gcode == "" || gcode.Substring(0, 1) == ";")
                            continue;
                        //filtr - odstranuje komentare za prikazem
                        if(gcode.IndexOf(';') > -1) 
                            gcode = gcode.Substring(0, gcode.IndexOf(';'));
                        //filtr - odstranuje \r jako znak, ne pri prenosu dat
                        if (gcode.IndexOf('\r') > -1 && !StateHolder.GetInstance().FileDataTransfer)
                            gcode = gcode.Substring(0, gcode.IndexOf('\r'));

                        #endregion
                    }

                    dataOK = false;
                    resendData = false;
                    byte[] b;

                    if (StateHolder.GetInstance().FileDataTransfer)
                    {
                        #region convertFileDataTransfer
                        if (gcode.IndexOf("M29") > -1)
                        {
                            Debug.Print(gcode);
                            StateHolder.GetInstance().FileDataTransfer = false;
                        }

                        //pokud nema gcode 300 znaku doplni ho '\n'
                        if (gcode.Length < numberOfData)
                        {

                            int number = numberOfData - gcode.Length;
                            Debug.Print("doplnuji data: " + number);
                            for (int i = 0; i < number; i++)
                            {
                                gcode += Constants.chNln;
                            }
                        }

                        b = Encoding.UTF8.GetBytes(gcode + Constants.chDlr);
                        #endregion
                    }
                    else
                    {
                        #region convertNormalState
                        if (gcode.IndexOf("M28") > -1)
                        {
                            //zahajeni prenosu a ukladani dat
                            Debug.Print(gcode);
                            StateHolder.GetInstance().FileDataTransfer = true;
                        }

                        if (gcode.IndexOf("G1") > -1 || gcode.IndexOf("G0") > -1)
                            isG1G0 = true;

                        if (gcode.IndexOf("M105") > -1)
                            dataOK = true;

                        gcode = GcodeManagere.CreateGcodeForMarlin(gcode);
                        b = Encoding.UTF8.GetBytes(gcode + Constants.chNln);

                        if (gcode.IndexOf("M112") > -1)
                            GcodeManagere.SetCountInstruction(1);
                        #endregion
                    }

                    Debug.Print(gcode);       
                    _uartPWB1.Write(b, 0, b.Length);
                    Thread.Sleep(5);

                }
                #endregion

                watchdogCount = 0;

                //ReceiveData
                do
                {
                    ReadAndWorkUartPWB1();
                    
                    
                    watchdogCount++;

                    if (watchdogCount == 300 && (PrintThreadBufferActive || PrintThreadActive) && isG1G0)
                        dataOK = true;

                } 
                // pokud prenasime data cekej na ok nebo no, jinak pokracuj dal
                while ((StateHolder.GetInstance().FileDataTransfer && !dataOK) || ((PrintThreadActive && !dataOK) || (PrintThreadBufferActive && !dataOK)));

                isG1G0 = false;

                if (BufferUartSenderPrinter1.Count == 0)
                {
                    if (StateHolder.GetInstance().FileDataTransfer || PrintThreadBufferActive)
                        Debug.Print("wait on data...");
                    
                    Thread.Sleep(400);
                }

              
            }

            
        }
        //--------------------------------------------------------------------------------------------------------------

        private static void ThreadSenderReceiverDataForPrinterUart2()
        {
            const int numberOfData = 300;

            string gcode = "";
            bool isG1G0 = false;

            while (_endThreadUartPowerBoard2)
            {
                //Send data PWB2
                #region SendDataPowerBoard
                if (BufferUartSenderPrinter2.Count > 0 || resendData)
                {
                    if (!resendData)
                    {
                        #region filtrace dat

                        lock (BufferUartSenderPrinter2)
                        {
                            // pokud je resend nevyvedavej dalsi data
                            gcode = (string)BufferUartSenderPrinter2.Dequeue();
                        }

                        //filtr - pokud radek zacina ; zahod je to komentar
                        if (gcode == "" || gcode.Substring(0, 1) == ";")
                            continue;
                        //filtr - odstranuje komentare za prikazem
                        if (gcode.IndexOf(';') > -1)
                            gcode = gcode.Substring(0, gcode.IndexOf(';'));
                        //filtr - odstranuje \r jako znak, ne pri prenosu dat
                        if (gcode.IndexOf('\r') > -1 && !StateHolder.GetInstance().FileDataTransfer)
                            gcode = gcode.Substring(0, gcode.IndexOf('\r'));

                        #endregion
                    }

                    dataOK = false;
                    resendData = false;
                    byte[] b;

                    if (StateHolder.GetInstance().FileDataTransfer)
                    {
                        #region convertFileDataTransfer
                        if (gcode.IndexOf("M29") > -1)
                        {
                            Debug.Print(gcode);
                            StateHolder.GetInstance().FileDataTransfer = false;
                        }

                        //pokud nema gcode 300 znaku doplni ho '\n'
                        if (gcode.Length < numberOfData)
                        {

                            int number = numberOfData - gcode.Length;
                            Debug.Print("doplnuji data: " + number);
                            for (int i = 0; i < number; i++)
                            {
                                gcode += Constants.chNln;
                            }
                        }

                        b = Encoding.UTF8.GetBytes(gcode + Constants.chDlr);
                        #endregion
                    }
                    else
                    {
                        #region convertNormalState
                        if (gcode.IndexOf("M28") > -1)
                        {
                            //zahajeni prenosu a ukladani dat
                            Debug.Print(gcode);
                            StateHolder.GetInstance().FileDataTransfer = true;
                        }

                        if (gcode.IndexOf("G1") > -1 || gcode.IndexOf("G0") > -1)
                            isG1G0 = true;

                        if (gcode.IndexOf("M105") > -1)
                            dataOK = true;

                        gcode = GcodeManagere.CreateGcodeForMarlin(gcode);
                        b = Encoding.UTF8.GetBytes(gcode + Constants.chNln);

                        if (gcode.IndexOf("M112") > -1)
                            GcodeManagere.SetCountInstruction(1);
                        #endregion
                    }

                    Debug.Print(gcode);
                    _uartPWB2.Write(b, 0, b.Length);
                    Thread.Sleep(5);

                }
                #endregion

                watchdogCount = 0;

                //ReceiveData
                do
                {
                    ReadAndWorkUartPWB2();


                    watchdogCount++;

                    if (watchdogCount == 300 && (PrintThreadBufferActive || PrintThreadActive) && isG1G0)
                        dataOK = true;

                }
                // pokud prenasime data cekej na ok nebo no, jinak pokracuj dal
                while ((StateHolder.GetInstance().FileDataTransfer && !dataOK) || ((PrintThreadActive && !dataOK) || (PrintThreadBufferActive && !dataOK)));

                isG1G0 = false;

                if (BufferUartSenderPrinter2.Count == 0)
                {
                    if (StateHolder.GetInstance().FileDataTransfer || PrintThreadBufferActive)
                        Debug.Print("wait on data...");

                    Thread.Sleep(400);
                }


            }


        }

        //--------------------------------------------------------------------------------------------------------------
        public static void EndThreadSenderReceiverDataForPrinter()
        {
            _endThreadUartPowerBoard1 = false;

            //pockej na ukonceni vlakna
            while (_ThreadSenderReceiverSerialUart1 != null && _ThreadSenderReceiverSerialUart1.IsAlive)
            {
                Thread.Sleep(10);
            }
            Debug.Print("Thread ReceiverDataForPrinter was ending...");
        }

        public static bool PrintThreadActive = false;
        public static bool HeatingPowerboardActive = false;
        public static bool PrintThreadBufferActive = false;
        public static bool SendDataFromPC = false;


        //vladkno pro buffer tisk y DeeControl
        public static void StartThreadBufferPrint()
        {
            _ThreadPrint = new Thread(ThreadPrintBuffer);
            _ThreadPrint.Start();
        }

        private static void ThreadPrintBuffer()
        {
            Thread.Sleep(500);
            PrintThreadBufferActive = true;
            bool readFromPC = true;

            while (PrintThreadBufferActive)
            {
                //nacteni bloku dat a rozdeleni do radku
                if (readFromPC)
                {
                    //posleme prikaz pro nacteni instukcí
                    SendDataToPc("STS:BUF|DATA|20;");
                    readFromPC = false;

                    SendDataFromPC = false;

                    while (!SendDataFromPC) ;
                }

                if (BufferUartSenderPrinter1.Count < 20)
                {
                    StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Print;
                    readFromPC = true;
                }
                else
                {
                    Thread.Sleep(200);
                }
            }

            LCDManager.GetInstance().DonePrint();
        }
          
        #endregion

        static void GraphicsStatusRefresh(object o) 
        {
            if (LCDManager.IsActivePreheatScreen)
            {
                LCDManager.GetInstance().UpdatePreheatScreen();
            }

            if (LCDManager.IsActiveChangeFilamentScreen)
            {
                LCDManager.GetInstance().UpdateProgresBarCutGut();
            }

            if (LCDManager.IsActiveChangeExtrudingScreen)
            {
                LCDManager.GetInstance().UpdateProgresBarExtruding();
            }

            if (LCDManager.IsActivePrintInfoScreen)
            {
                LCDManager.GetInstance().UpdatePrintInfoScreen();
            }

            if (LCDWaitingScreen.disableIncrement == false)
                LCDWaitingScreen.GetInstance().IncrementProgress();

        }

        public static void StartWatherPrint()
        {
            StartTime = DateTime.Now.Subtract(Date);
            StopTime = new TimeSpan(0,0,0,1);
        }

        public static void StopWatcherPrint()
        {
            StopTime = DateTime.Now.Subtract(Date);

            if (StopTime < StartTime) 
            {
                Debug.Print("G120:" + "Chyba ulozeni casu tisku, zaporna hodnota");
                return;
            }

            if (_basicConfig.OperationTime > new TimeSpan(1825, 0, 0, 1))
            {
                _basicConfig.OperationTime = new TimeSpan(0, 0, 0, 1);
                if (_basicConfig.ReserveTime1 > new TimeSpan(1825, 0, 0, 0))
                {
                    _basicConfig.ReserveTime1 = new TimeSpan(0, 0, 0, 1);
                } 
                else 
                {
                    _basicConfig.OperationTime += _basicConfig.ReserveTime1;
                }
            }

            _basicConfig.OperationTime = _basicConfig.OperationTime + (StopTime - StartTime);
            _basicConfig.ReserveTime1 = _basicConfig.OperationTime;
            

            Debug.Print("G120:" + "Doba tisku:" + (StopTime - StartTime));
            SaveConfigToEEprom();
        }

        public static void ResetOperatingTime()
        {
            _basicConfig.OperationTime = new TimeSpan(0, 0, 1);
            SaveConfigToEEprom();
        }

        private static void CreateNewBasicConfig(byte[] serConfig)
        {
            Debug.Print("G120: Create new BasicConfig in EEPROM");

            _configManagere.Write(0, 0, serConfig, serConfig.Length);
            Thread.Sleep(500);

            try
            {
                byte[] readData = _configManagere.Read(0, 0, (uint)serConfig.Length);
                _basicConfig = (BasicConfiguration)Reflection.Deserialize(readData, typeof(BasicConfiguration));
            }
            catch (Exception)
            {
                _basicConfig = null;
            }

            if (_basicConfig == null)
                Debug.Print("G120: ERROR: Create new BasicConfig in EEPROM");

        }

        public static void SaveConfigToEEprom()
        {
            BasicConfiguration _testConfig = null;
            int count = 6;

            for (int i = 1; i < count; i++)
            {
                //vse v poradku ukonci ukladani
                if (_testConfig != null)
                {
                    Debug.Print("G120: Data konzistentni");
                    return;
                }

                Debug.Print("G120: Pokus o ulozeni do eeprom :" + i);


                //ukladani dat
                byte[] serConfig = Reflection.Serialize(_basicConfig, typeof(BasicConfiguration));
                _configManagere.Write(0, 0, serConfig, serConfig.Length);
    
                Thread.Sleep(300);

                //kontrola dat
                Debug.Print("G120: Kontrola dat");
                try
                {
                    byte[] readData = _configManagere.Read(0, 0, (uint)serConfig.Length);
                    _testConfig = (BasicConfiguration)Reflection.Deserialize(readData, typeof(BasicConfiguration));
                }
                catch (Exception)
                {
                    _testConfig = null;
                }
            }

            Debug.Print("G120: ERROR: DATA NEBYLO MOZNE ULOZIT !!!!");

        }

        public static void HardwareResetPrinter1()
        {
            Debug.Print("HardwareReset powerboard");

            EndThreadRefreshState(); // ukonci vlakno na cyklicke vycitani teploty

            SerialBufferUartPWB1.LoadSerial(_uartPWB1);
            SerialBufferUartPWB1.ClearBuffer();
            BufferUartSenderPrinter1.Clear();
            GcodeManagere.SetCountInstruction(1);
            Thread.Sleep(100);

            PinResetPowerBoard.Write(false);
            Thread.Sleep(100);

            PinResetPowerBoard.Write(true);
            Thread.Sleep(100);

            PrintThreadActive = false;

            StateHolder.GetInstance().IsMountingSD = false;
            StateHolder.GetInstance().ReadingFileListFromSD = false;
            StateHolder.GetInstance().ReadingParametrsFromFile = false;
            StateHolder.GetInstance().DataLoadingFromSD = false;
            StateHolder.GetInstance().DataParametersLoadingFromSD = false;

            CreateThreadRefreshState();

        }

        public static void HardwareResetPrinter2()
        {
            Debug.Print("HardwareReset powerboard");

            EndThreadRefreshState(); // ukonci vlakno na cyklicke vycitani teploty

            SerialBufferUartPWB2.LoadSerial(_uartPWB2);
            SerialBufferUartPWB2.ClearBuffer();
            BufferUartSenderPrinter2.Clear();
            GcodeManagere.SetCountInstruction(1);
            Thread.Sleep(100);

            PinResetPowerBoard.Write(false);
            Thread.Sleep(100);

            PinResetPowerBoard.Write(true);
            Thread.Sleep(100);

            PrintThreadActive = false;

            StateHolder.GetInstance().IsMountingSD = false;
            StateHolder.GetInstance().ReadingFileListFromSD = false;
            StateHolder.GetInstance().ReadingParametrsFromFile = false;
            StateHolder.GetInstance().DataLoadingFromSD = false;
            StateHolder.GetInstance().DataParametersLoadingFromSD = false;

            CreateThreadRefreshState();

        }

        public static void DiscartUartPrinter1()
        {
            _uartPWB1.DiscardInBuffer();
        }

        public static void DiscartUartPrinter2()
        {
            _uartPWB2.DiscardInBuffer();
        }


        #region DATA SEND

        public static void SendDataToPrinter1(string data)
        {
            lock (BufferUartSenderPrinter1)
            {
                BufferUartSenderPrinter1.Enqueue(data);
            }
        }

        public static void SendDataToPrinter2(string data)
        {
            lock (BufferUartSenderPrinter2)
            {
                BufferUartSenderPrinter2.Enqueue(data);
            }
        }

        public static void SendDataToPc(string data)
        {
            byte[] b = Encoding.UTF8.GetBytes(data);

            if (_rs232Pc.IsOpen)
                _rs232Pc.Write(b, 0, b.Length);
        }

        #endregion

        #region DATA RECEIVED

        private static readonly SerialBuffer SerialBufferRs232 = new SerialBuffer(1024);
        private static readonly SerialBuffer SerialBufferUartPWB1 = new SerialBuffer(1024);
        private static readonly SerialBuffer SerialBufferUartPWB2 = new SerialBuffer(1024);

        private static string dataCommandPC;
        private static string dataCommandPWB1;
        private static string dataCommandPWB2;


        private static void ThreadRs232ReceiveSerialData()
        {
            while (true)
            {
                ReadAndWorkUartPC();

                if (!StateHolder.GetInstance().FileDataTransfer)
                    Thread.Sleep(100);
            }
        }

        private static void ReadAndWorkUartPC()
        {
            if (_rs232Pc == null || !_rs232Pc.IsOpen)
                return;

            SerialBufferRs232.LoadSerial(_rs232Pc);
            
            //string dataCommand;
            while ((dataCommandPC = SerialBufferRs232.ReadCommand(Constants.EndCharRs232)) != null)
            {
                DeeControlManager.GetInstance().CommandWorker(dataCommandPC);
                // opetovne cteni portu abychom neztratili data
                SerialBufferRs232.LoadSerial(_rs232Pc);
            }
        }


        private static void ThreadUartPrinterReceiveSerialData()
        {

        }
         
        private static void ReadAndWorkUartPWB1()
        {
            if (_uartPWB1 == null || !_uartPWB1.IsOpen)
                return;

            SerialBufferUartPWB1.LoadSerial(_uartPWB1);
           
            while ((dataCommandPWB1 = SerialBufferUartPWB1.ReadLine()) != null)
            {
                MarlinManager.GetInstace().CommandWorkerUart(dataCommandPWB1);

                // opetovne cteni portu abychom neztratili data 
                SerialBufferUartPWB1.LoadSerial(_uartPWB1);
            }
        }

        private static void ReadAndWorkUartPWB2()
        {
            if (_uartPWB2 == null || !_uartPWB2.IsOpen)
                return;

            SerialBufferUartPWB2.LoadSerial(_uartPWB2);

            while ((dataCommandPWB2 = SerialBufferUartPWB2.ReadLine()) != null)
            {
                MarlinManager.GetInstace().CommandWorkerUart(dataCommandPWB2);

                // opetovne cteni portu abychom neztratili data 
                SerialBufferUartPWB2.LoadSerial(_uartPWB2);
            }
        }


    #endregion

    }
}
