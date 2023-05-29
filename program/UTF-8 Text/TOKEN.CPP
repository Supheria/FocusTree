#include "token.h"

Token::Token(const T& _t, const char* _tok, const Token* _fr, const size_t& _lv) :
	tp(new T(_t)),
	tok(_tok),
	fr(_fr),
	lv(0)
{
}

Token::~Token()
{
	if (tp != nullptr) { delete tp; }
	if (tok != nullptr) { delete[] tok; } // must use delete[]
	if (fr != nullptr) { delete fr; }
}

const Token::T& Token::type() const
{
	return *tp;
}

const std::string& Token::token() const
{
	return std::string(tok); // convenient to compare
}

const Token* const Token::from() const
{
	return fr;
}

const size_t& Token::level() const
{
	if (lv != 0) { return lv; }
	lv = 1;
	const Token* cur = fr;
	while (cur != nullptr)
	{
		lv++;
	}
	return lv;
}

Token* const Token::parse(std::string& filename)
{
	return nullptr;
}

const char* Token::_e_val(const Element* _e, const char* null_val)
{
	const char* val = nullptr;
	if (_e != nullptr)
	{
		val = _e->get();
		delete _e;
	}
	if (val == nullptr)
	{
		val = null_val;
	}
	return val;
}