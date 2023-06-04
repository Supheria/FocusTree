#include "tokenizer.h"
#include <iostream>
#include <time.h>

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

void get_val(const token_list& _s, list<Value>& test)
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
					for (Volume v : tpr.second)
					{
						cout << *(v.get());
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
	clock_t start, end;
	start = clock();
	token_list tokens;
	Tokenizer parser("test.txt", tokens);
	//const  = parser.get();
	end = clock();
	cout << "time = " << double(end - start) / CLOCKS_PER_SEC << "s" << endl;
	//print_token(tokens);
	list<Value> test;
	get_val(tokens, test);
	//parser.~Tokenizer();
}