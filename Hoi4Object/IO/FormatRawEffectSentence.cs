using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Hoi4Object.IO
{
    /// <summary>
    /// 格式化原始效果语句
    /// </summary>
    internal class FormatRawEffectSentence
    {
        #region ==== 基本变量 ====

        /// <summary>
        /// 收集无法格式化的语句
        /// </summary>
        public static List<string> Unformattable { get; private set; } = new List<string>();

        #endregion

        #region ==== 正则模式 ====

        /// <summary>
        /// 单语句-触发事件
        /// 触发事件“骑着青牛的老者？”。
        /// </summary>
        static string Pattern_Single_TriggerEvent = "^(?:触发事件“)([\u4e00-\u9fa5？]+)";
        /// <summary>
        /// 单语句-其他国家触发事件
        /// 神灵庙触发事件“道教该如何面对道家？”
        /// </summary>
        static string Pattern_Single_OtherTriggerEvent = "^([\u4e00-\u9fa5]+)(?:触发事件“)([\u4e00-\u9fa5？]+)(?:”)$";
        /// <summary>
        /// 单语句-固定值或类型
        /// 将平均灵力值固定为50%
        /// 将世界观固定为唯心世界观
        /// </summary>
        static string Pattern_Single_FixValue = "^(?:将)([\u4e00-\u9fa5]+)(?:固定为)(.+)";
        /// <summary>
        /// 单语句-移除固定
        /// 移除对势力规模的固定
        /// </summary>
        static string Pattern_Single_RemoveFixation = "^(?:移除对)([\u4e00-\u9fa5]+)(?:的固定)";
        /// <summary>
        /// 单语句-加|减值
        /// x可以加入阵营灵力系科研速度：+35%
        /// 稳定度：-30%
        /// </summary>
        static string Pattern_Single_AddSub = "^([\u4e00-\u9fa5x]+)(?:：)([+|-])(\\d+%?)";
        /// <summary>
        /// 单语句-获得|增加|移除个数
        /// 获得1个科研槽
        /// 增加10个建筑位
        /// 移除1个民用工厂
        /// </summary>
        static string Pattern_Single_GainRemove = "^(获得|增加|移除)(\\d+)(?:个)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 单语句-获得|移除标签
        /// 获得五弊三缺
        /// 移除道教的探索精神
        /// </summary>
        static string Pattern_Single_ModifyLabel = "^(获得|移除)([^对][\u4e00-\u9fa5]+)$";
        /// <summary>
        /// 单语句-宣战可用性
        /// 规则修改：不能宣战
        /// 规则修改：可以宣战
        /// </summary>
        static string Pattern_Single_DeclarationOfWar = "^(?:规则修改：)(不能|可以)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 单语句-决议可用性
        /// 可以通过决议发动周边国家的内战使其变为附庸
        /// </summary>
        static string Pattern_Single_ResolutionAvailability = "^(可以|不能)(?:通过决议)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 单语句-自动获得核心可用性
        /// 幽灵种族的省份将自动获得核心：是
        /// </summary>
        static string Pattern_Single_AutoCoreAvailability = "^([\u4e00-\u9fa5]+)(?:的省份将自动获得核心：)([是|否])";
        /// <summary>
        /// 单语句-对他国的修正
        /// （天狗共和国）我国对其进攻修正：+20%
        /// </summary>
        static string Pattern_Single_ModifyToOther = "^(?:（)([\u4e00-\u9fa5]+)(?:）我国对其)(进攻|防御)(?:修正：)([+-])(\\d+%?)";
        /// <summary>
        /// 单语句-获得对别国的战争目标
        /// 获得对守矢神社的吞并战争目标
        /// </summary>
        static string Pattern_Single_GetWarGoal = "^(?:获得对)([\u4e00-\u9fa5]+)(?:的)([\u4e00-\u9fa5]+)(?:战争目标)";
        /// <summary>
        /// 单语句-研究加成
        /// 3x60%研究加成：基础武器。
        /// </summary>
        static string Pattern_Single_ResearchBonus = "^(\\dx\\d+%?)(?:研究加成：)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 复语句-获得|移除标签
        /// 获得无为而治，其效果为（每周人口：+1，每月人口：+15%）
        /// 移除圣人，其效果为（孤立倾向：+0.1，每日唯心度变化：+0.04%）
        /// </summary>
        static string Pattern_Complex_ModifyLabel = "^(获得|移除)([\u4e00-\u9fa5]+)(?:，其效果为（)([^）]+)";
        /// <summary>
        /// 复语句-获得限时效果
        /// 获得为期365天的“强大的威望”，其效果为（每日获得的政治点数：+0.25，部队组织度：+15%，稳定度：+20%，战争支持度：+20%，建造速度：+10%，部队攻击：+5%，部队防御：+5%，科研速度：+10%，工厂产出：+10%）
        /// </summary>
        static string Pattern_Complex_GainLabelWithinTime = "^(?:获得为期)(\\d+[天月年])(?:的“)([\u4e00-\u9fa5]+)(?:”，其效果为（)([^）]+)";
        /// <summary>
        /// 复语句-某国获得标签
        /// （隐居村落）获得自古以来（对命莲寺关系：+200）
        /// </summary>
        static string Pattern_Complex_SomeoneGainLabel = "^(?:（)([\u4e00-\u9fa5]+)(?:）)(获得|失去)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
        /// <summary>
        /// 复语句-每个国家获得标签
        /// 每个国家获得：道德观不同（对隐世村落关系：-20）
        /// 每个国家失去：道德观不同（对隐世村落关系：-20）
        /// </summary>
        static string Pattern_Complex_EveryoneModifyLabel = "^(每个国家)(获得|失去)(?:：)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
        /// <summary>
        /// 复语句-替换等级
        /// 向保守社会发展一级，以保守社会5取代保守社会4，效果变化（稳定度：+6% 科研速度：-3% 加密：+0.5 可出口资源：-5% 意识形态变化抵制力度：+10%）
        /// </summary>
        static string Pattern_Complex_ModifyGrade = "^(?:向)(?:[\u4e00-\u9fa5]+)(?:发展一级，以)([\u4e00-\u9fa5]+\\d)(?:取代)([\u4e00-\u9fa5]+\\d)(?:，效果变化（)([^）]+)";
        /// <summary>
        /// 复语句-追加效果
        /// 天地不仁追加效果：稳定度：-5%，适役人口：+2%
        /// </summary>
        static string Pattern_Complex_Append = "^([\u4e00-\u9fa5]+)(?:追加效果：)(.+)$";
        /// <summary>
        /// 复语句-某国触发可同意事件
        /// 每个孤立国家触发事件“隐世村落的无偿教导？”。如果他们同意，则获得强烈的战略结盟（对隐居村落的关系：+100）
        /// </summary>
        static string Pattern_Complex_SomeoneTriggerEventWithConsent = "^([\u4e00-\u9fa5]+)(?:触发事件“)([\u4e00-\u9fa5？]+)(?:”。如果他们同意，则获得)(.+)";
        /// <summary>
        /// 复语句-区域限制
        /// 村民生活区（增加10个建筑位，增加10个民用工厂）
        /// </summary>
        static string Pattern_Complex_RegionRestrict = "^([\u4e00-\u9fa5]+)(?:（)([^）]+)";

        #endregion

        #region ==== 格式化字段 ====

        /// <summary>
        /// 全句起始
        /// </summary>
        static string _BEGIN_ = "{ ";
        /// <summary>
        /// 全句终止
        /// </summary>
        static string _END_ = " }";
        /// <summary>
        /// 分割子句
        /// </summary>
        static string _CLAUSE_SPLITTER_ = ", ";
        /// <summary>
        /// 子句内分割
        /// </summary>
        static string _PHRASE_SPLITTER_ = " | ";
        /// <summary>
        /// 赋值
        /// </summary>
        static string _ASSIGNMENT_ = ": ";
        /// <summary>
        /// 施加动作
        /// </summary>
        enum Motion
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
        enum Type
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
        enum Tag
        {
            /// <summary>
            /// 触发国家
            /// </summary>
            TriggerState,
            /// <summary>
            /// 受施国家
            /// </summary>
            SufferState,
            /// <summary>
            /// 执行对象
            /// </summary>
            Object,
            /// <summary>
            /// 有效时长
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
        enum Availability
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

        #region ==== 格式化工具 ====

        /// <summary>
        /// Motion&Type tag赋值
        /// </summary>
        /// <param name="motions"></param>
        /// <returns></returns>
        private static string Assign(Motion motion, Type type)
        {
            return "Motion" + _ASSIGNMENT_ + Enum.GetName(typeof(Motion), motion) +
                _CLAUSE_SPLITTER_ +
                "Type" + _ASSIGNMENT_ + Enum.GetName(typeof(Type), type);
        }
        /// <summary>
        /// Object tag赋值：可用性对象
        /// </summary>
        /// <param name="availability"></param>
        /// <returns></returns>
        private static string Assign(Availability availability)
        {
            return "Object" + _ASSIGNMENT_ + Enum.GetName(typeof(Availability), availability);
        }
        /// <summary>
        /// 标签赋值
        /// </summary>
        /// <param name="tag">标签</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static string Assign(Tag tag, string value)
        {
            return Enum.GetName(typeof(Tag), tag) + _ASSIGNMENT_ + value;
        }

        private static bool GetMatch(string input, string pattern, out Match match)
        {
            match = Regex.Match(input, pattern);
            return match.Success;
        }

        #endregion

        #region ==== 主方法 ====

        /// <summary>
        /// 格式化器
        /// </summary>
        /// <param name="sentence">原始效果语句</param>
        /// <param name="formatted">格式化后的语句</param>
        /// <returns>如果格式化成功则返回true，否则返回false并记入Unformattable</returns>
        public static bool Formatter(string sentence, out string formatted)
        {
            if (SinglePatternFormatter(sentence, out formatted) ) 
            { 
                return true; 
            }
            if (ComplexPatternFormatter(sentence, out formatted))
            {
                return true;
            }
            Unformattable.Add(sentence);
            return false;
        }

        #endregion

        #region ==== 格式化方法 ====

        /// <summary>
        /// 格式化单语句
        /// </summary>
        /// <param name="str"></param>
        /// <returns>返回格式化后的单语句，如果无匹配的格式化模式则返回null</returns>
        private static bool SinglePatternFormatter(string str, out string formatted)
        {
            formatted = string.Empty;
            if (GetMatch(str, Pattern_Single_TriggerEvent, out Match match))
            {
                formatted = _BEGIN_ +
                    Assign(Motion.Trigger, Type.Event) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[1].Value) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_OtherTriggerEvent, out match))
            {
                formatted = _BEGIN_ +
                    Assign(Motion.Trigger, Type.Event) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.TriggerState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[2].Value) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_FixValue, out match))
            {
                formatted = _BEGIN_ +
                    Assign(Motion.Fixed, Type.Variable) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, match.Groups[2].Value) +
                    _END_;
                return true;
            }
            else if (GetMatch(str, Pattern_Single_RemoveFixation, out match))
            {
                formatted = _BEGIN_ + 
                    Assign(Motion.Unpin, Type.Variable) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[1].Value) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_AddSub, out match))
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[2].Value == "+" ? Motion.UpperModify : Motion.LowerModify, Type.Variable) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, match.Groups[3].Value) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_GainRemove, out match))
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[1].Value == "移除" ? Motion.LowerModify : Motion.UpperModify, Type.Variable) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[3].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, match.Groups[2].Value) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_ModifyLabel, out match))
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[1].Value == "获得" ? Motion.UpperModify : Motion.LowerModify, Type.Label) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[2].Value) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_DeclarationOfWar, out match))
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[1].Value == "可以" ? Motion.UpperModify : Motion.LowerModify, Type.Availability) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Availability.DeclarationOfWar) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_ResolutionAvailability, out match)) 
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[1].Value == "可以" ? Motion.UpperModify : Motion.LowerModify, Type.Availability) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Availability.Resolution) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, match.Groups[2].Value) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_AutoCoreAvailability, out match))
            {
                formatted = _BEGIN_ + 
                    Assign(match.Groups[2].Value == "是" ? Motion.UpperModify : Motion.LowerModify, Type.Availability) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.TriggerState, match.Groups[1].Value) + 
                    _CLAUSE_SPLITTER_ +
                    Assign(Availability.AutoGainCore) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_ModifyToOther, out match))
            {
                formatted = _BEGIN_ + 
                    Assign(match.Groups[3].Value == "+" ? Motion.UpperModify : Motion.LowerModify, Type.Variable) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.SufferState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, match.Groups[4].Value) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_GetWarGoal, out match)) 
            {
                formatted = _BEGIN_ + 
                    Assign(Motion.UpperModify, Type.Availability) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.SufferState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Availability.WarGoal) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, match.Groups[2].Value) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Single_ResearchBonus, out match))
            {
                formatted = _BEGIN_ + Assign(Motion.UpperModify, Type.ResearchBonus) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, match.Groups[1].Value) +
                    _END_;
            }
            else
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 格式化复语句
        /// </summary>
        /// <param name="str">原始语句</param>
        /// <param name="formatted">格式化后的语句，默认为string.Empty</param>
        /// <returns>格式化成功返回true，否则返回false。若子语句中有一个或以上的短句无法格式化，同样判定为格式化失败返回false</returns>
        private static bool ComplexPatternFormatter(string str, out string formatted)
        {
            formatted = string.Empty;
            if (GetMatch(str, Pattern_Complex_ModifyLabel, out Match match))
            {
                if (!EffectsFromSubSentence(match.Groups[3].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(match.Groups[1].Value == "获得" ? Motion.UpperModify : Motion.LowerModify, Type.Label) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, effects) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Complex_GainLabelWithinTime, out match))
            {
                if (!EffectsFromSubSentence(match.Groups[3].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(Motion.UpperModify, Type.Label) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Duration, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, effects) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Complex_SomeoneGainLabel, out match))
            {
                if (!EffectsFromSubSentence(match.Groups[4].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(match.Groups[2].Value == "获得" ? Motion.UpperModify : Motion.LowerModify, Type.Label) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.TriggerState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[3].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, effects) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Complex_EveryoneModifyLabel, out match))
            {
                if (!EffectsFromSubSentence(match.Groups[4].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(match.Groups[2].Value == "获得" ? Motion.UpperModify : Motion.LowerModify, Type.Label) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.TriggerState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, effects) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Complex_ModifyGrade, out match))
            {
                if (!EffectsFromSubSentence(match.Groups[3].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(Motion.Modify, Type.Grade) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[1].Value + _PHRASE_SPLITTER_ + match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, effects) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Complex_Append, out match))
            {
                if (!EffectsFromSubSentence(match.Groups[2].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(Motion.Append, Type.Label) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, effects) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Complex_SomeoneTriggerEventWithConsent, out match))
            {
                if (!EffectsFromSubSentence(match.Groups[3].Value, null, out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(Motion.Trigger, Type.RequestEvent) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.TriggerState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, effects) +
                    _END_;
            }
            else if (GetMatch(str, Pattern_Complex_RegionRestrict, out match))
            {
                if (!EffectsFromSubSentence(match.Groups[2].Value, null, out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(Motion.Restrict, Type.Region) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Object, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tag.Data, effects) +
                    _END_;
            }
            else
            { 
                return false; 
            }
            return true;
        }
        /// <summary>
        /// 从复合子句中提取所有效果
        /// </summary>
        /// <param name="subSentence">子句</param>
        /// <param name="splitter">子句分割符号</param>
        /// <param name="effects">提取的所有效果</param>
        /// <returns>全部短句格式化成功返回true，有一个或以上失败则返回false</returns>
        private static bool EffectsFromSubSentence(string subSentence, string? splitter, out string effects)
        {
            effects = "";
            var clauses = splitter == null ? new string[] { subSentence } : subSentence.Split(splitter);
            if (clauses == null || clauses.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < clauses.Length; i++)
            {
                if (!SinglePatternFormatter(clauses[i], out string formatted) && 
                    !ComplexPatternFormatter(clauses[i], out formatted))
                {
                    return false;
                }
                if (i > 0)
                {
                    effects += _PHRASE_SPLITTER_;
                }
                effects += formatted;
            }
            return true;
        }
        #endregion
    }
}
