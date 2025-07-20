namespace OrderConsumeService.Application.DTO
{
    public static class PedidoDtoExtensions
    {
        public static Domain.Entities.Pedido ToEntity(this PedidoDto dto)
        {
            return new Domain.Entities.Pedido
            {
                ClienteId = dto.ClienteId,
                TipoPedido = dto.TipoPedido,
                Total = dto.Total,
                Status = dto.Status,
                DataCriacao = dto.DataCriacao,
                DataAlteracao = dto.DataAlteracao
            };
        }
    }
}
