using System.Text;

namespace IndependentWork13;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var cache = new Cache<string, string>(2);

        cache.Add("A", "Перше значення");
        cache.Add("B", "Друге значення");

        Console.WriteLine($"A: {cache.Get("A")}");
        Console.WriteLine(
            $"Кількість до переповнення: {cache.Count}");

        // Кеш повний, тому найстаріший ключ A буде видалено.
        cache.Add("C", "Третє значення");

        Console.WriteLine($"Є ключ A: {cache.ContainsKey("A")}");
        Console.WriteLine($"Є ключ B: {cache.ContainsKey("B")}");
        Console.WriteLine($"C: {cache.Get("C")}");
        Console.WriteLine(
            $"Кількість після переповнення: {cache.Count}");

        try
        {
            cache.Get("A");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}