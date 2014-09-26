using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Turkok.Model
{
    public class Announcement : BaseEntity
    {
        public Announcement()
        {
            DateCreated = DateTime.Now;
        }

        [StringLength(512)]
        [Display(Name = "Başlık")]
        public string Title { get; set; }

        [AllowHtml]
        [DataType(DataType.Html)]
        [Display(Name = "İçerik")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Yayınlama Zamanı")]
        public DateTime PublishDate { get; set; }

        public DateTime? DateEdited { get; set; }

        public ICollection<Attachment> Attachments { get; set; }

        public ApplicationUser CreatedBy { set; get; }

        public ApplicationUser EditedBy { set; get; }

        public ApplicationUser DeletedBy { set; get; }

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            var item = (Announcement)obj;

            if (item.Id == Id)
            {
                return true;
            }

            return false;
        }
    }
}
