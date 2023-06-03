#pragma once
#include "token.h"
#include <list>


// a Volume is belong to a Token who uses it, 
// so delete a pointer of Token also will delete its all Volume-s.
// 
// if want to pass the ownership of value of Volume to another Token,
// create a new dynamic Volumn pointer and use get() of older one to pass value.
//
// also remember the principle of value-passing between Volume pointers (see volume.h)

class Tag;


typedef std::list<Volume> volume_list;
typedef volume_list tag_val;
typedef class Tag : public Token
{
	Volume op;
	Volume tg;
	tag_val val;
public:
	// will delete (*p_key) and set it to nullptr, and so for other pVolume* -s
	Tag(pcValue _key, pcValue _op, pcValue _tag, const size_t& _lv);
	~Tag();
	// use operat().get() to transfer ownership of value of op, so for others
	const Volume& operat();
	const Volume& tag();
	const tag_val& value();
	// will push_back and own _vol
	void append(pcValue p_e);
} *pTag;

typedef std::list<volume_list> arr_v;
typedef class ValueArray : public Token
{
	arr_v val;
public:
	// will delete (*p_key) and set it to nullptr, and so for other pVolume* -s
	ValueArray(pcValue _key, const size_t& _lv);
	~ValueArray();
	const arr_v& value();
	// will push_back and own _vol
	void append(pcValue _vol);
	// push_back in to a new array
	void append_new(pcValue _vol);
} *pValArr;

typedef std::pair<Volume, tag_val> tag_pair;
typedef std::list<tag_pair> tag_pair_list;
typedef std::list<tag_pair_list> arr_t;
typedef class TagArray : public Token
{
	arr_t val;
public:
	// will delete (*p_key) and set it to nullptr, and so for other pVolume* -s
	TagArray(pcValue _key, const size_t& _lv);
	~TagArray();
	const arr_t& value();
	// will push_back into tag
	void append(pcValue _vol);
	// push_back as a tag
	void append_tag(pcValue _vol);
	// push_back in to a new array
	void append_new(pcValue _vol);
} *pTagArr;


typedef std::list<pToken> token_list;
typedef class Scope : public Token
{
	token_list prop;
public:
	// will delete (*p_key) and set it to nullptr, and so for other pVolume* -s
	Scope(pcValue _key, const size_t& _lv);
	~Scope();
	const token_list& property();
	// if failed will delete, else add to map won't delete
	void append(pToken _t);
} *pScope;