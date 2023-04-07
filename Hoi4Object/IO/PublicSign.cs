using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4Object.IO
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
            /// 有错误
            /// </summary>
            IsErro = 0b1,
            /// <summary>
            /// 没有实际影响，可能会发生改变
            /// </summary>
            NoneButMayChange = IsErro << 1,
            /// <summary>
            /// 触发
            /// </summary>
            Trigger = NoneButMayChange << 1,
            /// <summary>
            /// 修改
            /// </summary>
            Modify = Trigger << 1,
            /// <summary>
            /// 创建
            /// </summary>
            Create = Modify << 1,
            /// <summary>
            /// 加入
            /// </summary>
            Join = Create << 1,
            /// <summary>
            /// 吞并
            /// </summary>
            Annexed = NoneButMayChange << 5,

            #endregion

            #region ==== 修改 ====

            /// <summary>
            /// 加
            /// </summary>
            Add = Join << 1 | Modify,
            /// <summary>
            /// 减
            /// </summary>
            Sub = Modify|(Add ^ Modify) << 1,
            /// <summary>
            /// 固定
            /// </summary>
            Fixed = (Sub ^ Modify) << 1 | Modify,
            /// <summary>
            /// 取消固定
            /// </summary>
            Unpin = (Fixed ^ Modify) << 1 | Modify,
            /// <summary>
            /// 追加
            /// </summary>
            Append = (Unpin ^ Modify) << 1 | Modify,
            /// <summary>
            /// 获得
            /// </summary>
            Gain = (Append ^ Modify) << 1 | Modify,
            /// <summary>
            /// 移除
            /// </summary>
            Remove = (Gain ^ Modify) << 1 | Modify,
            /// <summary>
            /// 加成
            /// </summary>
            Bonus = (Remove ^ Modify) << 1 | Modify,
            /// <summary>
            /// 取代
            /// </summary>
            Replace = (Bonus ^ Modify) << 1 | Modify

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
            Variable = RequestEvent << 1,
            /// <summary>
            /// 标签
            /// </summary>
            Label = Variable << 1,
            /// <summary>
            /// 可用性
            /// </summary>
            Availability = Label << 1,
            /// <summary>
            /// 区域
            /// </summary>
            Region = Availability << 1,
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
            /// <summary>
            /// 国家
            /// </summary>
            State = Event << 11,

            #endregion

            #region ==== 可用性 ====

            /// <summary>
            /// 可以宣战
            /// </summary>
            AbleToDeclareWar = Availability | State << 1,
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
