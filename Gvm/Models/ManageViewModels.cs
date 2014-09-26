using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Gvm.Models
{
    public class IndexViewModel
    {
        public IList<UserLoginInfo> Logins { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }
    }
    
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Eski Şifre")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} en azından {2} karakter içermeli.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifreyi Doğrula")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifre doğrulaması ile uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }
}