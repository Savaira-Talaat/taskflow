using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace TaskFlow2.Models.DTOs
{
    public class ProjectUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }
    }
}
