#include "parse_tree.h"
#include "token_types.h"
#include "exception_log.h"
#include <format>

using namespace std;
using namespace hoi4::parser;

const char ParseTree::openb = '{';
const char ParseTree::closb = '}';
const char ParseTree::equal = '=';
const char ParseTree::gter = '>';
const char ParseTree::less = '<';

extern hoi4::ExLog Logger;
const char* fn_tree = "parse_tree";
#define UNKNOWN_ERROR Logger(fn_tree, format( "unknown error at line({}), column({})", _e.line(), _e.column()).c_str(), ExLog::ERR)
#define UNEXPECTED_KEY Logger(fn_tree, format( "unexpected key at line({}), column({})", _e.line(), _e.column()).c_str(), ExLog::ERR)
#define UNEXPECTED_OPERATOR Logger(fn_tree, format( "unexpected operator at line({}), column({})", _e.line(), _e.column()).c_str(), ExLog::ERR)
#define UNEXPECTED_VALUE Logger(fn_tree, format( "unexpected value at line({}), column({})", _e.line(), _e.column()).c_str(), ExLog::ERR)
#define UNEXPECTED_ARRAY_TYPE Logger(fn_tree, format( "unexpected array type at line({}), column({})", _e.line(), _e.column()).c_str(), ExLog::ERR)
#define ERROR_SYNTAX_ARRAY Logger(fn_tree, format( "wrong array syntax at line({}), column({})", _e.line(), _e.column()).c_str(), ExLog::ERR)


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
ParseTree::ParseTree(const pTree _from, pcval_u _key, pcval_u _op, const size_t& _level) :
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

const pTree ParseTree::get_from() const
{
	return from;
}

void ParseTree::append(pToken _t) const
{
	pScope(build)->append(_t);
}

const pTree ParseTree::parse(Element& _e) const
{
	const char& ch = _e.head();
	if (step & SUB)
	{
		return par_sub(_e);
	}
	else if (step & ARR)
	{
		return par_arr(_e);
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
				_e.get();
				return from;
			case closb:
				_e.get();
				done();
				return from;
			case openb:
				step = Steps(ARR);
				_e.get();
				return pTree(this);
			default:
				step = Steps(SUB | KEY);
				value = pcval_u(_e.get());
				build = new Scope(key.release(), level);
				return pTree(this);
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
				_e.get();
				return from;
			case openb:
				if (op[0] != equal)
				{
					UNEXPECTED_OPERATOR;
					_e.get();
					return from;
				}
				else
				{
					step = Steps(OP | ON);
					_e.get();
					return pTree(this);
				}
			default:
				step = Steps(VAL);
				value = pcval_u(_e.get());
				build = new Tag(key.release(), op.release(), value.release(), level);
				return pTree(this);
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
			op = pcval_u(_e.get());
			return pTree(this);
		default:
			UNEXPECTED_OPERATOR;
			_e.get();
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
			_e.get();
			return pTree(this);
		default:
			done();
			// _e.get(); // leave *_e to next tree
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
			_e.get();
			return from;
		case closb:
			_e.get();
			done();
			return from;
		default:
			step = TAG;
			pTag(build)->append(_e.get());
			return pTree(this);
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
			_e.get();
			return from;
		default:
			step = KEY;
			key = pcval_u(_e.get());
			return pTree(this);
		}
	}
}

const pTree ParseTree::par_sub(Element& _e) const
{
	const char& ch = _e.head();
	//
	// 6
	//
	if (step & KEY)
	{
		switch (ch)
		{
		case openb:
			UNEXPECTED_VALUE;
			_e.get();
			return from;
		case closb:
			pScope(build)->append(new Token(value.release(), level + 1));
			_e.get();
			done();
			return from;
		case equal:
		case gter:
		case less:
			step = SUB;
			curr_sub = new ParseTree(pTree(this), value.release(), _e.get(), level + 1);
			return curr_sub;
		default:
			// step = Steps(SUB | KEY);
			pScope(build)->append(new Token(value.release(), level + 1));
			value = pcval_u(_e.get());
			return pTree(this);
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
			_e.get();
			done();
			return from;
		default:
			delete curr_sub;
			step = Steps(SUB | KEY);
			value = pcval_u(_e.get());
			return pTree(this);
		}
	}

}

