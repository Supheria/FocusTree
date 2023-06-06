#include "use_hoi4_parser.h"
#include <vector>

using namespace std;

typedef list<pcval_u> key_list;
typedef vector<key_list> key_map; // use level as index number

key_map map;

bool exist_in_list(const pcval_u& _v, key_list& _lst)
{
	for (pcval_u& exist : _lst)
	{
		if (strcmp(_v.get(), exist.get()))
		{
			return true;
		}
	}
	return false;
}

void cache_map(const token_list& _toks, key_map& _mp)
{
	for (const ptok_u& _t : _toks)
	{
		if (_t->level() < _mp.size())
		{
			key_list& to_find = _mp[_t->level()];
			if (!exist_in_list(_t->token(), to_find))
			{
				to_find.push_back(move(_t->token()));
			}
		}
		else
		{
			//_mp.push_back(key_list());
			_mp.back().push_back(move(_t->token()));
		}
	}
}

void add_script(const char* filepath)
{
	token_list tokens;
	parse(filepath, tokens);
	//cache_map(tokens, map);
}

int main()
{
	add_script("test.txt");
}