#ifdef IMPORT_PARSER	
#define PARSER_DLL extern "C" __declspec(dllimport)
#else
#define PARSER_DLL extern "C" __declspec(dllexport)
#endif

#include "tokenizer.h"
#include "dll_in.h"

using namespace hoi4::parser;

PARSER_DLL void parse(const char* filepath, token_list& tokens)
{
	//return path[0];
	Tokenizer tk(filepath, tokens);
	//print_list(Tokens);
}

PARSER_DLL void reset_log()
{
	Logger.reset();
}