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
using MRzeszowiak;
using MRzeszowiak.Droid;
using MRzeszowiak.Extends;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MultiLineLabel), typeof(MultiLineLabelRendererDroid))]
namespace MRzeszowiak.Droid
{
    public class MultiLineLabelRendererDroid : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var baseLabel = (MultiLineLabel)this.Element;

            Control.SetLines(baseLabel.Lines);
        }
    }
}