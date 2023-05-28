#ifndef _TOKEN_KEY_H
#define _TOKEN_KEY_H

#include <vector>
#include <unordered_map>
#include "token.h"
#include "exception.h"

constexpr auto FileName = "key.cpp";

class ValueKey : public Token
{
	const char* op; // the pointer to op may change, and also for other key-types' values or props
	const Token* val;
public:
	ValueKey(const std::string* _key, const Token* _val, const char* _op, const Token* _fr, const size_t _lv = 0)
		: Token(VAL_KEY, _key, _fr, _lv),
		op(_op),
		val(_val)
	{
	}
	~ValueKey()
	{
		if (op != nullptr) { delete op; }
		if (val != nullptr) { delete val; }
	}
	const char* operat() { return op; }
	const Token* value() { return val; }
	void append(const Token* _t)
	{
		if (_t == nullptr) { return; }
		//
		// replacement
		//
		if (token() == _t->token() && type() == _t->type())
		{
			if (level() == _t->level())
			{
				delete op;
				op = ((ValueKey*)_t)->operat();
				delete val;
				val = ((ValueKey*)_t)->value();
				errlog(FileName, "Replacement occurs in value of ValueKey", ErrorLog::WARN);
			}
			else { errlog(FileName, "Level mismatch in Value replacement", ErrorLog::ERRO); }
		}
		else if (_t->type() == T::VALUE)
		{
			errlog(FileName, "ValueKey cannot append value, assignment should be in constructor", ErrorLog::ERRO);
		}
		else { errlog(FileName, "Error type of appending to ValueKey ", ErrorLog::ERRO); }

		delete _t;
	}
};

typedef std::vector<const Token*> tag_val;

// no use of const vector or map, since its element will be changed

class Tag : public Token
{
	const Token* tg;
	tag_val* val;
private:
	void del_val()
	{
		for (auto el : *val)
		{
			if (el != nullptr) { delete el; }
		}
		delete val;
	}
public:
	Tag(const std::string* _key, const Token* _tag, const Token* _fr, const size_t _lv = 0)
		: Token(TAG, _key, _fr, _lv),
		tg(_tag),
		val(new tag_val)
	{
	}
	~Tag()
	{
		if (tg != nullptr) { delete tg; }
		del_val();
	}
	tag_val* value() { return val; }
	// if are equal will do replacement, which will delete value and log warning: repeat assign
	// otherwise only if type is Value will be added, or will be delete and log erro: assign wrong type
	void append(const Token* _t)
	{
		if (_t == nullptr) { return; }
		//
		// replacement
		//
		if (token() == _t->token() && type() == _t->type())
		{
			if (level() == _t->level())
			{
				del_val();
				val = ((Tag*)_t)->value();
				errlog(FileName, "Replacement occurs in value of Tag", ErrorLog::WARN);
			}
			else { errlog(FileName, "Level mismatch in Tag replacement", ErrorLog::ERRO); }
		}
		//
		// assignment
		//
		else if (_t->type() == VALUE && _t->from()->token() == token())
		{
			if (_t->level() == level() + 1)
			{
				val->push_back(_t);
			}
			else{ errlog(FileName, "Level mismatch in Tag assignment", ErrorLog::ERRO); }
		}
		else { errlog(FileName, "Error type of appending to Tag ", ErrorLog::ERRO); }

		delete _t;
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