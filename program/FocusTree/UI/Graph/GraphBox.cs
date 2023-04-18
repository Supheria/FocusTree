#define DEBUG
#define REBUILD
using FocusTree.Data;
using FocusTree.Data.Focus;
using FocusTree.IO;
using FocusTree.IO.FileManege;
using FocusTree.UI.Controls;
using FocusTree.UI.NodeToolDialogs;
using System.Numerics;
using static FocusTree.UI.Graph.LatticeCell;

namespace FocusTree.UI.Graph
{
    public class GraphBox : PictureBox
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
                if (ReadOnly) { return Graph.Name + "（只读）"; }
                else if (GraphEdited == true) { return Graph.Name + "（未保存）"; }
                else { return Graph.Name; }
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
        public bool? GraphEdited { get { return Graph?.IsEdit(); } }
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

#if REBUILD

        ///// <summary>
        ///// 节点元尺寸
        ///// </summary>
        //Size NodeMetaSize = new(55, 35);
        ///// <summary>
        ///// 节点空隙元尺寸
        ///// </summary>
        //Size NodePaddingMetaSize = new(10, 30);

#endif

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

        #region ---- 绘图指示器 ----

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
        /// 拖动图像时使用的鼠标参照坐标
        /// </summary>
        Point DragLatticeMouseFlagPoint = new(0, 0);
        bool DragGraph_Flag = false;
        /// <summary>
        /// 拖动节点时使用的鼠标参照坐标
        /// </summary>
        Point DragNodeMouseFlagPoint = new(0, 0);
        /// <summary>
        /// 鼠标拖动节点指示器
        /// </summary>
        bool DragNode_Flag = false;

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
            //DoubleBuffered = true;


