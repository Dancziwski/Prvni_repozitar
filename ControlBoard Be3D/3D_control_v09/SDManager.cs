using System;
using Microsoft.SPOT;
using Be3D.Constants;

namespace _3D_control_v09
{
    class SDManager 
    {
        private static SDManager _instance;

        private SDManager()
        {

        }

        public static SDManager GetInstance()
        {
            if (_instance == null)
                _instance = new SDManager();

            return _instance;
        }

        private SDPowerBoard _sdBoard = null;
        
        public void Init()
        {
            _sdBoard = new SDPowerBoard();
        }

        public bool GetParametersFromFile()
        {
            Debug.Print("Get parameters form file...");

            bool state = false;

            StateHolder.GetInstance().DataParametersLoadingFromSD = false;
            StateHolder.GetInstance().ClearParametersFromListFiles();

            Program.EndThreadRefreshState();

            state = _sdBoard.ReadParametersFromFile();
            Program.CreateThreadRefreshState();

            return state;

        }

        public void GetNamesOfFilesFromSd()
        {
            _sdBoard.GetNamesOfFilesFromSd();
        }

        public void InitSD()
        {
            _sdBoard.InitSD();
        }

        public void ReleaseSDcard()
        {
            _sdBoard.ReleaseSDcard();
        }

        public bool SelectSdFileForPrint(string nameFile)
        {
            return _sdBoard.SelectSdFileForPrint(nameFile);
        }

        public void StartResumeSdPrint()
        {
            _sdBoard.StartResumeSdPrint();
        }

        public void PauseSdPrint()
        {
            _sdBoard.PauseSdPrint();
        }

        public void SetSdPosition(int position)
        {
            _sdBoard.SetSdPosition(position);
        }

        public void GetSdPrintStatus()
        {
            _sdBoard.GetSdPrintStatus();
        }

        public void CreateFileAndStartWriteDataToFile(string name)
        {
            _sdBoard.CreateFileAndStartWriteDataToFile(name);
        }

        public void AddDataToFile(string gcode)
        {
            _sdBoard.AddDataToFile(gcode);
        }

        public void EndWriteDataToFile(string name)
        {
            _sdBoard.EndWriteDataToFile(name);
        }

        public void DeleteFileFromSd(string name)
        {
            _sdBoard.DeleteFileFromSd(name);
        }

        public string[] ReadBlockForPrint()
        {
            return _sdBoard.ReadBlockForPrint();
        }

        public void GetStateSpaceSD()
        {
            _sdBoard.GetStateSpaceSD();
        }

    }
}
