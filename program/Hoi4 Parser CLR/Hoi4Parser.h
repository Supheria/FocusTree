#pragma once

#include "..\Hoi4 Parser\NativeParserDll.h"

//#pragma comment(lib,"CDll1.lib")

ref class Hoi4Parser
{
public:
	Hoi4Parser();
	~Hoi4Parser();
	const token_list& parse(std::string filepath);
	const token_list& get();
	const log_list& get_logger();
};

