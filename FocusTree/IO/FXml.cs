using FocusTree.Focus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FocusTree.IO
{
    internal class FXml
    {
        /// <summary>
        /// 将 FGraph 序列化成 xml
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <param name="graph">FGraph</param>
        public static void SaveGraph(string path, FGraph graph)
        {
            var writer = new XmlSerializer(typeof(FGraph));
            var file = File.Create(path);
            writer.Serialize(file, graph);
            file.Close();
        }
        /// <summary>
        /// 从 xml 文件中反序列化 FGraph
        /// </summary>
        /// <param name="path">xml文件路径</param>
        /// <returns>FGraph</returns>
        public static FGraph LoadGraph(string path)
        {
            var reader = new XmlSerializer(typeof(FGraph));
            var file = File.OpenRead(path);
            return reader.Deserialize(file) as FGraph;
        }
    }
}
