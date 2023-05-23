
```mermaid
graph 
	main.begin
		--- ex.main.begin(NEXT other: unexpected key\n*skip and return null*)
	main.begin{call} --> main.key[string]
	
	%% value
	subgraph main
		main.key 
			--- ex.main.key(NEXT other: unexpected operator) 
		main.key 
			--> main.op[operator]
			--- ex.main.op(NEXT other: unexpected value)
		main.op 
			-.-> main.openb((open brace))
			--- ex.main.openb(PREV NOT '=': unexpected operator\n\nNEXT operator: unexpected key or value)
		main.openb -.-> sub.key[string]
		main.openb -.-> main.closeb((close brace)) 
		main.openb -.-> main.array.openb((open brace))
		
		main.closeb((close brace)) --> main.closeb.return{return}
		
		main.op 
			-.-> main.value_t[string]
            -.-> main.value_t.openb((open brace))
		main.value_t -.-> main.value_t.return{return}
	end

	%% value tag
	subgraph main
		subgraph tagged-value
			main.value_t.openb ---
				ex.main.tag.openb(NEXT other: unexpected value)
			main.value_t.openb -.-> main.value_t.value[string]
			main.value_t.openb -.-> main.value_t.closeb

			main.value_t.value ---
				ex.main.tag(NEXT other: unexpected value)
			main.value_t.value -.-> main.value_t.value
			main.value_t.value -.-> main.value_t.closeb((close brase))
		end
		main.value_t.closeb --> main.value_t.value.return{return}
	end
	
	%% value-array array OR struct-array array
	subgraph main
		subgraph array
			main.array.openb 
				--- ex.main.array.openb(NEXT other: unexpected value)
			main.array.openb -.-> main.array.ele.closeb((close brace))
			main.array.openb -.-> main.array.ele[string]
			
			main.array.ele.closeb -.-> main.array.closeb((close brace))
			main.array.ele.closeb
				--- ex.main.array.ele.closeb(NEXT other: wrong array syntax)
			main.array.ele.closeb((close brace)) -.-> main.array.ele.openb((open brace))
		
			main.array.ele.openb -.-> main.array.ele.closeb
			main.array.ele.openb
				--- ex.main.array.ele.openb(NEXT other: unexpected value)
			main.array.ele.openb -.-> main.array.ele
			
			main.array.ele 
				-.-> varr{{value-array array}}
				--> varr.ele[string]
				--- ex.varr.ele(NEXT other: unexpected value)
			varr.ele -.-> varr.ele
				-.-> vsarr.ele.closeb
			varr --> vsarr.ele.closeb
			
			main.array.ele
				--- ex.main.array.ele(NEXT open brace: wrong array syntax)
			main.array.ele -.-> sarr{{struct-array array}}
				--> vsarr.ele.op[operator]
				
			arraytype --- ex.arraytype(type changed half way:\nunexpected array type)
			varr 
				o==o arraytype([contradictory type in loop and\ndecide one in the first time])
				o==o sarr
			arraytype 
				~~~ 
			
			vsarr.ele.op
				--- ex.vsarr.ele.op(NOT '=': unexpected operator\n\nNEXT other: unexpected value)
			vsarr.ele.op --> vsarr.ele.value.openb((open brace))
			
			vsarr.ele.value.openb
				--- ex.vsarr.value.openb(NEXT other: unexpected value)
			vsarr.ele.value.openb -.-> vsarr.ele.key.value[string]
			vsarr.ele.value.openb -.-> vsarr.ele.value.closeb
	
			
			vsarr.ele.key.value 
				--- ex.vsarr.ele.key.value(NEXT other: unexpected value)
			vsarr.ele.key.value -.-> vsarr.ele.value.closeb((close brace))
			vsarr.ele.key.value -.-> vsarr.ele.key.value

			vsarr.ele.value.closeb -.-> vsarr.ele.closeb((close brace))
			vsarr.ele.value.closeb -.-> |to next| main.array.ele
			vsarr.ele.value.closeb
				--- ex.vsarr.ele.value.closeb(NEXT other: wrong array syntax)
			
			vsarr.ele.closeb
				--- ex.vsarr.ele.closeb(NEXT other: wrong array syntax)
			vsarr.ele.closeb -.-> vsarr.closeb((close brace))
			
			vsarr.ele.openb 
				~~~ anchor
				~~~ vsarr.ele.closeb
			vsarr.ele.openb -.-> |to next| main.array.ele
			vsarr.ele.openb 
				--- ex.vsarr.ele.openb(NEXT other: unexpected value)
			vsarr.ele.closeb 
				-.-> vsarr.ele.openb((open brace))
				-.-> vsarr.ele.closeb 
		end
		main.array.closeb --> main.array.closeb.return{return}
		vsarr.closeb ---> main.vsarr.return{return}
	end

	%% sub
	subgraph main
		sub.return
			--- ex.sub.return(NEXT other: unexpected key or value\n*loop skip until close brace OR string OR eof*)
		subgraph sub
			sub.key
				--- ex.sub.key(NEXT open brace: unexpected key or value)
			sub.key -.-> sub.op[operator]
			sub.key -.-> sub.return
			
			sub.op
				--- ex.sub.op(NEXT other: unexpected value)
			sub.op 
				-.-> sub.openb((open brace))
				---ex.sub.openb(PREV NOT '=': unexpected operator\n\nNEXT operator: unexpected key or value)
			sub.op -.-> sub.value.key[string]
			
			sub.value.key -.-> sub.value.omit(...)
			sub.value.omit --> sub.return
			sub.value.key -.-> sub.return
			subgraph sub.value
				sub.value.omit
			end

			sub.openb -.-> sub.sub.omit(...)
			sub.sub.omit --> sub.return
			subgraph sub.sub
				sub.sub.omit
			end

			sub.openb 
				-.-> sub.array.omit(...)
				--> sub.return{return}
			subgraph sub.array
				sub.array.omit
			end
			sub.openb 
				-.-> sub.openb.c((close brace)) 
				--> sub.return
		end
		sub.return -.-> subr.key[string]
		sub.return 
			-.-> sub.closeb((close brace))
			--> main.sub.return{return}
	end
	
	%% sub recursive
	subgraph main
		subgraph sub-recursive
			subr.key --> subr.key.omit(...) --> subr.return{return}
		end
		subr.return -.-> subr.omit(...)
		
		subr.return 
			-.-> subr.closeb((close brace))
            --> main.subr.return{return}
	end
	
	%% style
	style varr fill:#bbf,stroke:#899,color:#fff
	style sarr fill:#bbf,stroke:#899,color:#fff
	
	style main fill:#bbb,stroke:#333,color:#fff,stroke-width:3px
	style tagged-value fill:#899,stroke:#fee,color:#fff,stroke-width:5px
	style array fill:#899,stroke:#fee,color:#fff,stroke-width:5px
	style sub fill:#899,stroke:#fee,color:#fff,stroke-width:5px
	
	style sub.value fill:#fff
	style sub.array fill:#fff
	style sub.sub fill:#fff
	style sub-recursive fill:#899,stroke:#fee,stroke-width:5px
	
	
	style ex.main.begin fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.main.key fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.main.op fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.main.openb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.main.tag.openb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.main.tag fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.main.array.openb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.main.array.ele.closeb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.main.array.ele.openb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.varr.ele fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.main.array.ele fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.vsarr.ele.op fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style arraytype fill:#f68,stroke:#fff,color:#eff,stroke-width:1px
	style ex.arraytype fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.vsarr.value.openb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.vsarr.ele.key.value fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.vsarr.ele.value.closeb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.vsarr.ele.closeb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.vsarr.ele.openb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.sub.return fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.sub.op fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.sub.key fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	style ex.sub.openb fill:#111,stroke:#fff,color:#ffe,stroke-width:1px
	
	style anchor fill:#899,color:#899,stroke-width:0px
	
	%%note
	style note fill:#333,stroke:#fff,color:#ffe,stroke-width:1px
	ex.main.begin === note([note:\nevery exception will use default disposal of \n*skip and return null*\nunless declares a new disposal])
	
```

# token type

|         token *as*         |     type     |                                                       *note* |
| :------------------------: | :----------: | -----------------------------------------------------------: |
|             =              |   operator   |                                                              |
|             <              |   operator   |                                                              |
|             >              |   operator   |                                                              |
|             {              |  open brace  |                                                              |
|             }              | close brace  |                                                              |
| utf-8 string without blank |    string    |                              blank for ' ', '\n', '\t', '\r' |
| utf-8 string within quote  |    string    | including escape quote "\\""<br><br>utf-8 characters within the quote will encode to Unicode when displaying |
|           empty            | end of chain |                            it is always  included in "other" |
