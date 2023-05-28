#ifndef _EXCEPTION_H
#define _EXCEPTION_H

#include <string>

class ErrorLog
{
public:
	enum T
	{
		ERRO,
		WARN
	};
public:
	ErrorLog();
	void append(std::string filename, std::string message, T type);
	void operator()(std::string filename, std::string message, T type);
} extern errlog;

#endif // !_EXCEPTION_H
