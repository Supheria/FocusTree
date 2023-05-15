#define _CRT_SECURE_NO_WARNINGS

#include <iostream>

#include "Utf8Text.h"
#include <fstream>

using namespace std;
std::string UnicodeToANSI(const std::wstring& wstr)
{
    std::string ret;
    std::mbstate_t state = {};
    const wchar_t* src = wstr.data();
    size_t len = std::wcsrtombs(nullptr, &src, 0, &state);
    if (static_cast<size_t>(-1) != len) {
        std::unique_ptr< char[] > buff(new char[len + 1]);
        len = std::wcsrtombs(buff.get(), &src, len, &state);
        if (static_cast<size_t>(-1) != len) {
            ret.assign(buff.get(), len);
        }
    }
    return ret;
}

int main()
{
    using namespace std;
    Utf8Text reader("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\germany.txt");
    try
    {
        ofstream ofile;
        ofile.open("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\resave.txt", ios::binary);
        //wstring wstr;
        reader.AddBOMHead(ofile.rdbuf());
        string buffer;
        while (reader.Read())
        {
            wstring wstr = reader.GetUnicodeChar();
            buffer += reader.UnicodeToUTF8(wstr);
            //cout << UnicodeToANSI(wstr);
        }

        ofile.close();
    }
    catch (string massage)
    {
        cerr << massage << endl;
    }
}