using FocusTree.Hoi4Object.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FocusTree.Hoi4Object.IO.Formatter
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

        #region ==== 主方法 ====

        /// <summary>
        /// 格式化器
        /// </summary>
        /// <param name="sentence">原始效果语句</param>
        /// <param name="formatted">格式化后的语句，默认值为 null</param>
        /// <returns>如果格式化成功则返回true，否则返回false并记入Unformattable</returns>
        public static bool Formatter(string sentence, out Sentence formatted)
        {
            if (SinglePatternFormatter(sentence, out formatted))
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

        #region ==== 工具 ====

        private static bool GetMatch(string input, string pattern, out Match match)
        {
            match = Regex.Match(input, pattern);
            return match.Success;
        }

        #endregion

        #region ==== 单语句 ====

        /// <summary>
        /// 格式化单句
        /// </summary>
        /// <param name="str">原始语句</param>
        /// <param name="formatted">格式化后的语句，默认值为 null</param>
        /// <returns>返回格式化后的单语句，如果无匹配的格式化模式则返回null</returns>
        private static bool SinglePatternFormatter(string str, out Sentence formatted)
        {
            formatted = null;

            // 单语句-触发事件
            /// 触发事件“骑着青牛的老者？”。
            if (GetMatch(str, "^触发事件“([\u4e00-\u9fa5？《》]+)”。?$", out Match match))
            {
                formatted = new(PublicSign.Motions.Trigger, PublicSign.Types.Event, match.Groups[1].Value, null, null);
            }
            // 单语句-某国触发事件
            /// 神灵庙触发事件“道教该如何面对道家？”
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)触发事件“([\u4e00-\u9fa5？]+)”。?$", out match))
            {
                formatted = new(PublicSign.Motions.Trigger, PublicSign.Types.Event, match.Groups[2].Value, match.Groups[1].Value, null);
            }
            // 单语句-固定值或类型
            /// 将平均灵力值固定为50%
            /// 将世界观固定为唯心世界观
            else if (GetMatch(str, "^将([\u4e00-\u9fa5]+)固定为([\u4e00-\u9fa5\\d%]+)$", out match))
            {
                var value = match.Groups[1].Value + PublicSign.Splitter + match.Groups[2].Value; // eg.世界观|唯心世界观
                formatted = new(PublicSign.Motions.Fixed, PublicSign.Types.Variable, value, null, null);
            }
            // 单语句-移除固定
            /// 移除对势力规模的固定
            else if (GetMatch(str, "^移除对([\u4e00-\u9fa5]+)的固定$", out match))
            {
                formatted = new(PublicSign.Motions.Unpin, PublicSign.Types.Variable, match.Groups[1].Value, null, null);
            }
            // 单语句-加|减值
            /// 灵力系科研速度：+35%
            /// 稳定度：-30%
            /// 每日获得的政治点数：+0.1
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)：?([+-])([\\d.]+%?)$", out match))
            {
                var motion = match.Groups[2].Value == "+" ? PublicSign.Motions.Add : PublicSign.Motions.Sub;
                var value = match.Groups[1].Value + PublicSign.Splitter + match.Groups[3].Value;
                formatted = new(motion, PublicSign.Types.Variable, value, null, null);
            }
            // 单语句-修正值
            /// 适役人口修正：15%
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)修正：([\\d.]+%?)$", out match))
            {
                var value = match.Groups[1].Value + PublicSign.Splitter + match.Groups[2].Value;
                formatted = new(PublicSign.Motions.Modify, PublicSign.Types.Variable, value, null, null);
            }
            // 单语句-获得|增加|移除个数
            /// 获得1个科研槽
            /// 增加10个建筑位
            /// 移除1个民用工厂
            else if (GetMatch(str, "^(获得|增加|移除)(\\d+)个([\u4e00-\u9fa5]+)$", out match))
            {
                var motion = match.Groups[1].Value == "移除" ? PublicSign.Motions.Sub : PublicSign.Motions.Add;
                var value = match.Groups[3].Value + PublicSign.Splitter + match.Groups[2].Value;
                formatted = new(motion, PublicSign.Types.Variable, value, null, null);
            }
            // 单语句-宣战可用性
            /// 规则修改：不能宣战
            /// 规则修改：可以宣战
            else if (GetMatch(str, "^规则修改：(不能|可以)宣战$", out match))
            {
                var motion = match.Groups[1].Value == "可以" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove;
                formatted = new(motion, PublicSign.Types.AbleToDeclareWar, null, null, null);
            }
            // 单语句-触发并开启决议
            /// 可以通过决议发动周边国家的内战使其变为附庸
            else if (GetMatch(str, "^可以通过决议([\u4e00-\u9fa5]+)$", out match))
            {
                formatted = new(PublicSign.Motions.Trigger, PublicSign.Types.Resolution, match.Groups[2].Value, null, null);
            }
            // 单语句-自动获得核心可用性
            /// 幽灵种族的省份将自动获得核心：是
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)将自动获得核心：(是|否)$", out match))
            {
                var motion = match.Groups[2].Value == "是" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove;
                formatted = new(motion, PublicSign.Types.AbleToGainCoreAuto, null, match.Groups[1].Value, null);
            }
            // 单语句-数值修正
            /// （天狗共和国）我国对其进攻修正：+20%
            else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）我国(对其[\u4e00-\u9fa5]+)修正：([+-])(\\d+%?)$", out match))
            {
                var motion = match.Groups[3].Value == "+" ? PublicSign.Motions.Add : PublicSign.Motions.Sub;
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value + PublicSign.Splitter + match.Groups[4].Value; // eg.对其进攻|天狗共和国|20%
                formatted = new(motion, PublicSign.Types.Variable, value, null, null);
            }
            // 单语句-获得对他国的战争目标
            /// 获得对守矢神社的吞并战争目标
            else if (GetMatch(str, "^获得对([\u4e00-\u9fa5]+)的([\u4e00-\u9fa5]+)战争目标", out match))
            {
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value; // eg.吞并|守矢神社
                formatted = new(PublicSign.Motions.Gain, PublicSign.Types.WarGoal, value, null, null);
            }
            // 单语句-研究加成
            /// 3x60%研究加成：基础武器。
            else if (GetMatch(str, "^(\\dx\\d+%?)研究加成：([\u4e00-\u9fa5]+)。?$", out match))
            {
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value; // eg.基础武器|3x60%
                formatted = new(PublicSign.Motions.Bonus, PublicSign.Types.Reaserch, value, null, null);
            }
            // 单语句-修改规则：可以创建阵营
            /// 可以创建阵营
            /// 获得允许创建阵营
            else if (GetMatch(str, "^可以创建阵营|获得允许创建阵营$", out match))
            {
                formatted = new(PublicSign.Motions.Gain, PublicSign.Types.AbleToCreateCamp, null, null, null);
            }
            // 单语句-暂时无影响
            /// 这项国策目前没有实际影响。但随着世界局势的变化可能会发生改变。
            else if (GetMatch(str, "这项国策目前没有实际影响。但随着世界局势的变化可能会发生改变。", out match))
            {
                formatted = new(PublicSign.Motions.NoneButMayChange, null, null, null, null);
            }
            // 单语句-创建阵营
            /// 获得允许创建阵营，创建阵营：道盟
            else if (GetMatch(str, "^获得允许创建阵营，创建阵营：([\u4e00-\u9fa5]+)。?$", out match))
            {
                formatted = new(PublicSign.Motions.Create, PublicSign.Types.Camp, match.Groups[1].Value, null, null);
            }
            // 单语句-创建指定属性的阵营
            /// （守矢神社）获得允许创建防御性阵营，创建阵营：妖怪山自卫联盟。
            else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）获得允许创建([\u4e00-\u9fa5]+)阵营，创建阵营：([\u4e00-\u9fa5]+)。?$", out match))
            {
                var value = match.Groups[3].Value + PublicSign.Splitter + match.Groups[2].Value; // eg.妖怪山自卫联盟|防御性
                formatted = new(PublicSign.Motions.Create, PublicSign.Types.Camp, value, match.Groups[1].Value, null);
            }
            // 单语句-不允许加入阵营
            /// x可以加入阵营
            else if (GetMatch(str, "^x可以加入阵营$", out match))
            {
                formatted = new(PublicSign.Motions.Remove, PublicSign.Types.AbleToJoinCamp, null, null, null);
            }
            // 单语句-获得|移除标签
            /// 获得五弊三缺
            /// 移除道教的探索精神
            else if (GetMatch(str, "^(获得|移除)([\u4e00-\u9fa5]+)$", out match))
            {
                var motion = match.Groups[1].Value == "获得" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove;
                formatted = new(motion, PublicSign.Types.Label, match.Groups[2].Value, null, null);
            }
            // 单语句-某国获得标签
            ///（神灵庙）获得命莲寺的宣称
            else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）获得([\u4e00-\u9fa5]+)$", out match))
            {
                formatted = new(PublicSign.Motions.Gain, PublicSign.Types.Label, match.Groups[2].Value, match.Groups[1].Value, null);
            }
            else
            {
                return false;
            }
            return true;
        }

        #endregion

        #region ==== 复语句 ====

        /// <summary>
        /// 格式化复语句
        /// </summary>
        /// <param name="str">原始语句</param>
        /// <param name="formatted">格式化后的语句，默认值为 null</param>
        /// <returns>格式化成功返回true，否则返回false。若子语句中有一个或以上的短句无法格式化，同样判定为格式化失败返回false</returns>
        private static bool ComplexPatternFormatter(string str, out Sentence formatted)
        {
            formatted = null;

            // 复语句-获得|移除标签
            /// 获得无为而治，其效果为（每周人口：+1，每月人口：+15%）
            /// 移除圣人，其效果为（孤立倾向：+0.1，每日唯心度变化：+0.04%）
            /// 获得强烈的战略结盟（对隐居村落的关系：+100）
            if (GetMatch(str, "^(获得|移除)([\u4e00-\u9fa5\\d]+)，其效果为（(.+)）$", out Match match) ||
                GetMatch(str, "^(获得|移除)([\u4e00-\u9fa5]+)（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[3].Value, "，", out List<Sentence> subSentences))
                {
                    return false;
                }
                var motion = match.Groups[1].Value == "获得" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove;
                formatted = new(motion, PublicSign.Types.Label, match.Groups[2].Value, null, subSentences);
            }
            // 复语句-获得限时标签
            /// 获得为期365天的“强大的威望”，其效果为（每日获得的政治点数：+0.25，部队组织度：+15%，稳定度：+20%，战争支持度：+20%，建造速度：+10%，部队攻击：+5%，部队防御：+5%，科研速度：+10%，工厂产出：+10%）
            else if (GetMatch(str, "^获得为期(\\d+[天月年])的“([\u4e00-\u9fa5]+)”，其效果为（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[3].Value, "，", out List<Sentence> subSentences))
                {
                    return false;
                }
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value; // eg.强大的威望|365天
                formatted = new(PublicSign.Motions.Gain, PublicSign.Types.Label, value, null, subSentences);
            }
            // 复语句-某国获得|失去标签
            /// （隐居村落）获得自古以来（对命莲寺关系：+200）
            /// （守矢神社）获得万民自化，其效果为（每周稳定度：+0.1%，每周战争支持度：-0.1%，战争支持度：+100%）
            else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）(获得|失去)([\u4e00-\u9fa5]+)，其效果为（(.+)）$", out match) ||
                GetMatch(str, "^（([\u4e00-\u9fa5]+)）(获得|失去)([\u4e00-\u9fa5]+)（(.+)）$", out match)) 
            {
                if (!GetSubSentence(match.Groups[4].Value, "，", out List<Sentence> subSentences))
                {
                    return false;
                }
                var motion = match.Groups[2].Value == "获得" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove;
                formatted = new(motion, PublicSign.Types.Label, match.Groups[3].Value, match.Groups[1].Value, subSentences);
            }
            // 复语句-每个国家获得标签
            /// 每个国家获得：道德观不同（对隐世村落关系：-20）
            else if (GetMatch(str, "^(每个国家)(获得|失去)：([\u4e00-\u9fa5]+)（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[4].Value, "，", out List<Sentence> subSentences))
                {
                    return false;
                }
                var motion = match.Groups[2].Value == "获得" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove;
                formatted = new(motion, PublicSign.Types.Label, match.Groups[3].Value, match.Groups[1].Value, subSentences);
            }
            // 复语句-变更等级
            /// 向保守社会发展一级，以保守社会5取代保守社会4，效果变化（稳定度：+6% 科研速度：-3% 加密：+0.5 可出口资源：-5% 意识形态变化抵制力度：+10%）
            else if (GetMatch(str, "^[\u4e00-\u9fa5]+，以([\u4e00-\u9fa5]+\\d)取代([\u4e00-\u9fa5]+\\d)，效果变化（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[3].Value, " ", out List<Sentence> subSentences))
                {
                    return false;
                }
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value; // eg.保守社会4|保守社会5
                formatted = new(PublicSign.Motions.Replace, PublicSign.Types.Grade, value, null, subSentences);
            }
            // 复语句-追加效果
            /// 天地不仁追加效果：稳定度：-5%，适役人口：+2%
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)追加效果：(.+)$", out match))
            {
                if (!GetSubSentence(match.Groups[2].Value, "，", out List<Sentence> subSentences))
                {
                    return false;
                }
                formatted = new(PublicSign.Motions.Append, PublicSign.Types.Label, match.Groups[1].Value, null, subSentences);
            }
            // 复语句-某国触发可同意事件
            /// 每个孤立国家触发事件“隐世村落的无偿教导？”。如果他们同意，则获得强烈的战略结盟（对隐居村落的关系：+100）
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)触发事件“([\u4e00-\u9fa5？]+)”。如果他们同意，则(.+)$", out match))
            {
                if (!GetSubSentence(match.Groups[3].Value, null, out List<Sentence> subSentences))
                {
                    return false;
                }
                formatted = new(PublicSign.Motions.Trigger, PublicSign.Types.RequestEvent, match.Groups[2].Value, match.Groups[1].Value, subSentences);
            }
            // 复语句-区域效果
            /// 村民生活区（增加10个建筑位，增加10个民用工厂）
            else if (GetMatch(str, "^([^获移][^得除][\u4e00-\u9fa5]+)（(.+)）$", out match) ||
                GetMatch(str, "^([^获移][^得除][\u4e00-\u9fa5]+)：(.+)", out match))
            {
                if (!GetSubSentence(match.Groups[2].Value, "，", out List<Sentence> subSentences))
                {
                    return false;
                }
                formatted = new(PublicSign.Motions.Append, PublicSign.Types.Region, match.Groups[1].Value, null, subSentences);
            }
            else
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 从复合子句中格式化所有短句
        /// </summary>
        /// <param name="subSentence">子句</param>
        /// <param name="splitter">复合子句分割符号</param>
        /// <param name="subSentences">拆分后的所有格式化的子句</param>
        /// <returns>全部子句格式化成功返回true，有一个或以上失败则返回false</returns>
        private static bool GetSubSentence(string subSentence, string splitter, out List<Sentence> subSentences)
        {
            subSentences = new();
            var clauses = splitter == null ? new string[] { subSentence } : subSentence.Split(splitter);
            if (clauses == null || clauses.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < clauses.Length; i++)
            {
                if (!SinglePatternFormatter(clauses[i], out Sentence formatted) &&
                    !ComplexPatternFormatter(clauses[i], out formatted))
                {
                    return false;
                }
                if (formatted != null)
                {
                    subSentences.Add(formatted);
                }
            }
            return true;
        }

        #endregion
    }
}
