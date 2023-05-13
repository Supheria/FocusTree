#include <fstream>
#include <string>
#include <codecvt>
#include <iostream>
#include "Hoi4Parser.h"

int main()
{
	using namespace std;
	std::wstring str = L"123,abc:我是谁！";

	std::wstring_convert<std::codecvt_utf8<wchar_t>> conv;

	std::string narrowStr = conv.to_bytes(str);
	{
		std::ofstream ofs("d:\\test.txt");			//文件是utf8编码
		ofs << narrowStr;
	}

	std::ifstream ifs("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\modding analysis\\test\\germany.txt");
	while (!ifs.eof())
	{
		string line;
		getline(ifs, line);
		wstring wb = conv.from_bytes(line);
		//cout.imbue(locale("chs"));			//更改区域设置 只为控制台输出显示
		wcout << wb << endl;
	}
}