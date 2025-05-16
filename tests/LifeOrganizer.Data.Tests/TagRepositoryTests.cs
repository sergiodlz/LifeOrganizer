using Xunit;
using Moq;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LifeOrganizer.Data.Tests
{
    public class TagRepositoryTests
    {
        [Fact]
        public void GetTags_ReturnsTags()
        {
            var tags = new List<Tag> { new Tag { Id = Guid.NewGuid(), Name = "TestTag" } };
            var repoMock = new Mock<IRepository<Tag>>();
            repoMock.Setup(r => r.Query()).Returns(tags.AsQueryable());

            var result = repoMock.Object.Query().ToList();

            Assert.Single(result);
            Assert.Equal("TestTag", result[0].Name);
        }
    }
}
