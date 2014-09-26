using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public enum Gender
    {
        [Display(Name="Erkek")]
        Erkek = 1,
        [Display(Name = "Kadın")]
        Kadin = 2
    }
}
