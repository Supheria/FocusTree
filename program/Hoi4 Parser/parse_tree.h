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
	mutable pVolume arr; // same as above ^
	mutable pToken build;
	const ParseTree* const from; // nullptr means to main-Tree or say root-Tree
	mutable ParseTree* curr_sub;
	const size_t level;
	mutable bool lose_built;
public:
	ParseTree();
	// for sub-tree
	ParseTree(const ParseTree* _from, pVolume _key, pVolume _op, const size_t& _level);
	~ParseTree();
	// for tokenizer to use
						// can only get build on time,
						// when get will transfer ownership of build that 
						// won't delete in ~ParseTree()
	pToken once_get() const;
	// for tokenizer to use, test whether parse process has interrupted
	const ParseTree* get_from() const;
	// for sub-tree to use, if for root-tree should use once_get()
								// if sub-tree has done, 
								// will append its build to from-tree and lose ownership
	void append(pToken _t) const;
	const ParseTree* parse(pElement* const p_e) const;
private:
	const ParseTree* par_sub(pElement* const p_e) const;
	const ParseTree* par_arr(pElement* const p_e) const;
	const ParseTree* par_tag_arr(pElement* const p_e) const;
	const ParseTree* par_val_arr(pElement* const p_e) const;
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
		SUB = 0b1 << 5,
		ON = 0b1 << 6,
		OFF = 0b1 << 7
	} mutable step; 
};

#endif