USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EmailMappingRexExs]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailMappingRexExs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailMappingId] [int] NOT NULL,
	[ReplacementRegex] [nvarchar](50) NOT NULL,
	[ReplacementValue] [nvarchar](50) NULL,
 CONSTRAINT [PK_EmailMappingRexExs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EmailMappingRexExs]  WITH CHECK ADD  CONSTRAINT [FK_EmailMappingRexExs_EmailMapping] FOREIGN KEY([EmailMappingId])
REFERENCES [dbo].[EmailMapping] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailMappingRexExs] CHECK CONSTRAINT [FK_EmailMappingRexExs_EmailMapping]
GO
