﻿using FocusTree.Focus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace FocusTree.UI
{
    internal class DisplayBox : PictureBox
    {
        /// <summary>
        /// 父窗体对象（用于定位自动尺寸的位置）
        /// </summary>
        readonly MainForm ParentForm;
        /// <summary>
        /// 数据存储结构
        /// </summary>
        public FGraph Graph;
        /// <summary>
        /// 绘图缩放倍率
        /// </summary>
        float GScale { get { return gscale; } set { gscale = value < 0.1f ? 0.1f : value > 10f ? 10f : value; } }
        float gscale = 1f; // 不要调用这个，不安全，用上边的访问器，有缩放尺寸限制
        /// <summary>
        /// 默认的字体
        /// </summary>
        const string GFont = "黑体";

        /// <summary>
        /// 默认字体样式
        /// </summary>
        readonly StringFormat GFontFormat = new();

        /// <summary>
        /// 节点文字颜色
        /// </summary>
        readonly SolidBrush NodeFG = new(Color.Black);

        /// <summary>
        /// 节点背景矩形颜色
        /// </summary>
        readonly SolidBrush NodeBG = new(Color.FromArgb(80, Color.Aqua));

        /// <summary>
        /// 节点连接线条
        /// </summary>
        readonly Pen NodeLink = new(Color.FromArgb(100, Color.Cyan), 1.5f);
        /// <summary>
        /// 节点间距 + 节点尺寸
        /// </summary>
        Rectangle NodePaddingSize = new(65, 65, 55, 35);
        /// <summary>
        /// 默认相机位置（画面中心）
        /// </summary>
        Vector2 Camera = new(0, 0);

        public DisplayBox(MainForm parent)
        {
            ParentForm = parent;
            GFontFormat.Alignment = StringAlignment.Center;
            GFontFormat.LineAlignment = StringAlignment.Center;
            SizeMode = PictureBoxSizeMode.Zoom;
            Dock = DockStyle.Fill;

            Invalidated += OnInValidated;
            SizeChanged += OnSizeSize;
            MouseDown += (obj, arg) => { MessageBox.Show(ClickedLocation(arg.Location).ToString()); };
        }
        /// <summary>
        /// 自动缩放居中
        /// </summary>
        public void RelocateCenter()
        {
            if(Graph == null) { return; }
            var bounds = Graph.GetNodeMapBounds();
            Camera = VectorToVisualVector(new Vector2(bounds.X, bounds.Y));
            // 画幅尺寸
            var visual_size = VectorToVisualVector(new Vector2(bounds.Z + 1, bounds.W + 1));
            float px = Size.Width / visual_size.X, py = Size.Height / visual_size.Y;

            // 我也不知道为什么这里要 *0.85，直接放结果缩放的尺寸会装不下，*0.85 能放下，而且边缘有空余
            GScale = Math.Min(px, py) * 0.85f; 
        }

        private void OnInValidated(object sender, EventArgs args)
        {
            if (Graph == null) { return; }
            Image ??= new Bitmap(Size.Width, Size.Height);

            var font = new Font(GFont, 10 * GScale, FontStyle.Bold, GraphicsUnit.Pixel);

            var g = Graphics.FromImage(Image);
            g.Clear(Color.White);

            var nodesEnumer = Graph.GetNodesEnumerator();
            while (nodesEnumer.MoveNext())
            {
                var node = nodesEnumer.Current;
                var name = node.Value.FocusData.Name;
                var rect = RectOnScreenRect(NodeMapToVisualMap(Graph.GetNodeMapElement(node.Key)));

                if (IsRectInScreen(rect))
                {
                    g.FillRectangle(NodeBG, rect);
                    g.DrawString(name, font, NodeFG, rect, GFontFormat);
                }
            }
            var mapEnumer = Graph.GetNodeMapEnumerator();
            while (mapEnumer.MoveNext())
            {
                var id = mapEnumer.Current.Key;
                var rect = RectOnScreenRect(NodeMapToVisualMap(mapEnumer.Current.Value));
                var links = Graph.GetNodeRelations(id).Where(x => x.Type == NodeRelation.FRelations.Linked);
                foreach (var link in links)
                {
                    foreach (var link_id in link.IDs)
                    {
                        var torect = RectOnScreenRect(NodeMapToVisualMap(Graph.GetNodeMapElement(link_id)));

                        // 如果起始点和终点都不在画面里，就不需要绘制
                        if (!(IsRectInScreen(rect) || IsRectInScreen(torect))) { continue; }

                        var startLoc = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height); // x -> 中间, y -> 下方
                        var endLoc = new Point(torect.X + torect.Width / 2, torect.Y); // x -> 中间, y -> 上方

                        g.DrawLine(NodeLink, startLoc, endLoc);
                    }
                }
            }
            g.Flush(); g.Dispose();
            Update();
        }
        /// <summary>
        /// 绘图区域尺寸变更时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSizeSize(object sender, EventArgs args)
        {
            Image = new Bitmap(Size.Width, Size.Height);
        }
        /// <summary>
        /// 获取矩形真实坐标在控件显示空间的投影 (显示坐标)
        /// </summary>
        /// <param name="rect">矩形真实坐标</param>
        /// <param name="cam">相机位置</param>
        /// <returns>矩形显示坐标</returns>
        private Rectangle RectOnScreenRect(Rectangle rect)
        {
            return new Rectangle(
                (int)((rect.X - Camera.X) * GScale + Size.Width / 2f),
                (int)((rect.Y - Camera.Y) * GScale + Size.Height / 2f),
                (int)(rect.Width * GScale),
                (int)(rect.Height * GScale)
                );
        }
        /// <summary>
        /// 获取用户点击的坐标
        /// </summary>
        /// <param name="click">相对于 PictureBox 的坐标</param>
        /// <param name="cam">相机位置</param>
        /// <returns>真实坐标</returns>
        private Point ClickedLocation(Point click)
        {
            return new Point(
                (int)((click.X - Size.Width / 2f) / GScale + Camera.X),
                (int)((click.Y - Size.Height / 2f) / GScale + Camera.Y)
                );
        }
        /// <summary>
        /// 判断矩形是否在需要控件可见空间内
        /// </summary>
        /// <param name="r">矩形</param>
        /// <returns>是否可见</returns>
        private bool IsRectInScreen(Rectangle r) { return r.Right >= 0 && r.Left <= Size.Width && r.Bottom >= 0 && r.Top <= Size.Height; }
        /// <summary>
        /// NodeMap 的位置信息转换为显示时的绘图信息<br/>
        /// </summary>
        /// <param name="nodeMap"></param>
        /// <returns></returns>
        private Rectangle NodeMapToVisualMap(Point nodeMap)
        {
            return new Rectangle(
                nodeMap.X * NodePaddingSize.X,
                nodeMap.Y * NodePaddingSize.Y,
                NodePaddingSize.Width,
                NodePaddingSize.Height
                );
        }
        private Vector2 VectorToVisualVector(Vector2 vec)
        {
            return new Vector2(
                vec.X * NodePaddingSize.X,
                vec.Y * NodePaddingSize.Y
                );
        }
    }
}