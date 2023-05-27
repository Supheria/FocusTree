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
	line(0),
    column(0),
    tree(nullptr),
    _tr(tree),
    elm(nullptr),
    state(None)
{
    fin.open(filepath, ios::binary);
    if (!fin.is_open() || fin.eof()) { return; }
    //
    // remove BOM
    //
    if (fget() != 0xEF || fget() != 0xBB || fget() != 0xBF) // fin.get() will return -1 if meet EOF
    { 
        fin.seekg(0, fin.beg);
        column = 0;
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
        map[_t->get()]->combine(_t);
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
            token << fget(); // keep the quote mark
            elm = new eToken(new string(token.str()), new size_t(line), column); // new eToken will delete within process of tree->parse(_e)
                                                            // new string and new size_t may delete finally in _e->~Token(), flow as:
                                                            //      this.compose() => tree->build => this.map[key].~Token()
                                                            // also may delete within process of parse() by using _e->del(), flow as:
                                                            //      this.compose() => tree->parse(_e) => _e->del()
            state = None;
            return true;
        }
        else if (ch == endline)
        {
            elm = new eToken(new string(token.str()), new size_t(line), column);
            state = None;
            return true;
        }
        token << fget();
        return false;
    case Build_unquo:
        if (ch == delimiter)
        {
            elm = new eToken(new string(token.str()), new size_t(line), column);
            state = None;
            return true;
        }
        token << fget();
        return false;
    case Note:
        if (ch == endline) { state = None; }
        fget();
        return false;
    default:
        if (ch == quote)
        {
            token.clear();
            token << fget(); // keep the quote mark
            state = Build_quo;
        }
        else if (ch == note)
        {
            state = Note;
            fget();
        }
        else if (ch == marker)
        {
            elm = new Marker(new char(fget()), new size_t(line), new size_t(column)); // same as eToken above ^
            return true;
        }
        else if (ch == blank)
        {
            if (fget() == '\n')
            {
                line++;
                column = 0;
            }
        }
        else
        {
            token.clear();
            token << fget();
            state = Build_unquo;
        }
        return false;
    }
}

char Tokenize::fget()
{
    column++;
    return fin.get();
}