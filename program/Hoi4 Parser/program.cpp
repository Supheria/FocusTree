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

void get_val(const token_list& _s, list<pcValue>& test)
{
	for (auto t : _s)
	{
		switch (t->type())
		{
		case Token::TAG_ARRAY:
			for (tag_pair_list lst : ((TagArray*)t)->value())
			{
				for (tag_pair tpr : lst)
				{
					for (pVolume v : tpr.second)
					{
						cout << *(v->get());
					}
				}
			}
			break;
		default:
			break;
		}
	}
}
int main()
{
	Tokenizer parser("test.txt");
	const token_list& tokens = parser.get();
	print_token(tokens);
	list<pcValue> test;
	get_val(tokens, test);
	//parser.~Tokenizer();
}