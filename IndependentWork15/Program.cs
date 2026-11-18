using System;
using System.Collections.Generic;

namespace IndependentWork15
{
    // клас для демонстрації
    public class Product
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }

    public static class CollectionUtils
    {
        // метод 1: Map
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        // метод 2: MaxBy
        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            T maxItem = default!;
            TKey maxKey = default!;
            bool first = true;
            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }
            return maxItem;
        }

        // метод 3: Filter (реалізовано самостійно)
        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        // метод 4: ForEach (реалізовано самостійно)
        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демонстрація CollectionUtils\n");

            // колекція 1: List<Product>
            var products = new List<Product>
            {
                new Product { Name = "ноутбук", Price = 25000 },
                new Product { Name = "мишка", Price = 500 },
                new Product { Name = "клавіатура", Price = 1500 }
            };

            Console.WriteLine("1. Робота з List<Product>:");
            var productNames = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Назви (Map): " + string.Join(", ", productNames));

            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий (MaxBy): {mostExpensive.Name} {mostExpensive.Price} грн");

            var expensiveProducts = CollectionUtils.Filter(products, p => p.Price > 1000);
            Console.WriteLine("Дорожчі за 1000 (Filter & ForEach):");
            CollectionUtils.ForEach(expensiveProducts, p => Console.WriteLine($" {p.Name}"));
            Console.WriteLine();


            // колекція 2: List<int>
            var numbers = new List<int> { 10, 45, 2, 89, 23 };
            
            Console.WriteLine("2. Робота з List<int>:");
            var squared = CollectionUtils.Map(numbers, n => n * n);
            Console.WriteLine("Квадрати чисел (Map): " + string.Join(", ", squared));

            var maxNumber = CollectionUtils.MaxBy(numbers, n => n);
            Console.WriteLine($"Максимальне число (MaxBy): {maxNumber}");

            var evens = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.Write("Парні числа (Filter & ForEach): ");
            CollectionUtils.ForEach(evens, n => Console.Write($"{n} "));
            Console.WriteLine("\n");


            // колекція 3: List<string>
            var words = new List<string> { "яблуко", "банан", "ківі", "полуниця" };
            
            Console.WriteLine("3. Робота з List<string>:");
            var wordLengths = CollectionUtils.Map(words, w => $"{w} ({w.Length})");
            Console.WriteLine("Довжини слів (Map): " + string.Join(", ", wordLengths));

            var longestWord = CollectionUtils.MaxBy(words, w => w.Length);
            Console.WriteLine($"Найдовше слово (MaxBy): {longestWord}");

            var shortWords = CollectionUtils.Filter(words, w => w.Length < 6);
            Console.Write("Короткі слова (Filter & ForEach): ");
            CollectionUtils.ForEach(shortWords, w => Console.Write($"{w} "));
            Console.WriteLine();
        }
    }
}