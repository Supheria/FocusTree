using FocusTree.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Focus
{
    internal class FRootNode : FNode
    {
        public FRootNode()
        {
            ID = -1;
            ReliedIDs= new List<int>();
            ChildIDs = new List<int>();
            Level = -1;
            StartColum = -1;
            EndColum = -1;
            FocusData = new FData(true);

            Parent = null;
            Children= new List<FNode>();
        }
    }
}
