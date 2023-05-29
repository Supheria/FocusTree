#ifndef _TOKEN_H
#define _TOKEN_H

#include <string>
#include "element.h"

class Token
{
public:
	enum T
	{
		VALUE,
		VAL_KEY,
		TAG,
		VAL_ARRAY,
		S_ARRAY,
		SCOPE
	};
private:
	// tok must be a new char[], won't be nullptr since every inherited class should call _e_val()
	const char* const tok;
	// nullptr means to main or say root
	const Token* const fr;
	const T* const tp;
	// level start from 1 to use 0 present uninitialized other than -1
	mutable size_t lv;
	//mutable bool lose_tok;
public:
	Token(const T& _t, const char* _tok, const Token* _fr, const size_t& _lv);
	~Token();
	const T& type() const;
	// for compare with, won't lose tok
	const char* token() const;
	// if has been called for any time will lose tok, and ~Token() won't delete[] tok.
	const char* get_tok() const;
	const Token* const from() const;
	const size_t& level() const;
	// parse from file with this->token(), and return a pointer to may new type of Token or null if failed to parse
	Token* const parse(std::string& filename);
	// will pass control right to me,
	// for replacement or combination, will delete given pointer whatever succeed or not 
	virtual void append(Token* _t) = 0;
protected:
	// return Element::val and delete _e
	static const char* _e_val(const Element* _e, const char* null_val);
};
#endif