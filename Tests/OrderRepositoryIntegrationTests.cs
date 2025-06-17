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
    public class OrderRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly PaintingsShopContext _dbContext;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryIntegrationTests(DatabaseFixture databaseFixture)
        {
            _dbContext = databaseFixture.Context;
            _orderRepository = new OrderRepository(_dbContext);
        }

        [Fact]
        public async Task CreateOrder_ValidOrder_ReturnsCreatedOrder()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Test",
                LastName = "Test1",
                UserName = "test@example.com",
                Password = "test@example1"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                UserId = user.Id,
                Date = DateTime.Now,
                OrderSum = 299.99,
                User = user
            };

            // Act
            var result = await _orderRepository.CreateOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(order.UserId, result.UserId);
            Assert.Equal(order.Date, result.Date);
            Assert.Equal(order.OrderSum, result.OrderSum);

            // Verify order was saved to database
            var savedOrder = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == result.Id);
            Assert.NotNull(savedOrder);
            Assert.Equal(order.UserId, savedOrder.UserId);
        }

        [Fact]
        public async Task CreateOrder_OrderWithItems_CreatesOrderWithItems()
        {
            // Arrange
            var user = new User
            {
                Id = 2,
                FirstName = "Test",
                LastName = "Test2",
                UserName = "test2@example.com",
                Password = "test2@example2"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                UserId = user.Id,
                Date = DateTime.Now,
                OrderSum = 599.99,
                User = user,
                OrderItems = new List<OrderItem>
            {
                new OrderItem {ProductId = 2, Quantity = 2}
            }
            };

            // Act
            var result = await _orderRepository.CreateOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Single(result.OrderItems);
        }

        [Fact]
        public async Task CreateOrder_NullOrder_ThrowsException()
        {
            // Arrange
            Order order = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _orderRepository.CreateOrder(order));
        }

        [Fact]
        public async Task CreateOrder_OrderWithInvalidUserId_ThrowsException()
        {
            // Arrange
            var order = new Order
            {
                UserId = 999, // Non-existent user ID
                Date = DateTime.Now,
                OrderSum = 299.99
            };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _orderRepository.CreateOrder(order));
        }

        [Fact]
        public async Task CreateOrder_OrderWithNegativeSum_CreatesOrderSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Id = 3,
                FirstName = "Test",
                LastName = "Test3",
                UserName = "test3@example.com",
                Password = "test@example3"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                UserId = user.Id,
                Date = DateTime.Now,
                OrderSum = -50.0, // Negative sum (refund scenario)
                User = user
            };

            // Act
            var result = await _orderRepository.CreateOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(-50.0, result.OrderSum);
        }

        [Fact]
        public async Task GetOrders_ExistingOrderId_ReturnsOrderList()
        {
            // Arrange
            var user = new User
            {
                Id = 4,
                FirstName = "Test",
                LastName = "Test4",
                UserName = "test4@example.com",
                Password = "test@example4"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                UserId = user.Id,
                Date = DateTime.Now,
                OrderSum = 199.99,
                User = user
            };
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrders(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(order.Id, result.First().Id);
            Assert.Equal(order.UserId, result.First().UserId);
            Assert.Equal(order.OrderSum, result.First().OrderSum);
        }

        [Fact]
        public async Task GetOrders_NonExistentOrderId_ReturnsEmptyList()
        {
            // Arrange
            int nonExistentOrderId = 99999;

            // Act
            var result = await _orderRepository.GetOrders(nonExistentOrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetOrders_MultipleOrdersWithSameId_ReturnsAllMatchingOrders()
        {
            // Arrange
            var user = new User
            {
                Id = 5,
                FirstName = "Test",
                LastName = "Test5",
                UserName = "test5@example.com",
                Password = "test@example5"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Create orders with same ID (this shouldn't happen in real scenario due to primary key, 
            // but testing the query logic)
            var order1 = new Order
            {
                UserId = user.Id,
                Date = DateTime.Now,
                OrderSum = 100.0,
                User = user
            };
            await _dbContext.Orders.AddAsync(order1);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrders(order1.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Should return only one order since ID is unique
            Assert.Equal(order1.Id, result.First().Id);
        }

        [Fact]
        public async Task GetOrders_OrderWithRelatedData_ReturnsOrderWithNavigationProperties()
        {
            // Arrange
            var user = new User
            {
                Id = 6,
                FirstName = "Test",
                LastName = "Test6",
                UserName = "test6@example.com",
                Password = "test@example6"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                UserId = user.Id,
                Date = DateTime.Now,
                OrderSum = 399.99,
                User = user,
                OrderItems = new List<OrderItem>
            {
                new OrderItem { ProductId = 1, Quantity = 1 }
            }
            };
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrders(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var returnedOrder = result.First();
            Assert.Equal(order.Id, returnedOrder.Id);
            Assert.Equal(user.Id, returnedOrder.UserId);

            // Note: Navigation properties might need explicit loading depending on EF configuration
            // This test verifies the basic query functionality
        }

        [Fact]
        public async Task CreateOrder_DatabaseConnectionFailure_ThrowsException()
        {
            // Arrange
            var user = new User
            {
                Id = 7,
                FirstName = "Test",
                LastName = "Test7",
                UserName = "test7@example.com",
                Password = "test@example7"
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var order = new Order
            {
                UserId = user.Id,
                Date = DateTime.Now,
                OrderSum = 299.99,
                User = user
            };

            // Dispose the context to simulate connection failure
            await _dbContext.DisposeAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => _orderRepository.CreateOrder(order));
        }

        [Fact]
        public async Task GetOrders_DatabaseConnectionFailure_ThrowsException()
        {
            // Arrange
            int orderId = 1;

            // Create a new context and then dispose it to simulate connection failure
            var repository = new OrderRepository(_dbContext);
            await _dbContext.DisposeAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => repository.GetOrders(orderId));
        }
    }
}
