using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Model
{
    public class Match
    {
        public string? Date { get; set; }
        public bool IsWin { get; set; }

        public string Result { get; set; }
        public string Competition { get; set; }
        public string Notes { get; set; }
    }
}
