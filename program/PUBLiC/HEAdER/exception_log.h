#ifndef _EXCEPTION_LOG_H
#define _EXCEPTION_LOG_H

namespace hoi4
{
	typedef class ExceptionLog
	{
	public:
		enum ExT
		{
			ERR,
			WRN
		};
	private:
		const char* path;
		void append(const char* msg);
	public:
		ExceptionLog();
		void operator()(const char* fname_no_ex, const char* message, ExT type);
		void reset();
	} ExLog;
}

extern hoi4::ExLog Logger;

#endif // ! _EXCEPTION_LOG_H



