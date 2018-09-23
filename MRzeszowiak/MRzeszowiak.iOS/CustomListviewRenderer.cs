using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using MRzeszowiak.Extends;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomListview), typeof(CustomListviewRenderer))]
namespace MRzeszowiak.iOS
{
    public class CustomListviewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            var customListview = Element as CustomListview;
            if (customListview == null)
                return;

            if (Control != null)
            {
                var tvDelegate = new TableViewDelegate();
                Control.Delegate = tvDelegate;

                tvDelegate.OnDecelerationStarted += (s, ev) =>
                {
                    CustomListview.OnScrollStateChanged(customListview,
                        new ScrollStateChangedEventArgs(ScrollStateChangedEventArgs.ScrollState.Running));
                };

                tvDelegate.OnDecelerationEnded += (s, ev) =>
                {
                    CustomListview.OnScrollStateChanged(customListview,
                        new ScrollStateChangedEventArgs(ScrollStateChangedEventArgs.ScrollState.Idle));
                };

                //tvDelegate.OnRowSelected += (s, ev) =>
                //{
                //    var index = s as NSIndexPath;
                //    CustomListview.OnItemTappedIOS(customListview, new ItemTappedIOSEventArgs(index.Row));
                //};
            }
        }

        /// <summary>
        /// Problem: Event registration is overwriting existing delegate. Either just use events or your own delegate:
        /// Solution: Create your own delegate and overide the required events
        /// </summary>

        public class TableViewDelegate : UIKit.UITableViewDelegate
        {
            public event EventHandler OnDecelerationEnded;
            public event EventHandler OnDecelerationStarted;
            public event EventHandler OnDidZoom;
            public event EventHandler OnDraggingStarted;
            public event EventHandler OnDraggingEnded;
            public event EventHandler OnScrollAnimationEnded;
            public event EventHandler OnScrolled;
            public event EventHandler OnScrolledToTop;
            public event EventHandler OnRowSelected;

            public override void DecelerationEnded(UIKit.UIScrollView scrollView)
            {
                OnDecelerationEnded?.Invoke(scrollView, null);
            }
            public override void DecelerationStarted(UIKit.UIScrollView scrollView)
            {
                OnDecelerationStarted?.Invoke(scrollView, null);
            }
            public override void DidZoom(UIKit.UIScrollView scrollView)
            {
                OnDidZoom?.Invoke(scrollView, null);
            }
            public override void DraggingStarted(UIKit.UIScrollView scrollView)
            {
                OnDraggingStarted?.Invoke(scrollView, null);
            }
            public override void DraggingEnded(UIKit.UIScrollView scrollView, bool willDecelerate)
            {
                OnDraggingEnded?.Invoke(scrollView, null);
            }
            public override void ScrollAnimationEnded(UIKit.UIScrollView scrollView)
            {
                OnScrollAnimationEnded?.Invoke(scrollView, null);
            }
            public override void Scrolled(UIKit.UIScrollView scrollView)
            {
                OnScrolled?.Invoke(scrollView, null);
            }
            public override void ScrolledToTop(UIKit.UIScrollView scrollView)
            {
                OnScrolledToTop?.Invoke(scrollView, null);
            }
            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                OnRowSelected?.Invoke(indexPath, null);
            }
        }
    }
}