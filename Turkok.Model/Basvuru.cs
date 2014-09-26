using System;

namespace Turkok.Model
{
    public class Basvuru : BaseEntity
    {
        public Basvuru()
        {
            DateCreated = DateTime.Now;
            Active = true;
            Completed = false;
        }

        public ApplicationUser CreatedBy { get; set; }
        public ApplicationUser EditedBy { get; set; }
        public GonulluVericiMerkezi GonulluVericiMerkezi { get; set; }
        public GonulluVerici GonulluVerici { get; set; }

        public bool BifCiktisiTamamlandi { get; set; }
        public bool Completed { get; set; }
        public bool Active { get; set; }

        public bool IsComplete()
        {
            if (GonulluVerici != null)
            {
                if (GonulluVerici.GonulluVericiYakini != null &&
                    GonulluVerici.VericiIletisimBilgileri != null &&
                    GonulluVerici.Hastaliklar != null &&
                    GonulluVerici.SerolojikTestler != null &&
                    GonulluVerici.HematolojikTestler != null &&
                    GonulluVerici.Hastaliklar != null)
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}