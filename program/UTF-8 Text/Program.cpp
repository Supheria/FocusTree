#define _CRT_SECURE_NO_WARNINGS

#include <iostream>

#include "Utf8Text.h"
#include <fstream>

using namespace std;

int main()
{
    using namespace std;
    Utf8Text reader("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\germany.txt");
    try
    {
        ofstream ofile;
        ofile.open("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\resave.txt", ios::binary);
        filebuf* fbuff = ofile.rdbuf();
        fbuff->sputn(Utf8Text::BOM, sizeof(Utf8Text::BOM));
        string buffer;
        while (reader.read())
        {
            buffer += reader.getu8char();
            //int a = 0;
            //buffer += Utf8Text::uto8(L"wstr");
            //cout << UnicodeToANSI(wstr);
        }

        wstring wstr = reader.getunichar(buffer);
        ofile.close();
    }
    catch (string massage)
    {
        cerr << massage << endl;
    }
}