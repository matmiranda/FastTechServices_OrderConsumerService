using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using OrderConsumerService.Application.Dto;
using OrderConsumerService.Application.Interfaces;
using OrderConsumerService.Domain.Entities;
using OrderConsumerService.Infrastructure.Persistence;

namespace OrderConsumerService.Infrastructure.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(
            IPedidoRepository PedidosRepository,
            ILogger<PedidoService> logger)
        {
            _pedidoRepository = PedidosRepository;
            _logger = logger;
        }

        public async Task RegistraPedidoAsync(OrderDto order)
        {
            var pedido = new Pedido
            {
                Id = order.OrderId,
                CustomerId = order.CustomerId,
                Total = order.Items.Sum(i => i.PriceAtOrder * i.Quantity),
                CreatedAt = DateTime.UtcNow.AddHours(-3),
                UpdatedAt = DateTime.UtcNow.AddHours(-3),
                Status = PedidoStatus.PENDENTE.ToString(),
                CancelReason = null,
                DeliveryMethod = Enum.Parse<DeliveryMethod>(order.DeliveryMethod, ignoreCase: true),
                Items = order.Items.Select(item => new PedidoItem
                {
                    MenuItemId = item.MenuItemId,
                    PriceAtOrder = item.PriceAtOrder,
                    Quantity = item.Quantity,
                    CreatedAt = DateTime.UtcNow.AddHours(-3)
                })
                .ToList()
            };

            await _pedidoRepository.AddPedidoAsync(pedido);
        }

        public async Task ConfirmaAceitePedidoAsync(OrderDto order)
        {
            var pedido = new Pedido
            {
                Id = order.OrderId,
                CustomerId = order.CustomerId,
                UpdatedAt = DateTime.UtcNow.AddHours(-3),
                Status = PedidoStatus.EM_PREPARO.ToString(),
                CancelReason = order.CancelReason
            };
            await _pedidoRepository.ConfirmaPedidoAsync(pedido);
        }

        public async Task CancelaPedidoAsync(OrderDto order)
        {
            var pedido = new Pedido
            {
                Id = order.OrderId,
                CustomerId = order.CustomerId,
                UpdatedAt = DateTime.UtcNow.AddHours(-3),
                Status = PedidoStatus.CANCELADO.ToString(),
                CancelReason = order.CancelReason
            };
            await _pedidoRepository.CancelaPedidoAsync(pedido);
        }

        public async Task FinalizaPedidoAsync(OrderDto order)
        {
            var pedido = new Pedido
            {
                Id = order.OrderId,
                UpdatedAt = DateTime.UtcNow.AddHours(-3),
                Status = PedidoStatus.PRONTO.ToString(),
            };
            await _pedidoRepository.FinalizaPedidoAsync(pedido);
        }
    }
}
