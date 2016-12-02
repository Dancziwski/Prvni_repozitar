
using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace _3D_control_v09
{
    class SDControlBoard: SDIBoard
    {
        
        private SD_G120FileSystem g120FileSystem;


        public SDControlBoard()
        {
            g120FileSystem = new SD_G120FileSystem();
        }

        public void InitSD()
        {
            //mount SD card, 
            
            if (g120FileSystem.InitSD())
            {
                Program._waitonSdInit = false;
                StateHolder.GetInstance().IsMountingSD = true;

                if ((StateHolder.GetInstance().GetListFiles() == null || StateHolder.GetInstance().GetListFiles().Count == 0) &&
                    StateHolder.GetInstance().ReadingFileListFromSD == false)
                {
                    StateHolder.GetInstance().ReadingFileListFromSD = true;

                    if (!StateHolder.GetInstance().DataLoadingFromSD)
                    {
                          GetNamesOfFilesFromSd();
                    }

                    Program.SendDataToPc(DeeControlManager.GetInstance().StsSdMount());
                }
            }
            else
            {
                Program._waitonSdInit = false;

                StateHolder.GetInstance().IsMountingSD = false;
                StateHolder.GetInstance().ReadingFileListFromSD = false;
                StateHolder.GetInstance().ClearFilesFromListFiles();
                StateHolder.GetInstance().DataLoadingFromSD = false;

                if (LCDManager.IsActiveModelScreen)
                    LCDManager.GetInstance().UpdateModelScreen();

                Program.SendDataToPc(DeeControlManager.GetInstance().StsSdUnMount());
            }
        }

        public void ReleaseSDcard()
        {
            //unmount SD kartu
            //stejne funkce jako pri SDinitFail

        }


        public bool ReadParametersFromFile()
        {
            //nactu a cekam na data
            string pathPrintFile = g120FileSystem.GetRootDirectory() + @"\" + StateHolder.GetInstance().PrintFile;
            StreamReader FileReader = new StreamReader(pathPrintFile);

            for (int i = 0; i < 4; i++)
            {
                string line = FileReader.ReadLine();
                StateHolder.GetInstance().AddParametersToListFiles(line);
            }

            FileReader.Close();

            return true;
        }

        public void GetStateSpaceSD()
        {
            throw new Exception();
        }

        public void GetNamesOfFilesFromSd()
        {
            // nacte seznam souboru na SD karte
            // nutne zpacovat jako pri FilesBegin a END
            StateHolder.GetInstance().ReadingFileListFromSD = true;

            //nacteni souboru
            ArrayList list = g120FileSystem.LoadFilesList();
            if (list != null)
            {
                StateHolder.GetInstance().SetListFiles(list);
            }

            StateHolder.GetInstance().ReadingFileListFromSD = false;

            if (LCDManager.IsActiveModelScreen)
                LCDManager.GetInstance().UpdateModelScreen();

            StateHolder.GetInstance().DataLoadingFromSD = true;     

        }

        public bool SelectSdFileForPrint(string nameFile)
        {
            return g120FileSystem.OpenFileForRead(nameFile);
        }

        public string[] ReadBlockForPrint()
        {
            return g120FileSystem.ReadBlockDateFromSDSeparateLine();
        }

        public void StartResumeSdPrint()
        {
            //spusti tisk a čteni dat         
            Program.StartThreadPrint();
        }

        public void PauseSdPrint()
        {
           

        }

        public void SetSdPosition(int position)
        {
            
        }

        public void GetSdPrintStatus()
        {
            
        }

        public void CreateFileAndStartWriteDataToFile(string name)
        {
            
        }

        public void AddDataToFile(string gcode)
        {
            
        }

        public void EndWriteDataToFile(string name)
        {
            
        }

        public void DeleteFileFromSd(string name)
        {
            
        }

    }
}
