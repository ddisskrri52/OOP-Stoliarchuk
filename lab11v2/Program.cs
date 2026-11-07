using System;
using System.Collections.Generic;
using System.Linq;

namespace lab17v2
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; } = null!;    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public static class OrderDetailExtensions
    {
        public static decimal TotalOrderAmount(this IEnumerable<OrderDetail> details)
        {
            if (details == null) return 0;
            return details.Sum(d => d.Product.Price * d.Quantity);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Ноутбук", Price = 25000m },
                new Product { Id = 2, Name = "Мишка", Price = 800m },
                new Product { Id = 3, Name = "Клавіатура", Price = 1500m },
                new Product { Id = 4, Name = "Монітор", Price = 7000m },
                new Product { Id = 5, Name = "Килимок", Price = 300m }
            };

            var orders = new List<Order>
            {
                new Order { Id = 1, OrderDate = DateTime.Now.AddDays(-5), OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 101, ProductId = 1, Quantity = 1, Product = products[0] },
                        new OrderDetail { Id = 102, ProductId = 2, Quantity = 2, Product = products[1] }
                    }
                },
                new Order { Id = 2, OrderDate = DateTime.Now.AddDays(-4), OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 103, ProductId = 4, Quantity = 2, Product = products[3] }
                    }
                },
                new Order { Id = 3, OrderDate = DateTime.Now.AddDays(-3), OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 104, ProductId = 1, Quantity = 1, Product = products[0] },
                        new OrderDetail { Id = 105, ProductId = 3, Quantity = 1, Product = products[2] },
                        new OrderDetail { Id = 106, ProductId = 5, Quantity = 3, Product = products[4] }
                    }
                },
                new Order { Id = 4, OrderDate = DateTime.Now.AddDays(-2), OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 107, ProductId = 2, Quantity = 1, Product = products[1] }
                    }
                },
                new Order { Id = 5, OrderDate = DateTime.Now.AddDays(-1), OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Id = 108, ProductId = 3, Quantity = 2, Product = products[2] },
                        new OrderDetail { Id = 109, ProductId = 4, Quantity = 1, Product = products[3] }
                    }
                }
            };

            // реалізація LINQ-запитів згідно з варіантом
            
            // selectMany
            Console.WriteLine("1. SelectMany: Всі замовлені продукти (без дублікатів)");
            var orderedProducts = orders
                .SelectMany(o => o.OrderDetails) // розгортання замовлення до деталей
                .Select(od => od.Product)        // отримання продуктів з деталей
                .Distinct()                      // видалення дублікатів
                .ToList();

            foreach (var p in orderedProducts)
            {
                Console.WriteLine($"- {p.Name} (Ціна: {p.Price} грн)");
            }
            // пагінація
            Console.WriteLine("\n2. Пагінація: 2 замовлення на сторінку");
            int pageSize = 2;
            int totalPages = (int)Math.Ceiling(orders.Count / (double)pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                Console.WriteLine($"\nСторінка {page} з {totalPages}");
                var pageOrders = orders
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var order in pageOrders)
                {
                    Console.WriteLine($"Замовлення №{order.Id} від {order.OrderDate.ToShortDateString()}");
                }
            }

            // сustom Extension
            Console.WriteLine("\n3. Custom Extension: Загальна сума кожного замовлення");
            foreach (var order in orders)
            {
                // виклик власного методу розширення для колекції OrderDetails
                decimal total = order.OrderDetails.TotalOrderAmount();
                Console.WriteLine($"Замовлення №{order.Id} - Загальна сума: {total} грн");
            }
        }
    }
}