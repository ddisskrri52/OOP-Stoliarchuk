using System;
using System.Collections.Generic;
using IndependentWork13;
using Xunit;

namespace IndependentWork13.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_ValidElements_AddsToCache()
        {
            var cache = new Cache<int, string>(3);

            cache.Add(1, "One");
            cache.Add(2, "Two");

            Assert.True(cache.ContainsKey(1));
            Assert.True(cache.ContainsKey(2));
            Assert.Equal("One", cache.Get(1));
            Assert.Equal("Two", cache.Get(2));
        }

        [Fact]
        public void Get_ExistingKey_ReturnsCorrectValue()
        {
            var cache = new Cache<string, int>(2);
            cache.Add("age", 25);

            int result = cache.Get("age");

            Assert.Equal(25, result);
        }

        [Fact]
        public void Get_NonExistentKey_ThrowsKeyNotFoundException()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("k1", "v1");

            Assert.Throws<KeyNotFoundException>(() => cache.Get("missing_key"));
        }

        [Fact]
        public void Add_ExceedingCapacity_EvictsOldestElementFifo()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "First");
            cache.Add(2, "Second");

            _ = cache.Get(1);

            cache.Add(3, "Third");

            Assert.False(cache.ContainsKey(1));
            Assert.True(cache.ContainsKey(2));
            Assert.True(cache.ContainsKey(3));
            Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
        }

        [Fact]
        public void ContainsKey_ReturnsTrueForExistingAndFalseForMissing()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("active", "yes");

            Assert.True(cache.ContainsKey("active"));
            Assert.False(cache.ContainsKey("inactive"));
        }

        [Fact]
        public void Count_TracksElementsAndDoesNotExceedCapacity()
        {
            var cache = new Cache<int, int>(2);
            Assert.Equal(0, cache.Count);

            cache.Add(10, 100);
            Assert.Equal(1, cache.Count);

            cache.Add(20, 200);
            Assert.Equal(2, cache.Count);

            cache.Add(30, 300);
            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void Add_ExistingKey_UpdatesValueWithoutEvicting()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("a", "old");
            cache.Add("b", "second");

            cache.Add("a", "new");

            Assert.Equal(2, cache.Count);
            Assert.Equal("new", cache.Get("a"));
            Assert.True(cache.ContainsKey("b"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Constructor_InvalidCapacity_ThrowsArgumentOutOfRangeException(int invalidCapacity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<int, int>(invalidCapacity));
        }
    }
}