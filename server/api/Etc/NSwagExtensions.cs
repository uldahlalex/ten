using NSwag.Generation;

namespace api.Etc;

public static class NSwagExtensions
{

    public static void AddTypeToSwagger<T>(this OpenApiDocumentGeneratorSettings settings)
    {
        settings.DocumentProcessors.Add(new TypeMapDocumentProcessor<T>());
    }

}