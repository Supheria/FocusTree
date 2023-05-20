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
	/// 获取下一个 token，并返回它的索引值
	/// </summary>
	/// <param name="_t"></param>
	/// <returns>token 的索引值，如果已到链尾则返回-1</returns>
	size_t get(Token& _t);
	/// <summary>
	/// 获取给定索引值之处的 token
	/// </summary>
	/// <param name="index"></param>
	/// <param name="_t"></param>
	/// <returns>存在并成功获取返回 true，否则返回 false</returns>
	bool get(int index, Token& _t);
private:
	void build(Utf8Reader& r);
};

#endif // ! _TOKENIZE_H


