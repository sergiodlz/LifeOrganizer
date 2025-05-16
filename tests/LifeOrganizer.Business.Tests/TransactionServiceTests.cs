using Xunit;
using Moq;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Data.UnitOfWorkPattern;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Business.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LifeOrganizer.Business.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new TransactionService(_unitOfWorkMock.Object /*, other dependencies */);
        }

        [Fact]
        public async Task GetTransactions_ReturnsTransactionsForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var transactions = new List<Transaction> { new Transaction { Id = Guid.NewGuid(), UserId = userId, Description = "TestTransaction" } };
            _unitOfWorkMock.Setup(u => u.Repository<Transaction>().Query()).Returns(transactions.AsQueryable());

            // Act
            var result = await _service.GetTransactionsForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TestTransaction", result.First().Description);
        }
    }
}
