#include "token_types.h"
#include "exception_log.h"

extern ErrorLog Errlog;
extern WarningLog Warnlog;

using namespace std;

const string FileName = "token_types";
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
Tag::Tag(pcValue _key, pcValue _op, pcValue _tag, const size_t& _lv)
	: Token(TAG, _vol_(_key, NLL_KEY), _lv),
	op(_vol_(_op, NLL_OP)),
	tg(_vol_(_tag, NLL_TAG))
{
}

Tag::~Tag()
{
}

const Volume& Tag::operat()
{
	return op;
}

const Volume& Tag::tag()
{
	return tg;
}

const tag_val& Tag::value()
{
	return val;
}

void Tag::append(pcValue _e)
{
	if (_e == nullptr ) { return; }
	val.push_back(Volume(_e));
}
//
//
//
// ValueArray
//
//
//
ValueArray::ValueArray(pcValue _key, const size_t& _lv)
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

void ValueArray::append(pcValue _vol)
{
	if (_vol == nullptr) { return; }
	val.back().push_back(Volume(_vol));
}

void ValueArray::append_new(pcValue _vol)
{
	if (_vol == nullptr) { return; }
	val.push_back(volume_list());
	val.back().push_back(Volume(_vol));
}
//
//
//
// TagArray
//
//
//
TagArray::TagArray(pcValue _key, const size_t& _lv)
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

void TagArray::append(pcValue _vol)
{
	if (_vol == nullptr) { return; }
	val.back().back().second.push_back(Volume(_vol));
}

void TagArray::append_tag(pcValue _vol)
{
	if (_vol == nullptr) { return; }
	val.back().push_back(tag_pair(Volume(_vol), tag_val()));
}

void TagArray::append_new(pcValue _vol)
{
	if (_vol == nullptr) { return; }
	val.push_back(tag_pair_list());
	val.back().push_back(tag_pair(Volume(_vol), tag_val()));
}
//
//
//
// Scope
//
//
//
Scope::Scope(pcValue _key, const size_t& _lv)
	: Token(SCOPE, _vol_(_key, NLL_KEY), _lv)
{
}

Scope::~Scope()
{
	for (auto t : prop)
	{
		delete t;
	}
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
		Errlog(FileName, "level mismatched of appending in Scope");
	}
	else
	{
		prop.push_back(_t);
		return; // do not delete _t
	}

	delete _t;
}
