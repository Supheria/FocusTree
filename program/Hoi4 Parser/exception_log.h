#ifndef _EXCEPTION_LOG_H
#define _EXCEPTION_LOG_H

#include <fstream>
#include <format>

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
	// file name without extension
	void operator()(const std::string fname_no_ex, const std::string message);
} extern Errlog;

class WarningLog : public ExceptionLog
{
public:
	WarningLog();
	// file name without extension
	void operator()(const std::string fname_no_ex, const std::string message);
} extern Warnlog;

#endif // ! _EXCEPTION_LOG_H

