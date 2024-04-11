using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace e_Project.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(30, ErrorMessage = "Author name cannot exceed 30 characters.")]
        [Display(Name ="Author")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(40, ErrorMessage = "Title cannot exceed 40 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Programming Language is required.")]
        [StringLength(15, ErrorMessage = "Programming Language cannot exceed 15 characters.")]
        [Display(Name ="Programming Language")]
        public string ProgrammingLanguage { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProjectStatus Status { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        [Display(Name ="Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Completion Time is required.")]
        [Display(Name ="Completion Time")]
        public int CompletionTime { get; set; }

    }

    public enum ProjectStatus
    {
        Pending,
        InProgress,
        Completed
    }
   
}