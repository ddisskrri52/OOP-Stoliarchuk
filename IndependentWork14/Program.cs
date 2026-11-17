using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndependentWork14
{
    // 1 сценарій 
     public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; } = string.Empty;

        public override string ToString() =>
            $"{FileName} | Розмір: {Size} байт | Змінено: {LastModified:yyyy-MM-dd HH:mm:ss} | Хеш: {Hash}";
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new(StringComparer.Ordinal);

        public void Add(string fileName, FileMetadata metadata)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Ім'я файлу не може бути порожнім.", nameof(fileName));

            ArgumentNullException.ThrowIfNull(metadata);

            _cache[fileName] = metadata;
        }

        public FileMetadata Get(string fileName)
        {
            if (!_cache.TryGetValue(fileName, out var metadata))
            {
                throw new KeyNotFoundException($"Файл '{fileName}' відсутній у кеші метаданих.");
            }

            return metadata;
        }

        public bool Remove(string fileName) => _cache.Remove(fileName);

        public bool Contains(string fileName) => _cache.ContainsKey(fileName);
    }

    // 2 сценарій
    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new(StringComparer.OrdinalIgnoreCase);

        public void AddTag(string articleId, string tag)
        {
            if (string.IsNullOrWhiteSpace(articleId))
                throw new ArgumentException("Ідентифікатор статті не може бути порожнім.", nameof(articleId));

            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Тег не може бути порожнім.", nameof(tag));

            if (!_articleTags.TryGetValue(articleId, out var tags))
            {
                tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _articleTags[articleId] = tags;
            }

            tags.Add(tag.Trim());
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
            {
                tags.Remove(tag.Trim());
                if (tags.Count == 0)
                {
                    _articleTags.Remove(articleId);
                }
            }
        }

        // UnionWith: об'єднання всіх унікальних тегів усіх статей
        public HashSet<string> GetAllUniqueTags()
        {
            var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tagSet in _articleTags.Values)
            {
                allTags.UnionWith(tagSet);
            }
            return allTags;
        }

        // IntersectWith: спільні теги двох статей
        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (!_articleTags.TryGetValue(articleId1, out var tags1) ||
                !_articleTags.TryGetValue(articleId2, out var tags2))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            var commonTags = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
            commonTags.IntersectWith(tags2);
            return commonTags;
        }

        // ExceptWith: теги першої статті, яких немає у другій
        public HashSet<string> FindDifferenceTags(string articleId1, string articleId2)
        {
            if (!_articleTags.TryGetValue(articleId1, out var tags1))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            var diffTags = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
            if (_articleTags.TryGetValue(articleId2, out var tags2))
            {
                diffTags.ExceptWith(tags2);
            }
            return diffTags;
        }
    }

    // демонстрація
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            DemonstrateFileMetadataCache();
            Console.WriteLine(new string('-', 60));
            DemonstrateTagManager();
        }

        static void DemonstrateFileMetadataCache()
        {
            Console.WriteLine("сценарій 1");
            var cache = new FileMetadataCache();

            // додавання запису з умовним хешем
            cache.Add("Program.cs", new FileMetadata
            {
                FileName = "Program.cs",
                Size = 2048,
                LastModified = new DateTime(2026, 11, 17, 14, 30, 0),
                Hash = "demo-hash"
            });

            // повторне додавання оновлює метадані
            cache.Add("Program.cs", new FileMetadata
            {
                FileName = "Program.cs",
                Size = 4096,
                LastModified = new DateTime(2026, 11, 17, 15, 0, 0),
                Hash = "demo-hash"
            });

            Console.WriteLine($"Contains(\"Program.cs\"): {cache.Contains("Program.cs")}");
            Console.WriteLine($"Contains(\"program.cs\") [Ordinal]: {cache.Contains("program.cs")}");
            Console.WriteLine($"Отримано: {cache.Get("Program.cs")}");

            // спроба отримати відсутній ключ
            try
            {
                cache.Get("program.cs");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Очікуваний виняток: {ex.Message}");
            }

            cache.Remove("Program.cs");
            Console.WriteLine($"Після видалення Contains(\"Program.cs\"): {cache.Contains("Program.cs")}");
        }

        static void DemonstrateTagManager()
        {
            Console.WriteLine("сценарій 2");
            var tagManager = new TagManager();

            // додавання тегів (дублікати та різні регістри ігноруються)
            tagManager.AddTag("post-1", "csharp");
            tagManager.AddTag("post-1", "CSHARP");
            tagManager.AddTag("post-1", "dotnet");
            tagManager.AddTag("post-1", "linq");

            tagManager.AddTag("post-2", "DotNet");
            tagManager.AddTag("post-2", "collections");
            tagManager.AddTag("post-2", "performance");

            var allTags = tagManager.GetAllUniqueTags();
            Console.WriteLine($"Усі унікальні теги (UnionWith): {string.Join(", ", allTags.OrderBy(t => t))}");

            var commonTags = tagManager.FindCommonTags("post-1", "post-2");
            Console.WriteLine($"Спільні теги post-1 та post-2 (IntersectWith): {string.Join(", ", commonTags.OrderBy(t => t))}");

            var diffTags = tagManager.FindDifferenceTags("post-1", "post-2");
            Console.WriteLine($"Теги лише у post-1 (ExceptWith): {string.Join(", ", diffTags.OrderBy(t => t))}");

            // перевірка захисту внутрішнього стану (зміна отриманої копії не впливає на TagManager)
            allTags.Clear();
            Console.WriteLine($"Кількість тегів у менеджері після очищення зовнішньої копії: {tagManager.GetAllUniqueTags().Count}");

            tagManager.RemoveTag("post-1", "linq");
            Console.WriteLine($"Усі теги після видалення 'linq': {string.Join(", ", tagManager.GetAllUniqueTags().OrderBy(t => t))}");
        }
    }
}