#ifndef _EXCEPTION_LOG_H
#define _EXCEPTION_LOG_H

#include <fstream>

class ExceptionLog
{
private:
	const std::string logpath;
	std::ofstream fout;
protected:
	ExceptionLog();
	void append(std::string msg);
};

class ErrorLog : public ExceptionLog
{
public:
	ErrorLog();
	void operator()(const std::string filename, const std::string message);
} extern ErrLog;

class WarningLog : public ExceptionLog
{
public:
	WarningLog();
	void operator()(const std::string filename, const std::string message);
} extern WarnLog;

#endif // ! _EXCEPTION_LOG_H

