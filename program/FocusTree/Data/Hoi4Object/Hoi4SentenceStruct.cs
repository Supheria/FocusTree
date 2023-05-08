using static FocusTree.Data.Hoi4Object.PublicSign;

namespace FocusTree.Data.Hoi4Object
{
    /// <summary>
    /// 句子属性
    /// </summary>
    public struct Hoi4SentenceStruct
    {
        /// <summary>
        /// 执行动作
        /// </summary>
        public Motions Motion;
        /// <summary>
        /// 值类型
        /// </summary>
        public Types ValueType;
        /// <summary>
        /// 执行值
        /// </summary>
        public string Value;
        /// <summary>
        /// 触发者类型
        /// </summary>
        public Types TriggerType;
        /// <summary>
        /// 动作触发者
        /// </summary>
        public string[] Triggers;
        public Hoi4SentenceStruct()
        {
            Motion = Motions.None;
            ValueType = Types.None;
            Value = string.Empty;
            TriggerType = Types.None;
            Triggers = Array.Empty<string>();
        }
    }
}
