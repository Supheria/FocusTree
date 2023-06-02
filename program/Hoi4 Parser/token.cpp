#include "token.h"

using namespace std;

Token::Token(const T& _t, pVolume _tok, const size_t& _lv) :
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
	delete tok;
}

Token::Token(pElement* const p_tok, const size_t& _lv) :
	tp(TOKEN),
	tok(_vol_(p_tok, "NULL_VALUE")),
	lv(_lv)
{
}

const Token::T& Token::type() const
{
	return tp;
}

const Volume& Token::token() const
{
	return *tok;
}

const size_t& Token::level() const
{
	return lv;
}

pVolume Token::_vol_(pElement* const p_e, const Value& null_val)
{
	if (p_e != nullptr && (*p_e) != nullptr)
	{
		return new Volume(p_e);
	}
	else
	{
		return new Volume(null_val);
	}
}
