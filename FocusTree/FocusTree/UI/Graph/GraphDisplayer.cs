//#define MOUSE_DRAG_FREE
using FocusTree.Data.Focus;
using FocusTree.Graph;
using FocusTree.IO.FileManage;
using FocusTree.UI.Controls;
using FocusTree.UI.NodeToolDialogs;

namespace FocusTree.UI
{
    public class GraphDisplayer : PictureBox
    {
        #region ---- 关联控件 ----

        public readonly new GraphForm Parent;
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
        public FocusData? SelectedNode
        {
            get => selectedNode;
            private set
            {
                selectedNode = value;
                PrevSelectNode = null;
            }
        }
        FocusData? selectedNode;
        /// <summary>
        /// 预选中的节点
        /// </summary>
        FocusData? PrevSelectNode;
        /// <summary>
        /// 图像拖动指示器
        /// </summary>
        bool DragGraph_Flag = false;
        /// <summary>
        /// 拖动节点指示器
        /// </summary>
        bool DragNode_Flag = false;

        #endregion

        #region ---- 绘图工具 ----

        /// <summary>
        /// 信息展示条区域
        /// </summary>
        Rectangle InfoBrandRect { get => new(Left, Bottom - 100, Width, 75); }
        /// <summary>
        /// 鼠标移动灵敏度（值越大越迟顿）
        /// </summary>
#if MOUSE_DRAG_FREE
        static int MouseMoveSensibility = 1;
#else
        static int MouseMoveSensibility = 20;
#endif
        /// <summary>
        /// 拖动事件使用的鼠标参照坐标
        /// </summary>
        Point DragMouseFlagPoint = new(0, 0);
        /// <summary>
        /// 格元放置边界
        /// </summary>
        public Rectangle LatticeBound 
        {
            get
            {
                var left = Left + 30;
                var right = left + Width - 60;
                var top = Top;
                var bottom = InfoBrandRect.Top - 30;
                var bkRight = Background.Size.Width;
                var bkBottom = Background.Size.Height;
                if (right > bkRight)
                {
                    right = bkRight;
                }
                if (bottom > bkBottom)
                {
                    bottom = bkBottom;
                }
                return new(left, top, right - left, bottom - top);
            }
        }
        /// <summary>
        /// 刷新时调用的绘图委托（两层）
        /// </summary>
        public DrawLayers OnRefresh = new(2);
        /// <summary>
        /// 是否绘制背景栅格
        /// </summary>
        public bool DrawBackLattice = false;

        #endregion

        #region ---- 初始化 ----

        public GraphDisplayer(GraphForm mainForm)
        {
            base.Parent = Parent = mainForm;
            NodeInfo = new NodeInfoDialog(this);

            SizeChanged += GraphDisplayer_SizeChanged;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;
        }

        #endregion

        #region ==== 绘图 ====

