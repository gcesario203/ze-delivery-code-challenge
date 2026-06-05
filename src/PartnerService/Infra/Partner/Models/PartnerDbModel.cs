
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartnerService.Infra.Shared.Models;

public sealed class PartnerDbModel : IDbModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string TradingName { get; set; }

    [Required]
    [MaxLength(255)]
    public string OwnerName { get; set; }

    [Required]
    [MaxLength(14)]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "Document must be a valid CNPJ format (14 digits).")]
    public string Document { get; set; }
}