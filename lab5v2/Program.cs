using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab5Variant2
{
    // базовий клас
    public class Vehicle
    {
        public double Speed { get; set; }

        public Vehicle(double speed)
        {
            Speed = speed;
        }

        // віртуальний метод для поліморфного виклику
        public virtual void Move()
        {
            Console.WriteLine($"Транспортний засіб рухається зі швидкістю {Speed} км/год.");
        }
    }

    // похідний клас 1: Автомобіль
    public class Car : Vehicle
    {
        public int NumWheels { get; set; }

        public Car(double speed, int numWheels) : base(speed)
        {
            NumWheels = numWheels;
        }

        public override void Move()
        {
            Console.WriteLine($"Автомобіль із {NumWheels} колесами їде зі швидкістю {Speed} км/год.");
        }
    }

    //похідний клас 2: Велосипед
    public class Bicycle : Vehicle
    {
        public bool HasGears { get; set; }

        public Bicycle(double speed, bool hasGears) : base(speed)
        {
            HasGears = hasGears;
        }

        public override void Move()
        {
            string gearsInfo = HasGears ? "з передачами" : "без передач";
            Console.WriteLine($"Велосипед ({gearsInfo}) їде зі швидкістю {Speed} км/год.");
        }
    }

    // похідний клас 3: Човен
    public class Boat : Vehicle
    {
        public string EngineType { get; set; }

        public Boat(double speed, string engineType) : base(speed)
        {
            EngineType = engineType;
        }

        public override void Move()
        {
            Console.WriteLine($"Човен із двигуном '{EngineType}' пливе зі швидкістю {Speed} км/год.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // створення колекції об'єктів(List<Vehicle>)
            List<Vehicle> vehicles = new List<Vehicle>
            {
                new Car(120.0, 4),
                new Bicycle(25.5, true),
                new Boat(45.0, "Бензиновий"),
                new Car(90.0, 4),
                new Bicycle(15.0, false)
            };

            Console.WriteLine("Рух транспортних засобів (Поліморфні виклики)");
            
            // поліморфізм
            foreach (var vehicle in vehicles)
            {
                vehicle.Move();
            }

            // агрегація
            double averageSpeed = vehicles.Average(v => v.Speed);

            Console.WriteLine("\nАгрегація результатів");
            Console.WriteLine($"Загальна кількість транспортних засобів: {vehicles.Count}");
            Console.WriteLine($"Середня швидкість усіх транспортних засобів: {averageSpeed:F2} км/год");
        }
    }
}