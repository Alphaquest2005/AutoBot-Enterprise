USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Consignees]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Consignees](
	[ConsigneeName] [nvarchar](100) NOT NULL,
	[ConsigneeCode] [nvarchar](100) NULL,
	[Address] [nvarchar](300) NULL,
	[CountryCode] [nvarchar](3) NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_Consignees_1] PRIMARY KEY CLUSTERED 
(
	[ConsigneeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'~IARRON.SIMMONS', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'AARON S WILSON', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'AARON WILSON', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ABIEL ELAHIE', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ABIEL ELAHIE ( FREIGHT 9.00 US)', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ANDREA LORD', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ANNISA ST PAUL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ARLENA MITCHELL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ATHENA NOEL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'BERNISHA FRANCIS', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'BERNISHA FRANCIS ( FREIGHT 5.00 US )', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'BRITANA BOWEN', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'CARLISHA ALEXANDER', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'Customs Office GDWBS

Man Reg Number', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'Customs Office GDWBS
Man Reg Number', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'DARION JOHN-LEWIS', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'DAVID BOATSWAIN', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'DESEAN EMERY', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'DONIEL LA POMPE', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'DONNICA LEWIS', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'JARRON SIMMONS', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'JASON JAMES', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'KERRY FRANK', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'KEVON TUITT', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'KIRLA SYLEVESTER', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'KIRLA SYLVESTER', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'KWESI TAWFIQ ( FREIGHT 5.00 US)', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'KWESTTAWFIQ', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'LA TOYA VICTOR', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'LESLIE ANN JOHNSON', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'LORIC DICK', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MAHALIA CHARLES', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MAKEDA GIBBON', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MARGARET NOEL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MAURINE WILSON', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MICAH MATHURA', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MICHELE MARELL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MICHELE MARELLI', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MICHELLE CAMPBELL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MOLLISHA NOEL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'MOLLISHA NOEL ( FREIGHT 5.00 US)', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'NAKISHA CORNWALL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'NIKIAH NOEL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'NOT_APPLICABLE', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'O', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ONIESHA NOEL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ONIESHA NOEL ( FREIGHT 19.00 US )', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'RAPHAEL CHARLES', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'REANNAH CHARLES', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'REBA CALLISTE', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'REGINA DECOTEAU', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'REGINALD ANDALL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'RHONOSHA REDHEAD', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'RIANNA ANTOINE', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ROCHELLE RAGBASINGH', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ROLANDA JULIEN', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'ROYAN GEORGE', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHANICE ST BERNARD', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHANICE ST. BERNARD', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHANICE ST. BERNARD (FREIGHT 5.00 US)', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHANICE ST.BERNARD', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHEMICA CHARLES', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHEMROCK FORTUNE', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHERECA LEWIS', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHERECA LEWIS ( FREIGHT 3.00 US )', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHORNA-LEE CHITTERMAN', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'SHORNEL ALBERT', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'STEVON PETERS', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'System Processing', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'System.Text.StringBuilder', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'TADDISH CAMPBELL', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'TANISHA ANDREW BARTHOLOMEW', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'TANISHA ANDREW-BARTHOLOMEW', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'TERRIE ANN CHARLES', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'TERRIEANN CHARLES', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'TERRIEANN CHARLES ( FREIGHT 11.00 US)', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'TIMOTHY WALKER', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'TRAVIS JOHN', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'TRE AUGUSTINE', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'UNKNOWN', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'VERNICA CHARLES', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'VICTORIA AYLIFFE', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'WENDANNA CHARLES', NULL, NULL, NULL, 3)
INSERT [dbo].[Consignees] ([ConsigneeName], [ConsigneeCode], [Address], [CountryCode], [ApplicationSettingsId]) VALUES (N'WENDY TAMAR ASHBY', NULL, NULL, NULL, 3)
GO
