//#define DEBUG
#define REBUILD
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

namespace FocusTree.UI
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

        #region ---- 节点绘制工具 ----

        /// <summary>
        /// 元坐标转画布坐标时的单位坐标伸长倍数
        /// </summary>
        Point ScalingUnit { get { return new(NodeSize.Width + 10, NodeSize.Height + 30); } }
        /// <summary>
        ///  节点尺寸
        /// </summary>
        Size NodeSize = new(55, 35);
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
        readonly SolidBrush NodeBG_Normal = new(Color.FromArgb(80, Color.Aqua));
        /// <summary>
        /// 冲突节点的背景颜色
        /// </summary>
        readonly SolidBrush NodeBG_Conflicted = new(Color.FromArgb(80, Color.Aqua));
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

        public Graphics gCore
        {
            get
            {
                gcore?.Flush(); gcore?.Dispose();
                Image ??= new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                gcore = Graphics.FromImage(Image);
                return gcore;
            }
        }
        Graphics gcore;
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

            LatticeCell.Width = 30;
            LatticeCell.Height = 30;
            LatticeCell.NodePaddingZoomFactor = new(0.3f, 0.5f);

            SizeChanged += OnSizeChanged;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;
            //Invalidated += UpdateGraph;
            ControlResize.SetTag(this);
        }

#if REBUILD
#else
            if (Graph == null)
            {
                return;
            }
            Invalidated -= UpdateGraph;

            UploadNodeMap();
            _draw_info(info,
                new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );

            Invalidate();
            Invalidated += UpdateGraph;
#endif

        #endregion

        #region ==== 绘图 ====

        /// <summary>
        /// 将节点绘制上载到栅格绘图委托（要更新栅格放置区域，应该先更新再调用此方法，因为使用了裁剪超出绘图区域的绘图方法）
        /// </summary>
        private void UploadNodeMap()
        {
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
                        UploadRequireLine(NodeRequire[color], focus.LatticedPoint, require.LatticedPoint);
                    }
                    color++;
                }
                LatticeCell cell = new(focus);
                var rect = cell.InnerPartRealRects[LatticeCell.Parts.Node];
                var brush = NodeBG_Normal;
                if (id == SelectedNode)
                {
                    brush = NodeBG_Selected;
                }
                Lattice.DrawFillWhileDrawing(rect, brush);
            }
        }
        /// <summary>
        /// 将节点关系线绘制到栅格绘图委托
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="startLoc"></param>
        /// <param name="endLoc"></param>
        private static void UploadRequireLine(Pen pen, Point startLoc, Point endLoc)
        {
            var widthDiff = endLoc.X - startLoc.X;
            var heightDiff = startLoc.Y - endLoc.Y;
            LatticeCell cell = new(startLoc.X, startLoc.Y);

            var paddingHeight = LatticeCell.NodePaddingHeight;
            var nodeWidth = LatticeCell.NodeWidth;
            //
            // 竖线1
            //
            var y1 = cell.RealTop + paddingHeight;
            int halfHeight = heightDiff / 2;
            cell.LatticedTop -= halfHeight;
            var y2 = cell.RealTop + paddingHeight / 2;
            var x = cell.NodeRealLeft + nodeWidth / 2;
            Lattice.DrawLineWhileDrawing(x, (y1, y2), pen);
            //
            // 横线
            //
            if (Math.Abs(widthDiff) > 0)
            {
                cell.LatticedLeft += widthDiff;
                var x2 = cell.NodeRealLeft + nodeWidth / 2;
                Lattice.DrawLineWhileDrawing((x, x2), y2, pen);
            }
            //
            // 竖线2
            //
            y1 = y2;
            cell.LatticedTop -= heightDiff - halfHeight - 1;
            y2 = cell.RealTop;
            x = cell.NodeRealLeft + nodeWidth / 2;
            Lattice.DrawLineWhileDrawing(x, (y1, y2), pen);
        }
        public void DrawNodeMapInfo()
        {
            DrawInfo($"节点数量：{Graph.NodesCount}，分支数量：{Graph.BranchesCount}",
                new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
        }
        public void DrawInfo(string info)
        {
            if (Graph == null) { return; }
            DrawInfo(info,
                new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
        }
        private void DrawInfo(string info, Font infoFont, Brush BackBrush, Brush FrontBrush)
        {
            Rectangle infoRect = new(Bounds.Left, Bounds.Bottom - 100, Bounds.Width, 66);
            gCore.FillRectangle(BackBrush, infoRect);
            gCore.DrawString(
                info,
                infoFont,
                FrontBrush,
                infoRect,
                NodeFontFormat);
        }

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
            Image = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Lattice.SetBounds(ClientRectangle);
            UploadNodeMap();
            Lattice.Draw(gCore);
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
            var data = Graph.GetFocus(id);
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

        /// <summary>
        /// 光标所处的节点
        /// </summary>
        LatticeCell CellCursorOn = new();
        /// <summary>
        /// 上次光标所处的节点部分
        /// </summary>
        LatticeCell.Parts LastCellPart = new();

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
            }

            Invalidate();
            //Parent.Text = $"W {LatticeCell.Width},H {LatticeCell.Height}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {args.Location}, cellPart: {cellPart}, lastCell{new Point(LastCell.LatticedLeft, LastCell.LatticedTop)}";



        }
        private void DragGraph(Point newPoint)
        {
            //var cellPart = LastCell.HighlightSelection(newPoint);
            //if (cellPart == LatticeCell.InnerParts.Leave)
            //{
            //    LatticeCell cell = new(newPoint);
            //    LastCell = cell;
            //}
            //Parent.Text = $"W {LatticeCell.Width},H {LatticeCell.Height}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {newPoint}, cellPart: {cellPart}, lastCell{new Point(LastCell.LatticedLeft, LastCell.LatticedTop)}";

            //UploadNodeMap();

            var diffInWidth = newPoint.X - DragLatticeMouseFlagPoint.X;
            var diffInHeight = newPoint.Y - DragLatticeMouseFlagPoint.Y;
            if (Math.Abs(diffInWidth) >= 1 || Math.Abs(diffInHeight) >= 1)
            {
                Lattice.OriginLeft += newPoint.X - DragLatticeMouseFlagPoint.X;
                Lattice.OriginTop += newPoint.Y - DragLatticeMouseFlagPoint.Y;
                DragLatticeMouseFlagPoint = newPoint;
                UploadNodeMap();
                Lattice.Draw(gCore);
                DrawNodeMapInfo();
            }
        }
        private void DragNode(Point newPoint)
        {
#if REBUILD
            var cursor = newPoint;

            var part = CellCursorOn.GetInnerPartPointOn(cursor);
            if (part == LatticeCell.Parts.Leave)
            {
                Lattice.ReDrawCell(gCore, CellCursorOn);
                LastCellPart = part;
                CellCursorOn = new(cursor);
                return;
            }
            if (part == LastCellPart) { return; }
            LastCellPart = part;
            Lattice.ReDrawCell(gCore, CellCursorOn);
            var rect = CellCursorOn.InnerPartRealRects[part];
            if (part == LatticeCell.Parts.Node)
            {
                gCore.FillRectangle(new SolidBrush(Color.FromArgb(150, Color.Orange)), rect);
            }
            else
            {
                gCore.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Gray)), rect);
            }
            //Parent.Text = $"W {LatticeCell.Width},H {LatticeCell.Height}, o: {Lattice.OriginLeft}, {Lattice.OriginTop}, cursor: {cursor}, cellPart: {cellPart}, lastCell{new Point(LastCell.LatticedLeft, LastCell.LatticedTop)}";
            Parent.Text = $"cell left: {CellCursorOn.LatticedLeft}, cell top: {CellCursorOn.LatticedTop}, last part: {LastCellPart}, part: {part}";
