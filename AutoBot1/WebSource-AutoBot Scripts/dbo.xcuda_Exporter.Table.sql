USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Exporter]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Exporter](
	[Exporter_name] [nvarchar](255) NULL,
	[Traders_Id] [int] NOT NULL,
	[Exporter_code] [nvarchar](20) NULL,
 CONSTRAINT [PK_xcuda_Exporter_1] PRIMARY KEY CLUSTERED 
(
	[Traders_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Exporter]  WITH NOCHECK ADD  CONSTRAINT [FK_Traders_Exporter] FOREIGN KEY([Traders_Id])
REFERENCES [dbo].[xcuda_Traders] ([Traders_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Exporter] CHECK CONSTRAINT [FK_Traders_Exporter]
GO
