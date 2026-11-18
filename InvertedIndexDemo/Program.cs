using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedIndexDemo
{
    // 1. клас Document
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public override string ToString() => $"[{Id}] {Title}";
    }

    // 2. utility методи для обробки тексту
    public static class TextProcessor
    {
        public static IEnumerable<string> Tokenize(string text)
        {
            // переводимо в нижній регістр та вилучаємо пунктуацію за допомогою Regex
            var matches = Regex.Matches(text.ToLowerInvariant(), @"\w+");
            return matches.Select(m => m.Value);
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            var stopWords = new HashSet<string>
            {
                "a", "an", "the", "is", "are", "was", "were",
                "in", "on", "at", "to", "for", "of", "and", "or"
            };
            return words.Where(w => !stopWords.Contains(w));
        }
    }

    // 3. клас InvertedIndex
    public class InvertedIndex
    {
        private Dictionary<string, HashSet<int>> _index;
        private Dictionary<int, Document> _documents;

        public InvertedIndex()
        {
            _index = new Dictionary<string, HashSet<int>>();
            _documents = new Dictionary<int, Document>();
        }

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;
            var words = TextProcessor.Tokenize(doc.Content);
            
            foreach (var word in words)
            {
                if (!_index.ContainsKey(word))
                {
                    _index[word] = new HashSet<int>();
                }
                _index[word].Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var normalizedWord = word.ToLowerInvariant();
            if (_index.TryGetValue(normalizedWord, out var docIds))
            {
                return docIds.Select(id => _documents[id]).ToList();
            }
            return new List<Document>();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            if (words.Length == 0) return new List<Document>();

            var normalizedWords = words.Select(w => w.ToLowerInvariant()).ToList();
            HashSet<int>? resultIds = null;

            foreach (var word in normalizedWords)
            {
                if (_index.TryGetValue(word, out var docIds))
                {
                    if (resultIds == null)
                    {
                        resultIds = new HashSet<int>(docIds);
                    }
                    else
                    {
                        resultIds.IntersectWith(docIds);
                    }
                }
                else
                {
                    // якщо хоча б одного слова немає, результат порожній
                    return new List<Document>();
                }
            }

            return resultIds?.Select(id => _documents[id]).ToList() ?? new List<Document>();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            var resultIds = new HashSet<int>();
            var normalizedWords = words.Select(w => w.ToLowerInvariant());

            foreach (var word in normalizedWords)
            {
                if (_index.TryGetValue(word, out var docIds))
                {
                    resultIds.UnionWith(docIds);
                }
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;
    }

    // 4. вимірювання продуктивності
    public class PerformanceTest
    {
        public static void CompareSearchMethods(List<Document> documents, string searchWord)
        {
            var sw = new Stopwatch();

            // 1. лінійний пошук
            sw.Start();
            var normalizedWord = searchWord.ToLowerInvariant();
            var linearResults = documents.Where(d =>
                TextProcessor.Tokenize(d.Content).Contains(normalizedWord)).ToList();
            sw.Stop();
            long linearTicks = sw.ElapsedTicks;
            Console.WriteLine($"Linear search: {linearTicks} ticks");

            // 2. inverted index
            var index = new InvertedIndex();
            foreach (var doc in documents)
                index.AddDocument(doc);

            sw.Restart();
            var indexResults = index.SearchSingleWord(searchWord);
            sw.Stop();
            long indexTicks = sw.ElapsedTicks;
            Console.WriteLine($"Inverted Index: {indexTicks} ticks");

            // перевірка коректності
            Console.WriteLine($"Linear found: {linearResults.Count}, Index found: {indexResults.Count}");
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var baseDocuments = new List<Document>
            {
                new Document { Id = 1, Title = "Introduction to C#", Content = "C# is a modern object-oriented programming language developed by Microsoft." },
                new Document { Id = 2, Title = "Java Basics", Content = "Java is a popular programming language known for its portability and performance." },
                new Document { Id = 3, Title = "Python Overview", Content = "Python is a high-level programming language with simple syntax." },
                new Document { Id = 4, Title = "C# Advanced Features", Content = "C# supports LINQ, async/await, and many modern programming paradigms." },
                new Document { Id = 5, Title = "Comparing Languages", Content = "C#, Java, and Python are among the most popular programming languages today." }
            };

            Console.WriteLine("performance testing\n");
            
            // генерація 10 документів
            var docs10 = GenerateDocuments(baseDocuments, 10);
            Console.WriteLine("Testing with 10 documents:");
            PerformanceTest.CompareSearchMethods(docs10, "programming");

            // генерація 1000 документів
            var docs1000 = GenerateDocuments(baseDocuments, 1000);
            Console.WriteLine("Testing with 1000 documents:");
            PerformanceTest.CompareSearchMethods(docs1000, "programming");

            Console.WriteLine("interactive demo\n");
            var index = new InvertedIndex();
            foreach (var doc in baseDocuments) index.AddDocument(doc);
            
            Console.WriteLine($"Loaded {index.DocumentCount} documents");
            Console.WriteLine($"Index contains {index.UniqueWords} unique words\n");

            while (true)
            {
                Console.Write("Enter search query (or 'exit'): ");
                var query = Console.ReadLine();
                if (query == "exit") break;
                if (string.IsNullOrWhiteSpace(query)) continue;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine("\nsearch results and");
                
                var results = index.SearchMultipleWords(words);
                foreach (var doc in results)
                {
                    Console.WriteLine(doc);
                    Console.WriteLine($"  {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}...");
                }
                Console.WriteLine($"\nFound {results.Count} documents\n");
            }
        }

        static List<Document> GenerateDocuments(List<Document> baseDocs, int count)
        {
            var generated = new List<Document>();
            for (int i = 0; i < count; i++)
            {
                var template = baseDocs[i % baseDocs.Count];
                generated.Add(new Document
                {
                    Id = i + 1,
                    Title = $"{template.Title} Vol.{i}",
                    Content = template.Content
                });
            }
            return generated;
        }
    }
}