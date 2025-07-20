using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using OrderConsumeService.Domain.Entities;

namespace OrderConsumeService.Infrastructure.Persistence
{
    public class PedidoItemRepository(IConfiguration configuration) : IPedidoItemRepository
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        public async Task AddPedidoItemAsync(PedidoItem pedidoItem)
        {
            // Garantir que a DataCriacao e DataAlteracao sejam atualizadas antes de salvar
            pedidoItem.DataCriacao = DateTime.Now;

            const string query = @"
            INSERT INTO order_items (order_id, menu_item_id, quantity, price_at_order, total_item, created_at) 
            VALUES (@PedidoId, @ItemMeneuId, @Quantidade, @Status, @PrecoUnitario, @Total, @DataCriacao);";

            Console.WriteLine($"No. Pedido: {pedidoItem.PedidoId}, Item Pedido: {pedidoItem.Id}, Item Menu: {pedidoItem.ItemMeneuId}, Qtd: {pedidoItem.Quantidade} Vlr Unitário: {pedidoItem.PrecoUnitario}, Total Pedido: {pedidoItem.Total}, DataCriacao: {pedidoItem.DataCriacao}");

            using var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(query, pedidoItem);
        }
    }
}
