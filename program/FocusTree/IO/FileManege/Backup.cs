using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace FocusTree.IO.FileManege
{
    internal static class Backup
    {
        /// <summary>
        /// 根目录
        /// </summary>
        readonly static DirectoryInfo RootDirectoryInfo = Directory.CreateDirectory("backup");
        /// <summary>
        /// 子根目录
        /// </summary>
        public static string SubRootDirectoryName { get { return Path.Combine(RootDirectoryInfo.FullName, "backups"); } }
        /// <summary>
        /// 对象根目录
        /// </summary>
        private static string DirectoryName<T>(this T obj) where T : IBackupable
        {
            var dir = Path.Combine(SubRootDirectoryName, obj.FileManageDirName);
            Directory.CreateDirectory(dir);
            return dir;
        }
        /// <summary>
        /// 文件备份路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path">what fileName is of</param>
        /// <returns>root\subRoot\obj's Root\fileName\objHash\dateTime</returns>
        private static string BackupPath<T>(this T obj, string path) where T : IBackupable
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var objHash = obj.GetHashString();
            return Path.Combine(obj.DirectoryName(), fileName, objHash, $"BK{DateTime.Now:yyyyMMddHHmmss}");
        }
        /// <summary>
        /// 备份指定文件路径的obj（如果存在的话）
        /// </summary>
        /// <param name="path">要备份的文件路径</param>
        public static void BackupFile<T>(string path) where T : IBackupable
        {
            try
            {
                var objToBk = XmlIO.LoadFromXml<T>(path);
                var bkPath = objToBk.BackupPath(path);
                var bkDir = Path.GetDirectoryName(bkPath);
                if (Directory.Exists(bkDir))
                {
                    Directory.Delete(bkDir, true);
                }
                Directory.CreateDirectory(bkDir);
                File.Copy(path, bkPath, true);
            }
            catch (Exception ex)
            {
                throw new Exception($"[2303051407]无法备份{path}。\n{ex.Message}");
            }
        }
        /// <summary>
        /// 将当前是备份状态的obj恢复到文件路径后删除备份，并备份文件路径的原obj
        /// </summary>
        /// <param name="path">要恢复到的文件路径</param>
        public static void RestoreBackup<T>(this T obj, string path) where T : IBackupable
        {
            BackupFile<T>(path);
            obj.SaveToXml(path);
            var objHashDir = Path.GetDirectoryName(obj.BackupPath(path));
            Directory.Delete(objHashDir, true);
        }
        /// <summary>
        /// 通过文件路径的文件名查找同名文件夹下的所有备份，形成文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns>文件列表</returns>
        public static List<string> GetBackupsList<T>(this T obj, string path) where T : IBackupable
        {
            List<string> result = new();
            string fileNameDir;
            if (IsBackupFile(path))
            {
                fileNameDir = Path.GetDirectoryName(Path.GetDirectoryName(path));
            }
            else
            {
                fileNameDir = Path.GetDirectoryName(Path.GetDirectoryName(obj.BackupPath(path)));
            }
            if (Directory.Exists(fileNameDir))
            {
                var root = new DirectoryInfo(fileNameDir);
                var dirs = root.GetDirectories();
                foreach (var dir in dirs)
                {
                    var file = dir.GetFiles();
                    result.Add(file.First().FullName);
                }
            }
            return result;
        }
        /// <summary>
        /// 删除当前备份
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">要删除的备份对象</param>
        /// <param name="path">有备份的文件路径</param>
        public static void DeleteBackup<T>(this T obj, string path) where T : IBackupable
        {
            var toDeleteDir = Path.GetDirectoryName(obj.BackupPath(path));
            if (!Directory.Exists(toDeleteDir) || MessageBox.Show("是否要删除当前备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            Directory.Delete(toDeleteDir, true);
        }
        /// <summary>
        /// 清空根目录并打成压缩包
        /// </summary>
        public static void Clear()
        {
            FolderBrowserDialog folderBrowser = new();
            folderBrowser.Description = "选择要打包到文件夹";
            if (folderBrowser.ShowDialog() == DialogResult.Cancel) { return; }
            var zipPath = Path.Combine(folderBrowser.SelectedPath, DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒") + ".zip");
            ZipFile.CreateFromDirectory(SubRootDirectoryName, zipPath);
            Directory.Delete(SubRootDirectoryName, true);
            Directory.CreateDirectory(SubRootDirectoryName);

            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = @" /select, " + zipPath;
            p.Start();
        }
        /// <summary>
        /// 查询文件是否是备份文件：位于备份子根目录下且具有备份文件名格式
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsBackupFile(string path)
        {
            var match = Regex.Match(Path.GetFileName(path), "^BK(\\d){4}((\\d){2}){5}$");
            return match.Success && Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(path)))) == SubRootDirectoryName;
        }
    }
}
