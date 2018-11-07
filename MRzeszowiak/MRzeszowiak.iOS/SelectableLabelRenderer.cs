using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using MRzeszowiak.Extends;
using MRzeszowiak.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SelectableLabel), typeof(SelectableLabelRenderer))]
namespace MRzeszowiak.iOS
{
    public class SelectableLabelRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            Control.Selectable = true;
            Control.Editable = false;
            Control.ScrollEnabled = false;
            Control.TextContainerInset = UIEdgeInsets.Zero;
            Control.TextContainer.LineFragmentPadding = 0;
        }
    }
}
