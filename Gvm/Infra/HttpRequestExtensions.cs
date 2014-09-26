using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Gvm.Infra
{
    public static class HttpRequestBaseExtensions
    {

        public static string ToRaw(this HttpRequestBase request)
        {
            var writer = new StringWriter();
             
            WriteStartLine(request, writer);
            WriteHeaders(request, writer);
            WriteBody(request, writer);

            return writer.ToString();
        }

        public static string GetBody(this HttpRequestBase request)
        {
            var writer = new StringWriter();
            WriteBody(request, writer);

            return writer.ToString();
        }

        private static void WriteStartLine(HttpRequestBase request, StringWriter writer)
        {
            const string SPACE = " ";

            writer.Write(request.HttpMethod);
            writer.Write(SPACE + request.Url);
            writer.WriteLine(SPACE + request.ServerVariables["SERVER_PROTOCOL"]);
        }


        private static void WriteHeaders(HttpRequestBase request, StringWriter writer)
        {
            foreach (string key in request.Headers.AllKeys)
            {
                writer.WriteLine(string.Format("{0}: {1}", key, request.Headers[key]));
            }

            writer.WriteLine();
        }


        private static void WriteBody(HttpRequestBase request, StringWriter writer)
        {
            var reader = new StreamReader(request.InputStream);

            try
            {
                string body = reader.ReadToEnd();
                writer.WriteLine(body);
            }
            finally
            {
                reader.BaseStream.Position = 0;
            }
        }
    }
}