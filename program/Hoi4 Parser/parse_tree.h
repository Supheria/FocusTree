#ifndef _PARSE_TREE
#define _PARSE_TREE

#include "element.h"
#include "token.h"

class ParseTree
{
public:
	static const char  openb, closb, equal, gter, less, eof;
private:
	// Token will delete key and set it to nullptr when pass to it
						 // if will have sub-tree,
						 // need to set from_key to key->get() before key pass a to new Token
	mutable Volume* key; 
	mutable Volume* op; // Token delete op and set it to nullptr when pass to it
	mutable Volume* value; // same as above ^
	mutable Token* build;
	bool sarr; // use struct-array
	const ParseTree* from; // nullptr means to main-Tree or say root-Tree
	const std::string* from_key;
	mutable ParseTree* curr_sub;
	const size_t level;
	mutable bool fine;
public:
	ParseTree();
	// for sub-tree
	ParseTree(const ParseTree* _from, Volume* _key, const std::string* _from_key, size_t _level);
	~ParseTree();
	const ParseTree* parse(Element** p_e) const;
	void fail_to_build(Element** const p_e) const;
	/// if is building or fail to built will return nullptr
	Token* get() const;
private:
	// last step is on
	enum ParseSteps : size_t
	{
		NONE = 0,
		KEY = 0b1,
		OP = 0b1 << 1,
		VAL = 0b1 << 2,
		TAG = 0b1 << 3,
		ARR = 0b1 << 4,
		SUB = 0b1 << 5,
		ON = 0b1 << 6,
		OFF = 0b1 << 7
	} mutable step; 
};

#endif