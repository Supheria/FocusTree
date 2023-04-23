using FocusTree.Data;
using FocusTree.Data.Focus;
using FocusTree.IO;
using FocusTree.IO.FileManege;
using FocusTree.UI.Controls;
using FocusTree.UI.Graph;
using FocusTree.UI.NodeToolDialogs;
using Newtonsoft.Json;
using System.Drawing;
using System.Security.Permissions;
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

        #endregion

        #region ==== 状态指示器 ====

        public string FileName
        {
            get
            {
                if (ReadOnly) { return Graph.Name + "（只读）"; }
                else if (GraphEdited == true) { return Graph.Name + "（未保存）"; }
                else { return Graph.Name; }
            }
        }
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

        #endregion

        #region ---- 绘图工具 ----

        /// <summary>
        /// 核心 GDI（Image绘制Graph专用，因为调用时会先 Clear Image）
        /// </summary>
        public readonly Graphics gCore;
        /// <summary>
        /// 拖动图像时使用的鼠标参照坐标
        /// </summary>
        Point DragMouseFlagPoint = new(0, 0);
        bool DragGraph_Flag = false;
        /// <summary>
        /// 拖动节点时使用的鼠标参照坐标
        /// </summary>
        Point DragNodeMouseFlagPoint = new(0, 0);
        /// <summary>
        /// 鼠标拖动节点指示器
        /// </summary>
        bool DragNode_Flag = false;
        /// <summary>
        /// 背景图片
        /// </summary>
        Image BackImage;
        /// <summary>
        /// 背景图片在当前 Client Rect 范围中的缓存
        /// </summary>
        Image BackImageCacher;

        #endregion


        //===== 方法 =====//

        #region ---- 初始化 ----

        public GraphBox(MainForm mainForm)
        {
            base.Parent = Parent = mainForm;
            NodeInfo = new InfoDialog(this);
            //SizeMode = PictureBoxSizeMode.Zoom;
            //Dock = DockStyle.Fill;
            //DoubleBuffered = true;
            BackColor = Color.White;
            var workArea = Screen.GetWorkingArea(this);
            Image = new Bitmap(workArea.Width, workArea.Height);
            gCore = Graphics.FromImage(Image);

            BackImage = Image.FromFile("Background.jpg");

            LatticeCell.Width = 30;
            LatticeCell.Height = 30;
            LatticeCell.NodePaddingZoomFactor = new(0.3f, 0.5f);

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
        /// 根据给定矩形将背景图片缓存的矩形部分填充到 Image
        /// </summary>
        /// <param name="rect"></param>
        public void DrawBackground(Rectangle rect)
        {
            DrawBackground(new Rectangle[] { rect });
        }
        /// <summary>
        /// 根据给定矩形数组将背景图片缓存的矩形部分填充到 Image
        /// </summary>
        public void DrawBackground(Rectangle[] rects)
        {
            
            foreach(var rect in rects)
            {
                gCore.DrawImage(BackImageCacher, rect, rect, GraphicsUnit.Pixel);
            }
            gCore.Flush();
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
                        GraphDrawer.UploadRequireLine(GraphDrawer.NodeRequire[color], focus, require);
                    }
                    color++;
                }
                var brush = id == SelectedNode ? GraphDrawer.NodeBG_Selected : GraphDrawer.NodeBG_Normal;
                GraphDrawer.UploadNodeMap(focus);
            }
        }
        public void DrawNodeMapInfo()
        {
            if (Graph == null) { return; }
            DrawInfo($"节点数量：{Graph.NodesCount}，分支数量：{Graph.BranchesCount}",
                new Font(GraphDrawer.NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
        }
        public void DrawInfo(string info)
        {
            if (Graph == null) { return; }
            DrawInfo(info,
                new Font(GraphDrawer.NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
        }
        private void DrawInfo(string info, Font infoFont, Brush BackBrush, Brush FrontBrush)
        {
            var g = Graphics.FromImage(Image);
            Rectangle infoRect = new(Bounds.Left, Bounds.Bottom - 100, Bounds.Width, 66);
            g.FillRectangle(BackBrush, infoRect);
            g.DrawString(
                info,
                infoFont,
                FrontBrush,
                infoRect,
                GraphDrawer.NodeFontFormat);
        }
        /// <summary>
        /// 获取 Graph 的真实矩形
        /// </summary>
        /// <returns></returns>
        public Rectangle GetGraphRealRect()
        {
            var rect = Graph.GetGraphMetaRect();
            return new(rect.Left * LatticeCell.Width, 
                rect.Top * LatticeCell.Height,
                rect.Width * LatticeCell.Width,
                rect.Height * LatticeCell.Height
                );
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
            SetBackImageCacher();
            DrawBackground(ClientRectangle); 
            Lattice.SetBounds(ClientRectangle);
            Lattice.Draw(gCore);
            Invalidate();
        }
        /// <summary>
        /// 根据当前 Client Rect 大小设置背景图片缓存
        /// </summary>
        private void SetBackImageCacher()
        {
            var bkWidth = Width;
            var bkHeight = Height;
            float sourceRatio = (float)BackImage.Width / (float)BackImage.Height;
            float clientRatio = (float)Width / (float)Height;
            if (sourceRatio < clientRatio)
            {
                bkWidth = Width;
                bkHeight = (int)(Width / sourceRatio);
            }
            else if (sourceRatio > clientRatio)
            {
                bkHeight = Height;
                bkWidth = (int)(Height * sourceRatio);
            }
            if (BackImageCacher != null && bkWidth == BackImageCacher.Width && bkHeight == BackImageCacher.Height) { return; }
            BackImageCacher?.Dispose();
            BackImageCacher = new Bitmap(bkWidth, bkHeight);
            var g = Graphics.FromImage(BackImageCacher);
            g.DrawImage(BackImage, 0, 0, bkWidth, bkHeight);
            g.Flush(); g.Dispose();
        }
        private void SetImageBackColorWhileResized()
        {
            var prevSize = ControlResize.GetTag(this);
            var diffWidth = Width - prevSize.Width;
            var diffHeight = Height - prevSize.Height;
            //if (diffWidth > 0)
            //{
            //    Image.W
            //    Rectangle fillRect = new(Left + prevSize.Width, Top, diffWidth, prevSize.Height);
            //    gCore.
            //}
            //if (diffHeight > 0)
            //{

            //}
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
                GraphDrawer.DrawNode(gCore, focus, GraphDrawer.NodeBG_Normal);
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
            if (Math.Abs(diffInWidth) > 0 || Math.Abs(diffInHeight) > 0)
            {
                Lattice.OriginLeft += newPoint.X - DragMouseFlagPoint.X;
                Lattice.OriginTop += newPoint.Y - DragMouseFlagPoint.Y;
                DragMouseFlagPoint = newPoint;
#if BUILD
                //Rectangle cutRect = new(Lattice.OriginLeft, Lattice.OriginTop, )
                //gCore.DrawImage(ImageCacher, ClientRectangle, )
#endif
                UploadNodeMap();
                Lattice.Draw(gCore);
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
                LastCellDrawer = (g) => g.FillRectangle(brush, rect);
            }
            Lattice.Drawing += LastCellDrawer;
            Lattice.Draw(gCore);
            //Parent.Text = $"W {LatticeCell.Width},H {LatticeCell.Height}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {cursor}, cellPart: {cellPart}, lastCell{new Point(LastCell.LatticedLeft, LastCell.LatticedTop)}";
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
                    DragNode_Flag = false;
                    Lattice.DrawBackLattice = false;
                    Lattice.Drawing -= LastCellDrawer;
                    //UploadNodeMap();
                    Lattice.Draw(gCore);
                }
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

            Lattice.SetBounds(ClientRectangle);
#if BUILD
#else
            Lattice.Draw(gCore);
#endif
            DrawNodeMapInfo();
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
            if(!Graph.ContainLatticedPoint(cell.LatticedPoint, out var id)) { return null; }
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
            if (Lattice.DrawRect.Width < (gRect.Width) * LatticeCell.SizeMin.Width)
            {
                LatticeCell.Width = LatticeCell.SizeMin.Width;
                Parent.Width = (gRect.Width + 1) * LatticeCell.SizeMin.Width + Parent.Width - Width;
            }
            if (Lattice.DrawRect.Height < (gRect.Height) * LatticeCell.SizeMin.Height)
            {
                LatticeCell.Height = LatticeCell.SizeMin.Height;
                Parent.Height = (gRect.Height + 1) * LatticeCell.SizeMin.Height + Parent.Height - Height;
            }
            Lattice.SetBounds(ClientRectangle);
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
            Lattice.SetBounds(ClientRectangle);
            Lattice.Draw(gCore);
        }
        /// <summary>
        /// 缩放居中至节点
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="zoom">是否聚焦</param>
        private void RescaleToNode(int id, bool zoom)
        {
            if (Graph == null) { return; }
            var drRect = Lattice.DrawRect;
            if(zoom)
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
            Lattice.SetBounds(ClientRectangle);
            Lattice.Draw(gCore);
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
#if BUILD
#else
            RescaleToPanorama();
#endif
            DrawNodeMapInfo();
            Invalidate();
        }
        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            Graph.Undo();
            UploadNodeMap();
            Lattice.Draw(gCore);
            Invalidate();
        }
        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            Graph.Redo();
            UploadNodeMap();
            Lattice.Draw(gCore);
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
            Lattice.Draw(gCore);
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
