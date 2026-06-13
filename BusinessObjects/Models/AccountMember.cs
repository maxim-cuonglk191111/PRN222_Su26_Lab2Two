using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Models;

public class AccountMember
{
    [Key]
    public int MemberId { get; set; }

    [Required]
    public string MemberPassword { get; set; } = null!;

    [Required]
    public int MemberRole { get; set; }
}
