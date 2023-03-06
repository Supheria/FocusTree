namespace FocusTree.UITool
{
    public static class ResizeForm
    {
        static string SPLITER = " ";
        public static void SetTag(Control parent)
        {
            parent.Tag = parent.Width + SPLITER + parent.Height + SPLITER + parent.Left + SPLITER + parent.Top + SPLITER + parent.Font.Size;
            foreach (Control child in parent.Controls)
            {
                child.Tag = child.Width + SPLITER + child.Height + SPLITER + child.Left + SPLITER + child.Top + SPLITER + child.Font.Size;
                if (child.Controls.Count > 0)
                {
                    SetTag(child);
                }
            }
        }
        public static void ResizeControls(Control parent)
        {
            string[] tagContent = parent.Tag.ToString().Split(SPLITER);
            float newWidth = parent.Width / float.Parse(tagContent[0]);
            float newHeight = parent.Height / float.Parse(tagContent[1]);
            foreach (Control child in parent.Controls)
            {
                if (child.Tag != null)
                {
                    tagContent = child.Tag.ToString().Split(SPLITER);
                    child.Width = (int)(float.Parse(tagContent[0]) * newWidth);
                    child.Height = (int)(float.Parse(tagContent[1]) * newHeight);
                    child.Left = (int)(float.Parse(tagContent[2]) * newWidth);
                    child.Top = (int)(float.Parse(tagContent[3]) * newHeight);
                    var fontSize = float.Parse(tagContent[4]) * newHeight;
                    child.Font = new Font(child.Font.Name, fontSize, child.Font.Style, child.Font.Unit);
                    if (child.Controls.Count > 0)
                    {
                        ResizeControls(child);
                    }
                }
            }
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
