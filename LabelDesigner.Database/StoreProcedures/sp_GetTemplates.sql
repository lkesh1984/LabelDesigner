CREATE PROCEDURE sp_GetTemplates(
    @TemplateId UNIQUEIDENTIFIER = NULL
)
AS
BEGIN
    SELECT 
    T.SysId,
    T.[Name],
    T.CreatedOn,
    T.CreatedBy,
    T.ModifiedOn,
    T.ModifiedBy,
    T.CanvasSettings,
    T.Widgets,
    T.TemplateSvg,
    TV.SysId AS TemplateVersionId
    FROM Templates T WITH (NOLOCK)
    JOIN TemplateVersion TV ON T.Id = TV.TemplateId
    WHERE (@TemplateId IS NULL OR T.SysId = @TemplateId)
    AND TV.IsActive = 1
END