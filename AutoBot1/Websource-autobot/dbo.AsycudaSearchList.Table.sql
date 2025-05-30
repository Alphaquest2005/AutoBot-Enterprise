USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaSearchList]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsycudaSearchList](
	[Year] [smallint] NOT NULL,
	[Office] [nvarchar](50) NOT NULL,
	[Declarant] [nvarchar](100) NOT NULL,
	[Decl_Code] [float] NOT NULL,
	[Ref_Number] [nvarchar](50) NOT NULL,
	[Reg_Ser] [nvarchar](50) NOT NULL,
	[Reg_Nber] [int] NOT NULL,
	[Reg_Date] [date] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Gen_Proc] [tinyint] NOT NULL,
	[Items] [smallint] NOT NULL,
	[Exporter] [nvarchar](200) NULL,
	[Rcp_Serial] [nvarchar](50) NULL,
	[Reciept] [int] NULL,
	[Rcp_Date] [date] NULL,
	[Consignee] [nvarchar](150) NULL,
	[Consg_Code] [float] NULL,
	[Total_Taxes] [float] NOT NULL,
	[Warehouse_Account] [float] NULL,
	[Ast_Ser] [nvarchar](50) NOT NULL,
	[Ast] [int] NOT NULL,
	[Ast_Date] [date] NOT NULL,
	[Color] [nvarchar](50) NULL,
 CONSTRAINT [PK_AsycudaSearchList] PRIMARY KEY CLUSTERED 
(
	[Year] ASC,
	[Office] ASC,
	[Reg_Nber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
