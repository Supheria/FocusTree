#ifndef UTF8TEXT_H
#define UTF8TEXT_H

class Utf8Text
{
private:
    /// <summary>
    /// �ֽڻ�����
    /// </summary>
    char * Buffer;
    /// <summary>
    /// ����������
    /// </summary>
    size_t BufferLength;
    /// <summary>
    /// �Ƿ��� BOM �ļ�ͷ
    /// </summary>
    bool HasBOM;
public:
    static const char BOM_Head[3];
public:
    int GetBufferLength();
private:
    /// <summary>
    /// ��ǰ�� UTF-8 �ַ�
    /// </summary>
    std::string CurrentUtf8Char;
    /// <summary>
    /// ��ǰ�ַ���λ������
    /// </summary>
    size_t CharIndex;
public:
    /// <summary>
    /// ʹ���ļ�·���½��ֽڻ�����
    /// </summary>
    /// <param name="path">�ļ�·��</param>
    Utf8Text(std::string path);
    ~Utf8Text();
    /// <summary>
    /// ��ȡ���ת��Ϊ Unicode ���ַ��� UTF-8 �ַ�
    /// </summary>
    /// <param name="UnicodeChar"></param>
    /// <returns></returns>
    bool Read();
    /// <summary>
    /// ��õ�ǰ�� UTF-8 �ַ�������
    /// </summary>
    /// <returns>��ǰ�� UTF-8 �ַ������ã���Ҫ�ô˷���ֵ��ֵ��</returns>
    const std::string& GetUtf8Char();
    /// <summary>
    /// ��ȡ��ǰ UTF-8 �ַ��� Unicode �����ַ�
    /// </summary>
    /// <returns></returns>
    std::wstring GetUnicodeChar();
    /// <summary>
    /// �� Unicode ����ת��Ϊ UTF-8 ����
    /// </summary>
    /// <param name="unicode"></param>
    /// <returns></returns>
    static std::string UnicodeToUTF8(const std::wstring& unicode);
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
    size_t GetUtf8CharLength(const char& start);
public:
    void AddBOMHead(std::filebuf* fileBuffer);
};

#endif