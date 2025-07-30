using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Model
{
    public class Opponent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Club { get; set; }
        public bool Marked { get; set; }
    }
}
