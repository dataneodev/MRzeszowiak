using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using MRzeszowiak.Extends;
using MRzeszowiak.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MultiLineLabel), typeof(MultiLineLabelRendererIOS))]
namespace MRzeszowiak.iOS
{
    public class MultiLineLabelRendererIOS : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var baseLabel = (MultiLineLabel)this.Element;

            Control.Lines = baseLabel.Lines;
        }

    }
}