using System;
using System.Text;

namespace IndependentWork4
{
    public class LibraryItem
    {
     
        private readonly string _uniqueId;
        private readonly string _title;
        private readonly int _publicationYear;

        // властивості
        public string UniqueId => _uniqueId;
        public string Title => _title;
        public int PublicationYear => _publicationYear;

        // конструктор
        public LibraryItem(string uniqueId, string title, int publicationYear)
        {
            _uniqueId = uniqueId;
            _title = title;
            _publicationYear = publicationYear;
        }

        // віртуальний метод
        public virtual void DisplayDetails()
        {
            Console.WriteLine($"[ID: {UniqueId}] '{Title}' ({PublicationYear} р.)");
        }
    }

    // похідний клас Book
    public class Book : LibraryItem
    {
        private readonly string _author;

        public string Author => _author;

         public Book(string uniqueId, string title, int publicationYear, string author)
            : base(uniqueId, title, publicationYear)
        {
            _author = author;
        }

        public override void DisplayDetails()
        {
            Console.Write($"[КНИГА] ");
            base.DisplayDetails();
            Console.WriteLine($"   Автор: {Author}");
        }
    }

    // похідний клас Journal
    public class Journal : LibraryItem
    {
        private readonly int _issueNumber;

        public int IssueNumber => _issueNumber;

         public Journal(string uniqueId, string title, int publicationYear, int issueNumber)
            : base(uniqueId, title, publicationYear)
        {
            _issueNumber = issueNumber;
        }

        public override void DisplayDetails()
        {
            Console.Write($"[ЖУРНАЛ] ");
            base.DisplayDetails();
            Console.WriteLine($"   Випуск №: {IssueNumber}");
        }
    }

    // 5.похідний клас AudioBook
    public class AudioBook : Book
    {
        private readonly string _narrator;

        public string Narrator => _narrator;

        // викликає конструктор Book
        public AudioBook(string uniqueId, string title, int publicationYear, string author, string narrator)
            : base(uniqueId, title, publicationYear, author)
        {
            _narrator = narrator;
        }

        public override void DisplayDetails()
        {
            Console.Write($"[АУДІОКНИГА] ");
            base.DisplayDetails();
            Console.WriteLine($"   Диктор: {Narrator}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Система управління бібліотекою\n");

            // створення об'єктів
            Book book = new Book("B-101", "Кобзар", 1840, "Тарас Шевченко");
            Journal journal = new Journal("J-505", "Наука та техніка", 2023, 12);
            AudioBook audioBook = new AudioBook("A-909", "Тіні забутих предків", 1911, "Михайло Коцюбинський", "Василь Стус");

            // поліморфний виклик
            LibraryItem[] items = new LibraryItem[] { book, journal, audioBook };

            foreach (var item in items)
            {
                item.DisplayDetails();
                Console.WriteLine(new string('-', 40));
            }
        }
    }
}