// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
#define MAIN
using FocusTree;
using FocusTree.Data.Focus;
using FocusTree.IO;
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
#if MAIN
        //testInfo.Show();
        Application.Run(new GraphForm());
#else
        Test.FMapTest();
        Application.Run(Test.gTest);
#endif
    }
}

class Test
{
    public static GraphTest gTest = new();
    public static void FMapTest()
    {
        var graph = XmlIO.LoadFromXml<FocusGraph>("C:\\Users\\Non_E\\Documents\\GitHub\\FocusTree\\FocusTree\\program\\FILES\\神佑村落.xml");
        UploadNodeMap(graph);
        //var gRect = graph.GetGraphMetaRect();
        //var canvasWidth = LatticeCell.SizeMax.Width * (gRect.Width);
        ////var canvasHeight = LatticeCell.SizeMax.Height * (gRect.Height);
        //LatticeCell.Width = LatticeCell.SizeMax.Width;
        //LatticeCell.Height = LatticeCell.SizeMax.Height;
        //Lattice.SetBounds(new(0, 0, canvasWidth, canvasHeight));
        //Image cacher = new Bitmap(canvasWidth, canvasHeight);
        //gTest.LoadImageCacher(cacher, gRect);
        // FHistory.Enqueue(graph);

        //var suc = graph.AddNode(new FData(99,"Test", false, 0, "测试", "测试2", "测试3"));
    }
    /// <summary>
    /// 将节点绘制上载到栅格绘图委托（初始化节点列表时仅需上载第一次，除非节点列表或节点关系或节点位置信息发生变更才重新上载）
    /// </summary>
    public static void UploadNodeMap(FocusGraph Graph)
    {
        //Lattice.DrawingClear();
        foreach (var focus in Graph.FocusList)
        {
            int color = 0; //不同需求要变色
            foreach (var requires in focus.Requires)
            {
                foreach (var requireId in requires)
                {
                    //var require = Graph[requireId];
                    //GraphDrawer.UploadRequireLine(color, focus, require);
                }
                color++;
            }
            //var brush = id == SelectedNode ? GraphDrawer.NodeBG_Selected : GraphDrawer.NodeBG_Normal;
            //GraphDrawer.UploadNodeMap(focus);
        }
    }
}