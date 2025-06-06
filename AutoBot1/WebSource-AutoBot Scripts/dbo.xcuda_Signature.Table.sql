USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Signature]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Signature](
	[Date] [nvarchar](max) NULL,
	[Signature_Id] [int] NOT NULL,
	[Transit_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Signature] PRIMARY KEY CLUSTERED 
(
	[Signature_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Signature]  WITH NOCHECK ADD  CONSTRAINT [FK_Transit_Signature] FOREIGN KEY([Transit_Id])
REFERENCES [dbo].[xcuda_Transit] ([Transit_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Signature] CHECK CONSTRAINT [FK_Transit_Signature]
GO
