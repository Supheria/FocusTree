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
            Level = -1;
            FocusData = new FData(true);
        }
    }
}
