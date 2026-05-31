using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstroFarm.Api.Models
{
    [Table("PROPRIEDADE")]
    public class Propriedade
    {
        [Key]
        [Column("ID_PROPRIEDADE")]
        public int Id { get; set; }

        [Column("ID_PRODUTOR")]
        public int IdProdutor { get; set; }   // ← renomeado para IdProdutor

        [Required]
        [Column("NOME_FAZENDA")]
        public string NomeFazenda { get; set; } = string.Empty;

        [Required]
        [Column("AREA_HECTARES")]
        public double AreaHectares { get; set; }

        [Column("LATITUDE")]
        public double? Latitude { get; set; }

        [Column("LONGITUDE")]
        public double? Longitude { get; set; }

        [Required]
        [Column("ESTADO")]
        public string Estado { get; set; } = string.Empty;

        [Required]
        [Column("MUNICIPIO")]
        public string Municipio { get; set; } = string.Empty;

        [Column("DT_REGISTRO")]
        public DateTime DtRegistro { get; set; } = DateTime.Now;

// Relacionamentos
        [ForeignKey("IdProdutor")]
        public Produtor? Produtor { get; set; }

        public ICollection<Cultura> Culturas { get; set; } = new List<Cultura>();
        public ICollection<LeituraSatelital> Leituras { get; set; } = new List<LeituraSatelital>();
        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    }
}