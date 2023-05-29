#ifndef _TOKEN_KEY_H
#define _TOKEN_KEY_H

#include <vector>
#include <unordered_map>
#include "token.h"
#include "exception.h"

class Value : public Token
{
public:
	// volumn will be deleted 
	Value(const Element* volume, Token* from, const size_t& _lv);
	void append(Token*);
};


// the pointer to key-type Token' values or props may change, no use of (* const)  <-const pointer


class ValueKey : public Token
{
	const char* op; // new char[], must use delete[]
	const Token* val;
	mutable bool lose_op;
	mutable bool lose_val;
public:
	// _key and _op will be deleted
	ValueKey(const Element* _key, const Token* _val, const Element* _op, const Token* _fr, const size_t& _lv = 0);
	~ValueKey();
	// for compare with, won't lose op
	const char& operat() const;
	// for compare with, won't lose val
	const Token& value() const;
	// if has been called for any time will lose op, and ~Token() won't delete[] op.
	const char* get_op() const;
	// if has been called for any time will lose val, and ~Token() won't delete[] val.
	const Token* get_val() const;
	void append(const Token* _t);
};

// no use of const vector or map, since its element will be changed

typedef std::vector<const Token*> tag_val;
class Tag : public Token
{
	const char* tg;
	tag_val* val;
private:
	void del_val();
public:
	// _key should be a new char[] from eToken
	Tag(const Element* _key, const Element* _tag, const Token* _fr, const size_t _lv = 0);
	~Tag();
	const char* tag() const;
	const tag_val& val();
	const char* get_tag() const;
	const 
	tag_val* value();
	void append(const Token* _t);
};


typedef std::vector<std::vector<const Token*>> arr_val;
class Array : public Token
{
	arr_val* val;
	bool addnew;
public:
	Array(const Element* _key, const bool sarr, const Token* _fr, const size_t _lv = 0);
	~Array();
	arr_val* value();
	void append(const Token* _t);
	// set value assigned to a new array beginning
	// when called append() will set false automatically, only if assignment succeed
	void set_new();
};


typedef std::unordered_map<const std::string&, const Token*> tok_map;
class Scope : public Token
{
	tok_map* props;
public:
	Scope(const Element* _key, const Token* _fr, const size_t _lv = 0)
		: Token(SCOPE, _e_val(_key, NLL_KEY), _fr, _lv),
		props(new tok_map)
	{
	}
	~Scope()
	{
		for (std::pair<const std::string&, const Token*> p : *props)
		{
			if (p.second != nullptr) { delete p.second; }
		}
		delete props;
	}
	tok_map* property() { return props; }
	void append(const Token* _t)
	{
		if (_t == nullptr) { return; }
		//
		// combination
		//
		if (token() == _t->token() && type() == _t->type())
		{
			if (level() == _t->level())
			{
				for (std::vector<const Token*> arr : *((Scope*)_t)->value())
				{
					val->push_back(arr);
				}
				WarnLog(FileName, "combination occurs in value of Tag");
			}
			else { ErrLog(FileName, "level mismatch in Array combination"); }
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