USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[WeightCalculationMethods]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeightCalculationMethods](
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_WeightCalculationMethods] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[WeightCalculationMethods] ([Name]) VALUES (N'MinimumWeight')
INSERT [dbo].[WeightCalculationMethods] ([Name]) VALUES (N'OneOrMore')
INSERT [dbo].[WeightCalculationMethods] ([Name]) VALUES (N'Value')
INSERT [dbo].[WeightCalculationMethods] ([Name]) VALUES (N'WeightEqualQuantity')
GO
