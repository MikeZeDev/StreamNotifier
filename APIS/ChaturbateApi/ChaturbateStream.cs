using Common;
using Newtonsoft.Json.Linq;
using StreamNotifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;

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

            //Download thumbnail
            try
            {
                string filename = "thumbnail_" + id+".jpg";
                localavatar = Path.Combine(ChaturbateCore.CacheFolder, filename);
                if (File.Exists(localavatar)) { return; }
                using (WebClient client = new WebClient())
                {
                    //File.Delete(local_thumbnail_small_url);
                    client.DownloadFile(String.Format(URLTHUMBBASE, id), localavatar);
                    Helper.ScaleImageFile(localavatar, 80, 80);


                }

            }
            catch { }




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
            return changed;
        }
    }
}
