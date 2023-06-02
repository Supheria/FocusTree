#include "exception_log.h"
#include <sstream>

ErrorLog Errlog = ErrorLog();
WarningLog Warnlog = WarningLog();

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

void ErrorLog::operator()(const std::string fname_no_ex, const std::string message)
{
	stringstream ss;
	// format error log
	append(ss.str());
}

WarningLog::WarningLog()
{
}

void WarningLog::operator()(const std::string fname_no_ex, const std::string message)
{
	stringstream ss;
	// format warning log
	append(ss.str());
}
