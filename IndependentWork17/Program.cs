using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace IndependentWork17;

// моделі даних

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public class Sale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
}

// головний клас програми

class Program
{
    private static List<Product> _products = new();
    private static List<Customer> _customers = new();
    private static List<Sale> _sales = new();

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // генерація тестових даних
        GenerateTestData(seed: 42, productCount: 60, customerCount: 80, saleCount: 1500);

        Console.WriteLine($"Згенеровано даних: {_products.Count} товарів, {_customers.Count} клієнтів, {_sales.Count} продажів.\n");

        // виконання пайплайнів
        TopProductsByCategory();

        TotalRevenueByCity();

        SalesByMonthAndYear();

        VipCustomersAnalytics();
    }

    // генерація псевдовипадкових наборів даних
    private static void GenerateTestData(int seed, int productCount, int customerCount, int saleCount)
    {
        var random = new Random(seed);

        string[] categories = { "Ноутбуки", "Смартфони", "Аудіо", "Периферія", "Монітори" };
        string[] adjectives = { "Pro", "Max", "Lite", "Ultra", "Gaming", "Office", "Smart", "Air" };
        string[] cities = { "Рівне", "Київ", "Львів", "Одеса", "Дніпро", "Харків", "Вінниця" };
        string[] firstNames = { "Олександр", "Ірина", "Максим", "Олена", "Дмитро", "Наталія", "Андрій", "Тетяна" };
        string[] lastNames = { "Ковальчук", "Бондаренко", "Ткаченко", "Шевченко", "Мельник", "Кравчук", "Олійник" };

        // генерація продуктів
        for (int i = 1; i <= productCount; i++)
        {
            string category = categories[random.Next(categories.Length)];
            string adj = adjectives[random.Next(adjectives.Length)];
            _products.Add(new Product
            {
                Id = i,
                Name = $"{category.TrimEnd('и', 'і')} {adj} #{i}",
                Category = category,
                UnitPrice = Math.Round((decimal)(random.NextDouble() * 35000 + 500), 2)
            });
        }

        // генерація клієнтів
        for (int i = 1; i <= customerCount; i++)
        {
            string fullName = $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
            _customers.Add(new Customer
            {
                Id = i,
                Name = fullName,
                City = cities[random.Next(cities.Length)]
            });
        }

        // генерація продажів
        DateTime startDate = new DateTime(2025, 1, 1);
        int rangeDays = (new DateTime(2026, 11, 19) - startDate).Days;

        for (int i = 1; i <= saleCount; i++)
        {
            _sales.Add(new Sale
            {
                Id = i,
                ProductId = random.Next(1, productCount + 1),
                CustomerId = random.Next(1, customerCount + 1),
                Quantity = random.Next(1, 11),
                SaleDate = startDate.AddDays(random.Next(rangeDays))
            });
        }
    }

    // пайплайн 1: найпопулярніші продукти за категоріями
    private static void TopProductsByCategory()
    {
        var sw = Stopwatch.StartNew();

        var result = _sales
            .GroupBy(s => s.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantity = g.Sum(s => s.Quantity)
            })
            .Join(_products,
                  saleStat => saleStat.ProductId,
                  product => product.Id,
                  (saleStat, product) => new
                  {
                      product.Name,
                      product.Category,
                      saleStat.TotalQuantity
                  })
            .GroupBy(p => p.Category)
            .Select(catGroup => new
            {
                Category = catGroup.Key,
                TopProducts = catGroup
                    .OrderByDescending(p => p.TotalQuantity)
                    .Take(3)
                    .ToList()
            })
            .OrderBy(c => c.Category)
            .ToList();

        sw.Stop();

        Console.WriteLine($"[Пайплайн 1] Найпопулярніші продукти за категоріями (Час: {sw.Elapsed.TotalMilliseconds:F4} мс)");
        foreach (var group in result)
        {
            Console.WriteLine($"  Категорія: {group.Category}");
            int rank = 1;
            foreach (var prod in group.TopProducts)
            {
                Console.WriteLine($"    {rank++}. {prod.Name} — продано {prod.TotalQuantity} од.");
            }
        }
    }

    // пайплайн 2: загальний дохід за містами клієнтів
    private static void TotalRevenueByCity()
    {
        var sw = Stopwatch.StartNew();

        var result = _sales
            .Join(_products,
                  s => s.ProductId,
                  p => p.Id,
                  (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
            .Join(_customers,
                  sp => sp.CustomerId,
                  c => c.Id,
                  (sp, c) => new { c.City, sp.Revenue })
            .GroupBy(x => x.City)
            .Select(g => new
            {
                City = g.Key,
                TotalRevenue = g.Sum(x => x.Revenue),
                OrdersCount = g.Count()
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToList();

        sw.Stop();

        Console.WriteLine($"[Пайплайн 2] Загальний дохід за містами клієнтів (Час: {sw.Elapsed.TotalMilliseconds:F4} мс)");
        foreach (var item in result)
        {
            Console.WriteLine($"  {item.City,-10} | Замовлень: {item.OrdersCount,4} | Дохід: {item.TotalRevenue,14:N2} грн");
        }
    }

    // пайплайн 3: продажі за місяцями та роками
    private static void SalesByMonthAndYear()
    {
        var sw = Stopwatch.StartNew();

        var result = _sales
            .Join(_products,
                  s => s.ProductId,
                  p => p.Id,
                  (s, p) => new
                  {
                      s.SaleDate.Year,
                      s.SaleDate.Month,
                      s.Quantity,
                      Revenue = s.Quantity * p.UnitPrice
                  })
            .GroupBy(x => new { x.Year, x.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                TotalUnits = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Revenue)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();

        sw.Stop();

        Console.WriteLine($"[Пайплайн 3] Продажі за місяцями та роками (Час: {sw.Elapsed.TotalMilliseconds:F4} мс)");
        foreach (var item in result)
        {
            Console.WriteLine($"  {item.Year}-{item.Month:D2} | Продано од.: {item.TotalUnits,5} | Дохід: {item.TotalRevenue,14:N2} грн");
        }
    }

    // пайплайн 4: топ-5 vip-клієнтів
    private static void VipCustomersAnalytics()
    {
        var sw = Stopwatch.StartNew();

        var result = _sales
            .Where(s => s.Quantity >= 2)
            .Join(_products,
                  s => s.ProductId,
                  p => p.Id,
                  (s, p) => new
                  {
                      s.CustomerId,
                      Spent = s.Quantity * p.UnitPrice,
                      Category = p.Category
                  })
            .GroupBy(x => x.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                TotalSpent = g.Sum(x => x.Spent),
                AvgCheck = g.Average(x => x.Spent),
                FavoriteCategory = g.GroupBy(x => x.Category)
                                    .OrderByDescending(cg => cg.Count())
                                    .Select(cg => cg.Key)
                                    .First()
            })
            .Join(_customers,
                  stat => stat.CustomerId,
                  c => c.Id,
                  (stat, c) => new
                  {
                      c.Name,
                      c.City,
                      stat.TotalSpent,
                      stat.AvgCheck,
                      stat.FavoriteCategory
                  })
            .OrderByDescending(x => x.TotalSpent)
            .Take(5)
            .ToList();

        sw.Stop();

        Console.WriteLine($"[Пайплайн 4] Топ-5 VIP-клієнтів (замовлення від 2 од.) (Час: {sw.Elapsed.TotalMilliseconds:F4} мс)");
        foreach (var vip in result)
        {
            Console.WriteLine($"  {vip.Name,-20} ({vip.City,-8}) | Витрачено: {vip.TotalSpent,12:N2} грн | Сер. чек: {vip.AvgCheck,10:N2} грн | Улюблена категорія: {vip.FavoriteCategory}");
        }
    }
}