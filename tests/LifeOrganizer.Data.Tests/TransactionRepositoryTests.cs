using Xunit;
using Moq;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LifeOrganizer.Data.Tests
{
    public class TransactionRepositoryTests
    {
        [Fact]
        public void GetTransactions_ReturnsTransactions()
        {
            var transactions = new List<Transaction> { new Transaction { Id = Guid.NewGuid(), Description = "TestTransaction" } };
            var repoMock = new Mock<IRepository<Transaction>>();
            repoMock.Setup(r => r.Query()).Returns(transactions.AsQueryable());

            var result = repoMock.Object.Query().ToList();

            Assert.Single(result);
            Assert.Equal("TestTransaction", result[0].Description);
        }
    }
}
