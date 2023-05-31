#include "token.h"

using namespace std;

Token::Token(const T& _t, const pVolume _tok, pcValue _fr, const size_t& _lv) :
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

const Value& Token::from() const
{
	return *fr;
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
	if ((*p_vol) != nullptr)
	{
		val = (*p_vol)->get();
	}
	else
	{
		val = new Value(null_val);
	}
	Volume* vol = new Volume(val);

	delete (*p_vol);
	(*p_vol) = nullptr;
	return vol;
}

// this pure virtual function need to be called in ~Token(),
							// so it should have implementation.
							// pure virtual func is just a kind of declaration,
							// no limit on implementation.
void Token::del_extend()	
{
}
