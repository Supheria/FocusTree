using FocusTree.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Focus
{
    internal class FRootNode : FNodeBase
    {
        public override int ID { get; set; } = -1;
        public override List<int> ReliedIDs { get; set; } = null;
        public override List<int> ChildIDs { get; set; } = new();
        public override int Level { get; set; } = -1;
        public override int StartColum { get; set; } = -1;
        public override int EndColum { get; set; } = -1;
        public override FData FocusData { get; set; } = new FData(true);
    }
}
