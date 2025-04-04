USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Consignees]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Consignees](
	[ConsigneeName] [nvarchar](100) NOT NULL,
	[ConsigneeCode] [nvarchar](100) NULL,
	[Address] [nvarchar](300) NULL,
	[CountryCode] [nvarchar](3) NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_Consignees_1] PRIMARY KEY CLUSTERED 
(
	[ConsigneeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'AARON S WILSON', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'AARON WILSON', NULL, NULL, NULL, 3)
GO
