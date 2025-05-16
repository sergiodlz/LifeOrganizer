using Xunit;
using Moq;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LifeOrganizer.Data.Tests
{
    public class CategoryRepositoryTests
    {
        [Fact]
        public void GetCategories_ReturnsCategories()
        {
            var categories = new List<Category> { new Category { Id = Guid.NewGuid(), Name = "TestCategory" } };
            var repoMock = new Mock<IRepository<Category>>();
            repoMock.Setup(r => r.Query()).Returns(categories.AsQueryable());

            var result = repoMock.Object.Query().ToList();

            Assert.Single(result);
            Assert.Equal("TestCategory", result[0].Name);
        }
    }
}
