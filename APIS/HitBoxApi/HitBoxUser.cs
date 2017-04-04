using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using Common;

namespace HitBoxApi
{
    class HitBoxUser
    {

        public string id;
        public string user_name;
        public string user_logo_small = "";
        public string localavatar;


        public HitBoxUser(string userid)
        {
            this.id = userid;
            this.user_name = id;
        }

        public void GetInfos()
        {
            string url = "https://api.hitbox.tv/user/" + this.id;
            string Response = Helper.HttpGet(url);
            try
            {

                JsonConvert.PopulateObject(Response, this);

            }
            catch (Exception)
            {


            }
            //download user icon
            try
            {
                string filename = "avatar_" + this.id + "_" + Path.GetFileName(new Uri(HitBoxCore.CDNS[0]+user_logo_small).AbsolutePath);
                this.localavatar = Path.Combine(HitBoxCore.CacheFolder, filename);
                if (File.Exists(localavatar)) { return; }

                TryDownloadAvatar();


            }
            catch { }


        }

        private bool TryDownloadAvatar()
        {

            bool downloaded = false;
            WebClient client = new WebClient();

            foreach (string baseurl in HitBoxCore.CDNS)
            {
                if (downloaded) { break; }
                string tmpurl = baseurl + this.user_logo_small;

                if (Helper.RemoteFileExists(tmpurl))
                {
                    try
                    {
                        client.DownloadFile(tmpurl, this.localavatar);
                    }
                    catch (Exception)
                    {

                    }
                    downloaded = true;
                }
            }

            return downloaded;
        }


    }
}
