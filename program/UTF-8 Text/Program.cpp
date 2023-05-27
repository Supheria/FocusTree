#define _CRT_SECURE_NO_WARNINGS

#include <iostream>

#include <codecvt>
#include "Utf8Reader.h"
#include "Tokenize.h"
#include <fstream>

using namespace std;

char test(const char * ch) 
{
    string a;
    a.assign(ch);
    return a[0];
}

int main()
{
    using namespace std;

    const char* c = new char('a');
    c = nullptr;

    char c[3] = { '\0' };
    string str = "abcd";
    char a = test(str.c_str());
    int b = 0;
    
    try
    {
        
        Utf8Reader reader("test.txt");
        ofstream ofile("resave.txt", ios::binary);
        filebuf* fbuff = ofile.rdbuf();
        string str, buffer;
        while (reader.read(str))
        {
            buffer += str;
            size_t a = buffer.length();
            if (buffer == "\r")
            {
                int a = 0;
            }
            auto b = reader.char_pos();
            cout << b.line << ", " << b.column << '\n';
            //int a = 0;
            //buffer += Utf8Reader::uto8(L"wstr");
            //cout << UnicodeToANSI(wstr);
        }
        auto pos = reader.char_pos();
        cout << buffer;
        ofile << buffer;
        ofile.close();
    }
    catch (string massage)
    {
        cerr << massage << endl;
    }
}