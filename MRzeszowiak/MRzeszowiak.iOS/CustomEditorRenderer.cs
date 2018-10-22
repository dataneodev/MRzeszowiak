
using MRzeszowiak.Extends;
using MRzeszowiak.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace MRzeszowiak.iOS
{
    public class CustomEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {

            base.OnElementChanged(e);
            if (Control != null)
            {

                //Control.BorderStyle = UITextBorderStyle.None;
                //Control.Layer.CornerRadius = 10;
                //Control.TextColor = UIColor.White;
            }
        }
    }
}