            SizeChanged += OnSizeChanged;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;
            Invalidated += UpdateGraph;
            ControlResize.SetTag(this);
        }
        public Graphics gCore 
        {
            get
            {
                gcore?.Flush(); gcore?.Dispose();
                Image ??= new Bitmap(Width, Height);
                gcore = Graphics.FromImage(Image);
                return gcore;
            }
        }
        Graphics gcore;


        private void UpdateGraph(object sender, InvalidateEventArgs e)
        {
#if REBUILD
            //DrawLattice(sender, e);
#else
            if (Graph == null)
            {
                return;
            }
            DrawNodeMap();
            _draw_info($"节点数量：{Graph.NodesCount}，分支数量：{Graph.BranchesCount}",
                new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
#endif
            Update();
        }
        public void DrawInfo(string info)
        {
#if REBUILD
#else
            if (Graph == null)
            {
                return;
            }
            Invalidated -= UpdateGraph;

            DrawNodeMap();
            _draw_info(info,
                new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );

            Invalidate();
            Invalidated += UpdateGraph;
#endif
        }

        #endregion

        #region ---- 绘图 ----
#if REBUILD
#else
        private void DrawNodeMap()
        {
            if (Graph == null) { return; }
            Image ??= new Bitmap(Size.Width, Size.Height);
            var g = Graphics.FromImage(Image);
            g.Clear(Color.White);

            foreach (var node in Graph.GetNodes())
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
        private void _draw_info(string info, Font infoFont, Brush BackBrush, Brush FrontBrush)
        {
            if (Graph == null) { return; }
            Image ??= new Bitmap(Size.Width, Size.Height);
            var g = Graphics.FromImage(Image);
            Rectangle infoRect = new(Bounds.Left, Bounds.Bottom - 100, Bounds.Width, 66);
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
            foreach (var node in Graph.GetNodes())
            {
                if (id != node.ID && Graph.GetNode(id).MetaPoint == node.MetaPoint)
                {
                    return true;
                }
            }
            return false;
        }
#endif
        #endregion

        #region ---- 事件 ----

        //---- OnSizeChanged ----//

        private void OnSizeChanged(object sender, EventArgs args)
        {
            if (Graph == null || Math.Min(Size.Width, Size.Height) <= 0 || Parent.WindowState == FormWindowState.Minimized)
            {
                return;
            }
            Image?.Dispose();
            ControlResize.SetTag(this);
            Image = new Bitmap(Width, Height);
            Lattice.Draw(Graphics.FromImage(Image), ClientRectangle);
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
                    WriteNodeDragFlags(args.Location);
                }
            }

            else if ((args.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (PrevSelectNode == null)
                {
                    OpenGraphContextMenu(args.Button);
                    Parent.UpdateText("打开图像选项");
                }
                else
                {
                    NodeRightClicked();
                }
            }

            else if ((args.Button & MouseButtons.Middle) == MouseButtons.Middle)
            {
                OpenGraphContextMenu(args.Button);
                Parent.UpdateText("打开备份选项");
            }
        }
        private void GraphLeftClicked(Point startPoint)
        {
            DragGraph_Flag = true;
            DragLatticeMouseFlagPoint = startPoint;
            Invalidate();
            Parent.UpdateText("拖动图像");
        }
        private void NodeLeftClicked(int id)
        {
            var data = Graph.GetNode(id);
            var info = $"{data.Name}, {data.Duration}日\n{data.Descript}";
            DrawInfo(info);
            Parent.UpdateText("选择节点");
        }
        private void WriteNodeDragFlags(Point startPoint)
        {
            DragNode_Flag = true;
            var nodeRect = NodeDrawingRect(PrevSelectNode.Value);
            DragNodeMouseFlagPoint = new(startPoint.X - (int)nodeRect.X, startPoint.Y - (int)nodeRect.Y);
        }
        private void NodeRightClicked()
        {
            CloseAllNodeToolDialogs();
            NodeInfoTip.Hide(this);
            SelectedNode = PrevSelectNode;
            RescaleToNode(SelectedNode.Value, false);
            PicNodeContextMenu = new(this, Cursor.Position);
            Invalidate();
            Parent.UpdateText("打开节点选项");
        }
        private void OpenGraphContextMenu(MouseButtons button)
        {
            SelectedNode = PrevSelectNode;
            NodeInfoTip.Hide(this);
            PicGraphContextMenu = new(this, Cursor.Position, button);
            Invalidate();
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
                }
                else
                {
                    //NodeLeftDoubleClicked();
                }
            }
        }
        private void GraphLeftDoubleClicked()
        {
            SelectedNode = null;
            if (ReadOnly)
            {
                if (MessageBox.Show("[202303052340]是否恢复备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CloseAllNodeToolDialogs();
                    FileBackup.Backup<FocusGraph>(FilePath);
                    Graph.SaveToXml(FilePath);
                    ReadOnly = false;
                    SelectedNode = null;
                    RescaleToPanorama();
                }
                Parent.UpdateText("恢复备份");
            }
            Invalidate();
        }
        //private void NodeLeftDoubleClicked()
        //{
        //    SelectedNode = PrevSelectNode;
        //    RescaleToNode(SelectedNode.Value, false);
        //    NodeInfoTip.Hide(this);
        //    Invalidate();
        //}

        //---- OnMouseMove ----//

        static LatticeCell LastCell = new();

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (Graph == null) { return; }

            if (args.Button == MouseButtons.Left && DragGraph_Flag)
            {
                DragGraph(args.Location);
            }
            else if (args.Button == MouseButtons.Left && DragNode_Flag)
            {
                DragNode(args.Location);
            }
            else if (args.Button == MouseButtons.None)
            {
                ShowNodeInfoTip(args.Location);

                var cursor = args.Location;

                var cellPart = LastCell.HightLightCursorTouchOn(gCore, cursor);
                if (cellPart == CellParts.Leave)
                {
                    LatticeCell cell = new(cursor);
                    LastCell = cell;
                }
                Parent.Text = $"W {LatticeCell.Width},H {LatticeCell.Height}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {cursor}, cellPart: {cellPart}, lastCell{new Point(LastCell.LatticedLeft, LastCell.LatticedTop)}";

            }
            
            Invalidate();
            //Parent.Text = $"W {LatticeCell.Width},H {LatticeCell.Height}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {args.Location}, cellPart: {cellPart}, lastCell{new Point(LastCell.LatticedLeft, LastCell.LatticedTop)}";

            

        }
        private void DragGraph(Point newPoint)
        {
            var cellPart = LastCell.HighlightSelection(newPoint);
            if (cellPart == CellParts.Leave)
            {
                LatticeCell cell = new(newPoint);
                LastCell = cell;
            }
            Parent.Text = $"W {LatticeCell.Width},H {LatticeCell.Height}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {newPoint}, cellPart: {cellPart}, lastCell{new Point(LastCell.LatticedLeft, LastCell.LatticedTop)}";


            var diffInWidth = newPoint.X - DragLatticeMouseFlagPoint.X;
            var diffInHeight = newPoint.Y - DragLatticeMouseFlagPoint.Y;
            if (Math.Abs(diffInWidth) >= 1 || Math.Abs(diffInHeight) >= 1)
            {
                Lattice.OriginLeft += newPoint.X - DragLatticeMouseFlagPoint.X;
                Lattice.OriginTop += newPoint.Y - DragLatticeMouseFlagPoint.Y;
                DragLatticeMouseFlagPoint = newPoint;
                Lattice.Draw(gCore, ClientRectangle);
                Invalidate();
            }

            
        }
        private void DragNode(Point newPoint)
        {
#if REBUILD
#else
            NodeInfoTip.Hide(this);
            var dif = new Point(newPoint.X - DragNodeMouseFlagPoint.X, newPoint.Y - DragNodeMouseFlagPoint.Y);
            //if ((int)Math.Abs(dif.X) % (int)(ScalingUnit.X * GScale) == 0 || (int)Math.Abs(dif.Y) % (int)(ScalingUnit.Y * GScale) == 0) 
            {
                Invalidated -= UpdateGraph; 
                DrawNodeMap();
                string info = "拖动";
                _draw_info(info,
                    new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                    new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                    new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                    );
                Graphics g = Graphics.FromImage(Image);
                RectangleF pointerRect = new(
                    dif.X,
                    dif.Y,
                    NodeSize.Width * GScale,
                    NodeSize.Height * GScale
                    );
                g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Red)), pointerRect);
                g.Flush(); g.Dispose();
                Invalidate();
                Invalidated += UpdateGraph;
            }
