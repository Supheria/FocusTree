[提取结果](./three-letter contry tag/three_letter_contry_tag.md)

> There are some country tags that must be avoided, as they will cause in-game confusion whether something is meant to be a reference to a country or not. Their mere existence can cause other code
> to break. A list of them is the following:



> 看起来只需要替换/覆盖 history\countries 文件夹下的国家文件就能替换国家，而不需要覆盖 common\country_tags 文件夹的文件
>
> + ~~在主mode的 descriptor.mod 里没有找到 replace_path="common\country_tags" 语句~~
>
> + wiki原文：
>
>   > Create a file in the <yourmod>/history/countries folder.
>   >
>   > ...
>   >
>   > The important aspect is that the first three letters of the filename must be the same as the country's tag in order to be loaded, without taking capitalisation into account. 
>   >
>   > [Country creation: #Country history](../pdf/Country creation - Hearts of Iron 4 Wiki.pdf)
>
>   换句话说，如果原游戏在的 history/countries 下找不到三字母标签对应的文件就不会/不能加载国家了。
>
> + 把原游戏的此文件夹删了，发现游戏无法读取国家本地化信息，但剧本开始前的国家列表已然存在。*恢复*先前操作，又把 common\country_tags 删了，剧本中的国家列表已为空，只有空的 “other country”。*保留*先前操作，打上主mode，进入游戏发现可以正常打开mode剧本中的国家列表
>
>   > 这证伪了先前的推测，~~但为什么 descriptor.mod 里没有找到对 common\country_tags的替换语句呢~~
>
> + 又仔细在 descriptor.mod  找了一下，发现有如下语句：
>
>   > replace_path="common/country_tags"
>   >
>   > replace_path="common/countries"
>   >
>   > replace_path="history/countries"
>
>   这三条应该就是有关新建国家的全部了，其中 common/country_tags 记录了三字母标签，*作为国家信息也就是 history/countries 的入口点*，而关于 common/countries Wiki原文：
>
>   > color = { 2 10 222 } is the country's default colour on the political map mode (overridden by a colors.txt entry if one exists. Commonly, this doesn't work, making the colors.txt entry mandatory).
>   >
>   > Additionally, an entry can be made in <yourmod>/common/countries/colors.txt in order to specify more country colours. The file has to be called colors.txt, and so it will overwrite the base
>   > game file if the entry is added. An entry in the file is formatted as the following:
>   > SCO = {
>   > color = rgb { 2 10 222 }
>   > color_ui = rgb { 255 255 155 }
>   > }
>   > color will overwrite the country's colour on the political map...
>   >
>   > [Country creation: #Country file](../pdf/Country creation - Hearts of Iron 4 Wiki.pdf)
>
>   + 发现一个问题，在 history\countries 中可以找到对应 ENG 的文件名：ENG - Britain.txt，而在 common\country_tags\00_countries.txt 里找到的是 ENG = "countries/United Kingdom.txt"，说明 country_tags 里记录的标签可能history\countries 的*直接*入口点。但是打开 common\countries\United Kingdom.txt 其中只有 color 相关信息，没有与 history\countries 相关的任何信息。而在 common\countries\colors.txt 中也只找到 color 相关信息。
>
>   根据上面一段Wiki的说法，colors.txt 是对 common\countries\xx.txt 文件中 color 代码块的重写，
>
>   ```CSS
>   //
>   // common\countries\United Kingdom.txt
>   //
>   color = { 201  56  93 }
>   
>   //
>   // common\countries\colors.txt
>   //
>   ENG = {
>   	color = rgb { 201 56 93 }
>   	color_ui = rgb { 255 73 121 }
>   }
>   ```
>
>   发现 ENG 的两处 color 数值都是 { 201 56 93 }，对比 GER
>
>   ```CSS
>   //
>   // common\country_tags\00_countries.txt
>   //
>   GER	= "countries/Germany.txt"
>   
>   //
>   // common\countries\Germany.txt
>   //
>   color = { 106  119  89 } # { 76  97  121 }
>   
>   //
>   // common\countries\colors.txt
>   //
>   GER = {
>   	color = HSV { 0.1 0.15 0.4 }
>   	color_ui = rgb { 138 155 116 }
>   }
>   ```
>
>   发现 GER 的 color 在 colors.txt 中用HSV重写了。关于HSV，WIki原文：
>
>   > Colors may also be specified in the HSV color model, in which case rgb will need to be replaced with hsv.
>
>   在 color.txt 开头有一行注释
>
>   ```css
>   //
>   // common\countries\colors.txt
>   //
>   #reload countrycolors
>   ```
>
>   不知道钢4这样覆盖一层的用意是什么。
>
>   尝试把 common\countries 文件夹移除后，选择剧本和国家后，进入地图不再显示地块的颜色划分。顺带一提，动态加载需要文件在游戏启动前就存在，Wiki原文：
>
>   > This only applies to files that existed when the game was launched, with an exception: if a file's direct path gets mentioned elsewhere within the mod, then it can still get loaded for that use in particular.
>   >
>   > [Modding: #Debug advantages](../pdf/Modding - Hearts of Iron 4 Wiki.pdf)
>
>   不过 common\countries\xx.txt 的路径在 country_tags 是提到过的，启动游戏后恢复文件夹地图仍不显示色块，不知道为什么没有符合原文中的例外情况。对主mod尝试了相同的操作同样如此。开启调试模式，恢复文件夹，调用控制台命令
>
>   ```
>   reloadinterface
>   ```
>
>   等reload类型命令，地图仍不显示色块。
>
>   回到原游戏，尝试让 common\countries 只剩下 colors.txt，启动游戏后地图可以正常显示色块。
>
>   > 那么是否意味着，只需要替换并重写 colors.txt 就可以规定国家的地图划分着色
>
>   有关 color.txt 的意义Wiki原文：
>
>   > The file has to be called colors.txt, and so it will overwrite the base
>   > game file if the entry is added. 
>   >
>   > [Country creation: #Country file](../pdf/Country creation - Hearts of Iron 4 Wiki.pdf)
>
>   看起来是给dlc或mod及子mod用的，旨在要重新着色已有国家而不改变着色方式时使用。
>
> + 着色方式是能在*非* color.txt 中定义，有关着色方式Wiki
>
>   > Graphical culture defines the set of graphics used for units, while the 2D graphical culture defines portraits used for aces and generic advisors. A list can be found in /Hearts of Iron IV/common/graphicalculturetype.txt.
>   >
>   > [Country creation: #Country file](typora://app/pdf/Country creation - Hearts of Iron 4 Wiki.pdf)
>
>   在Hearts of Iron IV\文件夹下查找包含 common/graphicalculturetype.txt 中各个字段的文件名、文件夹名、文本***（需要用 parser 解析 common\countries 下包含各个字段的国家名）***
>
>   + western_european_gfx
>
>     > 文件夹名：0
>     >
>     > 文件名：0
>
>   + 
>
>   
>
>   
