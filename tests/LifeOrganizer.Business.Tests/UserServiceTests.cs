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
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new UserService(_unitOfWorkMock.Object /*, other dependencies */);
        }

        [Fact]
        public async Task GetUsers_ReturnsUsers()
        {
            // Arrange
            var users = new List<User> { new User { Id = Guid.NewGuid(), Username = "TestUser" } };
            _unitOfWorkMock.Setup(u => u.Repository<User>().Query()).Returns(users.AsQueryable());

            // Act
            var result = await _service.GetUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TestUser", result.First().Username);
        }
    }
}
