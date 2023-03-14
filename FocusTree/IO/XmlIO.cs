using FocusTree.Data;
using System.Xml.Serialization;

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
            try
            {
                var file = File.Create(path);
                var writer = new XmlSerializer(typeof(FocusGraph));
                graph.FilePath = path;
                graph.Latest = graph.Format();
                writer.Serialize(file, graph);
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[2303051316]无法保存文件。\n{ex.Message}");
            }
        }
        /// <summary>
        /// 从 xml 文件中反序列化 FGraph
        /// </summary>
        /// <param name="path">xml文件路径</param>
        /// <returns>FGraph</returns>
        public static FocusGraph LoadGraph(string path)
        {
            try
            {
                var file = File.OpenRead(path);
                var reader = new XmlSerializer(typeof(FocusGraph));
                var graph = reader.Deserialize(file) as FocusGraph;
                file.Close();
                return graph;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[2303051302]不受支持的文件。\n{ex.Message}");
                return null;
            }
        }
    }
}
