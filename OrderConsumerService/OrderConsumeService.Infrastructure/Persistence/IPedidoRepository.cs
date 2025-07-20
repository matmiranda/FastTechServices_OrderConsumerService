using OrderConsumeService.Domain.Entities;

namespace OrderConsumeService.Infrastructure.Persistence
{
    public interface IPedidoRepository
    {
        Task AddPedidoAsync(Pedido pedido);
    }
}
