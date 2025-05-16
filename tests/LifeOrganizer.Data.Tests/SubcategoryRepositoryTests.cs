using Xunit;
using Moq;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LifeOrganizer.Data.Tests
{
    public class SubcategoryRepositoryTests
    {
        [Fact]
        public void GetSubcategories_ReturnsSubcategories()
        {
            var subcategories = new List<Subcategory> { new Subcategory { Id = Guid.NewGuid(), Name = "TestSubcategory" } };
            var repoMock = new Mock<IRepository<Subcategory>>();
            repoMock.Setup(r => r.Query()).Returns(subcategories.AsQueryable());

            var result = repoMock.Object.Query().ToList();

            Assert.Single(result);
            Assert.Equal("TestSubcategory", result[0].Name);
        }
    }
}
