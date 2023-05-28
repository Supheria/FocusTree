#ifndef _TOKEN_KEY_H
#define _TOKEN_KEY_H

#include <vector>
#include <unordered_map>
#include "token.h"
#include "exception.h"

constexpr auto FileName = "key.cpp";;

class ValueKey : public Token
{
	const char* const op;
	const Token* val;
public:
	ValueKey(std::string* _key, Token* _val, char* _op, Token* _fr)
		: Token(VAL_KEY, _key, _fr),
		op(_op),
		val(_val)
	{
	}
	~ValueKey()
	{
		if (op != nullptr) { delete op; }
		if (val != nullptr) { delete val; }
	}
	const Token* value() { return val; }
	// if are equal will do replacement, which will delete value and log warning: repeat assign
	void append(Token* _t)
	{
		if (_t == nullptr) { return; }
		if (level() != _t->level() || token() != _t->token())
		{
			if (_t->type() != VALUE)
			{
				throw new ErroExc(FileName, "incompatible types");
			}
			if (_t->)
				delete val;
			val = _t;
			delete _t;
			throw new WarnExc(FileName, "repeated assignment");
		}
		if (type() != _t->type())
		{
			delete _t;
			throw new ErroExc(FileName, "different type for replacement");
		}
		delete val;
		val = ((ValueKey*)_t)->value();
		delete _t;
		throw new WarnExc(FileName, "repeated assignment");
		
	}
};

typedef std::vector<Token*> tag_val;

class Tag : public Token
{
	const Token* const tg;
	const std::vector<Token*>* const val;
public:
	Tag(std::string* _key, Token* _tag, Token* _fr)
		: Token(TAG, _key, _fr),
		tg(_tag),
		val(new tag_val{new Tag(), 2})
	{
	}
	~Tag()
	{
		if (tg != nullptr) { delete tg; }
		for (auto el : *val)
		{
			if (el != nullptr) { delete el; }
		}
		delete val;
	}
	const tag_val* value() { return val; }
	// if are equal will do replacement, which will delete value and log warning: repeat assign
	// otherwise only if type is Value will be added, or will be delete and log erro: assign wrong type
	void append(const Token* _t) const
	{
		if (equals(_t))
		{
			for (auto el : *val)
			{
				if (el != nullptr) { delete el; }
			}
			val = ((Tag*)_t)->value();
			delete _t;
		}
		if (_t->type() == T::Value)
		{
			val->pu
		}
	}
};

class Array : public Token
{
	bool sarr;
	std::vector<std::vector<const Token*>> val;
	bool addnew;
public:
	Array(const std::string* _key, bool _sarr, const Token* _fr)
		: Token(T::Array, _key, _fr),
		sarr(_sarr),
		addnew(false)
	{
	}
	~Array()
	{
		for (std::vector<const Token*> arr : val)
		{
			for (auto elm : arr)
			{
				if (elm != nullptr) { delete elm; }
			}
		}
	}
	void append(const Token* value)
	{
		if (addnew) 
		{ 
			val.push_back({}); 
			addnew = false;
		}
		val[val.size() - 1].push_back(value);
	}
	// set value to append to the beginning of a new array
	void set_new() { addnew = true; }
};

class Scope : public Token
{
	std::unordered_map<const std::string&, const Token*> props; // use const string & here since Token::get() return such a type
public:
	Scope(const std::string* _key, const Token* _fr)
		: Token(T::Scope, _key, _fr)
	{
	}
	~Scope()
	{
		for (std::pair<const std::string&, const Token*> p : props)
		{
			if (p.second != nullptr) { delete p.second; }
		}
	}
	void append(const Token* prop)
	{
		if (equals(prop))
		{
			// combine same scope's map elements
		}
		else
		{
			if (props.count(prop->token()) > 0)
			{
				props[prop->token()]->append(prop);
			}
			else { props[prop->token()] = prop; }
		}
	}
};

#endif // !_TOKEN_KEY_H