#ifndef _ELEMENT_H
#define _ELEMENT_H

#include<string>

struct Element
{
protected:
	const size_t* line;
	const size_t* column;
public:
	virtual const char& get() const = 0; // use const behind for const Element* elm pointer to call
	virtual void del() const = 0; // when element would be abandoned, and its value won't be used,
							// normally when ParseTree::parse() done with using an element, but
							// former won't pass latter's value to ParseTree::build, such as a 
							// Marker of open brace, its del() will be called after parse()
							// updated to new step. 
							// for some exception conditions, parse() calls 
							// ParseTree::fail_to_build(), current element passed to parse() 
							// should also be called for del().
							// for those signs or tokens passed to ParseTree::build, they will 
							// be delete in Token::~Token(), normally of pointer in token map.

	virtual Marker* a(Marker*) = 0;
};

struct Marker : public Element
{
private:
	const char* sign;
public:
	Marker(const char* dy_c, const size_t* dy_ln, const size_t* dy_col) // dy_ means to dynamically allocate memory
	{ 
		sign = dy_c;
		line = dy_ln;
		column = dy_col;
	}
	const char& get() const { return (*sign); }
	void del() const
	{
		if (sign != nullptr) { delete sign; }
		if (line != nullptr) { delete line; }
		if (column != nullptr) { delete column; }
	}
	Element* a(Element*) {}
};

struct eToken : public Element
{
private:
	const std::string* token;
public:
	eToken(const std::string* dy_s, const size_t* dy_ln, const size_t _end)
	{ 
		token = dy_s;
		line = dy_ln;
		column = new size_t(_end - (*dy_s).length() + 1);
	}
	const char& get() const { return (*token)[0]; }
	const std::string* get_token() const { return token; }
	void del() const
	{
		if (token != nullptr) { delete token; }
		if (line != nullptr) { delete line; }
		if (column != nullptr) { delete column; }
	}
};

#endif