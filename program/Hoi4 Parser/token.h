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
	pVolume tok;
	const size_t lv; // 0 mean to main-Token or say root-Token
public:
	// inherited class should not pass nullptr of _tok, use _vol_() to get a Value from an Element
	Token(const T& _t, pVolume _tok, const size_t& _lv);
	// to create a pure token
	Token(pVolume* const p_tok, const size_t& _lv);
	const T& type() const;
	// use token().get() to get specific string-value of tok
	const Volume& token() const;
	const size_t& level() const;
public:
	bool operator==(const pToken _t);
protected:
	// return a new Volume, and delete _e
	static pVolume _vol_(pVolume* const p_vol, const Value& null_val);
public:
	virtual ~Token();
};

#endif // ! _TOKEN_H
