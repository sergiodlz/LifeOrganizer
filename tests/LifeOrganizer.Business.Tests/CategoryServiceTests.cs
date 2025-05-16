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
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new CategoryService(_unitOfWorkMock.Object /*, other dependencies */);
        }

        [Fact]
        public async Task GetCategories_ReturnsCategoriesForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var categories = new List<Category> { new Category { Id = Guid.NewGuid(), UserId = userId, Name = "TestCategory" } };
            _unitOfWorkMock.Setup(u => u.Repository<Category>().Query()).Returns(categories.AsQueryable());

            // Act
            var result = await _service.GetCategoriesForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TestCategory", result.First().Name);
        }
    }
}
