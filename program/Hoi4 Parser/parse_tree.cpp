#include "parse_tree.h"
#include "token_types.h"
#include "exception_log.h"

const char ParseTree::openb = '{';
const char ParseTree::closb = '}';
const char ParseTree::equal = '=';
const char ParseTree::gter = '>';
const char ParseTree::less = '<';

extern ErrorLog Errlog;

using namespace std;


const string FileName = "parse_tree";
#define UNKNOWN_ERROR Errlog(FileName, format( "unknown error at line({}), column({})", (*p_e)->line(), (*p_e)->column()))
#define UNEXPECTED_KEY Errlog(FileName, format( "unexpected key at line({}), column({})", (*p_e)->line(), (*p_e)->column()))
#define UNEXPECTED_OPERATOR Errlog(FileName, format( "unexpected operator at line({}), column({})", (*p_e)->line(), (*p_e)->column()))
#define UNEXPECTED_VALUE Errlog(FileName, format( "unexpected value at line({}), column({})", (*p_e)->line(), (*p_e)->column()))
#define SYNTAX_ERROR Errlog(FileName, format( "wrong syntax at line({}), column({})", (*p_e)->line(), (*p_e)->column()))

ParseTree::ParseTree() :
	key(nullptr),
	op(nullptr),
	value(nullptr),
	arr_key(nullptr),
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
ParseTree::ParseTree(const ParseTree* _from, pVolume _key, pVolume _op, const size_t& _level) :
	key(_key),
	op(_op),
	value(nullptr),
	arr_key(nullptr),
	build(nullptr),
	from(_from),
	curr_sub(nullptr),
	sarr(false),
	step(OP),
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
	delete arr_key;
	if (!lose_built) { delete build; }
	// delete curr_sub; // do not delete here, it will delete in step SUB, or by tokenizer when parse interrupted in sub-tree
	// delete from; // do not delete here, it will delete by tokenizer or from's from
}

