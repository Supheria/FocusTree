using System.Numerics;

namespace FocusTree.UITool
{
    public static class ResizeForm
    {
        static string SPLITER = " ";
        public static void SetTag(Control parent, bool setChildren)
        {
            parent.Tag = parent.Width + SPLITER + 
                parent.Height + SPLITER + 
                parent.Left + SPLITER + 
                parent.Top + SPLITER + 
                parent.Font.Size;
            if(setChildren == false)
            {
                return;
            }
            foreach (Control child in parent.Controls)
            {
                child.Tag = child.Width + SPLITER + 
                    child.Height + SPLITER + 
                    child.Left + SPLITER + 
                    child.Top + SPLITER + 
                    child.Font.Size;
                if (child.Controls.Count > 0)
                {
                    SetTag(child, true);
                }
            }
        }
        public static void Resize(Control parent, bool keepScale)
        {
            string[] tagContent = parent.Tag.ToString().Split(SPLITER);
            float rX = parent.Width / float.Parse(tagContent[0]);
            float rY = parent.Height / float.Parse(tagContent[1]);
            var r = MathF.Max(rX, rY);
            parent.Width = (int)(float.Parse(tagContent[0]) * r);
            parent.Height = (int)(float.Parse(tagContent[1]) * r);
            foreach (Control child in parent.Controls)
            {
                if (child.Tag != null)
                {
                    tagContent = child.Tag.ToString().Split(SPLITER);
                    child.Width = (int)(float.Parse(tagContent[0]) * r);
                    child.Height = (int)(float.Parse(tagContent[1]) * r);
                    child.Left = (int)(float.Parse(tagContent[2]) * r);
                    child.Top = (int)(float.Parse(tagContent[3]) * r);
                    var fontSize = float.Parse(tagContent[4]) * r;
                    child.Font = new Font(child.Font.Name, fontSize, child.Font.Style, child.Font.Unit);
                    if (child.Controls.Count > 0)
                    {
                        Resize(child);
                    }
                }
            }
        }
        public static void Resize(Control parent)
        {
            string[] tagContent = parent.Tag.ToString().Split(SPLITER);
            float rX = parent.Width / float.Parse(tagContent[0]);
            float rY = parent.Height / float.Parse(tagContent[1]);
            foreach (Control child in parent.Controls)
            {
                if (child.Tag != null)
                {
                    tagContent = child.Tag.ToString().Split(SPLITER);
                    child.Width = (int)(float.Parse(tagContent[0]) * rX);
                    child.Height = (int)(float.Parse(tagContent[1]) * rY);
                    child.Left = (int)(float.Parse(tagContent[2]) * rX);
                    child.Top = (int)(float.Parse(tagContent[3]) * rY);
                    var fontSize = float.Parse(tagContent[4]) * MathF.Min(rX, rY);
                    child.Font = new Font(child.Font.Name, fontSize, child.Font.Style, child.Font.Unit);
                    if (child.Controls.Count > 0)
                    {
                        Resize(child);
                    }
                }
            }
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
