using GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:7253");
// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

var app = builder.Build();
app.UseRouting();

// must be added after UseRouting and before UseEndpoints 
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
// Configure the HTTP request pipeline.
//app.MapGrpcService<MessageService>();
app.UseCors();
app.UseEndpoints(endpoints =>
{
    // map to and register the gRPC service
    endpoints.MapGrpcService<MessageService>().EnableGrpcWeb().RequireCors("AllowAll"); ;
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    //endpoints.MapFallbackToFile("index.html");
});
//app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
