// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using FocusTree.Data;
using FocusTree.IO;
using FocusTree.UI;
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
        //Test.FGraphToXmlTest();
        Application.Run(new MainForm());

        //Application.EnableVisualStyles();
        //Application.SetCompatibleTextRenderingDefault(false);
        //Application.Run(new MainForm());


    }
}

class Test
{
    public static void FMapTest()
    {
        var graph = new FocusGraph("人类财阀联合.csv");

        // FHistory.Enqueue(graph);

        //var suc = graph.AddNode(new FData(99,"Test", false, 0, "测试", "测试2", "测试3"));

        graph.RemoveNode(1);

        DataHistory.Undo(graph);



        DataHistory.Redo(graph);

        var graphRequire = graph.GetNodeRequires(81);
        var graphLink = graph.GetNodeLinks(81);

        Console.WriteLine();
    }
    /// <summary>
    /// 序列化测试
    /// </summary>
    public static void FGraphToXmlTest()
    {
        var serializer = new XmlSerializer(typeof(FocusGraph));

        var graph = new FocusGraph("人类财阀联合.csv");

        XmlIO.SaveGraph("人类财阀联合.Graph.xml", graph);

        var readgraph = XmlIO.LoadGraph("人类财阀联合.Graph.xml");


    }
}