// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using FocusTree;
using FocusTree.Focus;
using FocusTree.Tree;
using System.Drawing;
using System.IO;
using System.Net;
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
        Test.BranchesTest();

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
        foreach(var g_root in graph_roots) { var set = graph.GetLeafNodes(g_root.ID); graphLeaf.UnionWith(set); }

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
        var branches = graph.GetBranches(graph.GetLevelNodes(0).Select(x=>x.ID).ToArray(),true,true);

        // ---- 尝试对分支进行一些排序 ----

        // 按已排序分支的顺序定位 x 和 y 的坐标
        var visited = new HashSet<int>();
        var width = branches.Count;
        var height = branches.Max(x => x.Length);
        var map = new Dictionary<int, Point>();

        var scale = new PointF(200,150);
        Func<Point, PointF, Point> ScalePoint = (p, s) => {
            return new Point((int)(p.X * s.X), (int)(p.Y * s.Y));
        };

        for (int x=0; x<branches.Count; x++)
        {
            var branch = branches[x];
            for(int y=0; y < branch.Length; y++)
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
        var pen = new Pen(Color.FromArgb(100,Color.Cyan), 3f);
 
        foreach(var loc_pair in map)
        {
            var id = loc_pair.Key;
            var point = loc_pair.Value;

            var links = graph.GetNodeRelations(id).Where(x=>x.Type==NodeRelation.FRelations.Linked);
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
}