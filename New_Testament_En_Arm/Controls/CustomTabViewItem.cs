using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using ThisApp.Controls;
using Windows.UI.Xaml.Controls;
using ThisApp.Models;

namespace ThisApp.Controls
{
    public class CustomTabViewItem : TabViewItem
    {
        static int _objectNN;
        int _objectID;
        public Chapter Chapter { get; }

        public CustomTabViewItem(Chapter chapter)
        {
            //this.DefaultStyleKey = typeof(CustomTabViewItem);
            _objectID = ++_objectNN;
            Chapter = chapter;
            Header = Chapter.BookTabHeader();
            //Icon = new SymbolIcon(Symbol.Document);
            BitmapIcon bitmapIcon = new BitmapIcon();
            bitmapIcon.ShowAsMonochrome = false;
            bitmapIcon.UriSource = new Uri("ms-appx:///Assets/favicon.png");
            Icon = bitmapIcon;
            // we intentionaly left Conten uninitialized. TabContent becomes content of page
            TabContent = new TabContent(chapter);
        }
        public TabContent TabContent { get; }
    }
}
