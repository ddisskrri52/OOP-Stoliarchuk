using System;

namespace Lab4Variant2
{
    // 1. базовий клас: Shape
    public class Shape
    {
        private string _color;

        public string Color
        {
            get => _color;
            set => _color = value;
        }

        public Shape(string color)
        {
            _color = color;
        }

        public virtual double GetArea()
        {
            return 0.0;
        }

        public string GetShapeType()
        {
            return "Базова фігура (Generic Shape)";
        }
    }

    // 2. похідний клас: Circle
    public class Circle : Shape
    {
        private double _radius;

        public double Radius
        {
            get => _radius;
            set => _radius = value;
        }

        public Circle(string color, double radius) : base(color)
        {
            _radius = radius;
        }

        public override double GetArea()
        {
            return Math.PI * _radius * _radius;
        }

        public void Draw()
        {
            Console.WriteLine($"[Circle] Малюємо коло радіуса {_radius} кольору {Color}.");
        }
    }

    // 3. похідний клас: Rectangle
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

        public Rectangle(string color, double width, double height) : base(color)
        {
            _width = width;
            _height = height;
        }

        public override double GetArea()
        {
            return _width * _height;
        }

        public void Draw()
        {
            Console.WriteLine($"[Rectangle] Малюємо прямокутник {_width}x{_height} кольору {Color}.");
        }

        public new string GetShapeType()
        {
            return "Прямокутник (Rectangle Shape)";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("1. СТВОРЕННЯ ОБ'ЄКТІВ ТА ВИКЛИК УНІКАЛЬНИХ МЕТОДІВ ");
            Circle circle = new Circle("Червоний", 5.0);
            Rectangle rectangle = new Rectangle("Синій", 4.0, 6.0);

            circle.Draw();
            rectangle.Draw();
            Console.WriteLine();

            Console.WriteLine("2. ДЕМОНСТРАЦІЯ ПОЛІМОРФІЗМУ (override)");
            Shape[] shapes = new Shape[]
            {
                new Shape("Білий"),
                circle,
                rectangle
            };

            foreach (var shape in shapes)
            {
                Console.WriteLine($"Фігура кольору: {shape.Color,-10} | Площа: {shape.GetArea():F2}");
            }
            Console.WriteLine();

            Console.WriteLine(" 3. ДЕМОНСТРАЦІЯ РІЗНИЦІ МІЖ override ТА new ");
            Rectangle rectDirect = new Rectangle("Зелений", 2, 3);
            Console.WriteLine("Виклик GetShapeType() через посилання Rectangle:");
            Console.WriteLine($"  rectDirect.GetShapeType() -> {rectDirect.GetShapeType()}"); 

            Shape rectAsShape = rectDirect;
            Console.WriteLine("Виклик GetShapeType() через посилання Shape (для того ж об'єкта):");
            Console.WriteLine($"  rectAsShape.GetShapeType() -> {rectAsShape.GetShapeType()}"); 

            Console.WriteLine("\nДля порівняння (поведінка override):");
            Console.WriteLine($"  rectDirect.GetArea()  -> {rectDirect.GetArea()}");
            Console.WriteLine($"  rectAsShape.GetArea() -> {rectAsShape.GetArea()}"); 
        }
    }
}