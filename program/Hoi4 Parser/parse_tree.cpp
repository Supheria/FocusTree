#include "parse_tree.h"
#include "token_types.h"
#include "exception_log.h"
#include <format>

const char ParseTree::openb = '{';
const char ParseTree::closb = '}';
const char ParseTree::equal = '=';
const char ParseTree::gter = '>';
const char ParseTree::less = '<';
const char ParseTree::eof = (char)-1;

extern ErrorLog ErrLog;
extern WarningLog WarnLog;

using namespace std;


const string FileName = "parse_tree";
#define UNKNOWN_ERROR ErrLog(FileName, format( "unknown error at line({}), column({})", (*p_e)->line(), (*p_e)->column()))
#define UNEXPECTED_KEY ErrLog(FileName, format( "unexpected key at line({}), column({})", (*p_e)->line(), (*p_e)->column()))
#define UNEXPECTED_OPERATOR ErrLog(FileName, format( "unexpected operator at line({}), column({})", (*p_e)->line(), (*p_e)->column()))


ParseTree::ParseTree() :
	key(nullptr),
	op(nullptr),
	value(nullptr),
	build(nullptr),
	from(nullptr),
	from_key(nullptr),
	curr_sub(nullptr),
	sarr(false),
	step(NONE),
	level(0),
	fine(false)
{
}

// call for sub-tree
ParseTree::ParseTree(const ParseTree* _from, Volume* _key, const string* _from_key, const size_t _level) :
	key(_key),
	op(nullptr),
	value(nullptr),
	build(nullptr),
	from(_from),
	from_key(_from_key),
	curr_sub(nullptr),
	sarr(false),
	step(KEY),
	level(_level),
	fine(false)
{
}

ParseTree::~ParseTree()
{
	// if (key != nullptr) { delete key; } // key and op are cache for build, so do not delete here but use fail_to_build() when failed
	// if (op != nullptr) { delete op; }
	// if (build != nullptr) { delete build; } // this shouldn't be deleted here since the memory may use for token map elsewhere
	// if (from != nullptr) { delete from; } // do not delete here, it will delete by tokenizer
}

const ParseTree* ParseTree::parse(Element** const p_e) const
{
	const char& ch = (*p_e)->head();
	if (step & TAG)
	{
		switch (step ^ TAG)
		{
		case ON: // 3
			break;
		case VAL: // 4
			break;
		case OFF: // 5
			break;
		default:
			UNKNOWN_ERROR;
			fail_to_build(p_e);
			return from;
		}
	}
	else if (step & ARR)
	{
		step = ParseSteps(step ^ ARR);
		if (step & OP)
		{
			switch (step ^ OP)
			{
			case ON: // 6
				break;
			case VAL: // 7
				break;
			case OFF: // 8
				break;
			default: // 9 - Arr | Op
				break;
			}
		}
		else
		{
			switch (step)
			{
			case ON: // 10
				break;
			case OFF: // 11
				break;
			case KEY: // 12
				break;
			default:
				UNKNOWN_ERROR;
				fail_to_build(p_e);
				return from;
			}
		}
	}
	else if (step & OP)
	{
		switch (step ^ OP)
		{
		case ON: // 13
			break;
		case OFF: // 14
			break;
		case VAL: // 15
			break;
		default: // 16 - OP
			break;
		}
	}
	else
	{
		switch (step)
		{
		case KEY: // 1
			switch (ch)
			{
			case equal:
			case gter:
			case less:
				op = new Volume(p_e);
				step = OP;
				break;
			default:
				UNEXPECTED_OPERATOR;
				fail_to_build(p_e);
				return from;
			}
			break;
		case VAL: // 2
			if (ch == openb)
			{
				delete (*p_e);
				(*p_e) = nullptr;
				step = ParseSteps(TAG | ON);
			}
			else
			{
				fine = true;
				build = new ValueKey(&key, &value, &op, from_key, level);
				return from;
			}
			break;
		case SUB: // 16
			// need to delete curr_sub
			// this step means sub parsed already
			break;
		default: // 0 - None
			switch (ch)
			{
			case openb:
			case closb:
			case equal:
			case gter:
			case less:
			case eof:
				UNEXPECTED_KEY;
				fail_to_build(p_e);
				return from;
			default:
				key = new Volume(p_e);
				step = KEY;
				break;
			}
			break;
		}
	}

	return this;
}

void ParseTree::fail_to_build(Element** const p_e) const
{
	//_e->del(); // make sure there is no value-pass of _e before calling fail_to_build
	delete (*p_e);
	(*p_e) = nullptr; // setting to null means to the Element has been used

	delete build; // only if parse failed will delete the memory 
	// and set the pointer give back to token map a nullptr,
	// also will call build->~Token() then delete build->key
// they will be passed to build or not when fail_to_build
	// set to nullptr when they pass to build
	delete key;
	delete op;
	delete value;
}

Token* ParseTree::get() const
{
	return fine ? build : nullptr;
}
