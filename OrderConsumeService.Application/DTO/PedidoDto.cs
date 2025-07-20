using System.Text.Json.Serialization;

namespace OrderConsumeService.Application.DTO
{
    public class PedidoDto
    {
        [JsonPropertyName("id")]
        public decimal Id { get; set; }

        [JsonPropertyName("clienteId")]
        public decimal ClienteId { get; set; }

        [JsonPropertyName("tipoPedido")]
        public string TipoPedido { get; set; } = string.Empty;

        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }
    }
}
