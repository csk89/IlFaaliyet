using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class AuthorisationResource
    {
        public string Name { get; set; }
        public List<ResourceAction> Actions { get; set; }
    }
}
