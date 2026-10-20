using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Book
        Book book = new Book("Тіні забутих предків", "Михайло Коцюбинський", 200);
        book.ReadPages(50);
        Console.WriteLine($"Книга '{book.Title}' ({book.Author})");
        Console.WriteLine($"Прочитано сторінок: {book.PagesRead}");
        Console.WriteLine($"Прогрес: {book.GetProgress():F1}%");
        Console.WriteLine($"Прочитана повністю: {book.IsFinished()}");
        Console.WriteLine();

        // BankAccount
        BankAccount account = new BankAccount("Іван Петренко", 1000);
        account.Deposit(500);
        bool success = account.Withdraw(300);
        Console.WriteLine($"Власник рахунку: {account.Owner}");
        Console.WriteLine($"Зняття 300 грн успішне: {success}");
        Console.WriteLine($"Баланс: {account.Balance:F2} грн");
        Console.WriteLine($"Відсотки (5%): {account.CalculateInterest(5):F2} грн");
        Console.WriteLine();

        // Thermostat
        Thermostat thermostat = new Thermostat("Вітальня", 18.5, 22);
        Console.WriteLine($"Кімната: {thermostat.Room}");
        Console.WriteLine($"Різниця до цілі: {thermostat.GetTemperatureDifference():F1} °C");
        Console.WriteLine($"Потрібен обігрів: {thermostat.NeedsHeating()}");
    }
}

public class Book
{
    // Приватні поля
    private string _title;
    private string _author;
    private int _totalPages;
    private int _pagesRead;

    // Read-only властивість
    public string Title
    {
        get { return _title; }
    }

    // Властивість з get та приватним set
    public string Author
    {
        get { return _author; }
        private set { _author = value; }
    }

    // Властивість з get та set і перевіркою
    public int PagesRead
    {
        get { return _pagesRead; }
        set { _pagesRead = Math.Clamp(value, 0, _totalPages); }
    }

    // Конструктор
    public Book(string title, string author, int totalPages)
    {
        _title = title;
        _author = author;
        _totalPages = totalPages;
        _pagesRead = 0;
    }

    // Метод: додає прочитані сторінки
    public void ReadPages(int pages)
    {
        PagesRead += pages;
    }

    // Метод: обчислює прогрес читання у відсотках
    public double GetProgress()
    {
        return (double)_pagesRead / _totalPages * 100;
    }

    // Метод: перевірка стану
    public bool IsFinished()
    {
        return _pagesRead == _totalPages;
    }
}

public class BankAccount
{
    // Приватні поля
    private string _owner;
    private double _balance;

    // Read-only властивість
    public string Owner
    {
        get { return _owner; }
    }

    // Властивість з get та set
    public double Balance
    {
        get { return _balance; }
        private set { _balance = value; }
    }

    // Конструктор
    public BankAccount(string owner, double initialBalance)
    {
        _owner = owner;
        _balance = initialBalance;
    }

    // Поповнення рахунку
    public void Deposit(double amount)
    {
        if (amount > 0)
            Balance += amount;
    }

    // Зняття коштів з перевіркою стану
    public bool Withdraw(double amount)
    {
        if (amount <= 0 || amount > _balance)
            return false;

        Balance -= amount;
        return true;
    }

    // Нарахування відсотків
    public double CalculateInterest(double percentage)
    {
        return _balance * percentage / 100;
    }
}

public class Thermostat
{
    // Приватні поля
    private string _room;
    private double _currentTemperature;
    private double _targetTemperature;

    // Read-only властивість
    public string Room
    {
        get { return _room; }
    }

    // Властивість з get та set
    public double TargetTemperature
    {
        get { return _targetTemperature; }
        set { _targetTemperature = value; }
    }

    public double CurrentTemperature
    {
        get { return _currentTemperature; }
        set { _currentTemperature = value; }
    }

    // Конструктор
    public Thermostat(string room, double currentTemperature, double targetTemperature)
    {
        _room = room;
        _currentTemperature = currentTemperature;
        _targetTemperature = targetTemperature;
    }

    // Різниця до цільової температури
    public double GetTemperatureDifference()
    {
        return _targetTemperature - _currentTemperature;
    }

    // Перевірка стану: чи потрібно вмикати обігрів
    public bool NeedsHeating()
    {
        return _currentTemperature < _targetTemperature;
    }
}