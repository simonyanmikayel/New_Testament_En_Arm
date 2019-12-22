using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System.Windows.Input;
using Windows.UI.Xaml.Controls.Primitives;

namespace ThisApp.Models
{
    public class AppMenu : MenuFlyout
    {
        public AppMenu(EventHandler ItemClicked)
        {
            Placement = FlyoutPlacementMode.Bottom;

            for (int bookNumber = 0; bookNumber < Book.BookList.Length; bookNumber++)
            {
                MenuFlyoutSubItem subItem = new MenuFlyoutSubItem();
                Book book = Book.BookList[bookNumber];
                subItem.Text = book.Caption;
                for (int chapterNumber = 0; chapterNumber < book.ChapterCount; chapterNumber++)
                {
                    subItem.Items.Add(new AppMenuItem(ItemClicked, bookNumber, chapterNumber));
                }
                Items.Add(subItem);
            }

        }
    }

    public class AppMenuItem : MenuFlyoutItem, ICommand
    {
        public int BookNumber { get; }
        public int ChapterNumber { get; }
#pragma warning disable 67 //is never used 
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
        private EventHandler ItemClicked;
        public AppMenuItem(EventHandler ItemClicked, int bookNumber, int chapterNumber)
        {
            this.ItemClicked = ItemClicked;
            this.BookNumber = bookNumber;
            this.ChapterNumber = chapterNumber;
            Command = this;
            CommandParameter = this;
            Book book = Book.BookList[BookNumber];
            Text = book.Name;
            if (book.ChapterCount > 1)
                Text = Text + " " + (chapterNumber + 1);
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            ItemClicked(parameter, EventArgs.Empty);
        }
    }
}
