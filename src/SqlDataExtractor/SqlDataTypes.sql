-- Source: https://stackoverflow.com/questions/1058322/anybody-got-a-c-sharp-function-that-maps-the-sql-datatype-of-a-column-to-its-clr

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DbVsCSharpTypes]') 
AND type in (N'U'))
DROP TABLE [dbo].[DbVsCSharpTypes]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DbVsCSharpTypes](
    [DbVsCSharpTypesId] [int] IDENTITY(1,1) NOT NULL,
    [Sql2008DataType] [varchar](200) NULL,
    [CSharpDataType] [varchar](200) NULL,
    [CLRDataType] [varchar](200) NULL,
    [CLRDataTypeSqlServer] [varchar](2000) NULL,

    CONSTRAINT [PK_DbVsCSharpTypes] PRIMARY KEY CLUSTERED 
(
    [DbVsCSharpTypesId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

SET IDENTITY_INSERT [dbo].[DbVsCSharpTypes] ON;
BEGIN TRANSACTION;
INSERT INTO [dbo].[DbVsCSharpTypes]([DbVsCSharpTypesId], [Sql2008DataType], [CSharpDataType], [CLRDataType], [CLRDataTypeSqlServer])
SELECT 1, N'bigint', N'long', N'Int64, Nullable<Int64>', N'SqlInt64' UNION ALL
SELECT 2, N'binary', N'byte[]', N'Byte[]', N'SqlBytes, SqlBinary' UNION ALL
SELECT 3, N'bit', N'bool', N'Boolean, Nullable<Boolean>', N'SqlBoolean' UNION ALL
SELECT 4, N'char', N'char', NULL, NULL UNION ALL
SELECT 5, N'cursor', NULL, NULL, NULL UNION ALL
SELECT 6, N'date', N'DateTime', N'DateTime, Nullable<DateTime>', N'SqlDateTime' UNION ALL
SELECT 7, N'datetime', N'DateTime', N'DateTime, Nullable<DateTime>', N'SqlDateTime' UNION ALL
SELECT 8, N'datetime2', N'DateTime', N'DateTime, Nullable<DateTime>', N'SqlDateTime' UNION ALL
SELECT 9, N'DATETIMEOFFSET', N'DateTimeOffset', N'DateTimeOffset', N'DateTimeOffset, Nullable<DateTimeOffset>' UNION ALL
SELECT 10, N'decimal', N'decimal', N'Decimal, Nullable<Decimal>', N'SqlDecimal' UNION ALL
SELECT 11, N'float', N'double', N'Double, Nullable<Double>', N'SqlDouble' UNION ALL
SELECT 12, N'geography', NULL, NULL, N'SqlGeography is defined in Microsoft.SqlServer.Types.dll, which is installed with SQL Server and can be downloaded from the SQL Server 2008 feature pack.' UNION ALL
SELECT 13, N'geometry', NULL, NULL, N'SqlGeometry is defined in Microsoft.SqlServer.Types.dll, which is installed with SQL Server and can be downloaded from the SQL Server 2008 feature pack.' UNION ALL
SELECT 14, N'hierarchyid', NULL, NULL, N'SqlHierarchyId is defined in Microsoft.SqlServer.Types.dll, which is installed with SQL Server and can be downloaded from the SQL Server 2008 feature pack.' UNION ALL
SELECT 15, N'image', NULL, NULL, NULL UNION ALL
SELECT 16, N'int', N'int', N'Int32, Nullable<Int32>', N'SqlInt32' UNION ALL
SELECT 17, N'money', N'decimal', N'Decimal, Nullable<Decimal>', N'SqlMoney' UNION ALL
SELECT 18, N'nchar', N'string', N'String, Char[]', N'SqlChars, SqlString' UNION ALL
SELECT 19, N'ntext', NULL, NULL, NULL UNION ALL
SELECT 20, N'numeric', N'decimal', N'Decimal, Nullable<Decimal>', N'SqlDecimal' UNION ALL
SELECT 21, N'nvarchar', N'string', N'String, Char[]', N'SqlChars, SqlStrinG SQLChars is a better match for data transfer and access, and SQLString is a better match for performing String operations.' UNION ALL
SELECT 22, N'nvarchar(1), nchar(1)', N'string', N'Char, String, Char[], Nullable<char>', N'SqlChars, SqlString' UNION ALL
SELECT 23, N'real', N'single', N'Single, Nullable<Single>', N'SqlSingle' UNION ALL
SELECT 24, N'rowversion', N'byte[]', N'Byte[]', NULL UNION ALL
SELECT 25, N'smallint', N'smallint', N'Int16, Nullable<Int16>', N'SqlInt16' UNION ALL
SELECT 26, N'smallmoney', N'decimal', N'Decimal, Nullable<Decimal>', N'SqlMoney' UNION ALL
SELECT 27, N'sql_variant', N'object', N'Object', NULL UNION ALL
SELECT 28, N'table', NULL, NULL, NULL UNION ALL
SELECT 29, N'text', N'string', NULL, NULL UNION ALL
SELECT 30, N'time', N'TimeSpan', N'TimeSpan, Nullable<TimeSpan>', N'TimeSpan' UNION ALL
SELECT 31, N'timestamp', NULL, NULL, NULL UNION ALL
SELECT 32, N'tinyint', N'byte', N'Byte, Nullable<Byte>', N'SqlByte' UNION ALL
SELECT 33, N'uniqueidentifier', N'Guid', N'Guid, Nullable<Guid>', N'SqlGuidUser-defined type(UDT)The same class that is bound to the user-defined type in the same assembly or a dependent assembly.' UNION ALL
SELECT 34, N'varbinary ', N'byte[]', N'Byte[]', N'SqlBytes, SqlBinary' UNION ALL
SELECT 35, N'varbinary(1), binary(1)', N'byte', N'byte, Byte[], Nullable<byte>', N'SqlBytes, SqlBinary' UNION ALL
SELECT 36, N'varchar', NULL, NULL, NULL UNION ALL
SELECT 37, N'xml', NULL, NULL, N'SqlXml'
COMMIT;
RAISERROR (N'[dbo].[DbVsCSharpTypes]: Insert Batch: 1.....Done!', 10, 1) WITH NOWAIT;
GO

SET IDENTITY_INSERT [dbo].[DbVsCSharpTypes] OFF;
