#include "exception_log.h"
#include <format>
#include <chrono>

ErrorLog Errlog = ErrorLog();
WarningLog Warnlog = WarningLog();

using namespace std;
using namespace chrono;

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

std::string ExceptionLog::get_time()
{
	typedef system_clock cl;
	stringstream ss;
	time_t t = cl::to_time_t(cl::now());
	tm now;
	localtime_s(&now, &t);
	ss << put_time(&now, "%H:%M:%S");
	return ss.str();
}

ErrorLog::ErrorLog()
{
}

void ErrorLog::operator()(const std::string fname_no_ex, const std::string message)
{
	append(format("[{}.cpp[{}]] error: {}", fname_no_ex, get_time(), message));
}

WarningLog::WarningLog()
{
}

void WarningLog::operator()(const std::string fname_no_ex, const std::string message)
{
	append(format("[{}.cpp[{}]] warning: {}", fname_no_ex, get_time(), message));
}