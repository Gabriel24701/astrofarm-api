using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstroFarm.Api.Models
{
    [Table("ALERTA")]
    public class Alerta
    {
        [Key]
        [Column("ID_ALERTA")]
        public int Id { get; set; }

        [Column("ID_PROPRIEDADE")]
        public int PropriedadeId { get; set; }

        [Required]
        [Column("TIPO_ALERTA")]
        public string TipoAlerta { get; set; } = string.Empty;

        [Required]
        [Column("NIVEL_RISCO")]
        public string NivelRisco { get; set; } = string.Empty;

        [Column("DESCRICAO")]
        public string? Descricao { get; set; }

        [Column("DT_ALERTA")]
        public DateTime DtAlerta { get; set; } = DateTime.Now;

        [ForeignKey("PropriedadeId")]
        public Propriedade? Propriedade { get; set; }
    }
}