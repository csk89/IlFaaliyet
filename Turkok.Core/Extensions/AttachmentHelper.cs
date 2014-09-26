using System;
using System.IO;
using System.Web.Hosting;

namespace Turkok.Core.Extensions
{
    public static class AttachmentHelper
    {
        public static string CreateGuidFilename(string file)
        {
            var guidName = Guid.NewGuid().ToString();

            var extension = Path.GetExtension(file);

            var newName = String.Format("{0}{1}", guidName, extension);

            return newName;
        }

        public static string GetUploadPath(string filename, string serverPath)
        {
            return HostingEnvironment.MapPath(string.Concat(serverPath, filename)); ;
        }

        public static string GetAccessPath(string filename, string accessPath)
        {
            return string.Format("{0}{1}", accessPath, filename);
        }
    }
}