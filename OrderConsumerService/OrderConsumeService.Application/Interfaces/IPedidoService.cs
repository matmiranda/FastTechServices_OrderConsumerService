namespace OrderConsumeService.Application.Interfaces
{
    public interface IPedidoService
    {
        Task EnviarPedidoAsync(Domain.Entities.Pedido pedido);
    }
}
