using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.TypeScript;
using NSwag.Generation;

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
            // Generate OpenAPI document
            var document = await _app.Services.GetRequiredService<IOpenApiDocumentGenerator>()
                .GenerateAsync("v1");

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