using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Data.Hoi4Object
{
    /// <summary>
    /// 句子属性
    /// </summary>
    public struct SentenceAttribute
    {
        /// <summary>
        /// 执行动作
        /// </summary>
        public string Motion;
        /// <summary>
        /// 值类型
        /// </summary>
        public string ValueType;
        /// <summary>
        /// 执行值
        /// </summary>
        public string Value;
        /// <summary>
        /// 触发者类型
        /// </summary>
        public string TriggerType;
        /// <summary>
        /// 动作触发者
        /// </summary>
        public string Trigger;
        public SentenceAttribute(string motion, string valueType, string value, string triggerType, string trigger)
        {
            Motion = motion;
            ValueType = valueType;
            Value = value;
            TriggerType = triggerType;
            Trigger = trigger;
        }
    }
}
