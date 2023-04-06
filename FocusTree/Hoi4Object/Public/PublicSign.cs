using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Hoi4Object.Public
{
    public static class PublicSign
    {
        public static char Splitter = '|';
        /// <summary>
        /// 执行动作
        /// </summary>
        [Flags]
        public enum Motions
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
            /// 修改
            /// </summary>
            Modify = NoneButMayChange << 2,
            /// <summary>
            /// 创建
            /// </summary>
            Create = NoneButMayChange << 3,
            /// <summary>
            /// 加入
            /// </summary>
            Join = NoneButMayChange << 4,

            #endregion

            #region ==== 修改 ====

            /// <summary>
            /// 加
            /// </summary>
            Add = Modify | Join << 1,
            /// <summary>
            /// 减
            /// </summary>
            Sub = Modify | (Add ^ Modify) << 1,
            /// <summary>
            /// 固定
            /// </summary>
            Fixed = Modify | (Add ^ Modify) << 2,
            /// <summary>
            /// 取消固定
            /// </summary>
            Unpin = Modify | (Add ^ Modify) << 3,
            /// <summary>
            /// 追加
            /// </summary>
            Append = Modify | (Add ^ Modify) << 4,
            /// <summary>
            /// 获得
            /// </summary>
            Gain = Modify | (Add ^ Modify) << 5,
            /// <summary>
            /// 移除
            /// </summary>
            Remove = Modify | (Add ^ Modify) << 6,
            /// <summary>
            /// 加成
            /// </summary>
            Bonus = Modify | (Add ^ Modify) << 7,
            /// <summary>
            /// 取代
            /// </summary>
            Replace = Modify | (Add ^ Modify) << 8

            #endregion
        };
        /// <summary>
        /// 受施对象类型
        /// </summary>
        public enum Types
        {
            #region ==== 基础 ====

            /// <summary>
            /// 事件
            /// </summary>
            Event = 0b1,
            /// <summary>
            /// 可同意的事件
            /// </summary>
            RequestEvent = Event << 1,
            /// <summary>
            /// 变量
            /// </summary>
            Variable = Event << 2,
            /// <summary>
            /// 标签
            /// </summary>
            Label = Event << 3,
            /// <summary>
            /// 可用性
            /// </summary>
            Availability = Event << 4,
            /// <summary>
            /// 区域
            /// </summary>
            Region = Event << 5,
            /// <summary>
            /// 战争目标
            /// </summary>
            WarGoal = Event << 6,
            /// <summary>
            /// 决议
            /// </summary>
            Resolution = Event << 7,
            /// <summary>
            /// 阵营
            /// </summary>
            Camp = Event << 8,
            /// <summary>
            /// 研究
            /// </summary>
            Reaserch = Event << 9,
            /// <summary>
            /// 等级
            /// </summary>
            Grade = Event << 10,

            #endregion

            #region ==== 可用性 ====

            /// <summary>
            /// 可以宣战
            /// </summary>
            AbleToDeclareWar = Availability | Grade << 1,
            /// <summary>
            /// 可以自动获取核心
            /// </summary>
            AbleToGainCoreAuto = Availability | (AbleToDeclareWar ^ Availability) << 1,
            /// <summary>
            /// 可以创建阵营
            /// </summary>
            AbleToCreateCamp = Availability | (AbleToDeclareWar ^ Availability) << 2,
            /// <summary>
            /// 可以加入阵营
            /// </summary>
            AbleToJoinCamp = Availability | (AbleToDeclareWar ^ Availability) << 3

            #endregion
        }
    }
}
