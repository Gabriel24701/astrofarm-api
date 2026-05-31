using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstroFarm.Api.Models
{
    [Table("CULTURA")]
    public class Cultura
    {
        [Key]
        [Column("ID_CULTURA")]
        public int Id { get; set; }

        [Required]
        [Column("TIPO_CULTURA")]
        public string TipoCultura { get; set; } = string.Empty;

        [Required]
        [Column("SAFRA")]
        public string Safra { get; set; } = string.Empty;

        [Column("AREA_PLANTADA")]
        public decimal? AreaPlantada { get; set; }

        [Column("DT_PLANTIO")]
        public DateTime? DtPlantio { get; set; }

        [Column("DT_COLHEITA_PREV")]
        public DateTime? DtColheitaPrev { get; set; }

        [Column("STATUS")]
        public string Status { get; set; } = "A";

        [Column("ID_PROPRIEDADE")]
        public int IdPropriedade { get; set; }

        [ForeignKey("IdPropriedade")]
        public Propriedade? Propriedade { get; set; } = null!;
    }
}