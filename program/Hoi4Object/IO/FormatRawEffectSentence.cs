//using System;
//using System.IO;
//using System.Text.RegularExpressions;

//namespace Hoi4Object.IO
//{
//    /// <summary>
//    /// 格式化原始效果语句
//    /// </summary>
//    public class FormatRawEffectSentence
//    {
//        #region ==== 基本变量 ====

//        /// <summary>
//        /// 收集无法格式化的语句
//        /// </summary>
//        public static List<string> Unformattable { get; private set; } = new List<string>();

//        #endregion


//        #region ==== 正则模式 ====

//        /// <summary>
//        /// 单语句-触发事件
//        /// 触发事件“骑着青牛的老者？”。
//        /// </summary>
//        static string Pattern_Single_TriggerEvent = "^(?:触发事件“)([\u4e00-\u9fa5？]+)";
//        /// <summary>
//        /// 单语句-其他国家触发事件
//        /// 神灵庙触发事件“道教该如何面对道家？”
//        /// </summary>
//        static string Pattern_Single_OtherTriggerEvent = "^([\u4e00-\u9fa5]+)(?:触发事件“)([\u4e00-\u9fa5？]+)(?:”)$";
//        /// <summary>
//        /// 单语句-固定值或类型
//        /// 将平均灵力值固定为50%
//        /// 将世界观固定为唯心世界观
//        /// </summary>
//        static string Pattern_Single_FixValue = "^(?:将)([\u4e00-\u9fa5]+)(?:固定为)(.+)";
//        /// <summary>
//        /// 单语句-移除固定
//        /// 移除对势力规模的固定
//        /// </summary>
//        static string Pattern_Single_RemoveFixation = "^(?:移除对)([\u4e00-\u9fa5]+)(?:的固定)";
//        /// <summary>
//        /// 单语句-加|减值
//        /// x可以加入阵营灵力系科研速度：+35%
//        /// 稳定度：-30%
//        /// </summary>
//        static string Pattern_Single_AddSub = "^([\u4e00-\u9fa5x]+)(?:：)([+|-])(\\d+%?)";
//        /// <summary>
//        /// 单语句-获得|增加|移除个数
//        /// 获得1个科研槽
//        /// 增加10个建筑位
//        /// 移除1个民用工厂
//        /// </summary>
//        static string Pattern_Single_GainRemove = "^(获得|增加|移除)(\\d+)(?:个)([\u4e00-\u9fa5]+)";
//        /// <summary>
//        /// 单语句-获得|移除标签
//        /// 获得五弊三缺
//        /// 移除道教的探索精神
//        /// </summary>
//        static string Pattern_Single_ModifyLabel = "^(获得|移除)([^对][\u4e00-\u9fa5]+)$";
//        /// <summary>
//        /// 单语句-宣战可用性
//        /// 规则修改：不能宣战
//        /// 规则修改：可以宣战
//        /// </summary>
//        static string Pattern_Single_DeclarationOfWar = "^(?:规则修改：)(不能|可以)([\u4e00-\u9fa5]+)";
//        /// <summary>
//        /// 单语句-决议可用性
//        /// 可以通过决议发动周边国家的内战使其变为附庸
//        /// </summary>
//        static string Pattern_Single_ResolutionAvailability = "^(可以|不能)(?:通过决议)([\u4e00-\u9fa5]+)";
//        /// <summary>
//        /// 单语句-自动获得核心可用性
//        /// 幽灵种族的省份将自动获得核心：是
//        /// </summary>
//        static string Pattern_Single_AutoCoreAvailability = "^([\u4e00-\u9fa5]+)(?:的省份将自动获得核心：)([是|否])";
//        /// <summary>
//        /// 单语句-对他国的修正
//        /// （天狗共和国）我国对其进攻修正：+20%
//        /// </summary>
//        static string Pattern_Single_ModifyToOther = "^(?:（)([\u4e00-\u9fa5]+)(?:）我国对其)(进攻|防御)(?:修正：)([+-])(\\d+%?)";
//        /// <summary>
//        /// 单语句-获得对别国的战争目标
//        /// 获得对守矢神社的吞并战争目标
//        /// </summary>
//        static string Pattern_Single_GetWarGoal = "^(?:获得对)([\u4e00-\u9fa5]+)(?:的)([\u4e00-\u9fa5]+)(?:战争目标)";
//        /// <summary>
//        /// 单语句-研究加成
//        /// 3x60%研究加成：基础武器。
//        /// </summary>
//        static string Pattern_Single_ResearchBonus = "^(\\dx\\d+%?)(?:研究加成：)([\u4e00-\u9fa5]+)";
//        /// <summary>
//        /// 复语句-获得|移除标签
//        /// 获得无为而治，其效果为（每周人口：+1，每月人口：+15%）
//        /// 移除圣人，其效果为（孤立倾向：+0.1，每日唯心度变化：+0.04%）
//        /// </summary>
//        static string Pattern_Complex_ModifyLabel = "^(获得|移除)([\u4e00-\u9fa5]+)(?:，其效果为（)([^）]+)";
//        /// <summary>
//        /// ......获得强烈的战略结盟（对隐居村落的关系：+100）
//        /// </summary>
//        static string Pattern_Complex_ModifyLabel2 = "^(获得|移除)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
//        /// <summary>
//        /// 复语句-获得限时效果
//        /// 获得为期365天的“强大的威望”，其效果为（每日获得的政治点数：+0.25，部队组织度：+15%，稳定度：+20%，战争支持度：+20%，建造速度：+10%，部队攻击：+5%，部队防御：+5%，科研速度：+10%，工厂产出：+10%）
//        /// </summary>
//        static string Pattern_Complex_GainLabelWithinTime = "^(?:获得为期)(\\d+[天月年])(?:的“)([\u4e00-\u9fa5]+)(?:”，其效果为（)([^）]+)";
//        /// <summary>
//        /// 复语句-某国获得标签
//        /// （隐居村落）获得自古以来（对命莲寺关系：+200）
//        /// </summary>
//        static string Pattern_Complex_SomeoneGainLabel = "^(?:（)([\u4e00-\u9fa5]+)(?:）)(获得|失去)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
//        /// <summary>
//        /// 复语句-每个国家获得标签
//        /// 每个国家获得：道德观不同（对隐世村落关系：-20）
//        /// 每个国家失去：道德观不同（对隐世村落关系：-20）
//        /// </summary>
//        static string Pattern_Complex_EveryoneModifyLabel = "^(每个国家)(获得|失去)(?:：)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
//        /// <summary>
//        /// 复语句-替换等级
//        /// 向保守社会发展一级，以保守社会5取代保守社会4，效果变化（稳定度：+6% 科研速度：-3% 加密：+0.5 可出口资源：-5% 意识形态变化抵制力度：+10%）
//        /// </summary>
//        static string Pattern_Complex_ModifyGrade = "^(?:向)(?:[\u4e00-\u9fa5]+)(?:发展一级，以)([\u4e00-\u9fa5]+\\d)(?:取代)([\u4e00-\u9fa5]+\\d)(?:，效果变化（)([^）]+)";
//        /// <summary>
//        /// 复语句-追加效果
//        /// 天地不仁追加效果：稳定度：-5%，适役人口：+2%
//        /// </summary>
//        static string Pattern_Complex_Append = "^([\u4e00-\u9fa5]+)(?:追加效果：)(.+)$";
//        /// <summary>
//        /// 复语句-某国触发可同意事件
//        /// 每个孤立国家触发事件“隐世村落的无偿教导？”。如果他们同意，则获得强烈的战略结盟（对隐居村落的关系：+100）
//        /// </summary>
//        static string Pattern_Complex_SomeoneTriggerRequestEvent = "^([\u4e00-\u9fa5]+)(?:触发事件“)([\u4e00-\u9fa5？]+)(?:”。如果他们同意，则)(.+)";
//        /// <summary>
//        /// 复语句-区域限制
//        /// 村民生活区（增加10个建筑位，增加10个民用工厂）
//        /// </summary>
//        static string Pattern_Complex_RegionRestrict = "^([^获|移][^得|除][\u4e00-\u9fa5]+)(?:（)([^）]+)";

