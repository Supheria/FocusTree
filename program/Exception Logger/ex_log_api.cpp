#include "exception_log.h"

using namespace hoi4;

extern ExceptionLog Logger;

hoi4::ExLog* ex_log()
{
	return &Logger;
}