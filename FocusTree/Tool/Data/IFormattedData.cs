using FocusTree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Tool.Data
{
    public interface IFormattedData
    {
        public string[] Items { get; }
        public bool Equals(IFormattedData other);
    }
    public class FormatedFocusGraph : IFormattedData
    {
        string Item1;
        string Item2;
        public string[] Items { get { return new string[] { Item1, Item2 }; } }
        public FormatedFocusGraph(string item1, string item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        public bool Equals(IFormattedData IOther)
        {
            var other = IOther as FormatedFocusGraph;
            if (other == null)
            {
                return false;
            }
            (string, string) thisMetas = new(Item1, Item2);
            (string, string) otherMetas = new(other.Item1, other.Item2);
            return thisMetas == otherMetas;
        }
    }
    public class FormatedInfoDialog : IFormattedData
    {
        string Item;
        public string[] Items { get { return new string[] { Item }; } }
        public FormatedInfoDialog(string item)
        { 
            Item = item;
        }
        public bool Equals(IFormattedData IOther)
        {
            var other = IOther as FormatedInfoDialog;
            if(other == null)
            {
                return false;
            }
            return Item == other.Item;
        }
    }
}
