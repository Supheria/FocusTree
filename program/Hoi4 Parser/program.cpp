#include "tokenizer.h"
#include <iostream>
#include <time.h>
#include "dll_in.h"
//#include <iostream>
//#include <fstream>

using namespace std;
using namespace hoi4::parser;


void print_list(const token_list& _s);

int main()
{
	Logger.reset();
	token_list tokens;
	Tokenizer tk("test.txt", tokens);
	print_list(tokens);
}

ofstream fout("out.txt");
void print_list(const token_list& _s)
{
	for (auto t : _s)
	{
		for (int i = 0; i < t->level(); i++)
		{
			fout << '\t';
		}
		fout << t->token().get() << ": tp" << t->type() << endl;
		if (t->type() == Token::SCOPE)
		{
			print_list(((pScope)t)->property());
		}
	}
}