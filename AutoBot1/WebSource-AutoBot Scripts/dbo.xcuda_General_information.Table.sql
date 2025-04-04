USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_General_information]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_General_information](
	[Value_details] [nvarchar](20) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
	[CAP] [nvarchar](20) NULL,
	[Additional_information] [nvarchar](20) NULL,
	[Comments_free_text] [nvarchar](255) NULL,
 CONSTRAINT [PK_xcuda_General_information_1] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_General_information] ([Value_details], [ASYCUDA_Id], [CAP], [Additional_information], [Comments_free_text]) VALUES (NULL, 48427, NULL, NULL, N'EffectiveAssessmentDate:Jul-15-2024')
INSERT [dbo].[xcuda_General_information] ([Value_details], [ASYCUDA_Id], [CAP], [Additional_information], [Comments_free_text]) VALUES (NULL, 48428, NULL, NULL, N'EffectiveAssessmentDate:Jul-15-2024')
INSERT [dbo].[xcuda_General_information] ([Value_details], [ASYCUDA_Id], [CAP], [Additional_information], [Comments_free_text]) VALUES (NULL, 48429, NULL, NULL, N'EffectiveAssessmentDate:Jul-15-2024')
GO
ALTER TABLE [dbo].[xcuda_General_information]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_General_information] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_General_information] CHECK CONSTRAINT [FK_ASYCUDA_General_information]
GO
