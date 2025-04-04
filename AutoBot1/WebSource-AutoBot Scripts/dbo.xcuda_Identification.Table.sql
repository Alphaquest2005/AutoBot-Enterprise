USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Identification]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Identification](
	[ASYCUDA_Id] [int] NOT NULL,
	[Manifest_reference_number] [nvarchar](50) NULL,
 CONSTRAINT [PK_xcuda_Identification] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_Identification] ([ASYCUDA_Id], [Manifest_reference_number]) VALUES (48427, N'2024 28')
INSERT [dbo].[xcuda_Identification] ([ASYCUDA_Id], [Manifest_reference_number]) VALUES (48428, N'2024 28')
INSERT [dbo].[xcuda_Identification] ([ASYCUDA_Id], [Manifest_reference_number]) VALUES (48429, N'2024 28')
GO
ALTER TABLE [dbo].[xcuda_Identification]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Identification] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Identification] CHECK CONSTRAINT [FK_ASYCUDA_Identification]
GO
