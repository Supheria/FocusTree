using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.IO.FileManege
{
    public static class Cache
    {
        /// <summary>
        /// 根缓存目录
        /// </summary>
        static string DirectoryName<T>(this T obj) where T : IFileManageable
        {
            var dir = Path.Combine(RootDirectoryInfo.FullName, obj.FileManageDirectory);
            Directory.CreateDirectory(dir);
            return dir;
        }
        static DirectoryInfo RootDirectoryInfo = Directory.CreateDirectory("cache");
        static string FileExtension = string.Empty;
        /// <summary>
        /// 保存到缓存文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="FileNameWithoutExtension">纯文件名</param>
        /// <returns></returns>
        public static string GetCachePath<T>(this T obj, string FileNameWithoutExtension) where T : IFileManageable
        {
            var cachePath = Path.Combine(obj.DirectoryName(), FileNameWithoutExtension);
            cachePath = Path.ChangeExtension(cachePath, FileExtension);
            return cachePath;
        }
        /// <summary>
        /// 清空缓存文件
        /// </summary>
        public static void ClearCache<T>(this T obj) where T : IFileManageable
        {
            Directory.Delete(obj.DirectoryName(), true);
        }
        public static void Clear()
        {
            Directory.Delete(RootDirectoryInfo.FullName, true);
        }
    }
}
