#include "token.h"

Token::Token(T _t, std::string* _s, Token* _fr) :
	tp(_t),
	tok(_s == nullptr ? new std::string("") : _s),
	fr(_fr),
	lv(0)
{
}

Token::~Token()
{
	if (tok != nullptr) { delete tok; }
	if (fr != nullptr) { delete fr; }
}

const Token::T& Token::type()
{
	return tp;
}

const std::string& Token::token()
{
	return *tok;
}

const Token* const Token::from() const
{
	return fr;
}

const size_t& Token::level()
{
	if (lv != 0) { return lv; }
	const Token* cur = this;
	for (lv = 1; cur->from() != nullptr; lv++)
	{
		cur = cur->from();
	}
	return lv;
}

const Token* const Token::parse(std::string& filename)
{
	return nullptr;
}
