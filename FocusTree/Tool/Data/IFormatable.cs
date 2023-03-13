namespace FocusTree.Tool.Data
{
    internal interface IFormatable<ForamtData>
    {
        /// <summary>
        /// 保留的历史记录
        /// </summary>
        public ForamtData[] History { get; }
        /// <summary>
        /// 获取格式化数据
        /// </summary>
        /// <returns>格式化数据</returns>
        public ForamtData Format();
        /// <summary>
        /// 复原格式化数据
        /// </summary>
        /// <param name="data">格式化数据</param>
        public void Deformat(ForamtData data);
    }
}