        /// <summary>
        /// 将节点绘制上载到栅格绘图委托（初始化节点列表时仅需上载第一次，除非节点列表或节点关系或节点位置信息发生变更才重新上载）
        /// </summary>
        private void UploadNodeMap()
        {
            OnRefresh.Clear();
            foreach (var focus in GraphBox.FocusList)
            {
                foreach (var requires in focus.Requires)
                {
                    foreach (var requireId in requires)
                    {
                        var require = GraphBox.GetFocus(requireId);
                        OnRefresh += (1, (image) => GraphDrawer.DrawRequireLine(image, focus.LatticedPoint, require.LatticedPoint));
                    }
                }
                OnRefresh += (0, (image) => GraphDrawer.DrawFocusNodeNormal(image, focus));
            }
        }
        public void DrawNodeMapInfo()
        {
            DrawInfo($"节点数量：{GraphBox.NodeCount}，分支数量：{GraphBox.BranchCount}",
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
        }
        public void DrawInfo(string info)
        {
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
        }

        #endregion

        #region ---- 事件 ----

        private void GraphDisplayer_SizeChanged(object sender, EventArgs e)
        {
            Image?.Dispose();
            Image = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Lattice.DrawRect = LatticeBound;
            Refresh();
        }

        //---- OnMouseDown ----//

        private void OnMouseDown(object sender, MouseEventArgs args)
        {
            if (CheckPrevSelect())
            {
                if (args.Button == MouseButtons.Left)
                {
                    NodeLeftClicked(PrevSelectNode.Value);
                }
                else if (args.Button == MouseButtons.Right)
                {
                    NodeRightClicked();
                }
            }
            else
            {
                if (args.Button == MouseButtons.Left)
                {
                    GraphLeftClicked(args.Location);
                }
                else if (args.Button == MouseButtons.Right)
                {
                    OpenGraphContextMenu(args.Button);
                    Parent.UpdateText("打开图像选项");
                }
                else if (args.Button == MouseButtons.Middle)
                {
                    OpenGraphContextMenu(args.Button);
                    Parent.UpdateText("打开备份选项");
                }
            }
        }
        private void GraphLeftClicked(Point startPoint)
        {
            DragGraph_Flag = true;
            DragMouseFlagPoint = startPoint;
            Invalidate();
            Parent.UpdateText("拖动图像");
        }
        Bitmap LineMapCache;
        Bitmap BackgroundCache;
        private void NodeLeftClicked(FocusData focus)
        {
            DragNode_Flag = true;
            LineMapCache = new(Image.Width, Image.Height);
            BackgroundCache = new(Image.Width, Image.Height);
            DrawBackLattice = true;
            Refresh();
            OnRefresh.Invoke(LineMapCache, 1);
            Background.Redraw(BackgroundCache);
            Lattice.Draw(BackgroundCache);
            OnRefresh.Invoke(BackgroundCache, 0);
            var info = $"{focus.Name}, {focus.Duration}日\n{focus.Descript}";
            DrawInfo(info);
            Parent.UpdateText("选择节点");
        }
        private void NodeRightClicked()
        {
            CloseAllNodeToolDialogs();
            NodeInfoTip.Hide(this);
            SelectedNode = PrevSelectNode;
            CameraLocateSelectedNode(false);
            new NodeContextMenu(this, Cursor.Position);
            Parent.UpdateText("打开节点选项");
        }
        private void OpenGraphContextMenu(MouseButtons button)
        {
            SelectedNode = PrevSelectNode;
            NodeInfoTip.Hide(this);
            new GraphContextMenu(this, Cursor.Position, button);
        }

        //---- OnMouseDoubleClick ----//

        private void OnMouseDoubleClick(object sender, MouseEventArgs args)
        {
            if (CheckPrevSelect())
            {
                // node double clicked
            }
            else
            {
                if (args.Button == MouseButtons.Left)
                {
                    GraphLeftDoubleClicked();
                }
            }
        }
        private void GraphLeftDoubleClicked()
        {
            if (SelectedNode != null)
            {
                //var focus = SelectedNode.Value;
                SelectedNode = null;
            }
            if (GraphBox.ReadOnly && MessageBox.Show("[202303052340]是否恢复备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                FileBackup.Backup<FocusGraph>(GraphBox.FilePath);
                GraphBox.Save();
                RefreshGraphBox();
                Parent.UpdateText("恢复备份");
            }
        }

        //---- OnMouseMove ----//

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

            //Invalidate();
        }
        private void DragGraph(Point newPoint)
        {
            var diffInWidth = newPoint.X - DragMouseFlagPoint.X;
            var diffInHeight = newPoint.Y - DragMouseFlagPoint.Y;
            if (Math.Abs(diffInWidth) > MouseMoveSensibility || Math.Abs(diffInHeight) > MouseMoveSensibility)
            {
#if MOUSE_DRAG_FREE
                Lattice.OriginLeft += (newPoint.X - DragMouseFlagPoint.X) / MouseMoveSensibility;
                Lattice.OriginTop += (newPoint.Y - DragMouseFlagPoint.Y) / MouseMoveSensibility;
#else
                Lattice.OriginLeft += (newPoint.X - DragMouseFlagPoint.X) / MouseMoveSensibility * LatticeCell.Length;
                Lattice.OriginTop += (newPoint.Y - DragMouseFlagPoint.Y) / MouseMoveSensibility * LatticeCell.Length;
#endif
                DragMouseFlagPoint = newPoint;
                Refresh();
                DrawNodeMapInfo();
            }
        }
        /// <summary>
        /// 上次光标所处的节点部分
        /// </summary>
        LatticeCell.Parts LastCellPart = LatticeCell.Parts.Leave;
        LatticedPoint LatticedPointCursorOn;
        FocusData FocusNodeToDrag;
        Rectangle LastPartRealRect;
        bool FirstDrag = true;
        private void DragNode(Point newPoint)
        {
            if (GraphBox.ReadOnly) { return; }
            NodeInfoTip.Hide(this);
            LatticedPointCursorOn = new(newPoint);
            LatticeCell cell = new(LatticedPointCursorOn);
            var cellPart = cell.GetPartPointOn(newPoint);
            if (cellPart == LastCellPart) { return; }
            ImageDrawer.DrawImageOn(BackgroundCache, (Bitmap)Image, LastPartRealRect, true);
            ImageDrawer.DrawImageOn(LineMapCache, (Bitmap)Image, LastPartRealRect, true);
            LastCellPart = cellPart;
            if (GraphBox.PointInAnyFocusNode(newPoint, out var focus))
            {
                if (FirstDrag)
                {
                    FocusNodeToDrag = focus.Value;
                    FirstDrag = false;
                }
                GraphDrawer.DrawFocusNodeSelected((Bitmap)Image, focus.Value);
                LastPartRealRect = cell.NodeRealRect;
            }
            else
            {
                GraphDrawer.DrawSelectedCellPart((Bitmap)Image, LatticedPointCursorOn, cellPart);
                LastPartRealRect = cell.CellPartsRealRect[cellPart];
                ImageDrawer.DrawImageOn(LineMapCache, (Bitmap)Image, LastPartRealRect, true);
            }
            Parent.Text = $"CellSideLength {LatticeCell.Length}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {newPoint}, cellPart: {LastCellPart}";
            Invalidate();
        }

        private void ShowNodeInfoTip(Point location)
        {
            if (!GraphBox.PointInAnyFocusNode(location, out var focus))
            {
                NodeInfoTip.Hide(this);
                return;
            }
            NodeInfoTip.BackColor = Color.FromArgb(0, Color.AliceBlue);
            NodeInfoTip.Show($"{focus.Value.Name}\nID: {focus.Value.ID}", this, location.X + 10, location.Y);
        }

        //---- OnMouseUp ----//

        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (DragGraph_Flag)
            {
                DragGraph_Flag = false;
            }
            if (DragNode_Flag)
            {
                LineMapCache.Dispose();
                BackgroundCache.Dispose();
                FirstDrag = true;
                DragNode_Flag = false;
                LastCellPart = LatticeCell.Parts.Leave;
                DrawBackLattice = false;
                FocusNodeToDrag.LatticedPoint = LatticedPointCursorOn;
                if (GraphBox.ContainLatticedPoint(LatticedPointCursorOn))
                {
                    Refresh();
                }
                else
                {
                    GraphBox.SetFocus(FocusNodeToDrag);
                    RefreshGraphBox();
                }
            }
        }

