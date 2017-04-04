using Common;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace DailymotionApi
{
    public static class DailyMotionCore
	{

		public static string CacheFolder;
		public static Icon Logo;
		public static bool inited = false;
		public static string Streamtype = "DailyMotion";

        private static string URLPATTERN = @"(http(s)?://)?(?<subdomain>([\w\-]+))?(.)?dailymotion.com/(video|live)/(?<media_id>[a-zA-Z0-9]+)";



        public static void StreamApiInitialize()
		{
			if (!inited)
			{
				CacheFolder = Path.Combine(Environment.CurrentDirectory, "cache", "DailyMotion");
				try
				{
					Directory.CreateDirectory(CacheFolder);
				}
				catch
				{

				}

				Logo = Properties.Resources.Dailymotion;
				inited = true;
			}
		}

        public static DailyMotionStream Activate(string id)
        {
            return new DailyMotionStream(id);
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
                        result = match.Groups["media_id"].Value;
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
