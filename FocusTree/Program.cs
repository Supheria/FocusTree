// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using FocusTree.Data;
using FocusTree.IO;
using FocusTree.Tool.Data;
using FocusTree.Tool.IO;
using FocusTree.Tool.UI;
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
        //string test = "";
        //this<string>.Enqueue(test);
        //var b = this<(string, string)>.Length;
        //Test.FMapTest();
        //Test.FGraphToXmlTest();
        //var graph = XmlIO.LoadGraph(
        //    "C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\国策\\" +
        //    "隐居村落.xml");
        //GraphTool.DrawNodeMapWithInfo(graph);
        //Application.Run(testInfo);

        Application.Run(new MainForm());

        //Application.EnableVisualStyles();
        //Application.SetCompatibleTextRenderingDefault(false);
        //Application.Run(new MainForm());
        //var c = this<string>.Length;
        //var e = this<(string, string)>.Length;
        //var d = this<FocusGraph>.Length;

        //FocusGraph a = XmlIO.LoadGraph("D:\\Non_E\\documents\\GitHub\\FocusTree\\FocusTree\\国策\\隐居村落.xml");
        //FocusGraph b = XmlIO.LoadGraph("D:\\Non_E\\documents\\GitHub\\FocusTree\\FocusTree\\国策\\神佑村落.xml");
        //a.Enqueue();
        //b.Enqueue();
        //int al =  a.GetHistoryLength();
        //int b1 =  b.GetHistoryLength();
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

//        var graphRequire = graph.GetNodeRequireGroups(81);
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