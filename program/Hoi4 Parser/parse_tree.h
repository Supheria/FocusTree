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
	ParseTree(const ParseTree* _from, pVolume _key, size_t _level);
	~ParseTree();
	const ParseTree* parse(pElement* p_e) const;
	// if is building or fail to built will return nullptr,
						// otherwise will transfer ownership of build that won't 
						// delete the pToken in ~ParseTree()
	pToken get() const;
	const ParseTree* get_from() const;
private:
	void dispose(pElement* p_e) const;
private:
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