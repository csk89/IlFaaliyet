using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class ClaimAuthorisation: BaseEntity
    {
        public ClaimAuthorisation()
        {
            DateCreated = DateTime.Now;
        }

        [DisplayName("Rol/Yetkili")]
        public string Claim { get; set; }
        public string ClaimType { get; set; }
        [DisplayName("Kaynak")]
        public string Resource { get; set; }
        [DisplayName("Aksiyon")]
        public string Action { get; set; }
    }
}
