using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Turkok.Model
{
    public class GonulluVerici : BaseEntity
    {
        public GonulluVerici()
        {
            DateCreated = DateTime.Now;
        }

        public bool KaraListedemi { set; get; }

        public string AdiSoyadi { get; set; }
        public string TcKimlikNo { get; set; }
        public DateTime DogumTarihi { get; set; }
        public int Yas { get; set; }
        public Gender Cinsiyet { get; set; } 
        public int GonulluVericiMerkezi{ get; set; } 
        public virtual IletisimBilgileri VericiIletisimBilgileri { get; set; }
        public virtual GonulluVericiYakini GonulluVericiYakini{ get; set; }
        
        public virtual Hastaliklar Hastaliklar { get; set; }
        public string Isbt128VericiNumarasi { get; set; }
        public virtual SerolojikTestler SerolojikTestler { get; set; }
        public virtual HematolojikTestler HematolojikTestler { get; set; }
          
        public int Age()
        {
            DateTime today = DateTime.Today;
            int age = today.Year - DogumTarihi.Year;
            if (DogumTarihi > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        // TODO : Uygun bulunan GV adaylari kan testleri etc. 
    }
}
