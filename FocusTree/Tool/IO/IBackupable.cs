using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Tool.IO
{
    internal interface IBackupable
    {
        string BackupDirectory { get; }
    }
}
