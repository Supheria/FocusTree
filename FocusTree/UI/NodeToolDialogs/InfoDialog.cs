using FocusTree.Tool;
using FocusTree.UI.Controls;

namespace FocusTree.UI.NodeToolDialogs
{
    public partial class InfoDialog : NodeToolDialog
    {
        bool DoFontScale = false;
        float SizeRatio = 0.618f;

        #region ==== 初始化和更新 ====

        internal InfoDialog(GraphBox display)
        {
            Display = display;
            InitializeComponent();

            Invalidated += InfoDialog_Invalidated;
            FormClosing += InfoDialog_FormClosing;
            Resize += InfoDialog_SizeChanged;
            ResizeEnd += InfoDialog_ResizeEnd;

            var font = new Font("仿宋", 20, FontStyle.Regular, GraphicsUnit.Pixel);
            textBoxList.ForEach(x => x.Font = font);
            textBoxList.ForEach(x => x.KeyDown += TextBox_KeyDown);
            textBoxList.ForEach(x => x.MouseWheel += TextBox_MouseWheel);
            textBoxList.ForEach(x => x.KeyUp += TextBox_KeyUp);
            ButtonEvent.Click += ButtonEvent_Click;

            DrawClient();
            ResizeForm.SetTag(this);
        }

        #endregion

        #region ==== 事件和更新 ====

        private void InfoDialog_Invalidated(object sender, InvalidateEventArgs e)
        {
            var focusData = Display.GetSelectedNodeData();
            Text = $"{focusData.Name}, {focusData.ID}";
            Duration.Text = $"{focusData.Duration}日";
            Descript.Text = focusData.Descript;
            Effects.Text = focusData.Effects;

            AllowDrop = Display.ReadOnly ? false : true;
            Duration.ReadOnly = Display.ReadOnly;
            ButtonEvent.Text = Display.ReadOnly ? "开始" : "保存";
            Requires.ReadOnly = Display.ReadOnly;
            Descript.ReadOnly = Display.ReadOnly;
            Effects.ReadOnly = Display.ReadOnly;
        }

