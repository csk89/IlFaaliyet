using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class GonulluVericiMerkezi : BaseEntity
    {
        public GonulluVericiMerkezi()
        {
            DateCreated = DateTime.Now;
        }
        
        [DisplayName("Ad")]
        public string Adi { get; set; }
        
        [Required]
        public int SehirId { get; set; }

        [NotMapped]
        public string SehirAdi { get; set; }

        [Required]
        [MinLength(3)]
        public string Adres { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Telefon { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAdresi { get; set; }
    }
}
