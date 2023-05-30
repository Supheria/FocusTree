#include "token_types.h"
#include "exception_log.h"

extern ErrorLog ErrLog;
extern WarningLog WarnLog;

using namespace std;

const string FileName = "key.cpp";
const string NLL_VAL = "NULL_VALUE";
const string NLL_KEY = "NULL_KEY";
const string NLL_OP = "NULL_OPERATOR";
const string NLL_TAG = "NULL_TAG";
//
//
//
// ValueKey
//
//
//
ValueKey::ValueKey(const Element* _key, const Element* _val, const Element* _op, const std::string* _fr, const size_t& _lv)
	: Token(VAL_KEY, _e_vol(_key, NLL_KEY), _fr, _lv),
	op(_e_vol(_op, NLL_OP)),
	val(_e_vol(_val, NLL_VAL))
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

void ValueKey::mix(Token* _t)
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
Tag::Tag(const Element* _key, const Element* _tag, const std::string* _fr, const size_t& _lv)
	: Token(TAG, _e_vol(_key, NLL_KEY), _fr, _lv),
	tg(_e_vol(_tag, NLL_TAG))
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

void Tag::mix(Token* _t)
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

void Tag::append(const Volume* _vol)
{
	if (_vol == nullptr) { return; }

	val.push_back(new Volume(_vol));

	delete _vol;
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
// Array
//
//
//
Array::Array(const Element* _key, const bool sarr, const std::string* _fr, const size_t& _lv)
	:Token(sarr ? S_ARRAY : VAL_ARRAY, _e_vol(_key, NLL_KEY), _fr, _lv),
	addnew(false)
{
}

const arr_val& Array::value()
{
	return val;
}

void Array::set_new()
{
	addnew = true;
}

void Array::mix(Token* _t)
{
	if (_t == nullptr) { return; }
	//
	// combination
	//
	if (token() != _t->token())
	{
		ErrLog(FileName, "key-name mismatched of combination in Array");
	}
	else if (type() != _t->type())
	{
		ErrLog(FileName, "type mismatched of combination in Array");
	}
	else if (level() != _t->level())
	{
		ErrLog(FileName, "level mismatched of combination in Array");
	}
	else
	{
		for (list<const Volume*> _arr : val)
		{
			list<const Volume*> arr;
			for (auto it : _arr)
			{
				arr.push_back(new Volume(it));
			}
			val.push_back(arr);
		}
		WarnLog(FileName, "combination in Array");
	}

	delete _t;
}

void Array::append(const Volume* _vol)
{
	if (_vol == nullptr) { return; }
	if (addnew)
	{
		list<const Volume*> arr = { new Volume(_vol) };
		val.push_back(arr);
		addnew = false;
	}
	else
	{
		val.back().push_back(new Volume(_vol));
	}

	delete _vol;
}

void Array::del_extend()
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
Scope::Scope(const Element* _key, const std::string* _fr, const size_t& _lv)
	: Token(SCOPE, _e_vol(_key, NLL_KEY), _fr, _lv)
{
}

const tok_map& Scope::property()
{
	return prop;
}

void Scope::mix(Token* _t)
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
			const string* key = &(it->second->token().volumn());
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

void Scope::append(Token* _t)
{
	if (_t == nullptr) { return; }
	//
	// appendage
	//
	if (_t->level() != level() + 1)
	{
		ErrLog(FileName, "level mismatched of appending in Scope");
	}
	else if (_t->from() != token().volumn())
	{
		ErrLog(FileName, "from-key-name mismatched of appending in Scope");
	}
	else
	{
		const string* key = &(_t->token().volumn());
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
