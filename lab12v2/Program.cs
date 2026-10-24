using System;
using System.Collections.Generic;
using System.Linq;

namespace lab12v2
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class User : IEntity
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    // узагальнений інтерфейс репозиторію
    public interface IRepository<T> where T : IEntity
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    // реалізація репозиторію в пам'яті
    public class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly List<T> _items = new();

        public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

        public IEnumerable<T> GetAll() => _items.ToList();

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _items.Add(item);
        }

        public void Update(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var index = _items.FindIndex(x => x.Id == item.Id);
            if (index == -1) throw new KeyNotFoundException($"Елемент з Id={item.Id} не знайдено.");
            _items[index] = item;
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item == null) throw new KeyNotFoundException($"Елемент з Id={id} не знайдено.");
            _items.Remove(item);
        }
    }

    // точка входу в програму
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демонстрація роботи InMemoryRepository<User>");

            IRepository<User> userRepository = new InMemoryRepository<User>();

            userRepository.Add(new User { Id = 1, Username = "john_doe", Email = "john@example.com", IsActive = true });
            userRepository.Add(new User { Id = 2, Username = "jane_smith", Email = "jane@example.com", IsActive = false });

            Console.WriteLine("\nСписок користувачів:");
            foreach (var user in userRepository.GetAll())
            {
                Console.WriteLine($"ID: {user.Id}, Username: {user.Username}, Email: {user.Email}, Active: {user.IsActive}");
            }

            var userToUpdate = userRepository.GetById(1);
            if (userToUpdate != null)
            {
                userToUpdate.Email = "john_updated@example.com";
                userRepository.Update(userToUpdate);
            }

            userRepository.Delete(2);
            Console.WriteLine("\nЗалишилось користувачів після видалення ID=2: " + userRepository.GetAll().Count());
        }
    }
}