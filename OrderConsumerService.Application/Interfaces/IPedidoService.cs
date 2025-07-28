using OrderConsumerService.Application.Dto;

namespace OrderConsumerService.Application.Interfaces
{
    public interface IPedidoService
    {
        Task RegistraPedidoAsync(OrderDto order);
        Task CancelaPedidoAsync(OrderDto order);
        Task ConfirmaAceitePedidoAsync(OrderDto order);
        Task FinalizaPedidoAsync(OrderDto order);
    }
}
