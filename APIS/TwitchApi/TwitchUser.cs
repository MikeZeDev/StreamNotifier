using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace TwitchApi
{
    public class TwitchUser
    {

        public string internalid;
        public string display_name;
        public string logo;

        public string localavatar;
        public string twitch_user_id; //numerical ID for requests
        public string description;



        public TwitchUser(string channelid)
        {
            this.internalid = channelid;
        }

        public TwitchUser(XmlNode xML)
        {
            internalid = (xML["StreamKey"] == null) ? "_blank" : xML["StreamKey"].InnerText;
            display_name = (xML["Name"] == null) ? "_blank" : xML["Name"].InnerText;

            localavatar = (xML["Picture"] == null) ? "_blank" : (xML["Picture"].InnerText);
            localavatar = Encoding.UTF8.GetString(Convert.FromBase64String(localavatar));

            twitch_user_id = (xML["StreamerID"] == null) ? "_blank" : xML["StreamerID"].InnerText;

            if (twitch_user_id == "_blank")
            {
                GetUserInfos(); //upgrade the shit !
            }




        }

        public void GetUserInfos()
        {
            //Deprecated !
            //string url = "https://api.twitch.tv/kraken/users/" + this.internalid;

            string url = "https://api.twitch.tv/helix/users?login=" + this.internalid;

            string Response = Helper.HttpGet(url, TwitchCore.CustomHttpHeaders);

            /*
            try
            {
                JsonConvert.PopulateObject(Response, this);
            }
            catch (Exception)
            {

            }*/



            /*
             * 
             * 
              "data": [
                {
                  "id": "53157837",
                  "login": "orann_",
                  "display_name": "OraNN_",
                  "type": "",
                  "broadcaster_type": "affiliate",
                  "description": "Minecraft YouTubeur : https://www.youtube.com/user/Or4NN",
                  "profile_image_url": "https://static-cdn.jtvnw.net/jtv_user_pictures/4ca1a384-0010-4482-91a3-edad8e69aea4-profile_image-300x300.png",
                  "offline_image_url": "https://static-cdn.jtvnw.net/jtv_user_pictures/585b748a-6a80-44de-bd32-c0ffff9f348d-channel_offline_image-1920x1080.png",
                  "view_count": 9680
                }
              ]
            }
             * 
             * 
             * */


            JObject obj = JObject.Parse(Response);
            JToken token = (obj["data"] as JArray).FirstOrDefault();

            this.twitch_user_id = token.Value<string>("id");
            this.display_name = token.Value<string>("display_name");
            this.description = token.Value<string>("description");
            this.logo = token.Value<string>("profile_image_url");



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
