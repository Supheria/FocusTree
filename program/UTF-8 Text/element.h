#ifndef _ELEMENT_H
#define _ELEMENT_H

#include<string>

class Element
{
	const size_t* const ln;
	const size_t* const col;
	// if get() was called, will set to true, and ~Element() won't delete[] val.
	mutable bool lose_val;
	// as new char[], must use delete[]
	const char* const val;
public:
	// for marker
	Element(const char marker, const size_t& line, const size_t& column);
	// for token
	Element(const std::string& token, const size_t& line, const size_t& end_column);
	~Element();
private:
	const char* _str(const std::string& _s);
public:
	const size_t& line() const;
	const size_t& column() const;
	// use to compare with
	const char& head() const;
	// will get const char* = new char[], must use delete[]
	// if get() was called any time, ~Element() won't delete[] val.
	const char* get() const;
	// when element would be abandoned, and its value won't be used,
							// normally when ParseTree::parse() done with using an element, but
							// former won't pass latter's value to ParseTree::build, such as a 
							// Marker of open brace, its del() should be called.
							// for some exception conditions, parse() calls 
							// ParseTree::fail_to_build(), current element passed to parse() 
							// should also be called for del().
							// for those markers or tokens passed to ParseTree::build, they will 
							// be delete[] in Token::~Token(), normally of pointer in token map.

};

#endif