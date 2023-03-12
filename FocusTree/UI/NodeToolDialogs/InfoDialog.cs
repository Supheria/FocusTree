using FocusTree.UI.Controls;
using FocusTree.UITool;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FocusTree.UI.NodeToolDialogs
{
    public partial class InfoDialog : NodeToolDialog
    {
        List<TextBox> textBoxList;
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
            textBoxList = new()
            {
                Requires,
                Descript,
                Effects,
            };
            var font = new Font("仿宋", 20, FontStyle.Regular, GraphicsUnit.Pixel);
            textBoxList.ForEach(x => x.Font = font);
            textBoxList.ForEach(x => x.KeyDown += TextBox_KeyDown);
            textBoxList.ForEach(x => x.MouseWheel += TextBox_MouseWheel);
            textBoxList.ForEach(x => x.KeyUp += TextBox_KeyUp);

            DrawClient();
            ResizeForm.SetTag(this);
        }

        private void InfoDialog_Invalidated(object sender, InvalidateEventArgs e)
        {
            var focusData = Display.GetSelectedNodeData();
            Text = $"{focusData.Name}, {focusData.ID}";
            Duration.Text = $"{focusData.Duration}日";
            Descript.Text = focusData.Descript;
            Effects.Text = focusData.Effects;
        }

        private void InfoDialog_ResizeEnd(object sender, EventArgs e)
        {

            var differ = ResizeForm.GetDifference(this);
            if (differ.Width == 0 && differ.Height != 0)
            {
                Width = (int)(Height * SizeRatio);
            }
            else if (differ.Width != 0 && differ.Height == 0)
            {
                Height = (int)(Width / SizeRatio);
            }
            else if (differ.Width != 0 && differ.Height != 0)
            {
                var ratioVec = ResizeForm.GetRatio(this);
                if(ratioVec.X > ratioVec.Y)
                {
                    Width = (int)(Height * SizeRatio);
                }
                else
                {
                    Height = (int)(Width / SizeRatio);
                }
            }
            if (Bottom > Screen.PrimaryScreen.Bounds.Bottom)
            {
                if (Height > Screen.PrimaryScreen.Bounds.Height)
                {
                    Height = Screen.PrimaryScreen.Bounds.Height;
                }
                Top -= Bottom - Screen.PrimaryScreen.Bounds.Bottom;
                Width = (int)(Height * SizeRatio);
            }
            if(Left < Screen.PrimaryScreen.Bounds.Left)
            {
                Left = Screen.PrimaryScreen.Bounds.Left;
            }
            if(Right > Screen.PrimaryScreen.Bounds.Right)
            {
                Left -= Right - Screen.PrimaryScreen.Bounds.Right;
            }

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
                var textBox = sender as TextBox;
                var font = new Font(textBox.Font.FontFamily, textBox.Font.Size + e.Delta * 0.01f, FontStyle.Regular, GraphicsUnit.Pixel);
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
            SuspendLayout();
            int padding = 12;
            var fontSize = MathF.Min(Width * 0.05f, Height * 0.03f);
            //
            // FocusIcon
            //
            FocusIcon.Left = ClientRectangle.Left + padding;
            FocusIcon.Top = ClientRectangle.Top + padding;
            FocusIcon.Width = (int)(MathF.Min(ClientRectangle.Width * 0.382f, ClientRectangle.Height * 0.3f));
            FocusIcon.Height = (int)(MathF.Min(ClientRectangle.Width * 0.382f, ClientRectangle.Height * 0.3f));
            AllowDrop = Display.ReadOnly ? false : true;
            //
            // Duration
            //
            Duration.Left = FocusIcon.Right + padding;
            Duration.Top = ClientRectangle.Top + padding;
            Duration.Width = (int)((ClientRectangle.Right - FocusIcon.Right) * 0.5f);
            //Duration.Height = textFont.Height;
            Duration.Font = new Font(Font.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            Duration.ReadOnly = Display.ReadOnly;
            //
            // ButtonEvent
            //
            ButtonEvent.Left = (int)(Duration.Right + padding * 0.2f);
            ButtonEvent.Top = ClientRectangle.Top + padding;
            ButtonEvent.Width = (int)(ClientRectangle.Right - Duration.Right - padding * 1.7f);
            ButtonEvent.Height = Duration.Height;
            ButtonEvent.Font = new Font("黑体", fontSize * 0.8f, FontStyle.Regular, GraphicsUnit.Pixel);
            ButtonEvent.Text = Display.ReadOnly ? "开始" : "保存";
            //
            // Requires
            //
            Requires.Left = FocusIcon.Right + padding;
            Requires.Top = Duration.Bottom + padding;
            Requires.Width = (int)(ClientRectangle.Right - FocusIcon.Right - padding * 2.5f);
            Requires.Height = FocusIcon.Bottom - Duration.Bottom - padding;
            Requires.ReadOnly = Display.ReadOnly;
            //
            // Descript
            //
            Descript.Left = ClientRectangle.Left + padding;
            Descript.Top = FocusIcon.Bottom + padding;
            Descript.Width = (int)(ClientRectangle.Width - padding * 2.5f);
            Descript.Height = (int)(ClientRectangle.Height * 0.22f);
            Descript.ReadOnly = Display.ReadOnly;
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
            Effects.ReadOnly = Display.ReadOnly;
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

        private void btnEvent_Click(object sender, EventArgs e)
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
            //
            // FocusIcon
            //

            //
            // Duration
            //

            //
            // ButtonEvent
            //

            //
            // Requires
            //
            Requires.Multiline = true;
            Requires.WordWrap = false;
            Requires.ScrollBars = ScrollBars.Both;
            //
            // Descript
            //
            Descript.Multiline = true;
            Descript.WordWrap = false;
            Descript.ScrollBars = ScrollBars.Both;
            //
            //EffectsTitle
            //

            //
            // Effects
            //
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
            MinimizeBox = MaximizeBox = false;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
        }

        PictureBox FocusIcon = new();
        TextBox Duration = new();
        Button ButtonEvent = new();
        TextBox Requires = new TextBox();
        TextBox Descript = new();
        PictureBox EffectsTitle = new();
        TextBox Effects = new();

        #endregion
    }
}
