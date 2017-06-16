using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Common
{
    public class Helper
    {


        static public void ScaleImageFile(string file, int maxWidth, int maxHeight)
        {
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite);
            Image image = Image.FromStream(fs);

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);


            Bitmap bmp = new Bitmap(newImage);
            ImageFormat imageFormat = image.RawFormat;
            image.Dispose();
            fs.Dispose();
            fs.Close();
            try
            {
                bmp.Save(file, imageFormat);

            }
            catch (Exception e)
            {

                Console.Write(e.ToString());
            }
            bmp.Dispose();
            newImage.Dispose();

        }

        static public void Empty(string folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder);

            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    dir.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        //Twitch Api now requires a CLIENT-ID in Http requests headers
        public static string HttpGet(string url, Dictionary<string, string> headers)
        {
            try
            {
                using (WebClient strJson = new WebClient())
                {

                    foreach (KeyValuePair<string, string> KV in headers)
                    {
                        try
                        {
                            strJson.Headers.Add(KV.Key, KV.Value);
                        }

                        catch (Exception)
                        {
                            strJson.Headers.Set(KV.Key, KV.Value);
                        }

                    }


                    string str = strJson.DownloadString(url);
                    byte[] bytes = Encoding.Default.GetBytes(str);
                    return Encoding.UTF8.GetString(bytes);
                }
            }
            catch (Exception)
            {
                return "";
            }

        }

        public static string HttpGet(string url)
        {
            try
            {
                using (WebClient strJson = new WebClient())
                {
                    string str = strJson.DownloadString(url);
                    byte[] bytes = Encoding.Default.GetBytes(str);
                    return Encoding.UTF8.GetString(bytes);
                }
            }
            catch (Exception)
            {
                return "";
            }

        }

        public static string HttpPost(string url, Dictionary<string, string> headers, NameValueCollection pairs)
        {

            try
            {
                byte[] result = null;
                using (WebClient webClient = new WebClient())
                {

                    //Add headers 
                    foreach (KeyValuePair<string, string> KV in headers)
                    {
                        try
                        {
                            webClient.Headers.Add(KV.Key, KV.Value);
                        }

                        catch (Exception)
                        {
                            webClient.Headers.Set(KV.Key, KV.Value);
                        }

                    }




                    result = webClient.UploadValues(url, pairs);
                }

                return Encoding.UTF8.GetString(result);
            }
            catch (Exception ) 
            {

                return "";
            }

        }


        public static bool RemoteFileExists(string url)
        {
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }



        public static string Truncate(string str, int maxlen)
        {
            if (str.Length > maxlen)
            {
                return str.Substring(0, maxlen);
            }
            else return str;
        }


        /*
        public static string GetJSON(string url)
        {
            string result = "";
            WebRequest wb = WebRequest.Create(url);

            try
            {
                StreamReader loResponseStream = new StreamReader(wb.GetResponse().GetResponseStream(), Encoding.UTF8);
                result = loResponseStream.ReadToEnd();
     
                return result;
            }
            catch
            {
            }

            return result;
        }
    

       
        public static string GetJSON(string url, Encoding enc)
        {
            string result = "";
            WebRequest wb = WebRequest.Create(url);

            try
            {
                StreamReader loResponseStream = new StreamReader(wb.GetResponse().GetResponseStream(), enc);
                result = loResponseStream.ReadToEnd();

                return result;
            }
            catch
            {
                
            }

            return result;
        }
        */
    }


}
