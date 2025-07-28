namespace OrderConsumerService.Application.Dto
{
    public class OrderItemDto
    {
        public ulong MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
    }

}
