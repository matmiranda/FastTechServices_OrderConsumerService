using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using OrderConsumerService.Domain.Entities;

namespace OrderConsumerService.Infrastructure.Persistence
{
    public class PedidoRepository(
        IConfiguration configuration, 
        ILogger<PedidoRepository> logger) : IPedidoRepository
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");
        private readonly ILogger<PedidoRepository> _logger = logger;

        public async Task AddPedidoAsync(Pedido pedido)
        {
            const string insertPedidoQuery = @"
        INSERT INTO order_db.orders 
        (customer_id, delivery_method, total, status, cancel_reason, created_at, updated_at) 
        VALUES 
        (@ClienteId, @TipoPedido, @Total, @Status, @MotivoCancelamento, @DataCriacao, @DataAlteracao);
        SELECT LAST_INSERT_ID();";

            const string insertItemQuery = @"
        INSERT INTO order_db.order_items 
        (order_id, menu_item_id, price_at_order, quantity, created_at)
        VALUES 
        (@PedidoId, @MenuItemId, @Preco, @Quantidade, @CriadoEm);";

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                //Inserir pedido e capturar ID gerado
                var pedidoId = await connection.ExecuteScalarAsync<long>(insertPedidoQuery, new
                {
                    ClienteId = pedido.CustomerId,
                    TipoPedido = pedido.DeliveryMethod.ToString(),
                    Total = pedido.Total,
                    Status = pedido.Status.ToString(), // cuidado: enum precisa bater com o banco
                    MotivoCancelamento = pedido.CancelReason,
                    DataCriacao = pedido.CreatedAt,
                    DataAlteracao = pedido.UpdatedAt
                }, transaction);

                //Inserir todos os itens com o PedidoId
                var parametros = pedido.Items.Select(item => new
                {
                    PedidoId = pedidoId,
                    MenuItemId = item.MenuItemId,
                    Preco = item.PriceAtOrder,
                    Quantidade = item.Quantity,
                    CriadoEm = item.CreatedAt
                });

                await connection.ExecuteAsync(insertItemQuery, parametros, transaction);

                //Confirmar tudo
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Erro ao inserir pedido e seus itens.");
                throw;
            }
        }

        public async Task CancelaPedidoAsync(Pedido pedido)
        {
            const string updateQuery = @"
    UPDATE order_db.orders
    SET 
        status = @Status,
        cancel_reason = @MotivoCancelamento,
        updated_at = @AtualizadoEm
    WHERE id = @PedidoId AND customer_id = @ClienteId;";

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                await connection.ExecuteAsync(updateQuery, new
                {
                    PedidoId = pedido.Id,
                    ClienteId = pedido.CustomerId,
                    Status = pedido.Status.ToString(),
                    MotivoCancelamento = pedido.CancelReason,
                    AtualizadoEm = DateTime.UtcNow.AddHours(-3)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do pedido {PedidoId}.", pedido.Id);
                throw;
            }
        }

        public async Task ConfirmaPedidoAsync(Pedido pedido)
        {
            const string updateQuery = @"
    UPDATE order_db.orders
    SET 
        status = @Status,
        updated_at = @AtualizadoEm
    WHERE id = @PedidoId;";

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                await connection.ExecuteAsync(updateQuery, new
                {
                    PedidoId = pedido.Id,
                    Status = pedido.Status.ToString(),
                    AtualizadoEm = DateTime.UtcNow.AddHours(-3)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do pedido {PedidoId}.", pedido.Id);
                throw;
            }
        }

        public async Task FinalizaPedidoAsync(Pedido pedido)
        {
            const string updateQuery = @"
    UPDATE order_db.orders
    SET 
        status = @Status,
        updated_at = @AtualizadoEm
    WHERE id = @PedidoId;";

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                await connection.ExecuteAsync(updateQuery, new
                {
                    PedidoId = pedido.Id,
                    Status = pedido.Status.ToString(),
                    AtualizadoEm = DateTime.UtcNow.AddHours(-3)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do pedido {PedidoId}.", pedido.Id);
                throw;
            }
        }
    }
}
