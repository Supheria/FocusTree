#ifndef UTF8READER_H
#define UTF8READER_H

class Utf8Reader
{
public:
    static const char BOM[3];
    /// <summary>
    /// �� Unicode ����ת��Ϊ UTF-8 ����
    /// </summary>
    /// <param name="unicode"></param>
    /// <returns></returns>
    static std::string uto8(const std::wstring& unicode);
    static std::wstring _8tou(std::string utf8);
private:
    /// <summary>
    /// �ֽڻ�����
    /// </summary>
    char * buffer;
    /// <summary>
    /// ����������
    /// </summary>
    size_t length;
    /// <summary>
    /// �Ƿ��� BOM �ļ�ͷ
    /// </summary>
    bool hasbom;
public:
    int bufferlength();
private:
    /// <summary>
    /// ��ǰ�� UTF-8 �ַ�
    /// </summary>
    std::string u8char;
    /// <summary>
    /// ��ǰ�ַ���λ������
    /// </summary>
    size_t index;
public:
    /// <summary>
    /// ʹ���ļ�·���½��ֽڻ�����
    /// </summary>
    /// <param name="path">�ļ�·��</param>
    Utf8Reader(std::string filepath);
    ~Utf8Reader();
    /// <summary>
    /// ��ȡ���ת��Ϊ Unicode ���ַ��� UTF-8 �ַ�
    /// </summary>
    /// <param name="UnicodeChar"></param>
    /// <returns></returns>
    bool read();
    /// <summary>
    /// ��õ�ǰ�� UTF-8 �ַ�������
    /// </summary>
    /// <returns>��ǰ�� UTF-8 �ַ������ã���Ҫ�ô˷���ֵ��ֵ��</returns>
    const std::string& getu8char();
    /// <summary>
    /// ��ȡ��ǰ UTF-8 �ַ��� Unicode �����ַ�
    /// </summary>
    /// <returns></returns>
    //std::wstring getunichar(std::string utf8);
private:
    /// <summary>
    /// ��ȡUTF-8�ַ�����
    /// <para>U-00000000 - U-0000007F: 0xxxxxxx</para>
    /// <para>U-00000080 - U-000007FF: 110xxxxx 10xxxxxx</para>
    /// <para>U-00000800 - U-0000FFFF: 1110xxxx 10xxxxxx 10xxxxxx</para>
    /// <para>U-00010000 - U-001FFFFF: 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx</para>
    /// <para>U-00200000 - U-03FFFFFF: 111110xx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx</para>
    /// <para>U-04000000 - U-7FFFFFFF: 1111110x 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx</para>
    /// </summary>
    /// <param name="byte"></param>
    /// <returns>����1-6</returns>
    /// <exception cref="��Ч��UTF-8�ַ���ʶ"></exception>
    size_t get_u8char_length(const char& start);
};

#endif