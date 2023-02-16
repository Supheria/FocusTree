
using System.Text;
using System.Text.RegularExpressions;
using CSVFile;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.IO;

using FocusTree.Tree;
using FocusTree.Focus;
using FocusTree.IO;

namespace FocusTree
{
    /// <summary>
    /// 树窗
    /// </summary>
    public partial class TreeForm : Form
    {
        public GraphMap Map { get; init; }
        
        /// <summary>
        /// 画图开始的位置
        /// </summary>
        private Point ImageStartLocation = new Point(0, 0);
        /// <summary>
        /// 原始父文件夹路径
        /// </summary>
        private string rawParentFolderPath = string.Empty;

        
        #region  ==== 初始化窗体 ====
        /// <summary>
        /// 使用 .csv 文件或 .xml 文件 创建树实例
        /// </summary>
        /// <param name="path">文件路径</param>
        public TreeForm(string path)
        {
            var fileinfo = new FileInfo(path);  // 文件信息
            // !!!! 需要修改的变量名 !!!!
            // 不包含文件扩展名的完整路径
            rawParentFolderPath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length);

            // 文件不存在
            if (!fileinfo.Exists) { throw new FileNotFoundException($"[2302151951] 无法从文件生成树 - 文件不存在: {path}"); }
            // 根据扩展名决定读取方式
            switch (fileinfo.Extension.ToLower())
            {
                // csv 文件
                case ".csv":
                    {
                        var tree = new FTree(fileinfo.FullName);
                        var graph = new FGraph(tree);
                        Map = new GraphMap(graph);
                    }
                    break;
                // xml 文件
                case ".xml":
                    {
                        var graph = FXml.LoadGraph(fileinfo.FullName);
                        Map = new GraphMap(graph);
                    }
                    break;

                // 不支持的文件类型
                default:
                    throw new FileNotFoundException($"[2302152008] 不支持的文件类型，需要 .csv 或 .xml 文件 - 文件: {path}");
            }

            // 初始化窗体控件
            InitForm();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitForm()
        {
            InitializeComponent();
            this.Controls.Add(Map);
            ImageStartLocation = new Point(0, 0);
            Name = Text = Map.Name;
        }
        #endregion
        #region ==== 节点控件事件 ====
        
        #endregion
        #region ==== 窗体方法 ====
        
        #endregion
    }
}