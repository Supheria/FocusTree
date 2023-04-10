using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FocusTree.IO.FileManege
{
    internal interface IBackupable : IFileManageable, IXmlSerializable
    {
        /// <summary>
        /// 获取对象哈希值字符串
        /// </summary>
        /// <returns></returns>
        string GetHashString();
    }
}
