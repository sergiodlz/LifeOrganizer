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
    public class TagServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly TagService _service;

        public TagServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new TagService(_unitOfWorkMock.Object /*, other dependencies */);
        }

        [Fact]
        public async Task GetTags_ReturnsTagsForUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tags = new List<Tag> { new Tag { Id = Guid.NewGuid(), UserId = userId, Name = "TestTag" } };
            _unitOfWorkMock.Setup(u => u.Repository<Tag>().Query()).Returns(tags.AsQueryable());

            // Act
            var result = await _service.GetTagsForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("TestTag", result.First().Name);
        }
    }
}
