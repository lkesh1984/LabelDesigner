CREATE PROCEDURE sp_CreateTemplate
    @SysId UNIQUEIDENTIFIER,
    @Name NVARCHAR(200),
    @CreatedBy INT,
    @ModifiedOn DATETIME2,
    @ModifiedBy INT,
    @CanvasSettings NVARCHAR(MAX),
    @Widgets NVARCHAR(MAX),
    @TemplateSvg NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Templates
        (SysId, Name, CreatedBy, ModifiedOn, ModifiedBy, CanvasSettings, Widgets, TemplateSvg)
    VALUES
        (@SysId, @Name, @CreatedBy, @ModifiedOn, @ModifiedBy, @CanvasSettings, @Widgets, @TemplateSvg)

    DECLARE @TemplateId INT;
    SET @TemplateId = SCOPE_IDENTITY();

    INSERT INTO [dbo].[TemplateVersion]
        (SysId, TemplateId, CreatedBy, CreatedOn)
    VALUES
        (NEWID(), @TemplateId, @CreatedBy, GETDATE())
END
