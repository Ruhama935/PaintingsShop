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
    public class OrderRepositoryUnitTesting
    {
        [Fact]
        public async Task CreateOrder_ValidOrder_ReturnsCreatedOrder()
        {
            // Arrange
            var order = new Order { Id = 1, UserId = 1, Date = DateTime.Now.Date, OrderSum = 100.50 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.CreateOrder(order);

            // Assert
            Assert.Equal(order, result);
            mockDbSet.Verify(x => x.AddAsync(order, default), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_WithComplexOrder_ReturnsCreatedOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = 2,
                UserId = 5,
                Date = new DateTime(2025, 6, 16),
                OrderSum = 299.99
            };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.CreateOrder(order);

            // Assert
            Assert.Equal(order, result);
            Assert.Equal(2, result.Id);
            Assert.Equal(5, result.UserId);
            Assert.Equal(299.99, result.OrderSum);
            Assert.Equal(new DateTime(2025, 6, 16), result.Date);
        }

        [Fact]
        public async Task CreateOrder_WithNullableProperties_ReturnsCreatedOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = 3,
                UserId = 2,
                Date = null,
                OrderSum = null
            };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.CreateOrder(order);

            // Assert
            Assert.Equal(order, result);
            Assert.Equal(3, result.Id);
            Assert.Equal(2, result.UserId);
            Assert.Null(result.Date);
            Assert.Null(result.OrderSum);
        }

        [Fact]
        public async Task CreateOrder_WhenSaveChangesFails_ThrowsException()
        {
            // Arrange
            var order = new Order { Id = 1, UserId = 1, Date = DateTime.Now.Date, OrderSum = 100.50 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Database save failed"));

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => orderRepository.CreateOrder(order));
        }

        [Fact]
        public async Task GetOrders_ValidId_ReturnsMatchingOrders()
        {
            // Arrange
            var targetId = 1;
            var order1 = new Order { Id = 1, UserId = 1, Date = DateTime.Now.Date, OrderSum = 100.50 };
            var order2 = new Order { Id = 2, UserId = 2, Date = DateTime.Now.Date, OrderSum = 200.75 };
            var order3 = new Order { Id = 1, UserId = 3, Date = DateTime.Now.Date, OrderSum = 150.25 };
            var orders = new List<Order> { order1, order2, order3 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.GetOrders(targetId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, order => Assert.Equal(targetId, order.Id));
            Assert.Contains(result, o => o.UserId == 1);
            Assert.Contains(result, o => o.UserId == 3);
        }

        [Fact]
        public async Task GetOrders_NonExistentId_ReturnsEmptyList()
        {
            // Arrange
            var targetId = 999;
            var order1 = new Order { Id = 1, UserId = 1, Date = DateTime.Now.Date, OrderSum = 100.50 };
            var order2 = new Order { Id = 2, UserId = 2, Date = DateTime.Now.Date, OrderSum = 200.75 };
            var orders = new List<Order> { order1, order2 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.GetOrders(targetId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetOrders_SingleMatchingOrder_ReturnsSingleOrder()
        {
            // Arrange
            var targetId = 5;
            var order1 = new Order { Id = 1, UserId = 1, Date = DateTime.Now.Date, OrderSum = 100.50 };
            var order2 = new Order { Id = 5, UserId = 2, Date = DateTime.Now.Date, OrderSum = 200.75 };
            var order3 = new Order { Id = 3, UserId = 3, Date = DateTime.Now.Date, OrderSum = 150.25 };
            var orders = new List<Order> { order1, order2, order3 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.GetOrders(targetId);

            // Assert
            Assert.Single(result);
            Assert.Equal(targetId, result.First().Id);
            Assert.Equal(2, result.First().UserId);
            Assert.Equal(200.75, result.First().OrderSum);
        }

        [Fact]
        public async Task GetOrders_WithNullValues_ReturnsOrdersWithNullProperties()
        {
            // Arrange
            var targetId = 7;
            var order1 = new Order { Id = 7, UserId = 1, Date = null, OrderSum = null };
            var order2 = new Order { Id = 8, UserId = 2, Date = DateTime.Now.Date, OrderSum = 200.75 };
            var orders = new List<Order> { order1, order2 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            var result = await orderRepository.GetOrders(targetId);

            // Assert
            Assert.Single(result);
            Assert.Equal(targetId, result.First().Id);
            Assert.Null(result.First().Date);
            Assert.Null(result.First().OrderSum);
        }

        [Fact]
        public async Task GetOrders_WhenDatabaseThrowsException_ThrowsException()
        {
            // Arrange
            var targetId = 1;
            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Throws(new InvalidOperationException("Database query failed"));

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => orderRepository.GetOrders(targetId));
        }

        [Fact]
        public async Task CreateOrder_CallsAddAsyncAndSaveChangesAsync()
        {
            // Arrange
            var order = new Order { Id = 1, UserId = 1, Date = DateTime.Now.Date, OrderSum = 100.50 };

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);
            mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            await orderRepository.CreateOrder(order);

            // Assert
            mockDbSet.Verify(x => x.AddAsync(order, default), Times.Once);
            mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrders_CallsCorrectDbSetWithWhereClause()
        {
            // Arrange
            var targetId = 1;
            var orders = new List<Order>();

            var mockContext = new Mock<PaintingsShopContext>();
            var mockDbSet = new Mock<DbSet<Order>>();

            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.AsQueryable().GetEnumerator());

            mockContext.Setup(x => x.Orders).Returns(mockDbSet.Object);

            var orderRepository = new OrderRepository(mockContext.Object);

            // Act
            await orderRepository.GetOrders(targetId);

            // Assert
            mockContext.Verify(x => x.Orders, Times.Once);
        }
    }
}
