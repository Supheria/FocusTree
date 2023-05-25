#ifndef  _TOKENIZE_H
#define _TOKENIZE_H

#include <sstream>
#include <vector>
#include <fstream>
#include <unordered_map>
#include "parsetree.h"
#include "token.h"

struct CompareChar;

class Tokenize
{
public:
	static const CompareChar delimiter, blank, endline, marker;
	static const char  note, quote;
private:
	size_t pos;
	ParseTree* tree;
	Element* elm;
	const ParseTree* _tr;
	std::stringstream token;
	std::ifstream fin;
	std::unordered_map<std::string, Token*> map;
public:
	Tokenize(std::string filepath);
	void map_cache();
private:
	bool compose();
private:
	enum
	{
		None,
		Build_quo,
		Build_unquo,
		Note
	} state;
};


struct CompareChar
{
	std::vector<char> chs;
public:
	CompareChar(std::vector<char> chars)
	{
		chs = chars;
	}
	bool operator==(const char& ch) const
	{
		for (int i = 0; i < chs.size(); i++)
		{
			if (chs[i] != ch)
			{
				return false;
			}
		}
		return true;
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

#endif // ! _TOKENIZE_H