#include "tokenizer.h"
#include "use_ex_log.h"
#include <format>
#include <fstream>

using namespace std;
using namespace hoi4::parser;

const Tokenizer::CompareChar Tokenizer::delimiter({ '\t', ' ', '\n', '\r', '#', '=', '>', '<', '}', '{', '"', '\0'});
const Tokenizer::CompareChar Tokenizer::blank({ '\t', ' ', '\n', '\r', '\0' });
const Tokenizer::CompareChar Tokenizer::endline({ '\n', '\r', '\0' });
const Tokenizer::CompareChar Tokenizer::marker({ '=', '>', '<', '}', '{' });
const char Tokenizer::note = '#';
const char Tokenizer::quote = '"';
const char Tokenizer::escape = '\\';

const char* fn_tkn = "tokenizer";

Tokenizer::Tokenizer(const char* filepath, token_list& tokens) :
    buffer(nullptr),
    buflen(0),
    bufpos(0),
    line(1),
    column(0),
    tree(nullptr),
    state(None),
    tokens(tokens)
{
    read_buf(filepath);
    tree = new ParseTree();
    while (bufpos <= buflen)
    {
        if (compose(buffer.get()[bufpos]))
        {
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

void Tokenizer::read_buf(const char* path)
{
    buflen = 0;
    bufpos = 0;
    ifstream fin;
    fin.open(path, ios::binary);
    if (!fin.is_open())
    {
        ex_log()->append(fn_tkn, format("could not open file: {}", path).c_str(), ExLog::ERR);
        buffer.reset(new char[1] {'\0'});
        return;
    }
    size_t starter = fin.get() == 0xEF && fin.get() == 0xBB && fin.get() == 0xBF ? 3 : 0; // remove BOM
    filebuf* fbuf = fin.rdbuf();
    buflen = (size_t)fbuf->pubseekoff(0, ios::end, ios::in) - starter;
    buffer.reset(new char[buflen + 1]);
    fbuf->pubseekpos(starter, ios::in);
    fbuf->sgetn(buffer.get(), buflen);
    fin.close();
    buffer.get()[buflen] = '\0';
}

void Tokenizer::cache_list()
{
    pToken _t = tree->once_get();
    if (_t == nullptr) { return; }
    tokens.push_back(ptok_u(_t));
}

bool Tokenizer::compose(char& ch)
{
    // while elm has been used its value will to be nullptr
                            // if elm didn't be used should pass it to next tree
    if (elm) { return true; }
    // some delimiters will bring to next loop 
                        // that won't use fget() to loop one more time to update state
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
            elm(token.str().c_str(), token.str().length(), line, column);   // will delete within process of tree->parse(...) when 
                                                            // not send to any Token,
                                                            // or will delete in Token::_vol_(...)
            state = None;
            return true;
        }
        else if (ch == endline)
        {
            token << quote;
            elm(token.str().c_str(), token.str().length(), line, column);
            state = None;
            return true;
        }
        token << fget();
        return false;
    case Escape_quo:
        if (ch == endline)
        {
            token << quote << quote;
            elm(token.str().c_str(), token.str().length(), line, column);
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
            elm(token.str().c_str(), token.str().length(), line, column);
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
    char c = buffer.get()[bufpos];
    bufpos++;
    column++;
    return c;

}

void Tokenizer::del_tree()
{
    if (tree->get_from() != nullptr)
    {
        ex_log()->append(fn_tkn, format("interruption at line({}), column({})", line, column).c_str(), ExLog::WRN);
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
