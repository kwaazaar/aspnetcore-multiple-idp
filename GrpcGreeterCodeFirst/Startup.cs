using GrpcGreeterCodeFirst.Services;
using ProtoBuf.Grpc.Server;

namespace GrpcGreeterCodeFirst
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            services.AddCodeFirstGrpc(config => { config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal; });

            // Not sure if cors is needed
            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // Enable GrpcWeb for all Grpc services (may require explicit specification of UseRouting, etc, see docs)
            app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true }); // Must be added between UseRouting and UseEndpoints (https://docs.microsoft.com/en-us/aspnet/core/grpc/browser?view=aspnetcore-6.0#configure-grpc-web-in-aspnet-core)

            app.UseCors();

            // Configure the HTTP request pipeline.
            app.UseEndpoints(endpoints => {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        }
    }
}
