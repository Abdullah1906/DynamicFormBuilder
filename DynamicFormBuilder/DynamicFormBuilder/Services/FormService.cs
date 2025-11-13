using DynamicFormBuilder.Models;
using DynamicFormBuilder.Repositories;
using DynamicFormBuilder.ViewModels;
using Microsoft.AspNetCore.Http;

namespace DynamicFormBuilder.Services
{
    public class FormService : IFormService
    {
        private readonly IFormRepository _repo;
        public FormService(IFormRepository repo) => _repo = repo;

        public Task<int> CreateFormAsync(Form form) => _repo.CreateFormAsync(form);
        public Task<IEnumerable<FormListItemVM>> GetAllFormsAsync() => _repo.GetAllFormsAsync();
        public Task<Form?> GetFormWithFieldsAsync(int id) => _repo.GetFormWithFieldsAsync(id);
    }
}
