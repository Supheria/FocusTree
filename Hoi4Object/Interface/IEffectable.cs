using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoi4Object.Interface
{
    internal interface IEffectable
    {
        string Trigger { get; set; }
        string[] Targets { get; set; } 
    }
}
