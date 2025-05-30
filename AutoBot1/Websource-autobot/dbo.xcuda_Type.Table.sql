USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Type]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Type](
	[Type_of_declaration] [nvarchar](20) NULL,
	[Declaration_gen_procedure_code] [nvarchar](20) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Type] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Type]  WITH NOCHECK ADD  CONSTRAINT [FK_Identification_Type] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Identification] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Type] CHECK CONSTRAINT [FK_Identification_Type]
GO
