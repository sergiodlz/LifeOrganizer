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
    public class SubcategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly SubcategoryService _service;

        public SubcategoryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new SubcategoryService(_unitOfWorkMock.Object /*, other dependencies */);
        }

        [Fact]
        public async Task GetSubcategories_ReturnsSubcategoriesForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var subcategories = new List<Subcategory> { new Subcategory { Id = Guid.NewGuid(), UserId = userId, Name = "TestSubcategory" } };
            _unitOfWorkMock.Setup(u => u.Repository<Subcategory>().Query()).Returns(subcategories.AsQueryable());

            // Act
            var result = await _service.GetSubcategoriesForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TestSubcategory", result.First().Name);
        }
    }
}
