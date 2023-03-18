using System;

namespace FocusTree.Tool.Data
{
    public interface IHistoryable : IFormattable
    {
        /// <summary>
        /// 历史记录指针
        /// </summary>
        public int HistoryIndex { get; set; }
        /// <summary>
        /// 历史记录长度
        /// </summary>
        public int CurrentHistoryLength { get; set; }
        /// <summary>
        /// 开辟的的历史记录保存空间
        /// </summary>
        public FormattedData[] History { get; set; }
        /// <summary>
        /// 最近一次保存时的数据
        /// </summary>
        public FormattedData Latest { get; set; }
    }
}