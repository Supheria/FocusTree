#include "key.h"

using namespace std;

Key::Key(KeyTypes _tp, const string* _key, const Token* _fr)
	: Token(_key),
	tp(_tp),
	fr(_fr)
{
}

Key::~Key()
{
	if (fr != nullptr) { delete fr; }
}

const Key* Key::parse(const std::string& filename)
{
	return this;
}

void Key::combine(const Key* k)
{
}
