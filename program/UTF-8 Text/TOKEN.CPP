#include "token.h"

const size_t* _level(const Token* _fr)
{
	size_t* lv = new size_t(1); // will be delete in ~Token()
	const Token* cur = _fr;
	while (cur != nullptr)
	{
		(*lv)++;
	}
	return lv;
}

Token::Token(const T _t, const std::string* _s, const Token* _fr, const size_t _lv) :
	tp(_t),
	tok(_s == nullptr ? new std::string("NULL") : _s),
	fr(_fr),
	lv(_lv == 0 ? _level(_fr) : new size_t(_lv))
{
}

Token::~Token()
{
	if (tok != nullptr) { delete tok; }
	if (fr != nullptr) { delete fr; }
	if (lv != nullptr) { delete lv; }
}

const Token::T& Token::type() const
{
	return tp;
}

const std::string& Token::token() const
{
	return *tok;
}

const Token* const Token::from() const
{
	return fr;
}

const size_t& Token::level() const
{
	return *lv;
}

Token* const Token::parse(std::string& filename)
{
	return nullptr;
}
