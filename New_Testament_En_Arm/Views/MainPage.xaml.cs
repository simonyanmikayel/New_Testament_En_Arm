using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.ApplicationModel.Core;
using Helpers;
using Windows.UI.ViewManagement;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThisApp.Models;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ThisApp.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public AppData AppData { get; }
        TickEvent _tickEvent = new TickEvent();
        public static MainPage TheMainPage { get; private set; }

        public bool IsFullScreen
        {
            get { return (bool)GetValue(IsFullScreenProperty); }
            set { SetValue(IsFullScreenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFullScreen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFullScreenProperty =
            DependencyProperty.Register(nameof(IsFullScreen), typeof(bool), typeof(MainPage), new PropertyMetadata(false));

        public AppMenu AppMenu { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            TheMainPage = this;
            AppData = AppData.Instance;
            AppMenu = new AppMenu(MenuItem_Click);

            //var color = Windows.UI.Color.FromArgb(255, 0xFA, 0xED, 0xD0);
            //this.Background = new SolidColorBrush(color);
            //ContentPresenter.Background = new SolidColorBrush(color);
            // /MyTabView.BorderBrush = new SolidColorBrush(color);

            // Hide default title bar.
            // https://docs.microsoft.com/en-us/windows/uwp/design/shell/title-bar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Register for changes
            coreTitleBar.LayoutMetricsChanged += this.CoreTitleBar_LayoutMetricsChanged;
            CoreTitleBar_LayoutMetricsChanged(coreTitleBar, null);

            coreTitleBar.IsVisibleChanged += this.CoreTitleBar_IsVisibleChanged;

            // Set XAML element as draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Listen for Fullscreen Changes from Shift+Win+Enter or our F11 shortcut
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += this.MainPage_VisibleBoundsChanged;

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Windows.UI.Colors.White;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.White;

            int SelectedIndex = AppData.SelectedIndex;
            foreach (Chapter chapter in AppData.Chapters)
            {
                MyTabView.Items.Add(new TabItem(chapter));
            }
            _tickEvent.Triger(() =>
            {
                MyTabView.SelectedIndex = SelectedIndex;
            });
        }

        #region Handle App TitleBar
        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // Adjust our content based on the Titlebar's visibility
            // This is used when fullscreen to hide/show the titlebar when the mouse is near the top of the window automatically.
            MyTabView.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            AppTitleBar.Visibility = MyTabView.Visibility;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            LeftPaddingColumn.Width = new GridLength(sender.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(sender.SystemOverlayRightInset);

            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = sender.Height;
        }
        #endregion

        #region Handle FullScreen
        private void MainPage_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            // Update Fullscreen from other modes of adjusting view (keyboard shortcuts)
            IsFullScreen = ApplicationView.GetForCurrentView().IsFullScreenMode;
        }

        private void AppFullScreenShortcut(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            // Toggle FullScreen from F11 Keyboard Shortcut
            if (!IsFullScreen)
            {
                IsFullScreen = ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            else
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
                IsFullScreen = false;
            }
        }

        private void Button_FullScreen_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to our shortcut key.
            AppFullScreenShortcut(null, null);
        }
        #endregion

        #region Handle Property Change
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region Tab items
        private void AddTabButtonClick(object sender, RoutedEventArgs e)
        {
           SetTabItem(AppData.SelectedChapter.BookNumber, AppData.SelectedChapter.ChapterNumber, true);
        }

        public void SetTabItem(int bookNumber, int chapterNumber, bool newTab)
        {
            Chapter chapter = new Chapter(bookNumber, chapterNumber, 0);
            if (newTab)
            {
                AppData.AddChapter(chapter);
                MyTabView.Items.Add(new TabItem(chapter));
                _tickEvent.Triger(() =>
                {
                    MyTabView.SelectedIndex = MyTabView.Items.Count - 1;
                });
            }
            else
            {
                AppData.Chapters[AppData.SelectedIndex] = chapter;
                MyTabView.Items[AppData.SelectedIndex] = new TabItem(chapter);
                _tickEvent.Triger(() =>
                {
                    TabItem selectedTabItem = MyTabView.Items[AppData.SelectedIndex] as TabItem;
                    MyTabView.SelectedItem = selectedTabItem;
                });
            }
        }
        private void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var addedItem = e.AddedItems.FirstOrDefault();
            if (addedItem is TabItem addedTabItem)
            {
                AppData.SetSelectedChapter(addedTabItem.Chapter);
                ApplicationView.GetForCurrentView().Title = addedTabItem.Header.ToString();
            }
        }
        private void TabClosing(object sender, TabClosingEventArgs e)
        {
            if (AppData.Chapters.Count == 1)
            {
                CoreApplication.Exit();
            }
            else if (e.Item is TabItem closedTabItem)
            {
                Chapter selectedChapter = AppData.SelectedChapter;
                bool selectedClosed = (closedTabItem.Chapter == selectedChapter);
                AppData.RemoveChapter(closedTabItem.Chapter);
                if (!selectedClosed)
                {
                    AppData.SetSelectedChapter(selectedChapter); //updtae SelectedIndex
                }
            }
        }
        #endregion

        #region Main menu
        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as Button);
        }
        private void MenuItem_Click(object sender, EventArgs e)
        {
            AppMenuItem menuItem = sender as AppMenuItem;
            SetTabItem(menuItem.BookNumber, menuItem.ChapterNumber, false);
        }
        #endregion
    }
}
