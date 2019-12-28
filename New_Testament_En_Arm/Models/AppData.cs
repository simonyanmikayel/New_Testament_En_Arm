using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewTestamentEnArm.Extensions;
using NewTestamentEnArm.Helpers;
using Windows.Storage;

namespace NewTestamentEnArm.Models
{
    public static class AppData
    {
        public static Settings Settings { get; private set; }
        public static List<Chapter> Chapters { get; private set; }
        public static int SelectedIndex { get; private set; }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AppData()
        {
            Settings = new Settings();
            Chapters = new List<Chapter>();
            LoadChapters();
        }

        public static Chapter SavedChapter{ get; set; }
        public static Chapter SelectedChapter 
        { 
            get 
            {
                Debug.Assert(SelectedIndex < Chapters.Count);
                return Chapters[SelectedIndex]; 
            } 
        }

        public static void SetSelectedChapter(Chapter chapter)
        {
            int index = Chapters.IndexOf(chapter);
            Debug.Assert(index >= 0);
            SelectedIndex = index;
            Dbg.d("chapter.ID " + chapter.Id + " index " + SelectedIndex);
        }
        public static void AddChapter(Chapter chapter)
        {
            Debug.Assert(Chapters.IndexOf(chapter) < 0);
            Chapters.Add(chapter);
        }
        public static void RemoveChapter(Chapter chapter)
        {
            Debug.Assert(Chapters.IndexOf(chapter) >= 0);
            Chapters.Remove(chapter);
        }
        #region serialisation
        public static void SaveChapters()
        {
            if (Chapters.Count > SelectedIndex)
            {
                Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
                composite["bookNumber"] = Chapters[SelectedIndex].BookNumber;
                composite["chapterNumber"] = Chapters[SelectedIndex].ChapterNumber;
                composite["paragraph"] = Chapters[SelectedIndex].Paragraph;
                LocalSettings.Values["chapter"] = composite;
            }
        }
        private static void LoadChapters()
        {
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
