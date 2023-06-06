#ifdef DEBUG

#include "tokenizer.h"
#include <iostream>
#include <time.h>
#include "use_ex_log.h"
#include <fstream>
//#include <iostream>
//#include <fstream>

using namespace std;
using namespace hoi4::parser;


void print_list(const token_list& _s);

int main()
{
	ex_log()->reset();
	token_list tokens;
	Tokenizer* tk = new Tokenizer("test.txt", tokens);
	delete tk;
	print_list(tokens);
}

ofstream fout("out.txt");
void print_list(const token_list& _s)
{
	for (const ptok_u& t : _s)
	{
		for (int i = 0; i < t->level(); i++)
		{
			fout << '\t';
		}
		fout << t->token().get() << ": tp" << t->type() << endl;
		if (t->type() == Token::SCOPE)
		{
			print_list((pScope(t.get()))->property());
		}
	}
}

#endif // DEBUG

#include "par_api.h"
#include "tokenizer.h"

using namespace hoi4::parser;

void parse(const char* filepath, hoi4::parser::token_list& tokens)
{
	//return path[0];
	Tokenizer tk(filepath, tokens);
	//print_list(Tokens);
}