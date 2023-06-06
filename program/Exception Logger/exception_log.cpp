#include "exception_log.h"
#include <fstream>
#include <format>
#include <chrono>

using namespace std;
using namespace chrono;

ExceptionLog Logger;

ofstream flog;

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
}

void ExceptionLog::append(const char* fname_no_ex, const char* message, ExT type)
{
	flog.open(path, ios::app);
	if (!flog.is_open()) { return; }

	string msg;
	switch (type)
	{
	case ERR:
		msg = format("[{}.cpp[{}]] error: {}", fname_no_ex, get_time(), message).c_str();
		break;
	case WRN:
		msg = format("[{}.cpp[{}]] warning: {}", fname_no_ex, get_time(), message).c_str();
		break;
	}
	flog << msg << endl;
	flog.close();
}

void ExceptionLog::reset()
{
	flog.open(path, ios::ate);
	flog.close();
}