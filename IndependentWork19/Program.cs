using System;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork19;

// власний тип винятку
public class OutOfStockException : Exception
{
    public int RequestedQuantity { get; }
    public int AvailableQuantity { get; }

    public OutOfStockException() { }

    public OutOfStockException(string message) : base(message) { }

    public OutOfStockException(string message, Exception inner) : base(message, inner) { }

    public OutOfStockException(string message, int requested, int available) 
        : base(message)
    {
        RequestedQuantity = requested;
        AvailableQuantity = available;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        DemonstrateCustomException();
        await DemonstrateRetryAndTimeoutAsync();
        await DemonstrateCircuitBreakerAsync();
    }

    // демонстрація власного винятку
    static void DemonstrateCustomException()
    {
        try
        {
            throw new OutOfStockException("недостатньо товару на складі.", 10, 2);
        }
        catch (OutOfStockException ex)
        {
            Console.WriteLine($"помилка: {ex.Message} запитано: {ex.RequestedQuantity}, доступно: {ex.AvailableQuantity}");
        }
    }

    // сценарій 1: retry + timeout
    static async Task DemonstrateRetryAndTimeoutAsync()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<TimeoutRejectedException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    Console.WriteLine($"спроба {args.AttemptNumber + 1} невдала. очікування...");
                    return default;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(2))
            .Build();

        int attempt = 0;

        try
        {
            await pipeline.ExecuteAsync(async token =>
            {
                attempt++;
                if (attempt <= 2)
                {
                    await Task.Delay(3000, token);
                }
                Console.WriteLine("api успішно відповіло");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"остаточна невдача: {ex.Message}");
        }
    }

    // сценарій 2: circuit breaker
    static async Task DemonstrateCircuitBreakerAsync()
    {
        var pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 2,
                BreakDuration = TimeSpan.FromSeconds(3),
                OnOpened = args =>
                {
                    Console.WriteLine("ланцюг розімкнено");
                    return default;
                },
                OnClosed = args =>
                {
                    Console.WriteLine("ланцюг замкнено");
                    return default;
                },
                OnHalfOpened = args =>
                {
                    Console.WriteLine("ланцюг напіврозімкнено");
                    return default;
                }
            })
            .Build();

        for (int i = 1; i <= 6; i++)
        {
            try
            {
                pipeline.Execute(() =>
                {
                    if (i <= 3) throw new InvalidOperationException("помилка бд");
                    Console.WriteLine($"запит {i} успішний");
                });
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine($"запит {i} відхилено: ланцюг розімкнено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"запит {i} впав: {ex.Message}");
            }

            await Task.Delay(1000);
        }
    }
}