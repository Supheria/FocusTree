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

		enum token_types
		{
			TOKEN,
			TAGGED,
			VAL_ARRAY,
			TAG_ARRAY,
			SCOPE
		};


		typedef struct Tok
		{
			virtual pcval_u& name() = 0;
			virtual const token_types& type() const = 0;
			// 0 mean to main-Token or say root-Token
			virtual const size_t& level() const = 0;
		} *pToken;

		typedef std::unique_ptr<Tok> ptok_u;


		typedef std::list<pcval_u> value_list;
		typedef value_list tagged_val;
		typedef struct Tgg : Tok
		{
			virtual pcval_u& name() = 0;
			virtual const token_types& type() const = 0;
			// 0 mean to main-Token or say root-Token
			virtual const size_t& level() const = 0;
			virtual pcval_u& operat() = 0;
			virtual pcval_u& tag() = 0;
			virtual tagged_val& value() = 0;
		} *pTagged;


		typedef std::list<value_list> arr_v;
		typedef struct VAr : Tok
		{
			virtual pcval_u& name() = 0;
			virtual const token_types& type() const = 0;
			// 0 mean to main-Token or say root-Token
			virtual const size_t& level() const = 0;
			virtual arr_v& value() = 0;
		} *pValArr;


		typedef std::pair<pcval_u, tagged_val> tag_pair;
		typedef std::list<tag_pair> tag_pair_list;
		typedef std::list<tag_pair_list> arr_t;
		typedef struct TAr : Tok
		{
			virtual pcval_u& name() = 0;
			virtual const token_types& type() const = 0;
			// 0 mean to main-Token or say root-Token
			virtual const size_t& level() const = 0;
			virtual arr_t& value() = 0;
		} *pTagArr;


		typedef std::list<ptok_u> token_list;
		typedef struct Sco : Tok
		{
			virtual pcval_u& name() = 0;
			virtual const token_types& type() const = 0;
			// 0 mean to main-Token or say root-Token
			virtual const size_t& level() const = 0;
			virtual token_list& property() = 0;
		} *pScope;
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