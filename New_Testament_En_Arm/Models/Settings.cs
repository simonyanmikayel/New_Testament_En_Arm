using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThisApp.Extensions;
using ThisApp.Helpers;
using Windows.Storage;

namespace ThisApp.Models
{
    public class Settings : INotifyPropertyChanged
    {
        public enum UiColor
        {
            Sepa,
            White
        }
        public enum Play
        {
            All,
            Chapter,
            Paragraph,
            None
        }
    public enum Scroll
        {
            Smoothly,
            Fast,
            None
        }
    public ObservableCollection<String> Languages { get; }
        public ObservableCollection<String> FontSizes { get; }
        public ObservableCollection<String> ColorModes { get; }
        public ObservableCollection<String> AudioPlayModes { get; }
        public ObservableCollection<String> ScrollModes { get; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public Settings()
        {
            Languages = new ObservableCollection<string>();
            Languages.Add("English-Armenian");
            Languages.Add("English");
            Languages.Add("Armenian");

            FontSizes = new ObservableCollection<string>();
            FontSizes.Add("XX-Small");
            FontSizes.Add("X-Small");
            FontSizes.Add("Small");
            FontSizes.Add("Medium");
            FontSizes.Add("Large");
            FontSizes.Add("X-Large");
            FontSizes.Add("XX-Large");

            ColorModes = new ObservableCollection<string>();
            ColorModes.Add(UiColor.Sepa.ToString());
            ColorModes.Add(UiColor.White.ToString());

            AudioPlayModes = new ObservableCollection<string>();
            AudioPlayModes.Add(Play.All.ToString());
            AudioPlayModes.Add(Play.Chapter.ToString());
            AudioPlayModes.Add(Play.Paragraph.ToString());
            AudioPlayModes.Add(Play.None.ToString());

            ScrollModes = new ObservableCollection<string>();
            ScrollModes.Add(Scroll.Smoothly.ToString());
            ScrollModes.Add(Scroll.Fast.ToString());
            ScrollModes.Add(Scroll.None.ToString());

            LoadSettings();
        }
        private int _Language;
        public int Language
        {
            get { return _Language; }
            set { Dbg.d(); _Language = value; OnPropertyChanged(); }
        }
        private int _FontSize;
        public int FontSize
        {
            get { return _FontSize; }
            set { Dbg.d(); _FontSize = value; OnPropertyChanged(); }
        }
        private UiColor _ColorMode;
        public UiColor ColorMode
        {
            get { return _ColorMode; }
            set { Dbg.d(); _ColorMode = value; OnPropertyChanged(); }
        }
        private int _HighlightParagraph;
        public bool HighlightParagraph
        {
            get { return _HighlightParagraph != 0; }
            set { Dbg.d(); _HighlightParagraph = value ? 1 : 0; OnPropertyChanged(); }
        }
        private Play _AudioPlaylMode;
        public Play AudioPlayMode
        {
            get { return _AudioPlaylMode; }
            set { Dbg.d(); _AudioPlaylMode = value; OnPropertyChanged(); }
        }
        private Scroll _ScrollMode;
        public Scroll ScrollMode
        {
            get { return _ScrollMode; }
            set { Dbg.d(); _ScrollMode = value; OnPropertyChanged(); }
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            SaveSetting(propertyName);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #region serialisation
        private void SaveSetting(string propertyName)
        {
            //if (0 == string.Compare(propertyName, nameof(Language), StringComparison.InvariantCulture))
            LocalSettings.Values[nameof(Language)] = _Language;
            LocalSettings.Values[nameof(FontSize)] = _FontSize;
            LocalSettings.Values[nameof(ColorMode)] = (int)_ColorMode;
            LocalSettings.Values[nameof(HighlightParagraph)] = _HighlightParagraph;
            LocalSettings.Values[nameof(AudioPlayMode)] = (int)_AudioPlaylMode;
            LocalSettings.Values[nameof(ScrollMode)] = (int)_ScrollMode;
        }
        private void LoadSettings()
        {
            _Language = LocalSettings.GetInt(nameof(Language), 0);
            _FontSize = LocalSettings.GetInt(nameof(FontSize), 3);
            _ColorMode = LocalSettings.GetEnum<UiColor>(nameof(ColorMode), UiColor.Sepa);

            _HighlightParagraph = LocalSettings.GetInt(nameof(HighlightParagraph), 1);
            _AudioPlaylMode = LocalSettings.GetEnum<Play>(nameof(AudioPlayMode), Play.All);
            _ScrollMode = LocalSettings.GetEnum<Scroll>(nameof(ScrollMode), Scroll.Smoothly);
        }
        public String GetSettings()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_Language); sb.Append(';');
            sb.Append(_FontSize); sb.Append(';');
            sb.Append((int)_ColorMode); sb.Append(';');
            sb.Append(_HighlightParagraph); sb.Append(';');
            sb.Append((int)_AudioPlaylMode); sb.Append(';');
            sb.Append((int)_ScrollMode); sb.Append(';');
            return sb.ToString();
        }
        #endregion
    }
}
