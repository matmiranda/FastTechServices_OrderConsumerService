using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderConsumerService.Application.Dto;
using OrderConsumerService.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderConsumerService.Infrastructure.Messaging
{
    public class RabbitMQConsumer : IDisposable
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _settings;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMQConsumer(
            ILogger<RabbitMQConsumer> logger,
            IServiceProvider serviceProvider,
            RabbitMQSettings settings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _settings = settings;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                UserName = _settings.Username,
                Password = _settings.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "order.placed", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "order.cancelled", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "kitchen.order.accepted", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "kitchen.order.rejected", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(queue: "kitchen.order.ready", durable: true, exclusive: false, autoDelete: false);


            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    _logger.LogInformation("Mensagem recebida da fila '{0}': {1}", ea.RoutingKey, json);

                    var orderDto = JsonSerializer.Deserialize<OrderDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (orderDto == null)
                    {
                        _logger.LogWarning("Falha ao desserializar o pedido.");
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var orderService = scope.ServiceProvider.GetRequiredService<IPedidoService>();

                    if (ea.RoutingKey.Equals("order.placed", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Processando novo pedido: {0}", orderDto);
                        await orderService.RegistraPedidoAsync(orderDto);
                    }
                    else if (ea.RoutingKey.Equals("order.cancelled", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Cancelando pedido {0} com justificativa: {1}", orderDto.OrderId, orderDto.CancelReason);
                        await orderService.CancelaPedidoAsync(orderDto);
                    }
                    else if (ea.RoutingKey.Equals("kitchen.order.accepted", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Pedido {0} aceito pela cozinha.", orderDto.OrderId);
                        await orderService.ConfirmaAceitePedidoAsync(orderDto);
                    }
                    else if (ea.RoutingKey.Equals("kitchen.order.rejected", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Pedido {0} rejeitado pela cozinha. Motivo: {1}", orderDto.OrderId, orderDto.CancelReason);
                        await orderService.CancelaPedidoAsync(orderDto);
                    }
                    else if (ea.RoutingKey.Equals("kitchen.order.ready", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Pedido {0} foi finalizado e está pronto para entrega ou retirada.", orderDto.OrderId);
                        await orderService.FinalizaPedidoAsync(orderDto);
                    }

                    else
                    {
                        _logger.LogWarning("Fila desconhecida: {0}", ea.RoutingKey);
                        _channel.BasicNack(ea.DeliveryTag, false, false);
                        return;
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation("Evento de pedido {0} processado com sucesso.", ea.RoutingKey);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem da fila '{0}'", ea.RoutingKey);
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(queue: "order.placed", autoAck: false, consumer: consumer);
            _channel.BasicConsume(queue: "order.cancelled", autoAck: false, consumer: consumer);
            _channel.BasicConsume(queue: "kitchen.order.accepted", autoAck: false, consumer: consumer);
            _channel.BasicConsume(queue: "kitchen.order.rejected", autoAck: false, consumer: consumer);
            _channel.BasicConsume(queue: "kitchen.order.ready", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parando RabbitMQConsumer...");
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Finalizando RabbitMQConsumer...");
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
