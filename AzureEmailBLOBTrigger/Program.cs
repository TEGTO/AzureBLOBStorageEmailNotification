using Azure.Communication.Email;
using AzureEmailBLOBTrigger.Options;
using AzureEmailBLOBTrigger.Sevices.CreateFileUriServices;
using AzureEmailBLOBTrigger.Sevices.EmailNotificatorServices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

var build = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
     .ConfigureAppConfiguration(builder =>
     {
         builder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
     })
    .ConfigureServices((hostContext, services) =>
    {
        var triggerConfig = hostContext.Configuration.GetSection(TriggerOptions.OptionPosition);
        services.Configure<TriggerOptions>(triggerConfig);

        var emailConfig = hostContext.Configuration.GetSection(EmailServiceConnectionOptions.OptionPosition);
        services.Configure<EmailServiceConnectionOptions>(emailConfig);

        var emailOptions = emailConfig.Get<EmailServiceConnectionOptions>();
        EmailClient emailClient = new EmailClient(emailOptions.EmailConnectionString);

        services.AddSingleton(emailClient);
        services.AddSingleton<IEmailNotificator, EmailNotificator>();
        services.AddSingleton<ICreateFileUri, CreateFileSASUri>();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    });

var host = build.Build();
host.Run();
