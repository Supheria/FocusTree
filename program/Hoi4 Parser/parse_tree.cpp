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
#define UNEXPECTED_ARRAY_TYPE Errlog(FileName, format( "unexpected array type at line({}), column({})", (*p_e)->line(), (*p_e)->column()))
#define ERROR_SYNTAX_ARRAY Errlog(FileName, format( "wrong array syntax at line({}), column({})", (*p_e)->line(), (*p_e)->column()))

ParseTree::ParseTree() :
	key(nullptr),
	op(nullptr),
	value(nullptr),
	arr(nullptr),
	build(nullptr),
	from(nullptr),
	curr_sub(nullptr),
	step(NONE),
	level(0),
	lose_built(false)
{
}

// call for sub-tree
ParseTree::ParseTree(const ParseTree* _from, pVolume _key, pVolume _op, const size_t& _level) :
	key(_key),
	op(_op),
	value(nullptr),
	arr(nullptr),
	build(nullptr),
	from(_from),
	curr_sub(nullptr),
	step(OP),
	level(_level),
	lose_built(false)
{
}

ParseTree::~ParseTree()
{
	delete key;
	delete op;
	delete value;
	delete arr;
	if (!lose_built) { delete build; }
	// delete curr_sub; // do not delete here, it will delete in step SUB, or by tokenizer when parse interrupted in sub-tree
	// delete from; // do not delete here, it will delete by tokenizer or from's from
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

const ParseTree* ParseTree::parse(pElement* const p_e) const
{
	const char& ch = (*p_e)->head();
	if (step & SUB)
	{
		return par_sub(p_e);
	}
	else if (step & ARR)
	{
		return par_arr(p_e);
	}
	else if (step & OP)
	{
		//
		// 5
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
			case closb:
				dispose(p_e);
				done();
				return from;
			case openb:
				step = ParseSteps(ARR);
				dispose(p_e);
				return this;
			default:
				step = ParseSteps(SUB | KEY);
				value = new Volume(p_e);
				build = new Scope(&key, level);
				return this;
			}
		}
		//
		// 2 - OP
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
	// 3
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
	// 4
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

const ParseTree* ParseTree::par_sub(pElement* const p_e) const
{
	const char& ch = (*p_e)->head();
	//
	// 6
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
			((Scope*)build)->append(new Token(&value, level + 1));
			dispose(p_e);
			done();
			return from;
		case equal:
		case gter:
		case less:
			step = SUB;
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
	// 7 - SUB
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
			delete curr_sub;
			dispose(p_e);
			done();
			return from;
		default:
			delete curr_sub;
			step = ParseSteps(SUB | KEY);
			value = new Volume(p_e);
			return this;
		}
	}

}

const ParseTree* ParseTree::par_arr(pElement* const p_e) const
{
	const char& ch = (*p_e)->head();
	if (step & TAG)
	{
		return par_tag_arr(p_e);
	}
	else if (step & VAL)
	{
		// 11 - 14
		return par_val_arr(p_e);
	}
	//
	// 9
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
			ERROR_SYNTAX_ARRAY;
			dispose(p_e);
			return from;
		}
	}
	//
	// 10
	//
	else if (step & KEY)
	{
		switch (ch)
		{
		case openb:
			UNEXPECTED_VALUE;
			dispose(p_e);
			return from;
		case gter:
		case less:
			UNEXPECTED_OPERATOR;
			dispose(p_e);
			return from;
		case equal:
			step = ParseSteps(ARR | TAG);
			build = new TagArray(&key, level);
			((TagArray*)build)->append_new(&arr);
			dispose(p_e);
			return this;
		case closb:
			step = ParseSteps(ARR | VAL | OFF);
			build = new ValueArray(&key, level);
			((ValueArray*)build)->append_new(&arr);
			dispose(p_e);
			return this;
		default:
			step = ParseSteps(ARR | VAL);
			build = new ValueArray(&key, level);
			((ValueArray*)build)->append_new(&arr);
			arr = new Volume(p_e);
			((ValueArray*)build)->append(&arr);
			return this;
		}
	}
	//
	// 8 - ARR
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
			step = ParseSteps(ARR | KEY);
			arr = new Volume(p_e);
			return this;
		}
	}
}

