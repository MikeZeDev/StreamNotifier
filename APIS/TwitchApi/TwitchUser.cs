using Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace TwitchApi
{
    public class TwitchUser
    {

        public string internalid;
        public string display_name;
        public string logo;
        public string url;
        public string localavatar;

        public TwitchUser(string channelid)
        {
            this.internalid = channelid;
        }

        public void GetUserInfos()
        {
            string url = "https://api.twitch.tv/kraken/users/" + this.internalid;
            string Response = Helper.HttpGet(url, TwitchCore.CustomHttpHeaders);

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
                string filename = "user_" + Path.GetFileName(new Uri(logo).AbsolutePath);
                this.localavatar = Path.Combine(TwitchCore.CacheFolder, filename);
                if (File.Exists(localavatar)) { return; }

                using (WebClient client = new WebClient())
                {
                    File.Delete(this.localavatar);
                    client.DownloadFile(this.logo, this.localavatar);
                    Helper.ScaleImageFile(this.localavatar, 80, 80);
                }

            }
            catch { }


        }

    }
}
