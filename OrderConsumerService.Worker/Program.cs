using OrderConsumerService.Worker;
using OrderConsumeService.Infrastructure.Persistence;
using OrderConsumeService.Application.Interfaces;
using OrderConsumeService.Infrastructure.Services;
using OrderConsumeService.Infrastructure.Messaging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("/app/logs/criar-pedidos/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Carrega o arquivo appsettings.json
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Carrega as configurações do RabbitMQ do appsettings.json
        var rabbitMqSettings = context.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();

        if (rabbitMqSettings == null)
            throw new InvalidOperationException("RabbitMQSettings não pode ser nulo. Verifique o arquivo appsettings.json.");

        services.AddSingleton(rabbitMqSettings);

        // Registrando serviços e repositórios
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IPedidoService, PedidoService>();

        services.AddScoped<IPedidoItemRepository, PedidoItemRepository>();
        services.AddScoped<IPedidoItemService, PedidoItemService>();

        // Registrando o RabbitMQConsumer como Singleton e o Worker como HostedService
        services.AddSingleton<RabbitMQConsumer>();
        services.AddHostedService<RabbitWorker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

await host.RunAsync();
