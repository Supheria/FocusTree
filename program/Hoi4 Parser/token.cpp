#include "token.h"

using namespace std;

Token::Token(const T& _t, pcValue _tok, const size_t& _lv) :
	tp(_t),
	tok(_tok),
	lv(_lv)
{
}
// this pure virtual function need to be called in deconstruction,
				// so it should have implementation.
				// pure virtual func is just a kind of declaration,
				// no limitations on implementation.
Token::~Token()
{
}

Token::Token(pcValue _tok, const size_t& _lv) :
	tp(TOKEN),
	tok(_vol_(_tok, "NULL_VALUE")),
	lv(_lv)
{
}

const Token::T& Token::type() const
{
	return tp;
}

const Volume& Token::token() const
{
	return tok;
}

const size_t& Token::level() const
{
	return lv;
}

pcValue Token::_vol_(pcValue _v, const Value& null_val)
{
	if (_v != nullptr)
	{
		return (_v);
	}
	else
	{
		return new std::string(null_val);
	}
}
