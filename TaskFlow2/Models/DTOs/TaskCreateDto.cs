using System.ComponentModel.DataAnnotations;

namespace TaskFlow2.Models.DTOs
{
    public class TaskCreateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public int ProjectId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
