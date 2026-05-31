using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstroFarm.Api.Models
{
    [Table("HISTORICO_CLIMA")]
    public class HistoricoClima
    {
        [Key]
        [Column("ID_CLIMA")]
        public int Id { get; set; }

        [Required]
        [Column("ESTADO")]
        public string Estado { get; set; } = string.Empty;

        [Required]
        [Column("MUNICIPIO")]
        public string Municipio { get; set; } = string.Empty;

        [Required]
        [Column("ANO_MES")]
        public string AnoMes { get; set; } = string.Empty;

        [Column("TEMP_MEDIA")]
        public decimal? TempMedia { get; set; }

        [Column("TEMP_MAX")]
        public decimal? TempMax { get; set; }

        [Column("TEMP_MIN")]
        public decimal? TempMin { get; set; }

        [Column("PRECIPITACAO_MM")]
        public decimal? PrecipitacaoMm { get; set; }

        [Column("UMIDADE_MEDIA")]
        public decimal? UmidadeMedia { get; set; }
    }
}