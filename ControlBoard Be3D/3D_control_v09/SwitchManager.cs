using System.Threading;
using LCD;
using Microsoft.SPOT.Hardware;
using Be3D.Constants;
using GHI.Hardware.G120;
using Microsoft.SPOT;

namespace _3D_control_v09
{

    class SwitchManager
    {
        private static SwitchManager _instance;

        private InterruptPort SwitchDoor1 = new InterruptPort(Pin.P0_0, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeBoth);
        private InterruptPort SwitchDoor2 = new InterruptPort(Pin.P2_11, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeBoth);
        private InterruptPort SwitchFilament1 = new InterruptPort(Pin.P0_27, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeBoth);
        private InterruptPort SwitchFilament2 = new InterruptPort(Pin.P0_28, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeBoth);

        private const int LockTimeout = 500;

        private bool _lockInteruptDoor1;
        private bool _lockInteruptDoor2;
        private bool _lockInteruptFil1;
        private bool _lockInteruptFil2;

        private ConfigurationPrinter configPrinter;

        private SwitchManager()
        {
            configPrinter = ConfigurationPrinter.GetInstance();

            if(configPrinter.IsPresentSwitchDoor1())
                SwitchDoor1.OnInterrupt += SwitchDoor1OnInterrupt;

            if(configPrinter.IsPresentSwitchDoor2())
                SwitchDoor2.OnInterrupt += SwitchDoor2OnInterrupt;

            if(configPrinter.IsPresentSwitchFilament1())
                SwitchFilament1.OnInterrupt += SwitchFilament1OnInterrupt;

            if(configPrinter.IsPresentSwitchFilament2())
                SwitchFilament2.OnInterrupt += SwitchFilament2OnInterrupt;

        }

        public static SwitchManager GetInstance()
        {
            if(_instance == null)
                _instance = new SwitchManager();

            return _instance;
        }


        public bool IsOpenDoor1()
        {
            if (configPrinter.IsPresentSwitchDoor1() == false) // pokud neni fyzicky v tiskarne koncak ignoruj check a hlas zavrene dvere
                return false;
            if (!SwitchDoor1.Read())       // otevrene dvere True, zavrene false;
                return true;

            return false;
        }

        public bool IsOpenDoor2()
        {
            if (configPrinter.IsPresentSwitchDoor2() == false) // pokud neni fyzicky v tiskarne koncak ignoruj check a hlas zavrene dvere
                return false;
            if (!SwitchDoor2.Read())       // otevrene dvere false, zavrene true;
                return true;

            return false;
        }

        public bool IsFilamentExt1()
        {
            if (configPrinter.IsPresentSwitchFilament1() == false)
                return true;
            if (SwitchFilament1.Read())
                return true;

            return false;
        }

        public bool IsFilamentExt2()
        {
            if (configPrinter.IsPresentSecondaryExtruder() == false)
                return true;
            if (configPrinter.IsPresentSwitchFilament2() == false)
                return true;

            if (SwitchFilament2.Read())
                return true;

            return false;
        }


        private void StopMotion()
        {
            LCDManager.GetInstance().MoveStop();
        }

        private void HardStopPrinting(LCDScreen.StateScreen origScreen, Constants.ACTUAL_STATE state)
        {
            LCDManager.GetInstance().StopPrint();
            
            if (state == Constants.ACTUAL_STATE.Print)
            {
                LCDManager.GetInstance().ShowErrorScreenLcd(origScreen, Resources.GetString(Resources.StringResources.TextErrorStopPrinting), "", true);
            }

            if (state == Constants.ACTUAL_STATE.MotionTest || state == Constants.ACTUAL_STATE.ParkingPosition)
            {
                LCDManager.GetInstance().ShowErrorScreenLcd(origScreen, Resources.GetString(Resources.StringResources.TextErrorStopMove), "", true);
            }
        }

       
        private void SwitchDoor1OnInterrupt(uint data1, uint data2, System.DateTime time)
        {
            if (StateHolder.GetInstance().FileDataTransfer)
                return;

            if (!_lockInteruptDoor1)
            {
                _lockInteruptDoor1 = true;
                new Thread(ThreadLockSwitchDoor1).Start();
            }
        }

        private void SwitchDoor2OnInterrupt(uint data1, uint data2, System.DateTime time)
        {
            if (StateHolder.GetInstance().FileDataTransfer)
                return;

            if (!_lockInteruptDoor2)
            {
                _lockInteruptDoor2 = true;
                new Thread(ThreadLockSwitchDoor2).Start();
            }
        }


        private void ThreadLockSwitchDoor1()
        {
            Thread.Sleep(LockTimeout);
            _lockInteruptDoor1 = false;

            if (configPrinter.IsPresentSwitchDoor1())
            {
                ProcessSwitchDoorAndCeiling(true);
            }
        }

        private void ThreadLockSwitchDoor2()
        {
            Thread.Sleep(LockTimeout);

            _lockInteruptDoor2 = false;

            if (configPrinter.IsPresentSwitchDoor2())
            {
                ProcessSwitchDoorAndCeiling(false);
            }

        }


