USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xC71_Value_declaration_form_Registered]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xC71_Value_declaration_form_Registered](
	[Value_declaration_form_Id] [int] NOT NULL,
	[id] [nvarchar](50) NOT NULL,
	[RegNumber] [nvarchar](50) NOT NULL,
	[SourceFile] [nvarchar](300) NOT NULL,
	[DocumentReference] [nvarchar](300) NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_xC71_RegisteredValuation] PRIMARY KEY CLUSTERED 
(
	[Value_declaration_form_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (313, N'188211', N'17023', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\17023-C71.xml', NULL, 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (314, N'188221', N'17033', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\17033-C71.xml', N'Driftwood', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (352, N'191778', N'20583', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\20583-C71.xml', N'Driftwood2', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (359, N'192975', N'21776', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\21776-C71.xml', N'SUCR411536-Witold', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (383, N'193644', N'22441', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\22441-C71.xml', N'SUCR417490-SH', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (385, N'194246', N'23040', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\23040-C71.xml', N'SUCR420688-K', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (389, N'194261', N'23055', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\23055-C71.xml', N'Driftwood3', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (411, N'197517', N'26307', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\26307-C71.xml', N'SUCR437227-NG', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (418, N'199400', N'1332', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\1332-C71.xml', N'SUCR450220-SM', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (424, N'199490', N'1421', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\1421-C71.xml', N'SUCR450220-KN', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (426, N'199515', N'1446', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\1446-C71.xml', N'SUCR450220-TB', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (432, N'199973', N'1904', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\1904-C71.xml', N'PEVGRE11193-NG', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (435, N'199979', N'1910', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\1910-C71.xml', N'PEVGRE11193-D', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (437, N'201061', N'2990', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\2990-C71.xml', N'SUCR459387-Liesel', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (439, N'201105', N'3034', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\3034-C71.xml', N'SUCR459387-Kani', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (440, N'201114', N'3043', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\3043-C71.xml', N'SUCR459387-Joy', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (455, N'202734', N'4660', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\4660-C71.xml', N'SUCR467998-Turbulence', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (457, N'202754', N'4680', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\4680-C71.xml', N'SUCR468202-Turbulence', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (2490, N'214164', N'16061', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\16061-C71.xml', N'SUCR527642-PTG', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (2501, N'215125', N'17021', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\17021-C71.xml', N'SMW1522211', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (2518, N'216611', N'18502', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\C71\18502-C71.xml', N'PIXB12348LP', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (2725, N'256281', N'658', N'C:\Users\josep\OneDrive\Clients\AutoBrokerage\Emails\Imports\C71\658-C71.xml', N'TSCW15986314USA', 3)
INSERT [dbo].[xC71_Value_declaration_form_Registered] ([Value_declaration_form_Id], [id], [RegNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (2731, N'256292', N'669', N'C:\Users\josep\OneDrive\Clients\AutoBrokerage\Emails\Imports\C71\669-C71.xml', N'TSCW15994828USA', 3)
GO
ALTER TABLE [dbo].[xC71_Value_declaration_form_Registered]  WITH CHECK ADD  CONSTRAINT [FK_xC71_RegisteredValuation_xC71_Value_declaration_form] FOREIGN KEY([Value_declaration_form_Id])
REFERENCES [dbo].[xC71_Value_declaration_form] ([Value_declaration_form_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xC71_Value_declaration_form_Registered] CHECK CONSTRAINT [FK_xC71_RegisteredValuation_xC71_Value_declaration_form]
GO
ALTER TABLE [dbo].[xC71_Value_declaration_form_Registered]  WITH CHECK ADD  CONSTRAINT [FK_xC71_Value_declaration_form_Registered_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xC71_Value_declaration_form_Registered] CHECK CONSTRAINT [FK_xC71_Value_declaration_form_Registered_ApplicationSettings]
GO
