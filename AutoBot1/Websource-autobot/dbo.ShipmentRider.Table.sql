USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentRider]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentRider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ETA] [date] NOT NULL,
	[DocumentDate] [date] NOT NULL,
	[EmailId] [nvarchar](255) NULL,
	[SourceFile] [nvarchar](max) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[FileTypeId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentRider_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
