#ifndef _TOKEN_H
#define _TOKEN_H

#include "volume.h"

typedef class Token* pToken;

class Token
{
public:
	enum T
	{
		TOKEN,
		TAG,
		VAL_ARRAY,
		TAG_ARRAY,
		SCOPE
	};
protected:
	const T tp;
	const Volume tok;
	const size_t lv; // 0 mean to main-Token or say root-Token
protected:
	// inherited class should not pass nullptr of _tok, use _vol_() to get a Value from an Element
	Token(const T& _t, pcValue _tok, const size_t& _lv);
	// return a new Volume, and delete p_vol
	static pcValue _vol_(pcValue _v, const Value& null_val);
public:
	// to create a pure token, will delete (*p_key) and set it to nullptr
	Token(pcValue _tok, const size_t& _lv);
	const T& type() const;
	// use token().get() to transfer ownership of value of tok
	const Volume& token() const;
	const size_t& level() const;
public:
	virtual ~Token();
};

#endif // ! _TOKEN_H
