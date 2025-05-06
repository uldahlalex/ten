using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation;

namespace api.Extensions;

public static class GenerateTypescriptClient
{
    public static async Task GenerateTypeScriptClient(this WebApplication app, string path)
    {
        var document = await app.Services.GetRequiredService<IOpenApiDocumentGenerator>()
            .GenerateAsync("v1");
        var settings = new TypeScriptClientGeneratorSettings
        {
            Template = TypeScriptTemplate.Fetch,
            TypeScriptGeneratorSettings =
            {
                TypeStyle = TypeScriptTypeStyle.Class,
                DateTimeType = TypeScriptDateTimeType.Date,
                NullValue = TypeScriptNullValue.Undefined,
                TypeScriptVersion = 5.2m,
                GenerateCloneMethod = false,
                MarkOptionalProperties = true
            }
        };

        var generator = new TypeScriptClientGenerator(document, settings);
        var code = generator.GenerateFile();

        var outputPath = Path.Combine(Directory.GetCurrentDirectory() + path);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        await File.WriteAllTextAsync(outputPath, code);
        app.Services.GetRequiredService<ILogger<Program>>()
            .LogInformation("TypeScript client generated at: " + outputPath);
    }
}