using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Models;

public class AccountMember
{
    [Key]
    [MaxLength(20)]
    public string MemberID { get; set; } = null!;

    [Required]
    [MaxLength(80)]
    public string MemberPassword { get; set; } = null!;

    [Required]
    [MaxLength(80)]
    public string FullName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string EmailAddress { get; set; } = null!;

    public int MemberRole { get; set; }
}
