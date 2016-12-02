using System;
using Microsoft.SPOT;

namespace _3D_control_v09
{
    interface SDIBoard
    {
        // nacte seznam parametru z gcode souboru 
        bool ReadParametersFromFile();
        // zadost SD o nacteni seznamu
        void GetNamesOfFilesFromSd();
        // zadost inicializuje SD kartu
        void InitSD();
        // zadost o odpojeni SD karty
        void ReleaseSDcard();
        // vybere soubor k tisku
        bool SelectSdFileForPrint(string nameFile);
        // startu tisku z SD
        void StartResumeSdPrint();
        // pauzovani tisku z SD
        void PauseSdPrint();
        // nastaveni pozice posledniho cteni z SD
        void SetSdPosition(int position);
        // zadost o vraceni jiz vytiskleho mnozstvi
        void GetSdPrintStatus();
        // vyvtvori soubor a zacne zapisovat do souboru
        void CreateFileAndStartWriteDataToFile(string name);
        // pridej data do vytvoreneho souboru
        void AddDataToFile(string gcode);
        // ukonci zapis do souboru
        void EndWriteDataToFile(string name);
        // smaz soubor z SD carty
        void DeleteFileFromSd(string name);
        // vrati kacitu SD a obsazene misto
        void GetStateSpaceSD();

        string[] ReadBlockForPrint();
    }
}
