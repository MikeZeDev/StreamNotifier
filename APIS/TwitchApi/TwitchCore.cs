using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TwitchApi
{
    public static class TwitchCore
    {
        public static string CacheFolder;
        public static Icon Logo;
        public static string Streamtype = "Twitch";
        private static bool inited = false;
        public static Encoding enc = Encoding.Unicode;

        private static string CLIENTID = "rgjhr76bqu32xzpwz8ai05134hi3jqe"; //Application ID, registered on Twitch

        private static string URLPATTERN = @"(http(s)?://)?(?<subdomain>([\w\-]+))?(.)?twitch.tv/(?<channel>[_a-zA-Z0-9]{3,24})";

        public static Dictionary<string, string> CustomHttpHeaders = new Dictionary<string, string>();




        public static void StreamApiInitialize()
        {
            if (!inited)
            {
                CacheFolder = Path.Combine(Environment.CurrentDirectory, "cache", "Twitch");
                try
                {
                    Directory.CreateDirectory(CacheFolder);
                }
                catch
                {

                }

                Logo = Properties.Resources.Twitch;
                inited = true;

                //Set the Client-ID needed for HTTP/S requests
                CustomHttpHeaders.Add("Client-ID", CLIENTID);



            }
        }

        public static TwitchStream Activate(string id)
        {
            return new TwitchStream(id);
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
