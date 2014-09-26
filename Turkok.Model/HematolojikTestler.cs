using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class HematolojikTestler : BaseEntity
    {
        public HematolojikTestler()
        {
            DateCreated = DateTime.Now;
        }

        public int GonulluVerici{ get; set; }

        public string Hemogram{ get; set; }
        public string KanGrubu{ get; set; }

    }
}
