#include "token.h"

Token::Token(const std::string* _key) 
	: key(_key)
{}

Token::~Token()
{
	if (key != nullptr) { delete key; }
}

const std::string& Token::get()
{
	return *key;
}
