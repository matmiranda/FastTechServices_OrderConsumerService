namespace OrderConsumerService.Application.Dto
{
    public class OrderDto
    {
        public ulong OrderId { get; set; }
        public ulong CustomerId { get; set; }
        public decimal Total { get; set; }
        public string DeliveryMethod { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
        public string? CancelReason { get; set; }
    }

}
