using FocusTree.Data;
using FocusTree.Data.Focus;
using FocusTree.IO;
using FocusTree.IO.FileManege;
using FocusTree.UI.NodeToolDialogs;
using System.IO;
using System.Numerics;

namespace FocusTree.UI.Controls
{
    internal class GraphBox : PictureBox
    {
        //===== 变量 =====//

        #region ---- 文件路径 ----
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath;
        public string FileName
        {
            get
            {
                if (ReadOnly) { return Path.GetFileNameWithoutExtension(Graph.FilePath) + $"{Path.GetFileNameWithoutExtension(FilePath)}" + "（只读）"; }
                else if (GraphEdited) { return Path.GetFileNameWithoutExtension(FilePath) + "（未保存）"; }
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
            /*private */
            set
            {
                selectedNode = value;
                PrevSelectNode = null;
            }
        }
        int? selectedNode;
        /// <summary>
        /// 预选中的节点
        /// </summary>
        int? PrevSelectNode;
        /// <summary>
        /// 图像已更改
        /// </summary>
        public bool GraphEdited { get { return Graph.IsEdit(); } }
        public bool ReadOnly { get; private set; }
        public Dictionary<string, NodeToolDialog> ToolDialogs { get { return toolDialogs; } }

        #endregion

        #region ---- 关联控件 ----

        readonly new MainForm Parent;
        readonly Dictionary<string, NodeToolDialog> toolDialogs = new()
        {
            ["国策信息"] = new NodeToolDialog()
        };
        InfoDialog NodeInfo
        {
            get { return (InfoDialog)ToolDialogs["国策信息"]; }
            init { toolDialogs["国策信息"] = value; }
        }
        NodeContextMenu PicNodeContextMenu;
        GraphContextMenu PicGraphContextMenu;
        readonly ToolTip NodeInfoTip = new();

        #endregion

        #region ---- 元数据 ----

        /// <summary>
        /// 数据存储结构
        /// </summary>
        public FocusGraph Graph { get; private set; }

        #endregion

        #region ---- 节点绘制工具 ----

        /// <summary>
        /// 元坐标转画布坐标时的单位坐标伸长倍数
        /// </summary>
        Vector2 ScalingUnit { get { return new(NodeSize.Width + 10, NodeSize.Height + 30); } }
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
            set { gscale = value < 0.1f ? 0.1f : value > 5f ? 5f : value; }
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

        #region ---- 初始化和更新 ----

        public GraphBox(MainForm mainForm)
        {
            base.Parent = Parent = mainForm;
            NodeInfo = new InfoDialog(this);
            NodeFontFormat.Alignment = StringAlignment.Center;
            NodeFontFormat.LineAlignment = StringAlignment.Center;
            //SizeMode = PictureBoxSizeMode.Zoom;
            Dock = DockStyle.Fill;
            DoubleBuffered = true;

            SizeChanged += OnSizeChanged;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;
            Invalidated += UpdateGraph;

            ControlResize.SetTag(this);
        }
        private void UpdateGraph(object sender, InvalidateEventArgs e)
        {
            if (Graph == null)
            {
                return;
            }
            DrawNodeMap();
            _draw_info($"节点数量：{Graph.NodesCount}，分支数量：{Graph.BranchesCount}",
                new Rectangle(Bounds.Left, Bounds.Bottom - 88, Bounds.Width, 35),
                new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
            Parent.UpdateText();

            Update();
        }
        public void DrawInfo(string info, Rectangle infoRect, Font infoFont, Brush BackBrush, Brush FrontBrush)
        {
            if (Graph == null)
            {
                return;
            }
            Invalidated -= UpdateGraph;

            DrawNodeMap();
            _draw_info(info,
                infoRect,
                infoFont,
                BackBrush,
                FrontBrush
                );

            Invalidate();
            Invalidated += UpdateGraph;
        }
        public void DrawInfo(string info)
        {
            if (Graph == null)
            {
                return;
            }
            Invalidated -= UpdateGraph;

            DrawNodeMap();
            _draw_info(info,
                new Rectangle(Bounds.Left, Bounds.Bottom - 100, Bounds.Width, 66),
                new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );

            Invalidate();
            Invalidated += UpdateGraph;
        }

        #endregion

        #region ---- 绘图 ----
        private void DrawNodeMap()
        {
            if (Graph == null) { return; }
            Image ??= new Bitmap(Size.Width, Size.Height);
            var g = Graphics.FromImage(Image);
            g.Clear(Color.White);

            foreach(var node in Graph.GetNodes())
            {
                var id = node.ID;
                var rect = NodeDrawingRect(id);
                var font = new Font(NodeFont, 10 * GScale, FontStyle.Bold, GraphicsUnit.Pixel);

                if (IsRectVisible(rect))
                {
                    if (IsNodeConflict(id))
                    {
                        SolidBrush BG = new(Color.FromArgb(80, Color.Red));
                        g.FillRectangle(BG, rect);
                    }
                    else if (id == PrevSelectNode)
                    {
                        g.FillRectangle(NodeBG_Selecting, rect);
                    }
                    else if (id == SelectedNode)
                    {
                        g.FillRectangle(NodeBG_Selected, rect);
                    }
                    else
                    {
                        g.FillRectangle(NodeBG, rect);
                    }
                    g.DrawString(node.Name, font, NodeFG, rect, NodeFontFormat);
                }

                var requires = Graph.GetNode(id).Requires;
                int requireColor = 0; //不同需求要变色
                foreach (var requireGroup in requires)
                {
                    foreach (var require in requireGroup)
                    {
                        var torect = NodeDrawingRect(require);

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
        private void _draw_info(string info, Rectangle infoRect, Font infoFont, Brush BackBrush, Brush FrontBrush)
        {
            if (Graph == null) { return; }
            Image ??= new Bitmap(Size.Width, Size.Height);
            var g = Graphics.FromImage(Image);

            g.FillRectangle(BackBrush, infoRect);
            g.DrawString(
                info,
                infoFont,
                FrontBrush,
                infoRect,
                NodeFontFormat);

            g.Flush();
            g.Dispose();
        }
        /// <summary>
        /// 判断有无节点冲突
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <returns></returns>
        private bool IsNodeConflict(int id)
        {
            foreach(var node in Graph.GetNodes())
            {
                if (id != node.ID && Graph.GetNode(id).MetaPoint == node.MetaPoint)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region ---- 事件 ----

        //---- OnPaint ----


        //---- OnSizeChanged ----//

        private void OnSizeChanged(object sender, EventArgs args)
        {
            if (Graph == null || Math.Min(Size.Width, Size.Height) < 0 || Parent.WindowState == FormWindowState.Minimized)
            {
                return;
            }
            if (Image != null)
            {
                Image.Dispose();
            }
            var ratioVec = ControlResize.GetRatio(this);
            var ratio = MathF.Min(ratioVec.X, ratioVec.Y);
            ControlResize.SetTag(this);
            Image = new Bitmap(Size.Width, Size.Height);
            GScale = GScale * ratio;
            Invalidate();
        }

        //---- OnMouseDown ----//

        private void OnMouseDown(object sender, MouseEventArgs args)
        {
            if (Graph == null) { return; }

            CheckPrevSelect();

            if (args.Button == MouseButtons.Left)
            {
                if (PrevSelectNode == null)
                {
                    GraphLeftClicked(args.Location);
                }
                else
                {
                    NodeLeftClicked(PrevSelectNode.Value);
                }
            }

            else if ((args.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (PrevSelectNode == null)
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
        private void GraphLeftClicked(Point dragMousePoint)
        {
            DragMousePoint_Flag = true;
            DragMousePoint = dragMousePoint;
            Invalidate();
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
            CloseAllNodeToolDialogs();
            SelectedNode = PrevSelectNode;
            RescaleToNode(SelectedNode.Value, false);
            PicNodeContextMenu = new NodeContextMenu(this);
            PicNodeContextMenu.Show(Cursor.Position);
            Invalidate();
        }
        private void NodeLeftClicked(int id)
        {
            var data = Graph.GetNode(id);
            var info = $"{data.Name}, {data.Duration}日\n{data.Descript}";
            DrawInfo(info);
        }

        //---- OnMouseDoubleClick ----//

        private void OnMouseDoubleClick(object sender, MouseEventArgs args)
        {
            if (Graph == null) { return; }

            CheckPrevSelect();

            if ((args.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (PrevSelectNode == null)
                {
                    GraphLeftDoubleClicked();
                    RestoreBackup();
                }
                else
                {
                    NodeLeftDoubleClicked();
                }
            }
        }
        private void GraphLeftDoubleClicked()
        {
            SelectedNode = null;
            Invalidate();
        }
        private void RestoreBackup()
        {
            if (ReadOnly)
            {
                if (MessageBox.Show("[202303052340]是否恢复并删除备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CloseAllNodeToolDialogs();
                    Graph.RestoreBackup(FilePath);
                    ReadOnly = false;
                    SelectedNode = null;
                    RescaleToPanorama();
                    Invalidate();
                }
            }
        }
        private void NodeLeftDoubleClicked()
        {
            SelectedNode = PrevSelectNode;
            RescaleToNode(SelectedNode.Value, false);
            NodeInfoTip.Hide(this);
            Invalidate();
        }

        //---- OnMouseMove ----//

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (Graph == null) { return; }
            if (args.Button == MouseButtons.Left && DragMousePoint_Flag)
            {
                DragGraph(args.Location);
            }
            else if (args.Button == MouseButtons.None)
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
                Invalidate();
            }
        }
        private void ShowNodeInfoTip(Point location)
        {
            var node = PointInAnyNodeDrawingRect(location);
            if (node != null)
            {
                NodeInfoTip.BackColor = Color.FromArgb(0, Color.AliceBlue);
                NodeInfoTip.Show($"{Graph.GetNode(node.Value).Name}\nID: {node.Value}", this, location.X + 10, location.Y);
            }
            else
            {
                NodeInfoTip.Hide(this);
            }
        }

        //---- OnMouseUp ----//

        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (Graph == null) { return; }
            // 用于拖动事件
            if (args.Button == MouseButtons.Left)
            {
                DragMousePoint_Flag = false;
            }
        }

        //---- OnMouseWheel ----//

        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            if (Graph == null) { return; }
            var mulDelta = 1 + args.Delta * 0.002f; // 对，这个数就是很小，不然鼠标一滚就飞了

            // 鼠标点击的位置与窗口中心的偏移量
            var click = new Vector2(args.Location.X, args.Location.Y);
            var center = new Vector2(Width / 2, Height / 2);
            var dif = click - center;

            DrawingCenter += dif * 0.2f / GScale; // 这个函数不是算出来的，只是目前恰好能用 ;p

            // 缩放
            GScale = MathF.Min(GScale * mulDelta, MathF.Min(Width * 0.1f, Height * 0.2f) * 0.02f);
            Invalidate();
        }

        //---- Public ----//
        private void CheckPrevSelect()
        {
            var clickPos = PointToClient(Cursor.Position);
            PrevSelectNode = PointInAnyNodeDrawingRect(clickPos);
        }
        private void CloseAllNodeToolDialogs()
        {
            ToolDialogs.ToList().ForEach(x => x.Value.Close());
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
        private Vector2 CanvasPointToDrawingPoint(Vector2 point)
        {
            return new Vector2(
                (point.X - DrawingCenter.X) * GScale + Width / 2,
                (point.Y - DrawingCenter.Y) * GScale + Height / 2
                );
        }
        /// <summary>
        /// 元坐标转换为绘图坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vector2 MetaPointToCanvasPoint(Vector2 point)
        {
            return new Vector2(
                point.X * ScalingUnit.X,
                point.Y * ScalingUnit.Y
                );
        }
        private SizeF MetaSizeToCanvasSize(SizeF size)
        {
            return new SizeF(
                size.Width * ScalingUnit.X,
                size.Height * ScalingUnit.Y
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

            foreach (var id in Graph.IdList)
            {
                var rect = NodeDrawingRect(id);
                if (rect.Contains(location))
                {
                    return id;
                }
            }
            return null;
        }
        /// <summary>
        /// 节点的绘图区域
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private RectangleF NodeDrawingRect(int id)
        {
            var point = MetaPointToCanvasPoint(Graph.GetNode(id).MetaPoint);
            var rect = new RectangleF(new(point.X, point.Y), NodeSize);
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
        /// <summary>
        /// 缩放居中至全景
        /// </summary>
        private void RescaleToPanorama()
        {
            if (Graph == null)
            {
                return;
            }
            var meta = Graph.GetGraphMetaData();
            DrawingCenter = MetaPointToCanvasPoint(meta.Item1);
            var canvasSize = MetaSizeToCanvasSize(meta.Item2);
            float px = Size.Width / canvasSize.Width;
            float py = Size.Height / canvasSize.Height;

            // 我也不知道为什么这里要 *0.90，直接放结果缩放的尺寸会装不下，*0.90 能放下，而且边缘有空余
            GScale = MathF.Min(px, py);
        }
        /// <summary>
        /// 缩放居中至节点
        /// </summary>
        /// <param name="ID">节点ID</param>
        /// <param name="zoom">是否聚焦</param>
        private void RescaleToNode(int ID, bool zoom)
        {
            if (Graph == null)
            {
                return;
            }
            var point = Graph.GetNode(ID).MetaPoint;
            var canvasPoint = MetaPointToCanvasPoint(point);
            DrawingCenter = new(canvasPoint.X + NodeSize.Width / 2, canvasPoint.Y + NodeSize.Height / 2);
            if (zoom)
            {
                GScale = MathF.Min(Width * 0.1f, Height * 0.2f) * 0.02f;
            }
            Cursor.Position = Parent.PointToScreen(new Point(
                Bounds.X + Bounds.Width / 2,
                Bounds.Y + Bounds.Height / 2
                ));
        }

        #endregion

        #region ---- 读写操作 ----

        public void SaveGraph(string path, FocusGraph graph)
        {
            graph.Save(FilePath);
            graph.FilePath = path;
            graph.Latest = graph.Format();
        }
        /// <summary>
        /// 保存
        /// </summary>
        public void SaveGraph()
        {
            if (Graph == null || ReadOnly) { return; }
            if (GraphEdited)
            {
                Graph.BackupFile(FilePath);
                Graph.Save(FilePath);
                Invalidate();
            }
            else
            {
                Graph.Save(FilePath);
            }
        }
        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="path"></param>
        public void SaveAsNew(string path)
        {
            if (Graph == null) { return; }
            Graph.FilePath = FilePath = path;
            Graph.Save(path);
            Graph.NewHistory();
            Invalidate();
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="path"></param>
        public void LoadGraph(string path)
        {
            CloseAllNodeToolDialogs();
            ReadOnly = Backup.IsBackupFile(path);
            if (!ReadOnly) { FilePath = path; }
            Graph = XmlIO.LoadFromXml<FocusGraph>(path);
            Graph.NewHistory();
            SelectedNode = null;
            RescaleToPanorama();
            Invalidate();
        }
        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            Graph.Undo();
            Invalidate();
        }
        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            Graph.Redo();
            Invalidate();
        }

        public bool HasPrevHistory()
        {
            if (Graph == null) { return false; }
            return Graph.HasPrev();
        }
        public bool HasNextHistory()
        {
            if (Graph == null) { return false; }
            return Graph.HasNext();
        }

        #endregion

        #region ---- 镜头操作调用 ----

        public void CamLocatePanorama()
        {
            RescaleToPanorama();
            Invalidate();
        }
        /// <summary>
        /// 聚焦先前选中的节点
        /// </summary>
        public void CamLocateSelected()
        {
            if (SelectedNode == null) { return; }
            else
            {
                RescaleToNode(SelectedNode.Value, true);
            }
            Invalidate();
        }

        #endregion

        #region ---- 节点操作调用 ----

        public void ShowNodeInfo()
        {
            var rect = NodeDrawingRect(SelectedNode.Value);
            var point = new Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            NodeInfo.Show(PointToScreen(point));
        }
        public void RemoveNode()
        {
            if (SelectedNode == null) { return; }
            Graph.RemoveNode(SelectedNode.Value);
            Graph.EnqueueHistory();
            SelectedNode = null;
            Invalidate();
        }
        public FocusNode GetSelectedNodeData()
        {
            if (SelectedNode == null) { return null; }
            return Graph.GetNode(SelectedNode.Value);
        }
        public Point GetSelectedNodeCenterOnScreen()
        {
            var rect = NodeDrawingRect(SelectedNode.Value);
            var point = new Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            return PointToScreen(point);
        }

        #endregion

        #region ---- 图像操作调用 ----

        public void ReorderNodeIds()
        {
            if (Graph == null) { return; }
            Graph.ReorderNodeIds();
            Invalidate();
        }

        #endregion

        #region ---- 绘图操作调用 ----

        public void DrawAddtionalInfo(string info)
        {
            Invalidate();
            DrawInfo(info);
        }

        #endregion
    }
}
