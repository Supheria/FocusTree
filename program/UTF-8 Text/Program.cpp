#define _CRT_SECURE_NO_WARNINGS

#include <iostream>

#include "Utf8Reader.h"
#include <fstream>

using namespace std;

int main()
{
    using namespace std;
    try
    {
        
        Utf8Reader reader("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\新建 文本文档.txt");
        ofstream ofile("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\resave.txt", ios::binary);
        filebuf* fbuff = ofile.rdbuf();
        fbuff->sputn(Utf8Reader::BOM, sizeof(Utf8Reader::BOM));
        string buffer;
        while (reader.read())
        {
            buffer += reader.getu8char();
            //int a = 0;
            //buffer += Utf8Reader::uto8(L"wstr");
            //cout << UnicodeToANSI(wstr);
        }

        //wstring wstr = reader.getunichar(buffer);
        ofile.close();
    }
    catch (string massage)
    {
        cerr << massage << endl;
    }
}