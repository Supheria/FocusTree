namespace FocusTree.IO.FileManege
{
    internal class AutoSave
    {
        /// <summary>
        /// 根目录
        /// </summary>
        public static string DirectoryName
        {
            get { return DirectoryInfo.FullName; }
        }
        static string FolderPath = "backup\\AutoSave";
        static DirectoryInfo DirectoryInfo = Directory.CreateDirectory(FolderPath);
    }
}
