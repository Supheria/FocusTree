#include "tokenizer.h"
#include "exception_log.h"

const CompareChar Tokenizer::delimiter({ '\t', ' ', '\n', '\r', '#', '=', '>', '<', '}', '{', '"', '\0'});
const CompareChar Tokenizer::blank({ '\t', ' ', '\n', '\r', '\0' });
const CompareChar Tokenizer::endline({ '\n', '\r', '\0' });
const CompareChar Tokenizer::marker({ '=', '>', '<', '}', '{' });
const char Tokenizer::note = '#';
const char Tokenizer::quote = '"';
const char Tokenizer::escape = '\\';

extern ErrorLog Errlog;
extern WarningLog Warnlog;

using namespace std;

const string FileName = "tokenizer";

Tokenizer::Tokenizer(std::string filepath) :
    path(filepath),
    buffer(nullptr),
    buflen(0),
    bufpos(0),
    line(1),
    column(0),
    tree(nullptr),
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

void Tokenizer::read_buf()
{
    if (buffer != nullptr) 
    { 
        delete buffer;
        buflen = 0;
        bufpos = 0;
    }
    ifstream fin;
    fin.open(path, ios::binary);
    if (!fin.is_open())
    {
        Errlog(FileName, format("could not open file: {}", path));
        buffer = new char[1] {'\0'};
        return;
    }
    size_t starter = fin.get() == 0xEF && fin.get() == 0xBB && fin.get() == 0xBF ? 3 : 0; // remove BOM
    filebuf* fbuf = fin.rdbuf();
    buflen = (size_t)fbuf->pubseekoff(0, ios::end, ios::in) - starter;
    buffer = new char[buflen];
    fbuf->pubseekpos(starter, ios::in);
    fbuf->sgetn(buffer, buflen);
    fin.close();
    buffer[buflen] = '\0';
}

void Tokenizer::parse()
{
    read_buf();
    tree = new ParseTree();
    while (bufpos <= buflen)
    {
        if (compose(buffer[bufpos]))
        {
            //elm.get();
            auto _tree = tree->parse(elm);  
            if (_tree == nullptr) // main-tree finish, go to next main-tree
            {
                cache_list();
                delete tree;
                tree = new ParseTree();
            }
            else { tree = _tree; }
        }
    }
    del_tree();
}

void Tokenizer::cache_list()
{
    pToken _t = tree->once_get();
    if (_t == nullptr) { return; }
    tokens.push_back(_t);
}

bool Tokenizer::compose(char& ch)
{
    // if tree built successfully (has gone to "return" node), 
                                        // elm didn't be used, should pass to next tree
    if (elm) { return true; }
    // some delimiters will bring to next loop 
                        // that won't use fin.get() to loop one more time to update state
    /*char ch = fin.peek(); */
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
            elm(token.str(), line, column);   // will delete within process of tree->parse(...) when 
                                                            // not send to any Token,
                                                            // or will delete in Token::_vol_(...)
            state = None;
            return true;
        }
        else if (ch == endline)
        {
            token << quote;
            elm(token.str(), line, column);
            state = None;
            return true;
        }
        token << fget();
        return false;
    case Escape_quo:
        if (ch == endline)
        {
            token << quote << quote;
            elm(token.str(), line, column);
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
            elm(token.str(), line, column);
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
            elm(fget(), line, column); // same as eToken above ^
            return true;
        }
        else if (ch == blank)
        {
            if (ch == '\n')
            {
                line++;
                column = 0;
            }
            else if (ch == '\t')
            {
                column += 3;
            }
            fget();
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
    char c = buffer[bufpos];
    bufpos++;
    column++;
    return c;

}

void Tokenizer::del_tree()
{
    if (tree->get_from() != nullptr)
    {
        Warnlog(FileName, format("interruption at line({}), column({})", line, column));
        tree->get_from()->append(tree->once_get());
        pTree _tree = tree->get_from();
        delete tree;
        tree = _tree;
        while (tree->get_from() != nullptr)
        {
            tree->get_from()->append(tree->once_get());
            _tree = tree->get_from();
            delete tree;
            tree = _tree;
        }
    }
    // since tree->once_get() only can get its build once
                // here try to cache one more time for interruption case
    cache_list();
    delete tree;
    tree = nullptr;
}