const ParseTree* ParseTree::parse(pElement* const p_e) const
{
	const char& ch = (*p_e)->head();
	if (step & ARR)
	{
		//
		// 11
		//
		if (step & OFF)
		{
			switch (ch)
			{
			case openb:
				step = ARR;
				dispose(p_e);
				return this;
			case closb:
				dispose(p_e);
				done();
				return from;
			default:
				SYNTAX_ERROR;
				dispose(p_e);
				return from;
			}
		}
		//
		// 12
		//
		else if (step & KEY)
		{
			switch (ch)
			{
			case openb:
			case gter:
			case less:
				UNEXPECTED_VALUE;
				dispose(p_e);
				return from;
			case equal:
				step = ParseSteps(TAG_ARR);
				dispose(p_e);
				if (build == nullptr)
				{
					build = new TagArray(&key, level);
				}
				return this;
			case closb:
				step = ParseSteps(ARR | OFF);
				dispose(p_e);
				if (build == nullptr)
				{
					build = new ValueArray(&key, level);
				}
				return this;
			default:
				step = ParseSteps(VAL_ARR);
				if (build == nullptr)
				{
					build = new ValueArray(&key, level);
				}
				else
				{
					((Array*)build)->append_new(p_e);
				}
				return this;
			}
		}
		//
		// 10 - ARR
		//
		else
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
				step = ParseSteps(ARR | OFF);
				dispose(p_e);
				return this;
			default:
				step = ParseSteps(VAL_ARR);
				if (build == nullptr)
				{
					arr_key = new Volume(p_e);
				}
				return this;
			}
		}
	}
	//
	// 4
	//
	else if (step & VAL_ARR)
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
			step = ParseSteps(ARR | OFF);
			dispose(p_e);
			return this;
		default:
			step = ParseSteps(VAL_ARR);
			((ValueArray*)build)->append(p_e);
			return this;
		}
	}
	else if (step & TAG_ARR)
	{
		//
		// 5
		//
		if (step & KEY)
		{
			switch (ch)
			{
			case equal:
				step = ParseSteps(TAG_ARR);
				dispose(p_e);
				return this;
			default:
				SYNTAX_ERROR;
				dispose(p_e);
				return from;
			}
		}
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
				step = ParseSteps(TAG_ARR | OFF);
				dispose(p_e);
				return this;
			default:
				step = ParseSteps(TAG_ARR | ON);
				((TagArray*)build)->append(p_e);
				return this;
			}
		}
		//
		// 8
		//
		else if (step & OFF)
		{
			switch (ch)
			{
			case openb:
			case equal:
			case gter:
			case less:
				SYNTAX_ERROR;
				dispose(p_e);
				return from;
			case closb:
				step = ParseSteps(ARR | OFF);
				dispose(p_e);
				return this;
			default:
				step = ParseSteps(TAG_ARR | KEY);
				((TagArray*)build)->append_tag(p_e);
				return this;
			}
		}
		//
		// 9 - TAG_ARR
		//
		else
		{
			switch (ch)
			{
			case openb:
				step = ParseSteps(TAG_ARR | ON);
				dispose(p_e);
				return this;
			default:
				SYNTAX_ERROR;
				dispose(p_e);
				return from;
			}
		}
	}
	else if (step & OP)
	{
		//
		// 13
		//
		if (step & ON)
		{
			switch (ch)
			{
			case equal:
			case gter:
			case less:
				UNEXPECTED_VALUE;
				dispose(p_e);
				return from;
			case openb:
				step = ARR;
				dispose(p_e);
				return this;
			case closb:
				dispose(p_e);
				done();
				return from;
			default:
				step = ParseSteps(SUB | KEY);
				build = new Scope(&key, level);
				value = new Volume(p_e);
				return this;
			}
		}
		//
		// 16 - OP
		//
		else
		{
			switch (ch)
			{
			case closb:
			case equal:
			case gter:
			case less:
				UNEXPECTED_VALUE;
				dispose(p_e);
				return from;
			case openb:
				if (op->head() != equal)
				{
					UNEXPECTED_OPERATOR;
					dispose(p_e);
					return from;
				}
				else
				{
					step = ParseSteps(OP | ON);
					dispose(p_e);
					return this;
				}
			default:
				step = ParseSteps(VAL);
				value = new Volume(p_e);
				build = new Tag(&key, &op, &value, level);
				return this;
			}
		}
	}
	else if (step & SUB)
	{
		//
		// 17
		//
		if (step & KEY)
		{
			switch (ch)
			{
			case openb:
				UNEXPECTED_VALUE;
				dispose(p_e);
				return from;
			case closb:
				dispose(p_e);
				((Scope*)build)->append(new Token(&value, level + 1));
				done();
				return from;
			case equal:
			case gter:
			case less:
				step = SUB;
				delete curr_sub;
				curr_sub = new ParseTree(this, new Volume(&value), new Volume(p_e), level + 1);
				return curr_sub;
			default: 
				// step = ParseSteps(SUB | KEY);
				((Scope*)build)->append(new Token(&value, level + 1));
				value = new Volume(p_e);
				return this;
			}
		}
		//
		// 18 - SUB
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
				dispose(p_e);
				done();
				return from;
			default:
				step = ParseSteps(SUB | KEY);
				value = new Volume(p_e);
				return this;
			}
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
			step = OP;
			op = new Volume(p_e);
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
			step = TAG;
			dispose(p_e);
			return this;
		default:
			done();
			// dispose(p_e); // leave *p_e to next tree
			return from;
		}
	}
	//
	// 3
	//
	else if (step & TAG)
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
			dispose(p_e);
			done();
			return from;
		default:
			step = TAG;
			((Tag*)build)->append(p_e);
			return this;
		}
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
			step = KEY;
			key = new Volume(p_e);
			return this;
		}
	}
}

pToken ParseTree::once_get() const
{
	if (!lose_built)
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

void ParseTree::append(pToken _t) const
{
	((Scope*)build)->append(_t);
}

void ParseTree::dispose(pElement* p_e) const
{
	delete (*p_e);
	(*p_e) = nullptr;
}

void ParseTree::done() const
{
	fine = true;
	if (from != nullptr)
	{
		from->append(build);
		lose_built = true;
	}
}
