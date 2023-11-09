using Frontend.BlazorWebassembly;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Services.AddScoped(provide =>
//{
//    var channel = GrpcChannel.ForAddress("https://localhost:7253");
//    return new ChatbotService.ChatbotServiceClient(channel);
//});

//builder.Services.AddSingleton(services =>
//{
//    var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
//    var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;
//    var channel = GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient });
//    return new ChatbotService.ChatbotServiceClient(channel);
//});
builder.Services
    .AddGrpcClient<ChatbotService.ChatbotServiceClient>(options =>
    {
        options.Address = new Uri("https://localhost:7253");
    })
    .ConfigurePrimaryHttpMessageHandler(
        () => new GrpcWebHandler(new HttpClientHandler()));

var channel = GrpcChannel.ForAddress("https://localhost:7253", new GrpcChannelOptions
{
    HttpHandler = new GrpcWebHandler(new HttpClientHandler())
});


await builder.Build().RunAsync();
