namespace FocusTree.Tool.Data
{
    internal interface IHistoryable : IFormattable
    {
        /// <summary>
        /// 保留的历史记录
        /// </summary>
        public IFormattedData[] History { get; }
        /// <summary>
        /// 最近一次保存时的数据
        /// </summary>
        public IFormattedData Latest { get; set; }
        /// <summary>
        /// 较最近一次保存是否已被编辑
        /// </summary>
        public bool IsEdit();
    }
}
