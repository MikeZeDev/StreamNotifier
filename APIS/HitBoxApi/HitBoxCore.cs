using System;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using Common;

namespace HitBoxApi

{
    public static class HitBoxCore
    {
        public static string CacheFolder;
        public static Icon Logo;
        public static string Streamtype = "HitBox";
        private static bool inited = false;
        public static string[] CDNS = { "http://edge.bf.hitbox.tv", "http://edge.fra.hitbox.tv", "http://edge.sf.hitbox.tv", "http://edge.vie.hitbox.tv" };

        private static string URLPATTERN = @"(http(s)?://)?(?<subdomain>([\w\-]+))?(.)?hitbox.tv/(?<channel>[a-zA-Z0-9]+)";


        public static void StreamApiInitialize()
        {
            if (!inited)
            {
                CacheFolder = Path.Combine(Environment.CurrentDirectory, "cache", "HitBox");
                try
                {
                    Directory.CreateDirectory(CacheFolder);
                }
                catch
                {

                }

                Logo = Properties.Resources.HitBox;
                inited = true;

            }
        }

        public static HitBoxStream Activate(string id)
        {
            return new HitBoxStream(id);
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
                        result = match.Groups["channel"].Value;
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
