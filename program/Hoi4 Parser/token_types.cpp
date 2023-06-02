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
Tag::Tag(pVolume* const p_key, pVolume* const p_op, pVolume* const p_tag, const size_t& _lv)
	: Token(TAG, _vol_(p_key, NLL_KEY), _lv),
	op(_vol_(p_op, NLL_OP)),
	tg(_vol_(p_tag, NLL_TAG))
{
}

Tag::~Tag()
{
	delete op;
	delete tg;
	for (auto v : val)
	{
		delete v;
	}
}

const Volume& Tag::operat()
{
	return *op;
}

const Volume& Tag::tag()
{
	return *tg;
}

const tag_val& Tag::value()
{
	return val;
}

void Tag::append(pElement* const p_e)
{
	if (p_e == nullptr || (*p_e) == nullptr) { return; }
	val.push_back(new Volume((p_e)));
}
//
//
//
// Array
//
//
//
Array::Array(const T& _t, pVolume* p_key, const size_t& _lv)
	: Token(_t, _vol_(p_key, NLL_KEY), _lv)
{
}

Array::~Array()
{
}
//
//
//
// ValueArray
//
//
//
ValueArray::ValueArray(pVolume* p_key, const size_t& _lv)
	: Array(VAL_ARRAY, p_key, _lv)
{
}

ValueArray::~ValueArray()
{
	for (volume_list vlst : val)
	{
		for (auto v : vlst)
		{
			delete v;
		}
	}
}

const arr_v& ValueArray::value()
{
	return val;
}

void ValueArray::append(pElement* const p_e)
{
	if (p_e == nullptr || (*p_e) == nullptr) { return; }
	val.back().push_back(new Volume((p_e)));
}

void ValueArray::append_new(pElement* const p_e)
{
	if (p_e == nullptr || (*p_e) == nullptr) { return; }
	volume_list vlst = { new Volume((p_e)) };
	val.push_back(vlst);
}
//
//
//
// TagArray
//
//
//
TagArray::TagArray(pVolume* p_key, const size_t& _lv)
	: Array(TAG_ARRAY, p_key, _lv)
{
}

TagArray::~TagArray()
{
	for (tag_pair_list tplst : val)
	{
		for (tag_pair tpr : tplst)
		{
			delete tpr.first;
			for (auto v : tpr.second)
			{
				delete v;
			}
		}
	}
}

const arr_t& TagArray::value()
{
	return val;
}

void TagArray::append(pElement* const p_e)
{
	if (p_e == nullptr || (*p_e) == nullptr) { return; }
	val.back().back().second.push_back(new Volume(p_e));
}

void TagArray::append_tag(pElement* const p_e)
{
	if (p_e == nullptr || (*p_e) == nullptr) { return; }
	tag_val tv;
	tag_pair tpr = { new Volume(p_e), tv };
	val.back().push_back(tpr);
	val.back().back().second.push_back(new Volume(p_e));
}

void TagArray::append_new(pElement* const p_e)
{
	tag_val tv;
	tag_pair tpr = { new Volume(p_e), tv };
	tag_pair_list tplst = { tpr };
	val.push_back(tplst);
	val.back().back().second.push_back(new Volume(p_e));
}
//
//
//
// Scope
//
//
//
Scope::Scope(pVolume* p_key, const size_t& _lv)
	: Token(SCOPE, _vol_(p_key, NLL_KEY), _lv)
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
