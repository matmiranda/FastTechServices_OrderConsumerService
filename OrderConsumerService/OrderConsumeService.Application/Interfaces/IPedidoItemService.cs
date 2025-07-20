namespace OrderConsumeService.Application.Interfaces
{
    public interface IPedidoItemService
    {
        Task EnviarPedidoItemAsync(List<Domain.Entities.PedidoItem> itens);
    }
}
