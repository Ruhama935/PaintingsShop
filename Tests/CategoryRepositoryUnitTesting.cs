using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CategoryRepositoryUnitTesting
    {
        [Fact]
        public async Task GetCategories_ReturnsListOfCategories()
        {
            // Arrange
            var category1 = new Category { Id = 1, CategoryName = "Landscapes"};
            var category2 = new Category { Id = 2, CategoryName = "Portraits"};
            var categories = new List<Category> { category1, category2 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Category>>();

            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

            var categoryRepository = new CategoryRepository(mockContext.Object);

            // Act
            var result = await categoryRepository.GetCategories();

            // Assert
            Assert.Equal(categories, result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Landscapes", result[0].CategoryName);
            Assert.Equal("Portraits", result[1].CategoryName);
        }

        [Fact]
        public async Task GetCategories_WhenNoCategoriesExist_ReturnsEmptyList()
        {
            // Arrange
            var categories = new List<Category>();

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Category>>();

            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(categories.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(categories.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(categories.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(categories.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Categories).Returns(mockDbSet.Object);

            var categoryRepository = new CategoryRepository(mockContext.Object);

            // Act
            var result = await categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
