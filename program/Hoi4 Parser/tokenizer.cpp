#include "tokenizer.h"
#include "exception_log.h"

const CompareChar Tokenizer::delimiter({ '\t', ' ', '\n', '\r', '#', '=', '>', '<', '}', '{', '"', (char)-1 });
const CompareChar Tokenizer::blank({ '\t', ' ', '\n', '\r', (char)-1 });
const CompareChar Tokenizer::endline({ '\n', '\r', (char)-1 });
const CompareChar Tokenizer::marker({ '=', '>', '<', '}', '{' });
const char Tokenizer::note = '#';
const char Tokenizer::quote = '"';
const char Tokenizer::escape = '\\';

extern WarningLog Warnlog;

using namespace std;

const string FileName = "tokenizer";

Tokenizer::Tokenizer(std::string filepath) :
    path(filepath),
    line(1),
    column(0),
    tree(nullptr),
    elm(nullptr),
    state(None)
{
}

Tokenizer::~Tokenizer()
{
    for (auto t : tokens)
    {
        delete t;
    }
}

const token_list& Tokenizer::get()
{
    parse();
    return tokens;
}

void Tokenizer::parse()
{
    fin.open(path, ios::binary);
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
    do
    {
        if (compose())
        {
            auto _tree = tree->parse(&elm);  // tree->parse() will return its sub pointer if call tree->sub->parse()
            // and will return its from pointer when parse process finish
            // main tree's from pointer is nullptr
            // if parse failed, any tree will return its from pointer
            if (_tree == nullptr) // main-tree finish, go to next main-tree
            {
                cache_list();
                delete tree;
                tree = new ParseTree();
            }
            else { tree = _tree; }
        }
    } while (!fin.eof());
    del_tree();
    fin.close();
}

void Tokenizer::cache_list()
{
    pToken _t = tree->once_get(); // parse process unfinished or failed will get nullptr
    if (_t == nullptr) { return; }
    tokens.push_back(_t);
}

bool Tokenizer::compose()
{
    // if tree built successfully (has gone to "return" node), 
                                        // elm didn't be used, should pass to next tree
    if (elm != nullptr) { return true; }
    // some delimiters will bring to next loop 
                        // that won't use fin.get() to loop one more time to update state
    char ch = fin.peek(); 
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

void Tokenizer::del_tree()
{
    if (tree->get_from() != nullptr)
    {
        Warnlog(FileName, format("interruption at line({}), column({})", line, column));
        tree->get_from()->append(tree->once_get());
        const ParseTree* _tree = tree->get_from();
        delete tree;
        tree = _tree;
        while (tree->get_from() != nullptr)
        {
            tree->get_from()->append(tree->once_get());
            const ParseTree* _tree = tree->get_from();
            delete tree;
            tree = _tree;
        }
    }
    cache_list(); // 
    delete tree;
    tree = nullptr;
}
