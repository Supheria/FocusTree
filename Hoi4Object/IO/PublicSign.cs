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
        /// 全句起始
        /// </summary>
        public static string _BEGIN_ = "{ ";
        /// <summary>
        /// 全句终止
        /// </summary>
        public static string _END_ = " }";
        /// <summary>
        /// 分割子句
        /// </summary>
        public static string _CLAUSE_SPLITTER_ = ", ";
        /// <summary>
        /// 子句内分割
        /// </summary>
        public static string _PHRASE_SPLITTER_ = " | ";
        /// <summary>
        /// 赋值
        /// </summary>
        public static string _ASSIGNMENT_ = ": ";
        /// <summary>
        /// 执行动作
        /// </summary>
        public enum Motions
        {
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


        #region ==== 赋值工具 ====

        /// <summary>
        /// 标签赋值
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string Assign(Tags tag, string? value)
        {
            return Enum.GetName(typeof(Tags), tag) +
                _ASSIGNMENT_ +
                (value != null ? value : "null");
        }

        #endregion

        #region ==== 格式化模板 ====

        /// <summary>
        /// 格式化模板-通用
        /// </summary>
        /// <param name="Motion">执行动作，不可为空</param>
        /// <param name="Type">执行对象类型，不可为空</param>
        /// <param name="Object">执行对象</param>
        /// <param name="TriggerState">触发动作的国家</param>
        /// <param name="SufferState">动作受施的国家</param>
        /// <param name="Duraion">持续时长</param>
        /// <param name="Data">执行数据</param>
        /// <returns>格式化语句</returns>
        public static string Formatter(Motions Motion, Types Type, string? Object, string? Data = null, string? TriggerState = null, string? SufferState = null, string? Duraion = null)
        {
            return _BEGIN_ +
                Assign(Tags.Motion, Enum.GetName(typeof(Motions), Motion)) + _CLAUSE_SPLITTER_ +
                Assign(Tags.Type, Enum.GetName(typeof(Types), Type)) + _CLAUSE_SPLITTER_ +
                Assign(Tags.Object, Object) + _CLAUSE_SPLITTER_ +
                Assign(Tags.TriggerState, TriggerState) + _CLAUSE_SPLITTER_ +
                Assign(Tags.SufferState, SufferState) + _CLAUSE_SPLITTER_ +
                Assign(Tags.Duration, Duraion) + _CLAUSE_SPLITTER_ +
                Assign(Tags.Data, Data) +
                _END_;
        }
        /// <summary>
        /// 格式化模板-有可用性的执行对象
        /// </summary>
        /// <param name="Motion">执行动作，不可为空</param>
        /// <param name="AvailabilityObject">有可用性的执行对象，不可为空</param>
        /// <param name="Data">执行数据</param>
        /// <param name="TriggerState">触发动作的国家</param>
        /// <param name="SufferState">动作受施的国家</param>
        /// <param name="Duraion">持续时长</param>
        /// <returns></returns>
        public static string Formatter(Motions Motion, AvailabilityObjects AvailabilityObject, string? Data = null, string? TriggerState = null, string? SufferState = null, string? Duraion = null)
        {
            return Formatter(
                Motion,
                Types.Availability,
                Enum.GetName(typeof(AvailabilityObjects), AvailabilityObject),
                Data,
                TriggerState,
                SufferState,
                Duraion
                );
        }

        #endregion
    }
}
