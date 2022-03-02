using GrpcGreeter.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

// Enable GrpcWeb for all Grpc services (may require explicit specification of UseRouting, etc, see docs)
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true }); // Must be added between UseRouting and UseEndpoints (https://docs.microsoft.com/en-us/aspnet/core/grpc/browser?view=aspnetcore-6.0#configure-grpc-web-in-aspnet-core)


app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<GreeterService>();
    endpoints.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
});

app.Run();
