using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class GonulluVericiYakini : BaseEntity
    {
        public GonulluVericiYakini()
        {
            DateCreated = DateTime.Now;
        }
        public string AdiSoyadi{ get; set; }

        public int GonulluVerici{ get; set; }

        public virtual IletisimBilgileri IletisimBilgileri { set; get; }
    }
}
