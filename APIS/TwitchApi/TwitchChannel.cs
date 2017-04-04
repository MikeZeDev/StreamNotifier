using Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace TwitchApi
{
    public class TwitchChannel
    {
        public string internalid;
        public string display_name;
        public string logo;
        public string url;
        public string localavatar;
        public string status;

        public TwitchChannel(string channelid)
        {
            this.internalid = channelid;
        }

        public void GetChannelInfos(bool init = false)
        {
            string url = String.Format("https://api.twitch.tv/kraken/channels/{0}", internalid);
            string Response = Helper.HttpGet(url, TwitchCore.CustomHttpHeaders);

            try
            {
                JsonConvert.PopulateObject(Response, this);
            }
            catch (Exception)
            {


            }


            if (init)
            { 
                //download user icon
                try
                {
                    string filename = "channel_" + this.internalid + "_" + Path.GetFileName(new Uri(logo).AbsolutePath);
                    this.localavatar = Path.Combine(TwitchCore.CacheFolder, filename);
                    if (File.Exists(localavatar)) { return; }

                    using (WebClient client = new WebClient())
                    {
                        File.Delete(this.localavatar);
                        client.DownloadFile(this.logo, this.localavatar);
                        Helper.ScaleImageFile(this.localavatar,80,80);
                    }

                }
                catch { }

            }
        }



    }
}
