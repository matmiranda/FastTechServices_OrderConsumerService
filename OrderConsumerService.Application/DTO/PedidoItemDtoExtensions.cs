using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderConsumerService.Application.DTO
{
    public static class PedidoItemDtoExtensions
    {
        public static Domain.Entities.PedidoItem ToEntity(this PedidoItemDto dto)
        {
            return new Domain.Entities.PedidoItem
            {
                PedidoId = dto.PedidoId,
                ItemMeneuId = dto.ItemMeneuId,
                Quantidade = dto.Quantidade,
                PrecoUnitario = dto.PrecoUnitario,
                DataCriacao = dto.DataCriacao
            };
        }
    }
}
