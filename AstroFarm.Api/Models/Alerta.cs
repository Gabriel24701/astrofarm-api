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

        [Column("RESOLVIDO")]
        public string Resolvido { get; set; } = "N";

        [Column("DT_RESOLUCAO")]
        public DateTime? DtResolucao { get; set; }
        
        [Column("ID_PROPRIEDADE")]
        public int IdPropriedade { get; set; }

        [ForeignKey("IdPropriedade")]
        public Propriedade? Propriedade { get; set; } = null!;
        
        [Column("ID_LEITURA")]
        public int? IdLeitura { get; set; }

        [ForeignKey("IdLeitura")]
        public LeituraSatelital? Leitura { get; set; }
    }
}