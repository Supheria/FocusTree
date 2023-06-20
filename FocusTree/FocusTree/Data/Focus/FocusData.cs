using FocusTree.Graph;

namespace FocusTree.Data.Focus
{
    /// <summary>
    /// 国策数据
    /// </summary>
    public struct FocusData
    {
        /// <summary>
        /// 栅格化坐标
        /// </summary>
        public LatticedPoint LatticedPoint;
        /// <summary>
        /// 节点ID
        /// </summary>
        public int ID;
        /// <summary>
        /// 国策名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 实施国策所需的天数
        /// </summary>
        public int Duration;
        /// <summary>
        /// 国策描述
        /// </summary>
        public string Descript;
        /// <summary>
        /// 备注
        /// </summary>
        public string Ps;
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        public bool BeginWithStar;
        /// <summary>
        /// 原始效果语句
        /// </summary>
        public List<string> RawEffects;
        /// <summary>
        /// 依赖组
        /// </summary>
        public List<HashSet<int>> Requires;
        public FocusData()
        {
            LatticedPoint = new();
            ID = 0;
            Name = string.Empty;
            Duration = 0;
            Descript = string.Empty;
            Ps = string.Empty;
            BeginWithStar = false;
            RawEffects = new();
            Requires = new();
        }
    }
}
