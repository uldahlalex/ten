using ten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument();

var app = builder.Build();

app.UseOpenApi();
app.MapControllers();
await app.GenerateTypeScriptClient("/generated-client.ts");


app.Run();