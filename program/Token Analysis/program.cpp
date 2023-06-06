#include "use_hoi4_parser.h"
#include <unordered_map>
#include <string>

using namespace std;

struct unordered_map_cmp { bool operator() (const char* lhs, const char* rhs) const { return strcmp(lhs, rhs) == 0; } };
struct unordered_map_hash { size_t operator()(const char* str) const { size_t seed = 131; size_t hash = 0; while (*str) { hash = hash * seed + (*str++); } return (hash & 0x7FFFFFFF); } };
typedef unordered_map<const char*, token_list, unordered_map_hash, unordered_map_cmp> name_set;
typedef unordered_map<size_t, name_set> key_map; // use level as index number

key_map map;

void cache_map(size_t _lv, token_list& _toks, key_map& _mp)
{
	if (!_mp.count(_lv))
	{
		_mp[_lv] = move(name_set());
	}
	for (ptok_u& _t : _toks)
	{
		if (_t->type() == SCOPE)
		{
			cache_map(_lv + 1, pScope(_t.get())->property(), _mp);
		}
		name_set& to_find = _mp[_lv];
		const char* _name = _t->name().get();
		if (!to_find.count(_name))
		{
			to_find[_name] = move(token_list());
		}
		to_find[_name].push_back(move(_t));
	}
}

void add_script(const char* filepath)
{
	token_list tokens;
	parse(filepath, tokens);
	cache_map(0, tokens, ::map);
}

int main()
{
	token_list tokens;
	//parse("test.txt", tokens);
	//cache_map(tokens, map);
	add_script("test.txt");
}