USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentConsignee]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentConsignee](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConsigneeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_ShipmentConsignee] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
