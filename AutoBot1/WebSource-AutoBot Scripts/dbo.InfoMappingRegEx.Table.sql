USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InfoMappingRegEx]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InfoMappingRegEx](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InfoMappingId] [int] NOT NULL,
	[KeyRegX] [nvarchar](1000) NOT NULL,
	[FieldRx] [nvarchar](1000) NOT NULL,
	[KeyReplaceRx] [nvarchar](1000) NULL,
	[FieldReplaceRx] [nvarchar](1000) NULL,
	[LineRegx] [nvarchar](1000) NOT NULL,
	[KeyValue] [nvarchar](50) NULL,
	[FieldValue] [nvarchar](1000) NULL,
 CONSTRAINT [PK_InfoMappingRegEx] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[InfoMappingRegEx] ON 

INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (28, 1045, N'Manifest', N'(?<Value>[\d]{4})[\s/]+(?<Value1>[\d]+)', NULL, N'$1 $2', N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (29, 1029, N'^BL', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (30, 1027, N'Freight', N'Freight((?!Currency).)[:\s\#]+(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (31, 1026, N'Weight.*kg', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)\s?(?:Kg)?', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (32, 1030, N'Country of Origin', N'Country\sof\sOrigin[:\s\#]+(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (33, 1031, N'Lot Number', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (34, 1032, N'Item Number', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (35, 1033, N'Description', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (36, 1034, N'Invoice Number', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (37, 1035, N'Credit Invoice Number', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (38, 1037, N'Packages', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (39, 1038, N'MaxLines', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (40, 1039, N'Location of Goods', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (41, 1036, N'Total Invoices', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (42, 1028, N'Currency', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (43, 1040, N'Freight Currency', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (44, 1041, N'.*\s\d+\s+\d+\s+ \d+', N'.*\s\d+\s+\d+\s+ (?<Value>\d+)', NULL, NULL, N'.*\s\d+\s+\d+\s+ (?<Value>\d+)', N'CNumber', NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (45, 1041, N'^\d+($|\r)', N'(?<Value>\d+)', NULL, NULL, N'^\d+($|\r)', N'CNumber', NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (46, 1042, N'Office', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (47, 1024, N'Expected Entries', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (48, 1043, N'PONumber', N'PONumber[:\s\#]+(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'PONumber[:\s\#]+(?<Value>[\d]+)', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (49, 1046, N'Consignee Name', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (51, 1047, N'Consignee Code', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
INSERT [dbo].[InfoMappingRegEx] ([Id], [InfoMappingId], [KeyRegX], [FieldRx], [KeyReplaceRx], [FieldReplaceRx], [LineRegx], [KeyValue], [FieldValue]) VALUES (52, 1048, N'Consignee Address', N'(?<=[:\#]+\s)(?<Value>[a-zA-Z0-9\- :$,/\.]*)', NULL, NULL, N'((?<Key>.[a-zA-Z\s\(\)]*):(?<Value>.[a-zA-Z0-9\- :$.,]*))', NULL, NULL)
SET IDENTITY_INSERT [dbo].[InfoMappingRegEx] OFF
GO
ALTER TABLE [dbo].[InfoMappingRegEx]  WITH CHECK ADD  CONSTRAINT [FK_InfoMappingRegEx_InfoMapping] FOREIGN KEY([InfoMappingId])
REFERENCES [dbo].[InfoMapping] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InfoMappingRegEx] CHECK CONSTRAINT [FK_InfoMappingRegEx_InfoMapping]
GO
