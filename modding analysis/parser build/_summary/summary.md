
### Read \*.txt, \*.gfx, \*.gui Under "Hearts of Iron IV\\" Using Basic Syntax for Test

>+ Hoi4 Original Game Version 1.12.12(13)
>+ the number of \*.txt is *3693*, \*.gfx is *294*, \*.gui is *414*
>+ [raw exception log](../_test/exception%20log.txt)


<span id="top"></span>

[toc]


#### <font color=SandyBrown>Basic Syntax</font>
> *Also **Tokenize Rule***
>1. read script file byte by byte, if meet '#' skip rest of the line
>2. *<font color=CornflowerBlue>[loop]</font>* collect bytes for token while next byte is not *blank\**, then skip any *blank* to read next token, and '{' '}' '=', '>', '<', '"', "\\"" are special byte(s) need to be a single token. *\*blank refers to ' ', '\n', '\t', '\r'*
>3. take a token assumed *operand*, *<font color=CornflowerBlue>[loop]</font>* then read next

| *next token(s)* | IF '{' AND '=' |  | ELSE IF '=' OR '>' OR '<' |  | ELSE |
|---:|---|---|---|---|---|
|  | IF contains any '=' '>' or '<' | ELSE | *type of value* |  | throw ***<font color=DarkGreen>Element name cannot contain blank.</font>*** |
|  | *type of scope* | *type of enum* | IF next non-space byte is '"' | ELSE |  |
|  |  | collect all tokens until found nearest '}' | value is a full-string, collect all tokens in quote, and trans to unicode | value is a string without *blank* |  |
| *note* | if found a '}', pop out once from '{' *stash* |  | if there is any "\\"" in value, throw ***<font color=DarkGreen>escape quote.</font>*** |  |  |
>4. if no exception throw, read for next *operand* token, and repeat *Tokenize Rule* over and over again, *<font color=CornflowerBlue>[loop]</font>* until meet the end of the script file
>5. at the end of file, if there is any '{' has not popped out from *stash**, throw ***<font color=DarkGreen>Brace is asymmetrical.</font>*** **stash records the sequence of all '{'*

![avatar](/basic_syntax.svg)


