using FocusTree.Data;
using System.IO.Compression;

namespace FocusTree.IO
{
    internal class Backup
    {
        /// <summary>
        /// 根备份目录
        /// </summary>
        public static string DirectoryName
        {
            get { return DirectoryInfo.FullName; }
        }
        static string FolderPath = "backup\\backups";
        static DirectoryInfo DirectoryInfo = Directory.CreateDirectory(FolderPath);
        /// <summary>
        /// 按文件路径备份
        /// </summary>
        /// <param name="path"></param>
        public static void BackupFile(string path)
        {
            if (File.Exists(path) == false)
            {
                return;
            }
            try
            {
                var dir = Directory.CreateDirectory(Path.Combine(DirectoryName, Path.GetFileNameWithoutExtension(path)));
                var copyPath = Path.Combine(dir.FullName, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                File.Copy(path, copyPath, true);
            }
            catch (Exception ex)
            {
                throw new Exception($"[2303051407]无法备份{path}。\n{ex.Message}");
            }
        }
        /// <summary>
        /// 从备份恢复出原graph，备份原文件所在的位置上的文件
        /// </summary>
        /// <param name="graph"></param>
        public static void BackupFile(FocusGraph graph)
        {
            var path = graph.FilePath;
            if (File.Exists(path) == false)
            {
                return;
            }
            try
            {
                var prevGraph = XmlIO.LoadGraph(path);
                if (graph.Equals(prevGraph))
                {
                    return;
                }
                var dir = Directory.CreateDirectory(Path.Combine(DirectoryName, Path.GetFileNameWithoutExtension(path)));
                var copyPath = Path.Combine(dir.FullName, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                File.Copy(path, copyPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[2303051407]无法备份{path}。\n{ex.Message}");
            }
        }
        /// <summary>
        /// 从备份恢复出原graph
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FocusGraph Restore(string path)
        {
            var graph = XmlIO.LoadGraph(path);
            if (graph.FilePath == path)
            {
                return graph;
            }
            if (File.Exists(graph.FilePath))
            {
                var prevGraph = XmlIO.LoadGraph(graph.FilePath);
                if (graph.Equals(prevGraph) == false)
                {
                    var dir = Directory.CreateDirectory(Path.Combine(DirectoryName, Path.GetFileNameWithoutExtension(graph.FilePath)));
                    var copyPath = Path.Combine(dir.FullName, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                    File.Copy(graph.FilePath, copyPath, true);
                }
            }
            XmlIO.SaveGraph(graph.FilePath, graph);
            File.Delete(path);
            return graph;
        }
        /// <summary>
        /// 通过文件路径的文件名查找同名文件夹下的所有备份，形成文件列表
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>文件列表</returns>
        public static List<FileInfo> GetBackupsList(string filePath)
        {
            List<FileInfo> result = new();
            var dir = Path.Combine(DirectoryName, Path.GetFileNameWithoutExtension(filePath));
            if (Directory.Exists(dir))
            {
                var dirInfo = new DirectoryInfo(dir);
                var fileList = dirInfo.EnumerateFiles();
                result = fileList.ToList();
            }
            return result;
        }
        /// <summary>
        /// 清空根目录
        /// </summary>
        public static void Clear()
        {
            if (MessageBox.Show("是否删除所有备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            else
            {
                MessageBox.Show("已删除所有备份。");
            }
            ZipFile.CreateFromDirectory(DirectoryName, Path.Combine(Path.GetDirectoryName(DirectoryName), DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分") + ".zip"));
            Directory.Delete(DirectoryName, true);
            DirectoryInfo = Directory.CreateDirectory(FolderPath);
        }
    }
}
