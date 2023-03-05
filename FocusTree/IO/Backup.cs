using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.IO
{
    internal class Backup
    {
        public static string DirectoryName
        {
            get { return DirectoryInfo.FullName; }
        }
        static DirectoryInfo DirectoryInfo = Directory.CreateDirectory("backups");
        public static void BackupFile(string path)
        {
            if (File.Exists(path) == false)
            {
                return;
            }
            try
            {
                var copyPath = Path.Combine(DirectoryName, DateTime.Now.ToString("yyyyMMddHHmmss ") + Path.GetFileName(path));
                File.Copy(path, copyPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[2303051407]无法备份{path}。\n{ex.Message}");
            }
        }
        public static void Clear()
        {
            Directory.Delete(DirectoryName, true);
            DirectoryInfo = Directory.CreateDirectory("backups");
        }
    }
}
