#include "parsetree.h"

ParseTree::ParseTree() :
	key(nullptr),
	op('='),
	build(nullptr),
	sub(nullptr),
	from(nullptr),
	sarr(false),
	step(Key)
{
}

ParseTree::~ParseTree()
{
	if (key != nullptr) { delete key; }
	// if (build != nullptr) { delete build; } // this shouldn't be deleted here since the memory may use for token map elsewhere
	if (sub != nullptr) { delete sub; }
	if (from != nullptr) { delete from; }
}

ParseTree* ParseTree::parse(const Element* e)
{
	return nullptr;
}

void ParseTree::check_eof()
{
}

void ParseTree::fail_to_build()
{
	if (build != nullptr) { delete build; } // only if parse failed will delete the memory 
											// and set the pointer give back to token map a nullptr,
											// also will call build->~Token() then delete build->key
	build = nullptr;
}

/// <summary>
/// give back to token map
/// </summary>
Token* ParseTree::get()
{
	return build;
}
