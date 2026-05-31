using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstroFarm.Api.Models
{
    [Table("LEITURA_SATELITAL")]
    public class LeituraSatelital
    {
        [Key]
        [Column("ID_LEITURA")]
        public int Id { get; set; }

        [Column("DT_LEITURA")]
        public DateTime DtLeitura { get; set; }

        [Column("NDVI")]
        public decimal? Ndvi { get; set; }

        [Column("TEMPERATURA")]
        public decimal? Temperatura { get; set; }

        [Column("UMIDADE")]
        public decimal? Umidade { get; set; }

        [Column("PRECIPITACAO")]
        public decimal? Precipitacao { get; set; }

        [Column("FONTE_SATELITE")]
        public string? FonteSatelite { get; set; }

        [Column("ID_PROPRIEDADE")]
        public int IdPropriedade { get; set; }

        [ForeignKey("IdPropriedade")]
        public Propriedade? Propriedade { get; set; }

        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    }
}