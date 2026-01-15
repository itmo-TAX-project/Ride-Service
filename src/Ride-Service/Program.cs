using Application.Extensions;
using Infrastructure.Database.Options;
using Infrastructure.Extensions;
using Itmo.Dev.Platform.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Extensions;
using Presentation.Grpc.Controllers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Postgres"));

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Platform:ServiceName"] = "Ride-Service",
    ["Platform:Observability:Tracing:IsEnabled"] = "false",
});

builder.Services.AddPlatform();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddPresentation(builder.Configuration);

WebApplication app = builder.Build();

app.MapGrpcService<RideController>();
app.MapGrpcReflectionService();

app.Run();