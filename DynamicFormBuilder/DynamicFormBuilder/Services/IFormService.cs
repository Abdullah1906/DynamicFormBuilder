using DynamicFormBuilder.Models;
using DynamicFormBuilder.ViewModels;

namespace DynamicFormBuilder.Services
{
    public interface IFormService
    {
        Task<int> CreateFormAsync(Form form);
        Task<IEnumerable<FormListItemVM>> GetAllFormsAsync();
        Task<Form?> GetFormWithFieldsAsync(int id);
    }
}

