using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Tool.Data
{
    public interface IHistoryData
    {
    }
    public class HistoryData_FocusGraph : IHistoryData
    {
        public string Item1;
        public string Item2;
        public HistoryData_FocusGraph(string item1, string item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}
