// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using FocusTree.IO.FileManege;
using FocusTree.UI;
using FocusTree.UI.test;

internal static class Program
{
    public static TestInfo testInfo = new TestInfo();
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    static void Main()
    {
        testInfo.Show();
        Application.Run(new MainForm());
        Cache.Clear();
    }
}

//class Test
//{
//    public static void FMapTest()
//    {
//        var graph = new FocusGraph("人类财阀联合.csv");

//        // FHistory.Enqueue(graph);

//        //var suc = graph.AddNode(new FData(99,"Test", false, 0, "测试", "测试2", "测试3"));

//        graph.RemoveNode(1);

//        DataHistory.Undo(graph);



//        DataHistory.Redo(graph);

//        var graphRequire = graph.GetNodeRequires(81);
//        var graphLink = graph.GetNodeChildLinkes(81);

//        Console.WriteLine();
//    }
//    /// <summary>
//    /// 序列化测试
//    /// </summary>
//    public static void FGraphToXmlTest()
//    {
//        var serializer = new XmlSerializer(typeof(FocusGraph));

//        var graph = new FocusGraph("人类财阀联合.csv");

//        XmlIO.SaveGraph("人类财阀联合.Graph.xml", graph);

//        //var readgraph = XmlIO.LoadGraph("人类财阀联合.Graph.xml");


//    }
//}