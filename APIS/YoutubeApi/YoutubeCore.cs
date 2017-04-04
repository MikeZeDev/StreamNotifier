using Common;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace YoutubeApi
{
    public static class YoutubeCore
    {
        public static string CacheFolder;
        public static Icon Logo;
        public static string Streamtype = "Youtube";
        private static bool inited = false;

        public static string APIKEY = "AIzaSyAScLykggFOWzPUN-UQMxIwhoO8vMJCFq0";

        private static string URLPATTERN = @"(http(s)?://)?(?<subdomain>([\w\-]+))?(.)?youtube.com/channel/(?<channel>[a-zA-Z0-9_-]+)";
        private static string URLPATTERN2 = @"(http(s)?://)?(?<subdomain>([\w\-]+))?(.)?youtube.com/embed/live_stream[\?]channel=(?<channel>[a-zA-Z0-9_-]+)";

        private static string URLPATTERN3 = @"youtu(?:\.be|be\.com)/(?:(.*)v(/|=)|(.*/)?)(?<videoid>[a-zA-Z0-9_-]+)"; //v ou watch
        private static string URLPATTERN4 = @"(http(s)?://)?(?<subdomain>([\w\-]+))?(.)?youtube.com/user/(?<userid>[a-zA-Z0-9_-]+)"; //user
        private static string URLPATTERN5 = @"(http(s)?://)?(?<subdomain>([\w\-]+))?(.)?youtube.com/(?<userid>[a-zA-Z0-9_-]+)"; //user


        /*
                     CanHandleUrl("https://www.youtube.com/channel/UCxuOjfwreG5zgpRtjzDczAQ");
                     CanHandleUrl("gaming.youtube.com/embed/live_stream?channel=UCmSF7icqELNOLTeYo4ZaoTQ&amp;modestbranding=1");
                     CanHandleUrl("https://www.youtube.com/user/aypierre");
                     CanHandleUrl("https://www.youtube.com/watch?v=4VasWbxu9mA");
                     CanHandleUrl"(https://gaming.youtube.com/Siphano13#action=sponsor");//pattern5 
      */



        public static void StreamApiInitialize()
        {
            if (!inited)
            {
                CacheFolder = Path.Combine(Environment.CurrentDirectory, "cache", "Youtube");
                try
                {
                    Directory.CreateDirectory(CacheFolder);
                }
                catch
                {

                }

                Logo = Properties.Resources.Youtube;
                inited = true;

                //Read the API KEY
                //settings = new PluginSettings("Youtube");
               // settings.Load();

                //APIKEY = settings.GetValue("APIKEY");


            }
        }

        public static YoutubeStream Activate(string channelid)
        {
            return new YoutubeStream(channelid);
        }


        
        public static string CanHandleUrl(string url)
        {
            string result = "";

            //1) methode 1 ) On teste si on a un channel dans l'url

            result = ExtractChannelId(URLPATTERN, url);
            if (!String.IsNullOrEmpty(result))
            {
                return result;
            }

            result = ExtractChannelId(URLPATTERN2, url);
            if (!String.IsNullOrEmpty(result))
            {
                return result;
            }

            //2) Si c'est une vidéo ou un lien de type /user/JeanMouloud
            //on fait un GET de la page et on recherche    <meta itemprop = "channelId" content="UCi1NbRmVK32knWk3I7malKw">

            Regex r = new Regex(URLPATTERN3);
            Regex r2 = new Regex(URLPATTERN4);
            Regex r3 = new Regex(URLPATTERN5);
            if (r.IsMatch(url) || r2.IsMatch(url) || r3.IsMatch(url))
            {
                result = ExtractChannelId(url);
                return result;
            }   


          return result;
        }


        public static void ClearCache()
        {
            Helper.Empty(CacheFolder);
        }



        private static string ExtractChannelId(string pattern, string url)
        {
            string result = "";

            Regex r = new Regex(pattern);
            if (r.IsMatch(url))
            {
                MatchCollection mc = Regex.Matches(url, pattern);
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



        private static string ExtractChannelId(string url)
        {
            string result = "";

            try
            {

                Regex metaTag = new Regex(@"<meta\b[^>]*\bitemprop=[""]channelId[""][^>]*\bcontent=(['""]?)((?:[^,>""'],?){1,})\1[>]", RegexOptions.IgnoreCase);
                string Response = Helper.HttpGet(url);

                foreach (Match m in metaTag.Matches(Response))
                {
                    result = m.Groups[2].Value;
                }

                //if channel id not found try to get it from the url in the tag 
                //<meta property="og:url" content="https://gaming.youtube.com/user/XXXX">
                if (string.IsNullOrEmpty(result))
                {
                    Regex meta2 = new Regex(@"<meta\b[^>]*\bproperty=[""]og:url[""][^>]*\bcontent=(['""]?)((?:[^,>""'],?){1,})\1[>]", RegexOptions.IgnoreCase);
                    foreach (Match m in meta2.Matches(Response))
                    {
                        url = m.Groups[2].Value;
                    }

                    url = url.Replace("/gaming.", "/"); //Workaround : there is no channelID on the gaming subdomain, WTF


                    Response = Helper.HttpGet(url);
                    foreach (Match m in metaTag.Matches(Response))
                    {
                        result = m.Groups[2].Value;
                    }


                }


            }
            catch
            {

            }
            return result;


        }
    }


}
