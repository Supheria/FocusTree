#define DEBUG
#define REBUILD
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
        Point DragGraphMouseFlagPoint = new(0, 0);
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
            DoubleBuffered = true;
            //DrawLattice();

            SizeChanged += OnSizeChanged;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;
            Invalidated += UpdateGraph;
            ControlResize.SetTag(this);
        }
        /// <summary>
        /// 格元
        /// </summary>
        struct LatticeCell
        {
            public static Point LeftTop
            {
                set
                {
                    Left = value.X + Lattice.CellLeftOffset;
                    Top = value.Y + Lattice.CellTopOffset;
                }
            }
            /// <summary>
            /// 格元元左边界（绘图应该调用 ToDrawLeft）
            /// </summary>
            public static int Left
            {
                set
                {
                    var mainRect = Lattice.ToDrawRect;
                    var left = mainRect.Left + value;
                    if (left < mainRect.Left) { left += mainRect.Width; }
                    else if (left > mainRect.Right) {  left -= mainRect.Width; }
                    var realWidth = mainRect.Right - left;
                    var cellWidth = Lattice.CellWidth;
                    if (realWidth >= cellWidth)
                    {
                        ToDrawLeft = new (int, int)[] { (left, left + cellWidth) };
                    }
                    else
                    {
                        ToDrawLeft = new (int, int)[]
                        {
                            (left, left + realWidth),
                            (mainRect.Left, mainRect.Left + cellWidth - realWidth)
                        };
                    }
                    //
                    // node left
                    //
                    left = mainRect.Left + value + Lattice.NodePaddingWidth;
                    if (left < mainRect.Left) { left += mainRect.Width; }
                    else if (left > mainRect.Right) { left -= mainRect.Width; }
                    realWidth = mainRect.Right - left;
                    var nodeWidth = Lattice.CellWidth - Lattice.NodePaddingWidth;
                    if (realWidth >= nodeWidth)
                    {
                        NodeToDrawLeft = new (int, int)[] { (left, left + nodeWidth) };
                    }
                    else
                    {
                        NodeToDrawLeft = new (int, int)[]
                        {
                            (left, left + realWidth),
                            (mainRect.Left, mainRect.Left + nodeWidth - realWidth)
                        };
                    }
                }
            }
            /// <summary>
            /// 栅格区域内格元绘图左边界
            /// </summary>
            public static (int, int)[] ToDrawLeft { get; private set; }
            /// <summary>
            /// 节点栅格区域内绘图上边界(起点x, 终点x)（如果分裂则返回右、左顺序的两个数对）
            /// </summary>
            public static (int, int)[] NodeToDrawLeft { get; private set; }
            /// <summary>
            /// 格元元左上边界（绘图应该调用 ToDrawTop）
            /// </summary>
            public static int Top
            {
                set
                {
                    var mainRect = Lattice.ToDrawRect;
                    var top = mainRect.Top + value;
                    if (top < mainRect.Top) { top += mainRect.Height; }
                    else if (top > mainRect.Bottom) { top -= mainRect.Height; }
                    var realHeight = mainRect.Bottom - top;
                    var cellHeight = Lattice.CellHeight;
                    if (realHeight >= cellHeight)
                    {
                        ToDrawTop = new (int, int)[] { (top, top + cellHeight) };
                    }
                    else
                    {
                        ToDrawTop = new (int, int)[]
                        {
                            (top, top + realHeight),
                            (mainRect.Top, mainRect.Top + cellHeight - realHeight)
                        };
                    }
                    //
                    // node top
                    //
                    top = mainRect.Top + value + Lattice.NodePaddingHeight;
                    if (top < mainRect.Top) { top += mainRect.Height; }
                    else if (top > mainRect.Bottom) { top -= mainRect.Height; }
                    realHeight = mainRect.Bottom - top;
                    var nodeHeight = Lattice.CellHeight - Lattice.NodePaddingHeight;
                    if (realHeight >= nodeHeight)
                    {
                        NodeToDrawTop = new (int, int)[] { (top, top + nodeHeight) };
                    }
                    else
                    {
                        NodeToDrawTop = new (int, int)[]
                        {
                            (top, top + realHeight),
                            (mainRect.Top, mainRect.Top + nodeHeight - realHeight)
                        };
                    }
                    
                }
            }
            /// <summary>
            /// 栅格区域内格元绘图上边界
            /// </summary>
            public static (int, int)[] ToDrawTop { get; private set; }
            /// <summary>
            /// 节点栅格区域内绘图上边界(起点y, 终点y)（如果分裂则返回下、上顺序的两个数对）
            /// </summary>
            public static (int, int)[] NodeToDrawTop { get; private set; }
        }
        /// <summary>
        /// 栅格
        /// </summary>
        struct Lattice
        {
            /// <summary>
            /// 栅格行数
            /// </summary>
            public static int RowNumber { get; set; } = 0;
            /// <summary>
            /// 栅格列数
            /// </summary>
            public static int ColNumber { get; set; } = 0;
            /// <summary>
            /// 栅格放置区域（绘图区域应该调用 ToDrawRect）
            /// </summary>
            public static Rectangle Bounds 
            {
                set
                {
                    CellWidth = value.Width / ColNumber;
                    RowWidth = ColNumber * CellWidth;
                    NodePaddingWidth = (int)(CellWidth * 0.3f);
                    CellHeight = value.Height / RowNumber; // 如果 ColRowNumber 未赋值则会触发除以零的异常
                    ColHeight = RowNumber * CellHeight;
                    NodePaddingHeight = (int)(CellHeight * 0.3f);
                    var deviOfDiffInWidth = (int)((float)(value.Width - RowWidth) * 0.5f);
                    var deviOfDiffInHeight = (int)((float)(value.Height - ColHeight) * 0.5f);
                    ToDrawRect = new Rectangle(
                        value.X + deviOfDiffInWidth,
                        value.Y + deviOfDiffInHeight,
                        RowWidth,
                        ColHeight
                        );
                }
            }
            /// <summary>
            /// 格元宽长（根据给定放置区域和给定列数自动生成）
            /// </summary>
            public static int CellWidth { get; private set; }
            /// <summary>
            /// 格元高长（根据给定放置区域和给定行数自动生成）
            /// </summary>
            public static int CellHeight { get; private set; }
            /// <summary>
            /// 节点 Left 到格元 Left 的空隙
            /// </summary>
            public static int NodePaddingWidth { get; private set; }
            /// <summary>
            /// 节点 Top 到格元 Top 的空隙
            /// </summary>
            public static int NodePaddingHeight { get; private set; }
            /// <summary>
            /// 栅格绘图区域（根据给定放置区域、列数、行数自动生成，并在给定放置区域内居中）
            /// </summary>
            public static Rectangle ToDrawRect { get; private set; }
            /// <summary>
            /// 栅格总列宽
            /// </summary>
            public static int RowWidth { get; private set; }
            /// <summary>
            /// 栅格总行高
            /// </summary>
            public static int ColHeight { get; private set; }
            /// <summary>
            /// 栅格坐标系原点
            /// </summary>
            public static Point OriginLeftTop
            {
                get => new(originLeft, originTop);
                set
                {
                    OriginLeft = value.X;
                    OriginTop = value.Y;
                }
            }
            public static int OriginLeft
            {
                get => originLeft;
                set
                {
                    originLeft = value;
                    CellLeftOffset = (value - ToDrawRect.X) % CellWidth;
                }
            }
            /// <summary>
            /// 格元横坐标偏移量，对栅格坐标系原点相对于 ToDrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
            /// </summary>
            public static int CellLeftOffset { get; private set; }
            static int originLeft;
            public static int OriginTop
            {
                get => originTop;
                set
                {
                    originTop = value;
                    CellTopOffset = (value - ToDrawRect.Y) % CellHeight;
                }
            }
            static int originTop;
            /// <summary>
            /// 格元纵坐标偏移量，对栅格坐标系原点相对于 ToDrawRect 的左上角的偏移量，在格元大小内实施相似偏移量
            /// </summary>
            public static int CellTopOffset { get; private set; }
        }
        private void DrawLattice(object sender, InvalidateEventArgs e)
        {
            Image ??= new Bitmap(Size.Width, Size.Height);
            Graphics g = Graphics.FromImage(Image);
            g.Clear(Color.White);

            Lattice.ColNumber = 15;
            Lattice.RowNumber = 10;
            Lattice.Bounds = ClientRectangle;

            for (int i = 0; i < Lattice.ColNumber; i++)
            {
                for (int j = 0; j < Lattice.RowNumber; j++)
                {
                    LatticeCell.LeftTop = new(Lattice.CellWidth * i, Lattice.CellHeight * j);
                    var toDrawLeft = LatticeCell.ToDrawLeft;
                    var toDrawTop = LatticeCell.ToDrawTop;
                    //
                    // cell left
                    //
                    g.DrawLine(new Pen(Color.Red, 2),
                            new Point(toDrawLeft[0].Item1, toDrawTop[0].Item1),
                            new Point(toDrawLeft[0].Item1, toDrawTop[0].Item2)
                            );
                    if (toDrawTop.Length > 1)
                    {
                        g.DrawLine(new Pen(Color.Red, 2),
                            new Point(toDrawLeft[0].Item1, toDrawTop[1].Item1),
                            new Point(toDrawLeft[0].Item1, toDrawTop[1].Item2)
                            );
                    }
                    //
                    // cell top
                    //
                    g.DrawLine(new Pen(Color.Red, 2),
                        new Point(toDrawLeft[0].Item1, toDrawTop[0].Item1),
                        new Point(toDrawLeft[0].Item2, toDrawTop[0].Item1)
                        );
                    if (toDrawLeft.Length > 1)
                    {
                        g.DrawLine(new Pen(Color.Red, 2),
                            new Point(toDrawLeft[1].Item1, toDrawTop[0].Item1),
                            new Point(toDrawLeft[1].Item2, toDrawTop[0].Item1)
                            );
                    }
                    var nodeToDrawLeft = LatticeCell.NodeToDrawLeft;
                    var nodeToDrawTop = LatticeCell.NodeToDrawTop;
                    //
                    // node left
                    //
                    g.DrawLine(new Pen(Color.Orange, 1),
                            new Point(nodeToDrawLeft[0].Item1, nodeToDrawTop[0].Item1),
                            new Point(nodeToDrawLeft[0].Item1, nodeToDrawTop[0].Item2)
                            );
                    if (nodeToDrawTop.Length > 1)
                    {
                        g.DrawLine(new Pen(Color.Orange, 1),
                            new Point(nodeToDrawLeft[0].Item1, nodeToDrawTop[1].Item1),
                            new Point(nodeToDrawLeft[0].Item1, nodeToDrawTop[1].Item2)
                            );
                    }
                    //
                    // node top
                    //
                    g.DrawLine(new Pen(Color.BlueViolet, 1),
                        new Point(nodeToDrawLeft[0].Item1, nodeToDrawTop[0].Item1),
                        new Point(nodeToDrawLeft[0].Item2, nodeToDrawTop[0].Item1)
                        );
                    if (nodeToDrawLeft.Length > 1)
                    {
                        g.DrawLine(new Pen(Color.BlueViolet, 1),
                            new Point(nodeToDrawLeft[1].Item1, nodeToDrawTop[0].Item1),
                            new Point(nodeToDrawLeft[1].Item2, nodeToDrawTop[0].Item1)
                            );
                    }
                }
            }

            // test
            g.DrawEllipse(new Pen(Color.Red), Lattice.OriginLeftTop.X - 10, Lattice.OriginLeftTop.Y - 10, 10, 10);
            Parent.Text = $"col {Lattice.ColNumber},row {Lattice.RowNumber}, {Lattice.CellLeftOffset},{Lattice.CellTopOffset} {Lattice.OriginLeftTop}";

            g.Flush(); g.Dispose();
        }


        private void UpdateGraph(object sender, InvalidateEventArgs e)
        {
#if REBUILD
            DrawLattice(sender, e);
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
            if (Image != null)
            {
                Image.Dispose();
            }
            var ratioVec = ControlResize.GetRatio(this);
            var ratio = MathF.Min(ratioVec.X, ratioVec.Y);
            ControlResize.SetTag(this);
            Image = new Bitmap(Size.Width, Size.Height);
            GScale *= ratio;
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
            DragGraphMouseFlagPoint = startPoint;
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

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
#if REBUILD
            if (args.Button == MouseButtons.Left && DragGraph_Flag)
            {
                var newPoint = args.Location;
                var diff = new Point(newPoint.X - DragGraphMouseFlagPoint.X, newPoint.Y - DragGraphMouseFlagPoint.Y);
                if (Math.Abs(diff.X) >= 1 || Math.Abs(diff.Y) >= 1)
                {
                    //var difvec = new Vector2(dif.X / GScale, dif.Y / GScale);

                    Lattice.OriginLeft += diff.X;
                    Lattice.OriginTop += diff.Y;
                    DragGraphMouseFlagPoint = newPoint;
                    Invalidate();
                }
            }

#else
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
#endif
        }
        private void DragGraph(Point newPoint)
        {
            var dif = new Point(newPoint.X - DragGraphMouseFlagPoint.X, newPoint.Y - DragGraphMouseFlagPoint.Y);
            if (Math.Abs(dif.X) >= 1 || Math.Abs(dif.Y) >= 1)
            {
                var difvec = new Vector2(dif.X / GScale, dif.Y / GScale);

                DrawingCenter -= difvec;
                DragGraphMouseFlagPoint = newPoint;
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
