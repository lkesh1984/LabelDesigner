CREATE TABLE [dbo].[TemplateVersion]
(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	SysId UNIQUEIDENTIFIER NOT NULL,
	TemplateId INT NOT NULL,
	IsActive BIT NOT NULL DEFAULT 1,
	CreatedBy INT NOT NULL,
	CreatedOn DATETIME NOT NULL,
	ModifiedBy INT,
	ModifiedOn DATETIME,
	CONSTRAINT [fk_TemplateVersion_Templates_TemplateId] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Templates]([Id])
)