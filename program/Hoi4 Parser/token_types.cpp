#include "token_types.h"
#include "exception_log.h"

extern ErrorLog ErrLog;
extern WarningLog WarnLog;

using namespace std;

const string FileName = "token_types";
const Value NLL_VAL = "NULL_VALUE";
const Value NLL_KEY = "NULL_KEY";
const Value NLL_OP = "NULL_OPERATOR";
const Value NLL_TAG = "NULL_TAG";
//
//
//
// ValueKey
//
//
//
ValueKey::ValueKey(pVolume* const p_key, pVolume* const p_val, pVolume* const p_op, const size_t& _lv)
	: Token(VAL_KEY, _vol_(p_key, NLL_KEY), _lv),
	op(_vol_(p_op, NLL_OP)),
	val(_vol_(p_val, NLL_VAL))
{
}

const Volume& ValueKey::operat()
{
	return *op;
}

const Volume& ValueKey::value()
{
	return *val;
}

void ValueKey::mix(pToken _t)
{
	if (_t == nullptr) { return; }
	//
	// replacement
	//
	if (token() != _t->token())
	{
		ErrLog(FileName, "key-name mismatched of replacement in ValueKey");
	}
	else if (type() != _t->type())
	{
		ErrLog(FileName, "type mismatched of replacement in ValueKey");
	}
	else if (level() != _t->level())
	{
		ErrLog(FileName, "level mismatched of replacement in ValueKey");
	}
	else
	{
		delete op;
		op = new Volume(((ValueKey*)_t)->operat());
		delete val;
		val = new Volume(((ValueKey*)_t)->value());
		WarnLog(FileName, "replacement in ValueKey");
	}

	delete _t;
}

void ValueKey::del_extend()
{
	delete op;
	delete val;
}
//
//
//
// Tag
//
//
//
Tag::Tag(pVolume* const p_key, pVolume* const p_tag, const size_t& _lv)
	: Token(TAG, _vol_(p_key, NLL_KEY), _lv),
	tg(_vol_(p_tag, NLL_TAG))
{
}

const Volume& Tag::tag()
{
	return *tg;
}

const tag_val& Tag::value()
{
	return val;
}

void Tag::mix(pToken _t)
{
	if (_t == nullptr) { return; }
	//
	// replacement
	//
	if (token() != _t->token())
	{
		ErrLog(FileName, "key-name mismatched of replacement in ValueKey");
	}
	else if (type() != _t->type())
	{
		ErrLog(FileName, "type mismatched of replacement in ValueKey");
	}
	else if (level() != _t->level())
	{
		ErrLog(FileName, "level mismatched of replacement in ValueKey");
	}
	else
	{
		del_val();
		for (auto it : ((Tag*)_t)->value())
		{
			val.push_back(new Volume(it));
		}
		WarnLog(FileName, "replacement in Tag");
	}

	delete _t;
}

void Tag::append(pElement* const p_e)
{
	if (p_e == nullptr || (*p_e) == nullptr) { return; }

	val.push_back(new Volume((p_e)));
}

void Tag::del_val()
{
	auto it = val.begin();
	while (it != val.end())
	{
		delete (*it);
		val.erase(it++);
	}
}

void Tag::del_extend()
{
	delete tg;
	del_val();
}
//
//
//
// ValueArray
//
//
//
ValueArray::ValueArray(pVolume* p_key, const size_t& _lv)
	:Token(VAL_ARRAY, _vol_(p_key, NLL_KEY), _lv)
{
}

const arr_v& ValueArray::value()
{
	return val;
}

void ValueArray::mix(pToken _t)
{
	if (_t == nullptr) { return; }
	//
	// combination
	//
	if (token() != _t->token())
	{
		ErrLog(FileName, "key-name mismatched of combination in ValueArray");
	}
	else if (type() != _t->type())
	{
		ErrLog(FileName, "type mismatched of combination in ValueArray");
	}
	else if (level() != _t->level())
	{
		ErrLog(FileName, "level mismatched of combination in ValueArray");
	}
	else
	{
		for (list<pVolume> _ar : val)
		{
			list<pVolume> arr;
			for (auto it : _ar)
			{
				arr.push_back(new Volume(it));
			}
			val.push_back(arr);
		}
		WarnLog(FileName, "combination in ValueArray");
	}

	delete _t;
}

void ValueArray::append(pElement* const p_e)
{
	if (p_e == nullptr || (*p_e) == nullptr) { return; }
	val.back().push_back(new Volume((p_e)));
}

void ValueArray::append_new(pElement* const p_e)
{
	if (p_e == nullptr || (*p_e) == nullptr) { return; }
	list<pVolume> arr = { new Volume((p_e)) };
	val.push_back(arr);
}

void ValueArray::del_extend()
{
	auto arr = val.begin();
	while (arr != val.end())
	{
		auto it = arr->begin();
		while (it != arr->end())
		{
			delete (*it);
			arr->erase(it++);
		}
		val.erase(arr++);
	}
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

const tok_map& Scope::property()
{
	return prop;
}

void Scope::mix(pToken _t)
{
	if (_t == nullptr) { return; }
	if (token() != _t->token())
	{
		ErrLog(FileName, "key-name mismatched of combination in Scope");
	}
	else if (type() != _t->type())
	{
		ErrLog(FileName, "type mismatched of combination in Scope");
	}
	else if (level() != _t->level())
	{
		ErrLog(FileName, "level mismatched of combination in Scope");
	}
	else
	{
		auto it = ((Scope*)_t)->property().begin();
		while (it != prop.end())
		{
			pcValue key = &(it->second->token().volumn());
			if (prop.count(key)) // has the key
			{
				prop[key]->mix(it->second);
			}
			else { prop[key] = _t; }

			prop.erase(it++);
		}
	}

	delete _t;
}

void Scope::append(pToken _t)
{
	if (_t == nullptr) { return; }
	if (_t->level() != level() + 1)
	{
		ErrLog(FileName, "level mismatched of appending in Scope");
	}
	else
	{
		pcValue key = &(_t->token().volumn());
		if (prop.count(key)) // has the key
		{
			prop[key]->mix(_t);
		}
		else { prop[key] = _t; }

		return;
	}
	
	delete _t;
}

void Scope::del_extend()
{
	auto it = prop.begin();
	while (it != prop.end())
	{
		delete it->second;
		prop.erase(it++);
	}
}
