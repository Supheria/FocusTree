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

        [Flags]
        public enum test_Motions
        {
            #region ==== 基础 ====

            /// <summary>
            /// 没有实际影响，可能会发生改变
            /// </summary>
            NoneButMayChange = 0b1,
            /// <summary>
            /// 触发
            /// </summary>
            Trigger = NoneButMayChange << 1,
            /// <summary>
            /// 固定
            /// </summary>
            Modify = NoneButMayChange << 2,
            /// <summary>
            /// 限制范围
            /// </summary>
            Restrict = NoneButMayChange << 3,
            /// <summary>
            /// 追加
            /// </summary>
            Create = NoneButMayChange << 4,
            /// <summary>
            /// 加入
            /// </summary>
            Join = NoneButMayChange << 5,

            #endregion

            #region ==== 修改 ====

            /// <summary>
            /// 加
            /// </summary>
            Add = Modify | Modifications.Add,
            /// <summary>
            /// 减
            /// </summary>
            Sub = Modify | Modifications.Sub,
            /// <summary>
            /// 固定
            /// </summary>
            Fixed = Modify | Modifications.Fixed,
            /// <summary>
            /// 取消固定
            /// </summary>
            Unpin = Modify | Modifications.Unpin,
            /// <summary>
            /// 追加
            /// </summary>
            Append = Modify | Modifications.Append,
            /// <summary>
            /// 替换
            /// </summary>
            Replace = Modify | Modifications.Replace,
            /// <summary>
            /// 获得
            /// </summary>
            Gain = Modify | Modifications.Gain,
            /// <summary>
            /// 移除
            /// </summary>
            Remove = Modify | Modifications.Remove

            #endregion
        };
        /// <summary>
        /// 动作：修改
        /// </summary>
        [Flags]
        public enum Modifications
        {
            /// <summary>
            /// 加
            /// </summary>
            Add = test_Motions.Join << 1,
            /// <summary>
            /// 减
            /// </summary>
            Sub = Add << 2,
            /// <summary>
            /// 固定
            /// </summary>
            Fixed = Add << 3,
            /// <summary>
            /// 取消固定
            /// </summary>
            Unpin = Add << 4,
            /// <summary>
            /// 追加
            /// </summary>
            Append = Add << 5,
            /// <summary>
            /// 替换
            /// </summary>
            Replace = Add << 6,
            /// <summary>
            /// 获得
            /// </summary>
            Gain = Add << 7,
            /// <summary>
            /// 移除
            /// </summary>
            Remove = Add << 8
        }
    }
}
