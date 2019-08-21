using Common;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Drawing;
using StreamNotifier.Interfaces;
using System.Xml;
using System.Text;

namespace DailymotionApi
{
    public class DailyMotionStream : LiveStream
    {



        #region Properties

        public string StreamerID { get; set; }
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
                return user.screenname;
            }

            set
            {
            }
        }

        public string Streamurl
        {
            get
            {
                return embed_url;
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
                return this.title;
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

        public string Quality
        {
            get;
            set;
        }


        public bool ShowPopup
        {
            get;
            set;
         
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



        //PRIVATE MEMBERS
        private string id;
        private string title;
        private string embed_url;
        private string thumbnail_small_url;
        private string local_thumbnail_small_url;
        private LiveStatus _onair;
        private LiveStatus oldLiveState;

        private DailyMotionUser user;


        public DailyMotionStream(string videoid)
        {
            id = videoid;
            oldLiveState = LiveStatus.Unknown;
            _onair = LiveStatus.Unknown;
        }

        public void FromXML(XmlNode xML)
        {
            Quality = (xML["Quality"] == null) ? "best" : xML["Quality"].InnerText;

            title = (xML["Title"] == null) ? "dummy" : xML["Title"].InnerText;
            title = Encoding.UTF8.GetString(Convert.FromBase64String(title));

            id = (xML["StreamKey"] == null) ? "_dummy" : xML["StreamKey"].InnerText;


            embed_url = (xML["Url"] == null) ? "_blank" : (xML["Url"].InnerText);
            embed_url = Encoding.UTF8.GetString(Convert.FromBase64String(embed_url));

            user = new DailyMotionUser(xML);

        }



        public static string GetStreamType()
        {
            return DailyMotionCore.Streamtype;
        }

        public static Icon GetStreamIcon()
        {
            return DailyMotionCore.Logo;
        }

        public  void GetStreamInfos()
        {

            string url = "https://api.dailymotion.com/video/" + id + "?fields=title,owner,onair,embed_url,thumbnail_small_url";
            string Response = Helper.HttpGet(url);

            UpdateMe(Response, true);

            //Download thumbnail
            try
            {
                string filename = "thumbnail_" + id + "_" + Path.GetFileName(new Uri(thumbnail_small_url).AbsolutePath);
                local_thumbnail_small_url = Path.Combine(DailyMotionCore.CacheFolder, filename);
                if (File.Exists(local_thumbnail_small_url)) { return; }
                using (WebClient client = new WebClient())
                {
                    //File.Delete(local_thumbnail_small_url);
                    client.DownloadFile(thumbnail_small_url, local_thumbnail_small_url);
                    Helper.ScaleImageFile(local_thumbnail_small_url,80,80);


                }

            }
            catch { }
            


        }


        private void UpdateMe(string Response,bool init = false)
        {


            try
            {
                //JsonConvert.PopulateObject(Response, this);
                JObject obj = JObject.Parse(Response);

                this.title = (string)obj["title"];

                if (init)
                {

                    this.embed_url = (string)obj["embed_url"];
                    this.thumbnail_small_url = (string)obj["thumbnail_small_url"];
                    this.oldLiveState = LiveStatus.Unknown;
                    this._onair = LiveStatus.Unknown;

                    user = new DailyMotionUser((string)obj["owner"]);
                    user.GetInfos();

                }
                else
                {
                    bool tmp = (bool)obj["onair"];
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
            string url = "https://api.dailymotion.com/video/" + id + "?fields=title,onair";
            string Response = Helper.HttpGet(url);

            UpdateMe(Response);
            bool changed = (oldLiveState != _onair) && (_onair.Equals(LiveStatus.Online));

            oldLiveState = _onair;
            return changed;

        }

    }




}
