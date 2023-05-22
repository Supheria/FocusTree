token struct

| type   | data   |                                     |
| ------ | ------ | ----------------------------------- |
| string | token  |                                     |
| size_t | line   |                                     |
| size_t | column | start position of token in the line |



tokenize element type


|              mode |     type      |                                                              |
| ----------------: | :-----------: | ------------------------------------------------------------ |
|                 - |   delimiter   | blank, line end, note, key char, "                           |
|            ignore |               |                                                              |
|                   |     blank     | \t, space                                                    |
|                   |   line end    | \n, \r                                                       |
|                   |     note      | #, except for within quote                                   |
|                   | noted string  | chars behind note in the same line                           |
| token single char |               |                                                              |
|                   |   key char    | =, >, <, }, {                                                |
|      token string |               |                                                              |
|                   | quoted string | include utf-8 chars and \\", begin and end with " in the same line |
|                   | normal string | include utf-8 chars between two delimiters, not noted string |

```mermaid
%%graph
	begin(begin)
	begin -.-> start.blk[blank]
		--> blk.next(skip)
		
	begin -.-> start.char[char] -.-> start.char
	start.char -.-> char.blk[blank]
		--> start.char.blk.next(skip)
	start.char -.-> start.char.quo[quote]
		--> start.char.quo.next(keep)
	start.char -.-> start.char.note[note]
		--> start.char.note.con( )
		-.-> start.char.note.blk[blank]
	
	begin -.-> start.quo[quote]
	begin -.-> start.note[note]
```