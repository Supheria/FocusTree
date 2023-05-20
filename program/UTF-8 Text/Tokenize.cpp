#include "Tokenize.h"
#include "Utf8Reader.h"

Tokenize::Tokenize(std::string filepath) :
	chain(std::vector<Token>(0)),
	index(0)
{
	Utf8Reader reader(filepath);
	if (reader.bufferlength() == 0) { return; }
	//chain = new Token[reader.bufferlength()];
	build(reader);
}

void Tokenize::build(Utf8Reader& r)
{
	while (r.read())
	{
		if (r.getu8char() == )
	}
}

