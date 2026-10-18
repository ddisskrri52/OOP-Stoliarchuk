using System;

namespace Lab6V2
{
    // Базовий клас Shape
    public class Shape
    {
        // Приватне поле та публічна властивість
        private string _color;

        public string Color
        {
            get => _color;
            set => _color = value;
        }

        // Конструктор базового класу
        public Shape(string color)
        {
            _color = color;
        }

        // Віртуальний метод
        public virtual double GetArea()
        {
            return 0.0;
        }

        // Звичайний (не віртуальний) метод для демонстрації 'new'
        public string GetShapeType()
        {
            return "Базова фігура (Shape)";
        }
    }

    // Похідний клас 1: Circle
    public class Circle : Shape
    {
        private double _radius;

        public double Radius
        {
            get => _radius;
            set => _radius = value;
        }

        // Конструктор, що викликає конструктор базового класу через base(...)
        public Circle(string color, double radius) : base(color)
        {
            _radius = radius;
        }

        // Перевизначення (override) віртуального методу базового класу
        public override double GetArea()
        {
            return Math.PI * _radius * _radius;
        }

        // Власний унікальний метод класу Circle
        public void Draw()
        {
            Console.WriteLine($" Малюємо коло кольору '{Color}' радіусом {_radius}.");
        }
    }

    // Похідний клас 2: Rectangle
    public class Rectangle : Shape
    {
        private double _width;
        private double _height;

        public double Width
        {
            get => _width;
            set => _width = value;
        }

        public double Height
        {
            get => _height;
            set => _height = value;
        }

        // Конструктор з викликом base(...)
        public Rectangle(string color, double width, double height) : base(color)
        {
            _width = width;
            _height = height;
        }

        // Перевизначення (override) віртуального методу базового класу
        public override double GetArea()
        {
            return _width * _height;
        }

        // Власний унікальний метод класу Rectangle
        public void Draw()
        {
            Console.WriteLine($" Малюємо прямокутник кольору '{Color}' розміром {_width}x{_height}.");
        }

        // Демонстрація приховування члена за допомогою new
        public new string GetShapeType()
        {
            return "Прямокутник (Rectangle)";
        }
    }

    // Головний клас програми
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("1. Створення об'єктів та виклик методів");
            Shape genericShape = new Shape("Сірий");
            Circle circle = new Circle("Червоний", 5.0);
            Rectangle rectangle = new Rectangle("Синій", 4.0, 6.0);

            Console.WriteLine($"Фігура: колір = {genericShape.Color}, площа = {genericShape.GetArea()}");
            circle.Draw();
            Console.WriteLine($"Площа кола: {circle.GetArea():F2}");
            rectangle.Draw();
            Console.WriteLine($"Площа прямокутника: {rectangle.GetArea():F2}");

            Console.WriteLine("\n 2. Демонстрація поліморфізму (override)");
            // Масив посилань базового типу Shape, що містить об'єкти похідних класів
            Shape[] shapes = new Shape[] { genericShape, circle, rectangle };

            foreach (var shape in shapes)
            {
                // Для кожного об'єкта викликається його власна реалізація GetArea()
                Console.WriteLine($"Фігура кольору '{shape.Color}' має площу: {shape.GetArea():F2}");
            }

            Console.WriteLine("\n3. Демонстрація різниці між override та new");

            // Створюємо екземпляр Rectangle
            Rectangle rectObj = new Rectangle("Зелений", 3.0, 7.0);

            // Посилання типу Rectangle
            Rectangle rectAsRectangle = rectObj;
            // Посилання типу Shape
            Shape rectAsShape = rectObj;

            Console.WriteLine("Викликаємо метод GetArea() (override)");
            Console.WriteLine($"Виклик через посилання типу Rectangle: {rectAsRectangle.GetArea()}");
            Console.WriteLine($"Виклик через посилання типу Shape:     {rectAsShape.GetArea()}");
            Console.WriteLine("Пояснення: Спрацьовує поліморфізм, в обох випадках виконується реалізація з Rectangle.");

            Console.WriteLine("\nВикликаємо метод GetShapeType() (new)");
            Console.WriteLine($"Виклик через посилання типу Rectangle: {rectAsRectangle.GetShapeType()}");
            Console.WriteLine($"Виклик через посилання типу Shape:     {rectAsShape.GetShapeType()}");
            Console.WriteLine("Пояснення: Модифікатор 'new' НЕ забезпечує поліморфізму. Викликаний метод залежить від типу ПОСИЛАННЯ, а не від типу об'єкта.");

            Console.ReadLine();
        }
    }
}