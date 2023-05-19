
```mermaid
graph

main.call(call) --> main.key[string]

main.closeb --> main.return(return)

subgraph main
    main.nextis{next is} -.-> main.nextkey(string) --> main.op
    main.nextis -.-> main.closeb((close brace))


    %% value
    main.key --> main.op[operator] -.-> main.value.key[string] -.-> main.nextis
    subgraph main.value
       main.value.key -.-> main.value.openb((open brace)) --> main.value.next.key[string] -.-> main.value.next.key -.-> main.value.closeb((close brase))
    end
    main.value.closeb --> main.nextis


    main.op -.-> main.openb((open brace))
    

    %% array
    main.openb -.-> main.array.openb((open brace))

    subgraph main.array
        main.array.openb((open brace)) -.-> varr.ele.key[string] -.->varr.ele.key
        
        %% value-array array
        subgraph value-array
            varr.ele.key -.-> varr.ele.closeb((close brace))
            varr.ele.closeb -.-> varr.ele.openb((open brace)) --> |to next| varr.ele.key
            varr.ele.closeb -.-> varr.closeb((close brace))
        end
    end
    varr.closeb((close brace)) --> main.nextis

    subgraph main.array
        main.array.openb -.-> sarr.ele.key[string]

        %% struct-array array
        subgraph struct-array
            sarr.ele.key --> sarr.ele.op[operator] --> sarr.ele.value.openb((open brace)) --> sarr.ele.value.key[string] -.-> sarr.ele.value.key -.-> sarr.ele.value.closeb((close brace))
            sarr.ele.value.closeb -.-> sarr.ele.value.openb ==> |to next| sarr.ele.key
            sarr.ele.value.closeb -.-> sarr.ele.closeb((close brace))
            sarr.ele.closeb -.-> sarr.ele.openb((open brace)) ---> |to next| sarr.ele.key
            sarr.ele.closeb -.-> sarr.closeb((close brace))
        end
    end
    sarr.closeb --> main.nextis


    main.openb -.-> sub.key[string]

    subgraph sub
        sub.key -.-> sub.key -.-> sub.closeb(close brace)
    end
    sub.closeb(close brace) --> main.nextis

    subgraph sub
        sub.key -.-> sub.op(operator) -.-> sub.openb(open brace)

        sub.openb -.-> omitarr(...) --> sub.nextis{next is}
        subgraph sub.array
            omitarr
        end

        sub.openb -.-> omitsub(...) --> sub.nextis
        subgraph sub.sub
            omitsub
        end
        
        sub.op -.-> sub.value.key(string) --> omitval(...) --> sub.nextis
        subgraph sub.value
            omitval
        end

        sub.nextis -.-> omitmain(...)
        subgraph sub.main
            omitmain
        end
        sub.nextis -.-> sub.closeb(close brace)
        omitmain --> sub.main.return(return)
    end
    
    sub.main.return --> main.nextis
end
```

|token *as*| type | *note* |
|:---:|:---:|---:|
| = | operator |
| < | operator |
| > | operator |
| { | open brace |
| } | close brace |
| utf-8 string without blank | string | blank for ' ', '\n', '\t', '\r'
| utf-8 string within quote | string | including escape quote "\\""
