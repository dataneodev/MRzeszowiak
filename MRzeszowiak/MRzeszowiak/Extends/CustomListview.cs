using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.Extends
{
    public class CustomListview : ListView
    {
        public CustomListview() { }

        public CustomListview(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy) { }

        public event EventHandler<ScrollStateChangedEventArgs> ScrollStateChanged;
        public static void OnScrollStateChanged(object sender, ScrollStateChangedEventArgs e)
        {
            var customListview = (CustomListview)sender;
            customListview.ScrollStateChanged?.Invoke(customListview, e);
        }

        //public event EventHandler<ItemTappedIOSEventArgs> ItemTappedIOS;
        //public static void OnItemTappedIOS(object sender, ItemTappedIOSEventArgs e)
        //{
        //    var customListview = (CustomListview)sender;
        //    customListview.ItemTappedIOS?.Invoke(customListview, e);
        //}
    }

    public class ScrollStateChangedEventArgs : EventArgs
    {
        public ScrollStateChangedEventArgs(ScrollState scrollState, int y)
        {
            this.CurScrollState = scrollState;
            this.Y = y;
        }

        public enum ScrollState
        {
            Idle = 0,
            Running = 1
        }

        public ScrollState CurScrollState { get; }
        public int Y { get; } = 2;
    }
}
