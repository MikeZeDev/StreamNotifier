using Common;
using System;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Text;

namespace YoutubeApi
{
    public class YoutubeChannel
    {
        public string channelid;

        public string channel_description;
        public string live_description;
        public string logo;
        public string localavatar;
        public string title;

        public YoutubeChannel(string channelid)
        {
            this.channelid = channelid;
        }

        public YoutubeChannel(XmlNode xML)
        {
            channelid = (xML["StreamKey"] == null) ? "_blank" : xML["StreamKey"].InnerText;

            localavatar = (xML["Picture"] == null) ? "_blank" : (xML["Picture"].InnerText);
            localavatar = Encoding.UTF8.GetString(Convert.FromBase64String(localavatar));

            title = (xML["Name"] == null) ? "_blank" : xML["Name"].InnerText;

            live_description = (xML["Title"] == null) ? "_blank" : (xML["Title"].InnerText);
            live_description = Encoding.UTF8.GetString(Convert.FromBase64String(live_description));

            /*
            display_name = (xML["Name"] == null) ? "_blank" : xML["Name"].InnerText;

      

            */


        }

        public void GetChannelInfos(bool init = false)
        {
            string url = String.Format("https://www.googleapis.com/youtube/v3/channels?part=snippet&id={0}&key={1}", channelid, YoutubeCore.APIKEY);
            string Response = Helper.HttpGet(url);

            try
            {
                //JsonConvert.PopulateObject(Response, this);
                JObject obj = JObject.Parse(Response);

                JObject item = (JObject) obj["items"][0]["snippet"];

                logo = (string)item["thumbnails"]["high"]["url"];
                channel_description = (string)item["description"];
                title = (string)item["title"];
                live_description = "";


            }
            catch (Exception)
            {


            }


            if (init)
            {
                //download user icon
                try
                {
                    string filename = "channel_" + this.channelid + "_" + Path.GetFileName(new Uri(logo).AbsolutePath);
                    this.localavatar = Path.Combine(YoutubeCore.CacheFolder, filename);
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


        public string GetBroadcastId()
        {
            string result = "";
       

            string url = String.Format("https://www.googleapis.com/youtube/v3/search?part=snippet&channelId={0}&eventType=live&type=video&key={1}", channelid, YoutubeCore.APIKEY);
            string Response = Helper.HttpGet(url);

            try
            {
                JObject obj = JObject.Parse(Response);

                result = (string)obj["items"][0]["id"]["videoId"];
                JObject item = (JObject)obj["items"][0]["snippet"];
                live_description = (string) item["title"];

            }
            catch (Exception)
            {


            }

            return result;
        }

    
    }
}
