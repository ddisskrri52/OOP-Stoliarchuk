using System;

namespace lab1v2
{
    class Car
    {
        private string brand;
        private string model;

       public int Year { get; set; }

       public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public Car(string brand, string model, int year)
        {
            this.brand = brand;
            this.model = model;
            Year = year;
        }

        public void Drive()
        {
            Console.WriteLine($"Автомобіль {Brand} {Model} ({Year} року) вирушив у дорогу!");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Car car1 = new Car("Toyota", "Camry", 2020);
            Car car2 = new Car("BMW", "M5", 2022);
            Car car3 = new Car("Tesla", "Model 3", 2023);

            car1.Drive();
            car2.Drive();
            car3.Drive();
        }
    }
}