using OrderConsumerService.Domain.Entities;

namespace OrderConsumerService.Infrastructure.Persistence
{
    public interface IPedidoItemRepository
    {
        Task AddPedidoItemAsync(PedidoItem pedidoItem);
    }
}
