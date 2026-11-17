## Самостійна робота No14
# Тема: Вивчення Dictionary<TKey, TValue> та HashSet<T> у бібліотеках .NET
# Мета: Дослідити реальні приклади використання узагальнених колекцій у популярних .NET-бібліотеках та зрозуміти, як вони застосовуються для вирішення практичних задач.

# Приклад 1
Використання Dictionary<TKey, TValue> у перевантаженнях методу ToDictionary
Джерело: ToCollection.cs у .NET Runtime (v10.0.0)
public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
    this IEnumerable<KeyValuePair<TKey, TValue>> source, 
    IEqualityComparer<TKey>? comparer) where TKey : notnull
{
    if (source is null)
    {
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
    }

    return new(source, comparer);
}

public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
    this IEnumerable<(TKey Key, TValue Value)> source, 
    IEqualityComparer<TKey>? comparer) where TKey : notnull =>
    source.ToDictionary(vt => vt.Key, vt => vt.Value, comparer);

Контекст (Яка задача вирішується?):Методи розширення Enumerable.ToDictionary матеріалізують ліниву послідовність пар «ключ-значення» (KeyValuePair<TKey, TValue>) або кортежів (TKey Key, TValue Value) у готову асоціативну таблицю для подальшої швидкої вибірки даних.

Чому обрано саме Dictionary<TKey, TValue>?Головне призначення результуючої структури — забезпечити прямий доступ до значення за довільним унікальним ключем без перебору всіх елементів. Якби замість словника використовувалися List<KeyValuePair<TKey, TValue>> або масив, кожен пошук вимагав би лінійного сканування колекції, а перевірка унікальності ключів під час створення перетворилася б на вкладений цикл.

Складність операцій:Додавання одного елемента та пошук за ключем — O(1) у середньому випадку (амортизовано).Побудова всього словника з n елементів — O(n) у середньому.У найгіршому випадку (при масових колізіях хеш-кодів або в момент збільшення ємності внутрішнього масиву resize) вставка окремого елемента може зайняти O(n).

Особливі вимоги:На узагальнений параметр типу ключа накладено обмеження where TKey : notnull, оскільки хеш-таблиця .NET не допускає null як ключ.Усі ключі мають бути унікальними: при спробі вставити дублікат конструктор Dictionary викине ArgumentException.Підтримується передача кастомного компаратора IEqualityComparer<TKey>? comparer (наприклад, StringComparer.OrdinalIgnoreCase), який перевизначає стандартну логіку обчислення хешу та порівняння ключів.

# Приклад 2
Використання HashSet<T> у методах Distinct та DistinctBy
Джерело: Distinct.cs у .NET Runtime (v10.0.0)
private HashSet<TSource>? _set; // Поле всередині ітератора DistinctIterator
private static IEnumerable<TSource> DistinctByIterator<TSource, TKey>(
    IEnumerable<TSource> source, 
    Func<TSource, TKey> keySelector, 
    IEqualityComparer<TKey>? comparer)
{
    using IEnumerator<TSource> enumerator = source.GetEnumerator();

    if (enumerator.MoveNext())
    {
        var set = new HashSet<TKey>(DefaultInternalSetCapacity, comparer);
        do
        {
            TSource element = enumerator.Current;
            if (set.Add(keySelector(element)))
            {
                yield return element;
            }
        }
        while (enumerator.MoveNext());
    }
}
Контекст (Яка задача вирішується?):
Методи Distinct (через поле _set) та DistinctBy (через локальну змінну set) виконують потокову фільтрацію послідовності, пропускаючи на вихід (yield return) лише перше входження елемента з унікальним ключем і відсіюючи всі наступні дублікати.

Чому обрано саме HashSet<T>?Для фільтрації дублікатів потрібно лише зафіксувати факт того, що певний ключ уже зустрічався раніше. Зберігати додаткове значення (як у Dictionary) тут зайве — це б марно витрачало пам'ять. Використання List<TKey> із перевіркою !list.Contains(key) змусило б на кожному кроці переглядати всі попередні унікальні елементи, що погіршило б швидкодію алгоритму з лінійної до квадратичної (O(n2)). Крім того, метод set.Add(...) одночасно виконує і перевірку наявності, і додавання за один прохід по хеш-таблиці.

Складність операцій:Операція set.Add(...) (перевірка + вставка) виконується за O(1) у середньому випадку.Загальна часова складність обробки послідовності з n елементів становить O(n).Витрати пам'яті становлять O(u), де u — кількість унікальних ключів у вхідному потоці.

Особливі вимоги:Ініціалізація відбувається з початковою ємністю DefaultInternalSetCapacity, щоб зменшити кількість перевиділень пам'яті на старті.Приймається IEqualityComparer<TKey>? comparer, який дозволяє гнучко налаштувати критерій унікальності (наприклад, вважати рядки "Admin" та "admin" однаковими).
|операція|List<T>|Dictionary<TKey, TValue>|HashSet<T>|коли використовувати|
|додавання|О(1)|О(1)|О(1)|List - збереження порядку; Dictionary - пари «ключ-значення»; HashSet - набір унікальних елементів.|
|пошук за значенням|O(n)|O(n)|O(1)|HashSet — найефективніший для швидкої перевірки належності елемента.|
|пошук за ключем|не застосовується(лише за індексом)|О(1)|не застосовується|Dictionary — миттєве отримання об'єкта за його унікальним ключем.|
|видалення|O(n)|O(1)|O(1)|Dictionary / HashSet — при частому видаленні елементів із середини колекції.|
|перевірка наявності|O(n)|О(1)|О(1)|Dictionary — для перевірки ключа; HashSet — для перевірки самого елемента.|
