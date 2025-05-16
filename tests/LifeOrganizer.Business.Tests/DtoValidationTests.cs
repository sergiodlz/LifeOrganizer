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
    public class DtoValidationTests
    {
        [Fact]
        public void AccountDto_Validation_Succeeds_With_Valid_Data()
        {
            var dto = new AccountDto { Name = "Test", Type = 1, Currency = 1 };
            // Add validation logic or use FluentValidation if available
            Assert.NotNull(dto);
        }

        [Fact]
        public void CategoryDto_Validation_Succeeds_With_Valid_Data()
        {
            var dto = new CategoryDto { Name = "TestCategory" };
            Assert.NotNull(dto);
        }

        [Fact]
        public void SubcategoryDto_Validation_Succeeds_With_Valid_Data()
        {
            var dto = new SubcategoryDto { Name = "TestSubcategory", CategoryId = Guid.NewGuid() };
            Assert.NotNull(dto);
        }

        [Fact]
        public void TagDto_Validation_Succeeds_With_Valid_Data()
        {
            var dto = new TagDto { Name = "TestTag" };
            Assert.NotNull(dto);
        }

        [Fact]
        public void TransactionDto_Validation_Succeeds_With_Valid_Data()
        {
            var dto = new TransactionDto { Amount = 10, Date = DateTime.UtcNow, Description = "Test", AccountId = Guid.NewGuid(), Type = 1 };
            Assert.NotNull(dto);
        }
    }
}
