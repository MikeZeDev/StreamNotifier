
using System.Xml;

namespace StreamNotifier.Interfaces
{

    public interface LiveStream
    {

        /// <summary>
        /// Stream Identifier (unique id amond the stream service)
        /// </summary>
        string StreamKey
        {
            get;
            set;
        }

        /// <summary>
        /// Stream logo. Most of the time, same as the user picture
        /// </summary>
        string Picture
        {
            get;
            set;
        }

        /// <summary>
        /// Logo ID in the dedicated image list.
        /// </summary>
        string PictureKey
        {
            get;
            set;
        }

        /// <summary>
        /// Stream name. Usually, the channel name or the username
        /// </summary>
        string Displayname
        {
            get;
            set;
        }

        /// <summary>
        ///  Internet link to the Stream. 
        /// </summary>
        string Streamurl
        {
            get;
            set;
        }
        /// <summary>
        /// Name of the stream service (case sensitive). 
        /// </summary>
        string StreamType
        {
            get;
            set;
        }

        /// <summary>
        /// Title of the service. Usually the description
        /// </summary>
        string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Is the live "On air", "Offline", or "Unknow"
        /// </summary>
        LiveStatus OnAir
        {
            get;
            set;
        }

        /// <summary>
        /// Prefered quality
        /// </summary>
        string Quality
        {
            get;
            set;
        }


        /// <summary>
        /// After a check, is true if we need to show a popup
        /// </summary>
        bool ShowPopup
        { get; set; }


        /// <summary>
        /// true if item is marked for manual checking (checking all streams doesnt require this)
        /// </summary>
        bool CheckPending
        { get; set; }
        string StreamerID { get; set; }


        /// <summary>
        /// Get Stream informations (if needed, should call other methods to get user channel infos. Download the avatars too)
        /// </summary>
        void GetStreamInfos();

        /// <summary>
        /// Update stream informations and return true if the stream is now online and he wasnt in the previous check
        /// </summary>
        bool GoneOnline();


        /// <summary>
        /// Update stream informations offline from an XML NODE
        /// </summary>
        void FromXML(XmlNode xML);


    }


    public enum LiveStatus
    {
        Unknown,
        Online,
        Offline,

    }
}

