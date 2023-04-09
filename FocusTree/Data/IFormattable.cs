namespace FocusTree.Data
{
    public interface IFormattable
    {
        /// <summary>
        /// 获取格式化数据
        /// </summary>
        /// <returns>格式化数据</returns>
        public FormattedData Format();
        /// <summary>
        /// 复原格式化数据
        /// </summary>
        /// <param name="data">格式化数据</param>
        public void Deformat(FormattedData IData);
    }
}
