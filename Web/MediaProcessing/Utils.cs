using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace VideoShow.Streaming.Services
{
    internal static class Utils
    {
        /// <summary>
        /// return full path of filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetConfigFile(string filename)
        {
            string location = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
            return Path.Combine(location, Path.Combine("ExpressionMediaConfig", filename));
        }

        //  --------------------------- CopyStream ---------------------------
        /// <summary>
        ///   Copies data from a source stream to a target stream.</summary>
        /// <param name="source">
        ///   The source stream to copy from.</param>
        /// <param name="target">
        ///   The destination stream to copy to.</param>
        public static void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 400000;//0x2000; // 8k
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
            {
                target.Write(buf, 0, bytesRead);
            }           
        }
    }
}
