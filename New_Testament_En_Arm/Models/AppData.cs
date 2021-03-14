using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using NewTestamentEnArm.Controls;
using NewTestamentEnArm.Extensions;
using NewTestamentEnArm.Helpers;
using Windows.Storage;

namespace NewTestamentEnArm.Models
{
    public static class AppData
    {
        public static Settings Settings { get; private set; }
        public static List<CustomTabViewItem> CustomTabViewItems { get; private set; }
        public static int SelectedIndex { get; private set; }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AppData()
        {
            Settings = new Settings();
            CustomTabViewItems = new List<CustomTabViewItem>();
            LoadChapters();
        }

        public static Chapter SavedChapter{ get; set; }
        public static Chapter SelectedChapter 
        { 
            get 
            {
                Debug.Assert(SelectedIndex < CustomTabViewItems.Count);
                return CustomTabViewItems[SelectedIndex].Chapter; 
            } 
        }
        public static int IndexOfItem(TabViewItem item)
        {
            int index = -1;
            for (int i = 0; i < CustomTabViewItems.Count; i++)
            {
                if (CustomTabViewItems[i].TabItem == item)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        public static void SetSelectedItem(TabViewItem item)
        {
            int index = IndexOfItem(item);
            Debug.Assert(index >= 0 && index < CustomTabViewItems.Count);
            SelectedIndex = index;
        }
        public static void AddChapter(CustomTabViewItem customTabViewItem)
        {
            Debug.Assert(IndexOfItem(customTabViewItem.TabItem) < 0);
            CustomTabViewItems.Add(customTabViewItem);
        }
        public static void RemoveChapter(TabViewItem item)
        {
            int index = IndexOfItem(item);
            Debug.Assert(index >= 0);
            CustomTabViewItems.RemoveAt(index);
        }
        public static int ChapterCount()
        {
            return CustomTabViewItems.Count;
        }
        #region serialisation
        public static void SaveChapters()
        {
            if (CustomTabViewItems.Count > SelectedIndex)
            {
                Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
                composite["bookNumber"] = CustomTabViewItems[SelectedIndex].Chapter.BookNumber;
                composite["chapterNumber"] = CustomTabViewItems[SelectedIndex].Chapter.ChapterNumber;
                composite["paragraph"] = CustomTabViewItems[SelectedIndex].Chapter.Paragraph;
                LocalSettings.Values["chapter"] = composite;
            }
        }
        private static void LoadChapters()
        {
            //LocalSettings.Values.Remove("chapter"); //!!!
            Windows.Storage.ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)LocalSettings.Values["chapter"];
            if (composite != null)
            {
                if ((composite["bookNumber"] is int bookNumber) &&
                    (composite["chapterNumber"] is int chapterNumber) &&
                     composite["paragraph"] is int paragraph)
                {
                    if (bookNumber < Book.BookList.Length && 
                        chapterNumber < Book.BookList[bookNumber].ChapterCount)
                        SavedChapter = new Chapter(bookNumber, chapterNumber, paragraph);
                }
            }
        }
        #endregion
    }
}
