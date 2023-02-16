// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using FocusTree;
using FocusTree.Focus;
using FocusTree.Tree;

internal static class Program
{
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    static void Main()
    {
        Test.FMapTest();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
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
}