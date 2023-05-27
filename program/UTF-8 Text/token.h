#ifndef _TOKEN_H
#define _TOKEN_H

#include <string>

class Token
{
	const std::string* token;
public:
	Token(const std::string* _t) : token(_t) {}
	~Token()
	{
		if (token != nullptr) { delete token; }
	}
	const std::string& get() const { return *token; }
	virtual void parse(const std::string&) = 0;
	virtual void combine(const Token*) = 0;
	virtual void append(const Token*) = 0;
};

class Value : public Token
{
public:
	Value(const std::string* _val) : Token(_val) {}
	
	// do nothing, and will throw an erro log
	void combine(const Token*) {}
	// do nothing, and will throw an erro log
	void append(const Token*) {}
};
#endif