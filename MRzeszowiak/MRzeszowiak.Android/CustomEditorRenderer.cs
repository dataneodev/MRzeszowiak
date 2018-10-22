using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using MRzeszowiak.Droid;
using MRzeszowiak.Extends;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace MRzeszowiak.Droid
{
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer(Context context):base(context) {  }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                this.Control.SetBackgroundDrawable(gd);
                this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
                //  Control.SetHintTextColor(ColorStateList.ValueOf(global::Android.Graphics.Color.White));
            }
        }
    }
}  