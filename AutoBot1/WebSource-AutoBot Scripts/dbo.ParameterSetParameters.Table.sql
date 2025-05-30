USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ParameterSetParameters]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParameterSetParameters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParameterSetId] [int] NOT NULL,
	[ParameterId] [int] NOT NULL,
 CONSTRAINT [PK_ParameterSetParameters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ParameterSetParameters]  WITH CHECK ADD  CONSTRAINT [FK_ParameterSetParameters_Parameters] FOREIGN KEY([ParameterId])
REFERENCES [dbo].[Parameters] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ParameterSetParameters] CHECK CONSTRAINT [FK_ParameterSetParameters_Parameters]
GO
ALTER TABLE [dbo].[ParameterSetParameters]  WITH CHECK ADD  CONSTRAINT [FK_ParameterSetParameters_ParameterSet] FOREIGN KEY([ParameterSetId])
REFERENCES [dbo].[ParameterSet] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ParameterSetParameters] CHECK CONSTRAINT [FK_ParameterSetParameters_ParameterSet]
GO
