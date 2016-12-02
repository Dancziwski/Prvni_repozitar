
using System.Collections;
using System.Threading;

namespace _3D_control_v09
{
    class SDPowerBoard
    {
       
        public SDPowerBoard()
        {
           
        }

        public  bool ReadParametersFromFile()
        {
            string code = "M34 " + StateHolder.GetInstance().PrintFile.ToLower() + " ";

            Program.SendDataToPrinter(code);

            for (int i = 0; i < 50; i++)
            {
                if (StateHolder.GetInstance().DataParametersLoadingFromSD)
                   return true;
                
                Thread.Sleep(100);
            }

            return false;
        }

        public  void GetNamesOfFilesFromSd()
        {
            //M20
            //ok Files: {SQUARE.G,SQCOM.G,}

            //string code = Gcode.CreateGcodeForMarlin("M20");
            string code = "M20";
            Program.SendDataToPrinter(code);

        }

        public  void InitSD()
        {
            //M21
            //string code = Gcode.CreateGcodeForMarlin("M21");
            string code = "M21";
            Program.SendDataToPrinter(code);
        }

        public  void ReleaseSDcard()
        {
            //M22
            //string code = Gcode.CreateGcodeForMarlin("M22");
            string code = "M22";
            Program.SendDataToPrinter(code);
        }

        public void GetStateSpaceSD()
        {
            string code = "M33";
            Program.SendDataToPrinter(code);
        }

        public  bool SelectSdFileForPrint(string nameFile)
        {
            //M23 filename.gco
            nameFile = nameFile.ToLower();

            // nesmi byt zadny znak na konci navic
            nameFile = nameFile.Trim();

            //string code = Gcode.CreateGcodeForMarlin("M23 " + nameFile);
            string code = "M23 " + nameFile;
            Program.SendDataToPrinter(code);

            return true;
        }

        public  void StartResumeSdPrint()
        {
            //M24
            //string code = Gcode.CreateGcodeForMarlin("M24");
            string code = "M24";
            Program.SendDataToPrinter(code);

        }

        public  void PauseSdPrint()
        {
            //M25
            //string code = Gcode.CreateGcodeForMarlin("M25");
            string code = "M25";
            Program.SendDataToPrinter(code);
        }

        public  void SetSdPosition(int position)
        {
            //M26 S12345
            //string code = Gcode.CreateGcodeForMarlin("M26 S" + position);
            string code = "M26 S" + position + " ";
            Program.SendDataToPrinter(code);
        }

        public  void GetSdPrintStatus()
        {
            //M27 
            //string code = Gcode.CreateGcodeForMarlin("M27");
            string code = "M27";
            Program.SendDataToPrinter(code);
        }
      
        public void CreateFileAndStartWriteDataToFile(string name)
        {
            //string code = Gcode.CreateGcodeForMarlin("M28 " + name);
            string code = "M28 " + name + " ";
            Program.SendDataToPrinter(code);
        }

        public void AddDataToFile(string gcode)
        {
            Program.SendDataToPrinter(gcode);
        }

        public void EndWriteDataToFile(string name)
        {
            string code = "M29 " + name + " ";
            Program.SendDataToPrinter(code);
        }

        public void DeleteFileFromSd(string name)
        {
            //string code = Gcode.CreateGcodeForMarlin("M30 " + name);
            string code = "M30 " + name + " ";
            Program.SendDataToPrinter(code);

        }

        public string[] ReadBlockForPrint()
        {
            return null;
        }
    }
}