const ParseTree* ParseTree::par_val_arr(pElement* const p_e) const
{
	const char& ch = (*p_e)->head();
	//
	// 12
	//
	if (step & OFF)
	{
		switch (ch)
		{
		case openb:
			step = ParseSteps(ARR | VAL | ON);
			dispose(p_e);
			return this;
		case closb:
			dispose(p_e);
			done();
			return from;
		default:
			ERROR_SYNTAX_ARRAY;
			dispose(p_e);
			return from;
		}
	}
	//
	// 13
	//
	else if (step & ON)
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
			step = ParseSteps(ARR | VAL | OFF);
			dispose(p_e);
			return this;
		default:
			step = ParseSteps(ARR | VAL | KEY);
			arr = new Volume(p_e);
			((ValueArray*)build)->append_new(&arr);
			return this;
		}
	}
	//
	// 14
	//
	else if (step & KEY)
	{
		switch (ch)
		{
		case openb:
		case equal:
		case gter:
		case less:
			UNEXPECTED_ARRAY_TYPE;
			dispose(p_e);
			return from;
		case closb:
			step = ParseSteps(ARR | VAL | OFF);
			dispose(p_e);
			return this;
		default:
			step = ParseSteps(ARR | VAL);
			arr = new Volume(p_e);
			((ValueArray*)build)->append(&arr);
			return this;
		}
	}
	//
	// 11 - ARR | VAL
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
			step = ParseSteps(ARR | VAL | OFF);
			dispose(p_e);
			return this;
		default:
			// step = ParseSteps(ARR | VAL);
			arr = new Volume(p_e);
			((ValueArray*)build)->append(&arr);
			return this;
		}
	}
}

const ParseTree* ParseTree::par_tag_arr(pElement* const p_e) const
{

	const char& ch = (*p_e)->head();
	if (step & VAL)
	{
		//
		// 17
		//
		if (step & OFF)
		{
			switch (ch)
			{
			case openb:
			case equal:
			case gter:
			case less:
				UNEXPECTED_KEY;
				dispose(p_e);
				return from;
			case closb:
				step = ParseSteps(ARR | TAG | OFF); //19
				dispose(p_e);
				return this;
			default:
				step = ParseSteps(ARR | TAG | KEY);
				arr = new Volume(p_e);
				((TagArray*)build)->append_tag(&arr);
				return this;
			}
		}
		//
		// 16 - ARR | TAG | VAL
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
				step = ParseSteps(ARR | TAG | VAL | OFF);
				dispose(p_e);
				return this;
			default:
				// step = ParseSteps(ARR | TAG | VAL);
				arr = new Volume(p_e);
				((TagArray*)build)->append(&arr);
				return this;
			}
		}
	}
	//
	// 18
	//
	else if (step & OFF)
	{
		switch (ch)
		{
		case openb:
			step = ParseSteps(ARR | TAG | ON);
			dispose(p_e);
			return this;
		case closb:
			dispose(p_e);
			done();
			return from;
		default:
			ERROR_SYNTAX_ARRAY;
			dispose(p_e);
			return from;
		}
	}
	//
	// 19
	//
	if (step & ON)
	{
		switch (ch)
		{
		case openb:
		case equal:
		case gter:
		case less:
			UNEXPECTED_KEY;
			dispose(p_e);
			return from;
		case closb:
			step = ParseSteps(ARR | TAG | OFF);
			dispose(p_e);
			return this;
		default:
			step = ParseSteps(ARR | TAG | KEY);
			arr = new Volume(p_e);
			((TagArray*)build)->append_new(&arr);
			return this;
		}
	}
	//
	// 20
	//
	else if (step & KEY)
	{
		switch (ch)
		{
		case equal:
			step = ParseSteps(ARR | TAG);
			dispose(p_e);
			return this;
		case gter:
		case less:
			UNEXPECTED_OPERATOR;
			dispose(p_e);
			return from;
		default:
			UNEXPECTED_ARRAY_TYPE;
			dispose(p_e);
			return from;
		}
	}
	//
	// 15 - ARR | TAG
	//
	else
	{
		switch (ch)
		{
		case openb:
			step = ParseSteps(ARR | TAG | VAL);
			dispose(p_e);
			return this;
		default:
			UNEXPECTED_ARRAY_TYPE;
			dispose(p_e);
			return from;
		}
	}
}

void ParseTree::dispose(pElement* p_e) const
{
	delete (*p_e);
	(*p_e) = nullptr;
}

void ParseTree::done() const
{
	if (from != nullptr)
	{
		from->append(build);
		lose_built = true;
	}
}
