using FocusTree.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Tool.UI
{
    public static class DrawNodeMapWithInfo
    {
        static SizeF NodeSize = new(600f, 666f);
        static Vector2 ScalingUnit { get { return new(NodeSize.Width + 10f, NodeSize.Height + 80f); } }
        static float Border = 1000f;

        public static void GraphSaveasImage(FocusGraph Graph)
        {
            if (Graph == null)
            {
                return;
            }
            var canvas = GetCanvas(Graph);
            Graphics g = Graphics.FromImage(canvas);
            g.Clear(Color.White);

            foreach (var id in Graph.IdList)
            {
                DrawNodeLinks(g, Graph, id);
            }
            var enumer = Graph.GetNodesDataEnumerator();
            while (enumer.MoveNext())
            {
                var id = enumer.Current.Key;
                var drawingRect = NodeDrawingRect(Graph, id);
                g.FillRectangle(
                    new SolidBrush(Color.FromArgb(60, Color.DimGray)),
                    drawingRect
                    );
                DrawNodeInfo(g, drawingRect, enumer.Current.Value);
            }
            g.Flush();
            g.Dispose();

            canvas.Save(Path.ChangeExtension(Graph.FilePath, ".jpg"));
            canvas.Dispose();
        }
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
            return new(
                    Graph.GetMetaPoint(id).X * ScalingUnit.X + Border,
                    Graph.GetMetaPoint(id).Y * ScalingUnit.Y + Border,
                    NodeSize.Width,
                    NodeSize.Height
                    );
        }
        private static void DrawNodeLinks(Graphics g, FocusGraph Graph, int id)
        {
            var drawingRect = NodeDrawingRect(Graph, id);
            var requireGroups = Graph.GetRequireGroups(id);
            // 对于根节点，requires 为 null
            if (requireGroups == null)
            {
                return;
            }

            foreach (var requireGroup in requireGroups)
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
        private static void DrawNodeInfo(Graphics g, RectangleF drawingRect, FocusData data)
        {
            var name = data.Name;
            var duration = $"{data.Duration}日";
            var descript = data.Descript;
            var effects = string.Join("\n\n", data.Effects);
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
    }
}
