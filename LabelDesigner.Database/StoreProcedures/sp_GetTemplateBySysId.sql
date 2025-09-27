CREATE PROCEDURE sp_GetTemplate
    @SysId UNIQUEIDENTIFIER
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
    WHERE T.SysId = @SysId
    AND TV.IsActive = 1
END
