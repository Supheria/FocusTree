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
	static const char  note, quote, escape;
private:
	size_t line;
	size_t column;
	ParseTree* tree;
	Element* elm;
	std::stringstream token;
	std::ifstream fin;
	tok_map tokenmap;
public:
	Tokenizer(std::string filepath);
private:
	void map_cache();
	bool compose();
	char fget();
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

