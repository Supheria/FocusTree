#ifndef _TOKEN_KEY_H
#define _TOKEN_KEY_H

#include <vector>
#include "token.h"

enum KeyTypes
{
	Value,
	Tag,
	Array,
	Scope
};

class Key : public Token
{
	KeyTypes tp;
	const Key* fr;
public:
	KeyTypes type() { return tp; }
	const Key* from() { return fr; }
protected:
	Key(KeyTypes _tp, const std::string* _key, const Key* _fr);
	~Key();
public:
	const Key* parse(const std::string& filename);
	// const Key* will be delete here, whatever combination is successful or not
	void combine(const Key* k);
	virtual void append(const Token* _t) = 0;
};

class Value : public Key
{
	const char* op;
	const Token* val;
public:
	Value(const std::string* _key, const Token* _value, const char* _oprat, const Key* _from)
		: Key(KeyTypes::Value, _key, _from),
		op(_oprat),
		val(_value)
	{
	}
	~Value()
	{
		if (op != nullptr) { delete op; }
		if (val != nullptr) { delete val; }
	}
	// do nothing
	void append(const Token* _t) {}
};

class Tag : public Key
{
	const Token* tag;
	std::vector<const Token*> val;
public:
	Tag(const std::string* _key, const Token* _tag, const Key* _from)
		: Key(KeyTypes::Tag, _key, _from),
		tag(_tag)
	{
	}
	~Tag()
	{
		if (tag != nullptr) { delete tag; }
		for (auto elm : val)
		{
			if (elm != nullptr) { delete elm; }
		}
	}
	void append(const Token* value) { val.push_back(value); }
};

class Array : public Key
{
	bool sarr;
	std::vector<std::vector<const Token*>> val;
	bool addnew;
public:
	Array(const std::string* _key, bool _sarr, const Key* _from)
		: Key(KeyTypes::Array, _key, _from),
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
	// set value to append the beginning of a new array
	void set_new() { addnew = true; }
};

#endif // !_TOKEN_KEY_H