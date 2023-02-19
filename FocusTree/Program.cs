﻿// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using FocusTree;
using FocusTree.Focus;
using FocusTree.IO;
using FocusTree.Tree;
using FocusTree.UI;
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
        Test.FMapTest();
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
        var graph = new FGraph("人类财阀联合.csv");

        var graphRequire = graph.GetNodeRequires(81);

        Console.WriteLine();
    }
    /// <summary>
    /// 序列化测试
    /// </summary>
    public static void FGraphToXmlTest()
    {
        var serializer = new XmlSerializer(typeof(FGraph));

        var graph = new FGraph("人类财阀联合.csv");

        FXml.SaveGraph("人类财阀联合.Graph.xml", graph);

        var readgraph = FXml.LoadGraph("人类财阀联合.Graph.xml");


    }
}