#ifndef _ELEMENT_H
#define _ELEMENT_H

#include<string>

struct Element
{
protected:
	size_t pos = 0;
public:
	virtual const char& get() = 0;
};

struct Marker : public Element
{
private:
	char sign;
public:
	Marker(char ch, size_t _pos) 
	{ 
		sign = ch; 
		pos = _pos;
	}
	const char& get() { return sign; }
};

struct eToken : public Element
{
private:
	const std::string* token;
public:
	eToken(const std::string* _s, size_t _end)
	{ 
		token = _s;
		pos = _end - (*_s).length() + 1;
	}
	const char& get() { return (*token)[0]; }
	const std::string* get_token() { return token; }
};

#endif