[^](#top)

#### <font color=OrangeRed>[syntax]</font> '#' Inside Quote


```CS
// such as
web_link = "Land_warfare#Theater"
```

> C:\Program Files\steam\steamapps\common\Hearts of Iron IV\interface\\**tutorialscreen.gui**
> *Element name cannot contain blank.*


#### <font color=OrangeRed>[syntax]</font> Special Assigning


[^](#top)

##### compound assigning


```CS
// such as
enable_equipments = {
    limit = {
        NOT = { has_dlc = "in Blood Alone" }
    }
    rocket_interceptor_equipment_1
}
```

> Hearts of Iron IV\common\technologies\\**electronic_mechanical_engineering.txt**
> *Element name cannot contain blank.*


[^](#top)

##### color assigning from struct enum


```CS
// such as
colors = {
    { bronze = { 155.0 105.0 87.0 1.0 } silver = { 1.0 1.0 1.0 1.0 } gold = { 0.93 0.74 0.38 1.0 } }
    { bronze = { 155.0 105.0 87.0 1.0 } silver = { 1.0 1.0 1.0 1.0 } gold = { 0.93 0.74 0.38 1.0 } }
    { bronze = { 155.0 105.0 87.0 1.0 } silver = { 1.0 1.0 1.0 1.0 } gold = { 0.93 0.74 0.38 1.0 } }
}
```

> Hearts of Iron IV\common\medals\\**00_medals.txt**
> *Element name cannot contain blank.*


[^](#top)

##### color assigning from array enum


```CS
// such as
colors = {
    { 192.0 66.0 66.0 1.0 }
    { 48.0 51.0 60.0 1.0 }
    { 238.0 189.0 96.0 1.0 }
    { 211.0 181.0 128.0 1.0 }
}
```

> Hearts of Iron IV\common\ribbons\\**00_ribbons.txt**
> *Element name cannot contain blank.*


[^](#top)

##### color assigning from value enum


```CS
// such as
color = rgb { 0 0 0 }
```

> Hearts of Iron IV\common\countries\\**Afar.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Algeria.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Andalusia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Bahamas.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Bangladesh.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Belize.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Benishangul-Gumuz.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Bosnia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**British Antilles.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Brittany.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Burma.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Burundi.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Cameroon.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Catalonia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Cayenne.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Central African Republic.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Chad.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**colors.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Corsica.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**cosmetic.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Curacao.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Cyprus.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Dahomey.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Danzig.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Djibouti.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Don Republic.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Equatorial Guinea.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Eritrea.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Fiji.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Gabon.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Galicia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Gambela.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Gambia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Ghana.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Guadeloupe.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Guam.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Guinea-Bissau.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Guinea.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Guyana.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Harar.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Hawaii.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Herzegovina.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Ivory Coast.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Jamaica.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Kashubia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Kosovo.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Kuban Republic.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Kurdistan.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Kuwait.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Kyrgyzstan.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Macedonia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Madagascar.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Malawi.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Maldives.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Mali.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Malta.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Mauritania.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Micronesia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Moldova.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Morocco.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Namibia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Navarra.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Niger.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Nigeria.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Northern Ireland.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Occitania.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Oromo.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Papua New Guinea.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Puerto Rico.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Qatar.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Qemant.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Republic of Congo.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Rif.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Rwanda.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Samoa.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Scotland.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Senegal.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Serbia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Sidamo.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Sierra Leone.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Silesia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Slovenia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Solomon.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Somalia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**South Africa.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Sri Lanka.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Sudan.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Suriname.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Tahiti.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Tajikistan.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Tanzania.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Tigray.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Togo.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Transylvania.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Trinidad and Tobago.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Tunisia.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Turkmenistan.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Uganda.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**United Arab Emirates.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Upper Volta.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Uzbekistan.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Wales.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Western Sahara.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\common\countries\\**Zambia.txt**
> *Element name cannot contain blank.*


[^](#top)

#### <font color=OrangeRed>[syntax]</font> Contains Operant As Quoted-String


```CS
// such as
"HMS Furious"= {
```

> Hearts of Iron IV\history\units\\**ENG_1936_air_bba.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\history\units\\**ENG_1936_air_legacy.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\history\units\\**ENG_1939_air_bba.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\history\units\\**ENG_1939_air_legacy.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\history\units\\**USA_1936_air_bba.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\history\units\\**USA_1936_air_legacy.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\history\units\\**USA_1939_air_bba.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\history\units\\**USA_1939_air_legacy.txt**
> *Element name cannot contain blank.*


[^](#top)

#### <font color=OrangeRed>[syntax]</font> Contains Escape Quote \\"


> Hearts of Iron IV\events\\**BBA_Ethiopia.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**BBA_ItaloEthiopianWar.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**LAR_occupation.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**LAR_Spain.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**Mexico.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**MTG_Britain.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**MTG_Netherlands.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**MTG_USA.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**NSB_Baltic.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**NSB_Soviet.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**ss_recruitment_event.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**TestEvents.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**TFV_Raj.txt**
> *escape quote.*
> Hearts of Iron IV\events\\**WTT_Japan.txt**
> *escape quote.*


> Hearts of Iron IV\common\decisions\\**BALTIC.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**BUL.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**ETH.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**FRA.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**ITA.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**MAN_decisions.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**POL.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**SOV.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**SPR.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**USA.txt**
> *escape quote.*
> Hearts of Iron IV\common\decisions\\**YUG.txt**
> *escape quote.*


> Hearts of Iron IV\common\national_focus\\**baltic_shared.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**china_shared.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**estonia.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**ethiopia.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**france.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**free_france.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**horn_of_africa.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**hungary.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**india.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**italy.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**japan.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**latvia.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**lithuania.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**manchukuo.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**mexico.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**netherlands.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**new_zealand.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**poland.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**romania.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**soviet.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**spain.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**switzerland.txt**
> *escape quote.*
> Hearts of Iron IV\common\national_focus\\**yugoslavia.txt**
> *escape quote.*
> Hearts of Iron IV\common\on_actions\\**05_lar_on_actions.txt**
> *escape quote.*
> Hearts of Iron IV\common\operations\\**00_operations.txt**
> *escape quote.*
> Hearts of Iron IV\common\operations\\**POL_historical_operations.txt**
> *escape quote.*


> Hearts of Iron IV\common\scripted_effects\\**00_scripted_effects.txt**
> *escape quote.*
> Hearts of Iron IV\common\scripted_effects\\**BUL_scripted_effects.txt**
> *escape quote.*
> Hearts of Iron IV\common\scripted_effects\\**ETH_scripted_effects.txt**
> *escape quote.*
> Hearts of Iron IV\common\scripted_effects\\**ITA_scripted_effects.txt**
> *escape quote.*
> Hearts of Iron IV\common\scripted_effects\\**POL_scripted_effects.txt**
> *escape quote.*
> Hearts of Iron IV\common\scripted_effects\\**PRC_scripted_effects.txt**
> *escape quote.*
> Hearts of Iron IV\common\scripted_effects\\**SOV_scripted_effects.txt**
> *escape quote.*
> Hearts of Iron IV\common\scripted_effects\\**USA_scripted_effects.txt**
> *escape quote.*
> Hearts of Iron IV\common\scripted_effects\\**zz_debug_effects.txt**
> *escape quote.*


[^](#top)

#### <font color=ForestGreen>[exception]</font> Unexpected Token


```CS
//
// SWE_1939_naval_legacy.txt
//
pride_of_the_fleet = yes definition definition = heavy
// unexpected token of "definition" in line 17
// this line is similar in SWE_1936_naval_legacy.txt but syntax good
```

> Hearts of Iron IV\history\units\\**SWE_1939_naval_legacy.txt**
> *Element name cannot contain blank.*

```CS
//
// core.gfx
//
effectFile = "gfx/FX/buttonstate.lua";
// unexpected token of ";" in line 1735
```

> Hearts of Iron IV\interface\\**core.gfx**
> *Element name cannot contain blank.*


[^](#top)

#### <font color=ForestGreen>[exception]</font> Missing Close Brace


> *missing for **idea** bolck at the end of file*
> Hearts of Iron IV\common\ideas\\**SOV.txt**
> *Brace is asymmetrical.*
> Hearts of Iron IV\common\ideas\\**switzerland.txt**
> *Brace is asymmetrical.*


> *missing for **objectTypes** bolck at the end of file*
> Hearts of Iron IV\gfx\entities\\**empty.gfx**
> *Brace is asymmetrical.*


> *missing for **1939 start** block at the end of file*
> Hearts of Iron IV\history\countries\\**NOR - Norway.txt**
> *Brace is asymmetrical.*


> *missing for **instant_effect** block at the end of file*
> Hearts of Iron IV\history\units\\**FRA_1936.txt**
> *Brace is asymmetrical.*
> Hearts of Iron IV\history\units\\**FRA_1936_nsb.txt**
> *Brace is asymmetrical.*


> *missing for **BRA_GAR_01** block at the end of file*
> Hearts of Iron IV\common\units\names_divisions\\**BRA_names_divisions.txt**
> *Brace is asymmetrical.*


> *missing for **spriteTypes** block at the end of file*
> Hearts of Iron IV\dlc\dlc029_allied_armor_pack\interface\\**aap_technologies.gfx**
> *Brace is asymmetrical.*


> *missing for **objectTypes** block at the end of file*
> Hearts of Iron IV\dlc\dlc031_battle_for_the_bosporus\gfx\entities\\**_BfB_meshes_infantry.gfx**
> *Brace is asymmetrical.*

> *missing for **guiTypes** block at the end of file*
> Hearts of Iron IV\interface\\**backend.gui**
> *Brace signs are asymmetrical.*

> *missing for **guiTypes** block at the end of file*
> Hearts of Iron IV\interface\\**sov_propaganda_campaigns_scripted_gui.gui***
> *Brace signs are asymmetrical.*


[^](#top)

#### <font color=DimGray>Nonstandard Hoi4 Script</font>


> Hearts of Iron IV\\**changelog.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\\**checksum_manifest.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\\**console_history.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\\**licenses.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\\**ThirdPartyLicenses.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\\**unit_test.txt**
> *Element name cannot contain blank.*


> Hearts of Iron IV\interface\\**credits.txt**
> *Element name cannot contain blank.*


> Hearts of Iron IV\localisation\\**ignored_loc_keys.txt**
> *Element name cannot contain blank.*


> Hearts of Iron IV\map\\**buildings.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\map\\**railways.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\map\\**supply_nodes.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\map\\**unitstacks.txt**
> *Element name cannot contain blank.*
> Hearts of Iron IV\map\\**weatherpositions.txt**
> *Element name cannot contain blank.*


> Hearts of Iron IV\pdx_launcher\\**motd.txt**
> *Element name cannot contain blank.*


> Hearts of Iron IV\previewer_assets\\**previewer_filefilter.txt**
> *Element name cannot contain blank.*


> Hearts of Iron IV\wiki\\**robots.txt**
> *Element name cannot contain blank.*


> Hearts of Iron IV\pdx_launcher\game\\**motd.txt**
> *Element name cannot contain blank.*


[^](#top)
