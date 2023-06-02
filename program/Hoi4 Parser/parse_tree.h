#ifndef _PARSE_TREE
#define _PARSE_TREE

#include "element.h"
#include "token.h"

class ParseTree
{
public:
	static const char  openb, closb, equal, gter, less;
private:
	// Token will delete key and set it to nullptr when pass to it
						 // if will have sub-tree,
						 // need to set from_key to key->get() before key pass a to new Token
	mutable pVolume key;
	mutable pVolume op; // Token delete op and set it to nullptr when pass to it
	mutable pVolume value; // same as above ^
	mutable pVolume arr_key; // same as above ^
	mutable pToken build;
	bool sarr; // use struct-array
	const ParseTree* const from; // nullptr means to main-Tree or say root-Tree
	mutable ParseTree* curr_sub;
	const size_t level;
	mutable bool fine;
	mutable bool lose_built;
public:
	ParseTree();
	// for sub-tree
	ParseTree(const ParseTree* _from, pVolume _key, pVolume _op, const size_t& _level);
	~ParseTree();
	const ParseTree* parse(pElement* p_e) const;
	// for tokenizer to use
						// only can get build on time,
						// when get will transfer ownership of build that won't 
						// delete the pToken in ~ParseTree()
	pToken once_get() const; 
	// for tokenizer to use, test whether parse process has interrupted
	const ParseTree* get_from() const;
	// for sub-tree to use, for root-tree should use get()
								// if sub-tree has done, will append its build and lose ownership,
								// otherwise won't pass anything
	void append(pToken _t) const;
private:
	void dispose(pElement* p_e) const;
	void done() const;
private:
	enum ParseSteps : size_t
	{
		NONE = 0,
		KEY = 0b1,
		OP = 0b1 << 1,
		VAL = 0b1 << 2,
		TAG = 0b1 << 3,
		ARR = 0b1 << 4,
		VAL_ARR = 0b1 << 5,
		TAG_ARR = 0b1 << 6,
		SUB = 0b1 << 7,
		ON = 0b1 << 8,
		OFF = 0b1 << 9
	} mutable step; 
};

#endif