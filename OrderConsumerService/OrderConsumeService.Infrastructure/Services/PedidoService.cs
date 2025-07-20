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
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoService(IPedidoRepository PedidosRepository)
        {
            _pedidoRepository = PedidosRepository;
        }

        public async Task EnviarPedidoAsync(Pedido pedido)
        {
            await _pedidoRepository.AddPedidoAsync(pedido);
        }
    }
}
