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
|                    |       blank       | \t, space                                                    |
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
		size_t _pos // the position in file stream
		virtual char get() = 0
	}
	class Marker{
		char sign
		Marker(char ch)
		void char get()
	}
	class Token{
		string token
		Token(string s)
		void char get()
	}
	direction BT
	Marker --|> Element
	Token --|> Element
	
	class ParseTree{
		Element key
		Element operator
		Enum Steps
		Steps step
		Token build
		void parse(Element e)
	}
```

```c++
char Marker::get()
{
    return sign;
}

char Token::get()
{
    return token[0];
}
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

```mermaid
classDiagram 
	class Element{
		size_t _pos // the position in file stream
		virtual char get() = 0
	}
	class Token{
		string token
		Token(string s)
		void char get()
	}
	Token --|> Element
	
	class Token{
		Token * from
		abstract void parse() // do nothing
	}
	class Value{
		char operator
		Value(Token _t) // conversion
	}
	class Tag{
		char operator
		vector~string~ value
		parse()
		Tag(Token _t) // conversion
	}
	class Array{
		Array(Token _t) // conversion
	}
		note for Scope "when find a existed key, it will take the block to\n(* value).subs other than use key-list<`Token*> here"
	class Scope{
		map<`string, Token *> props
		void parse()
		void parse(string token)
		Scope(Token _t) // conversion
	}
	direction BT
	Value --|> Token
	Tag --|> Token
	Array --|> Token
	Scope --|> Token
```