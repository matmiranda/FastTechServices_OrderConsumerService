namespace OrderConsumerService.Domain.Entities
{
    public class Pedido
    {
        public ulong Id { get; set; }
        public ulong CustomerId { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public decimal Total { get; set; }
        public required string Status { get; set; }
        public string? CancelReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PedidoItem> Items { get; set; } = new();
    }

}
