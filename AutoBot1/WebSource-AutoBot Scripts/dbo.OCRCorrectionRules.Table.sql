USE [WebSource-AutoBot]
GO

/****** Object:  Table [dbo].[OCRCorrectionRules]    Script Date: 5/25/2025 9:01:19 PM ******/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OCRCorrectionRules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceType] [nvarchar](100) NOT NULL,
	[IssuePattern] [nvarchar](500) NOT NULL,
	[CorrectionRegex] [nvarchar](1000) NOT NULL,
	[ReplacementPattern] [nvarchar](1000) NOT NULL,
	[Priority] [int] NOT NULL DEFAULT 1,
	[IsActive] [bit] NOT NULL DEFAULT 1,
	[Description] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL DEFAULT GETDATE(),
	[LastModifiedDate] [datetime] NULL,
	[SuccessCount] [int] NOT NULL DEFAULT 0,
	[FailureCount] [int] NOT NULL DEFAULT 0,
	[TestCases] [nvarchar](max) NULL,
	[AppliesTo] [nvarchar](100) NOT NULL DEFAULT 'All',
	[FieldName] [nvarchar](100) NULL,
	[ValidationRule] [nvarchar](500) NULL,
 CONSTRAINT [PK_OCRCorrectionRules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- Insert initial correction rules for common issues
INSERT INTO [dbo].[OCRCorrectionRules] 
([InvoiceType], [IssuePattern], [CorrectionRegex], [ReplacementPattern], [Priority], [Description], [AppliesTo], [FieldName])
VALUES 
('Amazon', 'Missing shipping charges', '(?i)shipping.*?(\$[\d,]+\.?\d*)', '$1', 1, 'Extract shipping charges from Amazon invoices', 'TotalOtherCost', 'Shipping'),
('Amazon', 'Missing tax amounts', '(?i)tax.*?(\$[\d,]+\.?\d*)', '$1', 1, 'Extract tax amounts from Amazon invoices', 'TotalOtherCost', 'Tax'),
('TEMU', 'Coupon deduction not captured', '(?i)coupon.*?-?\$?([\d,]+\.?\d*)', '-$1', 1, 'Extract coupon deductions from TEMU invoices', 'TotalDeduction', 'Coupon'),
('TEMU', 'Shipping fee missing', '(?i)shipping.*?fee.*?(\$[\d,]+\.?\d*)', '$1', 1, 'Extract shipping fees from TEMU invoices', 'TotalOtherCost', 'Shipping'),
('General', 'Currency symbol confusion', '[$](\d+[,.]?\d*)', '$1', 2, 'Remove currency symbols for numeric parsing', 'All', 'Amount'),
('General', 'Comma in numbers', '(\d+),(\d{3})', '$1$2', 3, 'Remove commas from large numbers', 'All', 'Amount')
GO
