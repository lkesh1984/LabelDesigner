using LabelDesigner.Data.Interfaces;
using LabelDesigner.Shared.Widgets;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;

public class TemplateRepository : ITemplateRepository
{
    private readonly string _connectionString;
    private readonly ILogger<TemplateRepository> _logger;

    public TemplateRepository(string connectionString, ILogger<TemplateRepository> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    /// <summary>
    /// Method to get the templates or a specific template
    /// </summary>
    /// <param name="templateId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TemplateNew>> GetTemplatesAsync(Guid? templateId = null)
    {
        var list = new List<TemplateNew>();
        try
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetTemplates", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(MapReaderToTemplate(reader));
            }

            _logger.LogInformation("Fetched {Count} templates from database", list.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching templates from database");
            throw; // Let controller handle HTTP response
        }

        return list;
    }

    public async Task<TemplateNew?> GetTemplateAsync(Guid templateSysId)
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetTemplate", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@SysId", templateSysId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (await reader.ReadAsync())
            {
                var template = MapReaderToTemplate(reader);
                _logger.LogInformation("Template {TemplateId} retrieved successfully", templateSysId);
                return template;
            }
            else
            {
                _logger.LogWarning("Template {TemplateId} not found", templateSysId);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching template {TemplateId}", templateSysId);
            throw;
        }
    }

    public async Task CreateTemplateAsync(TemplateNew template)
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_CreateTemplate", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddTemplateParameters(cmd, template);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            _logger.LogInformation("Template {TemplateId} created successfully", template.SysId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating template {TemplateId}", template.SysId);
            throw;
        }
    }

    public async Task<bool> UpdateTemplateAsync(TemplateNew template)
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_UpdateTemplate", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddTemplateParameters(cmd, template, true);

            await conn.OpenAsync();
            int affected = await cmd.ExecuteNonQueryAsync();

            if (affected > 0)
            {
                _logger.LogInformation("Template {TemplateId} updated successfully", template.SysId);
                return true;
            }
            else
            {
                _logger.LogWarning("Template {TemplateId} not found for update", template.SysId);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating template {TemplateId}", template.SysId);
            throw;
        }
    }

    public async Task<bool> DeleteTemplateAsync(Guid templateSysId)
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DeleteTemplate", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@SysId", templateSysId);

            await conn.OpenAsync();
            int affected = await cmd.ExecuteNonQueryAsync();

            if (affected > 0)
            {
                _logger.LogInformation("Template {TemplateId} deleted successfully", templateSysId);
                return true;
            }
            else
            {
                _logger.LogWarning("Template {TemplateId} not found for deletion", templateSysId);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", templateSysId);
            throw;
        }
    }

    public async Task<bool> CloneTemplateAsync(Guid templateSysId)
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_CloneTemplate", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TemplateSysId", templateSysId);

            await conn.OpenAsync();
            int affected = await cmd.ExecuteNonQueryAsync();

            if (affected > 0)
            {
                _logger.LogInformation("Template {TemplateId} cloned successfully", templateSysId);
                return true;
            }
            else
            {
                _logger.LogWarning("Template with ID {TemplateId} " + templateSysId + " is not cloned");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cloning template {TemplateId}", templateSysId);
            throw;
        }
    }

    #region Helpers

    private static void AddTemplateParameters(SqlCommand cmd, TemplateNew template, bool isUpdate = false)
    {
        cmd.Parameters.AddWithValue("@SysId", template.SysId);
        if (isUpdate)
            cmd.Parameters.AddWithValue("@VersionId", template.VersionId);
        cmd.Parameters.AddWithValue("@Name", template.Name);
        cmd.Parameters.AddWithValue("@CreatedBy", 1);
        cmd.Parameters.AddWithValue("@ModifiedOn", DateTime.Now);
        cmd.Parameters.AddWithValue("@ModifiedBy", 1);
        cmd.Parameters.AddWithValue("@CanvasSettings", JsonConvert.SerializeObject(template.CanvasSettings));
        cmd.Parameters.AddWithValue("@Widgets", template.FabricJson);
        cmd.Parameters.AddWithValue("@TemplateSvg", template.TemplateSvg);
    }

    private static TemplateNew MapReaderToTemplate(SqlDataReader reader)
    {
        return new TemplateNew
        {
            SysId = reader.GetGuid(reader.GetOrdinal("SysId")).ToString(),
            VersionId = reader.GetGuid(reader.GetOrdinal("TemplateVersionId")).ToString(),
            Name = reader.GetString(reader.GetOrdinal("Name")),
            CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
            CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy")),
            ModifiedOn = reader.GetDateTime(reader.GetOrdinal("ModifiedOn")),
            ModifiedBy = reader.GetInt32(reader.GetOrdinal("ModifiedBy")),
            CanvasSettings = reader.GetString(reader.GetOrdinal("CanvasSettings")),
            FabricJson = reader.GetString(reader.GetOrdinal("Widgets")),
            TemplateSvg = reader.GetString(reader.GetOrdinal("TemplateSvg"))
        };
    }

    #endregion
}
