﻿// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using AIPlugins.AzureFunctions.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Models;
using skchatgptazurefunction.WebSearcher;
using skchatgptazurefunction.WebSearcher.Models;

const string DefaultSemanticFunctionsFolder = "Prompts";
string semanticFunctionsFolder = Environment.GetEnvironmentVariable("SEMANTIC_SKILLS_FOLDER") ?? DefaultSemanticFunctionsFolder;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(configuration =>
    {
        var config = configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        var builtConfig = config.Build();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<JsonSerializerOptions>(options =>
        {
            // `ConfigureFunctionsWorkerDefaults` sets the default to ignore casing already.
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        

        services.AddScoped<BingSearch>();

        services.AddSingleton<PluginConfig>((provider => {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var pluginConfigSection = configuration.GetSection("Values").Get<PluginConfig>();
            return pluginConfigSection;
        }));
        services.AddSingleton<IOpenApiConfigurationOptions>(_ =>
        {
            var options = new OpenApiConfigurationOptions()
            {
                Info = new OpenApiInfo()
                {
                    Version = "1.0.0",
                    Title = "Web Searcher Plugin",
                    Description = "This plugin is capable of searching the internet."
                },
                Servers = DefaultOpenApiConfigurationOptions.GetHostNames(),
                OpenApiVersion = OpenApiVersionType.V3,
                //IncludeRequestingHostName = true,
                ForceHttps = false,
                ForceHttp = false,
            };

            return options;
        });
        services
            .AddScoped<IKernel>((providers) =>
            {
                // This will be called each time a new Kernel is needed

                // Get a logger instance
                ILogger<IKernel> logger = providers
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger<IKernel>();

                // Register your AI Providers...
                var appSettings = AppSettings.LoadSettings();
                IKernel kernel = new KernelBuilder()
                    .WithChatCompletionService(appSettings.Kernel)
                    .WithLoggerFactory(providers.GetRequiredService<ILoggerFactory>())
                    .Build();

                // Load your semantic functions...
                kernel.ImportPromptsFromDirectory(appSettings.AIPlugin.NameForModel, semanticFunctionsFolder);

                return kernel;
            })
            .AddScoped<IAIPluginRunner, AIPluginRunner>();
    })
    .Build();

host.Run();
