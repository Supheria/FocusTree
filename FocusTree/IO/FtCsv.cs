using CSVFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.IO
{
    internal class FtCsv
    {
        /// <summary>
        /// 读取Csv文件，获得 string[][] 二维文本
        /// </summary>
        /// <param name="filePath">Csv文件路径</param>
        /// <returns>string[][] csv纯文本表格</returns>
        public static string[][] ReadCsv(string filePath)
        {
            string[][] data;
            try
            {
                CSVReader csvData = CSVReader.FromFile(filePath);
                data = csvData.ToArray(); //作为二维数组返回
            }
            catch (Exception ex)
            {
                throw new Exception($"读取CSV文件失败：{ex.Message}");
            }
            return data;
        }
    }
}
