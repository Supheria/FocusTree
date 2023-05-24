#ifndef UTF8READER_H
#define UTF8READER_H

#include <vector>
#include <string>

class Utf8Reader
{
public:
    static const char BOM[3];
private:
    std::vector<std::string> buffer;
    bool hasbom;
private:
    struct CharPos
    {
        size_t line;
        size_t column;
    public:
        CharPos() { line = 0; column = 0; }
    } pos, nextpos;
public:
    Utf8Reader(std::string filepath);
    ~Utf8Reader();
    /// <summary>
    /// 读取一个 UTF-8 字符
    /// </summary>
    /// <returns>读取到缓存流末尾返回false，否则返回true</returns>
    bool read(std::string& u8char);
    /// <summary>
    /// 当前读取字符的位置
    /// </summary>
    /// <returns></returns>
    CharPos char_pos();
private:
    /// <summary>
    /// 获取UTF-8字符长度
    /// <para>U-00000000 - U-0000007F: 0xxxxxxx</para>
    /// <para>U-00000080 - U-000007FF: 110xxxxx 10xxxxxx</para>
    /// <para>U-00000800 - U-0000FFFF: 1110xxxx 10xxxxxx 10xxxxxx</para>
    /// <para>U-00010000 - U-001FFFFF: 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx</para>
    /// <para>U-00200000 - U-03FFFFFF: 111110xx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx</para>
    /// <para>U-04000000 - U-7FFFFFFF: 1111110x 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx</para>
    /// </summary>
    /// <param name="head">起始位字符</param>
    /// <returns>返回1-6</returns>
    /// <exception cref="无效的UTF-8起始符"></exception>
    size_t get_u8char_length(const char& head);
};

#endif