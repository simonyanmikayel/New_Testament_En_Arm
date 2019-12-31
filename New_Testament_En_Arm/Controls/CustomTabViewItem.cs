using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using NewTestamentEnArm.Controls;
using Windows.UI.Xaml.Controls;
using NewTestamentEnArm.Models;

namespace NewTestamentEnArm.Controls
{
    public class CustomTabViewItem
    {
        static int _objectNN;
        public int Id { get; }
        public Chapter Chapter { get; }
        public TabViewItem TabItem { get; }
        public TabContent TabContent { get; }
        public CustomTabViewItem(Chapter chapter)
        {
            //this.DefaultStyleKey = typeof(CustomTabViewItem);
            TabItem = new TabViewItem();
            Id = ++_objectNN;
            Chapter = chapter;
            TabItem.Header = Chapter.BookTabHeader();
            //Icon = new SymbolIcon(Symbol.Document);
            BitmapIcon bitmapIcon = new BitmapIcon();
            bitmapIcon.ShowAsMonochrome = false;
            bitmapIcon.UriSource = new Uri("ms-appx:///Assets/favicon.png");
            TabItem.Icon = bitmapIcon;
            TabContent = new TabContent(chapter);
            // we intentionaly left Conten uninitialized. TabContent becomes content of page
            //TabItem.Content = new TabContent(chapter);
        }
    }
}
