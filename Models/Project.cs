using System.ComponentModel.DataAnnotations;
namespace e_Project.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(30, ErrorMessage = "Author name cannot exceed 30 characters.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(40, ErrorMessage = "Title cannot exceed 40 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Programming Language is required.")]
        [StringLength(15, ErrorMessage = "Programming Language cannot exceed 10 characters.")]
        public string ProgrammingLanguage { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public ProjectStatus Status { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }
    }

    public enum ProjectStatus
    {
        Pending,
        InProgress,
        Completed
    }
}
