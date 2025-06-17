using Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class ProductRepositoryUnitTesting
    {
        [Fact]
        public async Task GetAllProducts_ReturnsListOfProducts()
        {
            // Arrange
            var product1 = new Product { Id = 1, CategoryId = 1, Name = "Mountain View", Descipition = "Beautiful mountain", Price = 150, Url = "mountain-view.jpg" };
            var product2 = new Product { Id = 2, CategoryId = 2, Name = "Portrait", Descipition = "Classic portrait", Price = 200, Url = "portrait.jpg" };
            var products = new List<Product> { product1, product2 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act
            var result = await productRepository.GetAllProducts();

            // Assert
            Assert.Equal(products, result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Mountain View", result[0].Name);
            Assert.Equal("Portrait", result[1].Name);
        }

        [Fact]
        public async Task GetAllProducts_WhenNoProductsExist_ReturnsEmptyList()
        {
            // Arrange
            var products = new List<Product>();

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act
            var result = await productRepository.GetAllProducts();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllProducts_WithNullableProperties_ReturnsProductsWithNullValues()
        {
            // Arrange
            var product1 = new Product { Id = 1, CategoryId = 1, Name = "Test Product", Descipition = "Test Desc", Price = null, Url = "test.jpg" };
            var product2 = new Product { Id = 2, CategoryId = 2, Name = "Another Product", Descipition = "Another Desc", Price = 100, Url = "another.jpg" };
            var products = new List<Product> { product1, product2 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act
            var result = await productRepository.GetAllProducts();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Null(result[0].Price);
            Assert.Equal(100, result[1].Price);
        }

        [Fact]
        public async Task GetAllProducts_WhenDatabaseThrowsException_ThrowsException()
        {
            // Arrange
            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Throws(new InvalidOperationException("Database connection failed"));

            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => productRepository.GetAllProducts());
        }

        [Fact]
        public async Task GerProductById_ValidId_ReturnsProduct()
        {
            // Arrange
            var targetId = 1;
            var product = new Product { Id = 1, CategoryId = 2, Name = "Sunset", Descipition = "Beautiful sunset", Price = 175, Url = "sunset.jpg" };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.Setup(x => x.FindAsync(targetId)).ReturnsAsync(product);
            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act
            var result = await productRepository.GerProductById(targetId);

            // Assert
            Assert.Equal(product, result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Sunset", result.Name);
            Assert.Equal("Beautiful sunset", result.Descipition);
            Assert.Equal(175, result.Price);
        }

        [Fact]
        public async Task GerProductById_NonExistentId_ReturnsNull()
        {
            // Arrange
            var targetId = 999;

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.Setup(x => x.FindAsync(targetId)).ReturnsAsync((Product)null);
            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act
            var result = await productRepository.GerProductById(targetId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GerProductById_WithNullPrice_ReturnsProductWithNullPrice()
        {
            // Arrange
            var targetId = 3;
            var product = new Product { Id = 3, CategoryId = 1, Name = "Free Art", Descipition = "Free artwork", Price = null, Url = "free-art.jpg" };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.Setup(x => x.FindAsync(targetId)).ReturnsAsync(product);
            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act
            var result = await productRepository.GerProductById(targetId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Null(result.Price);
            Assert.Equal("Free Art", result.Name);
        }

        [Fact]
        public async Task GerProductById_WhenDatabaseThrowsException_ThrowsException()
        {
            // Arrange
            var targetId = 1;
            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.Setup(x => x.FindAsync(targetId)).ThrowsAsync(new InvalidOperationException("Database query failed"));
            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => productRepository.GerProductById(targetId));
        }

        [Fact]
        public async Task GetAllProducts_CallsToListAsyncOnDbSet()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, CategoryId = 1, Name = "Test Product", Descipition = "Test Description", Price = 100, Url = "test.jpg" }
        };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act
            await productRepository.GetAllProducts();

            // Assert
            mockContext.Verify(x => x.Products, Times.Once);
        }

        [Fact]
        public async Task GerProductById_CallsFindAsyncWithCorrectId()
        {
            // Arrange
            var targetId = 5;
            var product = new Product { Id = 5, CategoryId = 1, Name = "Test", Descipition = "Test Desc", Price = 50, Url = "test.jpg" };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.Setup(x => x.FindAsync(targetId)).ReturnsAsync(product);
            mockContext.Setup(x => x.Products).Returns(mockDbSet.Object);

            var productRepository = new ProductRepository(mockContext.Object);

            // Act
            await productRepository.GerProductById(targetId);

            // Assert
            mockDbSet.Verify(x => x.FindAsync(targetId), Times.Once);
        }
        [Fact]
        public async Task GetProductsFiltered_WithCategoryFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product A", Price = 100, CategoryId = 1 },
            new Product { Id = 2, Name = "Product B", Price = 200, CategoryId = 2 },
        };

            var mockContext = new Mock<PaintingsShopContext>();
            mockContext.Setup(x => x.Products).ReturnsDbSet(products);

            var repo = new ProductRepository(mockContext.Object);

            // Act
            var result = await repo.GetProductsFiltered(1, null, null);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().CategoryId);
        }

        [Fact]
        public async Task GetProductsFiltered_WithPriceRange_ReturnsCorrectProducts()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Cheap", Price = 50, CategoryId = 1 },
            new Product { Id = 2, Name = "Mid", Price = 150, CategoryId = 1 },
            new Product { Id = 3, Name = "Expensive", Price = 300, CategoryId = 1 },
        };

            var mockContext = new Mock<PaintingsShopContext>();
            mockContext.Setup(x => x.Products).ReturnsDbSet(products);

            var repo = new ProductRepository(mockContext.Object);

            // Act
            var result = await repo.GetProductsFiltered(null, 100, 200);

            // Assert
            Assert.Single(result);
            Assert.Equal("Mid", result.First().Name);
        }

        [Fact]
        public async Task GetProductsFiltered_WithNoMatchingFilters_ReturnsEmpty()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product A", Price = 100, CategoryId = 1 },
            new Product { Id = 2, Name = "Product B", Price = 200, CategoryId = 2 },
        };

            var mockContext = new Mock<PaintingsShopContext>();
            mockContext.Setup(x => x.Products).ReturnsDbSet(products);

            var repo = new ProductRepository(mockContext.Object);

            // Act
            var result = await repo.GetProductsFiltered(3, 500, 800);

            // Assert
            Assert.Empty(result);
        }
    }
}
