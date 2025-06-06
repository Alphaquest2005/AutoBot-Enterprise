USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[OCR-FieldValue]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCR-FieldValue](
	[Id] [int] NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_OCR-FieldValue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (162, N'XYLEM')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (167, N'USD')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (362, N'GARMIN')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (363, N'MASTRY')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (364, N'US Spars')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (365, N'Victron')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (366, N'WEST SYSTEM')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (367, N'CLEARCOTE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (368, N'CAPTAIN')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (369, N'BIG ROCK')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (370, N'STAR BRITE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (371, N'LANCASTER')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (372, N'BRUNGEREXPORT')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (373, N'MARINECO')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (383, N'XCD')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (401, N'GROSS')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (412, N'CRESSI')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (473, N'CustomMarineCanvas')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (474, N'GILL')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (475, N'POWERPRODUCTS')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (476, N'PARAGON')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (479, N'MARINECO')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (515, N'REEF')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (524, N'Helios')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (528, N'International')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (551, N'OURAY')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (565, N'SeaHawk')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (578, N'LewisMarine')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (590, N'CalaMarine')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (594, N'3M')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (619, N'BATTERY')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (622, N'HARKEN')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (637, N'LANDSEA')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (691, N'Trivantage')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (702, N'BainBridge')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (709, N'Challenge SailCloth')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (722, N'1')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (736, N'ParaCay')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (737, N'Budget Invoice')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (738, N'International')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (739, N'Amazon')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (740, N'MPI')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (741, N'Xylem')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (742, N'SeaHawk')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (743, N'MARINECO')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (744, N'MARINECO')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (745, N'Brunger')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (747, N'STAR BRITE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (748, N'Big Rock')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (749, N'CAPTAIN''S FASTENERS')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (750, N'Clear Cote')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (751, N'West System')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (752, N'Victron Energy')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (753, N'US Spars')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (754, N'MASTRY')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (755, N'GARMIN')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (756, N'Gross Mechanical')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (757, N'CRESSI')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (758, N'Paragon')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (759, N'PowerProducts')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (760, N'GILL')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (761, N'CustomMarineCanvas')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (762, N'Helios')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (763, N'International-AkzoNobel')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (764, N'OURay')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (765, N'Lewis Marine')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (766, N'Cala Marine')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (767, N'3M')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (768, N'Battery Sales')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (769, N'HARKEN')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (770, N'LandSea')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (771, N'Challenge SailCloth')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (772, N'TRIVANTAGE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (773, N'BainBridge')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (774, N'A&B Top')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (775, N'Crowley Freight')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (776, N'ParaCay')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (777, N'SeaHawk')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (778, N'LANCASTER')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (790, N'Trojan')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (797, N'Portage Freight')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (798, N'KingOceanBL')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (799, N'Manifest')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (807, N'RedTree')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (816, N'MPowerd')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (843, N'Primus')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (853, N'UNICORD')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (868, N'Harken Derm')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (875, N'CMP')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (887, N'RAYMARINE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (900, N'JAKOB Patick')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (901, N'CrowleyBL')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (911, N'PropGlide')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (913, N'1')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (927, N'WEST MARINE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (948, N'LIFELINE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (949, N'Force10')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (950, N'Force10')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (1950, N'Indel')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (1951, N'Indel')
GO
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (1963, N'Hella')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (1964, N'Hella')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2005, N'LEATHERMAN')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2007, N'LiftAtout')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2034, N'Reef')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2067, N'Jaytron')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2083, N'Jaytron')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2095, N'WEST MARINE Pro')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2105, N'FLORIDA HARDWARE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2106, N'FLORIDA HARDWARE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2144, N'Portage BL2')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2146, N'TAYLOR')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2147, N'TAYLOR')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2175, N'USD')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2182, N'XANTREX')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2183, N'XANTREX')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2191, N'ParaCay')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2192, N'PARADISE CAY')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2213, N'BUDGET MARINE')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2238, N'3M')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2239, N'3M')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2269, N'GARMIN2')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2270, N'GARMIN2')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2283, N'RARITAN')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2284, N'RARITAN')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2316, N'DONOVAN2')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2317, N'DONOVAN2')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2387, N'Lewis Marine')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2388, N'Lewis Marine2')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2401, N'CEI Grenada')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2427, N'SIERRA')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2428, N'SIERRA')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2476, N'DONOVAN3')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2477, N'DONOVAN3')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2591, N'Harken Derm')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2592, N'Harken Derm')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2614, N'HILCO')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2615, N'HILCO')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2683, N'CRESSI')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (2684, N'CRESSI2')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (3085, N'Manifest')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (3092, N'GDWBS')
INSERT [dbo].[OCR-FieldValue] ([Id], [Value]) VALUES (3101, N'WEB SOURCE')
GO
ALTER TABLE [dbo].[OCR-FieldValue]  WITH CHECK ADD  CONSTRAINT [FK_OCR-FieldValue_OCR-Fields] FOREIGN KEY([Id])
REFERENCES [dbo].[OCR-Fields] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OCR-FieldValue] CHECK CONSTRAINT [FK_OCR-FieldValue_OCR-Fields]
GO
