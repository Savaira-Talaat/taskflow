using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TaskFlow2.Models;

namespace TaskFlow.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreationDate = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<TaskFlow2.Models.Task> Tasks { get; set; } = new List<TaskFlow2.Models.Task>();
    }
}
