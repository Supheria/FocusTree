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
	pVolume op;
	pVolume val;
public:
	ValueKey(pVolume* const p_key, pVolume* const p_val, pVolume* const p_op, const size_t& _lv);
	const Volume& operat();
	const Volume& value();
	// will delete _t
	void mix(pToken _t);
private:
	void del_extend();
};


typedef std::list<pVolume> tag_val;
class Tag : public Token
{
	pVolume tg;
	tag_val val;
public:
	Tag(pVolume* const p_key, pVolume* const p_tag, const size_t& _lv);
	const Volume& tag();
	const tag_val& value();
	// will delete _t
	void mix(pToken _t);
	// will push_back and own _vol
	void append(pElement* const p_e);
private:
	void del_val();
	void del_extend();
};


typedef std::list<std::list<pVolume>> arr_v;
class ValueArray : public Token
{
	arr_v val;
public:
	ValueArray(pVolume* p_key, const size_t& _lv);
	const arr_v& value();
	// will delete _t
	void mix(pToken _t);
	// will push_back and own _vol
	void append(pElement* const p_e);
	// push_back in to a new array
	void append_new(pElement* const p_e);
private:
	void del_extend();
};

typedef std::list<std::list<pToken>> arr_t;
class TagArray : public Token
{
	arr_t val;
public:
	TagArray(pVolume* p_key, const size_t& _lv);
	const arr_t& Value();
	// will delete _t
	void mix(pToken _t);
	// will push_back and own _vol
	void append(pElement* const p_e);
	// push_back in to a new array
	void append_new(pElement* const p_e);
private:
	void del_extend();
};

typedef std::unordered_map<pcValue, pToken> tok_map;
class Scope : public Token
{
 	tok_map prop;
public:
	Scope(pVolume* p_key, const size_t& _lv);
	const tok_map& property();
	// will delete _t
	void mix(pToken _t);
	// if failed will delete, else add to map won't delete
	void append(pToken _t);
private:
	void del_extend();
};