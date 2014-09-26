using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Gvm.Infra
{
    public static class Utils
    {
        public static void SendEmail(string from, string to, string subject, string body)
        {
            var fromAddress = new MailAddress(from);
            var toAddress = new MailAddress(to);

            var message = new MailMessage(fromAddress, toAddress)
                              {
                                  Subject = subject,
                                  Body = body,
                                  IsBodyHtml = true
                              };

            var client = new SmtpClient();
            client.Send(message);
        }
        
        public static string GetUniqueKey(int size)
        {
            //const string a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const string a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            //
            var chars = a.ToCharArray();
            var data = new byte[1];
            var crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            //
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            //
            var result = new StringBuilder(size);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            //
            return result.ToString();
        }
    }
}


