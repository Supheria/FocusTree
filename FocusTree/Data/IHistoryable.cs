namespace FocusTree.Data
{
    internal interface IHistoryable<DataType>
    {
        /// <summary>
        /// 保留的历史记录数量（用于撤回和重做）
        /// </summary>
        public DataType[] History { get; }
        public DataType Serialize();
        public void Deserialize(int index);
    }
}
