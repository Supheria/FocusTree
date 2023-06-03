#ifndef _ELEMENT_H
#define _ELEMENT_H

#include <string>

typedef std::string Value;
typedef const std::string* pcValue;

//typedef struct Element* pElement;
struct Element
{
private:
	size_t ln;
	size_t col;
	pcValue val;
	bool own_val;
public:
	Element() :
		own_val(false),
		ln(0),
		col(0),
		val(nullptr)
	{
	}
	// for Tokenizer::marker
	Element(const char& marker, const size_t& line, const size_t& column) :
		own_val(true),
		ln(line),
		col(column),
		val(new std::string(1, marker))	// new string finally may delete in any Type of Token's distributer,
										// or may delete within process of parse() in ParseTree
	{
	}
	// for Tokenizer::token
	Element(const std::string& token, const size_t& line, const size_t& end_column) :
		own_val(true),
		ln(line),
		col(end_column - token.length()),
		val(new std::string(token)) // same as above ^
	{
	}
	// will call (*p_e)->get() that transfers ownership of value, 
	// and will DELETE (*p_e) and set it to nullptr 
	Element(Element& _e) :
		own_val(true),
		ln(_e.line()),
		col(_e.column()),
		val(_e.get()) // same as above ^
	{
	}
	~Element()
	{
		if (own_val) { delete val; }
	}
	void operator()(const char& marker, const size_t& line, const size_t& column)
	{
		ln = line;
		if (own_val) { delete val; }
		else { own_val = true; }
		val = new std::string(1, marker);
		col = column;
		
	}
	void operator()(const std::string& token, const size_t& line, const size_t& column)
	{
		ln = line;
		if (own_val) { delete val; }
		else { own_val = true; }
		val = new std::string(token);
		col = column;
	}
	const size_t& line()
	{
		return ln;
	}
	const size_t& column()
	{
		return col;
	}
	// the first char to compare with 
	const char& head()
	{
		return (*val)[0];
	}
	const Value& value()
	{
		return *val;
	}
	// if called for any time, ownership of pointer to val will lose, and ~Element() won't delete val
	pcValue get()
	{
		own_val = false;
		return val;
	}
	operator bool()
	{
		return own_val;
	}
};

#endif // !_ELEMENT_H

