using Microsoft.AspNetCore.Mvc;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation;
using System.Reflection;

namespace api.Etc;

public static class TypeScriptClientWatcher
{
    private static WebApplication? _app;
    private static Timer? _debounceTimer;
    private static Dictionary<string, DateTime> _fileLastModified = new();
    private static Timer? _pollingTimer;

    public static void Initialize(WebApplication app)
    {
        _app = app;
        SetupTypeScriptClientWatcher();
        
        // Add an endpoint to manually trigger client generation
        app.MapPost("/api/dev/regenerate-client", async () =>
        {
            try 
            {
                Console.WriteLine("[MANUAL] Manually triggering TypeScript client generation...");
                await GenerateTypeScriptClient();
                return Results.Ok("TypeScript client generated successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MANUAL] Error: {ex.Message}");
                return Results.Problem($"Error generating client: {ex.Message}");
            }
        });
    }

    private static void SetupTypeScriptClientWatcher()
    {
        var currentDir = Directory.GetCurrentDirectory();
        Console.WriteLine($"Current directory: {currentDir}");
        
        // Use current directory as the project root (should be server/api)
        var projectRoot = currentDir;
        Console.WriteLine($"Watching directory: {projectRoot}");
        Console.WriteLine($"Directory exists: {Directory.Exists(projectRoot)}");
        
        // List some files to verify we're in the right place
        var csFiles = Directory.GetFiles(projectRoot, "*.cs", SearchOption.TopDirectoryOnly);
        Console.WriteLine($"Found {csFiles.Length} .cs files in root directory");

        try
        {
            var watcher = new FileSystemWatcher(projectRoot)
            {
                Filter = "*.cs",
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName
            };
            
            watcher.Changed += OnFileSystemEvent;
            watcher.Created += OnFileSystemEvent;
            watcher.Renamed += (sender, e) => OnFileSystemEvent(sender, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(e.FullPath)!, Path.GetFileName(e.FullPath)));

            watcher.Error += (sender, e) =>
            {
                Console.WriteLine($"[WATCHER] Error: {e.GetException().Message}");
                Console.WriteLine($"[WATCHER] Stack trace: {e.GetException().StackTrace}");
            };

            watcher.EnableRaisingEvents = true;
            Console.WriteLine($"[WATCHER] File watcher enabled - watching {projectRoot} for *.cs files");
            
            // Test the watcher by immediately listing what it should be watching
            var allCsFiles = Directory.GetFiles(projectRoot, "*.cs", SearchOption.AllDirectories);
            Console.WriteLine($"[WATCHER] Total .cs files being watched: {allCsFiles.Length}");
            var controllerFiles = allCsFiles.Where(f => f.Contains("Controllers")).ToArray();
            Console.WriteLine($"[WATCHER] Controller files: {controllerFiles.Length}");
            foreach (var file in controllerFiles.Take(3))
            {
                Console.WriteLine($"[WATCHER] - {file}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WATCHER] Failed to set up file watcher: {ex.Message}");
            Console.WriteLine($"[WATCHER] Stack trace: {ex.StackTrace}");
        }

        // Fallback: Set up polling as backup
        SetupPollingWatcher(projectRoot);
    }

    private static void SetupPollingWatcher(string projectRoot)
    {
        Console.WriteLine("[POLLING] Setting up polling watcher as fallback...");
        
        // Initialize file modification times
        var targetFiles = Directory.GetFiles(projectRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => f.Contains("Controllers") || f.Contains("Models") || f.Contains("Dto"))
            .ToList();
            
        foreach (var file in targetFiles)
        {
            _fileLastModified[file] = File.GetLastWriteTime(file);
        }
        
        Console.WriteLine($"[POLLING] Tracking {targetFiles.Count} relevant files");
        
        // Poll every 2 seconds
        _pollingTimer = new Timer(_ =>
        {
            try
            {
                CheckForFileChanges(projectRoot);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[POLLING] Error during polling: {ex.Message}");
            }
        }, null, 2000, 2000);
    }

    private static void CheckForFileChanges(string projectRoot)
    {
        var targetFiles = Directory.GetFiles(projectRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => f.Contains("Controllers") || f.Contains("Models") || f.Contains("Dto"))
            .ToList();

        foreach (var file in targetFiles)
        {
            var lastWrite = File.GetLastWriteTime(file);
            
            if (!_fileLastModified.ContainsKey(file))
            {
                // New file
                _fileLastModified[file] = lastWrite;
                Console.WriteLine($"[POLLING] New file detected: {Path.GetFileName(file)}");
                TriggerClientRegeneration(file, "Created");
            }
            else if (_fileLastModified[file] != lastWrite)
            {
                // Modified file
                _fileLastModified[file] = lastWrite;
                Console.WriteLine($"[POLLING] Modified file detected: {Path.GetFileName(file)}");
                TriggerClientRegeneration(file, "Changed");
            }
        }
    }

