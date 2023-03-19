using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4Object.Objects
{
    /// <summary>
    /// 国家属性类
    /// </summary>
    public abstract class StateAtrribute : Hoi4Object
    {
        /// <summary>
        /// 实施者
        /// </summary>
        public string Trigger { get; private set; } = string.Empty;
        /// <summary>
        /// 承受着
        /// </summary>
        public string Target { get; private set; } = string.Empty;
        public static string _NAME_VALUE_SPLITTER_ = ": ";
    }
}