        //door is true / ceiling is false
        private void ProcessSwitchDoorAndCeiling(bool doorOrCeiling)
        {
            Constants.PRINT_STATE printState = StateHolder.GetInstance().ActPrintState;
            Constants.ACTUAL_STATE actualState = StateHolder.GetInstance().ActState;

            Debug.Print("actSTS: " + actualState);
            Debug.Print("printSTS: " + printState);

            //menim strunu nedelej nic
            if (LCDManager.IsActiveChangeFilamentScreen)
                return;
            if (LCDManager.IsActiveChangeExtrudingScreen)
                return;

            //test pohybu os
            if (actualState == Constants.ACTUAL_STATE.MotionTest)
            {
                StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Idle;
                StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Init;

                LCDWaitingScreen.disableIncrement = true;

                Program.HardwareResetPrinter1();
                LCDManager.GetInstance().UpdateScreenLcd(LCDManager.GetInstance().GetMainScreen(), "");

                return;
            }

            if (actualState == Constants.ACTUAL_STATE.ParkingPosition)
            {
                StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Idle;
                StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Init;

                StopMotion();
                UpsManager.correctReadPositonAxis = true;
                //HardStopPrinting(LCDScreen.StateScreen.Main, Constants.ACTUAL_STATE.ParkingPosition);
                return;
            }

            //tisk ukoncen tlacitkem stop
            if (actualState == Constants.ACTUAL_STATE.Idle && printState == Constants.PRINT_STATE.Stop)
            {
                StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Idle;
                StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Init;

                StopMotion();
                //HardStopPrinting(LCDScreen.StateScreen.Model, Constants.ACTUAL_STATE.Print);
                return;
            }

            // spusten tisk, ale jeste kalibruje nutny hard reset
            if (actualState == Constants.ACTUAL_STATE.Print && printState == Constants.PRINT_STATE.Init)
            {
                StateHolder.GetInstance().ActState = Constants.ACTUAL_STATE.Idle;
                StateHolder.GetInstance().ActPrintState = Constants.PRINT_STATE.Init;

                StateHolder.GetInstance().FilChangeCorrect = false;

                HardStopPrinting(LCDScreen.StateScreen.Model, Constants.ACTUAL_STATE.Print);
                return;
            }

            // bezny tisk pausuj
            if (actualState == Constants.ACTUAL_STATE.Print && printState == Constants.PRINT_STATE.Print)
            {
                
                //zastavi tisk
                LCDManager.GetInstance().PausePrint();

                LCDManager.IsActivePrintInfoScreen = false;

                //door is true / ceiling is false
                if(doorOrCeiling)
                    LCDManager.GetInstance().UpdateScreenLcd(LCDScreen.StateScreen.PausePrint, Resources.GetString(Resources.StringResources.TextDoorOpen));
                else
                    LCDManager.GetInstance().UpdateScreenLcd(LCDScreen.StateScreen.PausePrint, Resources.GetString(Resources.StringResources.TextCeilingOpen));
                return;
            }

            //print/stop
            if (actualState == Constants.ACTUAL_STATE.Print && printState == Constants.PRINT_STATE.Stop)
            {
                //nedelej nic
                return;
            }

            //tisk skoncil, cekam na reakci uzivatele
            if (actualState == Constants.ACTUAL_STATE.Print && printState == Constants.PRINT_STATE.Done)
            {
                StopMotion();
            }

        }

       

        private void SwitchFilament1OnInterrupt(uint data1, uint data2, System.DateTime time)
        {
            if (StateHolder.GetInstance().FileDataTransfer)
                return;

            if (!_lockInteruptFil1)
            {
                _lockInteruptFil1 = true;
                new Thread(ThreadLockSwitchFil1).Start();
            }
        }

        private void SwitchFilament2OnInterrupt(uint data1, uint data2, System.DateTime time)
        {
            if (StateHolder.GetInstance().FileDataTransfer)
                return;

            if (!_lockInteruptFil2)
            {
                _lockInteruptFil2 = true;
                new Thread(ThreadLockSwitchFil2).Start();
            }
        }

        private void ThreadLockSwitchFil1()
        {
            Thread.Sleep(LockTimeout);

            _lockInteruptFil1 = false;

            if (configPrinter.IsPresentSwitchFilament1())
            {
                proccesSwitchFilament(Resources.GetString(Resources.StringResources.TextNoMaterialExtr1));
            }
        }

        private void ThreadLockSwitchFil2()
        {
            Thread.Sleep(LockTimeout);

            _lockInteruptFil2 = false;

            if (configPrinter.IsPresentSwitchFilament2())
            {
                proccesSwitchFilament(Resources.GetString(Resources.StringResources.TextNoMaterialExtr2));
            }
        }

        private void proccesSwitchFilament(string msg)
        {
            //udalost dosla struna
            if (StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print && StateHolder.GetInstance().ActPrintState == Constants.PRINT_STATE.Print)
            {
                StateHolder.GetInstance().FilChangeCorrect = false;
                LCDManager.IsActivePrintInfoScreen = false;
                //zastavi tisk
                LCDManager.GetInstance().PausePrint();
                LCDManager.GetInstance().UpdateScreenLcd(LCDScreen.StateScreen.PausePrint, msg);
            }
        }
    }

}
