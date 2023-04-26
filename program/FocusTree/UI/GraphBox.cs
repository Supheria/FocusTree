using FocusTree.Data;
using FocusTree.Data.Focus;
using FocusTree.IO;
using FocusTree.IO.FileManege;
using FocusTree.UI.Controls;
using FocusTree.UI.Graph;
using FocusTree.UI.NodeToolDialogs;
using FocusTree.UI.test;
using static System.Formats.Asn1.AsnWriter;

namespace FocusTree.UI
{
    public class GraphBox : PictureBox
    {
        #region ==== 基本变量 ====

        /// <summary>
        /// 元数据（数据存储结构）
        /// </summary>
        public FocusGraph Graph { get; private set; }
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

        #region ---- 关联控件 ----

        readonly new MainForm Parent;
        /// <summary>
        /// 工具对话框集（给父窗口下拉菜单“窗口”用）
        /// </summary>
        public readonly Dictionary<string, ToolDialog> ToolDialogs = new()
        {
            ["国策信息"] = new ToolDialog()
        };
        /// <summary>
        /// 国策信息对话框
        /// </summary>
        NodeInfoDialog NodeInfo
        {
            get { return (NodeInfoDialog)ToolDialogs["国策信息"]; }
            init { ToolDialogs["国策信息"] = value; }
        }
        /// <summary>
        /// 节点信息浮标
        /// </summary>
        readonly ToolTip NodeInfoTip = new();

        #endregion

        #region ==== 事件指示器 ====

        /// <summary>
        /// 已选中的节点
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
        /// <summary>
        /// 图像只读不可编辑
        /// </summary>
        public bool ReadOnly { get; private set; }
        /// <summary>
        /// 图像拖动指示器
        /// </summary>
        bool DragGraph_Flag = false;
        /// <summary>
        /// 拖动节点指示器
        /// </summary>
        bool DragNode_Flag = false;
        /// <summary>
        /// 绘制了底部信息框
        /// </summary>
        bool DrawnInfoBrand = false;

        #endregion

        #region ---- 绘图工具 ----

        /// <summary>
        /// Image 专用 GDI
        /// </summary>
        //public readonly Graphics gCore;
        /// <summary>
        /// 信息展示条区域
        /// </summary>
        Rectangle InfoBrandRect { get => new(Left, Bottom - 100, Width, 75); }
        /// <summary>
        /// 鼠标移动灵敏度（值越大越迟顿）
        /// </summary>
        static int MouseMoveSensibility = 20;
        /// <summary>
        /// 拖动事件使用的鼠标参照坐标
        /// </summary>
        Point DragMouseFlagPoint = new(0, 0);
        Rectangle LatticeBound { get=> new(Left, Top, Width, InfoBrandRect.Top - Top - 30); }

        #endregion


        //===== 方法 =====//

        #region ---- 初始化 ----

        public GraphBox(MainForm mainForm)
        {
            base.Parent = Parent = mainForm;
            NodeInfo = new NodeInfoDialog(this);
            //SizeMode = PictureBoxSizeMode.Zoom;
            //DoubleBuffered = true;

            SizeChanged += OnSizeChanged;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;
            ControlResize.SetTag(this);
        }

        #endregion

        #region ==== 绘图 ====

