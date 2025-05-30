USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentInvoiceRiderManualMatches]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentInvoiceRiderManualMatches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WarehouseCode] [nvarchar](50) NOT NULL,
	[RiderInvoiceNumber] [nvarchar](50) NOT NULL,
	[InvoiceNo] [nvarchar](50) NOT NULL,
	[Packages] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentInvoiceRiderManualMatches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ON 

INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (56, N'30557199', N'7822432', N'R2219094', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (57, N'30554258', N'7796395', N'R2219095', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (59, N'30557559', N'276042', N'1983767', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (60, N'30557570', N'276044', N'1983767', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (62, N'30557559', N'276042', N'1983767', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (63, N'30557570', N'276044', N'1983767', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (64, N'33702755', N'7822432', N'7822432', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (65, N'30557559', N'276042', N'276042', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (66, N'30557570', N'276044', N'276044', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (67, N'33702755', N'7822432', N'8251314666', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (68, N'33702755', N'7822432', N'7822432', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (69, N'33702755', N'7822432', N'8251314666', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (70, N'33702755', N'7822432', N'7822432', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (72, N'30543816', N'S81288', N'S81288', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (74, N'33702755', N'7822432', N'8251314666', -1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (75, N'30543816', N'S81288', N'81288', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (76, N'33702755', N'7822432', N'81288', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (77, N'SMW1529415', N'21-12643', N'2141-12643', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (78, N'SMW1534959', N'PSI-003713741', N'PSI-0003713741', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (79, N'SMW1536140', N'S009279108001', N'S009279108.001', 2)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (80, N'SMW1548025', N'100273', N'147982', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (81, N'SMW1548025', N'100273', N'147982', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (82, N'SMW1553623', N'49647328-00N', N'49647328-00N', -1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (83, N'SMW1553623', N'49647328-00N', N'49554826-00N', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (84, N'SMW1565758', N'19428', N'INV/SX/2021/019428', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (85, N'SMW1576946', N'O11-0004157-01', N'011-0004157-01', 3)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (87, N'SMW1596639', N'S009596282001', N'S009596282.001', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (88, N'SMW1596639', N'S009597143001', N'S009597143.001', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (91, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (94, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (95, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (98, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (99, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (102, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (103, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (106, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (107, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (110, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (111, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (114, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (115, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (118, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (119, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (122, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (123, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (126, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (127, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (130, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (131, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (134, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (135, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (138, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (139, N'WR4227US', N'203623', N'203623', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (142, N'WR6952US', N'107012', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (152, N'WR6588US', N'M0079512', N'M0279512', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (153, N'WR6628US', N'S009597143002', N'S009597143.002', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (154, N'WR6952US', N'0093478493', N'0093478493', 4)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (155, N'WR4326US', N'4132467067', N'4132467067', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (156, N'WR4326US', N'4132467066', N'4132467066', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (157, N'WR4326US', N'4132467070', N'4132467070', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (158, N'WR4326US', N'4132467069', N'4132467069', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (159, N'WR4326US', N'4132467068', N'4132467068', 0)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (164, N'WR15650US', N'ARINV339456', N'ARINV-339456', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (165, N'WR15684US', N'ARINV339456', N'ARINV-339456', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (166, N'WR15714US', N'S009737271001', N'S009737271.001', 2)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (167, N'WR15779US', N'ARINV339456', N'8833743', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (170, N'WR12698US', N'O11-0030929-01', N'011-0030929-01', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (171, N'WR17084US', N'O11-0031059-01', N'011-0031059-01', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (172, N'WR47102US', N'94077243', N'0094077243', 3)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (175, N'WR47102US', N'0094077243', N'0094077243', 2)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (179, N'WR50556US', N'144320', N'00144320', 5)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (180, N'WR50433US', N'144321', N'00144321', 2)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (181, N'WR48958US', N'144278', N'00144278', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (182, N'WR54289US', N'76799193', N'500484768', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (183, N'WR54488US', N'144583', N'00144583', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (184, N'WR53864US', N'7222022', N'10119315', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (188, N'WR54289US', N'500484768', N'500484768', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (189, N'WR54488US', N'00144583', N'00144583', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (190, N'WR53864US', N'10119315', N'10119315', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (192, N'Marks', N'019093', N'019083', 28)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (193, N'Marks', N'021108', N'021108', 12)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (194, N'Marks', N'021197', N'021197', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (195, N'11033543', N'OT148438', N'0T148438', 10)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (204, N'11043762', N'S010112400001', N'S010112400.001', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (205, N'11043762', N'S010112401001', N'S010112401.001', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (206, N'11043762', N'S010165084001', N'So10165084.001', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (207, N'11043762', N'S010165086001', N'S010165086.001', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (222, N'11057809', N'204453', N'204453', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (223, N'11057809', N'204454', N'204454', 1)
INSERT [dbo].[ShipmentInvoiceRiderManualMatches] ([Id], [WarehouseCode], [RiderInvoiceNumber], [InvoiceNo], [Packages]) VALUES (224, N'11501548', N'S010496440001', N'S010496440.001', 1)
SET IDENTITY_INSERT [dbo].[ShipmentInvoiceRiderManualMatches] OFF
GO
