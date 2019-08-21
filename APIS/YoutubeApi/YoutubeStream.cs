using Common;
using StreamNotifier.Interfaces;
using System;
using System.Drawing;
using System.Xml;

namespace YoutubeApi
{
    public class YoutubeStream : LiveStream
    {
        private string channelid;
        private string streamid;//videoid !

        private LiveStatus oldLiveState;
        private LiveStatus _onair;
        private YoutubeChannel channel;
        private const string BASEURL = "https://www.youtube.com";

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
                return channelid;
            }

            set
            {
            }
        }

        public string Picture
        {
            get
            {
                return channel.localavatar;
            }

            set
            {
            }
        }

        public string Displayname
        {
            get
            {
                return channel.title;
            }

            set
            {
            }
        }

        public string Streamurl
        {
            get
            {
                if (String.IsNullOrEmpty(streamid))
                {
                    return String.Concat(BASEURL,"/channel/"+ channelid);
                }
                else
                {
                    return String.Concat(BASEURL, "/watch?v=" + streamid);
                }


                
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
                if (String.IsNullOrEmpty(channel.live_description))
                {
                    return Helper.Truncate(channel.channel_description,200) + "...";
                }
                else return channel.live_description;

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


        public YoutubeStream(string channelid)
        {
            this.channelid = channelid;
        }

        public void FromXML(XmlNode xML)
        {
            Quality = (xML["Quality"] == null) ? "best" : xML["Quality"].InnerText;
            channel = new YoutubeChannel(xML);

        }


        public static string GetStreamType()
        {
            return YoutubeCore.Streamtype;
        }

        public static Icon GetStreamIcon()
        {
            return YoutubeCore.Logo;
        }

        public void GetStreamInfos()
        {
            channel = new YoutubeChannel(channelid);
            channel.GetChannelInfos(true);

        }



        


        public bool GoneOnline()
        {

            streamid = channel.GetBroadcastId();

            if (!String.IsNullOrEmpty(streamid))
                {
                     _onair = LiveStatus.Online;
                 }
            else
            {
                _onair = LiveStatus.Offline;
            }

            bool changed = false;
            changed = (oldLiveState != _onair) && (_onair.Equals(LiveStatus.Online));


            oldLiveState = _onair;

            if (_onair != LiveStatus.Online)
            {
                streamid = "";
            }

         //   channel.GetChannelInfos();

            return changed;
        }


    }

}
