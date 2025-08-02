using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Model
{
    public class Note
    {
        public int Id { get; set; }
        public int OpponentId { get;  set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Marked { get; set; }

    }
}
