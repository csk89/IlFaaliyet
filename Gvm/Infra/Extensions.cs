using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace Gvm.Infra
{
    public static class Extensions
    {
        public static bool IsPasswordValid(this string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length < 6)
            {
                return false;
            }

            return true;
        }
        public static bool IsPhoneValid(this string str)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Substring(0, 1) == "0" || str.Trim().Length != 10 || str.IsNumeric() == false)
            {
                return false;
            }

            return true;
        }
        public static bool IsTcnoValid(this string str)
        {
            if (string.IsNullOrEmpty(str) || str.IsNumeric() == false || str.Trim().Length != 11)
            {
                return false;
            }

            return true;
        }
        public static bool IsNumeric(this string str)
        {
            long n;
            bool isNumeric = long.TryParse(str, out n);

            return isNumeric;
        }
        public static bool IsValidEmail(this string str)
        {
            try
            {
                var address = new MailAddress(str);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            return list.Contains(source);
        }
        public static int CalculateAge(this DateTime? bornDate)
        {
            var bday = (DateTime)bornDate;

            DateTime today = DateTime.Today;
            int age = today.Year - bday.Year;
            if (bday > today.AddYears(-age)) age--;

            return age;
        }
        public static string FirstCharUpper(this string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }
        public static string GetUserRole(this string username)
        {
            var roles = Roles.GetRolesForUser(username);

            if (roles.Length < 1) return "Rolü Yok";
            else return roles[0];
        }
    }
}