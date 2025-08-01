using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Model
{
    public class Match
    {
        public int Id { get; set; }
        public int OpponentId { get; set; }

        public DateTime? Date { get; set; }
        public bool IsWin { get; set; }
        public bool Marked { get; set; }

        public string Result { get; set; }
        public string Competition { get; set; }
        public string Notes { get; set; }
    }
}
