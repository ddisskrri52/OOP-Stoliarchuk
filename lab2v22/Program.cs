using System;

namespace CarApp
{
  
    public class Car
    {
    
        private string _brand;
        private string _model;
        private int _year;

        public string Brand
        {
            get => _brand;
            set => _brand = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public string Model
        {
            get => _model;
            set => _model = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public int Year
        {
            get => _year;
            set
            {
                int currentYear = DateTime.Now.Year;
                if (value > currentYear)
                {
                    Console.WriteLine($"[Помилка]: Рік {value} не може бути в майбутньому! Встановлено за замовчуванням ({currentYear}).");
                    _year = currentYear;
                }
                else if (value < 1886) 
                {
                    Console.WriteLine("[Помилка]: Некоректний рік випуску автомобіля. Встановлено 2000 рік.");
                    _year = 2000;
                }
                else
                {
                    _year = value;
                }
            }
        }

        public Car() : this("Unknown", "Unknown", 2000)
        {
        }

        public Car(string brand, string model, int year)
        {
            Brand = brand;
            Model = model;
            Year = year; 
        }

        public void StartEngine()
        {
            Console.WriteLine($"Двигун автомобіля {Brand} {Model} ({Year} року) успішно запущено! Vroom-vroom!");
        }

        ~Car()
        {
            Console.WriteLine($"[Деструктор]: Об'єкт Car ({_brand} {_model}) знищено з пам'яті.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine(" Створення об'єкта через конструктор за замовчуванням ");
            Car defaultCar = new Car();
            defaultCar.StartEngine();

            Console.WriteLine("\n Створення об'єкта з коректними даними ");
            Car tesla = new Car("Tesla", "Model S", 2022);
            tesla.StartEngine();

            Console.WriteLine("\n Створення об'єкта з некоректним роком (валідація) ");
            Car futureCar = new Car("CyberCar", "X", 2035);
            futureCar.StartEngine();

            Console.WriteLine("\n Завершення роботи Main ");
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}