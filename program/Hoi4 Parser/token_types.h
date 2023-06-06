#ifndef _HOI4_PARSER_TOKEN_TYPES_H_
#define _HOI4_PARSER_TOKEN_TYPES_H_

#include "par_api.h"
#include "token.h"
using namespace hoi4::parser;

// a pcval_u is belong to a Token who uses it, 
// so delete a pointer of Token also will delete its all pcval_u-s.
// 
// if want to pass the ownership of value of pcval_u to another Token,
// create a new dynamic Volumn pointer and use get() of older one to pass value.
//
// also remember the principle of value-passing between pcval_u pointers (see volume.h)

class Token : public IToken
{
protected:
	const token_types tp;
	pcval_u tok;
	const size_t lv; // 0 mean to main-Token or say root-Token
protected:
	// inherited class should not pass nullptr of _tok, use _vol_() to get a pcval_u from an Element
	Token(const token_types& _t, Value _key, const size_t& _lv);
	// return a new Volume, and delete p_vol
	static Value _vol_(pcval_u& _v, const Value null_val);
public:
	// to create a pure token, will delete (*p_key) and set it to nullptr
	Token(pcval_u& _tok, const size_t& _lv);
	const token_types& type() const;
	// use token().get() to transfer ownership of value of tok
	pcval_u& token();
	const size_t& level() const;
};

typedef std::list<pcval_u> volume_list;
typedef volume_list tag_val;
typedef class Tag : public Token
{
	pcval_u op;
	pcval_u tg;
	tag_val val;
public:
	// will delete (*p_key) and set it to nullptr, and so for other ppcval_u* -s
	Tag(pcval_u& _key, pcval_u& _op, pcval_u& _tag, const size_t& _lv);
	// use operat().get() to transfer ownership of value of op, so for others
	pcval_u& operat();
	pcval_u& tag();
	const tag_val& value();
	// will push_back and own _vol
	void append(pcval_u& p_e);
} *pTag;

typedef std::list<volume_list> arr_v;
typedef class ValueArray : public Token
{
	arr_v val;
public:
	// will delete (*p_key) and set it to nullptr, and so for other ppcval_u* -s
	ValueArray(pcval_u& _key, const size_t& _lv);
	const arr_v& value();
	// will push_back and own _vol
	void append(pcval_u& _vol);
	// push_back in to a new array
	void append_new(pcval_u& _vol);
} *pValArr;

typedef std::pair<pcval_u, tag_val> tag_pair;
typedef std::list<tag_pair> tag_pair_list;
typedef std::list<tag_pair_list> arr_t;
typedef class TagArray : public Token
{
	arr_t val;
public:
	// will delete (*p_key) and set it to nullptr, and so for other ppcval_u* -s
	TagArray(pcval_u& _key, const size_t& _lv);
	const arr_t& value();
	// will push_back into tag
	void append(pcval_u& _vol);
	// push_back as a tag
	void append_tag(pcval_u& _vol);
	// push_back in to a new array
	void append_new(pcval_u& _vol);
} *pTagArr;


typedef std::list<ptok_u> token_list;
typedef class Scope : public Token
{
	token_list prop;
public:
	// will delete (*p_key) and set it to nullptr, and so for other ppcval_u* -s
	Scope(pcval_u& _key, const size_t& _lv);
	const token_list& property();
	// if failed will delete, else add to map won't delete
	void append(pToken _t);
} *pScope;

#endif // !_HOI4_PARSER_TOKEN_TYPES_H_