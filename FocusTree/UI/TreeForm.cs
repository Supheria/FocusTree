
using System.Text;
using System.Text.RegularExpressions;
using CSVFile;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.IO;

using FocusTree.Tree;

namespace FocusTree
{
    /// <summary>
    /// 树窗
    /// </summary>
    public partial class TreeForm : Form
    {
        private TreeMap TreeMap { get; init; }
        
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
                    TreeMap = new TreeMap(new Tree.Tree(fileinfo.FullName));
                    break;
                // xml 文件
                case ".xml":
                    var focusTree = DeserializeFromXml(fileinfo.FullName);
                    TreeMap = new TreeMap(focusTree);
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
            this.Controls.Add(TreeMap);
            ImageStartLocation = new Point(0, 0);
            Name = Text = TreeMap.Name;
        }
        #endregion
        #region ==== 节点控件事件 ====
        
        #endregion
        #region ==== 窗体方法 ====
        
        #endregion
        #region ==== 文件方法 ====
        /// <summary>
        /// 序列化成XML
        /// </summary>
        /// <param name="fstream"></param>
        public void SerializeToXml(string? szSave = null)
        {
            if (szSave == null)
            {
                szSave = string.Format($"{rawParentFolderPath}.xml");
            }
            #region ==== XmlWriterSettings ====
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineChars = Environment.NewLine;
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = true;  // 不生成声明头
            #endregion
            FileStream fileStream;
            XmlWriter xmlWriter;
            try
            {
                fileStream = new FileStream(szSave, FileMode.Create);
                xmlWriter = XmlWriter.Create(fileStream, settings);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}\n无法创建\"{szSave}。\"");
            }
            try
            {
                // 强制指定命名空间，覆盖默认的命名空间
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                XmlSerializer serializer = new XmlSerializer(TreeMap.Tree.GetType());
                serializer.Serialize(new XmlWriterForceFullEnd(xmlWriter), TreeMap.Tree, namespaces);
                xmlWriter.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                xmlWriter.Close();
                fileStream.Close();
                throw new Exception($"{ex.Message}\n无法写入\"{szSave}。\"");
            }
        }
        /// <summary>
        /// 从XML反序列化
        /// </summary>
        /// <param name="fstream"></param>
        public Tree.Tree DeserializeFromXml(string szLoad)
        {
            FileStream fileStream;
            StreamReader streamReader;
            try
            {
                fileStream = new FileStream(szLoad, FileMode.Open);
                streamReader = new StreamReader(fileStream, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}\n无法打开\"{szLoad}。\"");
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Tree.Tree));
                Tree.Tree tree = (Tree.Tree)serializer.Deserialize(streamReader);
                streamReader.Close();
                fileStream.Close();
                return tree;
            }
            catch (Exception ex)
            {
                streamReader.Close();
                fileStream.Close();
                throw new Exception($"{ex.Message}\n无法读取\"{szLoad}。\"");
            }
        }
        #endregion
    }
}