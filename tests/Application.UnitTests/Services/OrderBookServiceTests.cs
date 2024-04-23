using Microsoft.Extensions.Logging;
using Moq;
using OrderBookTestTask.Application.Dtos;
using OrderBookTestTask.Application.Services;
using OrderBookTestTask.Data.Interfaces;
using OrderBookTestTask.Data.Models;

namespace Application.UnitTests.Services;

public class OrderBookServiceTests
{
    private OrderBookService orderBookService;
    private Mock<IOrderBookRepository> orderBookRepositoryMock;
    
    [SetUp]
    public void Setup()
    {
        orderBookRepositoryMock = new Mock<IOrderBookRepository>();
        orderBookService = new OrderBookService(orderBookRepositoryMock.Object);
    }

    [Test]
    public async Task OrderBookService_GetOrderBookAsync_ShouldReturnOrderBook()
    {
        // Arrange
        var orderBook = new OrderBook();
        orderBookRepositoryMock.Setup(x => x.GetOrderBookAsync(It.IsAny<DateTime>(), It.IsAny<string>())).ReturnsAsync(orderBook);

        // Act
        var result = await orderBookService.GetOrderBookAsync(It.IsAny<DateTime>(), It.IsAny<string>());

        // Assert
        Assert.That(result, Is.EqualTo(orderBook));
    }

    [Test]
    public async Task OrderBookService_CreateOrderBookAsync_ShouldCreateOrderBook()
    {
        // Arrange
        var createOrderBookDto = new CreateOrderBookDto();

        // Act
        await orderBookService.CreateOrderBookAsync(createOrderBookDto);

        // Assert
        orderBookRepositoryMock.Verify(x => x.CreateOrderBookAsync(It.IsAny<OrderBook>()), Times.Once);
    }
}