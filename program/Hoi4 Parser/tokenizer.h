#ifndef _HOI4_PARSER_TOKENIZER_H_
#define _HOI4_PARSER_TOKENIZER_H_

#include "parse_tree.h"
#include "token_types.h"

class Tokenizer
{
private:
	struct CompareChar;
public:
	static const CompareChar delimiter, blank, endline, marker;
	static const char note, quote, escape;
private:
	std::unique_ptr<char[]> buffer;
	size_t buflen;
	size_t bufpos;
	size_t line;
	size_t column;
	pTree tree;
	Element elm;
	token_list& tokens;
public:
	Tokenizer(const char* filepath, token_list& tokens);
private:
	void read_buf(const char* path);
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
private:
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
};

#endif // _HOI4_PARSER_TOKENIZER_H_

