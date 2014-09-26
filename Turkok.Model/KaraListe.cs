using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class KaraListeGonulluVerici : BaseEntity
    {
        public KaraListeGonulluVerici()
        {
            DateCreated = DateTime.Now;
        }

        public GonulluVerici GonulluVerici{ get; set; }

        public ApplicationUser MarkedBy{ get; set; }

    }
}
