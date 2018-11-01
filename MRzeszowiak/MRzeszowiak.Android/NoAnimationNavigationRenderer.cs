using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Threading.Tasks;
using MRzeszowiak.Extends;
using MRzeszowiak.Droid;

[assembly: ExportRenderer(typeof(NoAnimationNavigationPage), typeof(NoAnimationNavigationRenderer))]
namespace MRzeszowiak.Droid
{
    public class NoAnimationNavigationRenderer : NavigationRenderer
    {
        public NoAnimationNavigationRenderer(Context context) : base(context)
        { }

        protected override Task<bool> OnPopViewAsync(Page page, bool animated)
        {
            return base.OnPopViewAsync(page, false);
        }

        protected override Task<bool> OnPushAsync(Page view, bool animated)
        {
            return base.OnPushAsync(view, false);
        }
    }
}