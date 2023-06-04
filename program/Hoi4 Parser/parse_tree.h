#ifndef _PARSE_TREE
#define _PARSE_TREE

#include "element.h"
#include "token.h"
#include <memory>
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
			// Token will delete key and set it to nullptr when pass to it
								 // if will have sub-tree,
								 // need to set from_key to key->get() before key pass a to new Token
			mutable pcval_u key;
			mutable pcval_u op; // Token delete op and set it to nullptr when pass to it
			mutable pcval_u value; // same as above ^
			mutable pcval_u arr; // same as above ^
			mutable pToken build;
			const pTree from; // nullptr means to main-Tree or say root-Tree
			mutable pTree curr_sub;
			const size_t level;
			mutable bool lose_built;
		public:
			ParseTree();
			// for sub-tree
			ParseTree(const pTree _from, pcval_u _key, pcval_u _op, const size_t& _level);
			~ParseTree();
			// for tokenizer to use
								// can only get build on time,
								// when get will transfer ownership of build that 
								// won't delete in ~ParseTree()
			pToken once_get() const;
			// for tokenizer to use, test whether parse process has interrupted
			const pTree get_from() const;
			// append sub-tree's build to this->(Scope*)build
			void append(pToken _t) const;
			// will return pointer to sub-tree if next step will be SUB
											// and will return its from pointer when parse process finish
											// main tree's from pointer is nullptr
											// if parse failed, any tree will return its from pointer
			const pTree parse(Element& _e) const;
		private:
			const pTree par_sub(Element& _e) const;
			const pTree par_arr(Element& _e) const;
			const pTree par_tag_arr(Element& _e) const;
			const pTree par_val_arr(Element& _e) const;
			void done() const;
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
			} mutable step;
		};
	}
}

#endif