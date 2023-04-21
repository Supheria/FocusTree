using System.Numerics;

namespace FocusTree.UI
{
    public static class ControlResize
    {
        static string SPLITER = " ";
        public static void SetTag(Control control)
        {
            control.Tag = control.Width + SPLITER +
                control.Height + SPLITER;
        }
        public static Size GetPrevSize(Control control)
        {
            if (control.Tag == null) { return control.Size; }
            string[] tagContent = control.Tag.ToString().Split(SPLITER);
            return new(int.Parse(tagContent[0]), int.Parse(tagContent[1]));
        }
        public static Size GetDifference(Control control)
        {
            if (control.Tag == null)
            {
                return new(0, 0);
            }
            string[] tagContent = control.Tag.ToString().Split(SPLITER);
            return new(
                control.Width - int.Parse(tagContent[0]),
                control.Height - int.Parse(tagContent[1])
                );
        }
        public static Vector2 GetRatio(Control control)
        {
            if (control.Tag == null)
            {
                return new(1, 1);
            }
            string[] tagContent = control.Tag.ToString().Split(SPLITER);
            return new(
                control.Width / float.Parse(tagContent[0]),
                control.Height / float.Parse(tagContent[1])
                );
        }
    }
}
