using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Turkok.Model
{
    public class Document : BaseEntity
    { 
        public Document()
        {
            DateCreated = DateTime.Now;
        }

        [Display(Name = "Başlık")]
        [Required]
        public string Title{ get; set; }

        [Display(Name = "İçerik")]
        [AllowHtml]
        public string Description{ get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Yayınlama Zamanı")]
        public DateTime PublishDate { get; set; } 

        public ApplicationUser CreateBy { set; get; }

        public ApplicationUser EditedBy { set; get; }

        public ApplicationUser DeletedBy { set; get; }

        public ICollection<Attachment> Attachments { get; set; }
        
    }
}
