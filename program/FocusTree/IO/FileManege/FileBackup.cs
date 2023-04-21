using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace FocusTree.IO.FileManege
{
    public static class FileBackup
    {
        /// <summary>
        /// 根目录
        /// </summary>
        readonly static DirectoryInfo RootDirectoryInfo = Directory.CreateDirectory("backup");
        /// <summary>
        /// 根目录名称
        /// </summary>
        public static string RootDirectoryName { get { return RootDirectoryInfo.FullName; } }
        /// <summary>
        /// 对象根目录
        /// </summary>
        private static string DirectoryName<T>(this T obj) where T : IBackupable
        {
            var dir = Path.Combine(RootDirectoryName, obj.FileManageDirName);
            Directory.CreateDirectory(dir);
            return dir;
        }
        /// <summary>
        /// 对象的文件备份路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>root\obj's Root\objHash\dateTime</returns>
        private static string BackupPath<T>(this T obj) where T : IBackupable
        {
            var objHash = obj.GetHashString();
            return Path.Combine(obj.DirectoryName(), objHash, $"BK{DateTime.Now:yyyyMMddHHmmss}");
        }
        /// <summary>
        /// 从文件路径获取备份路径
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">要获取备份路径的文件路径</param>
        /// <returns></returns>
        private static string BackupPath<T>(string path) where T : IBackupable
        {
            var obj = XmlIO.LoadFromXml<T>(path);
            return obj.BackupPath();
        }
        /// <summary>
        /// 备份指定文件路径的obj（如果存在的话）,如果已存在备份文件则不作替换
        /// </summary>
        /// <param name="path">要备份的文件路径</param>
        public static void Backup<T>(string path) where T : IBackupable
        {
            try
            {
                var bkPath = BackupPath<T>(path);
                var bkDir = Path.GetDirectoryName(bkPath);
                if (Directory.Exists(bkDir))
                {
                    return;
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
        /// 通过文件路径的文件名查找同名文件夹下的所有备份，形成文件列表
        /// </summary>
        /// <param name="path">要查找的文件路径</param>
        /// <returns>文件列表(备份文件路径, 备份名)，列表第一元素是文件路径本身</returns>
        public static List<(string, string)> GetBackupsList<T>(this T obj, string path) where T : IBackupable
        {
            List<(string, string)> result = new();
            string objRootDir;
            if (obj.IsBackupFile(path))
            {
                objRootDir = Path.GetDirectoryName(Path.GetDirectoryName(path));
            }
            else
            {
                objRootDir = Path.GetDirectoryName(Path.GetDirectoryName(obj.BackupPath()));
            }
            if (Directory.Exists(objRootDir))
            {
                var dirs = new DirectoryInfo(objRootDir).GetDirectories();
                Array.Sort(dirs, delegate (DirectoryInfo x, DirectoryInfo y) { return x.LastWriteTime.CompareTo(y.LastWriteTime); });
                result.Add((path, Path.GetFileNameWithoutExtension(path)));
                var bkPathOfPath = BackupPath<T>(path);
                foreach (var bkdir in dirs)
                {
                    var dir = bkdir.GetFiles().FirstOrDefault();
                    if (dir == null)
                    {
                        bkdir.Delete(true);
                        continue;
                    }
                    string bkPath = dir.FullName;
                    var testbkdir = Path.GetDirectoryName(BackupPath<T>(bkPath));
                    if (testbkdir != bkdir.FullName)
                    {
                        bkdir.Delete(true);
                        continue;
                    }
                    if (testbkdir == Path.GetDirectoryName(bkPathOfPath))
                    {
                        continue;
                    }
                    var match = Regex.Match(Path.GetFileName(bkPath), "^BK(\\d{4})(\\d{2})(\\d{2})(\\d{2})(\\d{2})(\\d{2})$");
                    result.Add((bkPath, $"{match.Groups[1].Value}/{match.Groups[2].Value}/{match.Groups[3].Value} {match.Groups[4].Value}:{match.Groups[5].Value}:{match.Groups[6].Value}"));
                }
            }
            return result;
        }
        /// <summary>
        /// 删除当前备份
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">要删除的备份对象</param>
        public static void DeleteBackup<T>(this T obj) where T : IBackupable
        {
            var objRootDir = Path.GetDirectoryName(obj.BackupPath());
            if (!Directory.Exists(objRootDir) || MessageBox.Show("是否要删除当前备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            Directory.Delete(objRootDir, true);
        }
        /// <summary>
        /// 清空根目录并打成压缩包
        /// </summary>
        public static void Clear(string zipPath)
        {
            ZipFile.CreateFromDirectory(RootDirectoryName, zipPath);
            Directory.Delete(RootDirectoryName, true);
            Directory.CreateDirectory(RootDirectoryName);

            Process p = new();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = @" /select, " + zipPath;
            p.Start();
        }
        /// <summary>
        /// 查询文件是否是备份文件：位于备份子根目录下且具有备份文件名格式
        /// </summary>
        /// <param name="path">要查询的文件路径</param>
        /// <returns></returns>
        public static bool IsBackupFile<T>(this T obj, string path) where T : IBackupable
        {
            var match = Regex.Match(Path.GetFileName(path), "^BK(\\d){4}((\\d){2}){5}$");
            return match.Success && obj.DirectoryName() == Path.GetDirectoryName(Path.GetDirectoryName(path));
        }
    }
}
