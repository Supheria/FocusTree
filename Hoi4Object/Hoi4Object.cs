using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4Object
{
    /// <summary>
    /// 钢4数据对象
    /// </summary>
    public abstract class Hoi4Object
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; } = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        public string Caption { get; private set; } = string.Empty;
    }
}
