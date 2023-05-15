#include <iostream>
#include <fstream>
#include <codecvt>
#include "Utf8Text.h"

const char Utf8Text::BOM_Head[3] = { 0xEF, 0xBB, 0xBF };

using namespace std;

Utf8Text::~Utf8Text()
{
    delete[] Buffer;
}
/// <summary>
/// �ο���https://blog.csdn.net/weixin_41055260/article/details/121434010
/// </summary>
Utf8Text::Utf8Text(std::string path) : CharIndex(0), CurrentUtf8Char(string())
{
    ifstream infile;
    infile.open(path.c_str(), ios::binary);
    if (!infile.is_open())
    {
        throw "[Utf8Text.cpp]: [2305131210]�޷���ȡ�ļ���\n";
    }
    char fileHead[3] = { 0, 0, 0 };
    for (int i = 0; i < 3 && !infile.eof(); i++)
    {
        fileHead[i] = infile.get();
    }
    HasBOM = fileHead[0] == BOM_Head[0] && fileHead[1] == BOM_Head[1] && fileHead[2] == BOM_Head[2];
    filebuf* filebuffer = infile.rdbuf();
    size_t starter = HasBOM ? 3 : 0;
    BufferLength = (int)filebuffer->pubseekoff(0, ios::end, ios::in) - starter;
    Buffer = new char[BufferLength];
    filebuffer->pubseekpos(starter, ios::in);
    filebuffer->sgetn(Buffer, BufferLength);
    infile.close();
}
/// <summary>
/// �ο���https://blog.csdn.net/weixin_41055260/article/details/121434010
/// </summary>
bool Utf8Text::Read()
{
    if (CharIndex == BufferLength) { return false; }
    size_t charLength = GetUtf8CharLength(Buffer[CharIndex]);
    size_t nextIndex = CharIndex + charLength;
    if (nextIndex > BufferLength)
    {
        throw "[Utf8Text.cpp]: [2305131213]�ַ���ʼλ��Խ�硣\n";
    }
    CurrentUtf8Char.assign(Buffer + CharIndex, charLength);
    CharIndex = nextIndex;
    return true;
}
/// <summary>
/// �ο���https://blog.csdn.net/weixin_41055260/article/details/121434010
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
            throw "[Utf8Text.cpp]: [2305131155]��Ч��UTF-8�ַ���ʶ��\n";
        }
        mask >>= 1;
    }
    if (0 == length) { return 1; } // ASCII's 8th bit is 0, and startMark of two-bytes utf-8's is 110xxxxx, so variable 'length' itself will be 0 or 2...6
    return length;
}
const std::string& Utf8Text::GetUtf8Char()
{
    return CurrentUtf8Char;
}
/// <summary>
/// �ο���https://blog.csdn.net/FlushHip/article/details/82836867
/// </summary>
/// <param name="str"></param>
/// <returns></returns>
std::wstring Utf8Text::GetUnicodeChar()
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
/// �ο���https://blog.csdn.net/FlushHip/article/details/82836867
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

void Utf8Text::AddBOMHead(std::filebuf* fileBuffer)
{
    if (fileBuffer == nullptr) { throw "[Utf8Text.cpp]: [2305141004]��Ч���ļ���������\n"; }
    fileBuffer->sputn(BOM_Head, sizeof(BOM_Head));
}

int Utf8Text::GetBufferLength()
{
    return BufferLength;
}