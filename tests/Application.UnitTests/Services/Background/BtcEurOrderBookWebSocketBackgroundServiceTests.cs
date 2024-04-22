using System.Net.WebSockets;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using OrderBookTestTask.Application.Abstract.Services.Background;
using OrderBookTestTask.Application.Hubs;
using OrderBookTestTask.Application.Interfaces;
using OrderBookTestTask.Application.Services.Background;

namespace Application.UnitTests;

public class BtcEurOrderBookWebSocketBackgroundServiceTests
{
    private BtcEurOrderBookWebSocketBackgroundService btcEurOrderBookWebSocketBackgroundService;
    private Mock<IHubContext<OrderBookHub>> hubContextMock;
    private Mock<IOrderBookService> orderBookServiceMock;
    private Mock<IOrderBookWebSockerService> orderBookWebSockerServiceMock;
    private Mock<ILogger<OrderBookWebSocketBackgroundService>> loggerMock;

    
    [SetUp]
    public void Setup()
    {
        hubContextMock = new Mock<IHubContext<OrderBookHub>>();
        orderBookServiceMock = new Mock<IOrderBookService>();
        orderBookWebSockerServiceMock = new Mock<IOrderBookWebSockerService>();
        loggerMock = new Mock<ILogger<OrderBookWebSocketBackgroundService>>();
        btcEurOrderBookWebSocketBackgroundService = new BtcEurOrderBookWebSocketBackgroundService(hubContextMock.Object, 
            orderBookServiceMock.Object, orderBookWebSockerServiceMock.Object, loggerMock.Object);
    }

    [Test]
    public async Task BtcEurOrderBookWebSocketBackgroundService_StratAsync_ShouldCallSubscribe()
    {
        // Arrange
        orderBookWebSockerServiceMock.Setup(x => x.Subscribe(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        orderBookWebSockerServiceMock.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((new WebSocketReceiveResult(0, WebSocketMessageType.Text, true), new byte[1024])))
            .Returns(Task.FromResult((new WebSocketReceiveResult(0, WebSocketMessageType.Close, true), new byte[1024])));

        // Act
        await btcEurOrderBookWebSocketBackgroundService.StartAsync(CancellationToken.None);

        // Assert
        orderBookWebSockerServiceMock.Verify(x => x.Subscribe(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Test]
    public async Task BtcEurOrderBookWebSocketBackgroundService_StopAsync_ShouldCallStopWebSocket()
    {
        // Arrange
        orderBookWebSockerServiceMock.Setup(x => x.StopWebSocket()).Returns(Task.CompletedTask);

        // Act
        await btcEurOrderBookWebSocketBackgroundService.StopAsync(CancellationToken.None);

        // Assert
        orderBookWebSockerServiceMock.Verify(x => x.StopWebSocket(), Times.Once);
    }


    [TearDown]
    public void TearDown()
    {
        btcEurOrderBookWebSocketBackgroundService.Dispose();
    }
}