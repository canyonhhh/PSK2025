using System.ComponentModel.DataAnnotations;

namespace PSK2025.Models.DTOs
{
    public class ChangeUserRoleDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}