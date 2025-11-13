using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services;
using DynamicFormBuilder.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class FormBuilderController : Controller
{
    private readonly IFormService _service;
    public FormBuilderController(IFormService service) => _service = service;

    public IActionResult Create()
    {
        return View(new FormCreateVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FormCreateVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        //if (string.IsNullOrWhiteSpace(vm.Title))
        //{
        //    ModelState.AddModelError(nameof(vm.Title), "Title is required.");
        //    return View(vm);
        //}

        List<FormFieldInput>? inputs = null;
        try
        {
            inputs = JsonSerializer.Deserialize<List<FormFieldInput>>(vm.FieldsJson);
        }
        catch
        {
            ModelState.AddModelError("", "Invalid fields payload.");
            return View(vm);
        }

        if (inputs == null || inputs.Count == 0)
        {
            ModelState.AddModelError("", "Please add at least one dropdown field.");
            return View(vm);
        }
        // Validate each dynamic field
        for (int i = 0; i < inputs.Count; i++)
        {
            var field = inputs[i];
            if (string.IsNullOrWhiteSpace(field.label))
            {
                ModelState.AddModelError("", $"Field #{i + 1}: Label is required.");
            }

            if (string.IsNullOrWhiteSpace(field.selectedOption))
            {
                ModelState.AddModelError("", $"Field #{i + 1}: Please select or enter an option.");
            }
        }

        if (!ModelState.IsValid)
            return View(vm);

        var form = new Form
        {
            Title = vm.Title,
            CreatedAt = DateTime.UtcNow,
            Fields = inputs.Select((x, idx) => new FormField
            {
                Label = x.label,
                SelectedOption = x.selectedOption,
                IsRequired = x.isRequired,
                SortOrder = idx
            }).ToList()
        };

        var id = await _service.CreateFormAsync(form);
        return RedirectToAction(nameof(Preview), new { id });
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> ListJson()
    {
        var list = await _service.GetAllFormsAsync();
        return Json(new { data = list });
    }

    public async Task<IActionResult> Preview(int id)
    {
        var form = await _service.GetFormWithFieldsAsync(id);
        if (form == null) return NotFound();
        return View(form);
    }

  
}
