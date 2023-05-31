#include "tokenizer.h"

const CompareChar Tokenizer::delimiter({ '\t', ' ', '\n', '\r', '#', '=', '>', '<', '}', '{', '"', (char)-1 });
const CompareChar Tokenizer::blank({ '\t', ' ', '\n', '\r' });
const CompareChar Tokenizer::endline({ '\n', '\r', (char)-1 });
const CompareChar Tokenizer::marker({ '=', '>', '<', '}', '{' });
const char Tokenizer::note = '#';
const char Tokenizer::quote = '"';
const char Tokenizer::escape = '\\';

using namespace std;

Tokenizer::Tokenizer(std::string filepath) :
    line(0),
    column(0),
    tree(nullptr),
    elm(nullptr),
    state(None)
{
    fin.open(filepath, ios::binary);
    if (!fin.is_open() || fin.eof()) { return; }
    //
    // remove BOM
    //
    if (fget() != (char)0xEF || fget() != (char)0xBB || fget() != (char)0xBF) // fin.get() will return -1 if meet EOF
    {
        fin.seekg(0, fin.beg);
        column = 0;
    }
    //
    // parsing
    //
    tree = new ParseTree();
    while (true)
    {
        if (compose())
        {
            auto _tree = tree->parse(&elm);  // tree->parse() will return its sub pointer if call tree->sub->parse()
                                            // and will return its from pointer when parse process finish
                                            // main tree's from pointer is nullptr
                                            // if parse failed, any tree will return its from pointer
            if (_tree == nullptr) // main-tree finish, go to next main-tree
            {
                map_cache();
                delete tree;
                tree = new ParseTree();
            }
            else { tree = _tree; }
            if (elm != nullptr) // tree built successfully (has gone to "return" node), elm didn't be used, should pass to next tree
            {

            }
        }
        if (fin.eof()) { break; }
    }
    delete tree;
    fin.close();
}

void Tokenizer::map_cache()
{
    pToken _t = tree->get(); // parse process failed will get nullptr
    if (_t == nullptr) { return; }
    pcValue key = &(_t->token().volumn());
    if (tokenmap.count(key)) // has the key
    {
        tokenmap[key]->mix(_t);
    }
    else { tokenmap[key] = _t; }
}

bool Tokenizer::compose()
{
    char ch = fin.peek(); // some delimiters will bring to next loop that won't use fin.get()
    switch (state)
    {
    case Build_quo:
        if (ch == escape)
        {
            token << fget();
            state = Escape_quo;
            return false;
        }
        else if (ch == quote)
        {
            token << fget(); // keep the quote mark
            elm = new Element(token.str(), line, column);   // will delete within process of tree->parse(...) when 
                                                            // not send to any Token,
                                                            // or will delete in Token::_vol_(...)
            state = None;
            return true;
        }
        else if (ch == endline)
        {
            token << quote;
            elm = new Element(token.str(), line, column);
            state = None;
            return true;
        }
        token << fget();
        return false;
    case Escape_quo:
        if (ch == endline)
        {
            token << quote << quote;
            elm = new Element(token.str(), line, column);
            state = None;
            return true;
        }
        else
        {
            token << fget();
            state = Build_quo;
            return false;
        }
    case Build_unquo:
        if (ch == delimiter)
        {
            elm = new Element(token.str(), line, column);
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
            token.str("");
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
            elm = new Element(fget(), line, column); // same as eToken above ^
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
            token.str("");
            token << fget();
            state = Build_unquo;
        }
        return false;
    }
}

char Tokenizer::fget()
{
    column++;
    return fin.get();
}
