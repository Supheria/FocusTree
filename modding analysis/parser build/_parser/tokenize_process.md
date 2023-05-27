``` mermaid
graph LR
	subgraph composeBytes[compose bytes to element - the main loop ]
		direction TB
		file{{binary file}}
			--> readfile(read byte by byte)
			--> usetable(follow extract mode)
			--> compose(compose to element)
			-.-> cmpdone(complish)
			--> takeparser(jump and take to parser)
		compose
			-.-> cmpundone(unfinished or skip)
			-.-> readfile
	end
	subgraph parse[parse a given element]
		direction TB
		element{{element}}
			--> usetree(take to current step of parse tree)
			--> tokenize(process in branch and cache on tree)
			-.-> |has next step| treenext(step next and wait for next element to be called)
		tokenize -.-> |end of tree| endtree(generate a type of token)
			-.-> addtomap(add to file's token map)
	end
	subgraph setclass[use key to take Token]
		direction TB
		providekey(provide class name in enum map as key)
			--> getvalue(try to get pointer of main token by key in token map)
			--> providemember(provide class member name in enum map as key)
			--> getmember(try to get pointer of sub token by key in main token's token map)
			--> setmember(set member's value or property according to its type)
			-.- memtype.value(value type)
		setmember -.- memtype.array(array)
		setmember -.- memtype.struct(struct)
		setmember -.- memtype.cla(class)
	end
	composeBytes --> parse  --> setclass
	
```




|       compose mode | (multi-byte) type |                                                              |
| -----------------: | :---------------: | ------------------------------------------------------------ |
|                  - |     delimiter     | blank, line end, note, key char, "                           |
|               skip |                   |                                                              |
|                    |       blank       | \t, space, \n, \r                                            |
|                    |     end line      | \n, \r                                                       |
|                    |   noted string    | chars behind # in the same line, except for within quote     |
| compose to  marker |                   |                                                              |
|                    |      marker       | =, >, <, }, {                                                |
|   compose to Token |                   |                                                              |
|                    |   quoted token    | include chars and \\", begin with ", and end with " or end line |
|                    |  unquoted token   | include chars between two delimiters, not noted string       |

```mermaid
classDiagram
	class Element{
		#size_t dy_line
		#size_t dy_column // char pos in line
		+get()* char
		+del() *
	}
	note for Element "dy_ means to dynamically allocate memory"
	class Marker{
		-char sign;
		+Marker(char dy_c, size_t dy_ln, size_t dy_col)
		+get() char
	}
	class eToken{
		-string token
		+eToken(string dy_s, size_t dy_ln, size_t end)
		+get() char
	}
	direction BT
	Marker --|> Element
	eToken --|> Element
	
```

```mermaid
classDiagram
	class Token{
		-string dy_token
		#Token(string dy_token)
		+get() string
	}
	note for Key "renew type: update value or props from\ndynamic file by searching for\nthe same key in the same level"
	class Key{
		-KeyTypes tp
		-Key dy_fr
		+enum KeyTypes
		#Key(KeyTypes _type, string dy_key, Key dy_from)
		#from() Key
		+type() KeyTypes
		+key() string
		+parse(string filepath) Key pointer // will renew Key type and give back pointer to a new dynamic memory
		+combine(Key dy_k) void // combine two Key instances' values or props into one instance's
		+append(Token dy_t)*
	}
	class Value{
		char dy_operator
		Token dy_value
		Value(string dy_key, Token dy_value, char dy_operator, Key dy_from)
	}
	
	class Tag{
		Token dy_tag
		vector~Token~ dy_value
		Tag(string dy_key, Token dy_tag, Key dy_from)
		append(Token dy_t)
	}
	
	class Array{
		bool sarr // use struct array
		vector~vector~Token~~ dy_value
		Array(string dy_key, bool _sarr, Key dy_from)
		append(Token dy_t)
	}
	class Scope{
		map<`string, Token> dy_props
		Scope(string dy_key, Key dy_from)
		append(Token dy_t)
	}
	direction BT
	Key --|> Token
	Value --|> Key
	Tag --|> Key
	Array --|> Key
	Scope --|> Key
```

```mermaid
classDiagram
	class ParseTree{
		string dy_key
		char dy_operator
		Token dy_build
		Enum Steps
		Steps step
		void parse(Element e)
	}
```

type of Token

```
Value
key = subkey

Scope
key = {
	// scope
	subkey = {
		...
	} ...
	
	// value
	subkey ...
}

Tag
key = tag{
	subkey ...
	}
	
