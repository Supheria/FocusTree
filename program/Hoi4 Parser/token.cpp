#include "token.h"

using namespace std;
using namespace hoi4::parser;


Token::Token(const TokT& _t, Value _tok, const size_t& _lv) :
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

Token::Token(pcval_u& _tok, const size_t& _lv) :
	tp(TOKEN),
	tok(_vol_(_tok, "NULL_VALUE")),
	lv(_lv)
{
}

const Token::TokT& Token::type() const
{
	return tp;
}

pcval_u& Token::token()
{
	return tok;
}

const size_t& Token::level() const
{
	return lv;
}

Value Token::_vol_(pcval_u& _v, const Value null_val)
{
	if (_v != nullptr)
	{
		return _v.release();
	}
	else
	{
		size_t strlen = 64;
		char* buf = new char[strlen]{ 0 };
		for (size_t i = 0; i < strlen; i++)
		{
			buf[i] = null_val[i];
		}
		return buf;
	}
}
