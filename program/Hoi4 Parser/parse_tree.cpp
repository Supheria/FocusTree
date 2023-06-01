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
#define UNEXPECTED_VALUE ErrLog(FileName, format( "unexpected value at line({}), column({})", (*p_e)->line(), (*p_e)->column()))


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
	//
	// 3
	//
	if (step & TAG)
	{
		switch (ch)
		{
		case openb:
		case equal:
		case gter:
		case less:
			UNEXPECTED_VALUE;
			dispose(p_e);
			return from;
		case closb:
			fine = true;
			dispose(p_e);
			return from;
		default:
			((Tag*)build)->append(p_e);
			step = ParseSteps(TAG);
			return this;
		}
	}
	else if (step & ARR)
	{
		step = ParseSteps(step ^ ARR);
		if (step & OP)
		{
			step = ParseSteps(step ^ OP);
			//
			// 6
			//
			if (step & ON)
			{
				switch (ch)
				{
				case openb:
				case equal:
				case gter:
				case less:
					UNEXPECTED_VALUE;
					dispose(p_e);
					return from;
				case closb:
					step = ParseSteps(ARR | OP | OFF);
					dispose(p_e);
					return this;
				default:
					((Array*)build)->append(p_e);
					break;
				}
			}
			//
			// 7
			//
			else if (step & VAL)
			{

			}
			//
			// 8
			//
			else if (step & OFF)
			{

			}
			//
			// 9 - Arr | Op
			//
			else
			{

			}
		}
		else
		{
			switch (step)
			{
				//
				// 10
				//
			case ON:
				break;
				//
				// 11
				//
			case OFF:
				break;
				//
				// 12
				//
			case KEY:
				break;
			default:
				UNKNOWN_ERROR;
				dispose(p_e);
				return from;
			}
		}
	}
	else if (step & OP)
	{
		switch (step ^ OP)
		{
			//
			// 13
			//
		case ON:
			break;
			//
			// 14
			//
		case OFF:
			break;
			//
			// 15
			//
		case VAL:
			break;
			//
			// 16 - OP
			//
		default:
			break;
		}
	}
	//
	// 1
	//
	else if (step & KEY)
	{
		switch (ch)
		{
		case equal:
		case gter:
		case less:
			op = new Volume(p_e);
			step = OP;
			return this;
		default:
			UNEXPECTED_OPERATOR;
			dispose(p_e);
			return from;
		}
	}
	//
	// 2
	//
	else if (step & VAL)
	{
		switch (ch)
		{
		case openb:
			build = new Tag(&key, &value, level);
			step = ParseSteps(TAG);
			dispose(p_e);
			return this;
		default:
			fine = true;
			build = new ValueKey(&key, &value, &op, level);
			// dispose(p_e); // leave *p_e to next tree
			return from;
		}
	}
	//
	// 16
	//
	else if (step & SUB)
	{
		// need to delete curr_sub
		// this step means sub parsed already
		return this;
	} 
	//
	// 0 - None
	//
	else
	{
		switch (ch)
		{
		case openb:
		case closb:
		case equal:
		case gter:
		case less:
			UNEXPECTED_KEY;
			dispose(p_e);
			return from;
		default:
			key = new Volume(p_e);
			step = KEY;
			return this;
		}
	}
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

void ParseTree::dispose(pElement* p_e) const
{
	delete (*p_e);
	(*p_e) = nullptr;
}
