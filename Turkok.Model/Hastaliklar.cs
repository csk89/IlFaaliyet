using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class Hastaliklar : BaseEntity
    {
        public Hastaliklar()
        {
            DateCreated = DateTime.Now;
        }

        public int GonulluVerici { set; get; }

        public bool SurekliTedaviOlmasiGereklimi { set; get; }

        public bool HepatitC{ get; set; }

        public bool HepatitB{ get; set; }

        public bool Sifiliz{ get; set; }

        public bool Kanser{ get; set; }

        public bool AidsHiv{ get; set; }
    }
}
