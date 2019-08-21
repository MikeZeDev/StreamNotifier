using Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace DailymotionApi
{
    public class DailyMotionUser
	{
		public string id;
		public string screenname;
		public string avatar_80_url;
		public string localavatar;
		//public P


		public DailyMotionUser(string userid)
		{
            id = userid;
		}

        public DailyMotionUser(XmlNode xML)
        {
            id = (xML["StreamKey"] == null) ? "_blank" : xML["StreamKey"].InnerText;
            screenname = (xML["Name"] == null) ? "_blank" : xML["Name"].InnerText;
            localavatar = (xML["Picture"] == null) ? "_blank" : xML["Picture"].InnerText;
            localavatar = Encoding.UTF8.GetString(Convert.FromBase64String(localavatar));



        }

        public void GetInfos()
		{
			string url = "https://api.dailymotion.com/user/" + id + "?fields=id,screenname,avatar_80_url";
            string Response = Helper.HttpGet(url);

            try
            {
                JsonConvert.PopulateObject(Response, this);
            }
            catch (Exception)
            {

               
            }

			//download user icon
			try
			{
				string filename = "avatar_" + id + "_" + Path.GetFileName(new Uri(avatar_80_url).AbsolutePath);
                localavatar = Path.Combine(DailyMotionCore.CacheFolder, filename);
				if (File.Exists(localavatar)) {return;}

				using (WebClient client = new WebClient())
				{
					//File.Delete(localavatar);
					client.DownloadFile(avatar_80_url, localavatar);
                    Helper.ScaleImageFile(localavatar, 80, 80);

                }

            }
			catch { }


		}

	}
}
