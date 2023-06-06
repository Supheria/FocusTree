#ifndef _EXCEPTION_LOG_H_
#define _EXCEPTION_LOG_H_

#include "ex_log_api.h"

using namespace hoi4;

class ExceptionLog : public ExLog
{
private:
	const char* path;
public:
	ExceptionLog();
	void append(const char* fname_no_ex, const char* message, hoi4::ExLog::ExT type);
	void reset();
};

extern ExceptionLog logger;

#endif // ! _EXCEPTION_LOG_H_