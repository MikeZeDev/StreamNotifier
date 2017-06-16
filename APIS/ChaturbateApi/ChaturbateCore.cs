using Common;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace ChaturbateApi
{
    public class ChaturbateCore
    {
        public static string CacheFolder;
        public static Icon Logo;
        public static bool inited = false;
        public static string Streamtype = "Chaturbate";

        private static string URLPATTERN = @"(http(s)?://)?(?<subdomain>([\w\-]+))?(.)?chaturbate.com/(?<username>[a-zA-Z0-9]+)";



        public static void StreamApiInitialize()
        {
            if (!inited)
            {
                CacheFolder = Path.Combine(Environment.CurrentDirectory, "cache", "Chaturbate");
                try
                {
                    Directory.CreateDirectory(CacheFolder);
                }
                catch
                {

                }

                Logo = Properties.Resources.Chaturbate;
                inited = true;
            }
        }

        
        public static ChaturbateStream Activate(string id)
        {
            return new ChaturbateStream(id);
        }



        public static string CanHandleUrl(string url)
        {
            string result = "";

            Regex r = new Regex(URLPATTERN);
            if (r.IsMatch(url))
            {
                MatchCollection mc = Regex.Matches(url, URLPATTERN);
                foreach (Match match in mc)
                {
                    try
                    {
                        result = match.Groups["username"].Value;
                        return result;
                    }
                    catch { }
                }


            }

            return result;
        }

        public static void ClearCache()
        {
            Helper.Empty(CacheFolder);
        }


    }
}
