#ifndef _ELEMENT_H
#define _ELEMENT_H

#include <string>

typedef const std::string* pcValue;

typedef struct Element
{
private:
	const size_t ln;
	const size_t col;
	mutable bool lose_val;
	pcValue val;
public:
	// for marker
	Element(const char& marker, const size_t& line, const size_t& column) :
		lose_val(false),
		ln(line),
		col(column),
		val(new std::string(1, marker))	// new string finally may delete in any Type of Token's distributer, flow as:
		//      new here => tree->build => ~ValueKey() somewhere in token map
		// or may delete within process of parse() by using this->del(), flow as:
		//      new here => tree->parse(...) => this->del()
	{
	}
	// for token
	Element(const std::string& token, const size_t& line, const size_t& end_column) :
		lose_val(false),
		ln(line),
		col(end_column - token.length()),
		val(new std::string(token)) // same as above ^
	{
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
	// use to compare with the first char
	const char& head() const
	{
		return (*val)[0];
	}
	// if called for any time, ownership of pointer to val will lose, and ~Element() won't delete val
	pcValue get() const
	{
		lose_val = true;
		return val;
	}
	const std::string& volume() const
	{
		return *val;
	}
} *pElement;

#endif // !_ELEMENT_H

