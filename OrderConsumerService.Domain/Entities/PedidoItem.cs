namespace OrderConsumerService.Domain.Entities
{
    public class PedidoItem
    {
        public ulong Id { get; set; }
        public ulong MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
        public decimal TotalItem => Quantity * PriceAtOrder;
        public DateTime CreatedAt { get; set; }
    }
}
