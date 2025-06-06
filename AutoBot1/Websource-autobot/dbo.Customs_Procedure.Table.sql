USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Customs_Procedure]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customs_Procedure](
	[Document_TypeId] [int] NOT NULL,
	[Customs_ProcedureId] [int] IDENTITY(1,1) NOT NULL,
	[Extended_customs_procedure] [nvarchar](5) NOT NULL,
	[National_customs_procedure] [nvarchar](5) NOT NULL,
	[CustomsProcedure]  AS (([Extended_customs_procedure]+'-')+[National_customs_procedure]) PERSISTED NOT NULL,
	[IsObsolete] [bit] NOT NULL,
	[IsPaid] [bit] NOT NULL,
	[BondTypeId] [int] NULL,
	[Stock] [bit] NOT NULL,
	[Discrepancy] [bit] NOT NULL,
	[Adjustment] [bit] NOT NULL,
	[Sales] [bit] NOT NULL,
	[CustomsOperationId] [int] NOT NULL,
	[SubmitToCustoms] [bit] NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[ExportSupportingEntryData] [bit] NOT NULL,
 CONSTRAINT [PK_Customs_Procedure] PRIMARY KEY CLUSTERED 
(
	[Customs_ProcedureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Customs_Procedure] ON 

INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 66, N'4000', N'000', 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 101, N'4500', N'SPO', 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 102, N'4700', N'SPO', 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 103, N'4000', N'800', 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 104, N'4500', N'T07', 0, 0, NULL, 0, 0, 0, 0, 1, 0, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (35, 106, N'5100', N'000', 0, 0, NULL, 0, 0, 0, 0, 1, 0, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (40, 126, N'1000', N'000', 0, 0, 1, 0, 0, 0, 0, 4, 0, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 140, N'4300', N'SPO', 0, 0, NULL, 0, 0, 0, 0, 1, 0, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 143, N'4000', N'802', 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 188, N'4500', N'H19', 0, 0, NULL, 0, 0, 0, 0, 1, 0, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 189, N'4000', N'827', 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (4, 195, N'4500', N'CH1', 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (41, 198, N'8300', N'000', 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (42, 201, N'8300', N'000', 0, 1, 1, 0, 0, 0, 0, 4, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (44, 203, N'4000', N'000', 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (44, 204, N'4000', N'800', 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (43, 205, N'4200', N'000', 0, 1, 1, 0, 0, 0, 0, 1, 1, 1, 0)
INSERT [dbo].[Customs_Procedure] ([Document_TypeId], [Customs_ProcedureId], [Extended_customs_procedure], [National_customs_procedure], [IsObsolete], [IsPaid], [BondTypeId], [Stock], [Discrepancy], [Adjustment], [Sales], [CustomsOperationId], [SubmitToCustoms], [IsDefault], [ExportSupportingEntryData]) VALUES (43, 206, N'4200', N'800', 0, 0, 1, 0, 0, 0, 0, 1, 1, 0, 0)
SET IDENTITY_INSERT [dbo].[Customs_Procedure] OFF
GO
ALTER TABLE [dbo].[Customs_Procedure] ADD  CONSTRAINT [DF_Customs_Procedure_IsObsolete]  DEFAULT ((0)) FOR [IsObsolete]
GO
ALTER TABLE [dbo].[Customs_Procedure] ADD  CONSTRAINT [DF_Customs_Procedure_IsPaid]  DEFAULT ((0)) FOR [IsPaid]
GO
ALTER TABLE [dbo].[Customs_Procedure] ADD  CONSTRAINT [DF_Customs_Procedure_Stock]  DEFAULT ((0)) FOR [Stock]
GO
ALTER TABLE [dbo].[Customs_Procedure] ADD  CONSTRAINT [DF_Customs_Procedure_Discrepancy]  DEFAULT ((0)) FOR [Discrepancy]
GO
ALTER TABLE [dbo].[Customs_Procedure] ADD  CONSTRAINT [DF_Customs_Procedure_Adjustment]  DEFAULT ((0)) FOR [Adjustment]
GO
ALTER TABLE [dbo].[Customs_Procedure] ADD  CONSTRAINT [DF_Customs_Procedure_Sales]  DEFAULT ((0)) FOR [Sales]
GO
ALTER TABLE [dbo].[Customs_Procedure] ADD  CONSTRAINT [DF_Customs_Procedure_SubmitToCustoms]  DEFAULT ((0)) FOR [SubmitToCustoms]
GO
ALTER TABLE [dbo].[Customs_Procedure] ADD  CONSTRAINT [DF_Customs_Procedure_IsDefault]  DEFAULT ((0)) FOR [IsDefault]
GO
ALTER TABLE [dbo].[Customs_Procedure] ADD  CONSTRAINT [DF_Customs_Procedure_ExportSupportingEntryData]  DEFAULT ((0)) FOR [ExportSupportingEntryData]
GO
ALTER TABLE [dbo].[Customs_Procedure]  WITH CHECK ADD  CONSTRAINT [FK_Customs_Procedure_BondTypes] FOREIGN KEY([BondTypeId])
REFERENCES [dbo].[BondTypes] ([Id])
GO
ALTER TABLE [dbo].[Customs_Procedure] CHECK CONSTRAINT [FK_Customs_Procedure_BondTypes]
GO
ALTER TABLE [dbo].[Customs_Procedure]  WITH CHECK ADD  CONSTRAINT [FK_Customs_Procedure_CustomsOperations] FOREIGN KEY([CustomsOperationId])
REFERENCES [dbo].[CustomsOperations] ([Id])
GO
ALTER TABLE [dbo].[Customs_Procedure] CHECK CONSTRAINT [FK_Customs_Procedure_CustomsOperations]
GO
ALTER TABLE [dbo].[Customs_Procedure]  WITH CHECK ADD  CONSTRAINT [FK_Customs_Procedure_Document_Type] FOREIGN KEY([Document_TypeId])
REFERENCES [dbo].[Document_Type] ([Document_TypeId])
GO
ALTER TABLE [dbo].[Customs_Procedure] CHECK CONSTRAINT [FK_Customs_Procedure_Document_Type]
GO
