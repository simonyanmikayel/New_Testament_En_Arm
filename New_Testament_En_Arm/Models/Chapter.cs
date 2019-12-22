using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThisApp.Models
{
    public class Chapter
    {
        public Book Book { get; }
        public int BookNumber { get; }
        public int ChapterNumber { get; }
        public int Paragraph { get; set; }
        static int _objectNN;
        public int ID { get; }
        public Chapter(int bookNumber, int chapterNumber, int paragraph)
        {
            ID = ++_objectNN;
            if (bookNumber < 0 || bookNumber > Book.BookList.Length)
                bookNumber = 0;
            BookNumber = bookNumber;
            Book = Book.BookList[BookNumber];

            if (chapterNumber < 0 || chapterNumber > Book.ChapterCount)
                chapterNumber = 0;
            ChapterNumber = chapterNumber;
            Paragraph = paragraph;
        }
        public string BookTabHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Book.Name);
            sb.Append(" ");
            if (Book.ChapterCount > 1)
                sb.Append(ChapterNumber + 1);
            return sb.ToString();
        }
        private string BookContentPath(bool sound)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sound ? "ms-appx:///" : "ms-appx-web:///");
            sb.Append("Assets/New_Testament_in_English_and_Armenian/");
            sb.Append(sound ? "sound/" : "content/");
            sb.Append(Book.Folder);
            sb.Append("/");
            sb.Append(sound ? Book.SoundFile : Book.Folder);
            if (Book.ChapterCount > 1)
                sb.Append(ChapterNumber + 1);
            sb.Append(sound ? "_e.mp3" : ".html");
            return sb.ToString();
        }
        public string BookTextPath()
        {
            return BookContentPath(false);
        }
        public string BookSoundPath()
        {
            return BookContentPath(true);
        }
        public bool HasPreviousChapter()
        {
            return (ChapterNumber > 0) || (BookNumber > 0);
        }
        public Tuple<int, int> PreviousChapter()
        {
            if (ChapterNumber > 0)
                return new Tuple<int, int>(BookNumber, ChapterNumber - 1);
            else if (BookNumber > 0)
                return new Tuple<int, int>(BookNumber -1, Book.BookList[BookNumber-1].ChapterCount - 1);
            else
                return null;
        }
        public bool HasNextChapter()
        {
            return (ChapterNumber < Book.ChapterCount - 1) || (BookNumber < Book.BookList.Length - 1);
        }
        public Tuple<int,int> NextChapter()
        {
            if (ChapterNumber < Book.ChapterCount - 1)
                return new Tuple<int, int>(BookNumber, ChapterNumber + 1);
            else if (BookNumber < Book.BookList.Length - 1)
                return new Tuple<int, int>(BookNumber + 1, 0);
            else
                return null;
        }
    }
}
