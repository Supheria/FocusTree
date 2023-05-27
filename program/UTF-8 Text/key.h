#ifndef _TOKEN_KEY_H
#define _TOKEN_KEY_H

#include <vector>
#include <unordered_map>
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
	const Token* fr;
public:
	KeyTypes type() { return tp; }
	const Token* from() { return fr; }
protected:
	Key(KeyTypes _tp, const std::string* _key, const Token* _fr);
	~Key();
public:
	const Token* parse(const std::string& filename);
	// const Key* will be delete here, whatever combination is successful or not
	void combine(const T* _k);
	virtual void append(const Token* _t) = 0;
};

class ValueKey : public Key
{
	const char* op;
	const Token* val;
public:
	ValueKey(const std::string* _key, const Token* _value, const char* _oprat, const Key* _from)
		: Key(KeyTypes::Value, _key, _from),
		op(_oprat),
		val(_value)
	{
	}
	~ValueKey()
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
	// set value to append to the beginning of a new array
	void set_new() { addnew = true; }
};

class Scope : public Key
{
	std::unordered_map<const std::string&, const Token*> props; // use const string & for operating this memory by Token* other than map's key, 
public:
	Scope(const std::string* _key, const Key* _from)
		: Key(KeyTypes::Scope, _key, _from)
	{
	}
	void append(const Token* property)
	{
		if (props.count(property->)
	}
};

#endif // !_TOKEN_KEY_H