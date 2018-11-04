using MRzeszowiak.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MRzeszowiak.Services
{
    public interface IRzeszowiakImageContainer
    {
        ImageSource ImageData { get; }
        event EventHandler OnDownloadFinish;
        Task<bool> DownloadImage(Advert advert, string ssid, int advertID, string advertURL, Cookie PHPSESSIDPcookie);
        void HideImage();
    }
}
