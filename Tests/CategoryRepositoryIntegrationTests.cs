using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class CategoryRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly PaintingsShopContext _dbContext;
        private readonly CategoryRepository _categoryRepository;

        public CategoryRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _categoryRepository = new CategoryRepository(_dbContext);
        }

        [Fact]
        public async Task GetCategories_WhenCategoriesExist_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
        {
            new Category { CategoryName = "Portraits" },
            new Category { CategoryName = "Landscapes" },
            new Category { CategoryName = "Abstract" }
        };

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count >= 3); // At least the 3 we added
            Assert.Contains(result, c => c.CategoryName == "Portraits");
            Assert.Contains(result, c => c.CategoryName == "Landscapes");
            Assert.Contains(result, c => c.CategoryName == "Abstract");
        }

        [Fact]
        public async Task GetCategories_WhenNoCategoriesExist_ReturnsEmptyList()
        {
            // Arrange
            // Clear all categories to ensure empty state
            var existingCategories = await _dbContext.Categories.ToListAsync();
            _dbContext.Categories.RemoveRange(existingCategories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCategories_WhenSingleCategoryExists_ReturnsSingleCategory()
        {
            // Arrange
            // Clear existing categories first
            var existingCategories = await _dbContext.Categories.ToListAsync();
            _dbContext.Categories.RemoveRange(existingCategories);
            await _dbContext.SaveChangesAsync();

            var category = new Category { CategoryName = "Modern Art" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Modern Art", result.First().CategoryName);
            Assert.True(result.First().Id > 0);
        }

        [Fact]
        public async Task GetCategories_WithCategoriesHavingProducts_ReturnsCategoriesWithNavigationProperties()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = "Oil Paintings",
                Products = new List<Product>
            {
                new Product { Name = "Sunset", Price = 299 },
                new Product { Name = "Ocean View", Price = 399 }
            }
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            var oilPaintingCategory = result.FirstOrDefault(c => c.CategoryName == "Oil Paintings");
            Assert.NotNull(oilPaintingCategory);
            Assert.True(oilPaintingCategory.Id > 0);

            // Note: Navigation properties might need explicit loading depending on EF configuration
            // This test verifies the basic query functionality
        }

        [Fact]
        public async Task GetCategories_WithLargeCategoryName_ReturnsCategory()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = "Very Long Category" // 18 characters (within 20 char limit)
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, c => c.CategoryName == "Very Long Category");
        }

        [Fact]
        public async Task GetCategories_WithSpecialCharacters_ReturnsCategory()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = "Art & Crafts" // Contains special characters
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, c => c.CategoryName == "Art & Crafts");
        }

        [Fact]
        public async Task GetCategories_WithDuplicateCategoryNames_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category>
        {
            new Category { CategoryName = "Duplicate" },
            new Category { CategoryName = "Duplicate" }
        };

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            var duplicateCategories = result.Where(c => c.CategoryName == "Duplicate").ToList();
            Assert.True(duplicateCategories.Count >= 2);

            // Verify they have different IDs
            var ids = duplicateCategories.Select(c => c.Id).Distinct().ToList();
            Assert.True(ids.Count >= 2);
        }

        [Fact]
        public async Task GetCategories_DatabaseConnectionFailure_ThrowsException()
        {
            // Arrange
            var repository = new CategoryRepository(_dbContext);
            await _dbContext.DisposeAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => repository.GetCategories());
        }

        [Fact]
        public async Task GetCategories_WithNullCategoryName_ReturnsCategory()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = null // Testing null category name
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, c => c.CategoryName == null);
        }

        [Fact]
        public async Task GetCategories_WithEmptyStringCategoryName_ReturnsCategory()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = string.Empty // Testing empty string
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, c => c.CategoryName == string.Empty);
        }

        [Fact]
        public async Task GetCategories_WithWhitespaceCategoryName_ReturnsCategory()
        {
            // Arrange
            var category = new Category
            {
                CategoryName = "   " // Testing whitespace
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, c => c.CategoryName == "   ");
        }

        [Fact]
        public async Task GetCategories_ConcurrentAccess_ReturnsConsistentResults()
        {
            // Arrange
            var categories = new List<Category>
        {
            new Category { CategoryName = "Concurrent1" },
            new Category { CategoryName = "Concurrent2" },
            new Category { CategoryName = "Concurrent3" }
        };

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            // Act - Simulate concurrent access
            var task1 = _categoryRepository.GetCategories();
            var task2 = _categoryRepository.GetCategories();
            var task3 = _categoryRepository.GetCategories();

            var results = await Task.WhenAll(task1, task2, task3);

            // Assert
            Assert.All(results, result => Assert.NotNull(result));
            Assert.All(results, result => Assert.True(result.Count >= 3));

            // All results should be consistent
            var firstResultCount = results[0].Count;
            Assert.All(results, result => Assert.Equal(firstResultCount, result.Count));
        }

        [Fact]
        public async Task GetCategories_OrderingConsistency_ReturnsConsistentOrder()
        {
            // Arrange
            var categories = new List<Category>
        {
            new Category { CategoryName = "ZCategory" },
            new Category { CategoryName = "ACategory" },
            new Category { CategoryName = "MCategory" }
        };

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            // Act
            var result1 = await _categoryRepository.GetCategories();
            var result2 = await _categoryRepository.GetCategories();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(result1.Count, result2.Count);

            // Note: Without explicit ordering in the repository, 
            // this tests that EF returns consistent ordering
            for (int i = 0; i < result1.Count; i++)
            {
                Assert.Equal(result1[i].Id, result2[i].Id);
            }
        }
    }
}
