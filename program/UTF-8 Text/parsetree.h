#ifndef _PARSE_TREE
#define _PARSE_TREE

#include "element.h"
#include "token.h"

class ParseTree
{
	const std::string* key;
	const char* op;
	Token* build;
	bool sarr; // use struct-array
	ParseTree* sub;
	ParseTree* from;
public:
	ParseTree();
	~ParseTree();
	ParseTree* parse(const Element* _e);
	void fail_to_build(const Element* _e);
	Token* get();
private:
	enum ParseSteps
	{
		Key = 0b1,
		Op = 0b1 << 1,
		Val = 0b1 << 2,
		Tag = 0b1 << 3,
		Arr = 0b1 << 4,
		Sub = 0b1 << 5,
		On = 0b1 << 6,
		Off = 0b1 << 7
	} step;
};
/*
enum ParseSteps
{
	Key,
	Op,
	//
	// value, tag
	//
	Val,
	Tag_on,
	Tag_val,
	Tag_off,
	//
	// sub, array
	//
	Op_on,
	Op_off,
	//
	// array
	//
	Arr_on,
	Arr_off,
	Arr_key,
	Arr_op,
	Arr_opon,
	Arr_opval,
	Arr_opoff,
	//
	// sub
	//
	Op_val,
	Sub_op
};
*/
#endif
