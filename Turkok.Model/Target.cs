using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class Target
    {

        public Target()
        {
            CreationDate = DateTime.Now;
        }
        public int Id { get; set; }
        [DisplayName("Hedef Adı")]
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        [DisplayName("Oluşturulma Zamanı")]
        public DateTime CreationDate { get; set; }
        [DisplayName("Hedef No")]
        public string TargetId { get; set; }
        public ICollection<Action> Actions { get; set; }
    }
}
