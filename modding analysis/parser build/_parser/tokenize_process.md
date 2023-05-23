token struct

|   type |  data  |                                     |
| -----: | :----: | ----------------------------------- |
| string | token  |                                     |
| size_t |  line  |                                     |
| size_t | column | start position of token in the line |




| tokenize mode | (multi-byte) char type |                                                              |
| ------------: | :--------------------: | ------------------------------------------------------------ |
|             - |       delimiter        | blank, line end, note, key char, "                           |
|        ignore |                        |                                                              |
|               |         blank          | \t, space                                                    |
|               |        end line        | \n, \r                                                       |
|               |      noted string      | chars behind # in the same line, except for within quote     |
|    char token |                        |                                                              |
|               |        key char        | =, >, <, }, {                                                |
|  string token |                        |                                                              |
|               |     quoted string      | include chars and \\", begin and end with " in the same line |
|               |     normal string      | include chars between two delimiters, not noted string       |

# tokenize process

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