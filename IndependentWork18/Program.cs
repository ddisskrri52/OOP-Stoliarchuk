using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork18;

class Program
{
    static void Main()
    {
        Console.WriteLine("дослідження продуктивності linq та plinq\n");

        int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };

        foreach (var size in sizes)
        {
            Console.WriteLine($"розмір колекції: {size} елементів");
            var data = GenerateData(size);

            RunPerformanceTest(data);
            Console.WriteLine();
        }

        Console.WriteLine("дослідження потокобезпеки (побічні ефекти plinq)\n");
        DemonstrateSideEffects();
    }

    // генерація випадкових чисел
    private static List<double> GenerateData(int size)
    {
        var random = new Random(42);
        var list = new List<double>(size);
        for (int i = 0; i < size; i++)
        {
            list.Add(random.NextDouble() * 1000);
        }
        return list;
    }

    // обчислювально інтенсивна операція
    private static double HeavyCalculation(double x)
    {
        double result = 0;
        for (int i = 0; i < 10; i++)
        {
            result += Math.Sqrt(x) * Math.Sin(x);
        }
        return result;
    }

    // вимірювання продуктивності
    private static void RunPerformanceTest(List<double> data)
    {
        var sw = new Stopwatch();

        // звичайний linq
        sw.Start();
        var linqResult = data.Select(HeavyCalculation).ToList();
        sw.Stop();
        Console.WriteLine($"linq час: {sw.ElapsedMilliseconds} мс");

        sw.Reset();

        // паралельний plinq
        sw.Start();
        var plinqResult = data.AsParallel().Select(HeavyCalculation).ToList();
        sw.Stop();
        Console.WriteLine($"plinq час: {sw.ElapsedMilliseconds} мс");
    }

    // демонстрація побічних ефектів
    private static void DemonstrateSideEffects()
    {
        var data = Enumerable.Range(1, 100_000).ToList();
        
        // проблема: запис без синхронізації
        int unsafeCounter = 0;
        data.AsParallel().ForAll(x => 
        {
            unsafeCounter++; // гонка даних
        });
        Console.WriteLine($"небезпечний лічильник (очікувано 100000): {unsafeCounter}");

        // рішення 1: використання lock
        int safeCounterLock = 0;
        object lockObj = new object();
        data.AsParallel().ForAll(x => 
        {
            lock (lockObj)
            {
                safeCounterLock++;
            }
        });
        Console.WriteLine($"безпечний лічильник (lock): {safeCounterLock}");

        // рішення 2: використання interlocked
        int safeCounterInterlocked = 0;
        data.AsParallel().ForAll(x => 
        {
            Interlocked.Increment(ref safeCounterInterlocked);
        });
        Console.WriteLine($"безпечний лічильник (interlocked): {safeCounterInterlocked}");
    }
}