using Frontend.BlazorWebassembly;
using Frontend.BlazorWebassembly.Services;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// Persistent data
builder.Services.AddSingleton<AppStateService>();
builder.Services
    .AddGrpcClient<ChatbotService.ChatbotServiceClient>(options =>
    {
        options.Address = new Uri("http://localhost:7253");
    })
    .ConfigurePrimaryHttpMessageHandler(
        () => new GrpcWebHandler(new HttpClientHandler()));

var channel = GrpcChannel.ForAddress("http://localhost:7253", new GrpcChannelOptions
{
    HttpHandler = new GrpcWebHandler(new HttpClientHandler())
});


await builder.Build().RunAsync();
