using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace Tests
{
    public class ProductRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly PaintingsShopContext _dbContext;
        private readonly ProductRepository _productRepository;

        public ProductRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _productRepository = new ProductRepository(_dbContext);
        }

        #region GetAllProducts Tests - Happy & Unhappy Paths

        [Fact]
        public async Task GetAllProducts_WithMultipleProducts_ReturnsAllProducts()
        {
            // Arrange
            var category = new Category { Id = 1, CategoryName = "Test Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product1 = new Product
            {
                CategoryId = category.Id,
                Name = "Mona Lisa",
                Price = 1000,
                Descipition = "Famous painting",
                Url = "http://example.com/mona"
            };
            var product2 = new Product
            {
                CategoryId = category.Id,
                Name = "Starry Night",
                Price = 850,
                Descipition = "Van Gogh piece",
                Url = "http://example.com/starry"
            };
            var product3 = new Product
            {
                CategoryId = category.Id,
                Name = "The Scream",
                Price = 750,
                Descipition = "Expressionist art",
                Url = "http://example.com/scream"
            };

            await _dbContext.Products.AddRangeAsync(product1, product2, product3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count >= 3);
            Assert.Contains(result, p => p.Name == "Mona Lisa");
            Assert.Contains(result, p => p.Name == "Starry Night");
            Assert.Contains(result, p => p.Name == "The Scream");
            Assert.All(result, p => Assert.True(p.Id > 0));
            Assert.All(result, p => Assert.Equal(category.Id, p.CategoryId));

            // Cleanup
            _dbContext.Products.RemoveRange(product1, product2, product3);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProducts_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange - ensure database is clean
            var existingProducts = await _dbContext.Products.ToListAsync();
            _dbContext.Products.RemoveRange(existingProducts);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            Assert.IsType<List<Product>>(result);
        }

        [Fact]
        public async Task GetAllProducts_WithSingleProduct_ReturnsSingleProduct()
        {
            // Arrange
            var existingProducts = await _dbContext.Products.ToListAsync();
            _dbContext.Products.RemoveRange(existingProducts);
            await _dbContext.SaveChangesAsync();

            var category = new Category { Id = 2, CategoryName = "Single Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product = new Product
            {
                CategoryId = category.Id,
                Name = "Single Painting",
                Price = 500,
                Descipition = "Only painting",
                Url = "http://example.com/single"
            };
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Single Painting", result.First().Name);
            Assert.Equal(500, result.First().Price);
            Assert.Equal("Only painting", result.First().Descipition);
            Assert.Equal("http://example.com/single", result.First().Url);

            // Cleanup
            _dbContext.Products.Remove(product);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProducts_WithNullableFields_ReturnsProducts()
        {
            // Arrange
            var category = new Category { Id = 3, CategoryName = "Nullable Test" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var productWithNullPrice = new Product
            {
                CategoryId = category.Id,
                Name = "Free Art",
                Price = null,
                Descipition = "Free sample",
                Url = "http://example.com/free"
            };
            var productWithPrice = new Product
            {
                CategoryId = category.Id,
                Name = "Paid Art",
                Price = 299,
                Descipition = "Premium art",
                Url = "http://example.com/paid"
            };

            await _dbContext.Products.AddRangeAsync(productWithNullPrice, productWithPrice);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, p => p.Price == null);
            Assert.Contains(result, p => p.Price == 299);

            // Cleanup
            _dbContext.Products.RemoveRange(productWithNullPrice, productWithPrice);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProducts_WithMaxStringLengths_ReturnsProducts()
        {
            // Arrange
            var category = new Category { Id = 4, CategoryName = "Max Length Test" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product = new Product
            {
                CategoryId = category.Id,
                Name = new string('A', 20), // Max 20 chars
                Price = int.MaxValue,
                Descipition = new string('B', 20), // Max 20 chars  
                Url = new string('C', 30) // Max 30 chars
            };

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            var maxProduct = result.FirstOrDefault(p => p.Name == new string('A', 20));
            Assert.NotNull(maxProduct);
            Assert.Equal(20, maxProduct.Name.Length);
            Assert.Equal(20, maxProduct.Descipition.Length);
            Assert.Equal(30, maxProduct.Url.Length);

            // Cleanup
            _dbContext.Products.Remove(product);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProducts_WithDifferentCategories_ReturnsAllProducts()
        {
            // Arrange
            var category1 = new Category { Id = 5, CategoryName = "Category 1" };
            var category2 = new Category { Id = 6, CategoryName = "Category 2" };
            await _dbContext.Categories.AddRangeAsync(category1, category2);
            await _dbContext.SaveChangesAsync();

            var product1 = new Product { CategoryId = category1.Id, Name = "Product Cat 1", Price = 100 };
            var product2 = new Product { CategoryId = category2.Id, Name = "Product Cat 2", Price = 200 };

            await _dbContext.Products.AddRangeAsync(product1, product2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, p => p.CategoryId == category1.Id);
            Assert.Contains(result, p => p.CategoryId == category2.Id);

            // Cleanup
            _dbContext.Products.RemoveRange(product1, product2);
            _dbContext.Categories.RemoveRange(category1, category2);
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region GetProductById Tests - Happy & Unhappy Paths

        [Fact]
        public async Task GetProductById_ValidId_ReturnsCorrectProduct()
        {
            // Arrange
            var category = new Category { Id = 7, CategoryName = "Valid Test Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product = new Product
            {
                CategoryId = category.Id,
                Name = "Test Painting",
                Price = 299,
                Descipition = "Beautiful test art",
                Url = "http://example.com/test"
            };
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GerProductById(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(category.Id, result.CategoryId);
            Assert.Equal("Test Painting", result.Name);
            Assert.Equal(299, result.Price);
            Assert.Equal("Beautiful test art", result.Descipition);
            Assert.Equal("http://example.com/test", result.Url);

            // Cleanup
            _dbContext.Products.Remove(product);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductById_InvalidId_ReturnsNull()
        {
            // Act
            var result = await _productRepository.GerProductById(99999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductById_NegativeId_ReturnsNull()
        {
            // Act
            var result = await _productRepository.GerProductById(-1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductById_ZeroId_ReturnsNull()
        {
            // Act
            var result = await _productRepository.GerProductById(0);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductById_WithNullPrice_ReturnsProduct()
        {
            // Arrange
            var category = new Category { Id = 8, CategoryName = "Null Price Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product = new Product
            {
                CategoryId = category.Id,
                Name = "Free Product",
                Price = null,
                Descipition = "No cost item",
                Url = "http://example.com/free"
            };
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GerProductById(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Null(result.Price);
            Assert.Equal("Free Product", result.Name);

            // Cleanup
            _dbContext.Products.Remove(product);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductById_ExistingIdAfterDeletion_ReturnsNull()
        {
            // Arrange
            var category = new Category { Id = 9, CategoryName = "Deletion Test" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product = new Product
            {
                CategoryId = category.Id,
                Name = "Temporary Product",
                Price = 150,
                Descipition = "Will be deleted"
            };
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            var productId = product.Id;

            // Delete the product
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GerProductById(productId);

            // Assert
            Assert.Null(result);

            // Cleanup
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductById_VeryLargeId_ReturnsNull()
        {
            // Act
            var result = await _productRepository.GerProductById(int.MaxValue);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductById_WithMinimalData_ReturnsProduct()
        {
            // Arrange
            var category = new Category { Id = 10, CategoryName = "Minimal Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product = new Product
            {
                CategoryId = category.Id,
                Name = "Min Product"
                // Price, Descipition, Url intentionally null/not set
            };
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GerProductById(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Min Product", result.Name);
            Assert.Equal(category.Id, result.CategoryId);
            Assert.Null(result.Price);

            // Cleanup
            _dbContext.Products.Remove(product);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductById_WithMaxStringLengths_ReturnsProduct()
        {
            // Arrange
            var category = new Category { Id = 11, CategoryName = "Max String Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var maxName = new string('N', 20);        // Max 20 chars
            var maxDesc = new string('D', 20);        // Max 20 chars
            var maxUrl = new string('U', 30);         // Max 30 chars

            var product = new Product
            {
                CategoryId = category.Id,
                Name = maxName,
                Price = 999,
                Descipition = maxDesc,
                Url = maxUrl
            };
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GerProductById(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(maxName, result.Name);
            Assert.Equal(maxDesc, result.Descipition);
            Assert.Equal(maxUrl, result.Url);
            Assert.Equal(20, result.Name.Length);
            Assert.Equal(20, result.Descipition.Length);
            Assert.Equal(30, result.Url.Length);

            // Cleanup
            _dbContext.Products.Remove(product);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductById_MultipleProductsExist_ReturnsCorrectOne()
        {
            // Arrange
            var category = new Category { Id = 12, CategoryName = "Multiple Products Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product1 = new Product { CategoryId = category.Id, Name = "Product 1", Price = 100, Descipition = "First" };
            var product2 = new Product { CategoryId = category.Id, Name = "Product 2", Price = 200, Descipition = "Second" };
            var product3 = new Product { CategoryId = category.Id, Name = "Product 3", Price = 300, Descipition = "Third" };

            await _dbContext.Products.AddRangeAsync(product1, product2, product3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GerProductById(product2.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product2.Id, result.Id);
            Assert.Equal("Product 2", result.Name);
            Assert.Equal(200, result.Price);
            Assert.Equal("Second", result.Descipition);
            Assert.Equal(category.Id, result.CategoryId);

            // Cleanup
            _dbContext.Products.RemoveRange(product1, product2, product3);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductById_WithIntegerPriceBoundaries_ReturnsProduct()
        {
            // Arrange
            var category = new Category { Id = 13, CategoryName = "Price Boundary Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var minPriceProduct = new Product
            {
                CategoryId = category.Id,
                Name = "Min Price Product",
                Price = 0,
                Descipition = "Zero price"
            };
            var maxPriceProduct = new Product
            {
                CategoryId = category.Id,
                Name = "Max Price Product",
                Price = int.MaxValue,
                Descipition = "Maximum price"
            };

            await _dbContext.Products.AddRangeAsync(minPriceProduct, maxPriceProduct);
            await _dbContext.SaveChangesAsync();

            // Act
            var minResult = await _productRepository.GerProductById(minPriceProduct.Id);
            var maxResult = await _productRepository.GerProductById(maxPriceProduct.Id);

            // Assert
            Assert.NotNull(minResult);
            Assert.NotNull(maxResult);
            Assert.Equal(0, minResult.Price);
            Assert.Equal(int.MaxValue, maxResult.Price);

            // Cleanup
            _dbContext.Products.RemoveRange(minPriceProduct, maxPriceProduct);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region GetProductsFiltered Tests - Happy & Unhappy Paths
        [Fact]
        public async Task GetProductsFiltered_WithCategoryAndPriceRange_ReturnsExpected()
        {
            // Arrange
            var p1 = new Product { Name = "Painting A", Price = 150, CategoryId = 1 };
            var p2 = new Product { Name = "Painting B", Price = 250, CategoryId = 1 };
            var p3 = new Product { Name = "Painting C", Price = 350, CategoryId = 2 };

            _dbContext.Products.AddRange(p1, p2, p3);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetProductsFiltered(1, 100, 200);

            // Assert
            Assert.Single(result);
            Assert.Equal("Painting A", result.First().Name);

            // Cleanup
            _dbContext.Products.RemoveRange(p1, p2, p3);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductsFiltered_WithNoFilter_ReturnsAll()
        {
            // Arrange
            var p1 = new Product { Name = "P1", Price = 100, CategoryId = 1 };
            var p2 = new Product { Name = "P2", Price = 200, CategoryId = 2 };

            _dbContext.Products.AddRange(p1, p2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _productRepository.GetProductsFiltered(null, null, null);

            // Assert
            Assert.True(result.Count >= 2);

            // Cleanup
            _dbContext.Products.RemoveRange(p1, p2);
            await _dbContext.SaveChangesAsync();
        }
        #endregion

        #region Edge Cases and Performance Tests

        [Fact]
        public async Task GetAllProducts_CalledMultipleTimes_ReturnsConsistentResults()
        {
            // Arrange
            var category = new Category { Id = 14, CategoryName = "Consistency Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product = new Product { CategoryId = category.Id, Name = "Consistent Test", Price = 100 };
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result1 = await _productRepository.GetAllProducts();
            var result2 = await _productRepository.GetAllProducts();
            var result3 = await _productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.Equal(result1.Count, result2.Count);
            Assert.Equal(result2.Count, result3.Count);

            // Cleanup
            _dbContext.Products.Remove(product);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetProductById_CalledMultipleTimes_ReturnsConsistentResults()
        {
            // Arrange
            var category = new Category { Id = 15, CategoryName = "Consistent ID Category" };
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            var product = new Product { CategoryId = category.Id, Name = "Consistent Product", Price = 150 };
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var result1 = await _productRepository.GerProductById(product.Id);
            var result2 = await _productRepository.GerProductById(product.Id);
            var result3 = await _productRepository.GerProductById(product.Id);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.Equal(result1.Id, result2.Id);
            Assert.Equal(result2.Id, result3.Id);
            Assert.Equal(result1.Name, result2.Name);
            Assert.Equal(result2.Name, result3.Name);

            // Cleanup
            _dbContext.Products.Remove(product);
            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
