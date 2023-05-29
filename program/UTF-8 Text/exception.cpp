#include "exception.h"

#include <sstream>

ErrorLog ErrLog = ErrorLog();
WarningLog WarnLog = WarningLog();

using namespace std;

ExceptionLog::ExceptionLog() :
	logpath("parse.log")
{
	fout.open(logpath, ios::ate);
	fout.close();
}

void ExceptionLog::append(std::string msg)
{
	fout.open(logpath, ios::app);
	fout << msg << endl;
	fout.close();
}

ErrorLog::ErrorLog()
{
}

void ErrorLog::operator()(const char* filename, const char* message)
{
	stringstream ss;
	// format error log
	append(ss.str());
}

WarningLog::WarningLog()
{
}

void WarningLog::operator()(const char* filename, const char* message)
{
	stringstream ss;
	// format warning log
	append(ss.str());
}
