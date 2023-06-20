namespace FocusTree.IO.FileManage
{
    public static class FileCache
    {
        /// <summary>
        /// 根目录
        /// </summary>
        static DirectoryInfo RootDirectoryInfo = Directory.CreateDirectory("cache");
        /// <summary>
        /// 对象根目录
        /// </summary>
        private static string DirectoryName<T>(this T obj) where T : IFileManageable
        {
            var dir = Path.Combine(RootDirectoryInfo.FullName, obj.FileManageDirName);
            Directory.CreateDirectory(dir);
            return dir;
        }
        /// <summary>
        /// 保存到缓存文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="FileNameWithoutExtension">纯文件名</param>
        /// <returns></returns>
        public static string GetCachePath<T>(T obj, string FileNameWithoutExtension) where T : IFileManageable
        {
            var cachePath = Path.Combine(obj.DirectoryName(), FileNameWithoutExtension);
            return cachePath;
        }
        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public static void ClearCache<T>(T obj) where T : IFileManageable
        {
            var dir = obj?.DirectoryName();
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }
        /// <summary>
        /// 清空根目录
        /// </summary>
        public static void Clear()
        {
            Directory.Delete(RootDirectoryInfo.FullName, true);
        }
    }
}
