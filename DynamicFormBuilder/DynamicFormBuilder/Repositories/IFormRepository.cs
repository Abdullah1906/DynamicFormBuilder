using DynamicFormBuilder.Models;
using DynamicFormBuilder.ViewModels;

namespace DynamicFormBuilder.Repositories
{
    public interface IFormRepository
    {
        Task<int> CreateFormAsync(Form form);
        Task<IEnumerable<FormListItemVM>> GetAllFormsAsync();
        Task<Form?> GetFormWithFieldsAsync(int id);
    }
}
