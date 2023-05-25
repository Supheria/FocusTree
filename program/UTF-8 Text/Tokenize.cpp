#include "tokenize.h"
#include "element.h"

const CompareChar Tokenize::delimiter({ '\t', ' ', '\n', '\r', '#', '=', '>', '<', '}', '{', '"', (char)-1 });
const CompareChar Tokenize::blank({ '\t', ' ', '\n', '\r' });
const CompareChar Tokenize::endline({ '\n', '\r', (char)-1 });
const CompareChar Tokenize::marker({ '=', '>', '<', '}', '{' });
const char Tokenize::note = '#';
const char Tokenize::quote = '"';

using namespace std;


Tokenize::Tokenize(std::string filepath) :
	pos(0),
    tree(nullptr),
    _tr(tree),
    elm(nullptr)
{
    fin.open(filepath, ios::binary);
    if (!fin.is_open() || fin.eof()) { return; }
    //
    // remove BOM
    //
    if (fin.get() != 0xEF || fin.get() != 0xBB || fin.get() != 0xBF) // fin.get() will return -1 if meet EOF
    { 
        fin.seekg(0, fin.beg);
    }
    tree = new ParseTree();
    while (true)
    {
        if (compose())
        {
            auto _tree = tree->parse(elm);  // tree->parse() will return its sub pointer if call tree->sub->parse()
                                            // and will return its from pointer when parse process finish
                                            // main tree's from pointer is nullptr
            if (_tree == nullptr) 
            {
                map_cache();
                delete tree;
                tree = new ParseTree();
            }
            else { tree = _tree; }
        }
        if (fin.eof()) { break; }
    }
    fin.close();
}

void Tokenize::map_cache()
{
    auto _t = tree->get(); // parse process failed will get nullptr
    if (_t == nullptr) { return; }
    if (map.count(_t->get()))
    {
        map[_t->get()]->append(_t);
    }
    else
    {
        map[_t->get()] = _t;
    }
}

bool Tokenize::compose()
{
    char ch = fin.peek(); // some delimiters will bring to next loop that won't use fin.get()
    switch (state)
    {
    case Build_quo:
        if (ch == quote)
        {
            token << fin.get(); // keep the quote mark
            elm = new eToken(new string(token.str()), pos); // new eToken will delete in tree->~ParseTree()
                                                            // new string will delete finally in ~Token(), flow as:
                                                            //      this.compose().eToken => tree->build => this.map
            state = None;
            return true;
        }
        else if (ch == endline)
        {
            elm = new eToken(new string(token.str()), pos); // will delete in tree->~ParseTree()
            state = None;
            return true;
        }
        token << fin.get();
        return false;
    case Build_unquo:
        if (ch == delimiter)
        {
            elm = new eToken(new string(token.str()), pos); // will delete in tree->~ParseTree()
            state = None;
            return true;
        }
        token << fin.get();
        return false;
    case Note:
        if (ch == endline) { state = None; }
        fin.get();
        return false;
    default:
        if (ch == quote)
        {
            token.clear();
            token << fin.get(); // keep the quote mark
            state = Build_quo;
        }
        else if (ch == note)
        {
            state = Note;
            fin.get();
        }
        else if (ch == marker)
        {
            elm = new Marker(fin.get(), pos); // will delete in tree->~ParseTree()
            return true;
        }
        else if (ch == blank)
        {
            fin.get();
        }
        else
        {
            token.clear();
            token << fin.get();
            state = Build_unquo;
        }
        return false;
    }
}