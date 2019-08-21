using Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace TwitchApi
{

    /*
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

        public TwitchChannel(XmlNode xML)
        {

            internalid = (xML["StreamKey"] == null) ? "_blank" : xML["StreamKey"].InnerText;
            display_name = (xML["Name"] == null) ? "_blank" : xML["Name"].InnerText;

            localavatar = (xML["Picture"] == null) ? "_blank" : (xML["Picture"].InnerText);
            localavatar = Encoding.UTF8.GetString(Convert.FromBase64String(localavatar));

            status = (xML["Title"] == null) ? "_blank" : xML["Title"].InnerText;
            status = Encoding.UTF8.GetString(Convert.FromBase64String(status));

            url = (xML["Url"] == null) ? "_blank" : (xML["Url"].InnerText);
            url = Encoding.UTF8.GetString(Convert.FromBase64String(url));



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



    }*/
}
