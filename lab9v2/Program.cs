using System;
using System.Collections.Generic;

namespace OOP
{
    // Клас для представлення логу
    class LogEntry
    {
        public string IpAddress { get; set; }
        public string Message { get; set; }

        public LogEntry(string ip, string message)
        {
            IpAddress = ip;
            Message = message;
        }

        public override string ToString()
        {
            return $"[{IpAddress}]: {Message}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("1. HashSet");
            HashSet<string> uniqueIps = new HashSet<string>();

            // Додаємо IP, у тому числі дублікати
            uniqueIps.Add("192.168.1.10");
            uniqueIps.Add("10.0.0.1");
            uniqueIps.Add("192.168.1.10"); // Дублікат, не додасться

            Console.WriteLine("Унікальні IP в системі:");
            foreach (var ip in uniqueIps)
            {
                Console.WriteLine($"- {ip}");
            }
            Console.WriteLine($"Чи є IP 10.0.0.1 у базі? {uniqueIps.Contains("10.0.0.1")}");


            Console.WriteLine("\n2. Queue");
            Queue<LogEntry> logQueue = new Queue<LogEntry>();

            // Додаємо логи в чергу
            logQueue.Enqueue(new LogEntry("192.168.1.10", "User logged in"));
            logQueue.Enqueue(new LogEntry("10.0.0.1", "Database connection lost"));
            logQueue.Enqueue(new LogEntry("192.168.1.10", "User clicked settings"));

            Console.WriteLine($"Наступний лог на обробку: {logQueue.Peek()}");

            // Обробляємо чергу
            while (logQueue.Count > 0)
            {
                var currentLog = logQueue.Dequeue();
                Console.WriteLine($"Обробка логу -> {currentLog}");
            }


            Console.WriteLine("\n3. Stack");
            Stack<LogEntry> viewHistory = new Stack<LogEntry>();

            // Симуляція перегляду логів адміністратором
            viewHistory.Push(new LogEntry("192.168.1.10", "Log #1"));
            viewHistory.Push(new LogEntry("10.0.0.1", "Log #2"));
            viewHistory.Push(new LogEntry("172.16.0.5", "Log #3"));

            Console.WriteLine($"Останній переглянутий лог: {viewHistory.Peek()}");

            // Повернення назад (Undo)
            while (viewHistory.Count > 0)
            {
                var lastViewed = viewHistory.Pop();
                Console.WriteLine($"Закрили перегляд -> {lastViewed}");
            }
        }
    }
}