namespace OrderConsumerService.Domain.Entities
{
    public class Pedido
    {
        public decimal Id { get; set; }
        public decimal ClienteId { get; set; }
        public string TipoPedido { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? MotivoCancelamento { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAlteracao { get; set; } = DateTime.Now;
    }
}
