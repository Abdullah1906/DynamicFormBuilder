using System.ComponentModel.DataAnnotations;

namespace DynamicFormBuilder.ViewModels
{
    public class FormCreateVM
    {
        [Required(ErrorMessage = "Form title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;
        public string FieldsJson { get; set; } = string.Empty;
    }
    public class FormListItemVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

}