const pTree ParseTree::par_arr(Element& _e) const
{
	const char& ch = _e.head();
	if (step & TAG)
	{
		return par_tag_arr(_e);
	}
	else if (step & VAL)
	{
		// 11 - 14
		return par_val_arr(_e);
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
			_e.get();
			return pTree(this);
		case closb:
			_e.get();
			done();
			return from;
		default:
			ERROR_SYNTAX_ARRAY;
			_e.get();
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
			_e.get();
			return from;
		case gter:
		case less:
			UNEXPECTED_OPERATOR;
			_e.get();
			return from;
		case equal:
			step = Steps(ARR | TAG);
			build = new TagArray(key.release(), level);
			pTagArr(build)->append_new(arr.release());
			_e.get();
			return pTree(this);
		case closb:
			step = Steps(ARR | VAL | OFF);
			build = new ValueArray(key.release(), level);
			pValArr(build)->append_new(arr.release());
			_e.get();
			return pTree(this);
		default:
			step = Steps(ARR | VAL);
			build = new ValueArray(key.release(), level);
			pValArr(build)->append_new(arr.release());
			arr = pcval_u(_e.get());
			pValArr(build)->append(arr.release());
			return pTree(this);
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
			_e.get();
			return from;
		case closb:
			step = Steps(ARR | OFF);
			_e.get();
			return pTree(this);
		default:
			step = Steps(ARR | KEY);
			arr = pcval_u(_e.get());
			return pTree(this);
		}
	}
}

const pTree ParseTree::par_val_arr(Element& _e) const
{
	const char& ch = _e.head();
	//
	// 12
	//
	if (step & OFF)
	{
		switch (ch)
		{
		case openb:
			step = Steps(ARR | VAL | ON);
			_e.get();
			return pTree(this);
		case closb:
			_e.get();
			done();
			return from;
		default:
			ERROR_SYNTAX_ARRAY;
			_e.get();
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
			_e.get();
			return from;
		case closb:
			step = Steps(ARR | VAL | OFF);
			_e.get();
			return pTree(this);
		default:
			step = Steps(ARR | VAL | KEY);
			arr = pcval_u(_e.get());
			pValArr(build)->append_new(arr.release());
			return pTree(this);
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
			_e.get();
			return from;
		case closb:
			step = Steps(ARR | VAL | OFF);
			_e.get();
			return pTree(this);
		default:
			step = Steps(ARR | VAL);
			arr = pcval_u(_e.get());
			pValArr(build)->append(arr.release());
			return pTree(this);
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
			_e.get();
			return from;
		case closb:
			step = Steps(ARR | VAL | OFF);
			_e.get();
			return pTree(this);
		default:
			// step = Steps(ARR | VAL);
			arr = pcval_u(_e.get());
			pValArr(build)->append(arr.release());
			return pTree(this);
		}
	}
}

const pTree ParseTree::par_tag_arr(Element& _e) const
{

	const char& ch = _e.head();
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
				_e.get();
				return from;
			case closb:
				step = Steps(ARR | TAG | OFF); //19
				_e.get();
				return pTree(this);
			default:
				step = Steps(ARR | TAG | KEY);
				arr = pcval_u(_e.get());
				pTagArr(build)->append_tag(arr.release());
				return pTree(this);
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
				_e.get();
				return from;
			case closb:
				step = Steps(ARR | TAG | VAL | OFF);
				_e.get();
				return pTree(this);
			default:
				// step = Steps(ARR | TAG | VAL);
				arr = pcval_u(_e.get());
				pTagArr(build)->append(arr.release());
				return pTree(this);
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
			step = Steps(ARR | TAG | ON);
			_e.get();
			return pTree(this);
		case closb:
			_e.get();
			done();
			return from;
		default:
			ERROR_SYNTAX_ARRAY;
			_e.get();
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
			_e.get();
			return from;
		case closb:
			step = Steps(ARR | TAG | OFF);
			_e.get();
			return pTree(this);
		default:
			step = Steps(ARR | TAG | KEY);
			arr = pcval_u(_e.get());
			pTagArr(build)->append_new(arr.release());
			return pTree(this);
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
			step = Steps(ARR | TAG);
			_e.get();
			return pTree(this);
		case gter:
		case less:
			UNEXPECTED_OPERATOR;
			_e.get();
			return from;
		default:
			UNEXPECTED_ARRAY_TYPE;
			_e.get();
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
			step = Steps(ARR | TAG | VAL);
			_e.get();
			return pTree(this);
		default:
			UNEXPECTED_ARRAY_TYPE;
			_e.get();
			return from;
		}
	}
}

void ParseTree::done() const
{
	if (from != nullptr)
	{
		from->append(build);
		lose_built = true;
	}
}
