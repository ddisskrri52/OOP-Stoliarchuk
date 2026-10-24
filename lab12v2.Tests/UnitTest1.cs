using System;
using System.Collections.Generic;
using System.Linq;
using lab12v2;
using Xunit;

namespace lab12v2.Tests
{
    public class InMemoryRepositoryTests
    {
        [Fact]
        public void Add_ShouldAddItemToRepository()
        {
            var repository = new InMemoryRepository<User>();
            var user = new User { Id = 1, Username = "admin", Email = "admin@site.com", IsActive = true };

            repository.Add(user);

            var result = repository.GetById(1);
            Assert.NotNull(result);
            Assert.Equal("admin", result.Username);
        }

        [Fact]
        public void GetById_ExistingId_ShouldReturnItem()
        {
            var repository = new InMemoryRepository<User>();
            var user = new User { Id = 10, Username = "alex", Email = "alex@test.com", IsActive = true };
            repository.Add(user);

            var result = repository.GetById(10);

            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ShouldReturnNull()
        {
            var repository = new InMemoryRepository<User>();

            var result = repository.GetById(99);

            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllItems()
        {
            var repository = new InMemoryRepository<User>();
            repository.Add(new User { Id = 1, Username = "user1", Email = "u1@test.com", IsActive = true });
            repository.Add(new User { Id = 2, Username = "user2", Email = "u2@test.com", IsActive = false });

            var items = repository.GetAll();

            Assert.Equal(2, items.Count());
        }

        [Fact]
        public void Update_ExistingItem_ShouldUpdateData()
        {
            var repository = new InMemoryRepository<User>();
            repository.Add(new User { Id = 1, Username = "old_name", Email = "old@test.com", IsActive = true });

            var updatedUser = new User { Id = 1, Username = "new_name", Email = "new@test.com", IsActive = false };

            repository.Update(updatedUser);

            var result = repository.GetById(1);
            Assert.NotNull(result);
            Assert.Equal("new_name", result.Username);
        }

        [Fact]
        public void Update_NonExistingItem_ShouldThrowKeyNotFoundException()
        {
            var repository = new InMemoryRepository<User>();
            var user = new User { Id = 5, Username = "ghost", Email = "ghost@test.com", IsActive = true };

            Assert.Throws<KeyNotFoundException>(() => repository.Update(user));
        }

        [Fact]
        public void Delete_ExistingId_ShouldRemoveItem()
        {
            var repository = new InMemoryRepository<User>();
            repository.Add(new User { Id = 1, Username = "to_delete", Email = "del@test.com", IsActive = true });

            repository.Delete(1);

            Assert.Null(repository.GetById(1));
        }

        [Fact]
        public void Delete_NonExistingId_ShouldThrowKeyNotFoundException()
        {
            var repository = new InMemoryRepository<User>();

            Assert.Throws<KeyNotFoundException>(() => repository.Delete(100));
        }
    }
}