        private void InfoDialog_ResizeEnd(object sender, EventArgs e)
        {
            var differ = ResizeForm.GetDifference(this);
            if (differ.Width == 0 && differ.Height != 0)
            {
                Width = (int)(Height / SizeRatio);
            }
            else if (differ.Width != 0 && differ.Height == 0)
            {
                Height = (int)(Width * SizeRatio);
            }
            if (Bottom > Screen.PrimaryScreen.Bounds.Bottom)
            {
                if (Height > Screen.PrimaryScreen.Bounds.Height)
                {
                    Height = Screen.PrimaryScreen.Bounds.Height;
                }
                Top -= Bottom - Screen.PrimaryScreen.Bounds.Bottom;
            }
            if (Left < Screen.PrimaryScreen.Bounds.Left)
            {
                Left = Screen.PrimaryScreen.Bounds.Left;
            }
            if (Right > Screen.PrimaryScreen.Bounds.Right)
            {
                Left -= Right - Screen.PrimaryScreen.Bounds.Right;
            }
            var textBox = textBoxList.FirstOrDefault();
            var ratio = ResizeForm.GetRatio(this).Y;
            var fontSize = textBox.Font.Size * ratio;
            var font = new Font(
                    textBox.Font.FontFamily,
                    fontSize < Height * 0.025f ? Height * 0.025f :
                    fontSize > Height * 0.05f ? Height * 0.05f : fontSize,
                    FontStyle.Regular,
                    GraphicsUnit.Pixel);
            textBoxList.ForEach(x => x.Font = font);
            ResizeForm.SetTag(this);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            DoFontScale = false;
        }
        private void TextBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (DoFontScale == true)
            {
                var textBox = textBoxList.FirstOrDefault();
                var fontSize = textBox.Font.Size + e.Delta * 0.01f;
                var font = new Font(
                    textBox.Font.FontFamily,
                    fontSize < Height * 0.025f ? Height * 0.025f :
                    fontSize > Height * 0.05f ? Height * 0.05f : fontSize,
                    FontStyle.Regular,
                    GraphicsUnit.Pixel);
                textBoxList.ForEach(x => x.Font = font);
            }
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                DoFontScale = true;
            }
        }
        private void InfoDialog_SizeChanged(object sender, EventArgs e)
        {
            DrawClient();
        }
        private void InfoDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
        private void DrawClient()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                return;
            }
            SuspendLayout();
            int padding = 12;
            var fontSize = Height * 0.03f;
            //
            // FocusIcon
            //
            FocusIcon.Left = ClientRectangle.Left + padding;
            FocusIcon.Top = ClientRectangle.Top + padding;
            FocusIcon.Width = (int)(MathF.Min(ClientRectangle.Width * 0.382f, ClientRectangle.Height * 0.3f));
            FocusIcon.Height = (int)(MathF.Min(ClientRectangle.Width * 0.382f, ClientRectangle.Height * 0.3f));
            //
            // Duration
            //
            Duration.Left = FocusIcon.Right + padding;
            Duration.Top = ClientRectangle.Top + padding;
            Duration.Width = (int)((ClientRectangle.Right - FocusIcon.Right) * 0.5f);
            //Duration.Height = textFont.Height;
            Duration.Font = new Font(Font.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            //
            // ButtonEvent
            //
            ButtonEvent.Left = (int)(Duration.Right + padding * 0.2f);
            ButtonEvent.Top = ClientRectangle.Top + padding;
            ButtonEvent.Width = (int)(ClientRectangle.Right - Duration.Right - padding * 1.7f);
            ButtonEvent.Height = Duration.Height;
            ButtonEvent.Font = new Font("黑体", fontSize * 0.8f, FontStyle.Regular, GraphicsUnit.Pixel);
            //
            // Requires
            //
            Requires.Left = FocusIcon.Right + padding;
            Requires.Top = Duration.Bottom + padding;
            Requires.Width = (int)(ClientRectangle.Right - FocusIcon.Right - padding * 2.5f);
            Requires.Height = FocusIcon.Bottom - Duration.Bottom - padding;
            //
            // Descript
            //
            Descript.Left = ClientRectangle.Left + padding;
            Descript.Top = FocusIcon.Bottom + padding;
            Descript.Width = (int)(ClientRectangle.Width - padding * 2.5f);
            Descript.Height = (int)(ClientRectangle.Height * 0.22f);
            //
            //EffectsTitle
            //
            EffectsTitle.Left = ClientRectangle.Left + padding;
            EffectsTitle.Top = (int)(Descript.Bottom + padding * 1.5f);
            EffectsTitle.Width = (int)(ClientRectangle.Width - padding * 2.5);
            EffectsTitle.Height = (int)(ClientRectangle.Height * 0.05f);
            //
            // Effects
            //
            Effects.Left = ClientRectangle.Left + padding;
            Effects.Top = (int)(EffectsTitle.Bottom + padding * 0.5f);
            Effects.Width = (int)(ClientRectangle.Width - padding * 2.5f);
            Effects.Height = (int)(ClientRectangle.Bottom - EffectsTitle.Bottom - padding * 2.5f);
            //
            // draw picture box image
            //
            if (FocusIcon.Image != null) { FocusIcon.Image.Dispose(); }
            if (EffectsTitle.Image != null) { EffectsTitle.Image.Dispose(); }
            FocusIcon.Image = Image.FromFile("D:\\Non_E\\documents\\GitHub\\FocusTree\\FocusTree\\FocusTree\\Resources\\FocusTree.ico");
            EffectsTitle.Image = new Bitmap(EffectsTitle.Width, EffectsTitle.Height);
            var g = Graphics.FromImage(EffectsTitle.Image);
            g.Clear(BackColor);
            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            g.DrawString(
                "==== 效果 ====",
                new Font("仿宋", fontSize, FontStyle.Regular, GraphicsUnit.Pixel),
                new SolidBrush(Color.Black),
                new RectangleF(0, 0, EffectsTitle.Width, EffectsTitle.Height),
                stringFormat
                );
            g.Flush();
            g.Dispose();
            ResumeLayout();
        }
        /// <summary>
        /// 将关闭窗体设置为隐藏窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #endregion

        #region ==== 控件事件 ====

        private void ButtonEvent_Click(object sender, EventArgs e)
        {
            if (Display.ReadOnly)
            {
                EventReadOnly();
            }
            else
            {
                EventEdit();
            }
        }
        /// <summary>
        /// 作为展示对话框
        /// </summary>
        private void EventReadOnly()
        {

        }
        /// <summary>
        /// 作为可编辑对话框
        /// </summary>
        private void EventEdit()
        {

        }

        #endregion

        #region ==== InitializeComponent ====

        private void InitializeComponent()
        {
            textBoxList = new()
            {
                Requires,
                Descript,
                Effects,
            };
            //
            // FocusIcon
            //

            //
            // Duration
            //
            Duration.Name = "Duration";
            Duration.TextAlign = HorizontalAlignment.Center;
            //
            // ButtonEvent
            //

            //
            // Requires
            //
            Requires.Name = "Requires";
            Requires.Multiline = true;
            Requires.WordWrap = false;
            Requires.ScrollBars = ScrollBars.Both;
            //
            // Descript
            //
            Descript.Name = "Descript";
            Descript.Multiline = true;
            Descript.ScrollBars = ScrollBars.Vertical;
            //
            //EffectsTitle
            //

            //
            // Effects
            //
            Effects.Name = "Effects";
            Effects.Multiline = true;
            Effects.WordWrap = false;
            Effects.ScrollBars = ScrollBars.Both;
            //
            // main
            //
            Controls.AddRange(new Control[]
            {
                FocusIcon,
                Duration,
                ButtonEvent,
                Requires,
                Descript,
                EffectsTitle,
                Effects
            });
            TopMost = true;
            MinimumSize = Size = new((int)(500 * SizeRatio), 500);
            Location = new(
                (Screen.GetBounds(this).Width / 2) - (this.Width / 2),
                (Screen.GetBounds(this).Height / 2) - (this.Height / 2)
                );
            //MinimizeBox = MaximizeBox = false;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        System.Windows.Forms.PictureBox FocusIcon = new();
        System.Windows.Forms.TextBox Duration = new();
        System.Windows.Forms.Button ButtonEvent = new();
        System.Windows.Forms.TextBox Requires = new();
        System.Windows.Forms.TextBox Descript = new();
        System.Windows.Forms.PictureBox EffectsTitle = new();
        System.Windows.Forms.TextBox Effects = new();
        List<System.Windows.Forms.TextBox> textBoxList;

        #endregion
    }
}
