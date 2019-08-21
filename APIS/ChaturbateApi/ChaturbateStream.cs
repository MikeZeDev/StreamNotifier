using Common;
using Newtonsoft.Json.Linq;
using StreamNotifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace ChaturbateApi
{
    public class ChaturbateStream : LiveStream
    {
        private string id;
        private string localavatar;
        private LiveStatus oldLiveState;
        private LiveStatus _onair;

        private static string APIURL = "https://chaturbate.com/get_edge_hls_url_ajax/";

        private static string URLTHUMBBASE = "https://roomimg.stream.highwebmedia.com/ri/{0}.jpg";

        public ChaturbateStream(string id)
        {
            this.id = id;
            
        }



        #region Properties

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
                return localavatar;
            }

            set
            {
            }
        }

        public string Displayname
        {
            get
            {
                return id;
            }

            set
            {
            }
        }

        public string Streamurl
        {
            get
            {
                return String.Format("http://www.chaturbate.com/{0}/", id);
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
                return id;

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
        public string Quality
        {
            get;
            set;
        }
        public string StreamerID { get ; set ; }

        #endregion


        public static string GetStreamType()
        {
            return ChaturbateCore.Streamtype;
        }


        public void GetStreamInfos()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/x-www-form-urlencoded");
            headers.Add("CSRFToken", Guid.NewGuid().ToString().ToUpper());
            headers.Add("X-Requested-With", "XMLHttpRequest");
            headers.Add("Referer", "https://chaturbate.com/"+ id);

            NameValueCollection NVC = new NameValueCollection();
            NVC.Add("room_slug", id);
            NVC.Add("bandwidth", "high");

            string result = Helper.HttpPost(APIURL, headers, NVC);

            UpdateMe(result, true);

            bool changed = (oldLiveState != _onair) && (_onair.Equals(LiveStatus.Online));

            if (changed && _onair.Equals(LiveStatus.Online))
            {
                UpdateThumbnail();
            }
        }

        private void UpdateMe(string response, bool init=false)
        {
            try
            {
                JObject obj = JObject.Parse(response);

                if (init)
                {

                    this.oldLiveState = LiveStatus.Unknown;
                    this._onair = LiveStatus.Unknown;
                }
                else
                {
                    bool tmp = (bool)obj["success"];
                    tmp = tmp && ((string)obj["room_status"] == "public");
                    _onair = tmp ? LiveStatus.Online : LiveStatus.Offline;
                }

            }
            catch
            {

                _onair = LiveStatus.Unknown;

            }




        }

        public bool GoneOnline()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/x-www-form-urlencoded");
            headers.Add("CSRFToken", Guid.NewGuid().ToString().ToUpper());
            headers.Add("X-Requested-With", "XMLHttpRequest");
            headers.Add("Referer", "https://chaturbate.com/" + id);

            NameValueCollection NVC = new NameValueCollection();
            NVC.Add("room_slug", id);
            NVC.Add("bandwidth", "high");

            string result = Helper.HttpPost(APIURL, headers, NVC);

            UpdateMe(result);
            bool changed = (oldLiveState != _onair) && (_onair.Equals(LiveStatus.Online));

            oldLiveState = _onair;

            if (changed && _onair.Equals(LiveStatus.Online))
            {
                UpdateThumbnail();
            }


            return changed;
        }

  
        private void UpdateThumbnail()
        {

            try
            {
                string filename = "thumbnail_" + id + ".jpg";
                localavatar = Path.Combine(ChaturbateCore.CacheFolder, filename);

                //if (File.Exists(localavatar)) { return; }

                try
                {
                    File.Delete(localavatar);
                }
                catch (Exception)
                {
                }


                using (WebClient client = new WebClient())
                {
                    //File.Delete(local_thumbnail_small_url);
                    client.DownloadFile(String.Format(URLTHUMBBASE, id), localavatar);
                    Helper.ScaleImageFile(localavatar, 80, 80);
                }

               // UpdateAvatar(PictureKey, localavatar);
                

            }
            catch { }
        }

        public void FromXML(XmlNode xML)
        {
            Quality = (xML["Quality"] == null) ? "best" : xML["Quality"].InnerText;

            localavatar = (xML["Picture"] == null) ? "_blank" : xML["Picture"].InnerText;
            localavatar = Encoding.UTF8.GetString(Convert.FromBase64String(localavatar));


        }
    }
}
