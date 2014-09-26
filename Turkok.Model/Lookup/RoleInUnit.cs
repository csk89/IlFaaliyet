using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model.Lookup
{
    public enum RoleInUnit
    {
        [Display(Name = "Merkez Yöneticisi")]
        UnitManager = 1,
        [Display(Name = "Merkez Çalışanı")]
        UnitEmployee = 2
    }
}
