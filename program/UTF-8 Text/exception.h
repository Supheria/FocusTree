#ifndef _EXCEPTION_H
#define _EXCEPTION_H

#include <exception>
#include <string>

class ErroExc : public std::exception
{
public:
	ErroExc(std::string filename, std::string message)
	{

	}
};

class WarnExc : public std::exception
{
public:
	WarnExc(std::string filename, std::string message)
	{

	}
};

#endif // !_EXCEPTION_H
