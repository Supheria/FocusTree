﻿using FocusTree.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FocusTree.Focus
{
    internal class FHistory
    {
        /// <summary>
        /// 保留的历史记录数量（用于撤回和重做）
        /// </summary>
        private static (byte[], byte[])[] History = new (byte[], byte[])[20];
        /// <summary>
        /// 历史记录指针
        /// </summary>
        private static int Index = 0;
        private static int Length = 0;
        /// <summary>
        /// 判断是否有下一个历史记录
        /// </summary>
        /// <returns>是否有下一个历史记录</returns>
        public static bool HasNext()
        {
            return Index >= Length;
        }
        /// <summary>
        /// 判断是否有上一个历史记录
        /// </summary>
        /// <returns>是否有上一个历史记录</returns>
        public static bool HasPrev()
        {
            return Index > 0;
        }
        /// <summary>
        /// 将当前的状态添加到历史记录（会使后续的记录失效）
        /// </summary>
        /// <param name="graph">当前的Graph</param>
        [Obsolete("我不确定这东西有没有Bug，看起来很玄乎")]
        public static void Enqueue(FGraph graph)
        {
            var data = SerializeGraphData(graph);

            // 第一个历史记录
            if (Length == 0)
            {
                Length++;
                History[Index] = data;
                return;
            }

            // 新增的历史记录
            if(Length >= History.Length) // 如果已在历史记录的结尾
            {
                ShiftLeft();
                History[Index] = data;
                return;
            }

            // 如果在历史记录的中间
            if (Index < History.Length - 1) {
                Index++; // 指向下一个地址
                Length = Index + 1; // 扩展长度
                History[Index] = data;
                return;
            }
        }
        /// <summary>
        /// 撤回 (调用前需要检查 HasPrev())
        /// </summary>
        /// <param name="graph">当前的Graph</param>
        public static void Undo(FGraph graph)
        {
            Index--;
            DeSerializeGraphData(History[Index], ref graph);
            graph.UpdateNodes();
        }
        /// <summary>
        /// 重做 (调用前需要检查 HasNext())
        /// </summary>
        /// <param name="graph">当前的Graph</param>
        /// <exception cref="IndexOutOfRangeException">访问的历史记录越界</exception>
        public static void Redo(FGraph graph)
        {
            Index++;
            if (Index >= Length) { throw new IndexOutOfRangeException("[2302191735] 历史记录越界"); }
            DeSerializeGraphData(History[Index], ref graph);
            graph.UpdateNodes();
        }
        /// <summary>
        /// 将所有历史记录向左移动一位（不删除当前位）
        /// </summary>
        private static void ShiftLeft()
        {
            for(int i=0; i<Length-1; i++) {
                History[i] = History[i+1];
            }
        }
        /// <summary>
        /// 将 Graph 里的核心数据序列化为对象
        /// </summary>
        /// <param name="graph">Graph</param>
        /// <returns>序列化对象</returns>
        private static (byte[], byte[]) SerializeGraphData(FGraph graph)
        {
            var nodes = graph.GraphDataNodes_Get();
            var requires = graph.GraphDataRequires_Get();

            var nodeObj = JsonSerializer.SerializeToUtf8Bytes(nodes);
            var requiresObj = JsonSerializer.SerializeToUtf8Bytes(requires);

            return (nodeObj, requiresObj);
        }
        /// <summary>
        /// 反序列化 核心数据 到 Graph 里
        /// </summary>
        /// <param name="data">序列化的历史记录</param>
        /// <param name="graph">反序列化到目标</param>
        private static void DeSerializeGraphData((byte[], byte[]) data, ref FGraph graph)
        {
            graph.GraphDataNodes_Set(JsonSerializer.Deserialize<Dictionary<int, FData>>(data.Item1));
            graph.GraphDataRequires_Set(JsonSerializer.Deserialize<Dictionary<int, List<HashSet<int>>>>(data.Item2));
        }
    }
}
