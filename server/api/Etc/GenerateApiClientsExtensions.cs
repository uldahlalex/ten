using System.Reflection;
using System.Security.Cryptography;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation;
using Microsoft.OpenApi.Writers;
using Microsoft.OpenApi;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;

namespace api.Etc;

public static class GenerateApiClientsExtensions
{
    public static async Task GenerateApiClientsFromOpenApi(this WebApplication app, string path)
    {
        // Step 1: Generate OpenAPI document with full documentation
        var document = await app.Services.GetRequiredService<IOpenApiDocumentGenerator>()
            .GenerateAsync("v1");

        // Step 2: Serialize the document to JSON to verify it contains documentation
        var openApiJson = document.ToJson();
        
        // Optional: Save the OpenAPI JSON to verify it has documentation
        var openApiPath = Path.Combine(Directory.GetCurrentDirectory(), "openapi-with-docs.json");
        await File.WriteAllTextAsync(openApiPath, openApiJson);
        
        // Step 3: Parse the document back from JSON to ensure we're only using what's in the OpenAPI spec
        var documentFromJson = await NSwag.OpenApiDocument.FromJsonAsync(openApiJson);

        // Step 4: Generate TypeScript client from the parsed OpenAPI document
        var settings = new TypeScriptClientGeneratorSettings
        {
            Template = TypeScriptTemplate.Fetch,
             // = true,  // Enable JSDoc generation
            TypeScriptGeneratorSettings =
            {
                TypeStyle = TypeScriptTypeStyle.Interface,
                DateTimeType = TypeScriptDateTimeType.String,
                NullValue = TypeScriptNullValue.Undefined,
                TypeScriptVersion = 5.2m,
                GenerateCloneMethod = false,
                MarkOptionalProperties = true,
                GenerateConstructorInterface = true,
                ConvertConstructorInterfaceData = true
            }
        };

        // Step 5: Generate TypeScript client from the parsed OpenAPI document
        var generator = new TypeScriptClientGenerator(documentFromJson, settings);
        var code = generator.GenerateFile();

        var outputPath = Path.Combine(Directory.GetCurrentDirectory() + path);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

        await File.WriteAllTextAsync(outputPath, code);
        
        // Step 6: Generate C# client for testing (single unified client)
        var csSettings = new CSharpClientGeneratorSettings()
        {
            ClassName = "ApiClient",
            ClientClassAccessModifier = "public",
            GenerateClientClasses = true,
            GenerateClientInterfaces = true,
     
            UseHttpClientCreationMethod = false,
            HttpClientType = "System.Net.Http.HttpClient",
            GenerateOptionalParameters = true,
            UseBaseUrl = true,
            GenerateDtoTypes = false,
            WrapDtoExceptions = true,
            ClientBaseClass = null,
            ConfigurationClass = null,
            SerializeTypeInformation = false,
            // Key settings to prevent duplication
            OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator(),
            CSharpGeneratorSettings =
            {
                Namespace = "Generated",
                GenerateDataAnnotations = false,
                GenerateOptionalPropertiesAsNullable = true,
                GenerateNullableReferenceTypes = true,
                ClassStyle = NJsonSchema.CodeGeneration.CSharp.CSharpClassStyle.Poco,
                ExcludedTypeNames = new string[0],
                // Don't generate types since we'll use the API project's DTOs
                GenerateDefaultValues = false
            }
        };

        var csGenerator = new CSharpClientGenerator(documentFromJson, csSettings);
        var csCode = csGenerator.GenerateFile();

        // Add using statements for API project DTOs
        var usingStatements = @"using api.Models.Dtos.Requests;
using api.Models.Dtos.Responses;
using api.Models.Dtos;
using api.Controllers;

";
        
        // Insert using statements after the #nullable enable line
        var modifiedCode = csCode.Replace("#nullable enable", "#nullable enable\n" + usingStatements);

        var csOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "../tests/GeneratedApiClient.cs");
        Directory.CreateDirectory(Path.GetDirectoryName(csOutputPath)!);

        await File.WriteAllTextAsync(csOutputPath, modifiedCode);
        
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("OpenAPI JSON with documentation saved at: " + openApiPath);
        logger.LogInformation("TypeScript client generated at: " + outputPath);
        logger.LogInformation("C# client generated at: " + csOutputPath);
    }
}

public static class SwaggerConfigurationExtensions
{
    public static IServiceCollection AddSwaggerWithXmlDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Include XML documentation
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.EnableAnnotations();
            options.UseInlineDefinitionsForEnums();
            
            //todo add problemdetails manually

        });

        return services;
    }
    
 
    
}