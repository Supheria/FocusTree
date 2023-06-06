#include "token_types.h"
#include "use_ex_log.h"

using namespace std;
using namespace hoi4::parser;

const char* fn_tt = "token_types";
const Value NLL_KEY = "NULL_KEY";
const Value NLL_OP = "NULL_OPERATOR";
const Value NLL_TAG = "NULL_TAG";
//
//
//
// Tag
//
//
//
Tag::Tag(pcval_u& _key, pcval_u& _op, pcval_u& _tag, const size_t& _lv)
	: Token(TAG, _vol_(_key, NLL_KEY), _lv),
	op(_vol_(_op, NLL_OP)),
	tg(_vol_(_tag, NLL_TAG))
{
}

Tag::~Tag()
{
}

pcval_u& Tag::operat()
{
	return op;
}

pcval_u& Tag::tag()
{
	return tg;
}

const tag_val& Tag::value()
{
	return val;
}

void Tag::append(pcval_u& _e)
{
	if (_e == nullptr ) { return; }
	val.push_back(move(_e));
}
//
//
//
// ValueArray
//
//
//
ValueArray::ValueArray(pcval_u& _key, const size_t& _lv)
	: Token(VAL_ARRAY, _vol_(_key, NLL_KEY), _lv)
{
}

ValueArray::~ValueArray()
{
}

const arr_v& ValueArray::value()
{
	return val;
}

void ValueArray::append(pcval_u& _vol)
{
	if (_vol == nullptr) { return; }
	val.back().push_back(move(_vol));
}

void ValueArray::append_new(pcval_u& _vol)
{
	if (_vol == nullptr) { return; }
	val.push_back(volume_list());
	val.back().push_back(move(_vol));
}
//
//
//
// TagArray
//
//
//
TagArray::TagArray(pcval_u& _key, const size_t& _lv)
	: Token(TAG_ARRAY, _vol_(_key, NLL_KEY), _lv)
{
}

TagArray::~TagArray()
{
}

const arr_t& TagArray::value()
{
	return val;
}

void TagArray::append(pcval_u& _vol)
{
	if (_vol == nullptr) { return; }
	val.back().back().second.push_back(move(_vol));
}

void TagArray::append_tag(pcval_u& _vol)
{
	if (_vol == nullptr) { return; }
	val.back().push_back(tag_pair(move(_vol), tag_val()));
}

void TagArray::append_new(pcval_u& _vol)
{
	if (_vol == nullptr) { return; }
	val.push_back(tag_pair_list());
	val.back().push_back(tag_pair(move(_vol), tag_val()));
}
//
//
//
// Scope
//
//
//
Scope::Scope(pcval_u& _key, const size_t& _lv)
	: Token(SCOPE, _vol_(_key, NLL_KEY), _lv)
{
}

Scope::~Scope()
{
}

const token_list& Scope::property()
{
	return prop;
}

void Scope::append(pToken _t)
{
	if (_t == nullptr) { return; }
	if (_t->level() != level() + 1)
	{
		ex_log()->append(fn_tt, "level mismatched of appending in Scope", ExLog::ERR);
		delete _t;
	}
	else
	{
		prop.push_back(ptok_u(_t));
		// do not delete _t
	}
}
