using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Tool.Data
{
    public static class ObjectHistory
    {
        public static int GetHistoryLength<T>(this T obj) where T : IHistoryable
        {
            return obj.CurrentHistoryLength;
        }
        /// <summary>
        /// 较最近一次保存是否已被编辑
        /// </summary>
        public static bool IsEdit<T>(this T obj) where T : IHistoryable
        {
            return !obj.Latest.Equals(obj.Format());
        }
        /// <summary>
        /// 判断是否有下一个历史记录
        /// </summary>
        /// <returns>是否有下一个历史记录</returns>
        public static bool HasNext<T>(this T obj) where T : IHistoryable
        {
            return obj.HistoryIndex + 1 < obj.CurrentHistoryLength;
        }
        /// <summary>
        /// 判断是否有上一个历史记录
        /// </summary>
        /// <returns>是否有上一个历史记录</returns>
        public static bool HasPrev<T>(this T obj) where T : IHistoryable
        {
            return obj.HistoryIndex > 0;
        }
        /// <summary>
        /// 将当前的状态添加到历史记录（会使后续的记录失效）
        /// </summary>
        /// <param name="graph">当前的Graph</param>
        //[Obsolete("我不确定这东西有没有Bug，看起来很玄乎")]
        public static void EnqueueHistory<T>(this T obj) where T : IHistoryable
        {
            var data = obj.Format();
            // 第一个历史记录
            if (obj.CurrentHistoryLength == 0)
            {
                obj.CurrentHistoryLength++;
                obj.History[obj.HistoryIndex] = data;
                return;
            }

            // 新增的历史记录
            if (obj.CurrentHistoryLength >= obj.History.Length) // 如果已在历史记录的结尾
            {
                // 将所有历史记录向左移动一位（不删除当前位）
                for (int i = 0; i < obj.CurrentHistoryLength - 1; i++)
                {
                    obj.History[i] = obj.History[i + 1];
                }
                obj.History[obj.HistoryIndex] = data;
                return;
            }

            // 如果在历史记录的中间
            if (obj.HistoryIndex < obj.History.Length - 1)
            {
                obj.HistoryIndex++; // 指向下一个地址
                obj.CurrentHistoryLength = obj.HistoryIndex + 1; // 扩展长度
                obj.History[obj.HistoryIndex] = data;
                return;
            }
        }
        /// <summary>
        /// 撤回 (调用前需要检查 HasPrev())
        /// </summary>
        /// <param name="graph">当前的Graph</param>
        public static void Undo<T>(this T obj) where T : IHistoryable
        {
            if (obj.HasPrev() == false)
            {
                return;
            }
            obj.HistoryIndex--;
            obj.Deformat(obj.History[obj.HistoryIndex]);
        }
        /// <summary>
        /// 重做 (调用前需要检查 HasNext())
        /// </summary>
        /// <param name="graph">当前的Graph</param>
        /// <exception cref="HistoryIndexOutOfRangeException">访问的历史记录越界</exception>
        public static void Redo<T>(this T obj) where T : IHistoryable
        {
            if (obj.HasNext() == false)
            {
                return;
            }
            obj.HistoryIndex++;
            if (obj.HistoryIndex >= obj.CurrentHistoryLength)
            {
                throw new IndexOutOfRangeException("[2302191735] 历史记录越界");
            }
            obj.Deformat(obj.History[obj.HistoryIndex]);
        }
        public static void ClearHistory<T>(this T obj) where T : IHistoryable
        {
            obj.Latest = obj.Format();
            obj.CurrentHistoryLength = 0;
            obj.HistoryIndex = 0;
        }
    }
}
