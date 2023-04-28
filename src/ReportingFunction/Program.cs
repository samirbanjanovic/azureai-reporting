using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Orchestration;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(configuration =>
    {
        configuration.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        
        services.AddScoped<IKernel>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<KernelBuilder>>();
            var skillsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "skills");
            var kernel = Kernel
                            .Builder
                            .WithLogger(logger)
                            .Build();
            
            kernel.Config.AddAzureTextCompletionService("davinci", context.Configuration["DeploymentName"], context.Configuration["Endpoint"], context.Configuration["ApiKey"]);

            kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "SummarizeSkill");            

            return kernel;
        });
    })
    .Build();

host.Run();
