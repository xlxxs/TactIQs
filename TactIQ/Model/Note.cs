using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Model
{
    public class Note
    {
        public string Text { get; internal set; }
        public string Type { get; internal set; }
        public bool IsValid { get; internal set; }
        public string Category { get; internal set; }
    }
}
