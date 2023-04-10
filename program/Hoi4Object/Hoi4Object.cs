namespace Hoi4Object
{
    /// <summary>
    /// 钢4数据对象
    /// </summary>
    public abstract class Hoi4Object
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; } = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        public string Caption { get; private set; } = string.Empty;
    }
}
