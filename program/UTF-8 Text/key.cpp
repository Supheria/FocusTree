#include "key.h"

extern ErrorLog ErrLog;
extern WarningLog WarnLog;

const char* FileName = "key.cpp";
const char* NLL_VOL = "NULL_VOLUME";
const char* NLL_KEY = "NULL_KEY";
const char* NLL_OP = "NULL_OPERATOR";
const char* NLL_TAG = "NULL_TAG";

using namespace std;

#pragma region ==== Value ====

Value::Value(const Element* volume, Token* from, const size_t& _lv)
	: Token(VALUE, _e_val(volume, NLL_VOL), from, _lv)
{
}

void Value::append(Token*)
{
	ErrLog(FileName, "type Value is not appendable");
}

#pragma endregion



#pragma region ==== ValueKey ====

ValueKey::ValueKey(const Element* _key, const Token* _val, const Element* _op, const Token* _fr, const size_t& _lv)
	: Token(VAL_KEY, _e_val(_key, NLL_KEY), _fr, _lv),
	op(_e_val(_op, NLL_OP)),
	val(_val),
	lose_op(false),
	lose_val(false)
{
}

ValueKey::~ValueKey()
{
	if (!lose_op) { delete[] op; }
	if (!lose_val) { delete val; }
}

const char& ValueKey::operat() const 
{ 
	return *op;
}

const Token& ValueKey::value() const
{
	return *val;
}

const char* ValueKey::get_op() const
{
	lose_op = true;
	return nullptr;
}

const Token* ValueKey::get_val() const
{
	lose_val = true;
	return val;
}

void ValueKey::append(const Token* _t)
{
	if (_t == nullptr) { return; }
	//
	// replacement
	//
	if (token() == _t->token() && type() == _t->type())
	{
		if (level() == _t->level())
		{
			delete op;
			op = ((ValueKey*)_t)->get_op();
			delete val;
			val = ((ValueKey*)_t)->get_val();
			WarnLog(FileName, "value of ValueKey was replaced");
		}
		else { ErrLog(FileName, "mismatched level in replacement of ValueKey"); }
	}
	else if (_t->type() == T::VALUE)
	{
		ErrLog(FileName, "assignment to ValueKey is out constructor");
	}
	else { ErrLog(FileName, "type ValueKey appends wrong type"); }

	delete _t;
}

#pragma endregion



#pragma region Tag

void Tag::del_val()
{
	for (auto el : *val)
	{
		if (el != nullptr) { delete el; }
	}
	delete val;
}

Tag::Tag(const Element* _key, const Element* _tag, const Token* _fr, const size_t _lv)
	: Token(TAG, _e_val(_key, NLL_KEY), _fr, _lv),
	tg(_e_val(_tag, NLL_TAG)),
	val(new tag_val)
{
}

Tag::~Tag()
{
	if (tg != nullptr) { delete tg; }
	del_val();
}

tag_val* Tag::value()
{
	return val;
}

void Tag::append(const Token* _t)
{
	if (_t == nullptr) { return; }
	//
	// replacement
	//
	if (token() == _t->token() && type() == _t->type())
	{
		if (level() == _t->level())
		{
			del_val();
			val = ((Tag*)_t)->value();
			WarnLog(FileName, "replacement occurs in value of Tag");
		}
		else { ErrLog(FileName, "level mismatch in Tag replacement"); }
	}
	//
	// assignment
	//
	else if (_t->type() == VALUE && _t->from()->token() == token())
	{
		if (_t->level() == level() + 1)
		{
			val->push_back(_t);
		}
		else { ErrLog(FileName, "level mismatch in Tag assignment"); }
	}
	else { ErrLog(FileName, "error type of appending to Tag "); }

	delete _t;
}

#pragma endregion



#pragma region Array

Array::Array(const Element* _key, const bool sarr, const Token* _fr, const size_t _lv)
	: Token(sarr ? S_ARRAY : VAL_ARRAY, _e_val(_key, NLL_KEY), _fr, _lv),
	val(new arr_val),
	addnew(false)
{
}

Array::~Array()
{
	for (std::vector<const Token*> arr : *val)
	{
		for (auto elm : arr)
		{
			if (elm != nullptr) { delete elm; }
		}
	}
	delete val;
}

arr_val* Array::value()
{
	return val;
}

void Array::append(const Token* _t)
{
	if (_t == nullptr) { return; }
	//
	// combination
	//
	if (token() == _t->token() && type() == _t->type())
	{
		if (level() == _t->level())
		{
			for (std::vector<const Token*> arr : *((Array*)_t)->value())
			{
				val->push_back(arr);
			}
			WarnLog(FileName, "combination occurs in value of Tag");
		}
		else { ErrLog(FileName, "level mismatch in Array combination"); }
	}
	//
	// assignment
	//
	else if (_t->from()->token() == token())
	{
		if (_t->level() == level() + 1)
		{
			if (
				(type() == VAL_ARRAY && _t->type() == VALUE) ||
				(type() == S_ARRAY && _t->type() == SCOPE)
				)
			{
				if (addnew)
				{
					std::vector<const Token*> _arr({ _t });
					val->push_back(_arr);
					addnew = false;
				}
				else
				{
					val->back().push_back(_t);
				}
			}
			else { ErrLog(FileName, "array type mismatch in Array assignment"); }
		}
		else { ErrLog(FileName, "level mismatch in Array assignment"); }
	}
	else { ErrLog(FileName, "error type of appending to Array "); }

	delete _t;
}

void Array::set_new()
{
	addnew = true;
}

#pragma endregion


