#ifndef _HOI4_EX_LOG_API_H_
#define _HOI4_EX_LOG_API_H_

namespace hoi4
{
	struct ExLog
	{
	public:
		enum ExT
		{
			ERR,
			WRN
		};
	public:
		virtual void append(const char* fname_no_ex, const char* message, ExT type) = 0;
		virtual void reset() = 0;
	};
}

#ifdef IMPORT_HOI4_EX_LOG
#define HOI4_EX_LOG_API extern "C" __declspec(dllimport)
#else
#define HOI4_EX_LOG_API extern "C" __declspec(dllexport)
#endif

HOI4_EX_LOG_API hoi4::ExLog* ex_log();

#endif // !_HOI4_EX_LOG_API_H_