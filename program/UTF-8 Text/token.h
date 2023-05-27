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
	const std::string& get() { return *token; }
};
#endif