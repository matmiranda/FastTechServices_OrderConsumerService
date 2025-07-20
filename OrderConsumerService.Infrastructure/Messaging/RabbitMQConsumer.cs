﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderConsumerService.Application.DTO;
using OrderConsumerService.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OrderConsumerService.Infrastructure.Messaging
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _rabbitMqSettings;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, IServiceProvider serviceProvider, RabbitMQSettings rabbitMqSettings, IConnection connection = null, IModel channel = null)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMqSettings = rabbitMqSettings;

            if (connection == null || channel == null)
            {
                try
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = _rabbitMqSettings.Host,
                        UserName = _rabbitMqSettings.Username,
                        Password = _rabbitMqSettings.Password
                    };

                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    _channel.QueueDeclare(queue: _rabbitMqSettings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                    _logger.LogInformation("Conectado ao RabbitMQ em {0} e aguardando mensagens na fila '{1}'...",
                        _rabbitMqSettings.Host, _rabbitMqSettings.QueueName);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Erro ao conectar ao RabbitMQ: {0}", ex.Message);
                    throw;
                }
            }
            else
            {
                _connection = connection;
                _channel = channel;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Mensagem recebida: {0}", messageJson);

                    var jsonObject = JsonNode.Parse(messageJson);
                    var messageNode = jsonObject?["message"];

                    if (messageNode != null)
                    {
                        var pedidoJson = messageNode.ToString();
                        var pedido = JsonSerializer.Deserialize<PedidoDto>(pedidoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (pedido != null)
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

                            var pedidoEntity = pedido.ToEntity();
                            await pedidoService.EnviarPedidoAsync(pedidoEntity);

                            _channel.BasicAck(ea.DeliveryTag, false);
                            _logger.LogInformation("Pedido {0} salvo no banco!", pedido.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Falha ao desserializar o Pedido.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("JSON recebido não contém a propriedade 'message'.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Erro ao processar mensagem: {0}", ex.Message);
                }
            };

            _channel.BasicConsume(queue: _rabbitMqSettings.QueueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _logger.LogInformation("Finalizando RabbitMQConsumer...");
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }

    public class RabbitMQSettings
    {
        public string Host { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}
