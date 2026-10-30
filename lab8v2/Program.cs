using System;
using System.Collections.Generic;

namespace lab14v2
{
    // клас утиліт для роботи з колекціями
    public static class CollectionHelper<T>
    {
        // метод мap для перетворення колекції одного типу в колекцію іншого типу
        public static IEnumerable<TResult> Map<TResult>(IEnumerable<T> collection, Func<T, TResult> mapper)
        {
            // власна реалізація методу Map без використання LINQ
            foreach (var item in collection)
            {
                // застосовуємо делегат mapper до кожного елемента
                yield return mapper(item);
            }
        }
    }

    // допоміжок для демонстрації роботи CollectionHelper<T>.Map
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демонстрація роботи CollectionHelper<T>.Map");

            // перетворення List<int> на List<string>
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            
            // використовуємо лямбда-вираз для перетворення числа у рядок
            var stringNumbers = CollectionHelper<int>.Map(numbers, n => $"Число: {n}");
            
            Console.WriteLine("\nРезультат 1 (List<int> -> List<string>):");
            foreach (var str in stringNumbers)
            {
                Console.WriteLine(str);
            }

            // перетворення List<Product> на List<decimal> (ціни)
            List<Product> products = new List<Product>
            {
                new Product { Name = "Ноутбук", Price = 45000.50m },
                new Product { Name = "Мишка", Price = 1200.00m },
                new Product { Name = "Клавіатура", Price = 3500.75m }
            };

            // використовуємо лямбда-вираз для витягування ціни з об'єкта Product
            var prices = CollectionHelper<Product>.Map(products, p => p.Price);

            Console.WriteLine("\nРезультат 2 (List<Product> -> List<decimal>):");
            foreach (var price in prices)
            {
                Console.WriteLine($"{price} грн");
            }
        }
    }
}