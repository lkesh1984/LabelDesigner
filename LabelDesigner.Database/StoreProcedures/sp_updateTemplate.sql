CREATE PROCEDURE sp_UpdateTemplate
    @SysId UNIQUEIDENTIFIER,
    @VersionId UNIQUEIDENTIFIER,
    @Name NVARCHAR(200),
    @CreatedBy INT,
    @ModifiedOn DATETIME2,
    @ModifiedBy INT,
    @CanvasSettings NVARCHAR(MAX),
    @Widgets NVARCHAR(MAX),
    @TemplateSvg NVARCHAR(MAX)
AS
BEGIN
    UPDATE Templates
    SET
        [Name] = @Name,
        ModifiedOn = @ModifiedOn,
        ModifiedBy = @ModifiedBy,
        CanvasSettings = @CanvasSettings,
        Widgets = @Widgets,
        TemplateSvg = @TemplateSvg
    WHERE SysId = @SysId

    DECLARE @TemplateId INT;
    SET @TemplateId = (SELECT Id from Templates WHERE SysId = @SysId)

    UPDATE TemplateVersion
    SET IsActive = 0
    WHERE SysId = @VersionId

    INSERT INTO [dbo].[TemplateVersion]
        (SysId, TemplateId, CreatedBy, CreatedOn)
    VALUES
        (NEWID(), @TemplateId, @CreatedBy, GETDATE())
END
