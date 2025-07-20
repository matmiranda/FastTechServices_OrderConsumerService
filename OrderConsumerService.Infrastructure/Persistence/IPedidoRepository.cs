using OrderConsumerService.Domain.Entities;

namespace OrderConsumerService.Infrastructure.Persistence
{
    public interface IPedidoRepository
    {
        Task AddPedidoAsync(Pedido pedido);
    }
}
