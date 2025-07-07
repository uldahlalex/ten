using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Postgres.Scaffolding;
using Generated;
using TUnit.Core;
using api.Etc;

namespace tests.Utilities;

public abstract class ApiTestBase
{
    protected WebApplication App = null!;
    protected HttpClient Client = null!;
    protected IServiceProvider ScopedServiceProvider = null!;
    protected IApiClient ApiClient = null!;
    private IServiceScope _scope = null!;

    [Before(Test)]
    public virtual async Task Setup()
    {
        var builder = ApiTestSetupUtilities.MakeWebAppBuilderForTesting();
        builder.AddProgramcsServices();
        
        // Hook: After services are added, before test modifications
        await OnAfterServicesAdded(builder);
        
        builder.ModifyServicesForTesting();
        
        // Hook: After test modifications, before building app
        await OnAfterServicesModified(builder);
        
        App = builder.Build();
        
        // Hook: After app is built, before middleware
        await OnBeforeMiddleware(App);
        
        App.BeforeProgramcsMiddleware();
        App.AddProgramcsMiddleware();
        
        // Hook: After middleware, before starting
        await OnAfterMiddleware(App);
        
        App.AfterProgramcsMiddleware();

        var baseUrl = App.Urls.First() + "/";
        Client = ApiTestSetupUtilities.CreateHttpClientWithDefaultTestJwt();
        ApiClient = new ApiClient(baseUrl, Client);

        _scope = App.Services.CreateScope();
        ScopedServiceProvider = _scope.ServiceProvider;

        await OnSetupComplete();
    }

    [After(Test)]
    public virtual async Task Cleanup()
    {
        await OnBeforeCleanup();
        
        Client?.Dispose();
        _scope?.Dispose();
        
        if (App != null)
        {
            await App.DisposeAsync();
        }
        
        await OnAfterCleanup();
    }

    /// <summary>
    /// Called after Program.ConfigureServices but before test-specific service modifications.
    /// Override to add or replace services before test modifications are applied.
    /// </summary>
    protected virtual Task OnAfterServicesAdded(WebApplicationBuilder builder)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called after test-specific service modifications but before the app is built.
    /// Override to make final service adjustments.
    /// </summary>
    protected virtual Task OnAfterServicesModified(WebApplicationBuilder builder)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called after the app is built but before middleware is configured.
    /// Override to add early middleware or modify app configuration.
    /// </summary>
    protected virtual Task OnBeforeMiddleware(WebApplication app)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called after Program.ConfigureApp middleware but before the app starts.
    /// Override to add additional middleware or modify routing.
    /// </summary>
    protected virtual Task OnAfterMiddleware(WebApplication app)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called after all setup is complete and the app is ready for testing.
    /// Override for final initialization steps.
    /// </summary>
    protected virtual Task OnSetupComplete()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called before cleanup begins.
    /// Override to perform custom cleanup before resources are disposed.
    /// </summary>
    protected virtual Task OnBeforeCleanup()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called after all resources have been disposed.
    /// Override for final cleanup steps.
    /// </summary>
    protected virtual Task OnAfterCleanup()
    {
        return Task.CompletedTask;
    }
}