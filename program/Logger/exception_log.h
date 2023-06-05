#ifndef _EXCEPTION_LOG_H
#define _EXCEPTION_LOG_H

namespace hoi4
{
	extern "C" class __declspec(dllexport)  ExceptionLog
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
	};
}

#endif // ! _EXCEPTION_LOG_H


#ifdef IMPORT_LOGGER
#define LOG_DLL extern "C" __declspec(dllimport)
#else
#define LOG_DLL extern "C" __declspec(dllexport)
#endif

LOG_DLL hoi4::ExceptionLog Logger;