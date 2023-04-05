using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Hoi4Object.IO.PublicSign;

namespace Hoi4Object.IO
{
    public static class PublicSign
    {
        #region ==== 符号 ====

        /// <summary>
        /// 替换为
        /// </summary>
        public static string ReplaceTo = ":";
        /// <summary>
        /// 执行动作
        /// </summary>
        public enum Motions
        {
            None = 0,
            /// <summary>
            /// 触发
            /// </summary>
            Trigger,
            /// <summary>
            /// 固定
            /// </summary>
            Fixed,
            /// <summary>
            /// 取消固定
            /// </summary>
            Unpin,
            /// <summary>
            /// 增加、获得
            /// </summary>
            UpperModify,
            /// <summary>
            /// 减少、移除
            /// </summary>
            LowerModify,
            /// <summary>
            /// 修改
            /// </summary>
            Modify,
            /// <summary>
            /// 限制范围
            /// </summary>
            Restrict,
            /// <summary>
            /// 追加
            /// </summary>
            Append
        };
        /// <summary>
        /// 受施对象类型
        /// </summary>
        public enum Types
        {
            None = 0,
            /// <summary>
            /// 事件
            /// </summary>
            Event,
            /// <summary>
            /// 可同意的事件
            /// </summary>
            RequestEvent,
            /// <summary>
            /// 变量
            /// </summary>
            Variable,
            /// <summary>
            /// 标签
            /// </summary>
            Label,
            /// <summary>
            /// 可用性
            /// </summary>
            Availability,
            /// <summary>
            /// 研究加成
            /// </summary>
            ResearchBonus,
            /// <summary>
            /// 等级
            /// </summary>
            Grade,
            /// <summary>
            /// 区域
            /// </summary>
            Region
        }
        /// <summary>
        /// 赋值标签
        /// </summary>
        public enum Tags
        {
            /// <summary>
            /// 执行动作
            /// </summary>
            Motion,
            /// <summary>
            /// 执行对象类型
            /// </summary>
            Type,
            /// <summary>
            /// 触发动作的国家
            /// </summary>
            TriggerState,
            /// <summary>
            /// 动作受施的国家
            /// </summary>
            SufferState,
            /// <summary>
            /// 执行对象
            /// </summary>
            Object,
            /// <summary>
            /// 持续时长
            /// </summary>
            Duration,
            /// <summary>
            /// 执行数据
            /// </summary>
            Data
        }
        /// <summary>
        /// 有可用性的对象
        /// </summary>
        public enum AvailabilityObjects
        {
            /// <summary>
            /// 宣战
            /// </summary>
            DeclarationOfWar,
            /// <summary>
            /// 决议
            /// </summary>
            Resolution,
            /// <summary>
            /// 自动获取核心
            /// </summary>
            AutoGainCore,
            /// <summary>
            /// 战争目标
            /// </summary>
            WarGoal
        }

        #endregion
    }
}
