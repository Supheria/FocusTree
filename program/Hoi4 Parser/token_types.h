#pragma once
#include "token.h"
#include <list>
#include <unordered_map>


// a Volume is belong to a Token who uses it, 
// so delete a pointer of Token also will delete its all Volume-s.
// 
// if want to pass the ownership of Volume to another Token,
// create a new dynamic Volumn pointer and pass the value from older one.
//
// also remember the principle of value-passing between Volume pointers (see volume.h)


class ValueKey : public Token
{
	Volume* op;
	Volume* val;
public:
	ValueKey(const Element* _key, const Element* _val, const Element* _op, const std::string* _fr, const size_t& _lv);
	const Volume& operat();
	const Volume& value();
	// will delete _t
	void mix(Token* _t);
private:
	void del_extend();
};


typedef std::list<const Volume*> tag_val;
class Tag : public Token
{
	Volume* tg;
	tag_val val;
public:
	Tag(const Element* _key, const Element* _tag, const std::string* _fr, const size_t& _lv);
	const Volume& tag();
	const tag_val& value();
	// will delete _t
	void mix(Token* _t);
	// will push_back and own _vol
	void append(const Volume* _vol);
private:
	void del_val();
	void del_extend();
};


typedef std::list<std::list<const Volume*>> arr_val;
class Array : public Token
{
	arr_val val;
	bool addnew;
public:
	Array(const Element* _key, const bool sarr, const std::string* _fr, const size_t& _lv);
	const arr_val& value();
	// set to a new array beginning, after next append will auto set to false
	void set_new();
	// will delete _t
	void mix(Token* _t);
	// will push_back and own _vol
	void append(const Volume* _vol);
private:
	void del_extend();
};


typedef std::unordered_map<const std::string*, Token*> tok_map;
class Scope : public Token
{
 	tok_map prop;
public:
	Scope(const Element* _key, const std::string* _fr, const size_t& _lv);
	const tok_map& property();
	// will delete _t
	void mix(Token* _t);
	// if failed will delete, else add to map won't delete
	void append(Token* _t);
private:
	void del_extend();
};