Array
key = {
	// value
	{subkey ...}
	// scope
	{subkey = {} ...}
}
```

Token::combine mode

| instance type \| combine type | Value                                                        | Scope                                                        | Tag                                                   | Array                                                        |
| ----------------------------- | ------------------------------------------------------------ | ------------------------------------------------------------ | ----------------------------------------------------- | ------------------------------------------------------------ |
| Value                         | replace subkey to latter's [warn: replacement]               | add subkey to latter's map, skip if subkey exists in the map | ex: incompatible type                                 | ex: incompatible type                                        |
| Scope                         | add latter's subkey to map, skip if subkey exists in the map | combine map elements of both                                 | ex: incompatible type                                 | ex: incompatible type                                        |
| Tag                           | ex: incompatible type                                        | ex: incompatible type                                        | replace tag and value to latter's [warn: replacement] | ex: incompatible type                                        |
| Array                         | ex: incompatible type                                        | ex: incompatible type                                        | ex: incompatible type                                 | only if both type of array same will combine value elements, otherwise ex: incompatible type |



```mermaid
graph BT
	subgraph Tokenizer
	direction LR
		newEle(new eToken)
			--o newStr(new string: token)
		map(Token map)
	end
	subgraph ParseTree
	direction LR
		newEle 
			--> eleCache(element cache and dispose)
			--> build(Token* build)
			--> map
	end
	map ---> tkDis(Token remove and dispose)
	newStr ---> tkDis
```





# extract process

```mermaid
graph
	subgraph binary file
		%% bom
		subgraph BOM
			multi.BOM.1[0xEF]
				-.-> multi.BOM.2[0xBB]
				-.-> multi.BOM.3[0xBF]
		end
		
		%% begining
		subgraph begining blanks OR endlines
			multi.BOM.3 
				-.-> multi.fhead.blk[blank, end line]
				-.-> multi.fhead.blk
		end
			multi.fhead.blk
				-.-> multi.omit(...)
				-.-> multi.unquo.noted.head
				
		%% noted string
		subgraph noted string
				multi.unquo.noted.head(#)
				-.-> multi.unquo.noted.omit(...)
				-.-> multi.unquo.noted.rear[end line]
		end
			multi.unquo.noted.rear
				-.-> multi.unquo.omit(...)
				-.-> eof(EOF)
		
		%% char token
			multi.omit -.-> multi.cht
		subgraph char token
				multi.cht[key char]
		end
			multi.cht
				-.-> multi.cht.omit(...)
				-.-> eof
		
		%% normal string token
		multi.omit 
			-.-> multi.stk.head[delimiter]
			-.-> multi.nor.omit(...)
			-.-> multi.nor.rear[delimiter]
			-.-> multi.nor.om(...)
			-.-> eof
		subgraph normal string token
			multi.nor.omit
		end
		
		%% quoted string token
		multi.stk.head --o |as| multi.quo.head["#quot;"]
		subgraph quoted string token
			multi.quo.head
				-.-> multi.quo.omit(...)
				-.-> multi.quo.rear["#quot;"]
			multi.quo.omit -.-> multi.quo.endline(end line)
				~~~ multi.quo.rear
		end
		multi.quo.rear 
			-.-> multi.quo.om1(...)
			-.-> eof
		multi.quo.endline 
			-.-> multi.quo.om2(...)
			-.-> eof
		multi.quo.endline ~~~~ multi.quo.om2
	end
```

scope struct

| type                 | data     |      |
| -------------------- | -------- | ---- |
| Token                | key      |      |
| Token                | operator |      |
| map<string, Token *> | props    |      |
| Token *              | from     |      |
|                      |          |      |
|                      |          |      |

```mermaid
%%graph
	subgraph file
		file.parse(parse)
			--> file.main
		subgraph file.main[token map]
			a
		end
	end

	subgraph main.from = null
		main.parse(parse) 
			-.-> main.entry 
			-.- main.entry.next.omit(...) 
			--> main.rear(null)
		main.entry
			-.-> main.rear
		main.parse
			-.-> ex.main.entry(empty: cannot parse main entry)
		subgraph main.entry
			direction LR
			main.entry.parse(parse)
				-.-> main.entry.entry
				-.- main.entry.entry.next.omit(...)
				--> main.entry.rear(null)
			main.entry.entry
				-.-> main.entry.rear
			main.entry.parse
				-.-> main.entry.empty(empty)
			subgraph main.entry.entry
				direction LR
				main.entry.entry.parse
					-.-> main.entry.entry.empty(empty)
				main.entry.entry.parse(parse)
					-.-> main.entry.entry.omit(...)
			end
		end
	end
```

> attentionally, it can append sub-key's property by using same key elsewhere in the same level and name scope

