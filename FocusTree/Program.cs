// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using FocusTree;
using FocusTree.Focus;
using FocusTree.Tree;
using System.Drawing;
using System.IO;
using System.Net;
using System.Numerics;
using System.Xml.Serialization;

internal static class Program
{
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    static void Main()
    {
        //Test.FMapTest();
        Test.FormInterationTest();

        //Application.EnableVisualStyles();
        //Application.SetCompatibleTextRenderingDefault(false);
        //Application.Run(new MainForm());
    }
}

class Test
{
    public static void FMapTest()
    {
        var tree = new FTree("人类财阀联合.csv");
        var graph = new FGraph(tree);

        var allTreeNodes = tree.GetAllMapNodes();
        var allGraphNodes = graph.GetAllMapNodes();

        var treeLevelCount = tree.GetLevelNodeCount(2);
        var graphLevelCount = graph.GetLevelNodeCount(2);

        var treeLevelNodes = tree.GetLevelNodes(2);
        var graphLevelNodes = graph.GetLevelNodes(2);

        var treeSibs = tree.GetSiblingNodes(3);
        var graphSibs = graph.GetSiblingNodes(3);

        var treeWidth = tree.GetBranchWidth(81);
        var graphWidth = graph.GetBranchWidth(81);

        var treeRelation = tree.GetNodeRelations(81);
        var graphRelation = graph.GetNodeRelations(81);

        var treeLeaf = tree.GetLeafNodes(-1);
        var graph_roots = graph.GetLevelNodes(0);
        var graphLeaf = new HashSet<FMapNode>();
        foreach (var g_root in graph_roots) { var set = graph.GetLeafNodes(g_root.ID); graphLeaf.UnionWith(set); }

        Console.WriteLine();
    }
    /// <summary>
    /// 序列化测试
    /// </summary>
    public static void FGraphToXmlTest()
    {
        var tree = new FTree("人类财阀联合.csv");
        var graph = new FGraph(tree);
        var graphStruct = graph.GetStruct();
        XmlSerializer writer = new XmlSerializer(typeof(FGraphStruct));
        FileStream file = File.Create("人类财阀联合.Graph.xml");
        writer.Serialize(file, graphStruct);
        file.Close();
    }
    public static void BranchesTest()
    {
        var tree = new FTree("人类财阀联合.csv");
        var graph = new FGraph(tree);
        //var branch = graph.GetBranches(1);
        var branches = graph.GetBranches(graph.GetLevelNodes(0).Select(x => x.ID).ToArray(), true, true);

        // ---- 尝试对分支进行一些排序 ----

        // 按已排序分支的顺序定位 x 和 y 的坐标
        var visited = new HashSet<int>();
        var width = branches.Count;
        var height = branches.Max(x => x.Length);
        var map = new Dictionary<int, Point>();

        var scale = new PointF(200, 150);
        Func<Point, PointF, Point> ScalePoint = (p, s) =>
        {
            return new Point((int)(p.X * s.X), (int)(p.Y * s.Y));
        };

        for (int x = 0; x < branches.Count; x++)
        {
            var branch = branches[x];
            for (int y = 0; y < branch.Length; y++)
            {
                var id = branch[y];
                if (visited.Add(id))
                {
                    var p = ScalePoint(new Point(x, y), scale);
                    p.Offset(30, 30);
                    map[id] = p;
                }
            }
        }

        // 绘图进行测试
        var img = new Bitmap((int)(width * scale.X), (int)(height * scale.Y));
        var g = Graphics.FromImage(img);
        g.Clear(Color.White);
        var font = new Font("Consolas", 12);
        var brush = new SolidBrush(Color.Black);
        var bgb = new SolidBrush(Color.FromArgb(100, Color.Cyan));
        var wgb = new SolidBrush(Color.White);
        var pen = new Pen(Color.FromArgb(100, Color.Cyan), 3f);

        foreach (var loc_pair in map)
        {
            var id = loc_pair.Key;
            var point = loc_pair.Value;

            var links = graph.GetNodeRelations(id).Where(x => x.Type == NodeRelation.FRelations.Linked);
            foreach (var link in links)
            {
                foreach (var link_id in link.IDs)
                {
                    int spx = 80, spy = 32;
                    int epx = 80, epy = -5;
                    var startLoc = new Point(map[id].X + spx, map[id].Y + spy);
                    var endLoc = new Point(map[link_id].X + epx, map[link_id].Y + epy);

                    g.DrawLine(pen, startLoc, endLoc);
                }
            }
        }
        foreach (var loc_pair in map)
        {
            var id = loc_pair.Key;
            var point = loc_pair.Value;
            g.FillRectangle(wgb, point.X - 5, point.Y - 12, scale.X - 35, scale.Y - 105);
            g.FillRectangle(bgb, point.X - 5, point.Y - 12, scale.X - 35, scale.Y - 105);

            var node_str = graph.GetMapNodeById(id).FocusData.Name;
            node_str = node_str.Length > 8 ? node_str.Substring(0, 8) : node_str;
            g.DrawString(node_str.ToString().PadLeft(2), font, brush, point);
        }


        g.Flush();
        g.Dispose();
        img.Save("Test.png");
        img.Dispose();
    }
    public static void FormInterationTest()
    {
        Func<PointF, Vector2> PToVec = (point) => { return new Vector2(point.X, point.Y); };
        Func<Size, Vector2> SToVec = (size) => { return new Vector2(size.Width, size.Height); };
        Func<Vector2, Point> VToPoint = (vec) => { return new Point((int)vec.X, (int)vec.Y); };

        Func<Rectangle, Vector2, Size, float, Rectangle>
            GetNodeOnScreen
            = (node_rect, cam_center, control_size_s, scale_factor) =>
            {
                var node_loc = PToVec(node_rect.Location);
                var control_size = SToVec(control_size_s);
                var on_screen_loc = VToPoint((node_loc - cam_center) * scale_factor + control_size / 2);
                var on_screen_size = new Size((int)(node_rect.Width * scale_factor), (int)(node_rect.Height * scale_factor));
                return new Rectangle(on_screen_loc, on_screen_size);
            };
        Func<Point, Vector2, Size, float, Point>
            GetInputLocation
            = (screen_loc_p, cam_center, control_size_s, scale_factor) =>
            {
                var screen_loc = PToVec(screen_loc_p);
                var control_size = SToVec(control_size_s);
                return VToPoint((screen_loc - control_size / 2) / scale_factor + cam_center);
            };

        Func<Rectangle, Size, bool> IsRectInScreen = (point, size) =>
        {
            return point.Right >= 0 && point.Left <= size.Width && point.Bottom >= 0 && point.Top <= size.Height;
        };

        var form = new Form();
        form.ClientSize = new Size(1000, 1000);

        var picbox = new PictureBox();
        picbox.SizeMode = PictureBoxSizeMode.Zoom;
        picbox.Size = form.ClientSize;
        picbox.Image = new Bitmap(picbox.Size.Width, picbox.Size.Height);

        // 节点尺寸
        var node_size = new Size(55, 35);
        var node_padding = new Point(65, 45);

        // 测试数据
        var tree = new FTree("人类财阀联合.csv");
        var graph = new FGraph(tree);
        Func<Dictionary<int, Rectangle>> GetNodeMap = () =>
        {
            var map = new Dictionary<int, Rectangle>();
            var branches = graph.GetBranches(graph.GetLevelNodes(0).Select(x => x.ID).ToArray(), true, true);
            var visited = new HashSet<int>();
            var width = branches.Count;
            var height = branches.Max(x => x.Length);

            for (int x = 0; x < branches.Count; x++)
            {
                var branch = branches[x];
                for (int y = 0; y < branch.Length; y++)
                {
                    var id = branch[y];
                    if (visited.Add(id))
                    {
                        var rect = new Rectangle(
                    x * node_padding.X,
                    y * node_padding.Y,
                    node_size.Width,
                    node_size.Height
                );
                        map[id] = rect;
                    }
                }
            }

            return map;
        };
        var map = GetNodeMap();

        var cam = new Vector2(picbox.Size.Width / 2, picbox.Size.Height / 2);
        var scale = 1f;

        var background = new SolidBrush(Color.FromArgb(80, Color.Aqua));
        var nameBrush = new SolidBrush(Color.Black);
        var linkPen = new Pen(Color.FromArgb(100, Color.Cyan), 3f);

        picbox.Invalidated += (sender, args) =>
        {
            var g = Graphics.FromImage(picbox.Image);
            g.Clear(Color.White);

            var nameFont = new Font("黑体", 10 * scale, FontStyle.Bold, GraphicsUnit.Pixel);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            //var textflags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak;

            foreach (var node in graph.GetAllMapNodes())
            {
                var name = node.FocusData.Name;
                var rect = map[node.ID];

                var node_rect = GetNodeOnScreen(rect, cam, picbox.Size, scale);
                if (IsRectInScreen(node_rect, picbox.Size))
                {
                    g.FillRectangle(background, node_rect);
                    g.DrawString(name, nameFont, nameBrush, node_rect, stringFormat);
                    //TextRenderer.DrawText(g, name, nameFont, node_rect, Color.Black, textflags);  // 这个有点丑，执行效率还低
                }
            }

            foreach (var loc_pair in map)
            {
                var id = loc_pair.Key;
                var rect = GetNodeOnScreen(loc_pair.Value, cam, picbox.Size, scale);

                var links = graph.GetNodeRelations(id).Where(x => x.Type == NodeRelation.FRelations.Linked);
                foreach (var link in links)
                {
                    foreach (var link_id in link.IDs)
                    {
                        var torect = GetNodeOnScreen(map[link_id], cam, picbox.Size, scale);
                        var startLoc = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height); // x -> 中间, y -> 下方
                        var endLoc = new Point(torect.X + torect.Width / 2, torect.Y); // x -> 中间, y -> 上方

                        g.DrawLine(linkPen, startLoc, endLoc);
                    }
                }
            }
            g.Flush();
            g.Dispose();
            picbox.Update();
        };

