#ifndef _ELEMENT_H
#define _ELEMENT_H

#include <string>
#include <memory>

class Element
{
protected:
	const std::unique_ptr<size_t> ln;
	const std::unique_ptr<size_t> col;
public:
	Element(const size_t _ln, const size_t _col) :
		ln(new size_t(_ln)),
		col(new size_t(_col))
	{
	}
	const size_t& line() { return *ln; }
	const size_t& column() { return *col; }
	virtual const char& head() = 0;
	virtual void* const get() = 0;
	// when element would be abandoned, and its value won't be used,
							// normally when ParseTree::parse() done with using an element, but
							// former won't pass latter's value to ParseTree::build, such as a 
							// Marker of open brace, its del() will be called after parse()
							// updated to new step. 
							// for some exception conditions, parse() calls 
							// ParseTree::fail_to_build(), current element passed to parse() 
							// should also be called for del().
							// for those signs or tokens passed to ParseTree::build, they will 
							// be delete in Token::~Token(), normally of pointer in token map.
	virtual void del() = 0;
};

class Marker : public Element
{
private:
	char* const sg;
public:
	Marker(const char sign, const size_t line, const size_t column)
		: Element(line, column),
		sg(new char(sign))	// new char finally may delete in ~ValueKey(), flow as:
							//      new here => tree->build => ~ValueKey() somewhere in token map
							// or may delete within process of parse() by using this->del(), flow as:
							//      new here => tree->parse(...) => this->del()
	{
	}
	const char& head() { return *sg; }
	// char* 
	void* const get() { return sg; }
	void del()
	{
		if (sg != nullptr) { delete sg; }
	}
};

class eToken : public Element
{
private:
	std::string* const tok;
public:
	eToken(const std::string token, const size_t line, const size_t end_column)
		: Element(line, end_column - token.length() + 1),
		tok(new std::string(token))	// new string finally may delete in any Type of Token's distributer, flow as:
									//      new here => tree->build => ~TokenType() somewhere in token map
									// or may delete within process of parse() by using this->del(), flow as:
									//      new here => tree->parse(...) => this->del()
	{
	}
	const char& head() { return (*tok)[0]; }
	// const string*
	void* const get() { return tok; }
	void del()
	{
		if (tok != nullptr) { delete tok; }
	}
};

#endif