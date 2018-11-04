using MRzeszowiak.Extends;
using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MRzeszowiak.Services
{
    public class RzeszowiakImageContainer : IRzeszowiakImageContainer
    {
        public ImageSource ImageData { get; private set; }
        public event EventHandler OnDownloadFinish;
        private volatile string _session = String.Empty;

        public async Task<bool> DownloadImage(Advert advert, string ssid, int advertID, string advertURL, Cookie PHPSESSIDPcookie)
        {
            if(advert == null)
            {
                Debug.Write(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name +
                                            " => advert == null");
                return false;
            }

            if ((advert.PhoneImageByteArray?.Length ?? 0) > 0)
            {
                Debug.Write(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name +
                                            " => advert.PhoneImageByteArray exists");
                ImageData = ImageSource.FromStream(() => new MemoryStream(advert.PhoneImageByteArray));
                OnDownloadFinish?.Invoke(this, new EventArgs());
                return true;
            }

            if (ssid == null || advertID == 0 || advertURL.Length == 0 || PHPSESSIDPcookie == null)
            {
                Debug.Write(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name +
                            " => ssid == null || adrverID == 0 || advertURL.Length == 0");
                return false;
            }

            _session = new Guid().ToString();
            var localSession = _session;

            var inputData = new Dictionary<string, string>()
            {
                { "oid", advertID.ToString() },
                { "ssid", ssid },
            };

            var BodyResult = await GetWeb.PostWebPage("http://www.rzeszowiak.pl/telefon/", inputData, advertURL, new CookieCollection() { PHPSESSIDPcookie });
            if (BodyResult.Length == 0)
            {
                Debug.Write("RzeszowiakImageContainer => DownloadImage => BodyResult.Length == 0");
                return false;
            }

            Base64ToImage(BodyResult.ToString().Replace("data:image/jpeg;base64,",""), 
                         out ImageSource imageData, out byte[] byteArray);
            ImageData = imageData;
            advert.PhoneImageByteArray = byteArray;
            BodyResult.Clear();

            if(localSession == _session)
                OnDownloadFinish?.Invoke(this, new EventArgs());
            else
                Debug.Write("RzeszowiakImageContainer => DownloadImage() => localSession != _session");
            return true;
        }

        protected void Base64ToImage(string base64String, out ImageSource imageSource, out byte[] imageByteArray)
        {
            byte[] byteArray = Convert.FromBase64String(base64String);
            imageByteArray = byteArray;
            imageSource = ImageSource.FromStream(() => new MemoryStream(byteArray));
        }

        public void HideImage()
        {
            ImageData = null;
            OnDownloadFinish?.Invoke(this, new EventArgs());
        }
    }

}
