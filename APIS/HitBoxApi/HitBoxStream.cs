using Common;
using Newtonsoft.Json.Linq;
using StreamNotifier.Interfaces;
using System;
using System.Drawing;
using System.Text;
using System.Xml;

namespace HitBoxApi
{
    public class HitBoxStream : LiveStream

    {

        private string id;
        private string title;
        private string embed_url;
        private LiveStatus oldLiveState;
        private LiveStatus _onair;
        private HitBoxUser user;

        public string StreamerID { get; set; }

        #region Properties


        public string Quality
        {
            get;
            set;
        }

        public string StreamKey
        {
            get
            {
                return id;
            }

            set
            {
            }
        }

        public string Picture
        {
            get
            {
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
                return user.user_name;
            }

            set
            {
            }
        }

        public string Streamurl
        {
            get
            {
                return this.embed_url;
            }

            set
            {
                throw new NotImplementedException();
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

        public  string Title
        {
            get
            {
                return title;
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
            get;set;
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


        public HitBoxStream(string videoid)
        {
            id = videoid;
            embed_url = "http://www.hitbox.tv/" + id;
            title = id;
            oldLiveState = LiveStatus.Unknown;
            _onair = LiveStatus.Unknown;
        }

        public void FromXML(XmlNode xML)
        {
            title = (xML["Title"] == null) ? "_blank" : xML["Title"].InnerText;
            title = Encoding.UTF8.GetString(Convert.FromBase64String(title));

            user = new HitBoxUser(xML);

        }

        public static string GetStreamType()
        {
            return HitBoxCore.Streamtype;
        }

        public static Icon GetStreamIcon()
        {
            return HitBoxCore.Logo;
        }



        public void GetStreamInfos()
        {
            /*
            //Thread.Sleep(200);
            try
            {
                JObject jO = JObject.Parse(Response);
                string tmp = (string)jO.SelectToken("livestream[0].media_is_live");

                if (tmp.Equals("1"))
                {
                    OnAir = LiveStatus.Online;
                }
                else OnAir = LiveStatus.Offline;

            }
            catch {
                OnAir = LiveStatus.Unknown;
            }
            */

            user = new HitBoxUser(id);
            user.GetInfos();

          //  UpdateProperties();


        }

        public bool GoneOnline()
        {

            string url = "https://api.hitbox.tv/media/live/" + id;
            string Response = Helper.HttpGet(url);
            bool changed;
            //Thread.Sleep(200);

            try
            {
                JObject jO = JObject.Parse(Response);
                string tmp = (string)jO.SelectToken("livestream[0].media_is_live");


                if (tmp.Equals("1"))
                {
                    _onair = LiveStatus.Online;
                }
                else _onair = LiveStatus.Offline;

                
                title =  (string)jO.SelectToken("livestream[0].media_title");
                title = title == "" ? (string)jO.SelectToken("livestream[0].media_status") : title;
                title = title == "" ? id :  title;
            }
            catch
            {
                _onair = LiveStatus.Unknown;
                title = id;
                     
            }


            changed = (oldLiveState != _onair) && (_onair.Equals(LiveStatus.Online));

            oldLiveState = _onair;

            return changed;

        }

     
    }
}
