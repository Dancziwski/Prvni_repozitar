using System;
using System.Text;
using System.Threading;
using Microsoft.SPOT.Hardware;

namespace _3D_control_v09
{
    class TempSpaceSensor
    {

        public static AnalogInput Pt100Input1;
        public int FailConnectCounter = 0;
        

        public TempSpaceSensor(Cpu.AnalogChannel channel)
        {
            Pt100Input1 = new AnalogInput(Cpu.AnalogChannel.ANALOG_6);

        }

        public float ReadTemp()
        {
            return ReadTempFromAnalogInput(Pt100Input1);
        }

        private const double A = 3.9083E-3;
        private const double B = -5.775E-7;

        private const int Rpt1000_0 = 1000;
        private const int Rpt100_0 = 100;

        private const int R = 327;

        private const double Urefp = 3.3;
        private const double UnapDel = 3.3;

        private const int corectionTempCels = -1;

        private float ReadTempFromAnalogInput(AnalogInput input)
        {
            const int numberOfDiametral = 5;

            double voltage = 0;
            for (int i = 0; i < numberOfDiametral; i++)
            {
                voltage = voltage + input.Read();
            }
            voltage = voltage / numberOfDiametral;

            //return CountPt1000TempOfVoltage(voltage);
            return CountPt100TempOfVoltage(voltage);
        }

        private float CountPt1000TempOfVoltage(double voltage)
        {

            voltage = voltage * Urefp; // dle dokumentace pro prepocet napeti

            double Rpt = (voltage) / ((UnapDel - voltage) / R);

            //  Debug.Print("Napet: " + voltage);
            //  Debug.Print("Rpt:" + Rpt);

            double[] temps = SolveQuadraticEquation(B, A, (1 - (Rpt / Rpt1000_0)));

            foreach (var temp in temps)
            {
                if (temp > 0 && temp < 300)
                    return (float)Math.Round(temp) + corectionTempCels;
            }

            return 0;
        }

        private float CountPt100TempOfVoltage(double voltage)
        {

            voltage = voltage * Urefp; // dle dokumentace pro prepocet napeti

            double Rpt = 0;

            if(((UnapDel - voltage) / R) != 0) // ochrana deleni nulou
                Rpt = (voltage) / ((UnapDel - voltage) / R);
            else 
                return 0;

            //Debug.Print("Napet: " + voltage);
            //Debug.Print("Rpt:" + Rpt);

            double[] temps = SolveQuadraticEquation(B, A, (1 - (Rpt / Rpt100_0)));

            if (temps != null)
            {
                foreach (var temp in temps)
                {
                    if (temp > 0 && temp < 300)
                        return (float) Math.Round(temp) + corectionTempCels;
                }
            }
            return 0;
        }

                /**
        * Resi kvadratickou rovnici o jedne nezname ve tvaru
        * ax^2 + bx + c = 0
        * @param a
        * @param b
        * @param c
        * @return pole realnych korenu, null - pokud nema rovnice reseni v oboru
        * realnych cisel
        */
        public double[] SolveQuadraticEquation(double a, double b, double c)
        {
            double d = b * b - 4 * a * c; //diskriminant
            if (d < 0)
            {
                return null;
            }
            else if (d == 0)
            {
                double[] result = { -b / 2 * a };
                return result;
            }
            else
            {
                double[] result = { (-b + Math.Sqrt(d)) / (2 * a), (-b - Math.Sqrt(d)) / (2 * a) };
                return result;
            }
        }



    }
}
