#include "Tokenize.h"
#include<fstream>

const char Tokenize::BOM[3] = { 0xEF, 0xBB, 0xBF };
const CompareChar Tokenize::delimiter ({ '\t', ' ', '\n', '\r', '#', '=', '>', '<', '}', '{', '"' });
const CompareChar Tokenize::blank({ '\t', ' ' });
const CompareChar Tokenize::endline({ '\n', '\r' });
const CompareChar Tokenize::keychar({ '=', '>', '<', '}', '{' });
const CompareChar Tokenize::note({ '#' });
const CompareChar Tokenize::quote({ '"' });

using namespace std;

Tokenize::Tokenize(string filepath) :
	chain(std::vector<Token>(0)),
	line(0),
    column(0)
{
    ifstream fin;
    fin.open(filepath, ios::binary);
    if (!fin.is_open() || fin.eof()) { return; }

    string str;
    getline(fin, str);
    char fileHead[3] = { 0, 0, 0 };
    for (int i = 0; i < 3 && i < str.length(); i++)
    {
        fileHead[i] = fin.get();
    }
    if (str.length() >= 3 && str[0] == BOM[0] && str[1] == BOM[1] && str[2] == BOM[2]) 
    { 
        str = str.substr(3, str.length() - 3);
    }
    build_line(str);

    while (!fin.eof())
    {
        getline(fin, str);
        build_line(str);
    }
    fin.close();
}

void Tokenize::build_line(const string& s)
{
	for(auto ch : s)
	{
		if (ch == blank || ch == endline) { continue; }
		if (ch == note){}
	}
}

size_t Tokenize::get(Token& _t)
{
	return size_t();
}

bool Tokenize::get(int index, Token& _t)
{
	return false;
}
