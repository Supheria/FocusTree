#include <iostream>

#include "Utf8Text.h"


using namespace std;


int main()
{
    using namespace std;
    Utf8Text reader("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\test.txt");
    try
    {
        ofstream ofile;
        ofile.open("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\resave.txt");
        wstring wstr;
        while (reader.Read())
        {
            cout << reader.GetShortUnicode();
            wstr = reader.GetWideUnicode();
            ofile << reader.UnicodeToUTF8(wstr);
        }
        ofile.close();
    }
    catch (string massage)
    {
        cerr << massage << endl;
    }
}