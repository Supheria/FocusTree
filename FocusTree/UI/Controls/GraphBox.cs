using FocusTree.Data;
using FocusTree.IO;
using System.Numerics;
using FocusTree.UI.Forms;
using System.Drawing;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace FocusTree.UI.Controls
{
    internal class GraphBox : PictureBox
    {
        //===== 变量 =====//

        #region ---- 临时文件名 ----
        /// <summary>
        /// 临时文件名
        /// </summary>
        public string FilePath;
        public string FileName
        {
            get
            {
                if (ReadOnly) { return Path.GetFileNameWithoutExtension(Graph.FilePath) + $"_{Path.GetFileNameWithoutExtension(FilePath)}" + "（只读）"; }
                else if (GraphEdited) 
                {
                    if (OriginalGraph != Graph.Serialize()) { return Path.GetFileNameWithoutExtension(FilePath) + "（未保存）"; }
                    else { return Path.GetFileNameWithoutExtension(FilePath) + "*"; }
                    
                }
                else { return Path.GetFileNameWithoutExtension(FilePath); }
            }
        }

        #endregion

        #region ---- 指示器 ----

        /// <summary>
        /// 先前选中的节点
        /// </summary>
        public int? SelectedNode 
        {
            get { return selectedNode; }
            private set
            {
                selectedNode = value;
                DrawGraph();
            }
        }
        int? selectedNode;
        /// <summary>
        /// 当前选中的节点
        /// </summary>
        int? SelectingNode
        {
            get { return selectingNode; }
            set
            {
                selectingNode = value;
                DrawGraph();
            }
        }
        int? selectingNode;
        /// <summary>
        /// 图像已更改
        /// </summary>
        public bool GraphEdited
        {
            get
            {
                if (OriginalGraph != Graph.Serialize() || GraphHistory.HasHistory()) { return true; }
                else { return false; }
            }
        }
        public bool ReadOnly
        {
            get
            {
                if (Path.GetDirectoryName(Path.GetDirectoryName(FilePath)) == Backup.DirectoryName) { return true; }
                else { return false; }
            }
        }

        #endregion

        #region ---- 关联控件 ----

        readonly new MainForm Parent;
        readonly InfoDialog NodeInfo;
        NodeContextMenu PicNodeContextMenu;
        GraphContextMenu PicGraphContextMenu;
        readonly ToolTip NodeInfoTip = new();

        #endregion

        #region ---- 元数据 ----

        /// <summary>
        /// 数据存储结构
        /// </summary>
        public FocusGraph Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                OriginalGraph = value.Serialize();
                PicGraphContextMenu = new GraphContextMenu(this, MouseButtons.None);
                SelectedNode = null;
                NodeInfo.Hide();
                GraphHistory.Initialize(graph);
                RelocateCenter();
            }
        }
        FocusGraph graph;
        (string, string) OriginalGraph;
        string GraphInfo;

        #endregion

        #region ---- 节点绘制工具 ----

        /// <summary>
        /// 元坐标转画布坐标时的单位坐标伸长倍数
        /// </summary>
        Vector2 MetaOrdinateCanvasUnit = new(65, 65);
        /// <summary>
        ///  节点尺寸
        /// </summary>
        SizeF NodeSize = new(55, 35);
        /// <summary>
        /// 节点字体
        /// </summary>
        const string NodeFont = "黑体";
        /// <summary>
        /// 节点字体样式
        /// </summary>
        readonly StringFormat NodeFontFormat = new();
        /// <summary>
        /// 节点文字颜色
        /// </summary>
        readonly SolidBrush NodeFG = new(Color.Black);
        /// <summary>
        /// 默认节点背景颜色
        /// </summary>
        readonly SolidBrush NodeBG = new(Color.FromArgb(80, Color.Aqua));
        /// <summary>
        /// 先前选中节点背景颜色
        /// </summary>
        readonly SolidBrush NodeBG_Selected = new(Color.FromArgb(80, Color.DarkOrange));
        /// <summary>
        /// 选中节点背景颜色
        /// </summary>
        readonly SolidBrush NodeBG_Selecting = new(Color.FromArgb(80, Color.BlueViolet));
        /// <summary>
        /// 节点连接线条（每个依赖组使用单独的颜色）
        /// </summary>
        readonly Pen[] NodeRequire = new Pen[]{
            new Pen(Color.FromArgb(100, Color.Cyan), 2),
            new Pen(Color.FromArgb(100, Color.Yellow), 2),
            new Pen(Color.FromArgb(100, Color.Green), 2),
            new Pen(Color.FromArgb(100, Color.Orange), 2),
            new Pen(Color.FromArgb(100, Color.Purple), 2)
        };

        #endregion

        #region ---- 绘图工具 ----

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
        /// 绘图中心
        /// </summary>
        Vector2 DrawingCenter = new(0, 0);
        /// <summary>
        /// 鼠标拖动时使用的定位参数
        /// </summary>
        Point DragMousePoint = new(0, 0);
        bool DragMousePoint_Flag = false;

        #endregion

        //===== 方法 =====//

        #region ---- 初始化 ----

        public GraphBox(MainForm mainForm)
        {
            base.Parent = Parent = mainForm;
            NodeInfo = new InfoDialog(this);
            NodeFontFormat.Alignment = StringAlignment.Center;
            NodeFontFormat.LineAlignment = StringAlignment.Center;
            SizeMode = PictureBoxSizeMode.Zoom;
            Dock = DockStyle.Fill;

            SizeChanged += OnSizeChanged;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;
        }

        #endregion

        #region ---- 绘图 ----

        private void DrawGraph()
        {
            if (Graph == null)
            {
                return;
            }
            Image ??= new Bitmap(Size.Width, Size.Height);
            var g = Graphics.FromImage(Image);
            g.Clear(Color.White);
            g.Flush();
            g.Dispose();

            GraphInfo = $"节点数量：{Graph.NodesCount}，分支数量：{Graph.BranchesCount}";
            DrawGraphInfo();
            DrawNodes();
            DrawLinks();
            Invalidate();

            Parent.UpdateText();
        }
        private void DrawGraphInfo()
        {
            Image ??= new Bitmap(Size.Width, Size.Height);
            var g = Graphics.FromImage(Image);

            var GraphInfoRect = new Rectangle(Bounds.Left, Bounds.Bottom - 100, Bounds.Width, 56);
            var GraphInfoFont = new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel);
            
            g.DrawString(GraphInfo, GraphInfoFont, NodeFG, GraphInfoRect, NodeFontFormat);

            g.Flush();
            g.Dispose();
        }
        private void DrawNodes()
        {
            Image ??= new Bitmap(Size.Width, Size.Height);
            var g = Graphics.FromImage(Image);

            foreach (var node in Graph.NodesCatalog)
            {
                var name = node.Value.Name;
                var rect = NodeDrawingRect(node.Key);
                var font = new Font(NodeFont, 10 * GScale, FontStyle.Bold, GraphicsUnit.Pixel);

                if (IsRectVisible(rect))
                {
                    if (IsNodeConflict(node.Key))
                    {
                        SolidBrush BG = new(Color.FromArgb(80, Color.Red));
                        g.FillRectangle(BG, rect);
                    }
                    else if (node.Key == SelectingNode)
                    {
                        g.FillRectangle(NodeBG_Selecting, rect);
                    }
                    else if (node.Key == SelectedNode)
                    {
                        g.FillRectangle(NodeBG_Selected, rect);
                    }
                    else
                    {
                        g.FillRectangle(NodeBG, rect);
                    }
                    g.DrawString(name, font, NodeFG, rect, NodeFontFormat);
                }
            }

            g.Flush();
            g.Dispose();
        }
        /// <summary>
        /// 判断有无节点冲突
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <returns></returns>
        private bool IsNodeConflict(int nodeKey)
        {
            foreach (var meta in Graph.MetaPoints)
            {
                if (nodeKey != meta.Key && Graph.MetaPoints[nodeKey] == meta.Value)
                {
                    return true;
                }
            }
            return false;
        }
        private void DrawLinks()
        {
            Image ??= new Bitmap(Size.Width, Size.Height);
            var g = Graphics.FromImage(Image);

            foreach (var nodeKey in Graph.MetaPoints.Keys)
            {
                var id = nodeKey;
                var rect = NodeDrawingRect(nodeKey);
                // 这里应该去连接依赖的节点，而不是去对子节点连接
                var requires = Graph.GetNodeRequireGroups(id);
                // 对于根节点，requires 为 null
                if (requires == null) { continue; }

                int requireColor = 0; //不同需求要变色
                foreach (var require_ids in requires)
                {
                    foreach (var require_id in require_ids)
                    {
                        var torect = NodeDrawingRect(require_id);

                        // 如果起始点和终点都不在画面里，就不需要绘制
                        if (!(IsRectVisible(rect) || IsRectVisible(torect))) { continue; }

                        var startLoc = new Point((int)(rect.X + rect.Width / 2), (int)rect.Y); // x -> 中间, y -> 下方
                        var endLoc = new Point((int)(torect.X + torect.Width / 2), (int)(torect.Y + torect.Height)); // x -> 中间, y -> 上方

                        g.DrawLine(NodeRequire[requireColor], startLoc, endLoc);
                    }
                    requireColor++;
                }
            }

            g.Flush();
            g.Dispose();
        }

        #endregion

        #region ---- 事件 ----

        //---- OnSizeChanged ----//

        private void OnSizeChanged(object sender, EventArgs args)
        {
            if (Parent.WindowState != FormWindowState.Minimized)
            {
                Image = new Bitmap(Size.Width, Size.Height);
                DrawGraph();
            }
        }

        //---- OnMouseDown ----//

        private void OnMouseDown(object sender, MouseEventArgs args)
        {
            if (Graph == null)
            {
                return;
            }

            SelectingNode = PointInAnyNodeDrawingRect(args.Location);

            if (args.Button == MouseButtons.Left)
            {
                SetDragEventFlags(args.Location);
            }

            else if ((args.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (SelectingNode == null)
                {
                    OpenPicGraphContextMenu(args.Button);
                }
                else
                {
                    NodeRightClicked();
                }
            }

            else if ((args.Button & MouseButtons.Middle) == MouseButtons.Middle)
            {
                OpenPicGraphContextMenu(args.Button);
            }
        }
        private void SetDragEventFlags(Point dragMousePoint)
        {
            DragMousePoint_Flag = true;
            DragMousePoint = dragMousePoint;
        }
        private void OpenPicGraphContextMenu(MouseButtons button)
        {
            if (PicGraphContextMenu == null || PicGraphContextMenu.ButtonTag != button)
            {
                PicGraphContextMenu = new GraphContextMenu(this, button);
            }
            PicGraphContextMenu.Show(Cursor.Position);
        }
        private void NodeRightClicked()
        {
            SelectedNode = SelectingNode;
            PicNodeContextMenu = new NodeContextMenu(this);
            PicNodeContextMenu.Show(Cursor.Position);
            NodeInfo.Hide();
        }

        //---- OnMouseDoubleClick ----//

        private void OnMouseDoubleClick(object sender, MouseEventArgs args)
        {
            if (graph == null)
            {
                return;
            }

            SelectingNode = PointInAnyNodeDrawingRect(args.Location);

            if ((args.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (SelectingNode == null)
                {
                    RestoreBackup();
                }
                else
                {
                    // 左键双击节点事件
                }
            }
        }
        private void RestoreBackup()
        {
            if (ReadOnly)
            {
                if (MessageBox.Show("[202303052340]是否恢复并删除备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Graph = Backup.Restore(FilePath);
                    FilePath = Graph.FilePath;
                    Parent.UpdateText();
                }
            }
        }

        //---- OnMouseMove ----//

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left && DragMousePoint_Flag)
            {
                DragGraph(args.Location);
            }
            else if(args.Button == MouseButtons.None)
            {
                ShowNodeInfoTip(args.Location);
            }
        }
        private void DragGraph(Point newPoint)
        {
            var dif = new Point(newPoint.X - DragMousePoint.X, newPoint.Y - DragMousePoint.Y);
            if (Math.Abs(dif.X) >= 1 || Math.Abs(dif.Y) >= 1)
            {
                var difvec = new Vector2(dif.X / GScale, dif.Y / GScale);

                DrawingCenter -= difvec;
                DragMousePoint = newPoint;
                DrawGraph();
            }
        }
        private void ShowNodeInfoTip(Point location)
        {
            var node = PointInAnyNodeDrawingRect(location);
            if (node != null)
            {
                NodeInfoTip.Show($"{Graph.NodesCatalog[node.Value].Name}\nID: {node.Value}", this, location.X + 10, location.Y);
            }
            else
            {
                NodeInfoTip.Hide(this);
            }
        }

        //---- OnMouseUp ----//

        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            // 用于拖动事件
            if (args.Button == MouseButtons.Left)
            {
                DragMousePoint_Flag = false;
            }
        }

        //---- OnMouseWheel ----//

        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            var mulDelta = 1 + args.Delta * 0.002f; // 对，这个数就是很小，不然鼠标一滚就飞了

            // 鼠标点击的位置与窗口中心的偏移量
            var click = new Vector2(args.Location.X, args.Location.Y);
            var center = new Vector2(Width / 2, Height / 2);
            var dif = click - center;

            DrawingCenter += dif * 0.2f / GScale; // 这个函数不是算出来的，只是目前恰好能用 ;p

            // 缩放
            GScale *= mulDelta;
            DrawGraph();
        }

        #endregion

        #region ---- 坐标工具 ----

        /// <summary>
        /// 画布坐标转绘制坐标
        /// </summary>
        /// <param name="rect">矩形真实坐标</param>
        /// <param name="cam">相机位置</param>
        /// <returns>矩形显示坐标</returns>
        private RectangleF CanvasRectToDrawingRect(RectangleF rect)
        {
            return new RectangleF(
                (rect.X - DrawingCenter.X) * GScale + Width / 2,
                (rect.Y - DrawingCenter.Y) * GScale + Height / 2,
                rect.Width * GScale,
                rect.Height * GScale
                );
        }
        private PointF CanvasPointToDrawingPoint(PointF point)
        {
            return new PointF(
                (point.X - DrawingCenter.X) * GScale + Width / 2,
                (point.Y - DrawingCenter.Y) * GScale + Height / 2
                );
        }
        /// <summary>
        /// 元坐标转换为绘图坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private PointF MetaPointToCanvasPoint(Point point)
        {
            return new PointF(
                point.X * MetaOrdinateCanvasUnit.X,
                point.Y * MetaOrdinateCanvasUnit.Y
                );
        }
        private Vector2 MetaPointToCanvasPoint(PointF point)
        {
            return new Vector2(
                point.X * MetaOrdinateCanvasUnit.X,
                point.Y * MetaOrdinateCanvasUnit.Y
                );
        }
        private SizeF MetaSizeToCanvasSize(Size size)
        {
            return new SizeF(
                size.Width * MetaOrdinateCanvasUnit.X,
                size.Height * MetaOrdinateCanvasUnit.Y
                );
        }

        private SizeF MetaSizeToCanvasSize(SizeF size)
        {
            return new SizeF(
                size.Width * MetaOrdinateCanvasUnit.X,
                size.Height * MetaOrdinateCanvasUnit.Y
                );
        }
        /// <summary>
        /// 绘图区域包含指定坐标的节点，若没有返回null
        /// </summary>
        /// <param name="location">指定坐标 </param>
        /// <returns>绘图区域包含指定坐标的节点，若没有返回null</returns>
        private int? PointInAnyNodeDrawingRect(Point location)
        {
            if (Graph == null)
            {
                return null;
            }
            foreach (var nodeKey in Graph.MetaPoints.Keys)
            {
                var rect = NodeDrawingRect(nodeKey);
                if (rect.Contains(location))
                {
                    return nodeKey;
                }
            }
            return null;
        }
        /// <summary>
        /// 节点的绘图区域
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private RectangleF NodeDrawingRect(int node)
        {
            var rect = new RectangleF(MetaPointToCanvasPoint(Graph.MetaPoints[node]), NodeSize);
            return CanvasRectToDrawingRect(rect);
        }
        /// <summary>
        /// 判断矩形是否可见
        /// </summary>
        /// <param name="r">矩形</param>
        /// <returns>是否可见</returns>
        private bool IsRectVisible(RectangleF rect)
        {
            return rect.Right >= 0
                && rect.Left <= Size.Width
                && rect.Bottom >= 0
                && rect.Top <= Size.Height;
        }

        #endregion

        #region ---- 镜头操作 ----

        /// <summary>
        /// 自动缩放居中
        /// </summary>
        public void RelocateCenter()
        {
            if (Graph == null) { return; }
            var center = Graph.CenterMetaData();
            DrawingCenter = MetaPointToCanvasPoint(center.Item1);

            var canvasSize = MetaSizeToCanvasSize(center.Item2);
            float px = Size.Width / canvasSize.Width;
            float py = Size.Height / canvasSize.Height;

            // 我也不知道为什么这里要 *0.90，直接放结果缩放的尺寸会装不下，*0.90 能放下，而且边缘有空余
            GScale = Math.Min(px, py) * 0.90f;
            DrawGraph();
        }
        /// <summary>
        /// 聚焦先前选中的节点
        /// </summary>
        public void LocateSelected()
        {
            if (selectedNode == null)
            {
                RelocateCenter();
                return;
            }
            var point = Graph.MetaPoints[selectedNode.Value];
            var canvasPoint = MetaPointToCanvasPoint(point);
            DrawingCenter = new(canvasPoint.X + NodeSize.Width / 2, canvasPoint.Y + NodeSize.Height / 2);
            DrawGraph();
        }

        #endregion

        #region ---- 读写操作 ----

        /// <summary>
        /// 保存
        /// </summary>
        public void SaveGraph()
        {
            if(Graph == null)
            {
                return;
            }
            if (GraphEdited)
            {
                Backup.BackupFile(FilePath);
                XmlIO.SaveGraph(FilePath, Graph);
                OriginalGraph = Graph.Serialize();
                Parent.UpdateText();
            }
            else
            {
                XmlIO.SaveGraph(FilePath, Graph);
            }
        }
        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="path"></param>
        public void SaveAsNew(string path)
        {
            Graph.FilePath = FilePath = path;
            XmlIO.SaveGraph(path, Graph);
            OriginalGraph = Graph.Serialize();
            Parent.UpdateText();
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="path"></param>
        public void LoadGraph(string path)
        {
            FilePath = path;
            Graph = XmlIO.LoadGraph(path);
        }
        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            if (GraphHistory.HasPrev())
            {
                GraphHistory.Undo(ref graph);
            }
            DrawGraph();
        }
        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            if (GraphHistory.HasNext())
            {
                GraphHistory.Redo(ref graph);
            }
            DrawGraph();
        }

        #endregion

        #region ---- 节点操作 ----

        public void ShowNodeInfo()
        {
            if (NodeInfo == null || SelectedNode == null)
            {
                return;
            }
            var rect = NodeDrawingRect(SelectedNode.Value);
            var point = new Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            NodeInfo.Show(PointToScreen(point));
        }
        public void RemoveNode()
        {
            if (SelectedNode == null)
            {
                return;
            }
            Graph.RemoveNode(SelectedNode.Value);
            GraphHistory.Enqueue(Graph);
            SelectedNode = null;
            DrawGraph();
        }

        #endregion
    }
}
