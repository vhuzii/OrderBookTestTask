using OrderBookTestTask.Interfaces;
using OrderBookTestTask.Components;
using OrderBookTestTask.Services;
using OrderBookTestTask.Services.Background;
using OrderBookTestTask.Models;
using OrderBookTestTask.Hubs;
using OrderBookTestTask.Constants.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();
builder.Services.AddTransient<IOrderBookService, OrderBookService>();
builder.Services.AddHostedService<BtcEurOrderBookWebSocketBackgroundService>();
builder.Services.AddHostedService<BtcUsdOrderBookWebSocketBackgroundService>();

builder.Services.Configure<OrderBookSnapshotsDatabaseSettings>(
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

app.Run();
