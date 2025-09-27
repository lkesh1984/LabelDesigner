CREATE TABLE Templates
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SysId UNIQUEIDENTIFIER UNIQUE,
    Name NVARCHAR(200),
    CreatedOn DATETIME2 DEFAULT SYSDATETIME(),
    CreatedBy INT,
    ModifiedOn DATETIME2 DEFAULT SYSDATETIME(),
    ModifiedBy INT,
    CanvasSettings NVARCHAR(MAX),  -- JSON
    Widgets NVARCHAR(MAX),          -- JSON
    TemplateSvg NVARCHAR(MAX)
)