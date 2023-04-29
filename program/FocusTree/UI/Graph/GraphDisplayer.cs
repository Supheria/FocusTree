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
        static int MouseMoveSensibility = 20;
        /// <summary>
        /// 拖动事件使用的鼠标参照坐标
        /// </summary>
        Point DragMouseFlagPoint = new(0, 0);
        /// <summary>
        /// 格元放置边界
        /// </summary>
        public Rectangle LatticeBound { get => new(Left, Top, Width, InfoBrandRect.Top - Top - 30); }
        /// <summary>
        /// 节点绘制委托列表
        /// </summary>
        public static Dictionary<int, LayerDrawer> NodeDrawerCatalog { get; private set; } = new();

        #endregion


        //===== 方法 =====//

        #region ---- 初始化 ----

        public GraphDisplayer(GraphForm mainForm)
        {
            base.Parent = Parent = mainForm;
            NodeInfo = new NodeInfoDialog(this);
            //SizeMode = PictureBoxSizeMode.Zoom;
            //DoubleBuffered = true;

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
            Lattice.Drawing.Clear();
            foreach (var focus in GraphBox.FocusList)
            {
                int color = 0; //不同需求要变色
                foreach (var requires in focus.Requires)
                {
                    foreach (var requireId in requires)
                    {
                        var require = GraphBox.GetFocus(requireId);
                        Lattice.Drawing += new LayerDrawer(2, (image) => GraphDrawer.DrawRequireLine(image, focus.LatticedPoint, require.LatticedPoint));
                    }
                    color++;
                }
                Lattice.Drawing += NodeDrawerCatalog[focus.ID] = new(0, (image) => GraphDrawer.DrawFocusNode(image, focus, false));
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
            Invalidate();
        }
        private void GraphLeftClicked(Point startPoint)
        {
            DragGraph_Flag = true;
            DragMouseFlagPoint = startPoint;
            Invalidate();
            Parent.UpdateText("拖动图像");
        }
        private void NodeLeftClicked(FocusData focus)
        {
            DragNode_Flag = true;
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
            Invalidate();
        }
        private void GraphLeftDoubleClicked()
        {
            if (SelectedNode != null)
            {
                //var focus = SelectedNode.Value;
                Lattice.Drawing -= LastCellDrawer;
                SelectedNode = null;
            }
            if (GraphBox.ReadOnly && MessageBox.Show("[202303052340]是否恢复备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                FileBackup.Backup<FocusGraph>(GraphBox.FilePath);
                GraphBox.Save();
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

            Invalidate();
        }
        private void DragGraph(Point newPoint)
        {
            var diffInWidth = newPoint.X - DragMouseFlagPoint.X;
            var diffInHeight = newPoint.Y - DragMouseFlagPoint.Y;
            if (Math.Abs(diffInWidth) > MouseMoveSensibility || Math.Abs(diffInHeight) > MouseMoveSensibility)
            {
                Background.Redraw(Image);
                Lattice.OriginLeft += (newPoint.X - DragMouseFlagPoint.X) / MouseMoveSensibility * LatticeCell.Width;
                Lattice.OriginTop += (newPoint.Y - DragMouseFlagPoint.Y) / MouseMoveSensibility * LatticeCell.Height;
                DragMouseFlagPoint = newPoint;
                Lattice.Draw(Image); ;
                DrawNodeMapInfo();
            }
        }
        /// <summary>
        /// 上次光标所处的节点部分
        /// </summary>
        LatticeCell.Parts LastCellPart = LatticeCell.Parts.Leave;
        LatticedPoint LatticedPointCursorOn;
        /// <summary>
        /// 上次使用的绘制委托
        /// </summary>
        LayerDrawer LastCellDrawer;
        /// <summary>
        /// 1层级绘制委托备份暂存
        /// </summary>
        LayerDrawer NodeDrawerCache;
        FocusData FocusNodeToDrag;
        bool FirstDrag = true;
        private void DragNode(Point newPoint)
        {
            if (GraphBox.ReadOnly) { return; }
            NodeInfoTip.Hide(this);
            Lattice.DrawBackLattice = true;
            LatticedPointCursorOn = new(newPoint);
            LatticeCell cell = new(LatticedPointCursorOn);
            var cellPart = cell.GetPartPointOn(newPoint);
            if (cellPart == LastCellPart) { return; }
            LastCellPart = cellPart;
            Lattice.Drawing -= LastCellDrawer;
            if (GraphBox.PointInAnyFocusNode(newPoint, out var focus))
            {
                if (FirstDrag)
                {
                    FocusNodeToDrag = focus.Value;
                    FirstDrag = false;
                }
                Lattice.Drawing -= NodeDrawerCache = NodeDrawerCatalog[focus.Value.ID];
                Lattice.Drawing += LastCellDrawer = new(1, (image) => GraphDrawer.DrawFocusNode(image, focus.Value, true));
            }
            else
            {
                Lattice.Drawing += NodeDrawerCache;
                NodeDrawerCache = new();
                Lattice.Drawing += LastCellDrawer = new(1, (image) => GraphDrawer.DrawCellPart(image, LatticedPointCursorOn, cellPart));
            }
            Background.Redraw(Image);
            Lattice.Draw(Image);
            Parent.Text = $"W {LatticeCell.Width},H {LatticeCell.Height}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {newPoint}, cellPart: {LastCellPart}";
            //Parent.Text = $"cell left: {LatticedPointCursorOn.LatticedLeft}, cell top: {LatticedPointCursorOn.LatticedTop}, last part: {LastCellPart}, part: {part}";
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
            //if (Graph == null) { return; }
            // 用于拖动事件
            if (args.Button == MouseButtons.Left)
            {
                DragGraph_Flag = false;
                if (DragNode_Flag)
                {
                    FirstDrag = true;
                    DragNode_Flag = false;
                    LastCellPart = LatticeCell.Parts.Leave;
                    LastCellDrawer = new();
                    Lattice.DrawBackLattice = false;
                    FocusNodeToDrag.LatticedPoint = LatticedPointCursorOn;
                    if (!GraphBox.ContainLatticedPoint(LatticedPointCursorOn))
                    {
                        GraphBox.SetFocus(FocusNodeToDrag);
                    }
                    Background.Redraw(Image);
                    UploadNodeMap();
                    Lattice.Draw(Image); ;
                }
            }
        }

        //---- OnMouseWheel ----//

        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            Background.Redraw(Image);
            //if (Graph == null) { return; }
            var diffInWidth = args.Location.X - Width / 2;
            var diffInHeight = args.Location.Y - Height / 2;
            Lattice.OriginLeft += diffInWidth / LatticeCell.Width * Lattice.DrawRect.Width / 200;
            Lattice.OriginTop += diffInHeight / LatticeCell.Height * Lattice.DrawRect.Height / 200;

            LatticeCell.Width += args.Delta / 100 * Lattice.DrawRect.Width / 200;
            LatticeCell.Height += args.Delta / 100 * Lattice.DrawRect.Width / 200;

            Lattice.SetBounds(LatticeBound);

            Lattice.Draw(Image); ;
            //DrawNodeMapInfo();
            Invalidate();
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
            Background.Redraw(Image);
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
            Lattice.Draw(Image); ;
        }
        /// <summary>
        /// 居中或可缩放至选中的国策节点
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="zoom">是否聚焦</param>
        public void CameraLocateSelectedNode(bool zoom)
        {
            if (SelectedNode == null || GraphBox.IsNull) { return; }
            Background.Redraw(Image);
            var drRect = Lattice.DrawRect;
            if (zoom)
            {
                LatticeCell.Width = LatticeCell.SizeMax.Width;
                LatticeCell.Height = LatticeCell.SizeMax.Height;
            }
            LatticeCell cell = new(SelectedNode.Value.LatticedPoint);
            var halfNodeWidth = LatticeCell.NodeWidth / 2;
            var halfNodeHeight = LatticeCell.NodeHeight / 2;
            int NodeCenterX = cell.NodeRealLeft + halfNodeWidth;
            int NodeCenterY = cell.NodeRealTop + halfNodeHeight;
            int WidthCenterDiff = (drRect.Left + drRect.Width / 2) - NodeCenterX;
            int HeightCenterDiff = (drRect.Top + drRect.Height / 2) - NodeCenterY;
            Lattice.OriginLeft += WidthCenterDiff;
            Lattice.OriginTop += HeightCenterDiff;
            Lattice.SetBounds(LatticeBound);
            Lattice.Draw(Image);
            Cursor.Position = PointToScreen(new Point(
                cell.NodeRealLeft + halfNodeWidth,
                cell.NodeRealTop + halfNodeHeight
                ));
        }

        #endregion

        #region ==== 读写操作调用 ====

        /// <summary>
        /// 重置图像显示
        /// </summary>
        public void ResetDisplay()
        {
            CloseAllNodeToolDialogs();
            SelectedNode = null;
            UploadNodeMap();
            CameraLocatePanorama();
            Invalidate();
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
        /// 更新编辑显示
        /// </summary>
        public new void Refresh()
        {
            UploadNodeMap();
            Background.Redraw(Image);
            Lattice.Draw(Image);
            Invalidate();
        }

        #endregion
    }
}
