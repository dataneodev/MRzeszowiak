using Android.Widget;
using MRzeszowiak.Droid;
using MRzeszowiak.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(Toast_Android))]
namespace MRzeszowiak.Droid
{
    public class Toast_Android : IToast
    {
        public void Show(string message)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short).Show();
        }
    }
}