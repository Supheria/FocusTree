using System.Xml.Serialization;
using FocusTree.Data;

namespace FocusTree.IO
{
    internal class XmlIO
    {
        /// <summary>
        /// 将 FGraph 序列化成 xml
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <param name="graph">FGraph</param>
        public static void SaveGraph(string path, FocusGraph graph)
        {
            var writer = new XmlSerializer(typeof(FocusGraph));
            var file = File.Create(path);
            writer.Serialize(file, graph);
            file.Close();
        }
        /// <summary>
        /// 从 xml 文件中反序列化 FGraph
        /// </summary>
        /// <param name="path">xml文件路径</param>
        /// <returns>FGraph</returns>
        public static FocusGraph LoadGraph(string path)
        {
            var reader = new XmlSerializer(typeof(FocusGraph));
            var file = File.OpenRead(path);
            var graph = reader.Deserialize(file) as FocusGraph;
            graph.SetFileName(path);
            file.Close();
            return graph;
        }
    }
}
