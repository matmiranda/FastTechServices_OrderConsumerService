namespace OrderConsumerService.Domain.Entities
{
    public class PedidoItem
    {
        public decimal Id { get; set; }

        public decimal PedidoId { get; set; }
        public Pedido Pedido { get; set; } = new Pedido();

        public int ItemMeneuId { get; set; }

        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; } = 0;
        public decimal Total => Quantidade * PrecoUnitario;

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
