using System;
using System.ComponentModel.DataAnnotations;

namespace Turkok.Model
{
    public class Attachment :BaseEntity
    {
        public Attachment()
        {
            DateCreated = DateTime.Now;
        }
         
        public string Name { set; get; }
         
        public DateTime? DateRemoved { set; get; }
         
        public string Path { get; set; }

        public ApplicationUser CreatedBy{ get; set; }

    }
}
