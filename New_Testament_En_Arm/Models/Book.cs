using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThisApp.Models
{
    public class Book
    {
        public string Folder { get; }
        public string Name { get; }
        public int ChapterCount { get; }
        public string SoundFile { get; }
        public string Caption { get; }
        public Book(string Folder, string Name, int ChapterCount, string SoundFile, string Caption)
        {
            this.Folder = Folder;
            this.Name = Name;
            this.ChapterCount = ChapterCount;
            this.SoundFile = SoundFile;
            this.Caption = Caption;
        }

        public static readonly Book[] BookList = 
        {
            new Book("Matt", "Matthew", 28, "Matt", "The Gospel According to Matthew"),
            new Book("Mark", "Mark", 16, "mark", "The Gospel According to Mark"),
            new Book("Luke", "Luke", 24, "luke", "The Gospel According to Luke"),
            new Book("John", "John", 21, "John", "The Gospel According to John"),
            new Book("Acts", "Acts", 28, "ACTS", "Acts"),
            new Book("Romans", "Romans", 16, "ROMANS", "Paul's Letter to the Romans"),
            new Book("1Cor", "1 Corinthias", 16, "1COR", "Paul's First Letter to the Corinthians"),
            new Book("2Cor", "2 Corinthias", 13, "2COR", "Paul's Second Letter to the Corinthians"),
            new Book("Galatians", "Galatians", 6, "Galatians", "Paul's Letter to the Galatians"),
            new Book("Eph", "Ephesians", 6, "EPH", "Paul's Letter to the Ephesians"),
            new Book("Philip", "Philippians", 4, "PHILIP", "Paul's Letter to the Philippians"),
            new Book("Col", "Colossians", 4, "Col", "Paul's Letter to the Colossians"),
            new Book("1Thess", "1 Thessalonians", 5, "1THESS", "Paul's First Letter to the Thessalonians"),
            new Book("2Thess", "2 Thessalonians", 3, "2Thess", "Paul's Second Letter to the Thessalonians"),
            new Book("1Tim", "1 Timothy", 6, "1Tim", "Paul's First Letter to Timothy"),
            new Book("2Tim", "2 Timothy", 4, "2Tim", "Paul's Second Letter to Timothy"),
            new Book("Titus", "Titus", 3, "Titus", "Paul's Letter to Titus"),
            new Book("Philemon", "Philemon", 1, "Philemon", "Paul's Letter to Philemon"),
            new Book("Hebrews", "Hebrews", 13, "Hebrews", "The Letter to the Hebrews"),
            new Book("James", "James", 5, "James", "The Letter from James"),
            new Book("1Peter", "1 Peter", 5, "1PETER", "Peter's First Letter"),
            new Book("2Peter", "2 Peter", 3, "2Peter", "Peter's Second Letter"),
            new Book("1John", "1 John", 5, "1JOHN", "John's First Letter"),
            new Book("2John", "2 John", 1, "2John", "John's Second Letter"),
            new Book("3John", "3 John", 1, "3John", "John's Third Letter"),
            new Book("Jude", "Jude", 1, "Jude", "The Letter from Jude"),
            new Book("Rev", "Revelation", 22, "REV", "The Revelation to John"),
        };
    }
}