#else
            NodeInfoTip.Hide(this);
            var dif = new Point(newPoint.X - DragNodeMouseFlagPoint.X, newPoint.Y - DragNodeMouseFlagPoint.Y);
            //if ((int)Math.Abs(dif.X) % (int)(ScalingUnit.X * GScale) == 0 || (int)Math.Abs(dif.Y) % (int)(ScalingUnit.Y * GScale) == 0) 
            {
                Invalidated -= UpdateGraph; 
                UploadNodeMap();
                string info = "拖动";
                _draw_info(info,
                    new Font(NodeFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                    new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                    new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                    );
                Graphics g = Graphics.FromImage(Image);
                Rectangle pointerRect = new(
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

            Lattice.SetBounds(ClientRectangle);
            UploadNodeMap();
            Lattice.Draw(gCore);
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
        /// 画布坐标转绘制坐标
        /// </summary>
        /// <param name="rect">矩形真实坐标</param>
        /// <param name="cam">相机位置</param>
        /// <returns>矩形显示坐标</returns>
        private Rectangle CanvasRectToDrawingRect(Rectangle rect)
        {
            //return new Rectangle(
            //    (rect.X - DrawingCenter.X) * GScale + Width / 2,
            //    (rect.Y - DrawingCenter.Y) * GScale + Height / 2,
            //    rect.Width * GScale,
            //    rect.Height * GScale
            //    );
            return new();
        }
        /// <summary>
        /// 元坐标转换为绘图坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point MetaPointToCanvasPoint(Point point)
        {
            return new Point(
                point.X * ScalingUnit.X,
                point.Y * ScalingUnit.Y
                );
        }
        /// <summary>
        /// 坐标是否处于任何节点的绘图区域中
        /// </summary>
        /// <param name="location">指定坐标 </param>
        /// <returns>坐标所处于的节点id，若没有返回null</returns>
        private int? PointInAnyNode(Point point)
        {
            LatticeCell cell = new(point);
            if (!Graph.ContainLatticedPoint(cell.LatticedPoint, out var id)) { return null; }
            var part = cell.GetInnerPartPointOn(point);
            if (part != LatticeCell.Parts.Node) { return null; }
            return id;
        }
        /// <summary>
        /// 节点的绘图区域
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Rectangle NodeDrawingRect(int id)
        {
            var point = MetaPointToCanvasPoint(Graph.GetFocus(id).LatticedPoint);
            var rect = new Rectangle(new(point.X, point.Y), NodeSize);
            return CanvasRectToDrawingRect(rect);
        }
        /// <summary>
        /// 缩放居中至全景
        /// </summary>
        private void RescaleToPanorama()
        {
            var gRect = Graph.GetGraphRect();
            if (Lattice.DrawRect.Width < (gRect.Width + 1) * LatticeCell.SizeMin.Width)
            {
                LatticeCell.Width = LatticeCell.SizeMin.Width;
                Parent.Width = (gRect.Width + 2) * LatticeCell.SizeMin.Width + Parent.Width - ClientRectangle.Width;
            }
            if (Lattice.DrawRect.Height < (gRect.Height + 1) * LatticeCell.SizeMin.Height)
            {
                LatticeCell.Height = LatticeCell.SizeMin.Height;
                Parent.Height = (gRect.Height + 2) * LatticeCell.SizeMin.Height + Parent.Height - ClientRectangle.Height;
            }
            Lattice.SetBounds(ClientRectangle);
            LatticeCell.Width = Lattice.DrawRect.Width / (gRect.Width + 1);
            LatticeCell.Height = Lattice.DrawRect.Height / (gRect.Height + 1);
            int GraphCenterX = gRect.Left + gRect.Width * LatticeCell.Width / 2;
            int GraphCenterY = gRect.Top + gRect.Height * LatticeCell.Height / 2;
            int WidthCenterDiff = (Lattice.DrawRect.Left + Lattice.DrawRect.Width / 2) - GraphCenterX;
            int HeightCenterDiff = (Lattice.DrawRect.Top + Lattice.DrawRect.Height / 2) - GraphCenterY;
            Lattice.OriginLeft = WidthCenterDiff - LatticeCell.NodePaddingWidth - LatticeCell.NodeWidth / 2;
            Lattice.OriginTop = HeightCenterDiff - LatticeCell.NodePaddingHeight - LatticeCell.NodeHeight / 2;
            Lattice.SetBounds(ClientRectangle);
            UploadNodeMap();
            Lattice.Draw(gCore);
        }
        /// <summary>
        /// 缩放居中至节点
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="zoom">是否聚焦</param>
        private void RescaleToNode(int id, bool zoom)
        {
            var drRect = Lattice.DrawRect;
            LatticeCell.Width = LatticeCell.SizeMax.Width;
            LatticeCell.Height = LatticeCell.SizeMax.Height;
            LatticeCell cell = new(Graph.GetFocus(id));
            int NodeCenterX = cell.NodeRealLeft + LatticeCell.NodeWidth / 2;
            int NodeCenterY = cell.NodeRealTop + LatticeCell.NodeHeight / 2;
            int WidthCenterDiff = (drRect.Left + drRect.Width / 2) - NodeCenterX;
            int HeightCenterDiff = (drRect.Top + drRect.Height / 2) - NodeCenterY;
            Lattice.OriginLeft += WidthCenterDiff;
            Lattice.OriginTop += HeightCenterDiff;
            Lattice.SetBounds(ClientRectangle);
            UploadNodeMap();
            Lattice.Draw(gCore);
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
            //UnloadNodeMap();
            CloseAllNodeToolDialogs();
            ReadOnly = Graph == null ? false : Graph.IsBackupFile(path);
            if (!ReadOnly) { FilePath = path; }
            Graph?.ClearCache();
            Graph = XmlIO.LoadFromXml<FocusGraph>(path);
            FileBackup.Backup<FocusGraph>(FilePath);
            Graph.NewHistory();
            SelectedNode = null;
            RescaleToPanorama();
            DrawNodeMapInfo();
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
        public FocusData? GetSelectedNodeData()
        {
            if (SelectedNode == null) { return null; }
            return Graph.GetFocus(SelectedNode.Value);
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
            Graph.ResetNodeMetaPoints();
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
