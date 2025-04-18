USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AdjustmentComments]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdjustmentComments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Comments] [nvarchar](50) NOT NULL,
	[DutyFreePaid] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AdjustmentComments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AdjustmentComments] ON 

INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (1, N'CHARITY', N'Duty Paid')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (2, N'Cost of Warranty', N'Duty Free')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (3, N'CYCLE COUNT ADJUSTMENT', N'Duty Free')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (5, N'DONATION', N'Duty Paid')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (6, N'Error in System - Negative on Hand Report', N'Duty Free')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (8, N'Incorrect Internal Code Used', N'Duty Free')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (9, N'LOST/ STOLEN', N'Duty Paid')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (10, N'Office Expenses', N'Duty Paid')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (11, N'PROMOTION', N'Duty Paid')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (12, N'Sample/s', N'Duty Paid')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (13, N'Shop / Office / Warehouse Expenses', N'Duty Paid')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (14, N'SPONSORSHIP', N'Duty Paid')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (15, N'VEHICLE EXPENSE', N'Duty Paid')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (20, N'Warranty  Export for replacement ', N'Duty Free')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (21, N'WARRANTY ', N'Duty Free')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (22, N'Written Off - Damage Item', N'Duty Free')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (23, N'Written Off - Expired item ', N'Duty Free')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (24, N'DAMAGED ', N'Duty Free')
INSERT [dbo].[AdjustmentComments] ([Id], [Comments], [DutyFreePaid]) VALUES (25, N'Expired / Obsolete Goods', N'Duty Free')
SET IDENTITY_INSERT [dbo].[AdjustmentComments] OFF
GO
