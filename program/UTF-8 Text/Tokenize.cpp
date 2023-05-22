#include "Tokenize.h"

Tokenize::Tokenize(std::string filepath) :
	chain(std::vector<Token>(0)),
	index(0)
{
	Utf8Reader reader(filepath);
	if (reader.bufferlength() == 0) { return; }
	//chain = new Token[reader.bufferlength()];
	build(reader);
}

size_t Tokenize::get(Token& _t)
{
	return size_t();
}

bool Tokenize::get(int index, Token& _t)
{
	return false;
}

void Tokenize::build(Utf8Reader& r)
{
	while (r.read())
	{
		//if (r.getu8char() == )
	}
}

