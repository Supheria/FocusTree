using FocusTree.Data.Focus;
using System.Xml.Serialization;

namespace FocusTree.IO
{
    public static class XmlIO
    {
        /// <summary>
        /// 将 FGraph 序列化成 xml
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <param name="graph">FGraph</param>
        public static void SaveToXml<T>(this T obj, string path) where T : IXmlSerializable
        {
            var file = File.Create(path);
            var writer = new XmlSerializer(typeof(T));
            writer.Serialize(file, obj);
            file.Close();
        }
        /// <summary>
        /// 从 xml 文件中反序列化 FGraph
        /// </summary>
        /// <param name="path">xml文件路径</param>
        /// <returns>FGraph</returns>
        public static T LoadFromXml<T>(string path) where T : IXmlSerializable
        {
            try
            {
                var file = File.OpenRead(path);
                var reader = new XmlSerializer(typeof(T));
                var obj = (T)reader.Deserialize(file);
                file.Close();
                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception($"[2304130225]无法读取{path}。\n{ex.Message}");
            }
        }
    }
}
