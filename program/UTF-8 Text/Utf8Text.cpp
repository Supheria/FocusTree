#include <windows.h>
#include <comutil.h>
#include <codecvt>
#include "Utf8Text.h"

using namespace std;

Utf8Text::~Utf8Text()
{
}
/// <summary>
/// 参考：https://blog.csdn.net/weixin_41055260/article/details/121434010
/// </summary>
Utf8Text::Utf8Text(std::string path) : CharIndex(0), CurrentUtf8Char(string())
{
    ifstream inFile;
    inFile.open(path.c_str(), ios::binary);
    if (!inFile.is_open())
    {
        throw "[Utf8Text.cpp]: [2305131210]无法读取文件。\n";
    }
    int fileHead[3] = { 0, 0, 0 };
    for (int i = 0; i < 3 && !inFile.eof(); i++)
    {
        fileHead[i] = inFile.get();
    }
    isBOM = fileHead[0] == 0xEF && fileHead[1] == 0xBB && fileHead[2] == 0xBF;
    filebuf* filebuffer;
    filebuffer = inFile.rdbuf();
    int starter = isBOM ? 3 : 0;
    BufferLength = (int)filebuffer->pubseekoff(starter, ios::end, ios::in);
    filebuffer->pubseekpos(starter, ios::in);
    Buffer = new char[BufferLength + 1];
    filebuffer->sgetn(Buffer, BufferLength);
    inFile.close();
}
/// <summary>
/// 参考：https://blog.csdn.net/weixin_41055260/article/details/121434010
/// </summary>
bool Utf8Text::Read()
{
    if (CharIndex == BufferLength) { return false; }
    size_t charLength = GetUtf8CharLength(Buffer[CharIndex]);
    size_t nextIndex = CharIndex + charLength;
    if (nextIndex > BufferLength)
    {
        throw "[Utf8Text.cpp]: [2305131213]字符起始位置越界。\n";
    }
    char* str = new char[charLength + 1];
    for (int i = 0; i < charLength; i++)
    {
        str[i] = *(Buffer + CharIndex + i);
    }
    str[charLength] = '\0';
    CurrentUtf8Char = str;
    CharIndex = nextIndex;
    return true;
}
/// <summary>
/// 参考：https://blog.csdn.net/weixin_41055260/article/details/121434010
/// </summary>
size_t Utf8Text::GetUtf8CharLength(const char& startMark)
{
    size_t length = 0;
    unsigned char mask = 0b10000000;
    while (startMark & mask)
    {
        length++;
        if (length > 6)
        {
            throw "[Utf8Text.cpp]: [2305131155]无效的UTF-8字符标识。\n";
        }
        mask >>= 1;
    }
    if (0 == length) { return 1; } // ASCII's 8th byte is 0, and length of two utf-8's startMark is 110xxxxx, so variable 'length' itself will be 0 or 2...6
    return length;
}
std::string Utf8Text::GetUtf8Char()
{
    return CurrentUtf8Char;
}
/// <summary>
/// 参看：https://blog.csdn.net/FlushHip/article/details/82836867
/// </summary>
/// <param name="str"></param>
/// <returns></returns>
std::wstring Utf8Text::GetWideUnicode()
{
    setlocale(LC_CTYPE, "");
    wstring wstr;
    try {
        wstring_convert<codecvt_utf8<wchar_t>> wcv;
        wstr = wcv.from_bytes(CurrentUtf8Char);
    }
    catch (const exception& e) {
        cerr << e.what() << endl;
    }
    return wstr;
}
/// <summary>
/// 参考：https://blog.csdn.net/libaineu2004/article/details/119393505
/// </summary>
/// <param name="UnicodeChar"></param>
/// <returns></returns>
std::string Utf8Text::GetShortUnicode()
{
    wstring unicode = GetWideUnicode();
    int length = WideCharToMultiByte(LC_CTYPE, 0, unicode.c_str(), unicode.size(), NULL, 0, NULL, NULL);
    char* buffer = new char[length + 1];
    //宽字节编码转换成多字节编码
    WideCharToMultiByte(LC_CTYPE, 0, unicode.c_str(), unicode.size(), buffer, length, NULL, NULL);
    buffer[length] = '\0';
    //删除缓冲区并返回值
    return buffer;
}
/// <summary>
/// 参看：https://blog.csdn.net/FlushHip/article/details/82836867
/// </summary>
/// <param name="wstr"></param>
/// <returns></returns>
std::string Utf8Text::UnicodeToUTF8(const std::wstring& unicode)
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