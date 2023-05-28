#ifndef _TOKEN_H
#define _TOKEN_H

#include <string>

class Token
{
public:
	enum T
	{
		VALUE,
		//
		// key
		//
		VAL_KEY,
		TAG,
		ARRAY,
		SCOPE
	};
private:
	const std::string* const tok;
	// nullptr means to main or say root
	const Token* const fr;
	const T tp;
	// level start from 1 to use 0 present uninitialized other than -1
	size_t lv;
public:
	Token(T _t, std::string* _s, Token* _fr);
	~Token();
	const T& type();
	const std::string& token(); // use const string & to maintain control on memeory this->token points to
	const Token* const from() const;
	const size_t& level();
	// parse from file with this->token(), and return a pointer to may new type of Token or null if failed to parse
	const Token* const parse(std::string& filename);
	// for replacement or combination, will delete given pointer whatever succeed or not 
	virtual void append(Token* _t) = 0;
};

class Value : public Token
{
public:
	Value(std::string* volume, Token* from)
		: Token(VALUE, volume, from)
	{
	}
	// do nothing, and will throw an erro log
	void append(Token*)
	{
	}
};
#endif