using OrderBookTestTask.Application.Interfaces;
using OrderBookTestTask.Application.Services;
using OrderBookTestTask.Application.Services.Background;
using OrderBookTestTask.Application.Hubs;
using OrderBookTestTask.Application.Constants.SignalR;
using OrderBookTestTask.Data.Options;
using UI.Components;
using OrderBookTestTask.Data.Interfaces;
using System.Globalization;
using OrderBookTestTask.Application.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();
builder.Services.AddTransient<IOrderBookService, OrderBookService>();
builder.Services.AddTransient<IOrderBookRepository, OrderBookRepository>();
builder.Services.AddTransient<IOrderBookWebSocketService, OrderBookWebSocketService>();
builder.Services.AddHostedService<BtcEurOrderBookWebSocketBackgroundService>();
builder.Services.AddHostedService<BtcUsdOrderBookWebSocketBackgroundService>();

builder.Services.AddHttpClient("OrderBook", client => client.BaseAddress = new Uri(builder.Configuration["ApiUrl"]))
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        // Ignore SSL certificate validation
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        return handler;
    });

builder.Services.Configure<OrderBookSnapshotsDatabaseOptions>(
    builder.Configuration.GetSection("OrderBookDatabase"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<OrderBookHub>(HubUrls.OrderBookHub);

app.MapGet("/order-book", async (string tradingPair, string dateTime, IOrderBookService orderBookService) =>
{
    return await orderBookService.GetOrderBookAsync(DateTime.ParseExact(dateTime, Date.DateTimeFormat, CultureInfo.InvariantCulture), tradingPair);
});

app.Run();