//        #endregion


//        #region ==== 主方法 ====

//        /// <summary>
//        /// 格式化器
//        /// </summary>
//        /// <param name="sentence">原始效果语句</param>
//        /// <param name="formatted">格式化后的语句，默认值为 sentence</param>
//        /// <returns>如果格式化成功则返回true，否则返回false并记入Unformattable</returns>
//        public static bool Formatter(string sentence, out Sentence? formatted)
//        {
//            if (SinglePatternFormatter(sentence, out formatted))
//            {
//                return true;
//            }
//            if (ComplexPatternFormatter(sentence, out formatted))
//            {
//                return true;
//            }
//            Unformattable.Add(sentence);
//            return false;
//        }

//        #endregion


//        #region ==== 格式化方法 ====

//        private static bool GetMatch(string input, string pattern, out Match match)
//        {
//            match = Regex.Match(input, pattern);
//            return match.Success;
//        }
//        /// <summary>
//        /// 格式化单句
//        /// </summary>
//        /// <param name="str">原始语句</param>
//        /// <param name="formatted">格式化后的语句，默认值为 str</param>
//        /// <returns>返回格式化后的单语句，如果无匹配的格式化模式则返回null</returns>
//        private static bool SinglePatternFormatter(string str, out Sentence? formatted)
//        {
//            formatted = null;
//            if (GetMatch(str, Pattern_Single_TriggerEvent, out Match match))
//            {
//                formatted = new(
//                    PublicSign.Motions.Trigger,
//                    PublicSign.Types.Event,
//                    match.Groups[1].Value,
//                    null,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_OtherTriggerEvent, out match))
//            {
//                formatted = new(
//                    PublicSign.Motions.Trigger, 
//                    PublicSign.Types.Event,
//                    match.Groups[2].Value, 
//                    null,
//                    match.Groups[1].Value,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_FixValue, out match))
//            {
//                formatted = new(
//                    PublicSign.Motions.Fixed,
//                    PublicSign.Types.Variable, 
//                    match.Groups[1].Value, 
//                    match.Groups[2].Value,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_RemoveFixation, out match))
//            {
//                formatted = new(
//                    PublicSign.Motions.Unpin, 
//                    PublicSign.Types.Variable,
//                    match.Groups[1].Value,
//                    null,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_AddSub, out match))
//            {
//                formatted = new(
//                    match.Groups[2].Value == "+" ? PublicSign.Motions.UpperModify : PublicSign.Motions.LowerModify,
//                    PublicSign.Types.Variable, 
//                    match.Groups[1].Value, 
//                    match.Groups[3].Value,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_GainRemove, out match))
//            {
//                formatted = new(
//                    match.Groups[1].Value == "移除" ? PublicSign.Motions.LowerModify : PublicSign.Motions.UpperModify,
//                    PublicSign.Types.Variable, 
//                    match.Groups[3].Value, 
//                    match.Groups[2].Value,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_ModifyLabel, out match))
//            {
//                formatted = new(
//                    match.Groups[1].Value == "获得" ? PublicSign.Motions.UpperModify : PublicSign.Motions.LowerModify,
//                    PublicSign.Types.Label, 
//                    match.Groups[2].Value,
//                    null,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_DeclarationOfWar, out match))
//            {
//                formatted = new(
//                    match.Groups[1].Value == "可以" ? PublicSign.Motions.UpperModify : PublicSign.Motions.LowerModify,
//                    PublicSign.AvailabilityObjects.DeclarationOfWar,
//                    null,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_ResolutionAvailability, out match))
//            {
//                formatted = new(
//                    match.Groups[1].Value == "可以" ? PublicSign.Motions.UpperModify : PublicSign.Motions.LowerModify,
//                    PublicSign.AvailabilityObjects.Resolution,
//                    match.Groups[2].Value,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_AutoCoreAvailability, out match))
//            {
//                formatted = new(
//                    match.Groups[2].Value == "是" ? PublicSign.Motions.UpperModify : PublicSign.Motions.LowerModify,
//                    PublicSign.AvailabilityObjects.AutoGainCore,
//                    null,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_ModifyToOther, out match))
//            {
//                formatted = new(
//                    match.Groups[3].Value == "+" ? PublicSign.Motions.UpperModify : PublicSign.Motions.LowerModify,
//                    PublicSign.Types.Variable,
//                    match.Groups[2].Value,
//                    match.Groups[4].Value,
//                    null,
//                    match.Groups[1].Value,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_GetWarGoal, out match))
//            {
//                formatted = new(
//                    PublicSign.Motions.UpperModify,
//                    PublicSign.AvailabilityObjects.WarGoal,
//                    match.Groups[2].Value,
//                    null,
//                    match.Groups[1].Value,
//                    null
//                    );
//            }
//            else if (GetMatch(str, Pattern_Single_ResearchBonus, out match))
//            {
//                formatted = new(
//                    PublicSign.Motions.UpperModify,
//                    PublicSign.Types.ResearchBonus,
//                    match.Groups[2].Value,
//                    match.Groups[1].Value,
//                    null,
//                    null,
//                    null
//                    );
//            }
//            else
//            {
//                return false;
//            }
//            return true;
//        }
//        /// <summary>
//        /// 格式化复语句
//        /// </summary>
//        /// <param name="str">原始语句</param>
//        /// <param name="formatted">格式化后的语句，默认值为 str</param>
//        /// <returns>格式化成功返回true，否则返回false。若子语句中有一个或以上的短句无法格式化，同样判定为格式化失败返回false</returns>
//        private static bool ComplexPatternFormatter(string str, out Sentence? formatted)
//        {
//            formatted = null;
//            if (GetMatch(str, Pattern_Complex_ModifyLabel, out Match match) ||
//                GetMatch(str, Pattern_Complex_ModifyLabel2, out match))
//            {
//                if (!GetSubSentence(match.Groups[3].Value, "，", out List<Sentence> subSentences))
//                {
//                    return false;
//                }
//                formatted = new(
//                    match.Groups[1].Value == "获得" ? PublicSign.Motions.UpperModify : PublicSign.Motions.LowerModify,
//                    PublicSign.Types.Label,
//                    match.Groups[2].Value,
//                    null,
//                    null,
//                    null,
//                    subSentences
//                    );
//            }
//            else if (GetMatch(str, Pattern_Complex_GainLabelWithinTime, out match))
//            {
//                if (!GetSubSentence(match.Groups[3].Value, "，", out List<Sentence> subSentences))
//                {
//                    return false;
//                }
//                formatted = new(
//                    PublicSign.Motions.UpperModify,
//                    PublicSign.Types.Label,
//                    match.Groups[2].Value,
//                    match.Groups[1].Value,
//                    null,
//                    null,
//                    subSentences
//                    );
//            }
//            else if (GetMatch(str, Pattern_Complex_SomeoneGainLabel, out match))
//            {
//                if (!GetSubSentence(match.Groups[4].Value, "，", out List<Sentence> subSentences))
//                {
//                    return false;
//                }
//                formatted = new(
//                    match.Groups[2].Value == "获得" ? PublicSign.Motions.UpperModify : PublicSign.Motions.LowerModify,
//                    PublicSign.Types.Label,
//                    match.Groups[3].Value,
//                    null,
//                    match.Groups[1].Value,
//                    null,
//                    subSentences
//                    );
//            }
//            else if (GetMatch(str, Pattern_Complex_EveryoneModifyLabel, out match))
//            {
//                if (!GetSubSentence(match.Groups[4].Value, "，", out List<Sentence> subSentences))
//                {
//                    return false;
//                }
//                formatted = new(
//                    match.Groups[2].Value == "获得" ? PublicSign.Motions.UpperModify : PublicSign.Motions.LowerModify,
//                    PublicSign.Types.Label,
//                    match.Groups[3].Value,
//                    null,
//                    match.Groups[1].Value,
//                    null,
//                    subSentences
//                    );
//            }
//            else if (GetMatch(str, Pattern_Complex_ModifyGrade, out match))
//            {
//                if (!GetSubSentence(match.Groups[3].Value, " ", out List<Sentence> subSentences))
//                {
//                    return false;
//                }
//                formatted = new(
//                    PublicSign.Motions.Modify,
//                    PublicSign.Types.Grade,
//                    match.Groups[1].Value + PublicSign.ReplaceTo + match.Groups[2].Value,
//                    null,
//                    null,
//                    null,
//                    subSentences
//                    );
//            }
//            else if (GetMatch(str, Pattern_Complex_Append, out match))
//            {
//                if (!GetSubSentence(match.Groups[2].Value, "，", out List<Sentence> subSentences))
//                {
//                    return false;
//                }
//                formatted = new(
//                    PublicSign.Motions.Append,
//                    PublicSign.Types.Label,
//                    match.Groups[1].Value,
//                    null,
//                    null,
//                    null,
//                    subSentences
//                    );
//            }
//            else if (GetMatch(str, Pattern_Complex_SomeoneTriggerRequestEvent, out match))
//            {
//                if (!GetSubSentence(match.Groups[3].Value, null, out List<Sentence> subSentences))
//                {
//                    return false;
//                }
//                formatted = new(
//                    PublicSign.Motions.Trigger,
//                    PublicSign.Types.RequestEvent,
//                    match.Groups[2].Value,
//                    null,
//                    match.Groups[1].Value,
//                    null,
//                    subSentences
//                    );
//            }
//            else if (GetMatch(str, Pattern_Complex_RegionRestrict, out match))
//            {
//                if (!GetSubSentence(match.Groups[2].Value, "，", out List<Sentence> subSentences))
//                {
//                    return false;
//                }
//                formatted = new(
//                    PublicSign.Motions.Restrict,
//                    PublicSign.Types.Region,
//                    match.Groups[1].Value,
//                    null,
//                    null,
//                    null,
//                    subSentences
//                    );
//            }
//            else
//            {
//                return false;
//            }
//            return true;
//        }
//        /// <summary>
//        /// 从复合子句中提取所有效果
//        /// </summary>
//        /// <param name="subSentence">子句</param>
//        /// <param name="splitter">子句分割符号</param>
//        /// <param name="subSentences">提取的所有效果</param>
//        /// <returns>全部短句格式化成功返回true，有一个或以上失败则返回false</returns>
//        private static bool GetSubSentence(string subSentence, string? splitter, out List<Sentence> subSentences)
//        {
//            subSentences = new();
//            var clauses = splitter == null ? new string[] { subSentence } : subSentence.Split(splitter);
//            if (clauses == null || clauses.Length == 0)
//            {
//                return false;
//            }
//            for (int i = 0; i < clauses.Length; i++)
//            {
//                if (!SinglePatternFormatter(clauses[i], out Sentence? formatted) &&
//                    !ComplexPatternFormatter(clauses[i], out formatted))
//                {
//                    return false;
//                }
//                if (formatted != null)
//                {
//                    subSentences.Add(formatted);
//                }
//            }
//            return true;
//        }
//        #endregion
//    }
//}
