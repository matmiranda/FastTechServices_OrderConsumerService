namespace OrderConsumerService.Application.Interfaces
{
    public interface IPedidoService
    {
        Task EnviarPedidoAsync(Domain.Entities.Pedido pedido);
    }
}
