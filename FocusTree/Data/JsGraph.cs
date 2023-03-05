using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Data
{
    internal class JsGraph
    {
        /// <summary>
        /// 将 Graph 里的核心数据序列化为对象
        /// </summary>
        /// <param name="graph">Graph</param>
        /// <returns>序列化对象</returns>
        public static (string, string) SerializeGraph(FocusGraph graph)
        {
            if (graph == null)
            {
                return (null, null);
            }
            var jsNodes = JsonConvert.SerializeObject(graph.NodesCatalog);
            var jsRequires = JsonConvert.SerializeObject(graph.RequireGroups);
            return (jsNodes, jsRequires);
        }
        /// <summary>
        /// 反序列化 核心数据 到 Graph 里
        /// </summary>
        /// <param name="data">序列化的历史记录</param>
        /// <param name="graph">反序列化到目标</param>
        public static void DeSerializeGraph((string, string) data, ref FocusGraph graph)
        {
            var nodesCatalog = JsonConvert.DeserializeObject<Dictionary<int, FocusData>>(data.Item1);
            var requireGroups = JsonConvert.DeserializeObject<Dictionary<int, List<HashSet<int>>>>(data.Item2);
            graph.NodesCatalog = nodesCatalog;
            graph.RequireGroups = requireGroups;
        }
    }
}
