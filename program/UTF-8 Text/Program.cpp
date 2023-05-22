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
        
        Utf8Reader reader("Baltic.txt");
        ofstream ofile("resave.txt", ios::binary);
        filebuf* fbuff = ofile.rdbuf();
        fbuff->sputn(Utf8Reader::BOM, sizeof(Utf8Reader::BOM));
        string buffer;
        while (reader.read())
        {
            buffer = reader.getu8char();
            size_t a = buffer.length();
            if (buffer == "\r")
            {
                int a = 0;
            }
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