#ifndef UTF8READER_H
#define UTF8READER_H

class Utf8Reader
{
public:
    static const char BOM[3];
    /// <summary>
    /// 将 Unicode 编码转换为 UTF-8 编码
    /// </summary>
    /// <param name="unicode"></param>
    /// <returns></returns>
    static std::string uto8(const std::wstring& unicode);
    static std::wstring _8tou(std::string utf8);
private:
    /// <summary>
    /// 字节缓存区
    /// </summary>
    char * buffer;
    /// <summary>
    /// 缓存区长度
    /// </summary>
    size_t length;
    /// <summary>
    /// 是否有 BOM 文件头
    /// </summary>
    bool hasbom;
public:
    int bufferlength();
private:
    /// <summary>
    /// 当前的 UTF-8 字符
    /// </summary>
    std::string u8char;
    /// <summary>
    /// 当前字符的位置索引
    /// </summary>
    size_t index;
public:
    /// <summary>
    /// 使用文件路径新建字节缓冲区
    /// </summary>
    /// <param name="path">文件路径</param>
    Utf8Reader(std::string filepath);
    ~Utf8Reader();
    /// <summary>
    /// 读取宇哥转换为 Unicode 长字符的 UTF-8 字符
    /// </summary>
    /// <param name="UnicodeChar"></param>
    /// <returns></returns>
    bool read();
    /// <summary>
    /// 获得当前的 UTF-8 字符的引用
    /// </summary>
    /// <returns>当前的 UTF-8 字符的引用（不要用此返回值赋值）</returns>
    const std::string& getu8char();
    /// <summary>
    /// 获取当前 UTF-8 字符的 Unicode 编码字符
    /// </summary>
    /// <returns></returns>
    //std::wstring getunichar(std::string utf8);
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
    size_t get_u8char_length(const char& start);
};

#endif