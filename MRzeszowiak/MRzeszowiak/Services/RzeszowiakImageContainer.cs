using MRzeszowiak.Extends;
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

        public async Task<bool> DownloadImage(string ssid, int advertID, string advertURL, Cookie PHPSESSIDPcookie)
        {
            if (ssid == null || advertID == 0 || advertURL.Length == 0 || PHPSESSIDPcookie == null)
            {
                Debug.Write(this.GetType().Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name +
                            " => ssid == null || adrverID == 0 || advertURL.Length == 0");
                return false;
            }

            ImageData = null;
            OnDownloadFinish?.Invoke(this, new EventArgs());

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
            ImageData = Base64ToImage(BodyResult.ToString().Replace("data:image/jpeg;base64,",""));
            BodyResult.Clear();

            if(localSession == _session)
                OnDownloadFinish?.Invoke(this, new EventArgs());
            else
                Debug.Write("RzeszowiakImageContainer => DownloadImage() => localSession != _session");
            
            return true;
        }

        public ImageSource Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            ImageSource image = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            return image;
        }
    }

}
