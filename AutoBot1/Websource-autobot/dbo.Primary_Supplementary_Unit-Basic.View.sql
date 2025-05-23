USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Primary_Supplementary_Unit-Basic]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Primary_Supplementary_Unit-Basic]
with schemabinding
AS
SELECT  Tarification_Id, Suppplementary_unit_quantity
FROM    dbo.xcuda_Supplementary_unit 
WHERE (IsFirstRow = 1)
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF
GO
/****** Object:  Index [IDX_V2]    Script Date: 3/27/2025 1:48:24 AM ******/
CREATE UNIQUE CLUSTERED INDEX [IDX_V2] ON [dbo].[Primary_Supplementary_Unit-Basic]
(
	[Tarification_Id] ASC,
	[Suppplementary_unit_quantity] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
