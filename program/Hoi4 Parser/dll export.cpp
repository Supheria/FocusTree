#define DLL_EXPORT extern "C" __declspec(dllexport)

#include "tokenizer.h"
#include "exception_log.h"
//#include <iostream>
//#include <fstream>

using namespace std;
using namespace hoi4::parser;

//static token_list Tokens;
//static string LogPath = "ex.log";
//static string Bridgepath;
//ExLog Logger(LogPath);
//
//DLL_EXPORT void set_log(string logpath)
//{
//	LogPath = logpath;
//}
//
//DLL_EXPORT void set_bridge(string brgpath)
//{
//	Bridgepath = brgpath;
//}

//void print_list(const token_list& _s);

DLL_EXPORT void parse(const char* filepath, token_list& tokens)
{
	//return path[0];
	Tokenizer tk(filepath, tokens);
	//print_list(Tokens);
}

DLL_EXPORT void reset_log()
{
	Logger.reset();
}
//
//void print_list(const token_list& _s)
//{
//	ofstream fout;
//	fout.open(("out.txt"));
//	for (auto t : _s)
//	{
//		for (int i = 0; i < t->level(); i++)
//		{
//			fout << '\t';
//		}
//		fout << t->token().value() << ": tp" << t->type() << endl;
//		if (t->type() == Token::SCOPE)
//		{
//			print_list(((pScope)t)->property());
//		}
//	}
//}
//
//DLL_EXPORT void print_list()
//{
//	print_list(Tokens);
//}