#ifndef _EXCEPTION_H
#define _EXCEPTION_H

#include <fstream>

class ExceptionLog
{
private:
	const char* logpath;
	std::ofstream fout;
protected:
	ExceptionLog();
	void append(std::string msg);
};

class ErrorLog : public ExceptionLog
{
public:
	ErrorLog();
	void operator()(const char* filename, const char* message);
} extern ErrLog;

class WarningLog : public ExceptionLog
{
public:
	WarningLog();
	void operator()(const char* filename, const char* message);
} extern WarnLog;

#endif // !_EXCEPTION_H
