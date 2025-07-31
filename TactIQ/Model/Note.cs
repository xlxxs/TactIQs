using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Model
{
    public class Note
    {
        public int Id { get; internal set; }
        public int OpponentId { get; internal set; }
        public string Content { get; internal set; }
        public string Type { get; internal set; }
        public string Category { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }
}
