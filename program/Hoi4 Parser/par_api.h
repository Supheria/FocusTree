#ifndef _HOI4_PARSER_API_H_
#define _HOI4_PARSER_API_H_

#include <memory>
#include <list>

namespace hoi4
{
	namespace parser
	{
		typedef const char* Value;
		typedef std::unique_ptr<const char[]> pcval_u;

		typedef class Token* pToken;
		typedef std::unique_ptr<Token> ptok_u;
		struct IToken
		{
		public:
			enum token_types
			{
				TOKEN,
				TAG,
				VAL_ARRAY,
				TAG_ARRAY,
				SCOPE
			};
		public:
			virtual const token_types& type() const = 0;
			virtual pcval_u& token() = 0;
			virtual const size_t& level() const = 0;
		};
	}
}

#ifdef IMPORT_HOI4_PARSER
#define HOI4_PARSER_API extern "C" __declspec(dllimport)
#else
#define HOI4_PARSER_API extern "C" __declspec(dllexport)
#endif

#include "token_types.h"

HOI4_PARSER_API void parse(const char* filepath, hoi4::parser::token_list& tokens);

#endif // !_HOI4_PARSER_API_H_