        /// <summary>
        /// 重绘背景
        /// </summary>
        public void RedrawBackground()
        {
            if (Lattice.DrawBackLattice)
            {
                GraphDrawer.DrawFillBackImage(Image, ClientRectangle);
                return;
            }
            if (Graph != null)
            {
                GraphDrawer.RedrawDrawnCells(Image);
            }
            if (DrawnInfoBrand)
            {
                GraphDrawer.DrawRectWithBackImage(Image, InfoBrandRect);
                DrawnInfoBrand = false;
            }
        }
        public void DrawLattice()
        {
            GraphDrawer.SetRedrawBuffer();
            Lattice.Draw(Image);
        }
        /// <summary>
        /// 将节点绘制上载到栅格绘图委托（初始化节点列表时仅需上载第一次，除非节点列表或节点关系或节点位置信息发生变更才重新上载）
        /// </summary>
        private void UploadNodeMap()
        {
            if (Graph == null) { return; }
            Lattice.DrawingClear();
            foreach (var id in Graph.IdList)
            {
                var focus = Graph.GetFocus(id);
                int color = 0; //不同需求要变色
                foreach (var requires in focus.Requires)
                {
                    foreach (var requireId in requires)
                    {
                        var require = Graph.GetFocus(requireId);
                        GraphDrawer.UploadDrawerRequireLine(color, focus, require);
                    }
                    color++;
                }
                GraphDrawer.UploadDrawerNode(focus);
            }
        }
        public void DrawNodeMapInfo()
        {
            if (Graph == null) { return; }
            DrawInfo($"节点数量：{Graph.NodesCount}，分支数量：{Graph.BranchesCount}",
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
        }
        public void DrawInfo(string info)
        {
            if (Graph == null) { return; }
            DrawInfo(info,
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
        }
        private void DrawInfo(string info, Brush BackBrush, Brush FrontBrush)
        {
            var g = Graphics.FromImage(Image);
            Rectangle infoRect = new(Bounds.Left, Bounds.Bottom - 100, Bounds.Width, 66);
            g.FillRectangle(BackBrush, infoRect);
            g.DrawString(
                info,
                new Font(GraphDrawer.InfoFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                FrontBrush,
                infoRect,
                GraphDrawer.NodeFontFormat);
            DrawnInfoBrand = true;
        }

        #endregion

        #region ---- 事件 ----

        //---- OnSizeChanged ----//

        private void OnSizeChanged(object sender, EventArgs args)
        {
            if (Math.Min(Size.Width, Size.Height) <= 0 || Parent.WindowState == FormWindowState.Minimized)
            {
                return;
            }
            Image?.Dispose();
            Image = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            GraphDrawer.DrawFillBackImage(Image, ClientRectangle);
            Lattice.SetBounds(LatticeBound);
            DrawLattice();
            Invalidate();
        }

        //---- OnMouseDown ----//

        private void OnMouseDown(object sender, MouseEventArgs args)
        {
            //if (Graph == null) { return; }

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
            DragMouseFlagPoint = startPoint;
            Invalidate();
            Parent.UpdateText("拖动图像");
        }
        private void NodeLeftClicked(int id)
        {
            DragNode_Flag = true;
            var data = Graph.GetFocus(id);
            var info = $"{data.Name}, {data.Duration}日\n{data.Descript}";
            DrawInfo(info);
            Parent.UpdateText("选择节点");
        }
        private void NodeRightClicked()
        {
            CloseAllNodeToolDialogs();
            NodeInfoTip.Hide(this);
            SelectedNode = PrevSelectNode;
            RescaleToNode(SelectedNode.Value, false);
            new NodeContextMenu(this, Cursor.Position);
            Invalidate();
            Parent.UpdateText("打开节点选项");
        }
        private void OpenGraphContextMenu(MouseButtons button)
        {
            SelectedNode = PrevSelectNode;
            NodeInfoTip.Hide(this);
            new GraphContextMenu(this, Cursor.Position, button);
            Invalidate();
        }

        //---- OnMouseDoubleClick ----//

        private void OnMouseDoubleClick(object sender, MouseEventArgs args)
        {
            CheckPrevSelect();

            if ((args.Button & MouseButtons.Left) == MouseButtons.Left && PrevSelectNode == null)
            {
                GraphLeftDoubleClicked();
            }

            Invalidate();
        }
        private void GraphLeftDoubleClicked()
        {
            if (SelectedNode != null)
            {
                var focus = Graph.GetFocus(SelectedNode.Value);
                GraphDrawer.DrawNode((Bitmap)Image, focus);
                SelectedNode = null;
            }
            if (!ReadOnly || MessageBox.Show("[202303052340]是否恢复备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
            CloseAllNodeToolDialogs();
            FileBackup.Backup<FocusGraph>(FilePath);
            Graph.SaveToXml(FilePath);
            ReadOnly = false;
            SelectedNode = null;
            RescaleToPanorama();
            Parent.UpdateText("恢复备份");
        }

        //---- OnMouseMove ----//

        /// <summary>
        /// 光标所处的节点
        /// </summary>
        LatticeCell CellCursorOn = new();
        /// <summary>
        /// 上次光标所处的节点部分
        /// </summary>
        LatticeCell.Parts LastCellPart = new();
        CellDrawer LastCellDrawer;

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
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
            }

            Invalidate();
        }
        private void DragGraph(Point newPoint)
        {
            var diffInWidth = newPoint.X - DragMouseFlagPoint.X;
            var diffInHeight = newPoint.Y - DragMouseFlagPoint.Y;
            if (Math.Abs(diffInWidth) > MouseMoveSensibility || Math.Abs(diffInHeight) > MouseMoveSensibility)
            {
                RedrawBackground();
                Lattice.OriginLeft += (newPoint.X - DragMouseFlagPoint.X) / MouseMoveSensibility * LatticeCell.Width;
                Lattice.OriginTop += (newPoint.Y - DragMouseFlagPoint.Y) / MouseMoveSensibility * LatticeCell.Height;
                DragMouseFlagPoint = newPoint;
                DrawLattice();
                DrawNodeMapInfo();
            }
        }
        CellDrawer cache;
        private void DragNode(Point newPoint)
        {
            NodeInfoTip.Hide(this);
            Lattice.DrawBackLattice = true;
            var part = CellCursorOn.GetInnerPartPointOn(newPoint);
            if (Graph.ContainLatticedPoint(CellCursorOn.LatticedPoint, out var id))
            {
                Lattice.Drawing -= GraphDrawer.NodeDrawerCatalog[id];
                cache = GraphDrawer.NodeDrawerCatalog[id];
            }
            if (part == LatticeCell.Parts.Leave)
            {
                if (id != -1 && Lattice.RectWithin(CellCursorOn.InnerPartRealRects[LatticeCell.Parts.Node], out var sv))
                {
                    var rect = sv;
                    //CellDrawer drawer = (g) => g.FillRectangle(GraphDrawer.NodeBG_Normal, rect); 
                    //GraphDrawer.NodeDrawerCatalog[id] = (null, drawer);
                    Lattice.Drawing += cache;
                }
                LastCellPart = part;
                CellCursorOn = new(newPoint);
                return;
            }
            if (part == LastCellPart) { return; }
            LastCellPart = part;
            SolidBrush brush = new SolidBrush(Color.FromArgb(100, Color.Gray));
            if (part == LatticeCell.Parts.Node)
            {
                brush = new SolidBrush(Color.FromArgb(150, Color.Orange));
            }
            else if (id != -1 && Lattice.RectWithin(CellCursorOn.InnerPartRealRects[LatticeCell.Parts.Node], out var sv))
            {
                var rect = sv;
                //CellDrawer drawer = (g) => g.FillRectangle(GraphDrawer.NodeBG_Normal, rect);
                //GraphDrawer.NodeDrawerCatalog[id] = (null, drawer);
                Lattice.Drawing += cache;
            }
            Lattice.Drawing -= LastCellDrawer;
            if (Lattice.RectWithin(CellCursorOn.InnerPartRealRects[part], out var saveRect))
            {
                var rect = saveRect;
                LastCellDrawer = (Image) =>
                {
                    var g = Graphics.FromImage(Image);
                    g.FillRectangle(brush, rect);
                    g.Flush(); g.Dispose();
                };
            }
            Lattice.Drawing += LastCellDrawer;
            RedrawBackground();
            DrawLattice();
            Parent.Text = $"W {LatticeCell.Width},H {LatticeCell.Height}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {newPoint}, cellPart: {LastCellPart}";
            Parent.Text = $"cell left: {CellCursorOn.LatticedLeft}, cell top: {CellCursorOn.LatticedTop}, last part: {LastCellPart}, part: {part}";
        }

        private void ShowNodeInfoTip(Point location)
        {
            var node = PointInAnyNode(location);
            if (node != null)
            {
                NodeInfoTip.BackColor = Color.FromArgb(0, Color.AliceBlue);
                NodeInfoTip.Show($"{Graph.GetFocus(node.Value).Name}\nID: {node.Value}", this, location.X + 10, location.Y);
            }
            else
            {
                NodeInfoTip.Hide(this);
            }
        }

        //---- OnMouseUp ----//

        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            //if (Graph == null) { return; }
            // 用于拖动事件
            if (args.Button == MouseButtons.Left)
            {
                DragGraph_Flag = false;
                if (DragNode_Flag)
                {
                    RedrawBackground();
                    DragNode_Flag = false;
                    Lattice.DrawBackLattice = false;
                    Lattice.Drawing -= LastCellDrawer;
                    Lattice.Drawing -= cache;
                    Lattice.Drawing += cache;
                    DrawLattice();
                }
            }
        }

        //---- OnMouseWheel ----//

        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            RedrawBackground();
            //if (Graph == null) { return; }
            var diffInWidth = args.Location.X - Width / 2;
            var diffInHeight = args.Location.Y - Height / 2;
            Lattice.OriginLeft += diffInWidth / LatticeCell.Width * Lattice.DrawRect.Width / 200;
            Lattice.OriginTop += diffInHeight / LatticeCell.Height * Lattice.DrawRect.Height / 200;

            LatticeCell.Width += args.Delta / 100 * Lattice.DrawRect.Width / 200;
            LatticeCell.Height += args.Delta / 100 * Lattice.DrawRect.Width / 200;

            Lattice.SetBounds(LatticeBound);

            DrawLattice();
            //DrawNodeMapInfo();
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
            PrevSelectNode = PointInAnyNode(clickPos);
        }
        private void CloseAllNodeToolDialogs()
        {
            ToolDialogs.ToList().ForEach(x => x.Value.Close());
        }

        #endregion

        #region ---- 坐标工具 ----

        /// <summary>
        /// 坐标是否处于任何节点的绘图区域中
        /// </summary>
        /// <param name="location">指定坐标 </param>
        /// <returns>坐标所处于的节点id，若没有返回null</returns>
        private int? PointInAnyNode(Point point)
        {
            if (Graph == null) { return null; }
            LatticeCell cell = new(point);
            if (!Graph.ContainLatticedPoint(cell.LatticedPoint, out var id)) { return null; }
            var part = cell.GetInnerPartPointOn(point);
            if (part != LatticeCell.Parts.Node) { return null; }
            return id;
        }

        #endregion

        #region ==== 镜头操作 ====

        /// <summary>
        /// 外部调用 - 缩放居中至全景
        /// </summary>
        public void CamLocatePanorama()
        {
            if (Graph == null) { return; }
            RescaleToPanorama();
            Invalidate();
        }
        /// <summary>
        /// 外部调用 - 聚焦至已选中的节点
        /// </summary>
        public void CamLocateSelected()
        {
            if (Graph == null || SelectedNode == null) { return; }
            RescaleToNode(SelectedNode.Value, true);
            Invalidate();
        }
        /// <summary>
        /// 缩放居中至全景
        /// </summary>
        private void RescaleToPanorama()
        {
            if (Graph == null) { return; }
            var gRect = Graph.GetGraphMetaRect();
            //
            // 自适应大小
            //
            Parent.ForceResize = true;
            if (Lattice.DrawRect.Width < (gRect.Width) * LatticeCell.SizeMin.Width)
            {
                LatticeCell.Width = LatticeCell.SizeMin.Width;
                Parent.Width = (gRect.Width + 1) * LatticeCell.SizeMin.Width + Parent.Width - Width;
            }
            if (Lattice.DrawRect.Height < (gRect.Height) * LatticeCell.SizeMin.Height)
            {
                LatticeCell.Height = LatticeCell.SizeMin.Height;
                Parent.Height = (gRect.Height + 1) * LatticeCell.SizeMin.Height + Parent.Height - Height + InfoBrandRect.Height + 30/*30 is blank between Lattice's DrawRect.Bottom and InfoBrandRect.Top*/;
            }
            Parent.ForceResize = false;
            RedrawBackground();
            Lattice.SetBounds(LatticeBound);
            //
            //
            //
            var cellWidth = Lattice.DrawRect.Width / (gRect.Width + 1);
            var cellHeight = Lattice.DrawRect.Height / (gRect.Height + 1);
            LatticeCell.Width = LatticeCell.Height = Math.Min(cellWidth, cellHeight);
            int GraphCenterX = gRect.Left + gRect.Width * LatticeCell.Width / 2;
            int GraphCenterY = gRect.Top + gRect.Height * LatticeCell.Height / 2;
            int WidthCenterDiff = (Lattice.DrawRect.Left + Lattice.DrawRect.Width / 2) - GraphCenterX;
            int HeightCenterDiff = (Lattice.DrawRect.Top + Lattice.DrawRect.Height / 2) - GraphCenterY;
            Lattice.OriginLeft = WidthCenterDiff;
            Lattice.OriginTop = HeightCenterDiff;
            Lattice.SetBounds(LatticeBound);
            DrawLattice();
        }
        /// <summary>
        /// 缩放居中至节点
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="zoom">是否聚焦</param>
        private void RescaleToNode(int id, bool zoom)
        {
            if (Graph == null) { return; }
            RedrawBackground();
            var drRect = Lattice.DrawRect;
            if (zoom)
            {
                LatticeCell.Width = LatticeCell.SizeMax.Width;
                LatticeCell.Height = LatticeCell.SizeMax.Height;
            }
            LatticeCell cell = new(Graph.GetFocus(id));
            int NodeCenterX = cell.NodeRealLeft + LatticeCell.NodeWidth / 2;
            int NodeCenterY = cell.NodeRealTop + LatticeCell.NodeHeight / 2;
            int WidthCenterDiff = (drRect.Left + drRect.Width / 2) - NodeCenterX;
            int HeightCenterDiff = (drRect.Top + drRect.Height / 2) - NodeCenterY;
            Lattice.OriginLeft += WidthCenterDiff;
            Lattice.OriginTop += HeightCenterDiff;
            Lattice.SetBounds(LatticeBound);
            DrawLattice();
            Cursor.Position = Parent.PointToScreen(new Point(
                Bounds.X + Bounds.Width / 2,
                Bounds.Y + Bounds.Height / 2
                ));
        }

        #endregion

        #region ==== 读写操作调用 ====

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
            UploadNodeMap();
            RescaleToPanorama();
            Invalidate();
        }
        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            Graph.Undo();
            UploadNodeMap();
            RedrawBackground();
            DrawLattice();
            Invalidate();
        }
        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            Graph.Redo();
            UploadNodeMap();
            RedrawBackground();
            DrawLattice();
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

