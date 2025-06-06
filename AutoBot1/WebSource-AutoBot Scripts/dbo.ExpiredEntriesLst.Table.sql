USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ExpiredEntriesLst]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExpiredEntriesLst](
	[Office] [nvarchar](50) NOT NULL,
	[GeneralProcedure] [nvarchar](1) NOT NULL,
	[RegistrationSerial] [nvarchar](1) NOT NULL,
	[RegistrationNumber] [nvarchar](8) NOT NULL,
	[RegistrationDate] [nvarchar](50) NOT NULL,
	[AssessmentSerial] [nvarchar](1) NOT NULL,
	[AssessmentNumber] [nvarchar](8) NOT NULL,
	[AssessmentDate] [nvarchar](50) NOT NULL,
	[DeclarantCode] [nvarchar](50) NOT NULL,
	[DeclarantReference] [nvarchar](50) NOT NULL,
	[Exporter] [nvarchar](50) NULL,
	[Consignee] [nvarchar](50) NULL,
	[Expiration] [nvarchar](50) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_ExpiredEntriesLst_1] PRIMARY KEY CLUSTERED 
(
	[Office] ASC,
	[RegistrationNumber] ASC,
	[RegistrationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ExpiredEntriesLst] ADD  CONSTRAINT [DF_ExpiredEntriesLst_ApplicationSettingsId]  DEFAULT ((5)) FOR [ApplicationSettingsId]
GO
