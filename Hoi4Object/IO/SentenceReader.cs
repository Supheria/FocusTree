using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Hoi4Object.IO
{
    internal class SentenceReader
    {
        /// <summary>
        /// 单语句-触发事件
        /// 触发事件“骑着青牛的老者？”。
        /// </summary>
        static string Pattern_Single_TriggerEvent = @"^(?:触发事件“)([\u4e00-\u9fa5？]+)";
        /// <summary>
        /// 单语句-其他国家触发事件
        /// 神灵庙触发事件“道教该如何面对道家？”
        /// </summary>
        static string Pattern_Single_OtherTriggerEvent = @"^([\u4e00-\u9fa5]+)(?:触发事件“)([\u4e00-\u9fa5？]+)";
        /// <summary>
        /// 单语句-固定值或类型
        /// 将平均灵力值固定为50%
        /// 将世界观固定为唯心世界观
        /// </summary>
        static string Pattern_Single_Fix = @"^(?:将)([\u4e00-\u9fa5]+)(?:固定为)(.+)";
        /// <summary>
        /// 单语句-移除固定
        /// 移除对势力规模的固定
        /// </summary>
        static string Pattern_Single_RemoveFixation = @"^(?:移除对)([\u4e00-\u9fa5]+)(?:的固定)";
        /// <summary>
        /// 单语句-加|减值
        /// x可以加入阵营灵力系科研速度：+35%
        /// 稳定度：-30%
        /// </summary>
        static string Pattern_Single_AddSub = @"^([\u4e00-\u9fa5x]+)(?:：)([+|-])(\d+%?)";
        /// <summary>
        /// 单语句-获得|增加|移除个数
        /// 获得1个科研槽
        /// 增加10个建筑位
        /// 移除1个民用工厂
        /// </summary>
        static string Pattern_Single_GainRemove = @"^(获得|增加|移除)(\d+)(?:个)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 单语句-获得|移除标签
        /// 获得五弊三缺
        /// 移除道教的探索精神
        /// </summary>
        static string Pattern_Single_ChangeLabel = @"^(获得[^对]|移除[^对])([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 单语句-宣战可用性
        /// 规则修改：不能宣战
        /// 规则修改：可以宣战
        /// </summary>
        static string Pattern_Single_DeclarationAvailability = @"^(?:规则修改：)(不能|可以)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 单语句-决议可用性
        /// 可以通过决议发动周边国家的内战使其变为附庸
        /// </summary>
        static string Pattern_Single_ResolutionAvailability = @"^(可以|不能)(通过决议)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 单语句-自动获得核心可用性
        /// 幽灵种族的省份将自动获得核心：是
        /// </summary>
        static string Pattern_Single_AutoCoreAvailability = @"^([\u4e00-\u9fa5]+)(?:的省份将自动获得核心：)([是|否])";
        /// <summary>
        /// 单语句-对他国的修正
        /// （天狗共和国）我国对其进攻修正：+20%
        /// </summary>
        static string Pattern_Single_ModifyToOther = @"^(?:（)([\u4e00-\u9fa5]+)(?:）我国对其)(进攻|防御)(?:修正：)([+|-])(\d+%?)";
        /// <summary>
        /// 单语句-获得对别国的战争目标
        /// 获得对守矢神社的吞并战争目标
        /// </summary>
        static string Pattern_Single_GetWarGoal = @"^(?:获得对)([\u4e00-\u9fa5]+)(?:的)([\u4e00-\u9fa5]+)(?:战争目标)";
        /// <summary>
        /// 单语句-研究加成
        /// 3x60%研究加成：基础武器。
        /// </summary>
        static string Pattern_Single_ResearchBonus = @"^(\dx\d+%?)(?:研究加成：)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 复语句-获得|移除标签
        /// 获得无为而治，其效果为（每周人口：+1，每月人口：+15%）
        /// 移除圣人，其效果为（孤立倾向+0.1，每日唯心度变化+0.04%）
        /// </summary>
        static string Pattern_Complex_ChangeLabel = @"^(获得|移除)([\u4e00-\u9fa5]+)(?:，其效果为（)([^）]+)";
        /// <summary>
        /// 复语句-获得限时效果
        /// 获得为期365天的“强大的威望”，其效果为（每日获得的政治点数：+0.25，部队组织度：+15%，稳定度：+20%，战争支持度：+20%，建造速度：+10%，部队攻击：+5%，部队防御：+5%，科研速度：+10%，工厂产出：+10%）
        /// </summary>
        static string Pattern_Complex_GainLabelWithinTime = @"^(?:获得为期)([0-9天月年]+)(?:的“)([\u4e00-\u9fa5]+)(?:”，其效果为（)([^）]+)";
        /// <summary>
        /// 复语句-某国获得标签
        /// （隐居村落）获得自古以来（对命莲寺关系：+200）
        /// </summary>
        static string Pattern_Complex_SomeoneGainLabel = @"^(?:（)([\u4e00-\u9fa5]+)(?:）)(获得|失去)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
        /// <summary>
        /// 复语句-每个国家获得标签
        /// 每个国家获得：道德观不同（对隐世村落关系：-20）
        /// 每个国家失去：道德观不同（对隐世村落关系：-20）
        /// </summary>
        static string Pattern_Complex_EveryoneChangeLabel = @"^(每个国家)(获得|失去)(?:：)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
        /// <summary>
        /// 复语句-替换等级
        /// 向保守社会发展一级，以保守社会5取代保守社会4，效果变化（稳定度：+6% 科研速度：-3% 加密：+0.5 可出口资源：-5% 意识形态变化抵制力度：+10%）
        /// </summary>
        static string Pattern_Complex_ChangeGrade = @"^(?:向)([\u4e00-\u9fa5]+)(?:发展一级，以[\u4e00-\u9fa5]+)(\d+)(?:取代[\u4e00-\u9fa5]+)(\d+)(?:，效果变化（)([^）]+)";
        /// <summary>
        /// 复语句-追加效果
        /// 天地不仁追加效果：稳定度：-5%，适役人口：+2%
        /// </summary>
        static string Pattern_Complex_Append = @"^([\u4e00-\u9fa5]+)(?:追加效果：)(.+)$";
        /// <summary>
        /// 复语句-某国触发可同意事件
        /// 每个孤立国家触发事件“隐世村落的无偿教导？”。如果他们同意，则获得强烈的战略结盟（对隐居村落的关系：+100）
        /// </summary>
        static string Pattern_Complex_SomeoneTriggerEventWithConsent = @"^([\u4e00-\u9fa5]+)(?:触发事件“)([\u4e00-\u9fa5？]+)(?:”。如果他们同意，则获得)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
        /// <summary>
        /// 复语句-区域限制
        /// 村民生活区（增加10个建筑位，增加10个民用工厂）
        /// </summary>
        static string Pattern_Complex_RegionRestrict = @"^([\u4e00-\u9fa5]+)(?:（)([^）]+)";


        /// <summary>
        /// 收集无法格式化的语句
        /// </summary>
        public static List<string> Unformattable { get; private set; } = new List<string>();
        /// <summary>
        /// 全句起始
        /// </summary>
        static string _BEGIN_ = "{";
        /// <summary>
        /// 全句终止
        /// </summary>
        static string _END_ = "}";
        /// <summary>
        /// 分割子句
        /// </summary>
        static string _CLAUSE_SPLITTER_ = ",";
        /// <summary>
        /// 子句内分割
        /// </summary>
        static string _PHRASE_SPLITTER_ = "|";
        static string _ASSIGNMENT_ = "=";
        enum Motions
        {
            /// <summary>
            /// 触发事件
            /// </summary>
            TriggerEvent,
            /// <summary>
            /// 固定变量的值或类型
            /// </summary>
            Fix,
            /// <summary>
            /// 移除变量的固定
            /// </summary>
            RemoveFixation,
            /// <summary>
            /// 增加变量的值
            /// </summary>
            AddValue,
            /// <summary>
            /// 减少变量的值
            /// </summary>
            SubValue,
            /// <summary>
            /// 获得标签
            /// </summary>
            GainLabel,
            /// <summary>
            /// 移除标签
            /// </summary>
            RemoveLabel,
            /// <summary>
            /// 追加标签
            /// </summary>
            AppendLabel,
            /// <summary>
            /// 可以宣战
            /// </summary>
            DeclarationIsAble,
            /// <summary>
            /// 不能宣战
            /// </summary>
            DeclarationIsUnable,
            /// <summary>
            /// 可以通过决议
            /// </summary>
            ResolutionIsAble,
            /// <summary>
            /// 不能通过决议
            /// </summary>
            ResolutionIsUnable,
            /// <summary>
            /// 省份可以自动获得核心
            /// </summary>
            AutoCoreIsAble,
            /// <summary>
            /// 省份不能自动获得核心
            /// </summary>
            AutoCoreIsUnable,
            /// <summary>
            /// 对他国的修正
            /// </summary>
            ModifyToOther,
            /// <summary>
            /// 获得战争目标
            /// </summary>
            GainWarGoal,
            /// <summary>
            /// 研究加成
            /// </summary>
            ResearchBonus,
            /// <summary>
            /// 替换等级
            /// </summary>
            ChangeGrade
        };
        enum Tags
        {
            /// <summary>
            /// 触发者
            /// </summary>
            TriggerState,
            /// <summary>
            /// 受施者
            /// </summary>
            TargetState,
            /// <summary>
            /// 触发事件
            /// </summary>
            Event,
            /// <summary>
            /// 执行对象
            /// </summary>
            Variable,
            /// <summary>
            /// 执行值
            /// </summary>
            Value,
            /// <summary>
            /// 获得|移除的效果名
            /// </summary>
            Label,
            /// <summary>
            /// 有效时长
            /// </summary>
            Duration,
            /// <summary>
            /// 决议
            /// </summary>
            Resolution,
            /// <summary>
            /// 战争目标
            /// </summary>
            WarGoal,
            /// <summary>
            /// 附带效果
            /// </summary>
            Effects,
            /// <summary>
            /// 等级修改
            /// GradeModification=前等级|修改后等级
            /// </summary>
            GradeModification
        }

        private static string Assign(params Motions[] motions)
        {
            string result = "Motion" + _ASSIGNMENT_;
            for (int i = 0; i < motions.Length; i++)
            {
                if (i > 0)
                {
                    result += _PHRASE_SPLITTER_;
                }
                result += Enum.GetName(typeof(Motions), motions[i]);
            }
            return result;
        }
        private static string Assign(Tags nameTag, string name)
        {
            return Enum.GetName(typeof(Tags), nameTag) + _ASSIGNMENT_ + name;
        }

        /// <summary>
        /// 格式化原始的效果语句
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns>返回格式化后的语句，如果无法格式化则返回null，并记入Unformattable</returns>
        public static string? RawSentenceFormatter(string sentence)
        {
            //var result = SinglePatternFormatter(sentence);
            if (SinglePatternFormatter(sentence, out string formatted) ) 
            { 
                return formatted; 
            }
            if (ComplexPatternFormatter(sentence, out formatted))
            {
                return formatted;
            }
            Unformattable.Add(sentence);
            return null;
        }
        /// <summary>
        /// 格式化单语句
        /// </summary>
        /// <param name="str"></param>
        /// <returns>返回格式化后的单语句，如果无匹配的格式化模式则返回null</returns>
        private static bool SinglePatternFormatter(string str, out string formatted)
        {
            formatted = string.Empty;
            var matches = SingleMatches(str);
            int Case = -1;
            for (int i = 0; i < matches.Length; i++)
            {
                if (matches[i].Success)
                {
                    Case = i;
                    break;
                }
            }
            if (Case == -1)
            {
                return false;
            }
            var match = matches[Case];
            if (Case == 0)
            {
                formatted = _BEGIN_ +
                    Assign(Motions.TriggerEvent) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Event, match.Groups[1].Value) +
                    _END_;
            }
            else if (Case == 1)
            {
                formatted = _BEGIN_ +
                    Assign(Motions.TriggerEvent) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.TriggerState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Event, match.Groups[2].Value) +
                    _END_;
            }
            else if (Case == 2)
            {
                formatted = _BEGIN_ +
                    Assign(Motions.Fix) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Variable, match.Groups[1].Value) +
                    Assign(Tags.Value, match.Groups[2].Value) +
                    _END_;
            }
            else if (Case == 3)
            {
                formatted = _BEGIN_ +
                    Assign(Motions.RemoveFixation) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Variable, match.Groups[1].Value) +
                    _END_;
            }
            else if (Case == 4)
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[2].Value == "+" ? Motions.AddValue : Motions.SubValue) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Variable, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Value, match.Groups[3].Value) +
                    _END_;
            }
            else if (Case == 5)
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[1].Value == "移除" ? Motions.SubValue : Motions.AddValue) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Variable, match.Groups[3].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Value, match.Groups[2].Value) +
                    _END_;
            }
            else if (Case == 6)
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[1].Value == "获得" ? Motions.GainLabel : Motions.RemoveLabel) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Label, match.Groups[2].Value) +
                    _END_;
            }
            else if (Case == 7)
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[1].Value == "可以" ? Motions.DeclarationIsAble : Motions.DeclarationIsUnable) +
                    _END_;
            }
            else if (Case == 8) 
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[1].Value == "可以" ? Motions.ResolutionIsAble : Motions.ResolutionIsUnable) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Resolution, match.Groups[2].Value) +
                    _END_;
            }
            else if (Case == 9)
            {
                formatted = _BEGIN_ +
                    Assign(match.Groups[2].Value == "是" ? Motions.AutoCoreIsAble : Motions.AutoCoreIsUnable) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.TriggerState, match.Groups[1].Value) + 
                    _END_;
            }
            else if (Case == 10)
            {
                formatted = _BEGIN_ +
                    Assign(Motions.ModifyToOther, match.Groups[3].Value == "+" ? Motions.AddValue : Motions.SubValue) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.TargetState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Variable, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Value, match.Groups[4].Value) +
                    _END_;
            }
            else if (Case == 11) 
            {
                formatted = _BEGIN_ +
                    Assign(Motions.GainWarGoal) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.TargetState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.WarGoal, match.Groups[2].Value) +
                    _END_;
            }
            else if (Case == 12)
            {
                formatted = _BEGIN_ +
                    Assign(Motions.ResearchBonus) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Variable, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Value, match.Groups[1].Value) +
                    _END_;
            }
            return true;
        }
        private static Match[] SingleMatches(string str)
        {
            return Regex.Matches(str, /*0*/Pattern_Single_TriggerEvent).
                Union(Regex.Matches(str, /*1*/Pattern_Single_OtherTriggerEvent)).
                Union(Regex.Matches(str, /*2*/Pattern_Single_Fix)).
                Union(Regex.Matches(str, /*3*/Pattern_Single_RemoveFixation)).
                Union(Regex.Matches(str, /*4*/Pattern_Single_AddSub)).
                Union(Regex.Matches(str, /*5*/Pattern_Single_GainRemove)).
                Union(Regex.Matches(str, /*6*/Pattern_Complex_ChangeLabel)).
                Union(Regex.Matches(str, /*7*/Pattern_Single_DeclarationAvailability)).
                Union(Regex.Matches(str, /*8*/Pattern_Single_ResolutionAvailability)).
                Union(Regex.Matches(str, /*9*/Pattern_Single_AutoCoreAvailability)).
                Union(Regex.Matches(str, /*10*/Pattern_Single_ModifyToOther)).
                Union(Regex.Matches(str, /*11*/Pattern_Single_GetWarGoal)).
                Union(Regex.Matches(str, /*12*/Pattern_Single_ResearchBonus)).
                ToArray();
        }
        
        private static bool ComplexPatternFormatter(string str, out string formatted)
        {
            formatted = string.Empty;
            var matches = ComplexMatches(str);
            int Case = -1;
            for (int i = 0; i < matches.Length; i++)
            {
                if (matches[i].Success)
                {
                    Case = i;
                    break;
                }
            }
            if (Case == -1)
            {
                return false;
            }
            var match = matches[Case];
            if (Case == 0)
            {
                if (!EffectsFromSubSentence(match.Groups[2].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(Motions.GainLabel) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Label, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Effects, effects) +
                    _END_;
            }
            else if (Case == 1)
            {
                if (!EffectsFromSubSentence(match.Groups[3].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(Motions.GainLabel) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Label, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Duration, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Effects, effects) +
                    _END_;
            }
            else if (Case == 2)
            {
                if (!EffectsFromSubSentence(match.Groups[3].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(match.Groups[2].Value == "获得" ? Motions.GainLabel : Motions.RemoveLabel) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.TriggerState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Label, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Effects, effects) +
                    _END_;
            }
            else if (Case == 3)
            {
                if (!EffectsFromSubSentence(match.Groups[4].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(match.Groups[2].Value == "获得" ? Motions.GainLabel : Motions.RemoveLabel) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.TriggerState, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Label, match.Groups[2].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Effects, effects) +
                    _END_;
            }
            else if (Case == 4)
            {
                if (!EffectsFromSubSentence(match.Groups[4].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(Motions.ChangeGrade) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.Variable, match.Groups[1].Value) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.GradeModification, match.Groups[2].Value + _PHRASE_SPLITTER_ + match.Groups[3].Value) +
                    _END_;
            }
            else if (Case == 5)
            {
                if (!EffectsFromSubSentence(match.Groups[2].Value, "，", out string effects))
                {
                    return false;
                }
                formatted = _BEGIN_ +
                    Assign(Motions.AppendLabel) +
                    _CLAUSE_SPLITTER_ +
                    Assign(Tags.)
            }
            return true;
        }
        private static Match[] ComplexMatches(string str)
        {
            return Regex.Matches(str, /*0*/Pattern_Complex_ChangeLabel).
                Union(Regex.Matches(str, /*1*/Pattern_Complex_GainLabelWithinTime)).
                Union(Regex.Matches(str, /*2*/Pattern_Complex_SomeoneGainLabel)).
                Union(Regex.Matches(str, /*3*/Pattern_Complex_EveryoneChangeLabel)).
                Union(Regex.Matches(str, /*4*/Pattern_Complex_ChangeGrade)).
                Union(Regex.Matches(str, /*5*/Pattern_Complex_Append)).
                Union(Regex.Matches(str, /*6*/Pattern_Complex_SomeoneTriggerEventWithConsent)).
                Union(Regex.Matches(str, /*7*/Pattern_Complex_RegionRestrict)).
                ToArray();
        }
        private static bool EffectsFromSubSentence(string subSentence, string splitter, out string effects)
        {
            effects = "";
            var clauses = subSentence.Split(splitter);
            if (clauses == null || clauses.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < clauses.Length; i++)
            {
                if (!SinglePatternFormatter(clauses[i], out string formatted))
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
    }
}
