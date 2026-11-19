using System;
using System.Threading;

namespace IndependentWork14;

// клас для реалізації патерну retry
public class RetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;
    private readonly double _jitterFactor;
    private readonly Random _random = new Random();

    public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
    {
        _maxRetries = maxRetries;
        _initialDelay = initialDelay;
        _jitterFactor = jitterFactor;
    }

    public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
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
                if (shouldRetry != null && !shouldRetry(ex))
                    throw;

                if (attempt >= _maxRetries)
                    throw;

                double delayMs = _initialDelay.TotalMilliseconds * Math.Pow(2, attempt) * 
                                 (1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor);
                
                Console.WriteLine($"спроба {attempt + 1} невдала: {ex.Message}. очікування {delayMs:F0} мс");
                Thread.Sleep((int)delayMs);
                attempt++;
            }
        }
    }
}

class Program
{
    static void Main()
    {
        var policy = new RetryPolicy(3, TimeSpan.FromMilliseconds(500));

        int failCount = 0;
        try
        {
            var result1 = policy.Execute(() =>
            {
                failCount++;
                if (failCount < 3) throw new InvalidOperationException("тимчасовий збій");
                return "успіх";
            });
            Console.WriteLine($"результат: {result1}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"помилка: {ex.Message}\n");
        }

        try
        {
            var result2 = policy.Execute<string>(() =>
            {
                throw new TimeoutException("сервер не відповідає");
            }, ex => ex is TimeoutException);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"остаточна помилка: {ex.Message}");
        }
    }
}