using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class SerolojikTestler : BaseEntity
    {
        public SerolojikTestler()
        {
            DateCreated = DateTime.Now;
        }

        public int GonulluVerici{ get; set; }

        public bool HBsAg{ get; set; }
        public bool AntiHcv{ get; set; }
        public bool AntiHiv{ get; set; } 
        public bool Sifiliz { get; set; }
    }
}
