using FocusTree.Data.Focus;
using FocusTree.UI.Controls;
using System;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FocusTree.IO
{
    public class NodeMapDrawer : Form
    {
        private ProgressBar Progress;
        float Percent { get { return (float)Progress.Value / (float)Progress.Maximum * 100; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count">进度条的最大值</param>
        private NodeMapDrawer(int count)
        {
            InitializeComponent();
            Visible = true;
            Progress.Minimum = 0;
            Progress.Maximum = count;
            Progress.Value = 0;
            Progress.Step = 1;
        }

        private void InitializeComponent()
        {
            Progress = new ProgressBar();
            SuspendLayout();
            // 
            // Progress
            // 
            Progress.Dock = DockStyle.Fill;
            Progress.ForeColor = SystemColors.ActiveCaption;
            Progress.Location = new Point(0, 0);
            Progress.Name = "Progress";
            Progress.Size = new Size(305, 33);
            Progress.Style = ProgressBarStyle.Continuous;
            Progress.TabIndex = 0;
            // 
            // NodeMapDrawer
            // 
            ClientSize = new Size(305, 33);
            Controls.Add(Progress);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "NodeMapDrawer";
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            ResumeLayout(false);
        }

        /// <summary>
        /// 进度条步进下一刻
        /// </summary>
        private void StepNext()
        {
            Progress.PerformStep();
        }

        #region ==== 输出图片 ====

        static NodeMapDrawer Drawer;
        public static void SaveasImage(FocusGraph Graph)
        {
            if (Graph == null)
            {
                return;
            }
            var canvas = GetCanvas(Graph);
            Graphics g = Graphics.FromImage(canvas);
            g.Clear(Color.White);

            Drawer = new(Graph.NodesCount + 1);
            foreach (var id in Graph.IdList)
            {
                DrawNodeLinks(g, Graph, id);
            }
            foreach (var node in Graph.GetNodes())
            {
                var drawingRect = NodeDrawingRect(Graph, node.ID);
                g.FillRectangle(
                    new SolidBrush(Color.FromArgb(60, Color.DimGray)),
                    drawingRect
                    );
                DrawNodeInfo(g, drawingRect, node);
                Drawer.StepNext();
                Drawer.Text = $"{Graph.Name}.jpg: {(int)(Drawer.Percent)}%";
            }
            g.Flush();
            g.Dispose();

            canvas.Save(Path.ChangeExtension(Graph.FilePath, ".jpg"));
            canvas.Dispose();
            Drawer.Close();
        }

        #endregion

        #region ==== 绘图信息 ====

        static SizeF NodeSize = new(600f, 666f);
        static Vector2 ScalingUnit { get { return new(NodeSize.Width + 10f, NodeSize.Height + 80f); } }
        static float Border = 1000f;
        private static Image GetCanvas(FocusGraph Graph)
        {
            var center = Graph.GetGraphMetaData();
            var size = new Size(
                (int)(center.Item2.Width * ScalingUnit.X + Border * 2),
                (int)(center.Item2.Height * ScalingUnit.Y + Border * 2)
                );
            return new Bitmap(size.Width, size.Height);
        }
        private static RectangleF NodeDrawingRect(FocusGraph Graph, int id)
        {
            var point = Graph.GetNode(id).MetaPoint;
            return new(
                    point.X * ScalingUnit.X + Border,
                    point.Y * ScalingUnit.Y + Border,
                    NodeSize.Width,
                    NodeSize.Height
                    );
        }

        #endregion

        #region ==== 绘图 ====
        private static void DrawNodeLinks(Graphics g, FocusGraph Graph, int id)
        {
            var drawingRect = NodeDrawingRect(Graph, id);
            var requires = Graph.GetNode(id).Requires;
            // 对于根节点，requires 为 null
            if (requires == null)
            {
                return;
            }
            foreach (var requireGroup in requires)
            {
                foreach (var require in requireGroup)
                {
                    var todrawingRect = NodeDrawingRect(Graph, require);

                    var startLoc = new Point((int)(drawingRect.X + drawingRect.Width / 2), (int)(drawingRect.Y + drawingRect.Height / 2)); // x -> 中间, y -> 下方
                    var endLoc = new Point((int)(todrawingRect.X + todrawingRect.Width / 2), (int)(todrawingRect.Y + todrawingRect.Height / 2)); // x -> 中间, y -> 上方

                    g.DrawLine(
                        new Pen(Color.FromArgb(120, Color.OrangeRed), 10),
                        startLoc, endLoc
                        );
                }
            }
        }
        private static void DrawNodeInfo(Graphics g, RectangleF drawingRect, FocusNode node)
        {
            var name = node.Name;
            var duration = $"{node.Duration}日";
            var descript = node.Descript;
            var effects = string.Join("\n\n", node.Effects);
            float padding = 10f;

            RectangleF nameRect = new(
                drawingRect.Left + padding,
                drawingRect.Top + padding,
                drawingRect.Width * 0.618f - padding,
                drawingRect.Height * 0.15f - padding
                );
            RectangleF durationRect = new(
                nameRect.Right + padding,
                drawingRect.Top + padding,
                drawingRect.Right - nameRect.Right - padding * 2f,
                drawingRect.Height * 0.15f - padding
                );
            RectangleF descriptRect = new(
                drawingRect.Left + padding,
                nameRect.Bottom + padding,
                drawingRect.Width - padding * 2f,
                drawingRect.Height * 0.2f
                );
            RectangleF effectsRect = new(
                drawingRect.Left + padding,
                descriptRect.Bottom + padding,
                drawingRect.Width - padding * 2f,
                drawingRect.Bottom - descriptRect.Bottom - padding * 2f
                );
            g.FillRectangles(
                new SolidBrush(Color.FromArgb(150, Color.DarkGreen)),
                new RectangleF[]{
                nameRect,
                durationRect
            });
            g.FillRectangles(
                new SolidBrush(Color.FromArgb(255, Color.White)),
                new RectangleF[]{
                descriptRect,
                effectsRect
            });

            StringFormat nodeFontFormat = new();
            nodeFontFormat.Alignment = StringAlignment.Center;
            nodeFontFormat.LineAlignment = StringAlignment.Center;
            g.DrawString(name,
                new("黑体", 35, FontStyle.Regular, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(255, Color.White)),
                nameRect, nodeFontFormat
                );
            g.DrawString(duration,
                new("黑体", 35, FontStyle.Regular, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(255, Color.White)),
                durationRect, nodeFontFormat
                );
            nodeFontFormat.Alignment = StringAlignment.Near;
            g.DrawString(descript,
                new("仿宋", 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(255, Color.Black)),
                descriptRect, nodeFontFormat
                );
            nodeFontFormat.LineAlignment = StringAlignment.Near;
            g.DrawString(effects,
                new("仿宋", 23, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(255, Color.Black)),
                effectsRect, nodeFontFormat
                );
        }

        #endregion
    }
}
