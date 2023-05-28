#include "exception.h"

ErrorLog errlog = ErrorLog();

ErrorLog::ErrorLog()
{
}

void ErrorLog::append(std::string filename, std::string message, T type)
{
}

void ErrorLog::operator()(std::string filename, std::string message, T type)
{
	append(filename, message, type);
}
