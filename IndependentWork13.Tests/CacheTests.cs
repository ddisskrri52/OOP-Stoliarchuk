using IndependentWork13;
using Xunit;

namespace IndependentWork13.Tests;

public class CacheTests
{
    [Fact]
    public void Add_StoresNewElement()
    {
        var cache = new Cache<int, string>(2);

        cache.Add(1, "one");

        Assert.True(cache.ContainsKey(1));
        Assert.Equal("one", cache.Get(1));
    }

    [Fact]
    public void Get_ReturnsValueForRequestedKey()
    {
        var cache = new Cache<string, int>(2);

        cache.Add("A", 10);
        cache.Add("B", 20);

        Assert.Equal(20, cache.Get("B"));
    }

    [Fact]
    public void Add_WhenFull_RemovesOldest()
    {
        var cache = new Cache<int, string>(2);

        cache.Add(1, "one");
        cache.Add(2, "two");
        cache.Add(3, "three");

        Assert.False(cache.ContainsKey(1));
        Assert.Equal("two", cache.Get(2));
        Assert.Equal("three", cache.Get(3));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Get_MissingKey_Throws()
    {
        var cache = new Cache<int, string>(2);

        Assert.Throws<KeyNotFoundException>(
            () => cache.Get(99));
    }

    [Fact]
    public void ContainsKey_DetectsPresentAndMissingKeys()
    {
        var cache = new Cache<int, string>(2);

        cache.Add(1, "one");

        Assert.True(cache.ContainsKey(1));
        Assert.False(cache.ContainsKey(2));
    }

    [Fact]
    public void Count_TracksInsertionsAndEviction()
    {
        var cache = new Cache<int, int>(2);

        Assert.Equal(0, cache.Count);

        cache.Add(1, 10);
        Assert.Equal(1, cache.Count);

        cache.Add(2, 20);
        Assert.Equal(2, cache.Count);

        cache.Add(3, 30);
        Assert.Equal(2, cache.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_InvalidCapacity_Throws(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Cache<int, int>(capacity));
    }

    [Fact]
    public void Get_DoesNotChangeFifoOrder()
    {
        var cache = new Cache<int, int>(2);

        cache.Add(1, 10);
        cache.Add(2, 20);

        Assert.Equal(10, cache.Get(1));

        cache.Add(3, 30);

        Assert.Throws<KeyNotFoundException>(
            () => cache.Get(1));

        Assert.True(cache.ContainsKey(2));
    }

    [Fact]
    public void Add_ExistingKey_UpdatesWithoutChangingOrder()
    {
        var cache = new Cache<int, int>(2);

        cache.Add(1, 10);
        cache.Add(2, 20);
        cache.Add(1, 100);

        Assert.Equal(100, cache.Get(1));
        Assert.Equal(20, cache.Get(2));
        Assert.Equal(2, cache.Count);

        cache.Add(3, 30);

        Assert.False(cache.ContainsKey(1));
        Assert.True(cache.ContainsKey(2));
    }

    [Fact]
    public void Add_RepeatedOverflowAndReinsert_PreservesOrder()
    {
        var cache = new Cache<int, int>(2);

        cache.Add(1, 10);
        cache.Add(2, 20);
        cache.Add(3, 30);

        // Повертаємо раніше видалений ключ.
        cache.Add(1, 100);

        Assert.False(cache.ContainsKey(2));

        cache.Add(4, 40);

        Assert.False(cache.ContainsKey(3));
        Assert.Equal(100, cache.Get(1));
        Assert.Equal(40, cache.Get(4));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Add_CapacityOne_KeepsOnlyLatestKey()
    {
        var cache = new Cache<int, int>(1);

        cache.Add(1, 10);
        cache.Add(2, 20);

        Assert.False(cache.ContainsKey(1));
        Assert.Equal(20, cache.Get(2));
        Assert.Equal(1, cache.Count);
    }
}