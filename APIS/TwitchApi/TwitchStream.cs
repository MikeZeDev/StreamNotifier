using Common;
using Newtonsoft.Json.Linq;
using StreamNotifier.Interfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;

namespace TwitchApi
{
    public class TwitchStream : LiveStream
    {
        private string internalid;
        private LiveStatus oldLiveState;
        private LiveStatus _onair;

       // private TwitchChannel channel;
        private TwitchUser user;
         

        private string url;
        private string stream_title;


        #region Properties

        public string StreamerID {


            get
            {
                return user.twitch_user_id;
            }

            set { }
        }


        public string Quality
        {
            get;
            set;
        }

        public string StreamKey
        {
            get
            {
                return internalid;
            }

            set
            {
            }
        }

        public string Picture
        {
            get
            {

                /* if (user.localavatar == null)
                 {
                     return this.channel.localavatar;
                 }
                 else*/

                return user.localavatar;
            }

            set
            {
            }
        }

        public string Displayname
        {
            get
            {
              return user.display_name;
            }

            set
            {
            }
        }

        public string Streamurl
        {
            get
            {
                //return channel.url
                return this.url;
            }

            set
            {
            }
        }

        public string StreamType
        {
            get
            {
                return GetStreamType();
            }

            set
            {
            }
        }

        public string Title
        {
            get
            {

                //return channel.status;

                return stream_title;
            }

            set
            {
               
            }
        }

        public LiveStatus OnAir
        {
            get
            {
                return _onair;
            }

            set
            {
            
            }
        }

        public bool ShowPopup
        {
            get; set;
        }


        public string PictureKey
        {
            get;
            set;
        }

        public bool CheckPending
        {
            get;
            set;
        }

        #endregion




        public static string GetStreamType()
        {
            return TwitchCore.Streamtype;
        }

        public static Icon GetStreamIcon()
        {
            return TwitchCore.Logo;
        }

        public TwitchStream(string streamid)
        {
            this.internalid = streamid;
            this.url = "https://www.twitch.tv/" + this.internalid;
        }

        public void GetStreamInfos()
        {
            //channel = new TwitchChannel(this.internalid);
            //channel.GetChannelInfos(true);
            GetUser();

            if (String.IsNullOrEmpty(stream_title))
            {
                stream_title = user.description;
            }
        }

        public bool GoneOnline()
        {

            if (user == null)
            {
                GetUser();
            }

            //string url = "https://api.twitch.tv/kraken/streams/" + this.internalid;

            string url = "https://api.twitch.tv/helix/streams?user_id=" + user.twitch_user_id;

            string Response = Helper.HttpGet(url, TwitchCore.CustomHttpHeaders);
            bool changed = false;

            UpdateMe(Response);

            changed = (oldLiveState != _onair) && (_onair.Equals(LiveStatus.Online));


            oldLiveState = _onair;

   
            return changed;
        }

   
        private void GetUser()
        {
            user = new TwitchUser(this.internalid);
            user.GetUserInfos();

      
        }


        private void UpdateMe(string Response)
        {

            /*
            try
            {
                string tmpstr = JObject.Parse(Response)["stream"].ToString();

                if (tmpstr != "")
                {
                    _onair = LiveStatus.Online;
                }
                else
                {
                    _onair = LiveStatus.Offline;
                }

             
            }
            catch (System.Exception)
            {
                _onair = LiveStatus.Unknown;
            }*/


            /*
             * 
             * {
                "data": [{
                    "id": "35381748400",
                    "user_id": "29188740",
                    "user_name": "FantaBobShow",
                    "game_id": "509066",
                    "type": "live",
                    "title": "La Darksoulisation post-opéspé",
                    "viewer_count": 2145,
                    "started_at": "2019-08-21T21:00:04Z",
                    "language": "fr",
                    "thumbnail_url": "https://static-cdn.jtvnw.net/previews-ttv/live_user_fantabobshow-{width}x{height}.jpg",
                    "tag_ids": ["6f655045-9989-4ef7-8f85-1edcec42d648"]
                }],
                "pagination": {
                    "cursor": "eyJiIjpudWxsLCJhIjp7Ik9mZnNldCI6MX19"
    }
             * 
             */

            try
            {
                JObject o = JObject.Parse(Response);
                JToken jt = (o["data"] as JArray).FirstOrDefault();

                if (jt == null)
                {
                    _onair = LiveStatus.Offline;
                }
                else
                {
                    _onair = LiveStatus.Online;

                    this.stream_title = jt.Value<string>("title"); 
                }


            }
            catch (Exception)
            {
                _onair = LiveStatus.Unknown;
            }





        }

        public void FromXML(XmlNode xML)
        {
            url = "https://www.twitch.tv/" + this.internalid;

            Quality = (xML["Quality"] == null) ? "best" : xML["Quality"].InnerText;
            _onair = LiveStatus.Unknown;

            stream_title = (xML["Title"] == null) ? "_blank" : xML["Title"].InnerText;
            stream_title = Encoding.UTF8.GetString(Convert.FromBase64String(stream_title));

            //Create dummy channel
            //channel = new TwitchChannel(xML);

            user = new TwitchUser(xML);



        }
    }
}
