#include "exception_log.h"
#include <fstream>
#include <format>
#include <chrono>

using namespace std;
using namespace chrono;
using namespace hoi4;

ExLog Logger;

fstream flog;

inline std::string get_time()
{
	typedef system_clock cl;
	stringstream ss;
	time_t t = cl::to_time_t(cl::now());
	tm now;
	localtime_s(&now, &t);
	ss << put_time(&now, "%H:%M:%S");
	return ss.str();
}

ExceptionLog::ExceptionLog() :
	path("ex.log")
{
	flog.open(path, ios::ate);
	flog.close();
}

void ExceptionLog::operator()(const char* fname_no_ex, const char* message, ExT type)
{
	switch (type)
	{
	case ERR:
		append(format("[{}.cpp[{}]] error: {}", fname_no_ex, get_time(), message).c_str());
		break;
	case WRN:
		append(format("[{}.cpp[{}]] warning: {}", fname_no_ex, get_time(), message).c_str());
		break;
	}
}

void ExceptionLog::reset()
{
	flog.open(path, ios::ate);
	flog.close();
}

void ExceptionLog::append(const char* msg)
{
	flog.open(path, ios::app);
	if (flog.is_open())
	{
		flog << msg << endl;
		flog.close();
	}
}