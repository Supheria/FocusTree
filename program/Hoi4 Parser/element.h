#ifndef _ELEMENT_H
#define _ELEMENT_H

#include <string>

typedef std::string Value;
typedef const std::string* pcValue;

typedef struct Element* pElement;
struct Element
{
private:
	const size_t ln;
	const size_t col;
	pcValue const val;
	mutable bool lose_val;
public:
	// for Tokenizer::marker
	Element(const char& marker, const size_t& line, const size_t& column) :
		lose_val(false),
		ln(line),
		col(column),
		val(new std::string(1, marker))	// new string finally may delete in any Type of Token's distributer,
										// or may delete within process of parse() in ParseTree
	{
	}
	// for Tokenizer::token
	Element(const std::string& token, const size_t& line, const size_t& end_column) :
		lose_val(false),
		ln(line),
		col(end_column - token.length()),
		val(new std::string(token)) // same as above ^
	{
	}
	// will call (*p_e)->get() that transfers ownership of value, 
	// and will DELETE (*p_e) and set it to nullptr 
	Element(pElement* p_e) :
		lose_val(false),
		ln((*p_e)->line()),
		col((*p_e)->column()),
		val((*p_e)->get()) // same as above ^
	{
		delete (*p_e);
		(*p_e) = nullptr;
	}
	~Element()
	{
		if (!lose_val) { delete val; }
	}
	const size_t& line() const
	{
		return ln;
	}
	const size_t& column() const
	{
		return col;
	}
	// the first char to compare with 
	const char& head() const
	{
		return (*val)[0];
	}
	const Value& value() const
	{
		return *val;
	}
	// if called for any time, ownership of pointer to val will lose, and ~Element() won't delete val
	pcValue get() const
	{
		lose_val = true;
		return val;
	}
};

#endif // !_ELEMENT_H

