#ifndef _TOKEN_H
#define _TOKEN_H

#include <string>

class Token
{
	const std::string* const key;
public:
	Token(const std::string* _key);
	~Token();
	void parse(std::string filepath);
	virtual void append(Token*) = 0;
	const std::string& get();
};
#endif