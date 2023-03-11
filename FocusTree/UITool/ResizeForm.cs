using System.Numerics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace FocusTree.UITool
{
    public static class ResizeForm
    {
        static string SPLITER = " ";
        public static void SetTag(Control control, int width, int height)
        {
            control.Tag = width + SPLITER +
                height + SPLITER;
        }
        public static void SetTag(Control parent)
        {
            parent.Tag = parent.Width + SPLITER + 
                parent.Height + SPLITER + 
                parent.Left + SPLITER + 
                parent.Top + SPLITER + 
                parent.Font.Size;
        }
        public static void Resize(Control parent, bool keepScale)
        {

            string[] tagContent = parent.Tag.ToString().Split(SPLITER);
            var differ = ResizeForm.GetDifference(parent);
            var SizeRatio = float.Parse(tagContent[0]) / float.Parse(tagContent[1]);
            if (differ.Width == 0 && differ.Height != 0)
            {
                parent.Width = (int)(parent.Height * SizeRatio);
            }
            else if (differ.Width != 0 && differ.Height == 0)
            {
                parent.Height = (int)(parent.Width / SizeRatio);
            }
            //else
            //{
            //    float rX = parent.Width / float.Parse(tagContent[0]);
            //    float rY = parent.Height / float.Parse(tagContent[1]);
            //    var r = MathF.Max(rX, rY);
            //    parent.Width = (int)(float.Parse(tagContent[0]) * r);
            //    parent.Height = (int)(float.Parse(tagContent[1]) * r);
            //}
            
        }
        public static void Resize(Control parent)
        {
            string[] tagContent = parent.Tag.ToString().Split(SPLITER);
            float rX = parent.Width / float.Parse(tagContent[0]);
            float rY = parent.Height / float.Parse(tagContent[1]);
            var fontSize = float.Parse(tagContent[4]) * MathF.Min(rX, rY);
            parent.Font = new Font(parent.Font.Name, fontSize, parent.Font.Style, parent.Font.Unit);
            foreach (Control child in parent.Controls)
            {
                if (child.Tag != null)
                {
                    tagContent = child.Tag.ToString().Split(SPLITER);
                    child.Width = (int)(float.Parse(tagContent[0]) * rX);
                    child.Height = (int)(float.Parse(tagContent[1]) * rY);
                    child.Left = (int)(float.Parse(tagContent[2]) * rX);
                    child.Top = (int)(float.Parse(tagContent[3]) * rY);
                    //var fontSize = float.Parse(tagContent[4]) * MathF.Min(rX, rY);
                    child.Font = new Font(child.Font.Name, fontSize, child.Font.Style, child.Font.Unit);
                    if (child.Controls.Count > 0)
                    {
                        Resize(child);
                    }
                }
            }
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
            float rX = parent.Width / float.Parse(tagContent[0]);
            float rY = parent.Height / float.Parse(tagContent[1]);
            return new(rX, rY);
        }
        public static void DefultSize(Control parent)
        {
            string[] tagContent = parent.Tag.ToString().Split(SPLITER);
            int width = int.Parse(tagContent[0]);
            int height = int.Parse(tagContent[1]);
            parent.Size = new Size(width, height);
        }
    }
}
