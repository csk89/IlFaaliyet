using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class Target
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string TargetId { get; set; }
        public ICollection<Action> Actions { get; set; }
    }
}
