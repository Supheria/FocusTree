#ifndef _TOKEN_H
#define _TOKEN_H

#include "volume.h"

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
	const Volume* const tok;
	const std::string* const fr;
	mutable size_t lv;
public:
	// inherited class should not pass nullptr of _tok, use _vol_() to get a Value from an Element
	Token(const T& _t, const Volume* _tok, const std::string* _fr, const size_t& _lv);
	~Token();
	const T& type() const;
	// use this->token().get() to get specific string-value of tok
	const Volume& token() const;
	// return nullptr means to main-Token or say root-Token
	const std::string& from() const;
	const size_t& level() const;
public:
	bool operator==(const Token* _t);
	// judge if _sub is sub-Token
	bool operator>(const Token* _sub);
protected:
	// return a new Volume, and delete _e
	static Volume* _vol_(Volume** const p_vol, const std::string& null_vol);
	// will be called by ~Token(), for inherited class to delete their extend pointer in destruction
	virtual void del_extend() = 0; // do not want to delete tok in inherited class
public:
	virtual void mix(Token* _t) = 0;
};

#endif // ! _TOKEN_H