        #region ==== 节点操作调用 ====

        /// <summary>
        /// 弹出节点信息对话框
        /// </summary>
        public void ShowNodeInfo()
        {
            NodeInfo.Show();
        }
        /// <summary>
        /// 删除节点
        /// </summary>
        public void RemoveNode()
        {
            if (SelectedNode == null) { return; }
            Graph.RemoveNode(SelectedNode.Value);
            Graph.EnqueueHistory();
            SelectedNode = null;
            UploadNodeMap();
            RedrawBackground();
            DrawLattice();
            Invalidate();
        }
        /// <summary>
        /// 获取选中的节点的国策信息
        /// </summary>
        /// <returns></returns>
        public FocusData? GetSelectedNodeData()
        {
            if (SelectedNode == null) { return null; }
            return Graph.GetFocus(SelectedNode.Value);
        }
        /// <summary>
        /// 获取选中的节点的绘图中心
        /// </summary>
        /// <returns></returns>
        public Point GetSelectedNodeCenterOnScreen()
        {
            LatticeCell cell = new(Graph.GetFocus(SelectedNode.Value));
            var rect = cell.NodeRealRect;
            var point = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            return PointToScreen(point);
        }

        #endregion

        #region ==== 图像操作调用 ====

        /// <summary>
        /// 重排节点 id
        /// </summary>
        public void ReorderNodeIds()
        {
            if (Graph == null) { return; }
            Graph.ReorderNodeIds();
            Graph.EnqueueHistory();
            Invalidate();
            Parent.UpdateText("重排节点ID");
        }
        /// <summary>
        /// 自动排版
        /// </summary>
        public void ResetNodeLatticedPoints()
        {
            if (Graph == null) { return; }
            Graph.ResetNodeLatticedPoints();
            Graph.EnqueueHistory();
            UploadNodeMap();
            RescaleToPanorama();
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
