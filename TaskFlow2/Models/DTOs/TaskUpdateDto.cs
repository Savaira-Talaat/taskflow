using System.ComponentModel.DataAnnotations;

namespace TaskFlow2.Models.DTOs
{
    public class TaskUpdateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public TaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string> Comments {  get; set; } = new List<string>();
    }
}
