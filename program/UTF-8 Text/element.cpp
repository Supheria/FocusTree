#include "element.h"

//Element::Element(const char marker, const size_t line, const size_t column) :
//	lose_val(false),
//	ln(new size_t(line)),
//	col(new size_t(column)),
//	val(new char[2] {marker, 0})	// new char[] finally may delete in any Type of Token's distributer, flow as:
//									//      new here => tree->build => ~ValueKey() somewhere in token map
//									// or may delete within process of parse() by using this->del(), flow as:
//									//      new here => tree->parse(...) => this->del()
//{
//}

using namespace std;

Element::Element(const char& marker, const size_t& line, const size_t& column) :
	val(make_unique<string>(1, marker)),
	ln(line),
	col(column)
{
}

Element::Element(const std::string& token, const size_t& line, const size_t& end_column) :
	val(make_unique<string>(token)),
	ln(line),
	col(end_column - token.length() + 1)
{
}

Element::~Element()
{
}

const size_t& Element::line() const
{ 
	return ln;
}

const size_t& Element::column() const
{
	return col; 
}

const char& Element::head() const
{
	return (*val)[0];
}

std::shared_ptr<char> Element::get() const
{
	return move(shared_ptr<char>());
}

