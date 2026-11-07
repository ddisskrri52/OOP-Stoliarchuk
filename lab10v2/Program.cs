using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab16v2
{
    public class Course
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int Credits { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public int CourseId { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Тестові дані
            var courses = new List<Course>
            {
                new Course { Id = 1, Title = "Programming Basics", Credits = 4 },
                new Course { Id = 2, Title = "Advanced C#", Credits = 5 },
                new Course { Id = 3, Title = "Database Design", Credits = 3 }
            };

            var students = new List<Student>
            {
                new Student { Id = 1, Name = "Софія", Age = 18, CourseId = 1 },
                new Student { Id = 2, Name = "Марія", Age = 21, CourseId = 2 },
                new Student { Id = 3, Name = "Іван", Age = 18, CourseId = 1 },
                new Student { Id = 4, Name = "Олена", Age = 22, CourseId = 3 },
                new Student { Id = 5, Name = "Петро", Age = 19, CourseId = 1 },
                new Student { Id = 6, Name = "Анна", Age = 20, CourseId = 2 },
                new Student { Id = 7, Name = "Дмитро", Age = 19, CourseId = 3 },
                new Student { Id = 8, Name = "Катерина", Age = 23, CourseId = 2 },
                new Student { Id = 9, Name = "Максим", Age = 18, CourseId = 1 },
                new Student { Id = 10, Name = "Вікторія", Age = 19, CourseId = 1 },
                new Student { Id = 11, Name = "Сергій", Age = 21, CourseId = 3 },
                new Student { Id = 12, Name = "Юлія", Age = 18, CourseId = 2 }
            };

            // Прогрів JIT-компілятора для чистоти експерименту
            students.Where(s => s.Age > 0).ToList();

            Console.WriteLine("Запит 1: Студенти до 20 років на 'Programming Basics'");
            
            // Синтаксис методів
            var sw = Stopwatch.StartNew();
            var q1Method = students
                .Join(courses, s => s.CourseId, c => c.Id, (s, c) => new { Student = s, Course = c })
                .Where(x => x.Student.Age < 20 && x.Course.Title == "Programming Basics")
                .Select(x => x.Student)
                .ToList();
            sw.Stop();
            Console.WriteLine($"Синтаксис методів ({sw.ElapsedTicks} ticks): " + string.Join(", ", q1Method.Select(s => s.Name)));

            // Синтаксис запитів
            sw.Restart();
            var q1Query = (from s in students
                           join c in courses on s.CourseId equals c.Id
                           where s.Age < 20 && c.Title == "Programming Basics"
                           select s).ToList();
            sw.Stop();
            Console.WriteLine($"Синтаксис запитів ({sw.ElapsedTicks} ticks): " + string.Join(", ", q1Query.Select(s => s.Name)));


            Console.WriteLine("\nЗапит 2: Об'єднати студентів з курсами (Ім'я, Назва курсу)");
            
            // Синтаксис методів
            sw.Restart();
            var q2Method = students
                .Join(courses, s => s.CourseId, c => c.Id, (s, c) => new { s.Name, c.Title })
                .ToList();
            sw.Stop();
            Console.WriteLine($"Синтаксис методів ({sw.ElapsedTicks} ticks). Знайдено: {q2Method.Count} пар.");

            // Синтаксис запитів
            sw.Restart();
            var q2Query = (from s in students
                           join c in courses on s.CourseId equals c.Id
                           select new { s.Name, c.Title }).ToList();
            sw.Stop();
            Console.WriteLine($"Синтаксис запитів ({sw.ElapsedTicks} ticks). Знайдено: {q2Query.Count} пар.");


            Console.WriteLine("\nЗапит 3: Кількість студентів на кожному курсі");
            
            // Синтаксис методів
            sw.Restart();
            var q3Method = courses
                .GroupJoin(students, c => c.Id, s => s.CourseId, (c, st) => new { c.Title, Count = st.Count() })
                .ToList();
            sw.Stop();
            Console.WriteLine($"Синтаксис методів ({sw.ElapsedTicks} ticks):");
            q3Method.ForEach(r => Console.WriteLine($" - {r.Title}: {r.Count}"));

            // Синтаксис запитів
            sw.Restart();
            var q3Query = (from c in courses
                           join s in students on c.Id equals s.CourseId into courseGroup
                           select new { c.Title, Count = courseGroup.Count() }).ToList();
            sw.Stop();
            Console.WriteLine($"Синтаксис запитів ({sw.ElapsedTicks} ticks):");
            q3Query.ForEach(r => Console.WriteLine($" - {r.Title}: {r.Count}"));


            Console.WriteLine("\nЗапит 4: Наймолодший студент");
            
            // Синтаксис методів
            sw.Restart();
            var q4Method = students.OrderBy(s => s.Age).FirstOrDefault();
            sw.Stop();
            Console.WriteLine($"Синтаксис методів ({sw.ElapsedTicks} ticks): {q4Method?.Name} ({q4Method?.Age} р.)");

            // Синтаксис запитів
            sw.Restart();
            var q4Query = (from s in students
                           orderby s.Age
                           select s).FirstOrDefault();
            sw.Stop();
            Console.WriteLine($"Синтаксис запитів ({sw.ElapsedTicks} ticks): {q4Query?.Name} ({q4Query?.Age} р.)");
        }
    }
}