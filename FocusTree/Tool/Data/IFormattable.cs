using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Tool.Data
{
    public interface IFormattable
    {
        /// <summary>
        /// 获取格式化数据
        /// </summary>
        /// <returns>格式化数据</returns>
        public FormattedData Format();
        /// <summary>
        /// 复原格式化数据
        /// </summary>
        /// <param name="data">格式化数据</param>
        public void Deformat(FormattedData IData);
    }
}
