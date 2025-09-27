CREATE PROCEDURE sp_DeleteTemplate
    @SysId UNIQUEIDENTIFIER
AS
BEGIN

    DECLARE @TemplateId INT;
    SET @TemplateId = (SELECT Id FROM Templates WHERE SysId = @SysId)

    DELETE FROM TemplateVersion WHERE TemplateId = @TemplateId
    
    DELETE FROM Templates WHERE SysId = @SysId
END
