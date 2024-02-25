using Azure.Communication.Email;
using AzureEmailBLOBTrigger.Options;
using AzureEmailBLOBTrigger.Sevices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var build = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services) =>
    {
        var emailConfig = hostContext.Configuration.GetSection(EmailSendOptions.OptionPosition);
        services.Configure<EmailSendOptions>(emailConfig);

        var emailOptions = emailConfig.Get<EmailSendOptions>();
        EmailClient emailClient = new EmailClient(emailOptions.EmailConnectionString);
        services.AddSingleton(emailClient);
        services.AddSingleton<IEmailNotificator, EmailNotificator>();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    });

var host = build.Build();
host.Run();
