using Xunit;
using Moq;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LifeOrganizer.Data.Tests
{
    public class AccountRepositoryTests
    {
        [Fact]
        public void GetAccounts_ReturnsAccounts()
        {
            // Arrange
            var accounts = new List<Account> { new Account { Id = Guid.NewGuid(), Name = "TestAccount" } };
            var repoMock = new Mock<IRepository<Account>>();
            repoMock.Setup(r => r.Query()).Returns(accounts.AsQueryable());

            // Act
            var result = repoMock.Object.Query().ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("TestAccount", result[0].Name);
        }
    }
}
