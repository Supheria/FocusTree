#ifndef _TOKEN_H
#define _TOKEN_H

#include "volume.h"

typedef class Token* pToken;

class Token
{
public:
	enum T
	{
		VAL_KEY,
		TAG,
		VAL_ARRAY,
		S_ARRAY,
		SCOPE
	};
private:
	const T tp;
	const pVolume tok;
	const pcValue fr;
	mutable size_t lv;
public:
	// inherited class should not pass nullptr of _tok, use _vol_() to get a Value from an Element
	Token(const T& _t, const pVolume _tok, pcValue _fr, const size_t& _lv);
	~Token();
	const T& type() const;
	// use this->token().get() to get specific string-value of tok
	const Volume& token() const;
	// return nullptr means to main-Token or say root-Token
	const Value& from() const;
	const size_t& level() const;
public:
	bool operator==(const pToken _t);
protected:
	// return a new Volume, and delete _e
	static pVolume _vol_(pVolume* const p_vol, const Value& null_val);
	// will be called by ~Token(), for inherited class to delete their extend pointer in destruction
	virtual void del_extend() = 0; // do not want to delete tok in inherited class
public:
	virtual void mix(pToken _t) = 0;
};

#endif // ! _TOKEN_H
