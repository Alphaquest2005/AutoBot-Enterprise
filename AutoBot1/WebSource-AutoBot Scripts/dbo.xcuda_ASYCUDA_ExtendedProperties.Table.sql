USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_ASYCUDA_ExtendedProperties]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties](
	[ASYCUDA_Id] [int] NOT NULL,
	[AsycudaDocumentSetId] [int] NOT NULL,
	[FileNumber] [int] NULL,
	[IsManuallyAssessed] [bit] NOT NULL,
	[CNumber] [nvarchar](50) NULL,
	[RegistrationDate] [datetime2](7) NULL,
	[ReferenceNumber] [nvarchar](50) NULL,
	[Customs_ProcedureId] [int] NULL,
	[Description] [nvarchar](255) NULL,
	[ExportTemplateId] [int] NULL,
	[BLNumber] [nvarchar](50) NULL,
	[AutoUpdate] [bit] NOT NULL,
	[EffectiveRegistrationDate] [datetime2](7) NULL,
	[ApportionMethod] [int] NULL,
	[DoNotAllocate] [bit] NOT NULL,
	[ImportComplete] [bit] NOT NULL,
	[DocumentLines] [int] NULL,
	[Cancelled] [bit] NOT NULL,
	[TotalWeight] [float] NULL,
	[TotalFreight] [float] NULL,
	[TotalInternalFreight] [float] NULL,
	[TotalPackages] [int] NULL,
	[EffectiveExpiryDate] [datetime2](7) NULL,
	[SourceFileName] [nvarchar](500) NULL,
 CONSTRAINT [PK_xcuda_ASYCUDA_xcuda_ExtendedProperty] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IDX_1235]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [IDX_1235] ON [dbo].[xcuda_ASYCUDA_ExtendedProperties]
(
	[Customs_ProcedureId] ASC
)
INCLUDE([AsycudaDocumentSetId],[IsManuallyAssessed],[RegistrationDate],[EffectiveRegistrationDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_Cancelled_Customs_ProcedureId]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [IDX_Cancelled_Customs_ProcedureId] ON [dbo].[xcuda_ASYCUDA_ExtendedProperties]
(
	[Cancelled] ASC
)
INCLUDE([Customs_ProcedureId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_ASYCUDA_ExtendedProperties_121_120]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_ASYCUDA_ExtendedProperties_121_120] ON [dbo].[xcuda_ASYCUDA_ExtendedProperties]
(
	[AsycudaDocumentSetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_ASYCUDA_ExtendedProperties_15_14]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_ASYCUDA_ExtendedProperties_15_14] ON [dbo].[xcuda_ASYCUDA_ExtendedProperties]
(
	[ImportComplete] ASC
)
INCLUDE([AsycudaDocumentSetId],[Customs_ProcedureId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_ASYCUDA_ExtendedProperties_166_165]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_ASYCUDA_ExtendedProperties_166_165] ON [dbo].[xcuda_ASYCUDA_ExtendedProperties]
(
	[Customs_ProcedureId] ASC
)
INCLUDE([AsycudaDocumentSetId],[IsManuallyAssessed],[RegistrationDate],[EffectiveRegistrationDate],[Cancelled]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] ADD  CONSTRAINT [DF_xcuda_ASYCUDA_ExtendedProperties_IsManuallyAssessed]  DEFAULT ((0)) FOR [IsManuallyAssessed]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] ADD  CONSTRAINT [DF_xcuda_ASYCUDA_ExtendedProperties_AutoUpdate]  DEFAULT ((1)) FOR [AutoUpdate]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] ADD  CONSTRAINT [DF_xcuda_ASYCUDA_ExtendedProperties_DoNotAllocate]  DEFAULT ((0)) FOR [DoNotAllocate]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] ADD  CONSTRAINT [DF_xcuda_ASYCUDA_ExtendedProperties_Cancelled]  DEFAULT ((0)) FOR [Cancelled]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_AsycudaDocumentSet] FOREIGN KEY([AsycudaDocumentSetId])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] CHECK CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_AsycudaDocumentSet]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_Customs_Procedure] FOREIGN KEY([Customs_ProcedureId])
REFERENCES [dbo].[Customs_Procedure] ([Customs_ProcedureId])
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] CHECK CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_Customs_Procedure]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_ExportTemplate] FOREIGN KEY([ExportTemplateId])
REFERENCES [dbo].[ExportTemplate] ([ExportTemplateId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] CHECK CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_ExportTemplate]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_ExtendedProperty_inherits_xcuda_ASYCUDA] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] CHECK CONSTRAINT [FK_xcuda_ExtendedProperty_inherits_xcuda_ASYCUDA]
GO
