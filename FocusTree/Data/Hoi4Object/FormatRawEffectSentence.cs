using System.Text.RegularExpressions;

namespace FocusTree.Data.Hoi4Object
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
        /// <param name="formattedList">格式化后的语句（可能有多个），默认值为 null</param>
        /// <returns>如果格式化成功则返回true，否则返回false并记入Unformattable</returns>
        public static bool Formatter(string sentence, out List<Sentence> formattedList)
        {
            if (SinglePatternFormatter(sentence, out formattedList))
            {
                return true;
            }
            if (ComplexPatternFormatter(sentence, out formattedList))
            {
                return true;
            }
            //Unformattable.Add(sentence);
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
        /// <param name="formattedList">格式化后的语句</param>
        /// <returns>返回格式化后的单语句，如果无匹配的格式化模式则返回null</returns>
        private static bool SinglePatternFormatter(string str, out List<Sentence> formattedList)
        {
            formattedList = new();

            // 单语句-触发事件
            /// 触发事件“骑着青牛的老者？”。
            if (GetMatch(str, "^触发事件“([\u4e00-\u9fa5？《》]+)”。?$", out Match match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Trigger;
                var valueType = PublicSign.Types.Event;
                var value = match.Groups[1].Value;
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-某国触发事件
            /// 神灵庙触发事件“道教该如何面对道家？”
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)触发事件“([\u4e00-\u9fa5？]+)”。?$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Trigger;
                var valueType = PublicSign.Types.Event;
                var value = match.Groups[2].Value;
                var triggerType = PublicSign.Types.State;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            }
            // 单语句-固定值或类型
            /// 将平均灵力值固定为50%
            /// 将世界观固定为唯心世界观
            else if (GetMatch(str, "^将([\u4e00-\u9fa5]+)固定为([\u4e00-\u9fa5\\d%]+)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Fixed;
                var valueType = PublicSign.Types.Variable;
                var value = match.Groups[1].Value + PublicSign.Splitter + match.Groups[2].Value; // eg.世界观|唯心世界观
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-移除固定
            /// 移除对势力规模的固定
            else if (GetMatch(str, "^移除对([\u4e00-\u9fa5]+)的固定$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Unpin;
                var valueType = PublicSign.Types.Variable;
                var value = match.Groups[1].Value;
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-加|减值
            /// 灵力系科研速度：+35%
            /// 稳定度：-30%
            /// 每日获得的政治点数：+0.1
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)：?([+-])([\\d.]+%?)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | (match.Groups[2].Value == "+" ? PublicSign.Motions.Add : PublicSign.Motions.Sub);
                var valueType = PublicSign.Types.Variable;
                var value = match.Groups[1].Value + PublicSign.Splitter + match.Groups[3].Value;
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-加值（不带正号）
            /// 适役人口修正：15%
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)：?([\\d.]+%?)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Add;
                var valueType = PublicSign.Types.Variable;
                var value = match.Groups[1].Value + PublicSign.Splitter + match.Groups[2].Value;
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-获得|增加|移除个数
            /// 获得1个科研槽
            /// 增加10个建筑位
            /// 移除1个民用工厂
            else if (GetMatch(str, "^(获得|增加|添加|移除)(\\d+)个([\u4e00-\u9fa5]+)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | (match.Groups[1].Value == "移除" ? PublicSign.Motions.Sub : PublicSign.Motions.Add);
                var valueType = PublicSign.Types.Variable;
                var value = match.Groups[3].Value + PublicSign.Splitter + match.Groups[2].Value;
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-宣战可用性
            /// 规则修改：不能宣战
            /// 规则修改：可以宣战
            else if (GetMatch(str, "^规则修改：(不能|可以)宣战$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | (match.Groups[1].Value == "可以" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove);
                var valueType = PublicSign.Types.AbleToDeclareWar;
                formattedList = new() { new(motion, valueType, null, null, null, null) };
            }
            // 单语句-触发并开启决议
            /// 可以通过决议发动周边国家的内战使其变为附庸
            else if (GetMatch(str, "^可以通过决议([\u4e00-\u9fa5]+)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Trigger;
                var valueType = PublicSign.Types.Resolution;
                var value = match.Groups[2].Value;
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-自动获得核心可用性
            /// 幽灵种族的省份将自动获得核心：是
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)将自动获得核心：(是|否)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | (match.Groups[2].Value == "是" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove);
                var valueType = PublicSign.Types.AbleToGainCoreAuto;
                var triggerType = PublicSign.Types.Province;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, null, triggerType, trigger, null) };
            }
            // 单语句-某国的区域获得核心
            /// 山童镇，山童实验场，水坝控制站，河童大坝，蒸汽村落，南玄武川城，中玄武川镇，三平实验室：隐居村落获得该地区的核心。
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)((，[\u4e00-\u9fa5]+)+：[\u4e00-\u9fa5]+获得该地区的核心。$)", out match))
            {
                List<string> regions = new();
                do
                {
                    str = match.Groups[2].Value;
                    regions.Add(match.Groups[1].Value);
                } while (GetMatch(str, "^，([\u4e00-\u9fa5]+)(.+)$", out match));
                GetMatch(str, "^：([\u4e00-\u9fa5]+)获得该地区的核心。$", out match);
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Gain;
                var valueType = PublicSign.Types.RegionCore;
                var triggerType = PublicSign.Types.State;
                var t = match.Groups[1].Value;
                foreach(var region in regions)
                {
                    t += PublicSign.Splitter + region;
                }
                var trigger = new string[] { t }; // eg.隐居村落|山童镇|山童实验场|...
                formattedList = new() { new(motion, valueType, null, triggerType, trigger, null) };
            }
            // 单语句-数值修正
            /// （天狗共和国）我国对其进攻修正：+20%
            else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）我国(对其[\u4e00-\u9fa5]+)修正：([+-])(\\d+%?)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | (match.Groups[3].Value == "+" ? PublicSign.Motions.Add : PublicSign.Motions.Sub);
                var valueType = PublicSign.Types.Variable;
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value + PublicSign.Splitter + match.Groups[4].Value; // eg.对其进攻|天狗共和国|20%
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-获得对他国的战争目标
            /// 获得对守矢神社的吞并战争目标
            else if (GetMatch(str, "^获得对([\u4e00-\u9fa5]+)的([\u4e00-\u9fa5]+)战争目标", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Gain;
                var valueType = PublicSign.Types.WarGoal;
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value; // eg.吞并|守矢神社
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-研究加成
            /// 3x60%研究加成：基础武器。
            else if (GetMatch(str, "^(\\dx\\d+%?)研究加成：([\u4e00-\u9fa5]+)。?$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Bonus;
                var valueType = PublicSign.Types.Reaserch;
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value; // eg.基础武器|3x60%
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-修改规则：可以创建阵营
            /// 可以创建阵营
            /// 获得允许创建阵营
            else if (GetMatch(str, "^可以创建阵营|获得允许创建阵营$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Gain;
                var valueType = PublicSign.Types.AbleToCreateCamp;
                formattedList = new() { new(motion, valueType, null, null, null, null) };
            }
            // 单语句-暂时无影响
            /// 这项国策目前没有实际影响。但随着世界局势的变化可能会发生改变。
            else if (GetMatch(str, "这项国策目前没有实际影响。但随着世界局势的变化可能会发生改变。", out match))
            {
                var motion = PublicSign.Motions.NoneButMayChange;
                formattedList = new() { new(motion, null, null, null, null, null) };
            }
            // 单语句-创建阵营
            /// 获得允许创建阵营，创建阵营：道盟
            else if (GetMatch(str, "^获得允许创建阵营，创建阵营：([\u4e00-\u9fa5]+)。?$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Create;
                var valueType = PublicSign.Types.Camp;
                var value = match.Groups[1].Value;
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-某国创建指定属性的阵营
            /// （守矢神社）获得允许创建防御性阵营，创建阵营：妖怪山自卫联盟。
            else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）获得允许创建([\u4e00-\u9fa5]+)阵营，创建阵营：([\u4e00-\u9fa5]+)。?$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Create;
                var valueType = PublicSign.Types.Camp;
                var value = match.Groups[3].Value + PublicSign.Splitter + match.Groups[2].Value; // eg.妖怪山自卫联盟|防御性
                var triggerType = PublicSign.Types.State;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            }
            // 单语句-不允许加入阵营
            /// x可以加入阵营
            else if (GetMatch(str, "^x可以加入阵营$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Remove;
                var valueType = PublicSign.Types.AbleToJoinCamp;
                formattedList = new() { new(motion, valueType, null, null, null, null) };
            }
            // 单语句-某国加入阵营
            /// 神灵庙加入阵营
            /// 隐居村落加入妖怪山自卫联盟
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)加入([\u4e00-\u9fa5]+)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Join;
                var valueType = PublicSign.Types.Camp;
                var value = match.Groups[2].Value;
                var triggerType = PublicSign.Types.State;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            }
            // 某国吞并他国
            /// 隐居村落吞并河童长老会
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)吞并([\u4e00-\u9fa5]+)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Annexed;
                var valueType = PublicSign.Types.State;
                var value = match.Groups[2].Value;
                var triggerType = PublicSign.Types.State;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            }
            // 单语句-获得|移除标签
            /// 获得五弊三缺
            /// 移除道教的探索精神
            else if (GetMatch(str, "^(获得|移除)([\u4e00-\u9fa5]+)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | (match.Groups[1].Value == "获得" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove);
                var valueType = PublicSign.Types.Label;
                var value = match.Groups[2].Value;
                formattedList = new() { new(motion, valueType, value, null, null, null) };
            }
            // 单语句-某国获得标签
            ///（神灵庙）获得命莲寺的宣称
            else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）获得([\u4e00-\u9fa5]+)$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Gain;
                var valueType = PublicSign.Types.Label;
                var value = match.Groups[2].Value;
                var triggerType = PublicSign.Types.State;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            }
            // 单语句-增加部队
            /// 将会出现6个编制为神灵庙护卫编制的部队
            else if (GetMatch(str, "^将会出现(\\d+)个编制为([\u4e00-\u9fa5]+)编制的部队$", out match))
            {
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Add;
                var valueType = PublicSign.Types.Troop;
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value; // eg.神灵庙护卫|6
                formattedList = new() { new(motion, valueType, value, null, null, null) };
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
        /// <param name="formattedList">格式化后的语句，默认值为 null</param>
        /// <returns>格式化成功返回true，否则返回false。若子语句中有一个或以上的短句无法格式化，同样判定为格式化失败返回false</returns>
        private static bool ComplexPatternFormatter(string str, out List<Sentence> formattedList)
        {
            formattedList = new();

            // 复语句-获得理念类标签
            /// 获得世界线的观察见证者，其效果为（理念类，每日获得的政治点数：-0.05，理念类花费：+10%，稳定度：+35%，建造速度：+25%，部队核心领土攻击：+30%，部队核心领土防御：+30%，ai修正：专注防御：+30%，防御战争对稳定度修正：+20%，孤立倾向：+0.01，路上要塞建造速度：+25%，防空火炮建造速度：+25%，雷达站建造速度：+25%）
            if (GetMatch(str, "^(获得|移除)([\u4e00-\u9fa5\\d/]+)，其效果为（(理念类)，(.+)）$", out var match))
            {
                if (!GetSubSentence(match.Groups[4].Value, "，", out var subSentences))
                {
                    return false;
                }
                var motion = PublicSign.Motions.AfterDone | (match.Groups[1].Value == "获得" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove);
                var valueType = PublicSign.Types.Label;
                var value = match.Groups[3].Value + PublicSign.Splitter + match.Groups[2].Value; // eg.理念类|世界线的观察见证者
                formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            }
            // 复语句-获得限时标签
            /// 获得为期365天的“强大的威望”，其效果为（每日获得的政治点数：+0.25，部队组织度：+15%，稳定度：+20%，战争支持度：+20%，建造速度：+10%，部队攻击：+5%，部队防御：+5%，科研速度：+10%，工厂产出：+10%）
            else if (GetMatch(str, "^获得(?:为期)?(\\d+[天月年])的“([\u4e00-\u9fa5]+)”，其效果为（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[3].Value, "，", out var subSentences))
                {
                    return false;
                }
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Gain;
                var valueType = PublicSign.Types.Label;
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value; // eg.强大的威望|365天
                formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            }
            // 复语句-某国获得限时标签
            /// 所有敌国获得为期365天的“必有凶年”，其效果为（生活消费品工厂：+30%，适役人口修正：-80%）
            else if (GetMatch(str, "^([\u4e00-\u9fa5\\d/]+)获得(?:为期)?(\\d+[天月年])的“([\u4e00-\u9fa5]+)”，其效果为（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[4].Value, "，", out var subSentences))
                {
                    return false;
                }
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Gain;
                var valueType = PublicSign.Types.Label;
                var value = match.Groups[3].Value + PublicSign.Splitter + match.Groups[2].Value;
                var triggerType = PublicSign.Types.State;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            }
            // 复语句-获得|移除标签
            /// 获得无为而治，其效果为（每周人口：+1，每月人口：+15%）
            /// 移除圣人，其效果为（孤立倾向：+0.1，每日唯心度变化：+0.04%）
            /// 获得强烈的战略结盟（对隐居村落的关系：+100）
            /// 获得幽灵/亡灵，其效果为（战略资源获取率：+10%，部队组织度：-5%，适役人口：+0.2%，移动中组织度损失：+5%，部队损耗：-20%，补给损耗：-15%，部队组织度恢复：+10%，步兵部队攻击：-30%，步兵部队防御：-30%，训练时间：-40%，每日人类影响力基础变化：+0.02，征兵法案花费：+50%）
            else if (GetMatch(str, "^(获得|移除)([\u4e00-\u9fa5\\d/]+)，其效果为（(.+)）$", out match) ||
                GetMatch(str, "^(获得|移除)([\u4e00-\u9fa5]+)（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[3].Value, "，", out var subSentences))
                {
                    return false;
                }
                var motion = PublicSign.Motions.AfterDone | (match.Groups[1].Value == "获得" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove);
                var valueType = PublicSign.Types.Label;
                var value = match.Groups[2].Value;
                formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            }
            // 复语句-某国获得|失去标签
            /// （隐居村落）获得自古以来（对命莲寺关系：+200）
            /// （守矢神社）获得万民自化，其效果为（每周稳定度：+0.1%，每周战争支持度：-0.1%，战争支持度：+100%）
            else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）(获得|失去)([\u4e00-\u9fa5]+)，其效果为（(.+)）$", out match) ||
                GetMatch(str, "^（([\u4e00-\u9fa5]+)）(获得|失去)([\u4e00-\u9fa5]+)（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[4].Value, "，", out var subSentences))
                {
                    return false;
                }
                var motion = PublicSign.Motions.AfterDone | (match.Groups[2].Value == "获得" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove);
                var valueType = PublicSign.Types.Label;
                var value = match.Groups[3].Value;
                var triggerType = PublicSign.Types.State;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            }
            // 复语句-每个国家获得标签
            /// 每个国家获得：道德观不同（对隐世村落关系：-20）
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)(获得|失去)：([\u4e00-\u9fa5]+)（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[4].Value, "，", out var subSentences))
                {
                    return false;
                }
                var motion = PublicSign.Motions.AfterDone | (match.Groups[2].Value == "获得" ? PublicSign.Motions.Gain : PublicSign.Motions.Remove);
                var valueType = PublicSign.Types.Label;
                var value = match.Groups[3].Value;
                var triggerType = PublicSign.Types.State;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            }
            // 复语句-变更等级
            /// 向保守社会发展一级，以保守社会5取代保守社会4，效果变化（稳定度：+6% 科研速度：-3% 加密：+0.5 可出口资源：-5% 意识形态变化抵制力度：+10%）
            else if (GetMatch(str, "^[\u4e00-\u9fa5]+，以([\u4e00-\u9fa5]+\\d)取代([\u4e00-\u9fa5]+\\d)，效果变化（(.+)）$", out match))
            {
                if (!GetSubSentence(match.Groups[3].Value, " ", out var subSentences))
                {
                    return false;
                }
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Replace;
                var valueType = PublicSign.Types.Grade;
                var value = match.Groups[2].Value + PublicSign.Splitter + match.Groups[1].Value; // eg.保守社会4|保守社会5
                formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            }
            // 复语句-追加效果
            /// 天地不仁追加效果：稳定度：-5%，适役人口：+2%
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)追加效果：(.+)$", out match))
            {
                if (!GetSubSentence(match.Groups[2].Value, "，", out var subSentences))
                {
                    return false;
                }
                foreach (var subSentence in subSentences)
                {
                    subSentence.TriggerType = PublicSign.Types.Label;
                    subSentence.Trigger = new() { match.Groups[1].Value };
                    formattedList.Add(subSentence);
                }
            }
            // 复语句-某国触发可同意事件
            /// 每个孤立国家触发事件“隐世村落的无偿教导？”。如果他们同意，则获得强烈的战略结盟（对隐居村落的关系：+100）
            else if (GetMatch(str, "^(每个孤立国家)触发事件“([\u4e00-\u9fa5？]+)”。如果他们同意，则(.+)$", out match))
            {
                if (!GetSubSentence(match.Groups[3].Value, null, out var subSentences))
                {
                    return false;
                }
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Trigger;
                var valueType = PublicSign.Types.RequestEvent;
                var value = match.Groups[2].Value;
                var triggerType = PublicSign.Types.State;
                var trigger = new string[] { match.Groups[1].Value };
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            }
            // 复语句-多国触发事件
            /// （天狗帝国）（山姥部落）（妖兽仙界）触发事件“有偿帮助”。如果他们同意，则获得保护自己的臣民，其效果为（陆上要塞：建造速度：+25%）
            else if (GetMatch(str, "^(（[\u4e00-\u9fa5？]+）)+触发事件“([\u4e00-\u9fa5？]+)”。如果他们同意，则(.+)$", out match))
            {
                List<string> states = new();
                while (GetMatch(str, "^（([\u4e00-\u9fa5？]+)）(.+)$", out match))
                {
                    states.Add(match.Groups[1].Value);
                    str = match.Groups[2].Value;
                }
                GetMatch(str, "^触发事件“([\u4e00-\u9fa5？]+)”。如果他们同意，则(.+)$", out match);
                if (!GetSubSentence(match.Groups[2].Value, null, out var subSentences))
                {
                    return false;
                }
                var motion = PublicSign.Motions.AfterDone | PublicSign.Motions.Trigger;
                var valueType = PublicSign.Types.RequestEvent;
                var value = match.Groups[1].Value;
                var triggerType = PublicSign.Types.State;
                var trigger = states.ToArray(); // eg.天狗帝国|山姥部落|妖兽仙界
                formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            }
            // 复语句-开启国策后立即实施效果
            /// 当选中此项时：所有敌国获得为期365天的“必有凶年”，其效果为（生活消费品工厂：+30%，适役人口修正：-80%）
            else if (GetMatch(str, "^当选中此项时：(.+)$", out match))
            {
                if (!GetSubSentence(match.Groups[1].Value, null, out var subSentences))
                {
                    return false;
                }
                foreach (var subSentence in subSentences)
                {
                    if (subSentence.Motion == null)
                    {
                        return false;
                    }
                    subSentence.Motion = PublicSign.Motions.Instantly | subSentence.Motion ^ PublicSign.Motions.AfterDone;
                    formattedList.Add(subSentence);
                }
            }
            // 复语句-某国增加效果
            /// 守矢神社增加：科研共享加成+5%
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)增加：(.+)$", out match))
            {
                if (!GetSubSentence(match.Groups[2].Value, null, out var subSentences))
                {
                    return false;
                }
                foreach (var subSentence in subSentences)
                {
                    subSentence.TriggerType = PublicSign.Types.State;
                    subSentence.Trigger = new() { match.Groups[1].Value };
                    formattedList.Add(subSentence);
                }
            }
            // 复语句-ai修正
            /// ai修正：专注防御：+30%
            else if (GetMatch(str, "^ai修正：(.+)$", out match))
            {
                if (!GetSubSentence(match.Groups[1].Value, null, out var subSentences))
                {
                    return false;
                }
                foreach (var subSentence in subSentences)
                {
                    subSentence.TriggerType = PublicSign.Types.AiModifyer;
                    formattedList.Add(subSentence);
                }
            }
            // 复语句-区域增加效果
            /// 村民生活区（增加10个建筑位，增加10个民用工厂）
            /// 所有拥有的地区：添加5个基础设施
            /// 山中鸡场（移除1个民用工厂，增加1个军用工厂）
            else if (GetMatch(str, "^([\u4e00-\u9fa5]+)（(.+)）$", out match) ||
                GetMatch(str, "^([\u4e00-\u9fa5]+)：(.+)$", out match))
            {
                if (!GetSubSentence(match.Groups[2].Value, "，", out var subSentences))
                {
                    return false;
                }
                foreach (var subSentence in subSentences)
                {
                    subSentence.TriggerType = PublicSign.Types.Region;
                    subSentence.Trigger = new() { match.Groups[1].Value };
                    formattedList.Add(subSentence);
                }
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
                if (!SinglePatternFormatter(clauses[i], out var formattedList) &&
                    !ComplexPatternFormatter(clauses[i], out formattedList))
                {
                    return false;
                }
                if (formattedList != null)
                {
                    foreach (var formatted in formattedList)
                    {
                        subSentences.Add(formatted);
                    }
                }
            }
            return true;
        }

        #endregion
    }
}
