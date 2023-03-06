namespace FocusTree.Data
{
    internal class GraphHistory
    {
        /// <summary>
        /// 保留的历史记录数量（用于撤回和重做）
        /// </summary>
        private static (string, string)[] History
            = new (string, string)[20];
        /// <summary>
        /// 历史记录指针
        /// </summary>
        private static int Index = 0;
        public static int Length
        {
            get { return length; }
            private set { length = value; }
        }
        static int length = 0;
        /// <summary>
        /// 判断是否有下一个历史记录
        /// </summary>
        /// <returns>是否有下一个历史记录</returns>
        public static bool HasNext()
        {
            return Index + 1 < Length;
        }
        /// <summary>
        /// 判断是否有上一个历史记录
        /// </summary>
        /// <returns>是否有上一个历史记录</returns>
        public static bool HasPrev()
        {
            return Index > 0;
        }
        public static bool HasHistory()
        {
            return Length > 1;
        }
        /// <summary>
        /// 将当前的状态添加到历史记录（会使后续的记录失效）
        /// </summary>
        /// <param name="graph">当前的Graph</param>
        //[Obsolete("我不确定这东西有没有Bug，看起来很玄乎")]
        public static void Enqueue(FocusGraph graph)
        {
            var data = graph.Serialize();

            if (Length == 0)
            {
                throw new Exception("[2303071704] GraphHistory 未初始化。");
            }

            // 新增的历史记录
            if (Length >= History.Length) // 如果已在历史记录的结尾
            {
                ShiftLeft();
                History[Index] = data;
                return;
            }

            // 如果在历史记录的中间
            if (Index < History.Length - 1)
            {
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
        public static void Undo(ref FocusGraph graph)
        {
            Index--;
            graph.Deserialize(History[Index]);
        }
        /// <summary>
        /// 重做 (调用前需要检查 HasNext())
        /// </summary>
        /// <param name="graph">当前的Graph</param>
        /// <exception cref="IndexOutOfRangeException">访问的历史记录越界</exception>
        public static void Redo(ref FocusGraph graph)
        {
            Index++;
            if (Index >= Length) 
            { 
                throw new IndexOutOfRangeException("[2302191735] 历史记录越界"); 
            }
            graph.Deserialize(History[Index]);
        }
        /// <summary>
        /// 将所有历史记录向左移动一位（不删除当前位）
        /// </summary>
        private static void ShiftLeft()
        {
            for (int i = 0; i < Length - 1; i++)
            {
                History[i] = History[i + 1];
            }
        }
        public static void Initialize(FocusGraph graph)
        {
            History = new (string, string)[History.Length];
            var data = graph.Serialize();
            History[Index] = data;
            Length = 1;
            Index = 0;
        }
    }
}
