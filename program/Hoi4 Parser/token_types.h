#ifndef _HOI4_PARSER_TOKEN_TYPES_H_
#define _HOI4_PARSER_TOKEN_TYPES_H_

#include "par_api.h"

using namespace hoi4::parser;


class Token : public Tok
{
private:
	pcval_u na;
	const token_types tp;
	const size_t lv;
public:
	pcval_u& name();
	const token_types& type() const;
	const size_t& level() const;
public:
	// will release any pcval passed
	Token(pcval_u& _name, const size_t& _level);
};


class Tagged : public Tgg
{
private:
	pcval_u na;
	const token_types tp;
	const size_t lv;
	pcval_u op;
	pcval_u tg;
	tagged_val val;
public:
	pcval_u& name();
	const token_types& type() const;
	const size_t& level() const;
	pcval_u& operat();
	pcval_u& tag();
	tagged_val& value();
public:
	// will release any pcval passed
	Tagged(pcval_u& _name, pcval_u& _operat, pcval_u& _tag, const size_t& _level);
	// will push_back and own _val
	void append(pcval_u& _val);
};


class ValueArray : public VAr
{
private:
	pcval_u na;
	const token_types tp;
	const size_t lv;
	arr_v val;
public:
	pcval_u& name();
	const token_types& type() const;
	const size_t& level() const;
	arr_v& value();
public:
	// will release any pcval passed
	ValueArray(pcval_u& _name, const size_t& _level);
	// will push_back and own _val
	void append(pcval_u& _val);
	// push_back in to a new array
	void append_new(pcval_u& _val);
};


class TagArray : public TAr
{
private:
	pcval_u na;
	const token_types tp;
	const size_t lv;
	arr_t val;
public:
	pcval_u& name();
	const token_types& type() const;
	const size_t& level() const;
	arr_t& value();
public:
	//  will release any pcval passed
	TagArray(pcval_u& _name, const size_t& _level);
	// will push_back into tag
	void append(pcval_u& _val);
	// push_back as a tag
	void append_tag(pcval_u& _val);
	// push_back in to a new array
	void append_new(pcval_u& _val);
};


class Scope : public Sco
{
private:
	pcval_u na;
	const token_types tp;
	const size_t lv;
	token_list prop;
public:
	pcval_u& name();
	const token_types& type() const;
	const size_t& level() const;
	token_list& property();
public:
	// will release any pcval passed
	Scope(pcval_u& _name, const size_t& _level);
	// if failed will delete, else add to map that won't delete
	void append(pToken _t);
};

#endif // !_HOI4_PARSER_TOKEN_TYPES_H_