using System.Numerics;
using FocusTree.Data;

namespace FocusTree.UI
{
    internal class GraphBox : PictureBox
    {
        readonly MainForm ParentForm;
        readonly NodeContextMenu PicNodeContextMenu;
        /// <summary>
        /// 数据存储结构
        /// </summary>
        public FocusGraph Graph;
        /// <summary>
        /// 绘图缩放倍率
        /// </summary>
        float GScale 
        { 
            get { return gscale; } 
            set { gscale = value < 0.1f ? 0.1f : value > 10f ? 10f : value; } 
        }
        float gscale = 1f; // 不要调用这个，不安全，用上边的访问器，有缩放尺寸限制
        /// <summary>
        /// 默认的字体
        /// </summary>
        const string GFont = "黑体";
        /// <summary>
        /// 默认字体大小
        /// </summary>
        const float GFontSize = 10;
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
        /// 节点连接线条（每个依赖组使用单独的颜色）
        /// </summary>
        readonly Pen[] NodeRequire = new Pen[]{
            new Pen(Color.FromArgb(100, Color.Cyan), 1.5f),
            new Pen(Color.FromArgb(100, Color.Yellow), 1.5f),
            new Pen(Color.FromArgb(100, Color.Green), 1.5f),
            new Pen(Color.FromArgb(100, Color.Orange), 1.5f),
            new Pen(Color.FromArgb(100, Color.Purple), 1.5f)
        };
        /// <summary>
        /// 节点间距 + 节点尺寸
        /// </summary>
        Rectangle NodePaddingSize = new(65, 65, 55, 35);
        /// <summary>
        /// 默认相机位置（画面中心）
        /// </summary>
        Vector2 Camera = new(0, 0);
        /// <summary>
        /// 鼠标拖动时使用的定位参数
        /// </summary>
        Point DragMousePoint = new(0, 0);
        bool DragMousePoint_Flag = false;
        public GraphBox(MainForm mainForm)
        {
            Parent = ParentForm = mainForm;
            PicNodeContextMenu = new NodeContextMenu(this);
            GFontFormat.Alignment = StringAlignment.Center;
            GFontFormat.LineAlignment = StringAlignment.Center;
            SizeMode = PictureBoxSizeMode.Zoom;
            Dock = DockStyle.Fill;

            Invalidated += OnInValidated;
            SizeChanged += OnSizeSize;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
        }
        /// <summary>
        /// 当节点被右键时响应的事件
        /// </summary>
        /// <param name="id">被点击的节点ID</param>
        private void NodeRightClicked(int id)
        {
            PicNodeContextMenu.NodeId = id;
            PicNodeContextMenu.Show(Cursor.Position);
        }
        /// <summary>
        /// 自动缩放居中
        /// </summary>
        public void RelocateCenter()
        {
            if (Graph == null) { return; }
            var bounds = Graph.GetNodeMapBounds();
            Camera = VectorToVisualVector(new Vector2(bounds.X, bounds.Y));
            // 画幅尺寸
            var visual_size = VectorToVisualVector(new Vector2(bounds.Z + 1, bounds.W + 1));
            float px = Size.Width / visual_size.X, py = Size.Height / visual_size.Y;

            // 我也不知道为什么这里要 *0.90，直接放结果缩放的尺寸会装不下，*0.90 能放下，而且边缘有空余
            GScale = Math.Min(px, py) * 0.90f;
        }
        /// <summary>
        /// 要求重绘时更新画面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnInValidated(object sender, EventArgs args)
        {
            if (Graph == null) { return; }
            Image ??= new Bitmap(Size.Width, Size.Height);

            var font = new Font(GFont, GFontSize * GScale, FontStyle.Bold, GraphicsUnit.Pixel);

            var g = Graphics.FromImage(Image);
            g.Clear(Color.White);

            var nodesEnumer = Graph.GetNodesCatalogEnumerator();
            while (nodesEnumer.MoveNext())
            {
                var node = nodesEnumer.Current;
                var name = node.Value.Name;
                var rect = RectOnScreenRect(NodeMapToVisualMap(Graph.GetNodePointsElement(node.Key)));

                if (IsRectInScreen(rect))
                {
                    g.FillRectangle(NodeBG, rect);
                    g.DrawString(name, font, NodeFG, rect, GFontFormat);
                }
            }
            var mapEnumer = Graph.GetNodePointsEnumerator();
            while (mapEnumer.MoveNext())
            {
                var id = mapEnumer.Current.Key;
                var rect = RectOnScreenRect(NodeMapToVisualMap(mapEnumer.Current.Value));
                // 这里应该去连接依赖的节点，而不是去对子节点连接
                var requires = Graph.GetNodeRequireGroups(id);
                // 对于根节点，requires 为 null
                if (requires == null) { continue; }

                int requireColor = 0; //不同需求要变色
                foreach (var require_ids in requires)
                {
                    foreach (var require_id in require_ids)
                    {
                        var torect = RectOnScreenRect(NodeMapToVisualMap(Graph.GetNodePointsElement(require_id)));

                        // 如果起始点和终点都不在画面里，就不需要绘制
                        if (!(IsRectInScreen(rect) || IsRectInScreen(torect))) { continue; }

                        var startLoc = new Point(rect.X + rect.Width / 2, rect.Y); // x -> 中间, y -> 下方
                        var endLoc = new Point(torect.X + torect.Width / 2, torect.Y + torect.Height); // x -> 中间, y -> 上方

                        g.DrawLine(NodeRequire[requireColor], startLoc, endLoc);
                    }
                    requireColor++;
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
            if (ParentForm.WindowState == FormWindowState.Minimized)
            {
                return;
            }
            Image = new Bitmap(Size.Width, Size.Height);
        }
        private void OnMouseDown(object sender, MouseEventArgs args)
        {
            // 用于拖动事件
            if (args.Button == MouseButtons.Left)
            {
                DragMousePoint_Flag = true;
                DragMousePoint = args.Location;
            }
            // 对节点打开右键菜单
            if ((args.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                var point = ClickedLocation(args.Location);
                int? clickedNode = GetFirstNodeClicked(point);
                if (clickedNode != null) { NodeRightClicked(clickedNode.Value); }
            }
        }
        /// <summary>
        /// 返回第一个被点击的节点ID，如果没有节点被点击则返回 null
        /// </summary>
        /// <param name="location">点击的真实坐标 (不是绘制坐标) </param>
        /// <returns>节点ID 或 null</returns>
        private int? GetFirstNodeClicked(Point location)
        {
            if (Graph == null) { return null; }
            var mapEnumer = Graph.GetNodePointsEnumerator();
            while (mapEnumer.MoveNext())
            {
                var rect = NodeMapToVisualMap(mapEnumer.Current.Value);
                if (rect.Contains(location))
                {
                    return mapEnumer.Current.Key;
                }
            }
            return null;
        }
        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            // 用于拖动事件
            if (args.Button == MouseButtons.Left && DragMousePoint_Flag)
            {
                var newPoint = args.Location;
                var dif = new Point(newPoint.X - DragMousePoint.X, newPoint.Y - DragMousePoint.Y);
                if (Math.Abs(dif.X) >= 1 || Math.Abs(dif.Y) >= 1)
                {
                    var difvec = new Vector2(dif.X / GScale, dif.Y / GScale);

                    // Console.WriteLine($"p:{newPoint}, drag:{DragMousePoint}, cam: {Camera}, dif: {dif}, vec: {difvec}");

                    Camera -= difvec;
                    DragMousePoint = newPoint;
                    Invalidate();
                }
            }
        }
        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            // 用于拖动事件
            if (args.Button == MouseButtons.Left)
            {
                DragMousePoint_Flag = false;
            }
        }
        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            var mulDelta = 1 + (args.Delta * 0.002f); // 对，这个数就是很小，不然鼠标一滚就飞了

            // 缩放前进行偏移
            var cLoc = ClickedVec(args.Location);
            var dif = cLoc - Camera;

            Camera += dif * 0.2f; // 这个函数不是算出来的，只是目前恰好能用 ;p

            // 缩放
            GScale *= mulDelta;
            Invalidate();
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
        private Vector2 ClickedVec(Point click)
        {
            return new Vector2(
                (click.X - Size.Width / 2f) / GScale + Camera.X,
                (click.Y - Size.Height / 2f) / GScale + Camera.Y
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
