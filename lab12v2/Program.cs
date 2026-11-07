using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab12v2
{
    // клас, що імітує роботу з файлами
    public class FileProcessor
    {
        private int _writeAttempts = 0;

        // імітація IOException перші 3 рази
        public void WriteToFile(string path, string content)
        {
            _writeAttempts++;
            if (_writeAttempts <= 3)
            {
                throw new IOException($"Помилка доступу до файлу '{path}'.");
            }
            Console.WriteLine($"[FileProcessor] Спроба {_writeAttempts}: Дані успішно записано у файл '{path}'.");
        }
    }

    // імітація мережевих запитів
    public class NetworkClient
    {
        private int _uploadAttempts = 0;

        // імітація HttpRequestException перші 2 рази
        public bool UploadFile(string url, string filePath)
        {
            _uploadAttempts++;
            if (_uploadAttempts <= 2)
            {
                throw new HttpRequestException($"Помилка з'єднання із сервером '{url}'.");
            }
            Console.WriteLine($"[NetworkClient] Спроба {_uploadAttempts}: Файл '{filePath}' успішно завантажено.");
            return true;
        }
    }

    // узагальнений допоміжний клас RetryHelper
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool>? shouldRetry = null) //
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    return operation();
                }
                catch (Exception ex)
                {
                    attempt++;
                    // перевірка чи варто повторювати спробу
                    if (attempt > retryCount || (shouldRetry != null && !shouldRetry(ex)))
                    {
                        Console.WriteLine($"[RetryHelper] Операцію перервано. Ліміт спроб або критична помилка: {ex.Message}");
                        throw;
                    }

                    // експоненційна затримка: initialDelay * 2^(attempt-1)
                    double delayMs = initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1);
                    Console.WriteLine($"[RetryHelper] Невдача (Спроба {attempt}/{retryCount}): {ex.Message}. Очікування {delayMs} мс...");
                    Thread.Sleep(TimeSpan.FromMilliseconds(delayMs));
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            // Повторювати для IOException та HttpRequestException
            Func<Exception, bool> shouldRetry = ex => ex is IOException || ex is HttpRequestException;

            Console.WriteLine("Сценарій: Запис у файл");
            try
            {
                // Обертаємо void-метод у Func<bool>, щоб він відповідав сигнатурі ExecuteWithRetry<T>
                RetryHelper.ExecuteWithRetry<bool>(() =>
                {
                    fileProcessor.WriteToFile("data.txt", "Важлива інформація");
                    return true;
                }, retryCount: 4, initialDelay: TimeSpan.FromMilliseconds(500), shouldRetry: shouldRetry);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Фінальна помилка: {e.Message}");
            }

            Console.WriteLine("\nСценарій: Завантаження файлу на сервер");
            try
            {
                RetryHelper.ExecuteWithRetry<bool>(() =>
                {
                    return networkClient.UploadFile("https://api.example.com/upload", "data.txt");
                }, retryCount: 3, initialDelay: TimeSpan.FromMilliseconds(500), shouldRetry: shouldRetry);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Фінальна помилка: {e.Message}");
            }
        }
    }
}