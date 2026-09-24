using System;

namespace Lab3
{
    public class DatabaseConnection : IDisposable
    {
        private bool _disposed = false; 
        private string _connectionString;
        private bool _isConnected;

       
        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        public bool IsConnected => _isConnected;

        public DatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
            _isConnected = true;
            Console.WriteLine($" З'єднання з БД створено: {_connectionString}");
        }

        public void ExecuteQuery(string query)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DatabaseConnection), "Неможливо виконати запит: з'єднання вже закрито або знищено!");
            }

            if (_isConnected)
            {
                Console.WriteLine($" Виконання запиту: \"{query}\" через {_connectionString}");
            }
            else
            {
                Console.WriteLine(" Немає активного з'єднання з БД.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine(" Звільнення керованих ресурсів...");
                }

                if (_isConnected)
                {
                    _isConnected = false;
                    Console.WriteLine($" З'єднання з БД закрито: {_connectionString}");
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        ~DatabaseConnection()
        {
            Console.WriteLine("Виклик деструктора (~DatabaseConnection) Garbage Collector'ом");
            Dispose(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" 1. Використання блоку using ");
            using (var db1 = new DatabaseConnection("Server=ServerA;Database=TestDB;"))
            {
                db1.ExecuteQuery("SELECT * FROM Users");
            }

            Console.WriteLine("\n2. Явний виклик Dispose() без using");
            var db2 = new DatabaseConnection("Server=ServerB;Database=ProdDB;");
            db2.ExecuteQuery("UPDATE Users SET Active = 1");
            db2.Dispose();

            Console.WriteLine("\n3. Демонстрація роботи деструктора через GC.Collect()");
            CreateAndAbandonObject();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nРобота програми завершена.");
        }

        static void CreateAndAbandonObject()
        {
            var db3 = new DatabaseConnection("Server=ServerC;Database=TempDB;");
            db3.ExecuteQuery("DELETE FROM TempData");
        }
    }
}