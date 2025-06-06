USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[CurrencyRates]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CurrencyRates](
	[CurrencyCode] [nvarchar](3) NOT NULL,
	[Rate] [float] NOT NULL,
 CONSTRAINT [PK_CurrencyRates] PRIMARY KEY CLUSTERED 
(
	[CurrencyCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[CurrencyRates] ([CurrencyCode], [Rate]) VALUES (N'CAD', 1.97998)
INSERT [dbo].[CurrencyRates] ([CurrencyCode], [Rate]) VALUES (N'CHF', 3.00284)
INSERT [dbo].[CurrencyRates] ([CurrencyCode], [Rate]) VALUES (N'EUR', 3.19937)
INSERT [dbo].[CurrencyRates] ([CurrencyCode], [Rate]) VALUES (N'GBP', 3.55199)
INSERT [dbo].[CurrencyRates] ([CurrencyCode], [Rate]) VALUES (N'TTD', 0.40241)
INSERT [dbo].[CurrencyRates] ([CurrencyCode], [Rate]) VALUES (N'US', 2.7169)
INSERT [dbo].[CurrencyRates] ([CurrencyCode], [Rate]) VALUES (N'USD', 2.7169)
INSERT [dbo].[CurrencyRates] ([CurrencyCode], [Rate]) VALUES (N'XCD', 1)
GO
