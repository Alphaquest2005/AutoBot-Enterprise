USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EmptyFullCodes]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmptyFullCodes](
	[EmptyFullCode] [nvarchar](20) NULL,
	[EmptyFullDescription] [nvarchar](100) NULL,
	[EmptyFullCodeId] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_EmptyFullCodes] PRIMARY KEY CLUSTERED 
(
	[EmptyFullCodeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