        //---- OnMouseWheel ----//

        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            var drWidth = Lattice.DrawRect.Width;
            var drHeight = Lattice.DrawRect.Height;
            var diffInWidth = args.Location.X - Width / 2;
            var diffInHeight = args.Location.Y - Height / 2;
            Lattice.OriginLeft += diffInWidth / LatticeCell.Length * drWidth / 200;
            Lattice.OriginTop += diffInHeight / LatticeCell.Length * drHeight / 200;
            LatticeCell.Length += args.Delta / 100 * Math.Max(drWidth, drHeight) / 200;
            Refresh();
            Parent.UpdateText("打开节点选项");
        }

        //---- Public ----//

        /// <summary>
        /// 检查预选择节点
        /// </summary>
        private bool CheckPrevSelect()
        {
            var clickPos = PointToClient(Cursor.Position);
            if (!GraphBox.PointInAnyFocusNode(clickPos, out PrevSelectNode)) { return false; }
            return true;
        }
        private void CloseAllNodeToolDialogs()
        {
            ToolDialogs.ToList().ForEach(x => x.Value.Close());
        }

        #endregion

        #region ==== 镜头操作 ====

        /// <summary>
        /// 缩放居中至全景
        /// </summary>
        public void CameraLocatePanorama()
        {
            if (GraphBox.IsNull) { return; }
            var gRect = GraphBox.MetaRect;
            //
            // 自适应大小
            //
            if (Lattice.DrawRect.Width < (gRect.Width) * LatticeCell.LengthMin)
            {
                LatticeCell.Length = LatticeCell.LengthMin;
                Parent.Width = (gRect.Width + 1) * LatticeCell.LengthMin + Parent.Width - Lattice.DrawRect.Width;
            }
            if (Lattice.DrawRect.Height < (gRect.Height) * LatticeCell.LengthMin)
            {
                LatticeCell.Length = LatticeCell.LengthMin;
                Parent.Height = (gRect.Height + 1) * LatticeCell.LengthMin + Parent.Height - Lattice.DrawRect.Height;
            }
            Background.DrawNew(Image);
            Lattice.DrawRect = LatticeBound;
            //
            //
            //
            LatticeCell.Length = Math.Min(Lattice.DrawRect.Width / (gRect.Width + 1), Lattice.DrawRect.Height / (gRect.Height + 1));
            Lattice.OriginLeft = (Lattice.DrawRect.Left + Lattice.DrawRect.Width / 2) - (gRect.Left + gRect.Width * LatticeCell.Length / 2);
            Lattice.OriginTop = (Lattice.DrawRect.Top + Lattice.DrawRect.Height / 2) - (gRect.Top + gRect.Height * LatticeCell.Length / 2);
            Refresh();
        }
        /// <summary>
        /// 居中或可缩放至选中的国策节点
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="zoom">是否聚焦</param>
        public void CameraLocateSelectedNode(bool zoom)
        {
            if (SelectedNode == null || GraphBox.IsNull) { return; }
            if (zoom)
            {
                LatticeCell.Length = LatticeCell.LengthMax;
            }
            LatticeCell cell = new(SelectedNode.Value.LatticedPoint);
            var halfNodeWidth = LatticeCell.NodeWidth / 2;
            var halfNodeHeight = LatticeCell.NodeHeight / 2;
            Lattice.OriginLeft += (Lattice.DrawRect.Left + Lattice.DrawRect.Width / 2) - (cell.NodeRealLeft + halfNodeWidth);
            Lattice.OriginTop += (Lattice.DrawRect.Top + Lattice.DrawRect.Height / 2) - (cell.NodeRealTop + halfNodeHeight);
            Lattice.DrawRect = LatticeBound;
            Refresh();
            Cursor.Position = PointToScreen(new Point(
                cell.NodeRealLeft + halfNodeWidth,
                cell.NodeRealTop + halfNodeHeight
                ));
        }

        #endregion

        #region ==== 读写操作调用 ====

        /// <summary>
        /// 重置显示器
        /// </summary>
        public void ResetDisplay()
        {
            CloseAllNodeToolDialogs();
            SelectedNode = null;
            UploadNodeMap();
            CameraLocatePanorama();
        }
        /// <summary>
        /// 更改 GraphBox 后刷新显示
        /// </summary>
        public void RefreshGraphBox()
        {
            UploadNodeMap();
            Refresh();
        }
        /// <summary>
        /// 刷新显示（不重新上载绘图委托）
        /// </summary>
        public new void Refresh()
        {
            Background.Redraw(Image);
            if (DrawBackLattice) { Lattice.Draw(Image); }
            OnRefresh.Invoke((Bitmap)Image);
            Invalidate();
        }
        /// <summary>
        /// 弹出节点信息对话框
        /// </summary>
        public void ShowNodeInfo()
        {
            NodeInfo.Show();
        }

        #endregion
    }
}