    private static void TriggerClientRegeneration(string filePath, string changeType)
    {
        Console.WriteLine($"[POLLING] {changeType}: {Path.GetFileName(filePath)} - Starting debounce timer...");
        
        // Debounce the generation to avoid multiple rapid calls
        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(_ =>
        {
            Console.WriteLine("[POLLING] Debounce timer triggered - regenerating client...");
            Task.Run(async () =>
            {
                try
                {
                    await GenerateTypeScriptClient();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[POLLING] Error generating client: {ex.Message}");
                }
            });
        }, null, 2000, Timeout.Infinite);
    }

    private static void OnFileSystemEvent(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"[WATCHER] File {e.ChangeType}: {e.FullPath} at {DateTime.Now:HH:mm:ss.fff}");
    
        if (e.FullPath.Contains("Controllers") || 
            e.FullPath.Contains("Models") || 
            e.FullPath.Contains("Dto"))
        {
            Console.WriteLine($"[WATCHER] Relevant file {e.ChangeType}: {Path.GetFileName(e.FullPath)} - Starting debounce timer...");
            
            // Debounce the generation to avoid multiple rapid calls
            _debounceTimer?.Dispose();
            _debounceTimer = new Timer(_ =>
            {
                Console.WriteLine("[WATCHER] Debounce timer triggered - regenerating client...");
                Task.Run(async () =>
                {
                    try
                    {
                        await GenerateTypeScriptClient();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WATCHER] Error generating client: {ex.Message}");
                    }
                });
            }, null, 2000, Timeout.Infinite);
        }
    }


    public static async Task GenerateTypeScriptClient()
    {
        if (_app == null) return;

        try
        {
            Console.WriteLine("[GENERATION] Starting OpenAPI document generation...");
            
            // Force garbage collection to ensure old assemblies are released
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            // Add a longer delay to ensure hot reload compilation is complete
            await Task.Delay(2000);
            
            // Try to clear any cached type information
            if (typeof(Program).Assembly.GetType("System.Reflection.Assembly") != null)
            {
                Console.WriteLine("[GENERATION] Attempting to clear assembly caches...");
            }
            
            // Create a new service scope to ensure fresh services during hot reload
            using var scope = _app.Services.CreateScope();
            
            // Try to force refresh the controller discovery
            var controllerTypes = typeof(Program).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ControllerBase)) && !t.IsAbstract)
                .ToList();
            
            Console.WriteLine($"[GENERATION] Found {controllerTypes.Count} controller types at runtime:");
            foreach (var type in controllerTypes)
            {
                var methods = type.GetMethods()
                    .Where(m => m.IsPublic && m.DeclaringType == type && 
                               (m.GetCustomAttributes(typeof(HttpGetAttribute), false).Length > 0 || 
                                m.GetCustomAttributes(typeof(HttpPostAttribute), false).Length > 0))
                    .Count();
                Console.WriteLine($"[GENERATION] - {type.Name}: {methods} HTTP methods");
            }
            
            // Generate OpenAPI document with fresh scope
            var openApiGenerator = scope.ServiceProvider.GetRequiredService<IOpenApiDocumentGenerator>();
            Console.WriteLine($"[GENERATION] Using generator: {openApiGenerator.GetType().Name}");
            
            var document = await openApiGenerator.GenerateAsync("v1");
            Console.WriteLine($"[GENERATION] Generated document with {document.Paths.Count} paths");
            
            // Save OpenAPI JSON file
            var openApiJson = document.ToJson();
            var openApiPath = Path.Combine(Directory.GetCurrentDirectory(), "openapi.json");
            await File.WriteAllTextAsync(openApiPath, openApiJson);
            Console.WriteLine($"[GENERATION] Saved OpenAPI spec to: {openApiPath}");
            
            // Debug: Log some paths and their operations to verify we're getting the latest
            foreach (var path in document.Paths.Take(3))
            {
                Console.WriteLine($"[GENERATION] - Path: {path.Key}");
                foreach (var operation in path.Value)
                {
                    Console.WriteLine($"[GENERATION]   - {operation.Key}: {operation.Value.OperationId}");
                }
            }
            
            // Also check if we can see the schema definitions
            var dtoSchemas = document.Definitions.Where(d => d.Key.Contains("Dto") || d.Key.Contains("Filter")).Take(3);
            Console.WriteLine($"[GENERATION] Found {document.Definitions.Count} schema definitions");
            foreach (var schema in dtoSchemas)
            {
                Console.WriteLine($"[GENERATION] - Schema: {schema.Key} with {schema.Value.Properties.Count} properties");
            }

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

            // Generate TypeScript client
            var generator = new TypeScriptClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Write to your frontend project
            var outputPath = "../../client/src/models/generated-client.ts";
            var fullOutputPath = Path.Combine(Directory.GetCurrentDirectory(), outputPath);
            
            Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath)!);
            await File.WriteAllTextAsync(fullOutputPath, code);
            
            Console.WriteLine("TypeScript client regenerated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating TypeScript client: {ex.Message}");
        }
    }
}