using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Turkok.Model;

namespace Gvm.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Kod")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Tarayıcıyı hatırla")]
        public bool RememberBrowser { get; set; }

        [Display(Name = "Oturum açık kalsın")]
        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-posta")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            DateCreated = DateTime.Now;
        }

        public DateTime DateCreated { get; set; }

        [Required]
        [Display(Name = "Tc Numarası")]
        public string TcNo { get; set; }
        [Required]
        [Display(Name = "Cilt No")]
        public int? CiltNo { get; set; }
        [Required]
        [Display(Name = "Aile Sıra No")]
        public int? AileSiraNo { get; set; }
        [Required]
        [Display(Name = "(Birey) Sira No")]
        public int? BireySiraNo { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "{0} en azından {2} karakter olmalı.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Doğrulaması")]
        [Compare("Password", ErrorMessage = "Şifre ve Şifre doğrulaması aynı olmalı.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "İş/Ev Telefonu")]
        public string WorkPhone { get; set; }

        [Required]
        [Display(Name = "Cep Telefonu")]
        public string CellPhone { get; set; }
        
        public List<GonulluVericiMerkezi> Units { get; set; }

        [Display(Name = "Bir gönüllü veri merkezinde çalışmıyorum.")]
        public bool NotWorkingForUnit { get; set; }

        public int? UnitId { get; set; }

        public UserRoleInUnitDropdown? UserRoleInUnit { get; set; }
    }

    public enum UserRoleInUnitDropdown
    {
        [Display(Name = "Merkezdeki rolünüz nedir?")]
        NotChosen,
        [Display(Name = "Merkez Yöneticisi")]
        UnitManager = 1,
        [Display(Name = "Merkez Çalışanı")]
        UnitEmployee = 2
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} en azından {2} karakter olmalı.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Doğrulaması")]
        [Compare("Password", ErrorMessage = "Şifre ve Şifre doğrulaması aynı olmalı.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
    }
}
