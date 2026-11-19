using System.Text;
using System.Runtime.Serialization;

namespace lab13v2;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var service = new UserService();
        ShowRegistration(service, "daryna", "study123");
        ShowRegistration(service, "DARYNA", "another123");
        ShowRegistration(service, "oleh", "123");
        ShowRegistration(service, " ", "password123");

        try
        {
            User user = service.GetUser("DARYNA");
            Console.WriteLine($"Знайдено користувача: {user.Username}");
        }
        catch (UserNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }

        try
        {
            service.GetUser("missing");
        }
        catch (UserNotFoundException ex)
        {
            Console.WriteLine($"Перехоплено UserNotFoundException: {ex.Message}");
            Console.WriteLine($"Username: {ex.Username}");

            Result<User> result = Result<User>.Failure(ex);
            if (!result.IsSuccess)
            {
                Console.WriteLine($"Failure(Exception): {result.Error}");
            }
        }

        Console.WriteLine("Програма продовжила роботу після обробки помилки.");
    }

    private static void ShowRegistration(
        UserService service, string username, string password)
    {
        Result<bool> result = service.RegisterUser(username, password);
        string label = string.IsNullOrWhiteSpace(username)
            ? "<порожньо>"
            : username;

        Console.WriteLine($"Реєстрація '{label}': IsSuccess = {result.IsSuccess}");
        if (result.IsSuccess)
        {
            Console.WriteLine($"  Value = {result.Value}");
        }
        else
        {
            Console.WriteLine($"  Error = {result.Error}");
        }
    }
}

[Serializable]
public class UserNotFoundException : Exception
{
    public string Username { get; } = string.Empty;

    public UserNotFoundException()
        : base("Користувача не знайдено.")
    {
    }

    public UserNotFoundException(string? message)
        : base(message)
    {
    }

    public UserNotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public UserNotFoundException(string username, string message)
        : base(message)
    {
        Username = username;
    }

    
    [Obsolete("Legacy serialization support.", DiagnosticId = "SYSLIB0051")]
    protected UserNotFoundException(
        SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Username = info.GetString(nameof(Username)) ?? string.Empty;
    }

    [Obsolete("Legacy serialization support.", DiagnosticId = "SYSLIB0051")]
    public override void GetObjectData(
        SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);
        base.GetObjectData(info, context);
        info.AddValue(nameof(Username), Username);
    }
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }

    private Result(T value, bool isSuccess, string error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, string.Empty);
    }

    public static Result<T> Failure(string error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result<T>(default!, false, error);
    }

    public static Result<T> Failure(Exception ex)
    {
        ArgumentNullException.ThrowIfNull(ex);
        return Failure(ex.Message);
    }
}

public sealed class User
{
    public string Username { get; }

    public User(string username)
    {
        Username = username;
    }
}

public sealed class UserService
{
    public const int MinimumPasswordLength = 8;

    private readonly Dictionary<string, User> _users =
        new(StringComparer.OrdinalIgnoreCase);

    public User GetUser(string username)
    {
        string normalized = username?.Trim() ?? string.Empty;

        if (normalized.Length == 0 ||
            !_users.TryGetValue(normalized, out User? user))
        {
            throw new UserNotFoundException(
                normalized,
                $"Користувача '{normalized}' не знайдено.");
        }

        return user;
    }

    public Result<bool> RegisterUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return Result<bool>.Failure("Ім'я користувача не може бути порожнім.");
        }

        if (password is null || password.Length < MinimumPasswordLength)
        {
            return Result<bool>.Failure(
                $"Пароль має містити щонайменше {MinimumPasswordLength} символів.");
        }

        string normalized = username.Trim();
        if (_users.ContainsKey(normalized))
        {
            return Result<bool>.Failure(
                $"Користувач '{normalized}' уже існує.");
        }

        _users.Add(normalized, new User(normalized));
        return Result<bool>.Success(true);
    }
}
