using Common;
using Newtonsoft.Json.Linq;
using StreamNotifier.Interfaces;
using System.Drawing;

namespace TwitchApi
{
    public class TwitchStream : LiveStream
    {
        private string internalid;
        private LiveStatus oldLiveState;
        private LiveStatus _onair;
        private TwitchChannel channel;
        private TwitchUser user;

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

                if (user.localavatar == null)
                {
                    return this.channel.localavatar;
                }
                else    return user.localavatar;
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
                return channel.url;
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
                return channel.status;
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
        }

        public void GetStreamInfos()
        {
            channel = new TwitchChannel(this.internalid);
            channel.GetChannelInfos(true);
            GetUser();
        }

        public bool GoneOnline()
        {
            string url = "https://api.twitch.tv/kraken/streams/" + this.internalid;
            string Response = Helper.HttpGet(url, TwitchCore.CustomHttpHeaders);
            bool changed = false;

            UpdateMe(Response);

            changed = (oldLiveState != _onair) && (_onair.Equals(LiveStatus.Online));


            oldLiveState = _onair;

            //update title
            channel.GetChannelInfos();


            return changed;
        }

   
        private void GetUser()
        {
            user = new TwitchUser(this.internalid);
            user.GetUserInfos();
        }


        private void UpdateMe(string Response)
        {
            

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
            }
        }



   

    }
}
