﻿using System;
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

[assembly: ExportRenderer(typeof(SelectableLabel), typeof(SelectableLabelRenderer))]
namespace MRzeszowiak.Droid
{
    public class SelectableLabelRenderer : EditorRenderer
    {
        public SelectableLabelRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            Control.Background = null;
            Control.SetPadding(0, 0, 0, 0);
            Control.ShowSoftInputOnFocus = false;
            Control.Focusable = false;
            Control.SetTextIsSelectable(true);
            Control.CustomSelectionActionModeCallback = new CustomSelectionActionModeCallback();
            Control.CustomInsertionActionModeCallback = new CustomInsertionActionModeCallback();
        }
        protected override FormsEditText CreateNativeControl() =>  new CustomEditText(Context);
    }

    public class CustomEditText : FormsEditText
    {
        public CustomEditText(Context context) : base(context)
        {
            CustomSelectionActionModeCallback = new CustomSelectionActionModeCallback();
            CustomInsertionActionModeCallback = new CustomInsertionActionModeCallback();
            SetCursorVisible(false);
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow(); 
            Enabled = false;
            Enabled = true;
            
        }

        public new bool IsSuggestionsEnabled = false;  

        public override bool OnTextContextMenuItem(int id)
        {
            switch (id)
            {
                case Android.Resource.Id.Paste:
                case Android.Resource.Id.PasteAsPlainText:
                case Android.Resource.Id.Cut:
                    return false;
            }
            return base.OnTextContextMenuItem(id);
        }
    }

    public class CustomInsertionActionModeCallback : Java.Lang.Object, ActionMode.ICallback
    {
        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            try
            {
                var copyItem = menu.FindItem(Android.Resource.Id.Copy);
                var title = copyItem.TitleFormatted;
                menu.Clear();
                menu.Add(0, Android.Resource.Id.Copy, 0, title);
            }
            catch
            {
                // ignored
            }
            return true;
        }

        public bool OnActionItemClicked(ActionMode m, IMenuItem i) => false;

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            try
            {
                var copyItem = menu.FindItem(Android.Resource.Id.Copy);
                var title = copyItem.TitleFormatted;
                menu.Clear();
                menu.Add(0, Android.Resource.Id.Copy, 0, title);
            }
            catch
            {
                // ignored
            }

            return true;
        }

        public void OnDestroyActionMode(ActionMode mode) { }
    }

    public class CustomSelectionActionModeCallback : Java.Lang.Object, ActionMode.ICallback
    {
        private const int CopyId = Android.Resource.Id.Copy;

        public bool OnActionItemClicked(ActionMode m, IMenuItem i) => false;

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            try
            {
                var copyItem = menu.FindItem(Android.Resource.Id.Copy);
                var title = copyItem.TitleFormatted;
                menu.Clear();
                menu.Add(0, Android.Resource.Id.Copy, 0, title);
            }
            catch
            {
                // ignored
            }
            return true;
        }

        public void OnDestroyActionMode(ActionMode mode) { }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            try
            {
                var copyItem = menu.FindItem(CopyId);
                var title = copyItem.TitleFormatted;
                menu.Clear();
                menu.Add(0, CopyId, 0, title);
            }
            catch
            {
                // ignored
            }

            return true;
        }
    }
}