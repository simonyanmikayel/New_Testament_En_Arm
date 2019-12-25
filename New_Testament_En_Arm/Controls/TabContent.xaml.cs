using ThisApp.Models;
using System;
using System.Globalization;
using WebViewRuntimeComponent;
using Windows.ApplicationModel.Core;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ThisApp.Helpers;
using ThisApp.Views;
using Windows.UI.Xaml.Media;

namespace ThisApp.Controls
{
    public sealed partial class TabContent : UserControl, IWebViewEventHandler
    {
        static CultureInfo inv = CultureInfo.InvariantCulture;
        String _soundPosString = null;
        double[] _soundPosList = null;
        double _NaturalDuration = -1;
        int _curParagraph = -1;
        string[] _scrollArgs = new string[1];
        string[] _handleSettingsArgs = new string[1];
        Chapter _chapter;
        static int _objectNN;
        int _objectID;
        bool _firstLoaded = true;
        
        public TabContent(Chapter chapter)
        {
            _objectID = ++_objectNN;
            _chapter = chapter;
            this.InitializeComponent();

            ApplyUiColors();

            MyWebView.Settings.IsJavaScriptEnabled = true; //enabled by default

            AppData.Settings.PropertyChanged += TabContent_PropertyChanged;

            MyPlayer.MediaPlayer.CommandManager.PreviousBehavior.EnablingRule = _chapter.HasPreviousChapter() ? MediaCommandEnablingRule.Always : MediaCommandEnablingRule.Never;
            MyPlayer.TransportControls.IsPreviousTrackButtonVisible = _chapter.HasPreviousChapter() ? true : false;
            MyPlayer.MediaPlayer.CommandManager.NextBehavior.EnablingRule = _chapter.HasNextChapter() ? MediaCommandEnablingRule.Always : MediaCommandEnablingRule.Never;
            MyPlayer.TransportControls.IsNextTrackButtonVisible = _chapter.HasNextChapter() ? true : false;

            MyPlayer.MediaPlayer.CommandManager.NextReceived += CommandManager_NextReceived;
            MyPlayer.MediaPlayer.CommandManager.PreviousReceived += CommandManager_PreviousReceived;
            MyPlayer.MediaPlayer.PlaybackSession.NaturalDurationChanged += PlaybackSession_NaturalDurationChanged;
            MyPlayer.Source = MediaSource.CreateFromUri(new Uri(_chapter.BookSoundPath()));
            MyWebView.Navigate(new Uri(_chapter.BookTextPath()));
        }

