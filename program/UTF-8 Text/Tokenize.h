#ifndef  _TOKENIZE_H
#define _TOKENIZE_H

#include <string>
#include <vector>

class Tokenize
{
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
	size_t index;
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
	void build(Utf8Reader& r);
};

#endif // ! _TOKENIZE_H


