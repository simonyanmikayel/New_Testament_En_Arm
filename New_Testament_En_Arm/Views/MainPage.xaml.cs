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
using NewTestamentEnArm.Models;
using System.Diagnostics;
using System.Globalization;
using NewTestamentEnArm.Controls;
using Windows.UI.Xaml.Navigation;
using NewTestamentEnArm.Helpers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NewTestamentEnArm.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public Settings Settings { get; }
        private DispatcherTimerTick _tickEvent = new DispatcherTimerTick();
        //private ThreadPoolTimerTick _tickEvent = new ThreadPoolTimerTick();
        public static MainPage TheMainPage { get; private set; }

        public AppMenu AppMenu { get; private set; }

        public MainPage()
        {
            InitializeComponent();
            TheMainPage = this;
            Settings = AppData.Settings;
            AppMenu = new AppMenu(MenuItem_Click);

            AppData.Settings.PropertyChanged += Settings_PropertyChanged;
            ApplyUiColors();
            // Set XAML element as draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Hide default title bar.
            // https://docs.microsoft.com/en-us/windows/uwp/design/shell/title-bar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // Register for changes
            coreTitleBar.LayoutMetricsChanged += this.CoreTitleBar_LayoutMetricsChanged;
            CoreTitleBar_LayoutMetricsChanged(coreTitleBar, null);

            coreTitleBar.IsVisibleChanged += this.CoreTitleBar_IsVisibleChanged;

            // Listen for Fullscreen Changes from Shift+Win+Enter or our F11 shortcut
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += this.MainPage_VisibleBoundsChanged;

            if (AppData.SavedChapter == null)
            {
                WelcomeBubble.Visibility = Visibility.Visible;
                AddTabButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                SetTabItem(AppData.SavedChapter.BookNumber, AppData.SavedChapter.ChapterNumber, AppData.SavedChapter.Paragraph, true);
                AppData.SavedChapter = null;
            }
        }
        private  void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ApplyUiColors()
        {
            Windows.UI.Color uiColor = AppData.Settings.UiColor;
            Brush uiBrush = new SolidColorBrush(AppData.Settings.UiColor);
            this.Background = uiBrush;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = uiColor;
            titleBar.ButtonInactiveBackgroundColor = uiColor;
            AppTitleBar.Background = uiBrush;
            //MyTabView.Background = uiBrush;
        }
        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Settings.ColorMode), StringComparison.Ordinal))
            {
                ApplyUiColors();
            }
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
        bool _IsFullScreen;
        public bool IsFullScreen
        {
            get { return _IsFullScreen; }
            set { Set<bool>(ref _IsFullScreen, value); }
        }
        private void MainPage_VisibleBoundsChanged(ApplicationView sender, object args)
        {
            // Update Fullscreen from other modes of adjusting view (keyboard shortcuts)
            IsFullScreen = ApplicationView.GetForCurrentView().IsFullScreenMode;
            //ApplyUiColors();
        }

        private void AppFullScreenShortcut(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            // Toggle FullScreen from F11 Keyboard Shortcut
            if (!IsFullScreen)
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            else
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
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
        TabContent _ActiveTabContent;
        public TabContent ActiveTabContent { 
            get
            {
                return _ActiveTabContent;
            }
            private set
            {
                Set<TabContent>(ref _ActiveTabContent, value);
            }
        }
        private void AddTabButtonClick(object sender, RoutedEventArgs e)
        {
            if (AppData.ChapterCount() > 0)
                SetTabItem(AppData.SelectedChapter.BookNumber, AppData.SelectedChapter.ChapterNumber, 0, true);
            else
                SetTabItem(0, 0, 0, true);
        }
        public void SetTabItem(int bookNumber, int chapterNumber, int paragraphNumber, bool newTab)
        {
            if (WelcomeBubble.Visibility == Visibility.Visible)
            {
                WelcomeBubble.Visibility = Visibility.Collapsed;
                AddTabButton.Visibility = Visibility.Visible;
            }
            Chapter chapter = new Chapter(bookNumber, chapterNumber, paragraphNumber);
            CustomTabViewItem customTabViewItem = new CustomTabViewItem(chapter);
            if (newTab || AppData.ChapterCount() == 0)
            {
                AppData.AddChapter(customTabViewItem);
                AppData.SetSelectedItem(customTabViewItem.TabItem);
                MyTabView.Items.Add(customTabViewItem.TabItem);
                _tickEvent.Triger(() =>
                {
                    MyTabView.SelectedIndex = MyTabView.Items.Count - 1;
                });
            }
            else
            {
                AppData.CustomTabViewItems[AppData.SelectedIndex] = customTabViewItem;
                MyTabView.Items[MyTabView.SelectedIndex] = customTabViewItem.TabItem;
                _tickEvent.Triger(() =>
                {
                    MyTabView.SelectedIndex = AppData.SelectedIndex;
                });
            }
        }
        private void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var addedItem = e.AddedItems.FirstOrDefault();
            if (addedItem is TabViewItem addedTabItem)
            {
                Debug.Assert(MyTabView.SelectedIndex == AppData.SelectedIndex);
                AppData.SetSelectedItem(addedTabItem);
                ApplicationView.GetForCurrentView().Title = addedTabItem.Header.ToString();
                ActiveTabContent = AppData.CustomTabViewItems[AppData.SelectedIndex].TabContent;
            }
        }
        private void TabClosing(object sender, TabClosingEventArgs e)
        {
            if (AppData.ChapterCount() == 1)
            {
                AppData.SaveChapters();
                CoreApplication.Exit();
            }
            else if (e.Item is TabViewItem closedTabItem)
            {
                bool selectedClosed = (AppData.CustomTabViewItems[AppData.SelectedIndex].TabItem == closedTabItem);
                AppData.RemoveChapter(closedTabItem);
                if (!selectedClosed)
                {
                    AppData.SetSelectedItem(closedTabItem);
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
            SetTabItem(menuItem.BookNumber, menuItem.ChapterNumber, 0, false);
        }
        #endregion
    }
}
