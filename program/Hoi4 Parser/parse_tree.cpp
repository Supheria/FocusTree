#include "parse_tree.h"
#include "token_types.h"

ParseTree::ParseTree() :
	key(nullptr),
	op(nullptr),
	build(nullptr),
	sub(nullptr),
	from(nullptr),
	sarr(false),
	step(Key),
	level(0)
{
}

// call for sub-tree
ParseTree::ParseTree(const std::string* _key, size_t _level)
	: key(key)
{
}

ParseTree::~ParseTree()
{
	// if (key != nullptr) { delete key; } // key and op are cache for build, so do not delete here but use fail_to_build() when failed
	// if (op != nullptr) { delete op; }
	// if (build != nullptr) { delete build; } // this shouldn't be deleted here since the memory may use for token map elsewhere
	if (sub != nullptr) { delete sub; }
	if (from != nullptr) { delete from; }
}

ParseTree* ParseTree::parse(const Element* _e)
{
	
	return nullptr;
}

void ParseTree::fail_to_build(const Element* _e)
{
	if (_e != nullptr)
	{
		//_e->del(); // make sure there is no value-pass of _e before calling fail_to_build
		delete _e;
	}

	if (build != nullptr) { delete build; } // only if parse failed will delete the memory 
	// and set the pointer give back to token map a nullptr,
	// also will call build->~Token() then delete build->key
// they will be passed to build or not when fail_to_build
	// set to nullptr when they pass to build
	if (key != nullptr) { delete key; }
	if (op != nullptr) { delete op; }
}

/// <summary>
/// give back to token map
/// </summary>
Token* ParseTree::get()
{
	return build;
}
