using System.Text.Json.Serialization;

namespace OrderConsumerService.Application.DTO
{
    public class PedidoItemDto
    {
        [JsonPropertyName("pedidoId")]
        public decimal PedidoId { get; set; }

        [JsonPropertyName("itemMeneuId")]
        public int ItemMeneuId { get; set; }

        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }

        [JsonPropertyName("precoUnitario")]
        public decimal PrecoUnitario { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }
    }
}
