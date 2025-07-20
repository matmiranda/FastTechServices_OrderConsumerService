using OrderConsumeService.Application.Interfaces;
using OrderConsumeService.Domain.Entities;
using OrderConsumeService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderConsumeService.Infrastructure.Services
{
    public class PedidoItemService : IPedidoItemService
    {
        private readonly IPedidoItemRepository _pedidoItemRepository;

        public PedidoItemService(IPedidoItemRepository PedidoItemRepository)
        {
            _pedidoItemRepository = PedidoItemRepository;
        }

        public async Task EnviarPedidoItemAsync(List<PedidoItem> pedidosItens)
        {
            foreach (var pedidoItem in pedidosItens)
            {
                // Garantir que a DataCriacao e DataAlteracao sejam atualizadas antes de salvar
                pedidoItem.DataCriacao = DateTime.Now;

                await _pedidoItemRepository.AddPedidoItemAsync(pedidoItem);
            }            
        }
    }
}
