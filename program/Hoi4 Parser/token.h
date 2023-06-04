#ifndef _TOKEN_H
#define _TOKEN_H

#include "Element.h"

namespace hoi4
{
	namespace parser
	{
		typedef class Token* pToken; // not prefer to use unique_ptr of which trans-type to inherited class is horrible
		class Token
		{
		public:
			enum TokT
			{
				TOKEN,
				TAG,
				VAL_ARRAY,
				TAG_ARRAY,
				SCOPE
			};
		protected:
			const TokT tp;
			pcval_u tok;
			const size_t lv; // 0 mean to main-Token or say root-Token
		protected:
			// inherited class should not pass nullptr of _tok, use _vol_() to get a pcval_u from an Element
			Token(const TokT& _t, Value _tok, const size_t& _lv);
			// return a new Volume, and delete p_vol
			static Value _vol_(pcval_u& _v, const Value null_val);
		public:
			// to create a pure token, will delete (*p_key) and set it to nullptr
			Token(pcval_u& _tok, const size_t& _lv);
			const TokT& type() const;
			// use token().get() to transfer ownership of value of tok
			pcval_u& token();
			const size_t& level() const;
		public:
			virtual ~Token();
		};
	}
}

#endif // ! _TOKEN_H
