using System;
using Microsoft.SPOT;
using System.IO;
using GHI.Premium.System;

namespace _3D_control_v09
{
    class UpdateManager
    {
        private static UpdateManager _instance;

        string pathFileFirm = @"SD\update\Firmware.hex";
        string pathFileFirm2 = @"SD\update\Firmware2.hex";
        string pathFileConf = @"SD\update\Config.hex";
        string pathFileApp = @"SD\update\testFirmware.hex";

        private UpdateManager()
        {

        }

        public static UpdateManager GetInstance()
        {
            if (_instance == null)
                _instance = new UpdateManager();

            return _instance;
        }


        public static string calculateCRC(byte[] _arr)
        {
            string crc ="*";
            int _cs = 0;
            
            for (int i = 0; i < _arr.Length; i++)
            {
                _cs = _cs ^ _arr[i];
            }

            _cs &= 0xff;

            return crc + _cs.ToString();
            // vystup ve tvaru "*98"

        }

     
        private bool LoadHexFWFirmwareFromSD(string pathFileFirm)
        {
            if (!File.Exists(pathFileFirm))
                return false;

            // Load in the 1st firmware file
            LoadDataFromSD(pathFileFirm, SystemUpdate.SystemUpdateType.Firmware);
          
            return true;
        }

        private bool LoadHexFWFirmware2FromSD(string pathFileFirm2)
        {
            if (!File.Exists(pathFileFirm2))
                return false;

            // Load in the 2nd firmware file
            LoadDataFromSD(pathFileFirm2, SystemUpdate.SystemUpdateType.Firmware);
           
            return true;
        }

        private bool LoadHexFWConfigFromSD(string pathFileConf)
        {
            if (!File.Exists(pathFileConf))
                return false;

            // Load in the Configuration file
            LoadDataFromSD(pathFileConf, SystemUpdate.SystemUpdateType.Config);
            
                return true;
        }

        private bool LoadHexFWAppFromSD(string pathFileApp)
        {
            if (!File.Exists(pathFileApp))
                return false;

            // Load in your newly created application
            LoadDataFromSD(pathFileApp, SystemUpdate.SystemUpdateType.Deployment);
            return true;
        }


        public bool RunUpdateFirmwareFromSD()
        {
            try
            {
                SystemUpdate.Initialize(SystemUpdate.SystemUpdateType.Firmware);

                if(LoadHexFWFirmwareFromSD(pathFileFirm) && LoadHexFWFirmware2FromSD(pathFileFirm2))
                    return FlashAndReset();
                else 
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RunUpdateConfigFromSD()
        {
            try
            {
                SystemUpdate.Initialize(SystemUpdate.SystemUpdateType.Config);

                if (LoadHexFWConfigFromSD(pathFileConf))
                    return FlashAndReset();
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RunUpdateDeploymentFromSD()
        {
            try
            {
                SystemUpdate.Initialize(SystemUpdate.SystemUpdateType.Deployment);

                if (LoadHexFWAppFromSD(pathFileApp))
                    return FlashAndReset();
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RunUpdateAllFromSD()
        {
            try
            {
                SystemUpdate.Initialize(SystemUpdate.SystemUpdateType.Config | SystemUpdate.SystemUpdateType.Deployment | SystemUpdate.SystemUpdateType.Firmware);

                if (LoadHexFWFirmwareFromSD(pathFileFirm) && LoadHexFWFirmware2FromSD(pathFileFirm2) && LoadHexFWConfigFromSD(pathFileConf) && LoadHexFWAppFromSD(pathFileApp))
                    return FlashAndReset();
                return
                    false;
            }
            catch (Exception)
            {
                return false;
            }
        }





        public bool ActivateUpdateOnTheFlight(SystemUpdate.SystemUpdateType type) 
        {
            try
            {
                SystemUpdate.Initialize(type);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddDataToFlash(SystemUpdate.SystemUpdateType type, byte[] data)
        {
            if (data == null)
                return false;
            try
            {
                SystemUpdate.Load(type, data, data.Length);
                return true;
            } 
            catch(Exception) 
            {
                return false;
            }
        }

        public bool RunUpdateOnTheFlight()
        {
            try
            {
                return FlashAndReset();
            }
            catch (Exception)
            {
                return false;
            }
        }


        private bool FlashAndReset()
        {

            SystemUpdate.FlashAndReset();
            return true;

        }

       


        private const int BLOCK_SIZE = 10 * 1024;
        private static FileStream _fs;

        private void LoadDataFromSD(string filename, SystemUpdate.SystemUpdateType ifutype)
        {
            int rest;
            byte[] hex;
            long len;
            int blocknum;

            _fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            len = _fs.Length;

            blocknum = (int)len / BLOCK_SIZE;
            rest = (int)len % BLOCK_SIZE;
            hex = new byte[BLOCK_SIZE];

            for (int i = 0; i < blocknum; i++)
            {
                _fs.Read(hex, 0, BLOCK_SIZE);

                SystemUpdate.Load(ifutype, hex, BLOCK_SIZE);

                Debug.Print("Loading file " + filename + ", block " + i + "/" + blocknum);

            }

            _fs.Read(hex, 0, rest);
            SystemUpdate.Load(ifutype, hex, rest);

            _fs.Close();
            _fs.Dispose();
            _fs = null;
            hex = null;

            Debug.GC(true);
        }

    }
}
