#include "parse_tree.h"
#include "token_types.h"
#include "exception_log.h"
#include <format>

const char ParseTree::openb = '{';
const char ParseTree::closb = '}';
const char ParseTree::equal = '=';
const char ParseTree::gter = '>';
const char ParseTree::less = '<';

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
	curr_sub(nullptr),
	sarr(false),
	step(NONE),
	level(0),
	fine(false),
	lose_built(false)
{
}

// call for sub-tree
ParseTree::ParseTree(const ParseTree* _from, pVolume _key, const size_t _level) :
	key(_key),
	op(nullptr),
	value(nullptr),
	build(nullptr),
	from(_from),
	curr_sub(nullptr),
	sarr(false),
	step(KEY),
	level(_level),
	fine(false),
	lose_built(false)
{
}

ParseTree::~ParseTree()
{
	delete key;
	delete op;
	delete value;
	if (!lose_built) { delete build; }
	// delete curr_sub; // do not delete here, it will delete in step SUB, or by tokenizer when parse interrupted in sub-tree
	// delete from; // do not delete here, it will delete by tokenizer or from's from
}

const ParseTree* ParseTree::parse(pElement* const p_e) const
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
			delete (*p_e);
			(*p_e) = nullptr;
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
				delete (*p_e);
				(*p_e) = nullptr;
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
				delete (*p_e);
				(*p_e) = nullptr;
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
				build = new ValueKey(&key, &value, &op, level);
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
				UNEXPECTED_KEY;
				delete (*p_e);
				(*p_e) = nullptr;
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

pToken ParseTree::get() const
{
	if (fine)
	{
		lose_built = true;
		return build;
	}
	else
	{
		return nullptr;
	}
}

const ParseTree* ParseTree::get_from() const
{
	return from;
}
