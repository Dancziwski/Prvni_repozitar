using System;
using System.Threading;
using Be3D.Constants;
using GHI.Hardware.G120;
using Microsoft.SPOT.Hardware;


namespace _3D_control_v09
{
    internal class TempManagere
    {
        private static TempManagere _instance;

        private readonly TempSpaceSensor _tempSpace;

        private static OutputPort OutSpacePin = new OutputPort(Pin.P1_9, false);
        private static OutputPort OutFanSpacePin = new OutputPort(Pin.P1_10, false);

        private static Cpu.AnalogChannel AnalogInput1 = Cpu.AnalogChannel.ANALOG_6;

        private TempManagere()
        {
            _tempSpace = new TempSpaceSensor(AnalogInput1);          
        }

        public static TempManagere GetInstance()
        {
            if(_instance == null)
                _instance = new TempManagere();

            return _instance;
        }

        public float GetAndRegulTempSpace()
        {
            float actTemp = 0;

            if (ConfigurationPrinter.GetInstance().IsPresentSpaceHeat())
            {
                actTemp = _tempSpace.ReadTemp();
                int setTempSpace = StateHolder.GetInstance().ActSetTempSpace;

                if(StateHolder.GetInstance().StateHeating || StateHolder.GetInstance().ActState == Constants.ACTUAL_STATE.Print)
                    RegulationTemp(OutSpacePin, actTemp, setTempSpace, 5,false);
                else
                    OutSpacePin.Write(false);

                // vzajemna blokace pokud je zapnuty vyhrev prostoru netopi
                if (setTempSpace == 0) // setTemp <= 0 blokuje vzajemnou koleraci s topenim aby nebezeli soucasne
                    RegulationTemp(OutFanSpacePin, actTemp, 10, 5, true);
                else
                    OutFanSpacePin.Write(false);
            }

            return actTemp;
        }

        private bool RegulationTemp(OutputPort outputPin, float actTemp, int setTemp, int offsetToMin, bool invertLogic)
        {
            if ((int)actTemp >= setTemp)
            {   
                if(invertLogic)
                    outputPin.Write(true);
                else
                    outputPin.Write(false);
                return true;
            }
            else
            {    // zapinam tolerantni -5C
                if (actTemp <= (setTemp - offsetToMin))
                {
                    if (invertLogic)
                        outputPin.Write(false);
                    else
                        outputPin.Write(true);
                    
                    return false;
                }
            }

            return false;
        }

        public void IncrementFailCounter()
        {
            _tempSpace.FailConnectCounter++;
        }

        public int GetFailCounter()
        {
            return _tempSpace.FailConnectCounter;
        }

        public void ClearFailCounter()
        {
            _tempSpace.FailConnectCounter = 0;
        }
    
        
    }
}
