using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4Parser
{
    public class ParseTree
    {
        string Key;
        char Op;
        Token Build;
        bool SArr;
        ParseTree SubTree;
        ParseTree FromTree;

        public ParseTree()
        {

        }
    }
}
