#include "token.h"

using namespace std;

Token::Token(const T& _t, const pVolume _tok, const size_t& _lv) :
	tp(_t),
	tok(_tok),
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

const size_t& Token::level() const
{
	return lv;
}

bool Token::operator==(const pToken _t)
{
	return type() == _t->type() && token() == _t->token() && level() == _t->level();
}

pVolume Token::_vol_(pVolume* const p_vol, const Value& null_val)
{
	pcValue val = nullptr;
	if (p_vol != nullptr && (*p_vol) != nullptr)
	{
		val = (*p_vol)->get();
		delete (*p_vol);
		(*p_vol) = nullptr;
	}
	else
	{
		val = new Value(null_val);
	}
	Volume* vol = new Volume(val);

	return vol;
}

// this pure virtual function need to be called in ~Token(),
							// so it should have implementation.
							// pure virtual func is just a kind of declaration,
							// no limit on implementation.
void Token::del_extend()	
{
}