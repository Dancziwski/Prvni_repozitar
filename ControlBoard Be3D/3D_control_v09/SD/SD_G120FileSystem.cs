using System;
using Microsoft.SPOT;
using System.Collections;
using System.IO;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.IO;
using System.Threading;
using System.Text;
using Be3D.Constants;
using GHI.Hardware.G120;
using GHI.Premium.IO;

namespace _3D_control_v09
{
    class SD_G120FileSystem
    {
          #region Members

        private static InterruptPort detectPin = new InterruptPort(Pin.P0_5, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeBoth);
       
        //private static OutputPort resetSdPin = new OutputPort(Pin.P1_14,false);
        
        private string _rootDirectory = "";
        private PersistentStorage _sd = null;
        private VolumeInfo _vol = null;
        
        #endregion Members

        private const ThreadPriority PriorityThread = ThreadPriority.Normal;
        
        public SD_G120FileSystem()
        {

        }

        private void DeleteObject()
        {
            _sd = null;
            _vol = null;
            _rootDirectory = "";           
        }

        public bool InitSD()
        {
            if (!detectPin.Read())
                return Mount();
            else
                return UnMount();
        }

        public bool Mount()
        {
            try
            {
                // Check to avoid exception
                if (_sd != null) UnMount();

                _sd = new PersistentStorage("SD");
                _sd.MountFileSystem();

                if (_sd == null)
                    return false;

                // Find volume
                _vol = new VolumeInfo("SD");
                if (_vol == null)
                    return false;

                // Format if not already formatted
                if (!_vol.IsFormatted) _vol.Format("FAT", 0, true);

                _rootDirectory = _vol.RootDirectory;
              
                return true;
            }
            catch (Exception)
            {
                DeleteObject();

                return false;
            }

        }

        public bool UnMount()
        {
            try
            {
                // Check to avoid exception
                if (_sd == null) 
                    return false;
                
                // Unmount
                _sd.UnmountFileSystem();
                _sd.Dispose();

                DeleteObject();

                return false;

            }
            catch (Exception)
            {
                Debug.Print("SDFileSystem not unmount");
                return false;
            }
        }

        public ArrayList LoadFilesList()
        {
            if (_vol == null || _sd == null || _rootDirectory == "")
            {
                return null;
            }

            return SelectFileFromPrefix(Directory.GetFiles(_rootDirectory));
        }

        public string GetRootDirectory()
        {
            return _rootDirectory;
        }

        public string GetCapacitySd()
        {
            if (_sd == null || _vol == null)
                return "";

            var capacite = (int)(_vol.TotalSize / 1000000);
            return capacite.ToString() + " MB";
        }

        public string GetFreeSapceSd()
        {
            if (_sd == null || _vol == null)
                return "";

            var freeCapacite = (int)(_vol.TotalFreeSpace / 1000000);
            return freeCapacite.ToString() + " MB";
        }

        public bool AddDataToFile(string fileName, byte[] data)
        {
            FileStream fs = new FileStream(_rootDirectory + @"\" + fileName, FileMode.Append);

            if (data != null)
            {
                fs.Write(data, 0, data.Length);
            }

            fs.Close();

            return true;
        }

        public bool DeleteFile(string fileName)
        {
            if (File.Exists(_rootDirectory + fileName))
                File.Delete(_rootDirectory + fileName);

            return true;
        }

        private ArrayList SelectFileFromPrefix(string[] files)
        {
            if (files == null)
                return null;

            string[] file, file1;
            var listFiles = new ArrayList();

            for (int i = 0; i < files.Length; i++)
            {
                file = files[i].Split('.');
                if (file.Length == 2)
                {
                    //if (file[1] == "dee" || file[1] == "DEE")
                    if (file[1] == Constants.Prefix1 || file[1] == Constants.Prefix1.ToLower())
                    {
                        file1 = file[0].Split('\\');
                        listFiles.Add(file1[2]);
                    }
                }
            }
            return listFiles;
        }

        private static FileStream _flStream;
        const char EndLine = '\n';
        
        //const int NumberOfCharInLine = 70;  //max byva 38znaku, zbytek je rezerva
        const int ReserveOfCharInLine = 50;
        const int NumberOfReadByte = 300;

        //private static byte[] outArray = new byte[NumberOfCharInLine];
        private static byte[] bArr = new byte[NumberOfReadByte];


        public bool OpenFileForRead(string nameFile)
        {
            if (_flStream != null)
                _flStream.Close();

            string pathFile = _rootDirectory + @"\" + nameFile;

            if (File.Exists(pathFile))
            {
                _flStream = File.OpenRead(pathFile);
                return true;
            }

            return false;
        }

        public void CloseFileForRead()
        {
            if (_flStream != null)
                _flStream.Close();
        }

        public string[] ReadBlockDateFromSDSeparateLine()
        {
            string[] lines;


            int state = _flStream.Read(bArr, 0, bArr.Length - ReserveOfCharInLine);

            if (state == 0) //return null end of file
                return null;

            if (bArr[bArr.Length - ReserveOfCharInLine - 1] != EndLine)
            {
                for (int i = 0; i < ReserveOfCharInLine; i++)
                {
                    byte[] b = new byte[1];
                    state = _flStream.Read(b, 0, 1);
                    bArr[bArr.Length - ReserveOfCharInLine + i] = b[0];
                    if ((char)b[0] == EndLine || state == 0)
                        break;
                }
            }

            string str = new string(Encoding.UTF8.GetChars(bArr));          
            lines = str.Split(EndLine);

            for(int i = 0; i < bArr.Length;i++)
                bArr[i] = 0;

            return lines;
        }

    }
}

