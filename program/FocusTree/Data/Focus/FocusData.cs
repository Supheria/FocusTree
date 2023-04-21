using FocusTree.Data.Hoi4Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Point LatticedPoint;
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
        public FocusData(int id, string name, bool beginWithStar, int duration, string descript, string ps, Point latticedPoint, List<string> rawEffects, List<HashSet<int>> requires) 
        { 
            ID = id; 
            Name = name; 
            BeginWithStar = beginWithStar; 
            Duration = duration; 
            Descript = descript; 
            Ps = ps;
            LatticedPoint = latticedPoint; 
            RawEffects = rawEffects;
            Requires = requires;
        }
    }
}
