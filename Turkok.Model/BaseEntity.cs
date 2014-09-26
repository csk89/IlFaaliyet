using System; 

namespace Turkok.Model
{
    public abstract class BaseEntity
    { 
        public int Id { get; set; }
         
        public bool IsDeleted { get; set; }
         
        public DateTime DateCreated { set; get; }
    }
}
