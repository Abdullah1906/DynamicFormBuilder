using DynamicFormBuilder.Models;
using DynamicFormBuilder.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DynamicFormBuilder.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly string _conn;

        public FormRepository(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public async Task<int> CreateFormAsync(Form form)
        {
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            using var tran = conn.BeginTransaction();
            try
            {
                // Insert Form
                var cmdForm = new SqlCommand("sp_InsertForm", conn, tran)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmdForm.Parameters.AddWithValue("@Title", form.Title);
                cmdForm.Parameters.AddWithValue("@CreatedAt", form.CreatedAt);
                cmdForm.Parameters.AddWithValue("@IsActive", form.IsActive);
                cmdForm.Parameters.AddWithValue("@IsDeleted", form.IsDeleted);

                var formId = (int)await cmdForm.ExecuteScalarAsync();

                // Insert Fields
                for (int i = 0; i < form.Fields.Count; i++)
                {
                    var f = form.Fields[i];
                    var cmdField = new SqlCommand("sp_InsertFormField", conn, tran)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmdField.Parameters.AddWithValue("@FormId", formId);
                    cmdField.Parameters.AddWithValue("@Label", f.Label);
                    cmdField.Parameters.AddWithValue("@SelectedOption", (object?)f.SelectedOption ?? DBNull.Value);
                    cmdField.Parameters.AddWithValue("@IsRequired", f.IsRequired);
                    cmdField.Parameters.AddWithValue("@SortOrder", i);
                    cmdField.Parameters.AddWithValue("@IsActive", f.IsActive);
                    cmdField.Parameters.AddWithValue("@IsDeleted", f.IsDeleted);

                    await cmdField.ExecuteNonQueryAsync();
                }

                await tran.CommitAsync();
                return formId;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<FormListItemVM>> GetAllFormsAsync()
        {
            var list = new List<FormListItemVM>();
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var cmd = new SqlCommand("sp_GetAllForms", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(new FormListItemVM
                {
                    Id = rdr.GetInt32(0),
                    Title = rdr.GetString(1),
                    CreatedAt = rdr.GetDateTime(2),
                    // Optional: map UpdatedAt if you extend FormListItemVM
                    // UpdatedAt = rdr.IsDBNull(3) ? null : rdr.GetDateTime(3)
                });
            }

            return list;
        }

        public async Task<Form?> GetFormWithFieldsAsync(int id)
        {
            using var conn = new SqlConnection(_conn);
            await conn.OpenAsync();

            var cmd = new SqlCommand("sp_GetFormWithFields", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@FormId", id);

            using var rdr = await cmd.ExecuteReaderAsync();
            Form? form = null;

            while (await rdr.ReadAsync())
            {
                if (form == null)
                {
                    form = new Form
                    {
                        Id = rdr.GetInt32(0),
                        Title = rdr.GetString(1),
                        CreatedAt = rdr.GetDateTime(2),
                        UpdatedAt = rdr.IsDBNull(3) ? null : rdr.GetDateTime(3),
                        IsActive = rdr.GetBoolean(4),
                        Fields = new List<FormField>()
                    };
                }

                if (!rdr.IsDBNull(5))
                {
                    form.Fields.Add(new FormField
                    {
                        Id = rdr.GetInt32(5),
                        FormId = form.Id,
                        Label = rdr.GetString(6),
                        SelectedOption = rdr.IsDBNull(7) ? null : rdr.GetString(7),
                        IsRequired = rdr.GetBoolean(8),
                        SortOrder = rdr.GetInt32(9),
                        UpdatedAt = rdr.IsDBNull(10) ? null : rdr.GetDateTime(10),
                        IsActive = rdr.GetBoolean(11)
                    });
                }
            }

            return form;
        }
    }
}
