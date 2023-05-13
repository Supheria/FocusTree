#ifndef UTF8TEXT_H
#define UTF8TEXT_H
#include <iostream>
#include <fstream>
#include <string>

class Utf8Text
{
private:
    /// <summary>
    /// 字节缓存区
    /// </summary>
    char * Buffer;
    /// <summary>
    /// 缓存区长度
    /// </summary>
    size_t BufferLength;
    /// <summary>
    /// 是否有 BOM 文件头
    /// </summary>
    bool isBOM;
private:
    /// <summary>
    /// 当前缓存区位置索引
    /// </summary>
    size_t CharIndex;
    std::string CurrentUtf8Char;
public:
    /// <summary>
    /// 使用文件路径新建字节缓冲区
    /// </summary>
    /// <param name="path">文件路径</param>
    Utf8Text(std::string path);
    ~Utf8Text();
    /// <summary>
    /// 读取宇哥转换为 Unicode 长字符的 UTF-8 字符
    /// </summary>
    /// <param name="UnicodeChar"></param>
    /// <returns></returns>
    bool Read();
    std::string GetUtf8Char();
    std::wstring GetWideUnicode();
    std::string GetShortUnicode();
    static std::string UnicodeToUTF8(const std::wstring& unicode);
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
    /// <param name="byte"></param>
    /// <returns>返回1-6</returns>
    /// <exception cref="无效的UTF-8字符标识"></exception>
    size_t GetUtf8CharLength(const char& start);
};

#endif