using System.ComponentModel.DataAnnotations;

namespace DynamicFormBuilder.Models
{
    public class Form
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }  
        public bool IsActive { get; set; } = true; 
        public bool IsDeleted { get; set; } = false;
        public List<FormField> Fields { get; set; } = new();
    }
    public class FormField
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string? SelectedOption { get; set; }
        public bool IsRequired { get; set; }
        public int SortOrder { get; set; }
        public DateTime? UpdatedAt { get; set; }  
        public bool IsActive { get; set; } = true; 
        public bool IsDeleted { get; set; } = false;

    }
    public class FormFieldInput
    {
        [Required(ErrorMessage = "Label is required.")]
        public string label { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please select or enter an option.")]
        public bool isRequired { get; set; }
        public string? selectedOption { get; set; }
    }
}
