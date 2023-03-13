using System.Numerics;

namespace FocusTree.Tool
{
    public static class ResizeForm
    {
        static string SPLITER = " ";
        public static void SetTag(Control parent)
        {
            parent.Tag = parent.Width + SPLITER +
                parent.Height + SPLITER;
        }
        public static Size GetDifference(Control parent)
        {
            string[] tagContent = parent.Tag.ToString().Split(SPLITER);
            return new(
                parent.Width - int.Parse(tagContent[0]),
                parent.Height - int.Parse(tagContent[1])
                );
        }
        public static Vector2 GetRatio(Control parent)
        {
            string[] tagContent = parent.Tag.ToString().Split(SPLITER);
            return new(
                parent.Width / float.Parse(tagContent[0]),
                parent.Height / float.Parse(tagContent[1])
                );
        }
    }
}
