

using GHI.Premium.Hardware.LowLevel;
namespace _3D_control_v09
{
    class MemoryConfigManager
    {
        const uint EEPROM_PAGE_SIZE = 64;

        Register regEECMD = new Register(0x00200080);
        Register regEEADDR = new Register(0x00200084);
        Register regEEWDATA = new Register(0x00200088);
        Register regEERDATA = new Register(0x0020008C);
        Register regEEWSTATE = new Register(0x00200090);
        Register regEECLKDIV = new Register(0x00200094);
        Register regEEPWRDWN = new Register(0x00200098);
        Register regEEINTEN = new Register(0x00200FE4);
        Register regEEINTCLR = new Register(0x00200FD8);
        Register regEEINTSET = new Register(0x00200FDC);
        Register regEEINTSTAT = new Register(0x00200FE0);
        Register regEEINTSTATCLR = new Register(0x00200FE8);
        Register regEEINTSTATSET = new Register(0x00200FEC);

        /// <summary>
        /// Default constructor 
        /// </summary>
        public MemoryConfigManager()
        {
            Init();
        }

        /// <summary>
        /// Initialize the G120 EEPROM.
        /// </summary>
        private void Init()
        {
            uint val = 0;
            uint cclk = 120000000;  // For G120

            // No power down
            regEEPWRDWN.Write(0);
            // Set correct divider
            regEECLKDIV.Write((cclk / 375000) - 1);

            // Set wait states
            val = ((((cclk / 1000000) * 15) / 1000) + 1);
            val |= (((((cclk / 1000000) * 55) / 1000) + 1) << 8);
            val |= (((((cclk / 1000000) * 35) / 1000) + 1) << 16);
            regEEWSTATE.Write(val);
        }

        /// <summary>
        /// Writes data to the internal EEPROM
        /// </summary>
        /// <param name="page">0-62</param>
        /// <param name="address">0-63</param>
        /// <param name="data">Data to write</param>
        /// <param name="size">Size of data</param>
        public void Write(uint page, uint address, byte[] data, int size)
        {
            uint i;

            regEEADDR.Write(address & 0x3F);
            for (i = 0; i < size; i++)
            {
                regEECMD.Write(0x03);
                regEEWDATA.Write(data[i]);
                while ((regEEINTSTAT.Read() & (0x01 << 26)) == 0) ;
                address++;

                if (address >= EEPROM_PAGE_SIZE)
                {
                    regEEINTSTATCLR.Write(0x01 << 28);
                    regEEADDR.Write((page & 0x3F) << 6);
                    regEECMD.Write(0x06);
                    while ((regEEINTSTAT.Read() & (0x01 << 28)) == 0) ;

                    address = 0;
                    page++;
                    regEEADDR.Write(0x00);
                }
                else if (i == size - 1)
                {
                    regEEINTSTATCLR.Write(0x01 << 28);
                    regEEADDR.Write((page & 0x3F) << 6);
                    regEECMD.Write(0x06);
                    while ((regEEINTSTAT.Read() & (0x01 << 28)) == 0) ;
                }
            }
        }

        /// <summary>
        /// Reads data from the internal EEPROM
        /// </summary>
        /// <param name="page">0-62</param>
        /// <param name="address">0-63</param>
        /// <param name="size">Size of data</param>
        /// <returns>Data read from EEPROM</returns>
        public byte[] Read(uint page, uint address, uint size)
        {
            byte[] retValue = new byte[size];

            uint i;
            regEEADDR.Write((page & 0x3F) << 6 | address & 0x3F);
            regEECMD.Write(0x01 << 3);
            for (i = 0; i < size; i++)
            {
                while ((regEEINTSTAT.Read() & (0x01 << 26)) == 0) ;
                retValue[i] = (byte)regEERDATA.Read();
                address++;

                if (address >= EEPROM_PAGE_SIZE)
                {
                    address = 0;
                    page++;
                    regEEADDR.Write((page & 0x3F) << 6 | address & 0x3F);
                    regEECMD.Write(0x01 << 3);
                }
            }

            return retValue;
        }
    }
}
