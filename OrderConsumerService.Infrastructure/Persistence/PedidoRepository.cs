using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using OrderConsumerService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderConsumerService.Infrastructure.Persistence
{
    public class PedidoRepository(IConfiguration configuration) : IPedidoRepository
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        public async Task AddPedidoAsync(Pedido pedido)
        {
            // Garantir que a DataCriacao e DataAlteracao sejam atualizadas antes de salvar
            pedido.DataCriacao = DateTime.Now;
            pedido.DataAlteracao = DateTime.Now;

            const string query = @"
            INSERT INTO orders (customer_id, delivery_method, total, status, cancel_reason, created_at, updated_at) 
            VALUES (@ClienteId, @TipoPedido, @Total, @Status, @MotivoCancelamento, @DataCriacao, @DataAlteracao);";

            Console.WriteLine($"ClienteId: {pedido.ClienteId}, TipoPedido: {pedido.TipoPedido}, Total: {pedido.Total}, Status: {pedido.Status}, MotivoCancelamento: {pedido.MotivoCancelamento}, DataCriacao: {pedido.DataCriacao}, DataAlteracao: {pedido.DataAlteracao}");

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(query, pedido);
        }
    }
}
