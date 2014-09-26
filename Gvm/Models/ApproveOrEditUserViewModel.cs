using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Turkok.Model;

namespace Gvm.Models
{
    public class ApproveOrEditUserViewModel
    {
        public string UserId { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Kullanıcnı Adı")]
        public string UserName { get; set; }
        [Display(Name = "Adı Soyadı")]
        public string AdiSoyadi { get; set; }
        [Display(Name = "Başvuru Tarihi")]
        public DateTime BasvuruTarihi { get; set; }
        [Display(Name = "Roller")]
        public IEnumerable<SelectListItem> Roller { get; set; }
        [Display(Name = "Kulanıcı Rolü")]
        public string SelectedRole { get; set; }
        [Display(Name = "Bir merkezde çalışmıyor mu")]
        public bool NotWorkingForUnit { get; set; }
    }
}