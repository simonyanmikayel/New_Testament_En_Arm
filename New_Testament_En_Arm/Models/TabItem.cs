using Microsoft.Toolkit.Uwp.UI.Controls;
using ThisApp.Controls;
using Windows.UI.Xaml.Controls;

namespace ThisApp.Models
{
    public class TabItem : TabViewItem
    {
        static int _objectNN;
        int _objectID;
        public Chapter Chapter { get; }
         public TabItem(Chapter chapter)
        {
            _objectID = ++_objectNN;
            Chapter = chapter;
            Header = Chapter.BookTabHeader();
            Icon = new SymbolIcon(Symbol.Document);
            // we intentionaly left Conten uninitialized. ItemContent becomes content of page
            TabContent = new TabContent(chapter);
        }
        public TabContent TabContent { get; }
    }
}
