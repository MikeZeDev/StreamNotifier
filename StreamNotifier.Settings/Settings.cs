using StreamNotifier.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace StreamNotifier.Settings
{
    public class AppSettings
    {
        public static readonly string VERSION = "1.0";

        public static List<LiveStream> LiveStreams = new List<LiveStream>();
        public static ImageList ServicesIcons = new ImageList();
        public static ImageList StreamsLogos = new ImageList();

        private static int default_retrycount = 3;
        private static int default_retrydelay = 3000;

        public static string Default_picturekey = "STN_DEFAULT_PICTURE";
        public static string Default_quality = "best";

        public static string[] Qualities = { "audio_only", "160p", "360p", "480p", "720p", "720p60_alt", "720p60", "best", "worst" };



        private static Dictionary<string, Type> Apis = new Dictionary<string, Type>();
        private static Dictionary<string, string> MySettings = new Dictionary<string, string>();
        private static Dictionary<string, PluginSettings> PlugSettings = new Dictionary<string, PluginSettings>();


        /// <summary>
        /// Load Application settings via config.xml
        /// </summary>
        public static void Load()
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config.xml");

                XmlNode xnGeneral = xml.SelectSingleNode("/StreamNotifier/General");

                foreach (XmlNode xn in xnGeneral.ChildNodes)
                {
                    Set(xn.Name, xn.InnerText);
                }
            }
            catch (Exception)
            {

            }
            xml = null;

            if (String.IsNullOrEmpty(Get("retrycount")))
            {
                Set("retrycount", default_retrycount.ToString());
            }

            if (String.IsNullOrEmpty(Get("retrydelay")))
            {
                Set("retrydelay", default_retrydelay.ToString());
            }

        }

        /// <summary>
        /// Get Application Setting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            string result = "";
            MySettings.TryGetValue(key, out result);
            return result;
        }

        /// <summary>
        /// Set Application Setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void Set(string key, string value)
        {
            if (MySettings.ContainsKey(key))
            {
                MySettings[key] = value;
            }
            else MySettings.Add(key, value);
        }

        /// <summary>
        /// Get plugin Setting 
        /// </summary>
        /// <param name="pluginname"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetPluginSetting(string pluginname, string key)
        {
            return PlugSettings[pluginname].Get(key);
        }

        /// <summary>
        /// Set plugin Setting 
        /// </summary>
        /// <param name="pluginname"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetPluginSetting(string pluginname, string key, string value)
        {
            PlugSettings[pluginname].Set(key, value);
        }





        /// <summary>
        /// Charge la liste des streams via le xml (offline)
        /// </summary>
        /// <returns></returns>
        public static void LoadStreamsList()
        {
            LiveStreams.Clear();

            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("streams.xml");

                XmlNodeList xnStreams = xml.SelectNodes("/Streams/Stream");

                foreach (XmlNode xn in xnStreams)
                {
                    string skey = xn["StreamKey"].InnerText;
                    string stype = xn["StreamType"].InnerText;
                    LiveStream lv = Activate(stype, skey);
                    lv.FromXML(xn);

                    //lv.GetStreamInfos();
                    //lv.Quality = (xn["Quality"] == null) ? Default_quality : xn["Quality"].InnerText;
                    AddStream(lv);

                }
            }
            catch (Exception)
            {

            }
            xml = null;

        }



        /// <summary>
        /// Enregistre la liste des streams dans le xml
        /// </summary>
        /// <returns></returns>
        public static void SaveStreamList()
        {

            try
            {
                File.Copy("streams.xml", "streams.xml.bak", true);
            }
            catch (Exception)
            {

                throw new Exception("Error while saving streams : can't make backup !");
            }



            try
            {

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";

                using (XmlWriter writer = XmlWriter.Create("streams.xml", settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Streams");

                    foreach (LiveStream entry in LiveStreams)
                    {

                        writer.WriteStartElement("Stream");
                        writer.WriteElementString("StreamKey", entry.StreamKey);
                        writer.WriteElementString("StreamerID", entry.StreamerID);

                        writer.WriteElementString("StreamType", entry.StreamType);
                        writer.WriteElementString("Name", entry.Displayname);
                        writer.WriteElementString("Quality", entry.Quality);
                        writer.WriteElementString("Title", Convert.ToBase64String(Encoding.UTF8.GetBytes(entry.Title)));

                        if (entry.Picture != null)
                        {
                            writer.WriteElementString("Picture", Convert.ToBase64String(Encoding.UTF8.GetBytes(entry.Picture)));
                        }
                        else writer.WriteElementString("Picture", Convert.ToBase64String(Encoding.UTF8.GetBytes(Default_picturekey)));

                        writer.WriteElementString("Url", Convert.ToBase64String(Encoding.UTF8.GetBytes(entry.Streamurl)));
                        writer.WriteEndElement();

                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();



                }
            }
            catch (Exception)
            {
                File.Delete("streams.xml");
                File.Copy("streams.xml.bak", "streams.xml", true);
            }


            try
            {
                File.Delete("streams.xml.bak");
            }
            catch (Exception)
            {
            }


        }


        public static void ClearCaches()
        {
            foreach (Type T in Apis.Values)
            {
                MethodInfo m = T.GetMethod("ClearCache");
                m.Invoke(null, null);
            }

            StreamsLogos.Images.Clear();

        }


        private static void SetDefaultPicture(LiveStream LS)
        {
            LS.Picture = null;
            LS.PictureKey = Default_picturekey;
        }

        #region PluginWrapper

        /// <summary>
        /// Instancie un Livestream de la classe fille adéquate (via l'URl")
        /// </summary>
        /// <param name="service"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static LiveStream ActivateFromUrl(string service, string url)
        {
            LiveStream stream = null;
            try
            {
                Type t = Apis[service];
                // stream = Activator.CreateInstance(t, id) as LiveStream;

                object[] parameters = new object[] { url };
                string id = (string)t.GetMethod("CanHandleUrl", BindingFlags.Public | BindingFlags.Static).Invoke(null, parameters);

                parameters = new object[] { id };
                stream = (LiveStream)t.GetMethod("Activate", BindingFlags.Public | BindingFlags.Static).Invoke(null, parameters);
                stream.Quality = "best";


            }
            catch (Exception)
            {

            }
            return stream;
        }




        /// <summary>
        /// Instancie un Livestream de la classe fille adéquate (via le "service")
        /// </summary>
        /// <param name="service"></param>
        /// <param name="id"></param>
        /// <returns></returns>
		public static LiveStream Activate(string service, string id)
        {
            LiveStream stream = null;
            try
            {
                Type t = Apis[service];
                // stream = Activator.CreateInstance(t, id) as LiveStream;
                object[] parameters = new object[] { id };
                stream = (LiveStream)t.GetMethod("Activate", BindingFlags.Public | BindingFlags.Static).Invoke(null, parameters);
                stream.Quality = "best";

            }
            catch (Exception)
            {

            }
            return stream;
        }

        /// <summary>
        /// Instancie un Livestream de la classe t 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static LiveStream Activate(Type t, string id)
        {
            LiveStream stream = null;
            try
            {
                // stream = Activator.CreateInstance(t, id) as LiveStream;
                object[] parameters = new object[] { id };
                stream = (LiveStream)t.GetMethod("Activate", BindingFlags.Public | BindingFlags.Static).Invoke(null, parameters);
                stream.Quality = "best";
            }
            catch (Exception)
            {

            }
            return stream;
        }

        /// <summary>
        /// Charge les assembly qui implémentent des classes filles de LiveStream
        /// </summary>
        public static void LoadApiAssembliesAndTypes()
        {

            string folder = Path.Combine(Environment.CurrentDirectory, "plugins");
            string[] assembliz = Directory.GetFiles(folder, "*.dll");

            foreach (string dir in assembliz)
            {

                //TODO  : Vérifier la signature numérique de l'assembly, pour des raisons de sécurité?
                Assembly ass = Assembly.LoadFrom(dir);

                /*
                //Recherches les classes qui héritent de LiveStream
                var q = from t in ass.GetTypes()
                        where t.IsSubclassOf(typeof(LiveStream))
                        select t;
                        */

                var q = from t in ass.GetTypes()
                  .Where(p => typeof(LiveStream).IsAssignableFrom(p) && p.IsClass)
                        select t;


                if (!q.Any())
                {
                    continue;
                }


                var z = from t in ass.GetTypes()
                        where (t.GetMethod("StreamApiInitialize") != null)
                        select t;

                z.ToList().ForEach(t =>
                {
                    string ServiceName = (string)t.GetField("Streamtype").GetValue(null);
                    //Read the Settings for the plugin

                    PluginSettings PL = new PluginSettings(ServiceName);
                    PL.Load();
                    PlugSettings.Add(ServiceName, PL);

                    t.GetMethod("StreamApiInitialize").Invoke(null, null);
                    Icon ServiceIcon = (Icon)t.GetField("Logo").GetValue(null); ;
                    ServicesIcons.Images.Add(ServiceName, ServiceIcon);

                    Apis.Add(ServiceName, t);
                }

                );


            }
        }


        /// <summary>
        /// Ajoute un stream à la liste des streams (via son URL et le nom de l'api utilisée)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="service"></param>
        public static void AddStream(string url, string service)
        {
            object[] parameters = new object[] { url };

            string result = (string)Apis[service].GetMethod("CanHandleUrl", BindingFlags.Public | BindingFlags.Static).Invoke(null, parameters);

            LiveStream lv = null;
            lv = Activate(Apis[service], result);
            lv.GetStreamInfos();
            LiveStreams.Add(lv);

        }

        ///
        ///
        public static bool SteamInList(LiveStream LS)
        {
            bool result = false;

            foreach (LiveStream LV in LiveStreams)
            {
                result = LV.Streamurl == LS.Streamurl;
                if (result) { break; }
            }

            return result;

        }








        public static void UpdateUiPicture(LiveStream LS)
        {
            try
            {

                if ((!String.IsNullOrEmpty(LS.Picture) && File.Exists(LS.Picture)))
                {
                    LS.PictureKey = LS.StreamKey;

                    try
                    {
                        StreamsLogos.Images.RemoveByKey(LS.PictureKey);
                    }
                    catch { }


                        FileStream fs = new FileStream(LS.Picture, FileMode.Open, FileAccess.Read);
                        StreamsLogos.Images.Add(LS.PictureKey, Image.FromStream(fs));
                        fs.Dispose();
                        fs.Close();
                    }
                    else
                    {
                        SetDefaultPicture(LS);
                    }


                
            }
            catch (Exception)
            {
                SetDefaultPicture(LS);
            }
        }


        /// <summary>
        /// Ajoute un stream à la liste des streams 
        /// </summary>
        /// <param name="LS"></param>
        public static void AddStream(LiveStream LS)
        {
            UpdateUiPicture(LS);
            LiveStreams.Add(LS);
        }



        /// <summary>
        /// Retourne la liste des apis capables de traiter l'url (normalement une seule, mais on gère quand même en renvoyant un tableau)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static List<string> WhoCanHandleThisUrl(string url)
        {
            List<string> result = new List<string>();

            object[] parameters = new object[] { url };

            foreach (KeyValuePair<string, Type> entry in Apis)
            {

                string tmp = (string)entry.Value.GetMethod("CanHandleUrl", BindingFlags.Public | BindingFlags.Static).Invoke(null, parameters);

                if (!String.IsNullOrEmpty(tmp))
                {
                    result.Add(entry.Key);
                }
            }

            return result;

        }


        #endregion

    }



    public class PluginSettings
    {
        private Dictionary<string, string> Settings = new Dictionary<string, string>();
        private string ApiName;

        public PluginSettings(string name)
        {
            if (name.Equals("StreamNotifier",StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Can't use StreamNotifier name for plugin");
                   
             }
            ApiName = name;
        }

        public string Get(string key)
        {
            return Settings[key];
        }

        public void Set(string key, string value)
        {
            if (Settings.ContainsKey(key))
            {
                Settings[key] = value;
            }
            else Settings.Add(key, value);
        }

        public void Load()
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load("config.xml");

                string PluginNode = String.Format("/StreamNotifier/Plugins/{0}", this.ApiName);

                XmlNode xnGeneral = xml.SelectSingleNode(PluginNode);

                foreach (XmlNode xn in xnGeneral.ChildNodes)
                {
                    Set(xn.Name, xn.InnerText);
                }
            }
            catch (Exception)
            {

            }
            xml = null;


        }


    }
}
