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
    public class AccountServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly AccountService _service;

        public AccountServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new AccountService(_unitOfWorkMock.Object /*, other dependencies */);
        }

        [Fact]
        public async Task GetAccounts_ReturnsAccountsForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var accounts = new List<Account> { new Account { Id = Guid.NewGuid(), UserId = userId, Name = "Test" } };
            _unitOfWorkMock.Setup(u => u.Repository<Account>().Query()).Returns(accounts.AsQueryable());

            // Act
            var result = await _service.GetAccountsForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test", result.First().Name);
        }
    }
}
