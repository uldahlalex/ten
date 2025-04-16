using Application;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(conf =>
{
    
});
var appOptions = builder.Services.AddAppOptions(builder.Configuration);
builder.Services.AddDbContext<MyDbContext>(ctx =>
{
    ctx.UseNpgsql(appOptions.DbConnectionString);
});

var app = builder.Build();

app.UseOpenApi(conf =>
{
    conf.Path = "openapi/v1.json";
});
app.MapControllers();
await app.GenerateTypeScriptClient("/generated-client.ts");
app.MapScalarApiReference();

app.Run();