using OrderConsumerService.Domain.Entities;

namespace OrderConsumerService.Infrastructure.Persistence
{
    public interface IPedidoRepository
    {
        Task AddPedidoAsync(Pedido pedido);
        Task CancelaPedidoAsync(Pedido pedido);
        Task ConfirmaPedidoAsync(Pedido pedido);
        Task FinalizaPedidoAsync(Pedido pedido);
    }
}