        picbox.MouseDown += (sender, args) =>
        {
            var point = GetInputLocation(args.Location, cam, picbox.Size, scale);
            foreach (var node in map)
            {
                var rect = node.Value;
                if (rect.Contains(point))
                {
                    var fnode = graph.GetMapNodeById(node.Key);
                    MessageBox.Show($"{fnode.FocusData.Name}\n\n" +
                        $"{fnode.FocusData.Effects}\n\n" +
                        $"实施 {fnode.FocusData.Duration} 天\n\n" +
                        $"{fnode.FocusData.Descript}\n\n" +
                        $"{fnode.FocusData.Ps}");
                }
            }
        };
        form.KeyDown += (sender, args) =>
        {
            // MessageBox.Show(args.KeyCode.ToString());
            switch (args.KeyCode)
            {
                case Keys.Left: cam += new Vector2(-50 / scale, 0); picbox.Invalidate(); break;
                case Keys.Right: cam += new Vector2(+50 / scale, 0); picbox.Invalidate(); break;
                case Keys.Up: cam += new Vector2(0, -50 / scale); picbox.Invalidate(); break;
                case Keys.Down: cam += new Vector2(0, +50 / scale); picbox.Invalidate(); break;
                case Keys.Oemplus:
                case Keys.Add: scale = scale > 10 ? scale : scale * 2; picbox.Invalidate(); break;
                case Keys.OemMinus:
                case Keys.Subtract: scale = scale < 0.1 ? scale : scale / 2; picbox.Invalidate(); break;
            }
        };

        form.SizeChanged += (sender, args) =>
        {
            picbox.Size = form.ClientSize;
            picbox.Image = new Bitmap(picbox.Width, picbox.Height);
            picbox.Invalidate();
        };

        form.Controls.Add(picbox);
        picbox.VisibleChanged += (sender, args) =>
        {
            ((PictureBox)sender).Invalidate();
        };

        Application.EnableVisualStyles();

        Application.Run(form);
    }
}