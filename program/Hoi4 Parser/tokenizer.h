#ifndef _TOKENIZER_H
#define _TOKENIZER_H

#include <list>
#include <sstream>
#include <fstream>
#include <unordered_map>
#include "parse_tree.h"
#include "token_types.h"

struct CompareChar;

class Tokenizer
{
public:
	static const CompareChar delimiter, blank, endline, marker;
	static const char note, quote, escape;
private:
	std::string path;
	char* buffer;
	size_t buflen;
	size_t bufpos;
	size_t line;
	size_t column;
	const ParseTree* tree;
	Element elm;
	std::stringstream token;
	token_list tokens;
public:
	Tokenizer(std::string filepath);
	~Tokenizer();
	// for t in get(), use t->token()->get() to tansfer the ownership of value, 
							// and so for other pVolume -s of t
	const token_list& get();
private:
	void read_buf();
	void parse();
	void cache_list();
	bool compose(char& ch);
	char fget();
	// need to test whether current tree has return to root,
					// otherwise need to del all tree remained
					// include main-tree
	void del_tree(); 
private:
	enum
	{
		None,
		Build_quo,
		Escape_quo,
		Build_unquo,
		Note
	} state;
};

struct CompareChar
{
private:
	std::list<char> chs;
public:
	CompareChar(std::list<char> chars)
	{
		chs = chars;
	}
	bool operator==(const char& ch) const
	{
		for (auto it : chs)
		{
			if (it == ch)
			{
				return true;
			}
		}
		return false;
	}
	bool operator!=(const char& ch) const
	{
		return !(*this == ch);
	}
	friend bool operator==(const char& ch, const CompareChar& cc)
	{
		return cc == ch;
	}
	friend bool operator!=(const char& ch, const CompareChar& cc)
	{
		return cc != ch;
	}
};

#endif // _TOKENIZER_H

