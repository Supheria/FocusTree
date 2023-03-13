using Newtonsoft.Json;

namespace FocusTree.Tool
{
    internal class JsObject
    {
        /// <summary>
        /// 将 Graph 里的核心数据序列化为对象
        /// </summary>
        /// <param name="graph">Graph</param>
        /// <returns>序列化对象</returns>
        public static (string, string) Serialize<MetaType1, MetaType2>(MetaType1 meta1, MetaType2 meta2)
        {
            var jsMeta1 = JsonConvert.SerializeObject(meta1);
            var jsMeat2 = JsonConvert.SerializeObject(meta2);
            return (jsMeta1, jsMeat2);
        }
        /// <summary>
        /// 反序列化 核心数据 到 Graph 里
        /// </summary>
        /// <param name="data">序列化的历史记录</param>
        /// <param name="graph">反序列化到目标</param>
        public static (MetaType1, MetaType2) DeSerialize<MetaType1, MetaType2>((string, string) data)
        {
            var meta1 = JsonConvert.DeserializeObject<MetaType1>(data.Item1);
            var meta2 = JsonConvert.DeserializeObject<MetaType2>(data.Item2);
            return (meta1, meta2);
        }
    }
}
