using OrderConsumeService.Domain.Entities;

namespace OrderConsumeService.Infrastructure.Persistence
{
    public interface IPedidoItemRepository
    {
        Task AddPedidoItemAsync(PedidoItem pedidoItem);
    }
}
