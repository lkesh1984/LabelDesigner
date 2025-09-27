using LabelDesigner.Shared.Widgets;

namespace LabelDesigner.Data.Interfaces
{
    public interface ITemplateRepository
    {
        Task<IEnumerable<TemplateNew>> GetTemplatesAsync(Guid? templateId = null);
        Task<TemplateNew?> GetTemplateAsync(Guid templateSysId);

        Task CreateTemplateAsync(TemplateNew template);
        Task<bool> UpdateTemplateAsync(TemplateNew template);
        Task<bool> DeleteTemplateAsync(Guid templateSysId);
        Task<bool> CloneTemplateAsync(Guid templateSysId);
    }
}
