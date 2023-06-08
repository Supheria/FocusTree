#include "token_types.h"
#include "use_ex_log.h"

using namespace std;

const char* fn_tt = "token_types";
const Value NLL_NAME = "NULL_NAME";
const Value NLL_OP = "NULL_OPERATOR";
const Value NLL_TAG = "NULL_TAG";

static Value _val_(pcval_u& _v, const Value null_val)
{
	if (_v != nullptr)
	{
		return _v.release();
	}
	else
	{
		return null_val;
	}
}
//
//
//
// Token
//
//
//
pcval_u& Token::name()
{
	return na;
}

const token_types& Token::type() const
{
	return tp;
}

const size_t& Token::level() const
{
	return lv;
}

Token::Token(pcval_u& _name, const size_t& _level) :
	na(_val_(_name, NLL_NAME)),
	tp(TOKEN),
	lv(_level)
{
}
//
//
//
// Tagged
//
//
//
pcval_u& Tagged::name()
{
	return na;
}

const token_types& Tagged::type() const
{
	return tp;
}

const size_t& Tagged::level() const
{
	return lv;
}

pcval_u& Tagged::operat()
{
	return op;
}

pcval_u& Tagged::tag()
{
	return tg;
}

tagged_val& Tagged::value()
{
	return val;
}

Tagged::Tagged(pcval_u& _name, pcval_u& _operat, pcval_u& _tag, const size_t& _level) :
	na(_val_(_name, NLL_NAME)),
	tp(TAGGED),
	lv(_level),
	op(_val_(_operat, NLL_OP)),
	tg(_val_(_tag, NLL_TAG))
{
}

void Tagged::append(pcval_u& _val)
{
	if (_val == nullptr) { return; }
	val.push_back(move(_val));
}
//
//
//
// ValueArray
//
//
//
pcval_u& ValueArray::name()
{
	return na;
}

const token_types& ValueArray::type() const
{
	return tp;
}

const size_t& ValueArray::level() const
{
	return lv;
}

arr_v& ValueArray::value()
{
	return val;
}

ValueArray::ValueArray(pcval_u& _name, const size_t& _level) :
	na(_val_(_name, NLL_NAME)),
	tp(VAL_ARRAY),
	lv(_level)
{
}

void ValueArray::append(pcval_u& _val)
{
	if (_val == nullptr) { return; }
	val.back().push_back(move(_val));
}

void ValueArray::append_new(pcval_u& _val)
{
	if (_val == nullptr) { return; }
	val.push_back(value_list());
	val.back().push_back(move(_val));
}
//
//
//
// TagArray
//
//
//
pcval_u& TagArray::name()
{
	return na;
}

const token_types& TagArray::type() const
{
	return tp;
}

const size_t& TagArray::level() const
{
	return lv;
}

arr_t& TagArray::value()
{
	return val;
}

TagArray::TagArray(pcval_u& _name, const size_t& _level) :
	na(_val_(_name, NLL_NAME)),
	tp(TAG_ARRAY),
	lv(_level)
{
}

void TagArray::append(pcval_u& _val)
{
	if (_val == nullptr) { return; }
	val.back().back().second.push_back(move(_val));
}

void TagArray::append_tag(pcval_u& _val)
{
	if (_val == nullptr) { return; }
	val.back().push_back(tag_pair(move(_val), tagged_val()));
}

void TagArray::append_new(pcval_u& _val)
{
	if (_val == nullptr) { return; }
	val.push_back(tag_pair_list());
	val.back().push_back(tag_pair(move(_val), tagged_val()));
}
//
//
//
// Scope
//
//
//
pcval_u& Scope::name()
{
	return na;
}

const token_types& Scope::type() const
{
	return tp;
}

const size_t& Scope::level() const
{
	return lv;
}

token_list& Scope::property()
{
	return prop;
}

Scope::Scope(pcval_u& _name, const size_t& _level) :
	na(_val_(_name, NLL_NAME)),
	tp(SCOPE),
	lv(_level)
{
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
