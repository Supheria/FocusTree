#ifndef _PARSE_TREE
#define _PARSE_TREE

#include "element.h"
#include "token.h"

class ParseTree
{
	const std::string* key; // set to nullptr when pass to build
	const char* op; // set to nullptr when pass to build
	Token* build;
	bool sarr; // use struct-array
	ParseTree* sub;
	ParseTree* from;
	size_t level;
public:
	ParseTree();
	// for sub-tree
	ParseTree(const std::string* _key, size_t _level);
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

#endif