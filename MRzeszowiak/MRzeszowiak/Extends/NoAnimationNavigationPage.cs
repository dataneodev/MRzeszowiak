using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MRzeszowiak.Extends
{
    public class NoAnimationNavigationPage : NavigationPage
    {
        public NoAnimationNavigationPage() : base(){ }
        public NoAnimationNavigationPage(Page startupPage) : base(startupPage){ }
    }
}
