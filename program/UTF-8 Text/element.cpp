#include "element.h"

Element::Element(const char marker, const size_t& line, const size_t& column) :
	lose_val(false),
	ln(new size_t(line)),
	col(new size_t(column)),
	val(new char[2] {marker, 0})	// new char[] finally may delete in any Type of Token's distributer, flow as:
									//      new here => tree->build => ~ValueKey() somewhere in token map
									// or may delete within process of parse() by using this->del(), flow as:
									//      new here => tree->parse(...) => this->del()
{
}

Element::Element(const std::string& token, const size_t& line, const size_t& end_column) :
	lose_val(false),
	ln(new size_t(line)),
	col(new size_t(end_column)),
	val(_str(token))
{
}

Element::~Element()
{
	if (ln != nullptr) { delete ln; }
	if (col != nullptr) { delete col; }
	if (lose_val || val == nullptr) { return; }
	delete[] val;
}

const char* Element::_str(const std::string& _s)
{
	char* s = new char[_s.length() + 1] {0};	// same as above ^
	for (int i = 0; i < _s.length(); i++)
	{
		s[i] = _s[i];
	}
	return s;
}

const size_t& Element::line() const
{ 
	return *ln;
}

const size_t& Element::column() const
{
	return *col; 
}

const char& Element::head() const
{
	return *val;
}

const char* Element::get() const
{
	lose_val = true;
	return val;
}