        private void ApplyUiColors()
        {
            Windows.UI.Color uiColor = AppData.Settings.UiColor;
            //Brush uiBrush = new SolidColorBrush(AppData.Settings.UiColor);
            MyWebView.DefaultBackgroundColor = uiColor;
        }
        private async void TabContent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Settings.ColorMode), StringComparison.Ordinal))
            {
                ApplyUiColors();
            }
            _handleSettingsArgs[0] = GetSettings();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await MyWebView.InvokeScriptAsync("handle_settings", _handleSettingsArgs);
            });
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dbg.d();
            MyPlayer.MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;
            MyPlayer.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            if (_firstLoaded)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (_chapter.Paragraph > 0)
                        SetPlaybackPos(_chapter.Paragraph);
                    else
                        _firstLoaded = false;
                });
            }
            if (AppData.Settings.AudioPlayMode != Settings.AudioPlayModeType.None)
                MyPlayer.MediaPlayer.Play();
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dbg.d();
            MyPlayer.MediaPlayer.Pause();
            MyPlayer.MediaPlayer.PlaybackSession.PositionChanged -= PlaybackSession_PositionChanged;
            MyPlayer.MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
        }
        private void InitSoundList()
        {
            if (_soundPosString == null || _NaturalDuration <= 0)
                return;
            Dbg.d();

            String[] strlist = _soundPosString.Split(',');
            _soundPosList = new double[strlist.Length];
            double sound_max = double.Parse(strlist[strlist.Length - 1], inv);
            double k = _NaturalDuration / sound_max;
            for (int i = 0; i < strlist.Length; i++)
            {
                double d = double.Parse(strlist[i], inv);
                long pos = (long)(10000 * d * k);
                TimeSpan timeSpan = new TimeSpan(pos);
                _soundPosList[i] = timeSpan.TotalMilliseconds;
            }
        }
        private void PlaybackSession_NaturalDurationChanged(MediaPlaybackSession sender, object args)
        {
            _NaturalDuration = sender.NaturalDuration.TotalMilliseconds;
            Dbg.d("_NaturalDuration = " + _NaturalDuration);
            InitSoundList();
        }
        private int paragraphBySoundPos(double pos)
        {
            var par = -1;
            if (_curParagraph >= 0 && pos >= _soundPosList[_curParagraph] && pos < _soundPosList[_curParagraph + 1])
            {
                par = _curParagraph;
            }
            else
            {
                if (pos <= _soundPosList[0])
                {
                    par = 0;
                }
                else
                {
                    //check next
                    if (_curParagraph < _soundPosList.Length - 2 && pos >= _soundPosList[_curParagraph + 1] && pos < _soundPosList[_curParagraph + 2])
                    {
                        par = _curParagraph + 1;
                    }
                    else
                    {
                        for (int i = 0; i < _soundPosList.Length - 1; i++)
                        {
                            if (pos >= _soundPosList[i] && pos < _soundPosList[i + 1])
                            {
                                par = i;
                                break;
                            }
                        }
                    }
                }
            }
            return par;
        }
        private async void scrollToPar(int par)
        {
            //double duration = _soundPosList[_curParagraph + 1] - _soundPosList[_curParagraph];
            _scrollArgs[0] = par.ToString(inv);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                if (_firstLoaded)
                {
                    _firstLoaded = false;
                    await MyWebView.InvokeScriptAsync("restore_pos", _scrollArgs);
                }
                else
                {
                    await MyWebView.InvokeScriptAsync("scroll_to_par", _scrollArgs);
                }
            });
        }
        private async void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object o)
        {
            if (_soundPosList == null)
            {
                //Dbg.d("_soundPosList == null " + (_soundPosList == null));
                return;
            }

            int par = paragraphBySoundPos(sender.Position.TotalMilliseconds);
            if (par < 0 || par == _curParagraph)
            {
                //Dbg.d("par " + par + " _curParagraph " + _curParagraph);
                return;
            }

            _curParagraph = par;
            _chapter.Paragraph = par;
            Dbg.d("_curParagraph " + _curParagraph);

            //if (AppData.Settings.ScrollMode != Settings.Scroll.None)
            scrollToPar(par);

            if (AppData.Settings.AudioPlayMode == Settings.AudioPlayModeType.None ||
                AppData.Settings.AudioPlayMode == Settings.AudioPlayModeType.Paragraph)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    MyPlayer.MediaPlayer.Pause();
                });

            }

        }

        private void WebViewCtrl_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            sender.AddWebAllowedObject("nativeObject", new WebViewEventHandler(this));
        }
        private async void SetPlaybackPos(int par)
        {
            Dbg.d("par " + par);
            if (_soundPosList == null || par < 0 || par >= _soundPosList.Length)
                return;
            //1ms = 1000000ns, TimeSpan gets 100ns
            long pos = (long)(_soundPosList[par] * 10000);
            MyPlayer.MediaPlayer.PlaybackSession.Position = new TimeSpan(pos);
            double pos2 = MyPlayer.MediaPlayer.PlaybackSession.Position.TotalMilliseconds;

            if (AppData.Settings.AudioPlayMode == Settings.AudioPlayModeType.Paragraph)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    MyPlayer.MediaPlayer.Play();
                });
            }
        }
        public void OnParagraphClick(int par, String dbgInfo)
        {
            SetPlaybackPos(par);
        }
        public void OnHtmlLoaded(string soundPosString)
        {
            Dbg.d();
            if (soundPosString == null)
                return;
            _soundPosString = soundPosString;
            InitSoundList();
            //TODO reurn settngs
        }
        public string GetSettings()
        {
            Dbg.d();
            return AppData.Settings.GetSettings();
        }
        private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            Dbg.d("MediaPlayer_MediaEnded " + _curParagraph);
            if (AppData.Settings.AudioPlayMode == Settings.AudioPlayModeType.All)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Tuple<int, int> tuple = _chapter.NextChapter();
                    if (tuple != null)
                        MainPage.TheMainPage.SetTabItem(tuple.Item1, tuple.Item2, false);
                });
            }
        }
        private async void CommandManager_PreviousReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerPreviousReceivedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Tuple<int, int> tuple = _chapter.PreviousChapter();
                if (tuple != null)
                    MainPage.TheMainPage.SetTabItem(tuple.Item1, tuple.Item2, false);
            });
        }

        private async void CommandManager_NextReceived(MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerNextReceivedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Tuple<int, int> tuple = _chapter.NextChapter();
                if (tuple != null)
                    MainPage.TheMainPage.SetTabItem(tuple.Item1, tuple.Item2, false);
            });
        }

    }
}
