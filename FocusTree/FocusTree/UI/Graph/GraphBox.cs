using FocusTree.Data;
using FocusTree.Data.Focus;
using FocusTree.Graph;
using FocusTree.IO;
using FocusTree.IO.FileManage;

namespace FocusTree.UI
{
    /// <summary>
    /// 封装给 UI 公用的
    /// </summary>
    public static class GraphBox
    {
        /// <summary>
        /// UI 不应该直接调用此对象的方法，而应该使用静态类封装好的
        /// </summary>
        public static FocusGraph Graph { get; private set; }
        /// <summary>
        /// 元图是否为空
        /// </summary>
        public static bool IsNull => Graph == null;
        /// <summary>
        /// 元图文件路径
        /// </summary>
        public static string FilePath { get; private set; }
        /// <summary>
        /// 元图带上只读和未保存后缀的名称
        /// </summary>
        public static string Name
        {
            get
            {
                if (ReadOnly) { return Graph.Name + "（只读）"; }
                else if (Graph.IsEdit() == true) { return Graph.Name + "（未保存）"; }
                else { return Graph.Name; }
            }
        }
        /// <summary>
        /// 是否只读（文件路径在备份文件夹）
        /// </summary>
        public static bool ReadOnly;
        /// <summary>
        /// 是否已编辑
        /// </summary>
        public static bool Edited => Graph != null && Graph.IsEdit();
        /// <summary>
        /// 是否有向前的历史记录
        /// </summary>
        public static bool HasPrevHistory => Graph != null && Graph.HasPrevHistory();
        /// <summary>
        /// 是否有向后的历史记录
        /// </summary>
        public static bool HasNextHistory => Graph != null && Graph.HasNextHistory();
        /// <summary>
        /// 元图的国策列表
        /// </summary>
        public static List<FocusData> FocusList => IsNull ? new() : Graph.FocusList;
        /// <summary>
        /// 元图节点数量
        /// </summary>
        public static int NodeCount => IsNull ? 0 : Graph.FocusList.Count;
        /// <summary>
        /// 元图分支数量
        /// </summary>
        public static int BranchCount => IsNull ? 0 : Graph.GetBranches(Graph.GetRootNodes(), false, false).Count;
        /// <summary>
        /// 元图备份列表
        /// </summary>
        /// <returns></returns>
        public static List<(string, string)> BackupList => Graph.GetBackupsList(FilePath);
        /// <summary>
        /// 元图元坐标矩形
        /// </summary>
        public static Rectangle MetaRect => Graph.GetMetaRect();
        /// <summary>
        /// 从文件路径加载元图，如果只读则封存文件路径
        /// </summary>
        /// <param name="filePath"></param>
        public static void Load(string filePath)
        {
            ReadOnly = Graph != null && Graph.IsBackupFile(filePath);
            if (!ReadOnly) { FilePath = filePath; }
            FileCache.ClearCache(Graph);
            Graph = XmlIO.LoadFromXml<FocusGraph>(filePath);
            Graph.NewHistory();
        }
        /// <summary>
        /// 从封存文件路径重新加载元图（如果文件路径存在的话）
        /// </summary>
        public static void Reload()
        {
            if (!File.Exists(FilePath)) { return; }
            ReadOnly = false;
            FileCache.ClearCache(Graph);
            Graph = XmlIO.LoadFromXml<FocusGraph>(FilePath);
            Graph.NewHistory();
        }
        /// <summary>
        /// 如果元图已修改，则备份源文件并保存到源文件
        /// </summary>
        public static void Save()
        {
            if (IsNull) { return; }
            ReadOnly = false;
            FileBackup.Backup<FocusGraph>(FilePath);
            XmlIO.SaveToXml(Graph, FilePath);
            Graph.UpdateLatest();
        }
        /// <summary>
        /// 将元图另存到新的文件路径（如果给定路径和静态文件路径相同，则执行备份和保存）
        /// </summary>
        /// <param name="filePath"></param>
        public static void SaveToNew(string filePath)
        {
            if (IsNull) { return; }
            if (filePath == FilePath)
            {
                Save();
                return;
            }
            ReadOnly = false;
            FileCache.ClearCache(Graph);
            XmlIO.SaveToXml(Graph, filePath);
            Graph?.NewHistory();
            FilePath = filePath;
        }
        /// <summary>
        /// 重做
        /// </summary>
        public static void Redo() => Graph?.Redo();
        /// <summary>
        /// 撤销
        /// </summary>
        public static void Undo() => Graph?.Undo();
        /// <summary>
        /// 从元图获取国策
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static FocusData GetFocus(int id) => Graph[id];
        /// <summary>
        /// 修改元图国策（根据国策数据内的 ID 值索引）
        /// </summary>
        /// <param name="focus"></param>
        public static void SetFocus(FocusData focus)
        {
            if (Graph == null) { return; }
            Graph[focus.ID] = focus;
            Graph.EnqueueHistory();
        }
        public static void RemoveFocusNode(FocusData focus)
        {
            Graph?.RemoveNode(focus.ID);
            Graph?.EnqueueHistory();
        }
        /// <summary>
        /// 按分支顺序重排所有国策 ID
        /// </summary>
        public static void ReorderFocusNodesID()
        {
            Graph?.ReorderNodeIds();
            Graph?.EnqueueHistory();
        }
        /// <summary>
        /// 自动排版节点
        /// </summary>
        public static void AutoLayoutAllFocusNodes()
        {
            Graph?.ResetAllNodesLatticedPoint();
            Graph?.EnqueueHistory();
        }
        /// <summary>
        /// 元图包含给定栅格化坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool ContainLatticedPoint(LatticedPoint point) => Graph != null && Graph.ContainLatticedPoint(point);
        /// <summary>
        /// 坐标是否处于任何国策节点的绘图区域中
        /// </summary>
        /// <param name="location">指定坐标 </param>
        /// <returns>坐标所处于的节点id，若没有返回null</returns>
        public static bool PointInAnyFocusNode(Point point, out FocusData? focus)
        {
            focus = null;
            if (Graph == null) { return false; }
            LatticeCell cell = new(new(point));
            if (!Graph.ContainLatticedPoint(cell.LatticedPoint, out focus)) { return false; }
            var part = cell.GetPartPointOn(point);
            if (part != LatticeCell.Parts.Node) { return false; }
            return true;
        }
        /// <summary>
        /// 删除当前备份
        /// </summary>
        public static void DeleteBackup()
        {
            Graph?.DeleteBackup();
        }
    }
}
