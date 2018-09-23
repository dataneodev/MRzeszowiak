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
using MRzeszowiak.Droid;
using MRzeszowiak.Extends;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(CustomListview), typeof(CustomListviewRenderer))]
namespace MRzeszowiak.Droid
{
    public class CustomListviewRenderer : ListViewRenderer
    {
        public CustomListviewRenderer(Context context) : base(context) { }

        public static void Init() { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            var customListView = Element as CustomListview;
            if (customListView == null) return;

            Control.ScrollStateChanged += (s, ev) =>
            {
                var currentScrollState = ScrollStateChangedEventArgs.ScrollState.Idle;
                var Y = (int) ev.View.GetChildAt(0).GetY();
                if (ev.ScrollState == Android.Widget.ScrollState.Idle)
                    currentScrollState = ScrollStateChangedEventArgs.ScrollState.Idle;
                else
                    currentScrollState = ScrollStateChangedEventArgs.ScrollState.Running;
                CustomListview.OnScrollStateChanged(customListView, new ScrollStateChangedEventArgs(currentScrollState, Y));
            };
        }
    }
}