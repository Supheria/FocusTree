Wiki原文如下：
> The errors related to the colouring characters can be fairly unintuitive to find, considering that they do not provide the location of the file.
There are two types of the error:
Could not find coloring for character 'M'......
Could not find coloring for character id '17' – Note that it specifies the character ID. In this case, the printable Unicode character ID is provided. 
This is typically done where providing the actual character would be confusing 

> (e.g. for the number "1", the game would specify the character id "17" if such a colour doesn't exist. Since "1" is the 18th printed character, it has the id of 17, as
the numeration typically starts from 0.)

这里把字符 '1' 的Unicode编码值说成17很奇怪，我测试了字符 '1' 的Unicode编码，甚至还测试了UTF-8和Ascii的，都是(byte)49。

我尝试首先删除interface\core.gfx里的bitmapfonts.textcolors块，开原版游戏测试后log显示最多的是：
> [bitmapfont.cpp:2850]: Could not find coloring for character 'L'

于是我尝试将所有在Localization文件夹下的 §L 替换为 §1，但 log 内容并没有改变。可能是 bitmapfont 用于定义字符颜色的脚本不在 Localization 里。
有关字符 '1' 在此处的编码值问题还待以后测试。[5月12日]

在彻底替换Localization文件夹下所有文件中的§类字符为 §1 或 §2 后，error.log最终出现了如下信息：
> [05:09:13][bitmapfont.cpp:2850]: Could not find coloring for character '1'

结果很明显，当前游戏版本（v1.12.12.456f(a266)）已使用了新的log格式。至于先前提出的关于字符'1'的编码值问题，由于不影响目前游戏版本的erro log，因此不再做考究。[5月13日]