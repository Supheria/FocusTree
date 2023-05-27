
/// <summary>
/// 参考：https://blog.csdn.net/weixin_41055260/article/details/121434010
/// </summary>

#include <fstream>
#include "Utf8Reader.h"

const char Utf8Reader::BOM[3] = { 0xEF, 0xBB, 0xBF };

using namespace std;

Utf8Reader::~Utf8Reader()
{
}

Utf8Reader::Utf8Reader(std::string path) : 
    pos(CharPos()),
    nextpos(CharPos()),
    buffer(vector<string>()),
    hasbom(false)
{
    ifstream fin;
    fin.open(path.c_str(), ios::binary);
    if (!fin.is_open()) { return; }
    if (fin.get() != 0xEF || fin.get() != 0xBB || fin.get() != 0xBF) // fin.get() will return -1 if meet EOF
    {
        fin.seekg(0, fin.beg);
    }
    char t = fin.peek();
    char h = fin.get();
    
    for (int i = 0; i < 3; i++)
    {
        char a = fin.get();
        char b = fin.get();
        char c = fin.get();
    }
    while (!fin.eof())
    {
        string str;
        getline(fin, str);
        buffer.push_back(str);
    }
    fin.close();

    if (buffer.size() == 0 || buffer[0].length() < 3) { return; }
    char fileHead[3] = { 0, 0, 0 };
    for (int i = 0; i < 3; i++)
    {
        fileHead[i] = buffer[0][i];
    }
    hasbom = fileHead[0] == BOM[0] && fileHead[1] == BOM[1] && fileHead[2] == BOM[2];
    if (!hasbom) { return; }
    buffer[0] = buffer[0].substr(3, buffer[0].length() - 3);
}

bool Utf8Reader::read(std::string& u8char)
{
    pos = nextpos;
    if (pos.line == buffer.size()) { return false; }
    size_t length = get_u8char_length(buffer[pos.line][pos.column]);
    u8char = buffer[pos.line].substr(pos.column, length);

    nextpos.column = pos.column + length;
    if (nextpos.column >= buffer[pos.line].length())
    {
        nextpos.line++;
        nextpos.column = 0;
    }
    return true;
}

size_t Utf8Reader::get_u8char_length(const char& head)
{
    size_t len = 0;
    unsigned char mask = 0b10000000;
    while (head & mask)
    {
        len++;
        if (len > 6)
        {
            throw "[Utf8Reader.cpp]: 无效的UTF-8起始符。\n";
        }
        mask >>= 1;
    }
    if (0 == len) { return 1; } // ASCII's 8th bit is 0, and head of two-bytes utf-8's is 110xxxxx, so variable 'length' itself will be 0 or 2...6
    return len;
}

Utf8Reader::CharPos Utf8Reader::char_pos()
{
    return pos;
}