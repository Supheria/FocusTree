#include "token.h"

using namespace std;

Token::Token(const T& _t, const Volume* _tok, const string* _fr, const size_t& _lv) :
	tp(_t),
	tok(_tok),
	fr(_fr),
	lv(_lv)
{
}

Token::~Token()
{
	delete tok;
	del_extend();
}

const Token::T& Token::type() const
{
	return tp;
}

const Volume& Token::token() const
{
	return *tok;
}

const string& Token::from() const
{
	return *fr;
}

const size_t& Token::level() const
{
	return lv;
}

bool Token::operator==(const Token* _t)
{
	return type() == _t->type() && token() == _t->token() && level() == _t->level();
}

bool Token::operator>(const Token* _sub)
{
	return token().volumn() == _sub->from() && _sub->level() == level() + 1;
}

Volume* Token::_vol_(Volume** const p_vol, const std::string& null_vol)
{
	const string* _v = nullptr;
	if ((*p_vol) != nullptr)
	{
		_v = (*p_vol)->get();
	}
	else
	{
		_v = new string(null_vol);
	}
	Volume* val = new Volume(_v);

	delete (*p_vol);
	(*p_vol) = nullptr;
	return val;
}

// this pure virtual function need to be called in ~Token(),
							// so it should have implementation.
							// pure virtual func is just a kind of declaration,
							// no limit on implementation.
void Token::del_extend()	
{
}
