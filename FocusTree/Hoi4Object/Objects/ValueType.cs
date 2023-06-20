using System.Drawing;

namespace Hoi4Object.Objects
{
    internal class ValueType : StateAtrribute
    {
        bool PositiveAdd;
        string[] StringColors = new string[]
        {
            Color.Black.Name,
            Color.Green.Name,
            Color.Orange.Name,
            Color.Red.Name
        };
        public static string _CHANGE_VALUE_NULL_ = "/CHANGE_VALUE_NULL/";

        public ValueType(bool positiveAdd)
        {
            PositiveAdd = positiveAdd;
        }
        protected virtual string[] SentenceOfChange(string value)
        {
            if (int.Parse(value) > 0)
            {
                return new string[]
                {
                    StringColors[0],
                    $"{Name}{_NAME_VALUE_SPLITTER_}",
                    PositiveAdd ? StringColors[1] : StringColors[2],
                    value
                };
            }
            else if (int.Parse(value) < 0)
            {
                return new string[]
                {
                    StringColors[0],
                    $"{Name}{_NAME_VALUE_SPLITTER_}",
                    PositiveAdd ? StringColors[1] : StringColors[2],
                    value
                };
            }
            return new string[]
            {
                StringColors[0],
                Name,
                StringColors[3],
                _CHANGE_VALUE_NULL_
            };
        }
        public override string[] ValueOfChange(string value)
        {
            return SentenceOfChange(value);
        }
        public override string[] PercentageOfChange(string percentage)
        {
            return SentenceOfChange(percentage);
        }
    }
}