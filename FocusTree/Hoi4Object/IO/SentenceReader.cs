using FocusTree.Hoi4Object.IO.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Hoi4Object.IO
{
    internal class SentenceReader
    {
        public static string FormatEffectSentence(string sentence)
        {
            var result = FormatRawEffectSentence.Formatter(sentence, out string formatted);
        }
    }
}
