#include "tokenizer.h"
#include <iostream>

using namespace std;

void print_token(const token_list& _s)
{
	for (auto t : _s)
	{
		for (int i = 0; i < t->level(); i++)
		{
			cout << '\t';
		}
		cout << t->token().value() << ": tp" << t->type() << endl;
		if (t->type() == Token::SCOPE)
		{
			print_token(((pScope)t)->property()); 
		}
	}
}

int main()
{
	Tokenizer parser("test.txt");
	const token_list& tokens = parser.get();
	print_token(tokens);
}