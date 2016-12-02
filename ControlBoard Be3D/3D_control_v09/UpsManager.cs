using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Be3D.Constants;
using GHI.Hardware.G120;

namespace _3D_control_v09
{
    class Ups
    {
        public static STATE actState = STATE.PowerFull; 

        public enum STATE
        {
            TrafficBatt = 0,
            LowBatt = 1,
            PowerFull = 2,
        }
    }

    class UpsManager
    {
        InputPort inputTrafficBatt;
        InputPort inputLowBatt;
        public static OutputPort upsEnable;

        public static bool correctReadPositonAxis = false;
        public static bool correctReadSDPosition = false;

        
        public UpsManager()
        {
            inputTrafficBatt = new InputPort(Pin.P0_18, true, Port.ResistorMode.PullUp);
            inputLowBatt = new InputPort(Pin.P0_1, false, Port.ResistorMode.PullUp);
            upsEnable = new OutputPort(Pin.P0_15, true);
        }

        bool tick = false;
        bool activatePowerDown = false;
        int timeOfPowerDown = 0;
        int seconds = 600; //hodina
        
        private void watchdogUps() 
        {
            GcodeManagere.GetInstance().SetFanSpeed(127);

            tick = true;
            activatePowerDown = false;

            timeOfPowerDown = seconds / 5;

            while (tick)
            {
                Thread.Sleep(5000);

                if (timeOfPowerDown <= 0)
                {
                    tick = false;
                    activatePowerDown = true;
                }

                timeOfPowerDown--;

            }

            if (activatePowerDown)
            {
                PowerDown();
            }

        }

        public void runTimerPowerDown()
        {
            if (!tick && !activatePowerDown && StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Print)
            {
                Thread thr = new Thread(watchdogUps);
                thr.Start();
            }
        }

        public void stopTimerPowerDown()
        {
            tick = false;
            activatePowerDown = false;
        }


        public void PowerDown()
        {
            correctReadPositonAxis = false;
            correctReadSDPosition = false;

            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print && StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Print)
            {
                LCDManager.GetInstance().PausePrint();

                LCDManager.IsActivePrintInfoScreen = false;
   
                SavePrintsParametrToMemory();

                GcodeManagere.GetInstance().SetFanSpeed(0);

                //nacitaji se v Marlin manageru
                if (correctReadPositonAxis && correctReadSDPosition)
                {                  
                    LCDManager.GetInstance().UpdateScreenLcd(LCD.LCDScreen.StateScreen.PausePrint, Resources.GetString(Resources.StringResources.TextCorrectSaveParam));
                }
                else
                {
                    LCDManager.GetInstance().UpdateScreenLcd(LCD.LCDScreen.StateScreen.PausePrint, Resources.GetString(Resources.StringResources.TextNoCorrectSaveParam));
                }
            }

        }

        public static void SavePrintsParametrToMemory()
        {
            if (Program.UPS == Constants.UPS.NO)
                return;

            Program._basicConfig.SetStrReserve1(StateHolder.GetInstance().PrintFile);

            int counter = 0;
            int maxCounter = 600;

            GcodeManagere.GetInstance().GetCurrentPosition();

            while (!correctReadPositonAxis)
            {
                Thread.Sleep(1000);
                counter++;
                if (counter >= maxCounter)
                {
                    correctReadPositonAxis = true;
                }
            }

            counter = 0;
            SDManager.GetInstance().GetSdPrintStatus();

            while (!correctReadSDPosition)
            {
                Thread.Sleep(1000);
                counter++;
                if (counter >= maxCounter)
                {
                    correctReadSDPosition = true;
                }
            }

            // ulozeni teplot extruderu        
            Program._basicConfig.IntReserve1 = StateHolder.GetInstance().ActSetTempInPowerBordPrimExt;
            Program._basicConfig.IntReserve2 = StateHolder.GetInstance().ActSetTempInPowerBordSecundExt;


            if(correctReadPositonAxis && correctReadSDPosition)
                Program.SaveConfigToEEprom();

        }

        public static void LoadPrintsParametrFromMemory()
        {
            //nacte printfile a inicializace printfile
            StateHolder.GetInstance().PrintFile = Program._basicConfig.GetStrReserve1();
            
            //nacte vstupni parametry souboru pro tisk a kontrolu kompatibility
            SDManager.GetInstance().GetParametersFromFile();

            //nastavy posledni pozice SD
            SDManager.GetInstance().SetSdPosition(Program._basicConfig.PostitionsInPrintfileOnSd);
            
            //nastaveni spravne pozice os
            GcodeManagere.GetInstance().SetCurrentPosition(Program._basicConfig.LatestPositionExtruder);
           
            //zacne nahrivat
            if(Program._basicConfig.IntReserve1 >= 50)
                GcodeManagere.GetInstance().SetExtruderTempAndWait(Constants.EXTRUDER.ExtruderPrimary, Program._basicConfig.IntReserve1);
            if(Program._basicConfig.IntReserve2 >=50)
                GcodeManagere.GetInstance().SetExtruderTempAndWait(Constants.EXTRUDER.ExtruderSecondary, Program._basicConfig.IntReserve2);

            //prepne spravny nastroj, kterym na konci prestal
            if (Program._basicConfig.LatestPositionExtruder[3] != 0.0)
                GcodeManagere.GetInstance().SetActExtruder(Constants.EXTRUDER.ExtruderPrimary);
            if(Program._basicConfig.LatestPositionExtruder[4] != 0.0)
                GcodeManagere.GetInstance().SetActExtruder(Constants.EXTRUDER.ExtruderSecondary);

            GcodeManagere.GetInstance().SetFanSpeed(255);

            Program._basicConfig.SetStrReserve1("");
            Program._basicConfig.PostitionsInPrintfileOnSd = 0;
            Program._basicConfig.LatestPositionExtruder = new double[5] { 0.0, 0.0, 0.0, 0.0, 0.0 };
            Program._basicConfig.IntReserve1 = 0;
            Program._basicConfig.IntReserve2 = 0;

            Program.SaveConfigToEEprom();

        }

        public Ups.STATE GetState()
        {
            // 0 v poradku, 1 sepnuto/low bat
            Ups.actState = Ups.STATE.PowerFull;

            if (inputTrafficBatt.Read() && !inputLowBatt.Read())
            {
                Ups.actState = Ups.STATE.TrafficBatt;
            }

            if (inputTrafficBatt.Read() && inputLowBatt.Read())
            {
                Ups.actState = Ups.STATE.LowBatt;
            }
            
            return Ups.actState;

        }

    }
}
