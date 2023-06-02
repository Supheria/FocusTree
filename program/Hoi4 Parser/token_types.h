#pragma once
#include "token.h"
#include <list>


// a Volume is belong to a Token who uses it, 
// so delete a pointer of Token also will delete its all Volume-s.
// 
// if want to pass the ownership of Volume to another Token,
// create a new dynamic Volumn pointer and pass the value from older one.
//
// also remember the principle of value-passing between Volume pointers (see volume.h)

class Tag;


typedef std::list<pVolume> volume_list;
typedef volume_list tag_val;
class Tag : public Token
{
	pVolume op;
	pVolume tg;
	tag_val val;
public:
	// will delete (*p_key) and set it to nullptr, and so for other pVolume* -s
	Tag(pVolume* const p_key, pVolume* const p_op, pVolume* const p_tag, const size_t& _lv);
	~Tag();
	const Volume& operat();
	const Volume& tag();
	const tag_val& value();
	// will push_back and own _vol
	void append(pElement* const p_e);
};

class Array : public Token
{
public:
	Array(const T& _t, pVolume* p_key, const size_t& _lv);
	virtual ~Array() = 0;
	// push_back in to a new array
	virtual void append_new(pElement* const p_e) = 0;
};

typedef std::list<volume_list> arr_v;
class ValueArray : public Array
{
	arr_v val;
public:
	// will delete (*p_key) and set it to nullptr, and so for other pVolume* -s
	ValueArray(pVolume* p_key, const size_t& _lv);
	~ValueArray();
	const arr_v& value();
	// will push_back and own _vol
	void append(pElement* const p_e);
	// push_back in to a new array
	void append_new(pElement* const p_e);
};

typedef std::pair<pVolume, tag_val> tag_pair;
typedef std::list<tag_pair> tag_pair_list;
typedef std::list<tag_pair_list> arr_t;
class TagArray : public Array
{
	arr_t val;
public:
	// will delete (*p_key) and set it to nullptr, and so for other pVolume* -s
	TagArray(pVolume* p_key, const size_t& _lv);
	~TagArray();
	const arr_t& value();
	// will push_back into tag
	void append(pElement* const p_e);
	// push_back as a tag
	void append_tag(pElement* const p_e);
	// push_back in to a new array
	void append_new(pElement* const p_e);
};


typedef std::list<pToken> token_list;
typedef class Scope : public Token
{
	token_list prop;
public:
	// will delete (*p_key) and set it to nullptr, and so for other pVolume* -s
	Scope(pVolume* p_key, const size_t& _lv);
	~Scope();
	const token_list& property();
	// if failed will delete, else add to map won't delete
	void append(pToken _t);
} *pScope;