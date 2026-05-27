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

        [Column("ID_PROPRIEDADE")]
        public int PropriedadeId { get; set; }

        [Required]
        [Column("DT_LEITURA")]
        public DateTime DtLeitura { get; set; }

        [Column("NDVI")]
        public double? Ndvi { get; set; }

        [Column("TEMPERATURA")]
        public double? Temperatura { get; set; }

        [Column("UMIDADE")]
        public double? Umidade { get; set; }

        [Column("PRECIPITACAO")]
        public double? Precipitacao { get; set; }

        [Column("FONTE_SATELITE")]
        public string? FonteSatelite { get; set; }

        [ForeignKey("PropriedadeId")]
        public Propriedade? Propriedade { get; set; }
    }
}