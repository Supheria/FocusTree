namespace FocusTree.Tool.Data
{
    internal interface IHistoryable
    {
        /// <summary>
        /// 保留的历史记录
        /// </summary>
        public IHistoryData[] History { get; }
        /// <summary>
        /// 获取格式化数据
        /// </summary>
        /// <returns>格式化数据</returns>
        public IHistoryData Format();
        /// <summary>
        /// 复原格式化数据
        /// </summary>
        /// <param name="data">格式化数据</param>
        public void Deformat(IHistoryData data);
    }
}
