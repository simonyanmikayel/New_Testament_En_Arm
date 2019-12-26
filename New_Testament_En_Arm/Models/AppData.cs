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
            LoadChapters();
        }
       
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
            Dbg.d("chapter.ID " + chapter.ID + " index " + index);
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
            Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
            composite[nameof(SelectedIndex)] = SelectedIndex;
            composite["count"] = Chapters.Count;
            for (int i = 0; i < Chapters.Count; i++)
            {
                composite["bookNumber" + i] = Chapters[i].BookNumber;
                composite["chapterNumber" + i] = Chapters[i].ChapterNumber;
                composite["paragraph" + i] = Chapters[i].Paragraph;
            }
            LocalSettings.Values[nameof(Chapters)] = composite;
        }
        private static void LoadChapters()
        {
            Windows.Storage.ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)LocalSettings.Values[nameof(Chapters)];
            while (composite != null)
            {
                SelectedIndex = composite[nameof(SelectedIndex)].GetInt(0);
                int count = composite["count"].GetInt(0);
                if (count == 0 || SelectedIndex < 0 || SelectedIndex >= count)
                    break;
                Chapters = new List<Chapter>();
                for (int i = 0; i < count; i++)
                {
                    if( (composite["bookNumber"+i] is int bookNumber) && 
                        (composite["chapterNumber"+i] is int chapterNumber) &&
                         composite["paragraph" + i] is int paragraph)
                    {
                        Chapter chapter = new Chapter(bookNumber, chapterNumber, paragraph);
                        AddChapter(chapter);
                    }
                    else
                    {
                        Chapters = null;
                        break;
                    }
                }
                break;
            }
            if (Chapters == null)
            {
                Chapters = new List<Chapter>();
                Chapter chapter = new Chapter(0, 0, 0);
                AddChapter(chapter);
                SelectedIndex = 0;
            }
        }
        #endregion
    }
}
