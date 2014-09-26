using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Turkok.Model.Lookup;

namespace Turkok.Model
{
    public class IletisimBilgileri : BaseEntity
    {
        public IletisimBilgileri()
        {
            DateCreated = DateTime.Now;
        }

        public int GonulluVerici { set; get; }

        public string EvAdresi { get; set; }

        public string IsAdresi { get; set; }

        public string EvTelefonu { get; set; }

        public string IsTelefonu { get; set; }

        public string CepTelefonu { get; set; }

        public string Email { get; set; }


    }
}
