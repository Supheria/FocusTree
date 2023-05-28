#ifndef _TOKEN_H
#define _TOKEN_H

#include <string>

// no use of const Token instance, since Token::lv is dynamic
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
	const std::string* const tok; // will be "NULL" if given pointer is nullptr
	// nullptr means to main or say root
	const Token* const fr;
	const T tp;
	// level start from 1 to use 0 present uninitialized other than -1
	const size_t* const lv;
public:
	Token(const T _t, const std::string* _s, const Token* _fr, const size_t _lv);
	~Token();
	const T& type() const;
	const std::string& token() const; // use const string & to maintain control on memeory this->token points to
	const Token* const from() const;
	const size_t& level() const;
	// parse from file with this->token(), and return a pointer to may new type of Token or null if failed to parse
	Token* const parse(std::string& filename);
	// will pass control right to me,
	// for replacement or combination, will delete given pointer whatever succeed or not 
	virtual void append(Token* _t) = 0;
};

class Value : public Token
{
public:
	Value(std::string* volume, Token* from, const size_t _lv)
		: Token(VALUE, volume, from, _lv)
	{
	}
	// do nothing, and will throw an erro log
	void append(Token*)
	{
	}
};
#endif