#endif
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
                DragGraph_Flag = false;
                DragNode_Flag = false;
            }
        }

        //---- OnMouseWheel ----//

        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            //if (Graph == null) { return; }
            var diffInWidth = args.Location.X - Width / 2;
            var diffInHeight = args.Location.Y - Height / 2;
            Lattice.OriginLeft += diffInWidth / LatticeCell.Width * Lattice.DrawRect.Width / 200;
            Lattice.OriginTop += diffInHeight / LatticeCell.Height * Lattice.DrawRect.Height / 200;

            LatticeCell.Width += args.Delta / 100 * Lattice.DrawRect.Width / 200;
            LatticeCell.Height += args.Delta / 100 * Lattice.DrawRect.Width / 200;

            Lattice.Draw(gCore, ClientRectangle);
            Invalidate();
            Parent.UpdateText("打开节点选项");
        }

        //---- Public ----//

        /// <summary>
        /// 检查预选择节点
        /// </summary>
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
        /// 坐标是否处于任何节点的绘图区域中
        /// </summary>
        /// <param name="location">指定坐标 </param>
        /// <returns>坐标所处于的节点id，若没有返回null</returns>
        private int? PointInAnyNodeDrawingRect(Point location)
        {
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
        /// 节点的绘图中心
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private PointF NodeDrawingCenter(int id)
        {
            var rect = NodeDrawingRect(id);
            return new(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
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
        /// <param name="id">节点ID</param>
        /// <param name="zoom">是否聚焦</param>
        private void RescaleToNode(int id, bool zoom)
        {
            var point = Graph.GetNode(id).MetaPoint;
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

        /// <summary>
        /// 保存
        /// </summary>
        public void SaveGraph()
        {
            if (Graph == null || ReadOnly || !Graph.IsEdit()) { return; }
            FileBackup.Backup<FocusGraph>(FilePath);
            Graph.SaveToXml(FilePath);
            Graph.Latest = Graph.Format();
            Invalidate();
        }
        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="path"></param>
        public void SaveAsNew(string path)
        {
            if (Graph == null) { return; }
            FilePath = path;
            Graph.ClearCache();
            Graph.SaveToXml(path);
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
            ReadOnly = Graph == null ? false : Graph.IsBackupFile(path);
            if (!ReadOnly) { FilePath = path; }
            Graph?.ClearCache();
            Graph = XmlIO.LoadFromXml<FocusGraph>(path);
            FileBackup.Backup<FocusGraph>(FilePath);
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
            if (Graph == null) { return; }
            RescaleToPanorama();
            Invalidate();
        }
        /// <summary>
        /// 聚焦先前选中的节点
        /// </summary>
        public void CamLocateSelected()
        {
            if (Graph == null || SelectedNode == null) { return; }
            RescaleToNode(SelectedNode.Value, true);
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
            Parent.UpdateText("重排节点ID");
        }
        public void ResetNodeMetaPoints()
        {
            if (Graph == null) { return; }
            Graph.ResetNodeMetaPoints(true);
            Invalidate();
            Parent.UpdateText("自动排版");
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
