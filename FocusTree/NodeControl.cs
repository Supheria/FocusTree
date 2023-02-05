#region ==== 类型别称 ====
// 分支包类型
// 外层List：记录所有分支
using BranchPackType = System.Collections.Generic.List<System.Collections.Generic.List<FocusTree.CNode>>;
// 分支类型
// 内层CNode：以节点作为分支上的叶
// List：记录从末节点到根节点的所有叶，作为一个分支（倒序分支）
using BranchType = System.Collections.Generic.List<FocusTree.CNode>;
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FocusTree
{
    /// <summary>
    /// 节点控件
    /// </summary>
    public partial class NodeControl : UserControl
    {
        public CNode mNode { get; private set; }
        /// <summary>
        /// 每个节点的控件宽度
        /// </summary>
        public int NODE_WIDTH;
        /// <summary>
        /// 每个节点的控件高度
        /// </summary>
        public int NODE_HIGHT;
        /// <summary>
        /// 节点所在的目标列
        /// </summary>
        public int ToSetColum { get; private set; }
        /// <summary>
        /// 节点转换成控件
        /// </summary>
        /// <param name="node">要转换成控件的节点</param>
        public NodeControl(CNode node)
        {
            InitializeComponent();
            mNode = node;
            // 节点名称、字段
            lblTitle.Text = Text = Name = node.mFocusData.mName;
            //NODE_WIDTH = Size.Width;
            //NODE_HIGHT = Size.Height;
            //Size = new Size(NODE_WIDTH, NODE_HIGHT);
            Location = new Point(0, 0);
            ToSetColum = (mNode.mEndColum - mNode.mStartColum) / 2 + mNode.mStartColum;
        }

    }

    [Serializable]
    /// <summary>
    /// 节点类
    /// </summary>
    public class CNode
    {
        #region ==== 节点属性 ====
        /// <summary>
        /// 节点ID
        /// </summary>
        public int mID { get; private set; }
        /// <summary>
        /// // 层级
        /// </summary>
        public int mLevel { get; private set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public CNode? mParent { get; private set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<CNode> mChildren { get; private set; }
        /// <summary>
        /// 国策数据
        /// </summary>
        public SFocusData mFocusData { get; private set; }
        #endregion
        #region ==== 节点在树中的属性 ====
        /// <summary>
        /// 节点在树中的终止列
        /// </summary>
        public int mEndColum;
        /// <summary>
        /// 节点在树中的起始列
        /// </summary>
        public int mStartColum;
        #endregion
        #region ==== 初始化节点 ====
        /// <summary>
        /// 创建节点，并作为子节点加入在父节点下
        /// </summary>
        /// <param name="ID">节点ID</param>
        /// <param name="level">层级(所在的列数)</param>
        /// <param name="parent">父节点</param>
        /// <param name="text">原始国策字段</param>
        public CNode(
            int ID,
            int level,
            CNode parent,
            string text
            )
        {
            InitNode(ID, level, parent);
            // 把节点加入父节点的子集
            mParent.mChildren.Add(this);
            // 设置国策数据
            try
            {
                SetFocusData(text);
            }
            catch (Exception ex)
            {
                throw new Exception($"创建ID={ID}的节点失败。\n{ex.Message}");
            }
            // 起始列和终止列默认为0
            mEndColum = mStartColum = 0;
        }
        /// <summary>
        /// 创建节点，并作为子节点加入在父节点下
        /// </summary>
        /// <param name="ID">节点ID</param>
        /// <param name="level">层级(所在的列数)</param>
        /// <param name="parent">父节点</param>
        /// <param name="focusData">国策数据</param>
        public CNode(
            int ID,
            int level,
            CNode parent,
            SFocusData focusData
            )
        {
            InitNode(ID, level, parent);
            // 把节点加入父节点的子集
            mParent.mChildren.Add(this);
            // 设置国策数据
            mFocusData = focusData;
            // 起始列和终止列默认为0
            mEndColum = mStartColum = 0;
        }
        /// <summary>
        /// 此构造函数专用于创建根节点
        /// 根节点ID为0，层级为-1，没有父节点
        /// </summary>
        public CNode()
        {
            // 根节点ID为0，层级为-1，没有父节点
            InitNode(0, -1);
        }
        private void InitNode(int ID, int level, CNode? parent = null)
        {
            mID = ID;
            mLevel = level;
            mParent = parent;
            mChildren = new List<CNode>();
        }
        /// <summary>
        /// 根据文本设置节点的国策数据
        /// </summary>
        /// <param name="text">原始国策字段</param>
        private void SetFocusData(string text)
        {
            // 在 C# 中的字符串，{ 需要转义，通过分割一对来避免歧义。 原 Regex: (.+?){(\d+)天}{(.+?)}(?:{(.+)})?(.+)?
            var pattern = "(.+?){" + "(\\d+)天}{" + "(.+?)}(?:{" + "(.+)})?(.+)?";
            try
            {
                var match = Regex.Match(text, pattern);
                // Groups[0] 是匹配成功部分的文本，应当等同于 text。
                // 从[1]开始才是括号匹配的数据
                // 是否以 * 开头
                var isBeginWithStar = match.Groups[1].Value.StartsWith("*");
                // 名称
                string name;
                // 如果以 * 开头，则去掉 *
                if (isBeginWithStar)
                    name = match.Groups[1].Value.Substring(1);
                else
                    name = match.Groups[1].Value;
                // 天数
                int duration = int.Parse(match.Groups[2].Value);
                // 效果
                string effects = match.Groups[3].Value;
                // 描述
                string descript = match.Groups[4].Value;
                // 备注
                string ps = match.Groups[5].Value;
                mFocusData = new SFocusData(
                    name,
                    isBeginWithStar,
                    duration,
                    effects,
                    descript,
                    ps
                    );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"正则匹配时发生了异常。\n" +
                    $"试图解析的内容: {text}\n" +
                    $"异常信息: {ex.Message}");
            }
        }
        #endregion
        #region ==== 节点方法 ====
        /// <summary>
        /// 获取nExcludeLevel节点之下的所有正序的分支
        /// </summary>
        /// <param name="nExcludeLevel">分支所不包含的层级，默认不包含根节点</param>
        /// <returns>所有分支打包成的分支包</returns>
        public BranchPackType GetBranches(int nExcludeLevel = -1)
        {
            
            // 创建本节点的分支包
            BranchPackType branchPack = new BranchPackType();
            // 这是分支上最后一个节点
            if (mChildren.Count == 0)
            {
                // 从末节点开始创建一条新分支
                BranchType branch = new BranchType { this };
                branchPack.Add(branch);
                return branchPack;
            }
            // 遍历子所有节点的分支包
            for (int i = 0; i < mChildren.Count; i++)
            {
                // 获取子节点的分支包
                BranchPackType buffer = mChildren[i].GetBranches();
                // 把分支包解包，并在每支节点上加入本节点的ID，最后打包到branchPack
                for (int j = 0; j < buffer.Count; j++)
                {
                    BranchType branch = buffer[j];
                    if (mLevel != nExcludeLevel)
                    {
                        // 在分支上加入本节点
                        branch.Add(this);
                    }
                    else
                    {
                        // 反序分支
                        branch.Reverse();
                    }
                    // 将分支加入新分支包
                    branchPack.Add(branch);
                }
            }
            return branchPack;
        }
        #endregion
        #region ==== 运算符重载 ====
        /// <summary>
        /// 得到妹节点
        /// </summary>
        /// <param name="node">如果没有，则返回自身</param>
        /// <returns></returns>
        public static CNode operator ++(CNode node)
        {
            int nIndex = node.mParent.mChildren.IndexOf(node);
            // 节点不是父节点的最后一个子节点
            if (nIndex < node.mParent.mChildren.Count - 1)
            {
                return node.mParent.mChildren[nIndex + 1];
            }
            else
                return node;
        }
        /// <summary>
        /// 得到兄节点
        /// </summary>
        /// <param name="node">如果没有，则返回自身</param>
        /// <returns></returns>
        public static CNode operator --(CNode node)
        {
            int nIndex = node.mParent.mChildren.IndexOf(node);
            // 节点不是父节点的第一个子节点
            if (nIndex != 0)
            {
                return node.mParent.mChildren[nIndex - 1];
            }
            else
                return node;
        }
        #endregion
    }

    [Serializable]
    /// <summary>
    /// 国策数据
    /// </summary>
    public struct SFocusData
    {
        /// <summary>
        /// 国策名称
        /// </summary>
        public string mName;
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        public bool nBeginWithStar;
        /// <summary>
        /// 实施天数
        /// </summary>
        public int mDuration;
        /// <summary>
        /// 国策效果
        /// </summary>
        public string mEffects;
        /// <summary>
        /// 国策描述
        /// </summary>
        public string mDescript;
        /// <summary>
        /// 备注
        /// </summary>
        public string mPs;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">国策名称</param>
        /// <param name="isBeginWithstar">字段是否以 * 开头</param>
        /// <param name="duration">实施天数</param>
        /// <param name="effects">国策效果</param>
        /// <param name="descript">国策描述</param>
        /// <param name="ps">备注</param>
        public SFocusData(
            string name,
            bool isBeginWithstar,
            int duration,
            string effects,
            string descript,
            string ps
            )
        {
            mName = name;
            nBeginWithStar = isBeginWithstar;
            mDuration = duration;
            mEffects = effects;
            mDescript = descript;
            mPs = ps;
        }
    }
}
