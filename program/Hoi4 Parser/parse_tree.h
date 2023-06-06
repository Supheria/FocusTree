#ifndef _HOI4_PARSER_PARSE_TREE_H_
#define _HOI4_PARSER_PARSE_TREE_H_

#include "element.h"
#include "token.h"

namespace hoi4
{
	namespace parser
	{

		typedef class ParseTree* pTree;
		class ParseTree
		{
		public:
			static const char  openb, closb, equal, gter, less;
		private:
			pcval_u key;
			pcval_u op;
			pcval_u value;
			pcval_u arr;
			ptok_u build;
			pTree from; // nullptr means to main-Tree or say root-Tree
			pTree curr_sub;
			size_t level;
		public:
			ParseTree();
			// to create sub-tree
			ParseTree(const pTree _from, Value _key, Value _op, const size_t& _level);
			// for tokenizer to use
								// can only get build one time,
								// when get will transfer ownership of build
			pToken once_get();
			// for tokenizer to use, test whether parse process has interrupted
			const pTree get_from();
			// append sub-tree's build to this->(pScope)build
			void append(pToken _t);
			// will return pointer to sub-tree if next step will be SUB,
											// or will return its from pointer when parse process finish or failed,
											// or will return this when parsing is in progress
			const pTree parse(Element& _e);
		private:
			const pTree par_sub(Element& _e);
			const pTree par_arr(Element& _e);
			const pTree par_tag_arr(Element& _e);
			const pTree par_val_arr(Element& _e);
			void done();
		private:
			enum Steps : size_t
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
			} step;
		};
	}
}

#endif // !_HOI4_PARSER_PARSE_TREE_H_
