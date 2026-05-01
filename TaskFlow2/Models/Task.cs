using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TaskFlow.Models;

namespace TaskFlow2.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.AFaire;
        public DateTime? DueDate { get; set; }

        public List<string> Comments { get; set; } = new List<string>();

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public enum TaskStatus
        {
            AFaire,
            EnCours,
            Termine
        } 

    }
}
