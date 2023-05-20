#include <iostream>
#include <fstream>
#include <codecvt>
#include "Utf8Reader.h"

const char Utf8Reader::BOM[3] = { 0xEF, 0xBB, 0xBF };

using namespace std;

Utf8Reader::~Utf8Reader()
{
    delete[] buffer;
}
/// <summary>
/// https://blog.csdn.net/weixin_41055260/article/details/121434010
/// </summary>
Utf8Reader::Utf8Reader(std::string path) : 
    index(0), 
    u8char(string()),
    buffer(nullptr), 
    length(0), 
    hasbom(false)
{
    ifstream infile;
    infile.open(path.c_str(), ios::binary);
    if (!infile.is_open())
    {
        return;
    }
    char fileHead[3] = { 0, 0, 0 };
    for (int i = 0; i < 3 && !infile.eof(); i++)
    {
        fileHead[i] = infile.get();
    }
    hasbom = fileHead[0] == BOM[0] && fileHead[1] == BOM[1] && fileHead[2] == BOM[2];
    filebuf* filebuffer = infile.rdbuf();
    size_t starter = hasbom ? 3 : 0;
    length = (int)filebuffer->pubseekoff(0, ios::end, ios::in) - starter;
    buffer = new char[length];
    filebuffer->pubseekpos(starter, ios::in);
    filebuffer->sgetn(buffer, length);
    infile.close();
}
/// <summary>
/// 参考：https://blog.csdn.net/weixin_41055260/article/details/121434010
/// </summary>
bool Utf8Reader::read()
{
    if (index == length) { return false; }
    size_t char_length = get_u8char_length(buffer[index]);
    size_t next_index = index + char_length;
    if (next_index > length)
    {
        throw "[Utf8Reader.cpp]: [2305131213]字符起始位置越界。\n";
    }
    u8char.assign(buffer + index, char_length);
    index = next_index;
    return true;
}
/// <summary>
/// 参考：https://blog.csdn.net/weixin_41055260/article/details/121434010
/// </summary>
size_t Utf8Reader::get_u8char_length(const char& starter)
{
    size_t len = 0;
    unsigned char mask = 0b10000000;
    while (starter & mask)
    {
        len++;
        if (len > 6)
        {
            throw "[Utf8Reader.cpp]: [2305131155]无效的UTF-8字符标识。\n";
        }
        mask >>= 1;
    }
    if (0 == len) { return 1; } // ASCII's 8th bit is 0, and startMark of two-bytes utf-8's is 110xxxxx, so variable 'length' itself will be 0 or 2...6
    return len;
}
const std::string& Utf8Reader::getu8char()
{
    return u8char;
}
/// <summary>
/// 参看：https://blog.csdn.net/FlushHip/article/details/82836867
/// </summary>
/// <param name="str"></param>
/// <returns></returns>
std::wstring Utf8Reader::_8tou(std::string utf8)
{
    setlocale(LC_CTYPE, "");
    wstring wstr;
    try {
        wstring_convert<codecvt_utf8<wchar_t>> wcv;
        wstr = wcv.from_bytes(utf8);
    }
    catch (const exception& e) {
        cerr << e.what() << endl;
    }
    return wstr;
}
/// <summary>
/// 参看：https://blog.csdn.net/FlushHip/article/details/82836867
/// </summary>
/// <param name="wstr"></param>
/// <returns></returns>
std::string Utf8Reader::uto8(const std::wstring& unicode)
{
    setlocale(LC_CTYPE, "");
    string ret;
    try {
        wstring_convert<codecvt_utf8<wchar_t>> wcv;
        ret = wcv.to_bytes(unicode);
    }
    catch (const std::exception& e) {
        cerr << e.what() << endl;
    }
    return ret;
}

int Utf8Reader::bufferlength()
{
    return length;
}