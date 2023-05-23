#ifndef  _TOKENIZE_H
#define _TOKENIZE_H

#include <string>
#include <vector>
#include "Utf8Reader.h"

struct CompareChar;

class Tokenize
{
public:
	static const char BOM[3];
	static const CompareChar delimiter, blank, endline, keychar, note, quote;
public: 
	struct Token
	{
		std::string token;
		size_t line;
		size_t column;
	public:
		Token() : token(std::string()), line(0), column(0) {};
	};
private:
	std::vector<Token> chain;
	size_t line;
	size_t column;
public:
	Tokenize(std::string filepath);
	/// <summary>
	/// ��ȡ��һ�� token����������������ֵ
	/// </summary>
	/// <param name="_t"></param>
	/// <returns>token ������ֵ������ѵ���β�򷵻�-1</returns>
	size_t get(Token& _t);
	/// <summary>
	/// ��ȡ��������ֵ֮���� token
	/// </summary>
	/// <param name="index"></param>
	/// <param name="_t"></param>
	/// <returns>���ڲ��ɹ���ȡ���� true�����򷵻� false</returns>
	bool get(int index, Token& _t);
private:
	void build_line(const std::string& s);
};


struct CompareChar
{
	std::vector<char> chs;
public:
	CompareChar(std::vector<char> chars)
	{
		chs = chars;
	}
	bool operator==(const char ch) const
	{
		for (int i = 0; i < chs.size(); i++)
		{
			if (chs[i] != ch)
			{
				return false;
			}
		}
		return true;
	}
	bool operator!=(const char ch) const
	{
		return !(*this == ch);
	}
	friend bool operator==(const char ch, const CompareChar& cc)
	{
		return cc == ch;
	}
	friend bool operator!=(const char ch, const CompareChar& cc)
	{
		return cc != ch;
	}
};

#endif // ! _TOKENIZE_H