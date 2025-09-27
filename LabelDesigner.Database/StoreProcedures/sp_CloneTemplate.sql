CREATE PROCEDURE [dbo].[sp_CloneTemplate](
	@TemplateSysId UNIQUEIDENTIFIER
)
AS
BEGIN

	 INSERT INTO Templates
         (SysId,
		 [Name],
         CreatedBy,
         ModifiedOn,
         ModifiedBy,
         CanvasSettings,
         Widgets,
         TemplateSvg)
     SELECT 
	 NEWID(),
        [Name],
        CreatedBy,
        GETDATE(),
        ModifiedBy,
        CanvasSettings,
        Widgets,
        TemplateSvg
        FROM Templates 
        WHERE SysId = @TemplateSysId

    DECLARE @TemplateId INT;
    SET @TemplateId = SCOPE_IDENTITY();

    INSERT INTO [dbo].[TemplateVersion]
        (SysId, TemplateId, CreatedBy, CreatedOn)
    VALUES
        (NEWID(), @TemplateId, 1, GETDATE())
        
END