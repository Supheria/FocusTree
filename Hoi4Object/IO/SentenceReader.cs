namespace Hoi4Object.IO
{
    internal class SentenceReader
    {
        /// <summary>
        /// 单语句-触发事件
        /// 触发事件“骑着青牛的老者？”。
        /// </summary>
        static string Pattern_Single_TriggerEvent = "";
        /// <summary>
        /// 单语句-固定值或类型
        /// 将平均灵力值固定为50%
        /// 将世界观固定为唯心世界观
        /// </summary>
        static string Pattern_Single_Fix = "^(?:将)([\u4e00-\u9fa5]+)(?:固定为)(.+)";
        /// <summary>
        /// 单语句模式-改变值(加减)
        /// 稳定度：-30%
        /// </summary>
        static string Pattern_Single_AddSub = "^[\u4e00-\u9fa5]+(?:：)([+|-][0-9]*%?)";
        /// <summary>
        /// 单语句-改变值(获得)
        /// 获得1个科研槽
        /// </summary>
        static string Pattern_Single_Get = "^(?:获得)([0-9]+)(?:个)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 单语句-改变值(失去)
        /// 获得1个科研槽
        /// </summary>
        static string Pattern_Single_Lose = "^(?:失去)([0-9]+)(?:个)([\u4e00-\u9fa5]+)";
        /// <summary>
        /// 单语句-规则修改
        /// 规则修改：不能宣战
        /// </summary>
        static string Pattern_Single_ChangeRule = "^(?:规则修改：)(.+)";
        /// <summary>
        /// 单语句-可用决议
        /// 可以通过决议发动周边国家的内战使其变为附庸
        /// </summary>
        static string Pattern_Single_AvailableResolution = "^(?:可以通过决议)(.+)";
        /// <summary>
        /// 单语句-自动获得核心
        /// 幽灵种族的省份将自动获得核心：是
        /// </summary>
        static string Pattern_Single_AutoGetCore = "^([\u4e00-\u9fa5]+)(?:的省份将自动获得核心：)([是|否])";
        /// <summary>
        /// 复语句-获得效果
        /// 获得无为而治，其效果为（每周人口：+1，每月人口：+15%）
        /// </summary>
        static string Pattern_Complex_GetEffect = "^(?:获得)([\u4e00-\u9fa5]+)(?:，其效果为（)([^）]+)";
        /// <summary>
        /// 复语句-获得外交关系
        /// （隐居村落）获得自古以来（对命莲寺关系：+200）
        /// </summary>
        static string Pattern_Complex_GetRelation = "^(?:（)([\u4e00-\u9fa5]+)(?:）获得)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
        /// <summary>
        /// 复语句-失去外交关系
        /// （隐居村落）获得自古以来（对命莲寺关系：+200）
        /// </summary>
        static string Pattern_Complex_LoseRelation = "^(?:（)([\u4e00-\u9fa5]+)(?:）失去)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
        /// <summary>
        /// 复语句-每个国家获得外交关系
        /// 每个国家获得：道德观不同（对隐世村落关系：-20）
        /// </summary>
        static string Pattern_Complex_EveryStateGetRelation = "^(?:每个国家获得：)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
        /// <summary>
        /// 复语句-每个国家失去外交关系
        /// 每个国家失去：道德观不同（对隐世村落关系：-20）
        /// </summary>
        static string Pattern_Complex_EveryStateLoseRelation = "^(?:每个国家失去：)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
        /// <summary>
        /// 复语句-发展一级
        /// 向保守社会发展一级，以保守社会5取代保守社会4，效果变化（稳定度：+6% 科研速度：-3% 加密：+0.5 可出口资源：-5% 意识形态变化抵制力度：+10%）
        /// </summary>
        static string Pattern_Complex_Upgrade = "^(?:向)([\u4e00-\u9fa5]+)(?:发展一级，以[\u4e00-\u9fa5]+)([0-9]+)(?:取代[\u4e00-\u9fa5]+)([0-9]+)(?:，效果变化（)([^）]+)";
        /// <summary>
        /// 复语句-追加效果
        /// 天地不仁追加效果：稳定度：-5%，适役人口：+2%
        /// </summary>
        static string Pattern_Complex_Append = "^([\u4e00-\u9fa5]+)(?:追加效果：)(.+)$";
        /// <summary>
        /// 复语句-别国触发可同意事件
        /// 每个孤立国家触发事件“隐世村落的无偿教导？”。如果他们同意，则获得强烈的战略结盟（对隐居村落的关系：+100）
        /// </summary>
        static string Pattern_Complex_OtherTriggerEventWithConsent = "^([\u4e00-\u9fa5]+)(?:触发事件“)([\u4e00-\u9fa5？]+)(?:”。如果他们同意，则获得)([\u4e00-\u9fa5]+)(?:（)([^）]+)";
    }
}
