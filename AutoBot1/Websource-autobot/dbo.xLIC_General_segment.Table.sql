USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xLIC_General_segment]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xLIC_General_segment](
	[Arrival_date] [nvarchar](255) NULL,
	[Application_date] [nvarchar](255) NULL,
	[Expiry_date] [nvarchar](255) NULL,
	[Importation_date] [nvarchar](255) NULL,
	[General_segment_Id] [int] NOT NULL,
	[Importer_cellphone] [nvarchar](255) NULL,
	[Exporter_address] [nvarchar](255) NULL,
	[Exporter_country_code] [nvarchar](255) NULL,
	[Importer_code] [nvarchar](255) NULL,
	[Owner_code] [nvarchar](255) NULL,
	[Exporter_email] [nvarchar](255) NULL,
	[Importer_name] [nvarchar](255) NULL,
	[Importer_contact] [nvarchar](255) NULL,
	[Exporter_name] [nvarchar](255) NULL,
	[Exporter_telephone] [nvarchar](255) NULL,
	[Importer_telephone] [nvarchar](255) NULL,
	[Exporter_country_name] [nvarchar](255) NULL,
	[Exporter_cellphone] [nvarchar](255) NULL,
	[Importer_email] [nvarchar](255) NULL,
 CONSTRAINT [PK_xLIC_General_segment] PRIMARY KEY CLUSTERED 
(
	[General_segment_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/27/19', N'8/27/19', NULL, N'11/28/19', 77, N'405-8243', N'14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'JOSEPH BARTHOLOMEW', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/2019', N'9/2/2019', NULL, N'12/2/2019', 78, N'405-8243', N'Ref:30-15861
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/2019', N'9/2/2019', NULL, N'12/2/2019', 79, N'405-8243', N'Ref:30-15861
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/19', N'9/2/19', NULL, N'12/2/19', 80, N'405-8243', N'Ref:30-15861
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/19/19', N'7/19/19', NULL, N'10/25/19', 81, N'407-3744', N'14806 49TH STREET NORTH
CLEARWATER, FLORIDA 33762
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/26/19', N'7/25/19', NULL, N'10/26/19', 82, N'407-3744', N'USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'AMAZON. COM', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/19/19', N'7/16/19', NULL, N'10/16/19', 83, N'407-3744', N'25B WATER FRONT ROAD
COLE BAY
ST MAARTEN', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'BUDGET MARINR N.V', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/19/19', N'7/25/19', NULL, N'10/25/19', 84, N'407-3744', N'14805 49TH STREET NORTH
CLEARWATER, FLORIDA 33762
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/7/19', N'6/6/19', NULL, N'9/6/19', 85, N'407-3744', N'2105 N.W. 102 AVENUE
MIAMI, FLORIDA 33172
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'EQUIPSA INC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/14/19', N'6/15/19', NULL, N'9/17/19', 86, NULL, N'2105 N.W. 102 AVENUE
MIAMI, FLORIDA 33172
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'EQUIPSA INC', NULL, N'407-3744', NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/17/19', N'5/16/19', NULL, N'8/20/19', 87, N'407-3744', N'25B WATERFRONT ROAD
COLE BAY
ST MAARTEN', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'BUDGET MARINE N.V', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/21/19', N'6/21/19', NULL, N'8/21/19', 88, NULL, N'WATERFRONT ROAD 25B
ST MAARTEN', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'BUDGET MARINE SXM', NULL, N'407-3744', NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/5/19', N'4/4/19', NULL, N'7/4/19', 89, N'407-3744', N'25B WATERFRONT ROAD
COLE BAY
ST MAARTEN', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'BUDGET MARINE N.V', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/25/19', N'4/25/19', NULL, N'7/26/19', 90, N'407-3744', N'6001 ANTOINE DRIVE
HOUSTON, TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT LLC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/18/19', N'4/19/19', NULL, N'7/23/19', 91, N'407-3744', N'6001 ANTOINE DRIVE
HOUSTON, TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT IVC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/29/19', N'4/2/19', NULL, N'7/3/19', 92, N'407-3744', N'6001 ANTOINE DRIVE
HOUSTON TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/22/19', N'3/22/19', NULL, N'6/22/19', 93, N'407 3744', N'6001 ANTOINE DRIVE
HOUSTON, TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/29/19', N'4/1/19', NULL, N'7/1/19', 94, N'407-3744', N'12514 SW 117 CT
MIAMI FLORIDA 33186
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'THE MIAMI SHIRT COMPANY', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/12/19', N'4/15/19', NULL, N'7/15/19', 95, N'407-3744', N'6001 ANTOINE DRIVE
HOUSTON, TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/15/19', N'2/15/19', NULL, N'5/15/19', 96, N'407-3744', N'2105 N.W. 102 AVENUE
MIAMI, FLORIDA 33172
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'EQUIPSA INC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/8/19', N'2/10/19', NULL, N'5/11/19', 97, N'407-3744', N'6001 ANTOINE DRIVE
HOUSTON, TX 77091
USA', N'UA', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT LLC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/25/19', N'1/24/19', NULL, N'4/24/19', 98, N'407-3744', N'6001 ANTOINE ROAD
HOUSTON, TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIOAL PAINT LLC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/1/19', N'1/30/19', NULL, N'4/28/19', 99, N'407-3744', N'25B WATERFRONT ROASD
COLE BAY 
ST MAARTEEN', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'BUDGET MARINE N.V', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/18/19', N'1/17/19', NULL, N'4/17/19', 100, N'407-3744', N'6001 ANTOINE DRIVE
HOUSTON, TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT LLC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/11/19', N'1/13/19', NULL, N'3/14/19', 101, N'407-3744', N'1025 PARKWAY INDUSTRIAL PARK DRIVE
BUFORD DA 30518
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/4/19', N'1/10/19', NULL, N'4/10/19', 102, N'407-3744', N'2801 ANVIL STREET
NORTH SAINT PETERSBURG
FLORIDA 33710
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/4/19', N'1/4/19', NULL, N'4/4/19', 103, N'407-3744', N'12514 SW 117 CT.
MIAMI, FLORIDA 33166
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'MIAMI SHIRT COMPANY', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/4/19', N'1/4/19', NULL, N'4/4/19', 104, N'407 -3744', N'25B WATERFRONT ROAD
COLE BAY
ST MAARTEN', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'BUDGET MARINE', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/11/19', N'1/13/19', NULL, N'3/14/19', 105, N'407-3744', N'6001 ANTOINE DRIVE
HOUSTON, TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT LLC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/4/19', N'1/4/19', NULL, N'4/4/19', 106, N'407-3744', N'6001 ANTOINE DRIVE
HOUSTON, TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT LLC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/28/19', N'2/21/19', NULL, N'5/21/19', 107, N'407-3744', N'25B WATERFRONT ROAD
COLE BAY
ST MAARTEN', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'BUDGET MARINE', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/22/19', N'2/21/19', NULL, N'5/22/19', 108, N'407-3744', N'14805 49TH STREET NORTH
CLEARWATER, FLORIDA 33762
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/9/19', N'8/8/19', NULL, N'11/8/19', 109, N'407-3744', N'14805 46TH STREET NORTH
CLEARWATER, FLORIDA 33762
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'NEW NAUTICAL COATING, INC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/16/19', N'8/15/19', NULL, N'11/16/19', 110, N'407-3744', N'25B WATERFRONT ROAD
COLE BAY
ST MAARTEN', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'BUDGET MARINE ST MAARTEN', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/2019', N'9/9/2019', NULL, N'12/9/2019', 111, N'405-8243', N'Ref:30-15900
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/19', N'9/9/19', NULL, N'12/10/19', 112, N'405-8243', N'Ref:30-15900
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/12/2019', N'9/12/2019', NULL, N'12/12/2019', 113, N'405-8243', N'Ref:Adjustments

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/12/2019', N'9/12/2019', NULL, N'12/12/2019', 114, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/12/2019', N'9/12/2019', NULL, N'12/12/2019', 115, N'405-8243', N'Ref:30-15759

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/12/2019', N'9/12/2019', NULL, N'12/12/2019', 116, N'405-8243', N'Ref:July 2019

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/12/2019', N'9/12/2019', NULL, N'12/12/2019', 117, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/12/2019', N'9/12/2019', NULL, N'12/12/2019', 118, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/12/2019', N'9/12/2019', NULL, N'12/12/2019', 119, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2019', N'9/13/2019', NULL, N'12/13/2019', 120, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2019', N'9/13/2019', NULL, N'12/13/2019', 121, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2019', N'9/13/2019', NULL, N'12/13/2019', 122, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2019', N'9/13/2019', NULL, N'12/13/2019', 123, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 124, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 125, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 126, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 127, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 128, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 129, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 130, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 131, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 132, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 133, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 134, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 135, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 136, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 137, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 138, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 139, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 140, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 141, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 142, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 143, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 144, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 145, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 146, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 147, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 148, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 149, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 150, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 151, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 152, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 153, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 154, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 155, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 156, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 157, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 158, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 159, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 160, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 161, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 162, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 163, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 164, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 165, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 166, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/14/2019', N'9/14/2019', NULL, N'12/14/2019', 167, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 168, N'405-8243', N'Ref:30-15861
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 169, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 170, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 171, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 172, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 173, N'405-8243', N'Ref:30-15861
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 174, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 175, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 176, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
GO
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 177, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 178, N'405-8243', N'Ref:30-15861
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 179, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 180, N'405-8243', N'Ref:35933225

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 181, N'405-8243', N'Ref:1409193414

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/15/2019', N'9/15/2019', NULL, N'12/15/2019', 182, N'405-8243', N'Ref:1409198416

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/12/19', N'9/12/19', NULL, N'11/29/19', 183, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 184, N'405-8243', N'Ref:30-15900
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 185, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 186, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 187, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 188, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 189, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 190, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 191, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 192, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 193, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 194, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 195, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 196, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 197, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/2019', N'9/16/2019', NULL, N'12/16/2019', 198, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/19', N'9/16/19', NULL, N'12/16/19', 199, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/19', N'9/16/19', NULL, N'12/16/19', 200, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/19', N'9/16/19', NULL, N'12/16/19', 201, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/19', N'9/16/19', NULL, N'12/16/19', 202, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/19', N'9/16/19', NULL, N'12/16/19', 203, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/19', N'9/16/19', NULL, N'12/16/19', 204, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/16/19', N'9/16/19', NULL, N'12/16/19', 205, N'405-8243', N'Ref:August2019
WATERFRONT ROAD 25B ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'BUDGET MARINE SXM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/19', N'9/16/19', NULL, N'11/29/19', 206, N'405-8243', N'WESTERN MAIN ROAD, CHAGUARAMAS,
TRINIDAD, TT', N'TT', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'JOSEPH', N'BUDGET MARINE (TRINIDAD) LIMITED', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/2019', N'9/23/2019', NULL, N'12/23/2019', 207, N'405-8243', N'Ref:7006174

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Supplier Code', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/2019', N'9/23/2019', NULL, N'12/23/2019', 208, N'405-8243', N'Ref:7006207
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/2019', N'9/23/2019', NULL, N'12/23/2019', 209, N'405-8243', N'Ref:7006196
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/2019', N'9/23/2019', NULL, N'12/23/2019', 210, N'405-8243', N'Ref:7006199
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/2019', N'9/23/2019', NULL, N'12/23/2019', 211, N'405-8243', N'Ref:7006227
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/2019', N'9/23/2019', NULL, N'12/23/2019', 212, N'405-8243', N'Ref:7006279
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/2019', N'9/23/2019', NULL, N'12/23/2019', 213, N'405-8243', N'Ref:7006188
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/2019', N'9/23/2019', NULL, N'12/23/2019', 214, N'405-8243', N'Ref:9001800
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/2019', N'9/23/2019', NULL, N'12/23/2019', 215, N'405-8243', N'Ref:5609822117
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'RalphLauren.com', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/19', N'9/23/19', NULL, N'12/23/19', 216, N'405-8243', N'REF:7006174
P.O. BOX 741503, ATLANTA, GA 30374-
1503, U.S.A.', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'INTERNATIONAL PAINT LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/19', N'9/23/19', NULL, N'12/23/19', 217, N'405-8243', N'Ref:5609822117
USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'RalphLauren.com', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/19', N'9/23/19', NULL, N'12/23/19', 218, N'405-8243', N'Ref:9001800
USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/19', N'9/23/19', NULL, N'12/23/19', 219, N'405-8243', N'Ref:7006196
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/19', N'9/23/19', NULL, N'12/23/19', 220, N'405-8243', N'Ref:7006279
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/19', N'9/23/19', NULL, N'12/23/19', 221, N'405-8243', N'Ref:7006227
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/19', N'9/23/19', NULL, N'12/23/19', 222, N'405-8243', N'Ref:7006207
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/19', N'9/23/19', NULL, N'12/23/19', 223, N'405-8243', N'Ref:7006199
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/23/19', N'9/23/19', NULL, N'12/23/19', 224, N'405-8243', N'REF:7006174
P.O. BOX 741503, ATLANTA, GA 30374-
1503, U.S.A.', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'INTERNATIONAL PAINT LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/26/19', N'9/26/19', NULL, N'12/27/19', 225, NULL, N'12514 SW 117  CT.MIAMI, FL  33186', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', NULL, N'THE MIAMI SHIRT COMPANY', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/29/2019', N'9/29/2019', NULL, N'12/29/2019', 226, N'405-8243', N'Ref:7006175
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/29/2019', N'9/29/2019', NULL, N'12/29/2019', 227, N'405-8243', N'Ref:7006176
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/29/2019', N'9/29/2019', NULL, N'12/29/2019', 228, N'405-8243', N'Ref:7006160
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/29/2019', N'9/29/2019', NULL, N'12/29/2019', 229, N'405-8243', N'Ref:7006160
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/29/2019', N'9/29/2019', NULL, N'12/29/2019', 230, N'405-8243', N'Ref:7006175
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/29/2019', N'9/29/2019', NULL, N'12/29/2019', 231, N'405-8243', N'Ref:7006176
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/29/19', N'9/29/19', NULL, N'12/30/19', 232, N'405-8243', N'Ref:7006176
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/29/19', N'9/29/19', NULL, N'12/30/19', 233, N'405-8243', N'Ref:7006175
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/29/19', N'9/29/19', NULL, N'12/30/19', 234, N'405-8243', N'Ref:7006160
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/12/2019', N'11/12/2019', NULL, N'2/12/2020', 235, N'405-8243', N'Ref:7006353
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/12/2019', N'11/12/2019', NULL, N'2/12/2020', 236, N'405-8243', N'Ref:7006353
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/12/19', N'11/12/19', NULL, N'2/13/20', 237, N'405-8243', N'Ref:7006353
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/13/2020', N'4/13/2020', NULL, N'7/13/2020', 238, N'405-8243', N'Ref:7007232
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/13/2020', N'4/13/2020', NULL, N'7/13/2020', 239, N'405-8243', N'Ref:7007067

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', N'SEAHAWK', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/14/2020', N'4/14/2020', NULL, N'7/14/2020', 240, N'405-8243', N'Ref:7007232
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/14/2020', N'4/14/2020', NULL, N'7/14/2020', 241, N'405-8243', N'Ref:7007067

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', N'SEAHAWK', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/14/2020', N'4/14/2020', NULL, N'7/14/2020', 242, N'405-8243', N'Ref:7007232
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/14/2020', N'4/14/2020', NULL, N'7/14/2020', 243, N'405-8243', N'Ref:7007067

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', N'SEAHAWK', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/14/2020', N'4/14/2020', NULL, N'7/14/2020', 244, N'405-8243', N'Ref:7007067

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', N'SEAHAWK', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/14/2020', N'4/14/2020', NULL, N'7/14/2020', 245, N'405-8243', N'Ref:7007232
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/19/2020', N'4/19/2020', NULL, N'7/19/2020', 246, N'405-8243', N'Ref:7007130
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/19/2020', N'4/19/2020', NULL, N'7/19/2020', 247, N'405-8243', N'Ref:7007098
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/19/2020', N'4/19/2020', NULL, N'7/19/2020', 248, N'405-8243', N'Ref:7007098
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/19/2020', N'4/19/2020', NULL, N'7/19/2020', 249, N'405-8243', N'Ref:7007097
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 250, N'405-8243', N'Ref:7007130
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 251, N'405-8243', N'Ref:7007098
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 252, N'405-8243', N'Ref:7007098
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 253, N'405-8243', N'Ref:7007097
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 254, N'405-8243', N'Ref:7007130
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 255, N'405-8243', N'Ref:7007098
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 256, N'405-8243', N'Ref:7007098
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 257, N'405-8243', N'Ref:7007097
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 258, N'405-8243', N'Ref:7007097
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 259, N'405-8243', N'Ref:7007098
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/2020', N'4/20/2020', NULL, N'7/20/2020', 260, N'405-8243', N'Ref:7007130
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/20', N'4/20/20', NULL, N'7/20/20', 261, N'405-8243', N'Ref:7007097
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/20', N'4/20/20', NULL, N'7/20/20', 262, N'405-8243', N'REF:7007098
P.O. BOX 741503, ATLANTA, GA 30374-1503, U.S.A.
,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/20/20', N'4/20/20', NULL, N'7/20/20', 263, N'405-8243', N'REF:7007130
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/15/2020', N'5/15/2020', NULL, N'8/15/2020', 264, N'405-8243', N'Ref:7007320
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/15/2020', N'5/15/2020', NULL, N'8/15/2020', 265, N'405-8243', N'Ref:7007320
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/15/2020', N'5/15/2020', NULL, N'8/15/2020', 266, N'405-8243', N'Ref:7007320
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/15/2020', N'5/15/2020', NULL, N'8/15/2020', 267, N'405-8243', N'Ref:7007320
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/15/2020', N'5/15/2020', NULL, N'8/15/2020', 268, N'405-8243', N'Ref:7007320
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/17/2020', N'5/17/2020', NULL, N'8/17/2020', 269, N'405-8243', N'Ref:7007320
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/15/20', N'5/17/20', NULL, N'8/17/20', 270, N'405-8243', N'Ref:7007320
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/15/20', N'5/15/20', NULL, N'8/14/20', 271, N'405-8243', N'Ref:7007344
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/8/2020', N'6/8/2020', NULL, N'9/8/2020', 272, N'405-8243', N'Ref:7007269
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/8/2020', N'6/8/2020', NULL, N'9/8/2020', 273, N'405-8243', N'Ref:7007190
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/8/2020', N'6/8/2020', NULL, N'9/8/2020', 274, N'405-8243', N'Ref:7007269
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/8/20', N'6/8/20', NULL, N'9/8/20', 275, N'405-8243', N'Ref:7007269
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/8/20', N'6/9/20', NULL, N'8/9/20', 276, N'405-8243', N'Ref:7007190
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
GO
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/28/2020', N'6/28/2020', NULL, N'9/28/2020', 277, N'405-8243', N'Ref:7007270
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/28/2020', N'6/28/2020', NULL, N'9/28/2020', 278, N'405-8243', N'Ref:7007437
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/28/2020', N'6/28/2020', NULL, N'9/28/2020', 279, N'405-8243', N'Ref:7007436
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/28/20', N'6/28/20', NULL, N'9/29/20', 280, N'405-8243', N'Ref:7007270
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/28/20', N'6/28/20', NULL, N'9/29/20', 281, N'405-8243', N'Ref:7007436
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/28/20', N'6/28/20', NULL, N'9/29/20', 282, N'405-8243', N'Ref:7007437
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/2020', N'7/30/2020', NULL, N'10/30/2020', 283, N'405-8243', N'Ref:00035
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/2020', N'7/30/2020', NULL, N'10/30/2020', 284, N'405-8243', N'Ref:00038
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/2020', N'7/30/2020', NULL, N'10/30/2020', 285, N'405-8243', N'Ref:00034
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/2020', N'7/30/2020', NULL, N'10/30/2020', 286, N'405-8243', N'Ref:00037
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/2020', N'7/30/2020', NULL, N'10/30/2020', 287, N'405-8243', N'Ref:00105
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/20', N'7/30/20', NULL, N'10/30/20', 288, N'405-8243', N'Ref:00034
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/20', N'7/30/20', NULL, N'10/30/20', 289, N'405-8243', N'Ref:00035
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/20', N'7/30/20', NULL, N'10/30/20', 290, N'405-8243', N'Ref:00038
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/20', N'7/30/20', NULL, N'10/30/20', 291, N'405-8243', N'Ref:00037
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/20', N'7/30/20', NULL, N'10/30/20', 292, N'405-8243', N'Ref:00105
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/16/20', N'7/30/20', NULL, N'10/30/20', 293, N'407-3744', N'6001 ANTOINE DRIVE
HOUSTON TX 77091
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'LEO AUSTIN', N'INTERNATIONAL PAINT LLC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/5/2020', N'8/5/2020', NULL, N'11/5/2020', 294, N'405-8243', N'Ref:00105
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/14/2020', N'8/14/2020', NULL, N'11/14/2020', 295, N'405-8243', N'Ref:00186
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/13/20', N'8/13/20', NULL, N'11/13/20', 296, N'405-8243', N'REF:IB501619
P.O. BOX 741503, ATLANTA, GA 30374-1503, U.S.A.', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'JOSEPH', N'INTERNATIONAL PAINT LLC', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/14/2020', N'8/14/2020', NULL, N'11/14/2020', 297, N'405-8243', N'Ref:5225844
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/13/20', N'8/13/20', NULL, N'11/13/20', 298, N'405-8243', N'REF:5225844
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'JOSEPH', N'AMAZON.COM', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/13/20', N'8/15/20', NULL, N'11/17/20', 299, N'405-8243', N'REF:5225844
USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'JOSEPH', N'AMAZON.COM', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/2020', N'8/19/2020', NULL, N'11/19/2020', 300, N'405-8243', N'Ref:00213
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/2020', N'8/19/2020', NULL, N'11/19/2020', 301, N'405-8243', N'Ref:00163
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/2020', N'8/19/2020', NULL, N'11/19/2020', 302, N'405-8243', N'Ref:00072
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/2020', N'8/19/2020', NULL, N'11/19/2020', 303, N'405-8243', N'Ref:00213
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/2020', N'8/19/2020', NULL, N'11/19/2020', 304, N'405-8243', N'Ref:00163
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/2020', N'8/19/2020', NULL, N'11/19/2020', 305, N'405-8243', N'Ref:00072
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/2020', N'8/19/2020', NULL, N'11/19/2020', 306, N'405-8243', N'Ref:00213
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/2020', N'8/19/2020', NULL, N'11/19/2020', 307, N'405-8243', N'Ref:00163
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/2020', N'8/19/2020', NULL, N'11/19/2020', 308, N'405-8243', N'Ref:00072
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/20', N'8/20/20', NULL, N'11/20/20', 309, N'405-8243', N'Ref:00072
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/20', N'8/20/20', NULL, N'11/20/20', 310, N'405-8243', N'Ref:00213
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/20', N'8/20/20', NULL, N'11/20/20', 311, N'405-8243', N'Ref:00163
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/20', N'8/19/20', NULL, N'11/20/20', 312, N'405-8243', N'Ref:00072
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/27/2020', N'8/27/2020', NULL, N'11/27/2020', 313, N'405-8243', N'Ref:Tobago9
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/27/2020', N'8/27/2020', NULL, N'11/27/2020', 314, N'405-8243', N'Ref:Tobago5
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/27/2020', N'8/27/2020', NULL, N'11/27/2020', 315, N'405-8243', N'Ref:Tobago11
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/28/2020', N'8/28/2020', NULL, N'11/28/2020', 316, N'405-8243', N'Ref:Tobago9
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'261495', NULL, NULL, NULL, N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/28/2020', N'8/28/2020', NULL, N'11/28/2020', 317, N'405-8243', N'Ref:Tobago5
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'261495', NULL, NULL, NULL, N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/28/2020', N'8/28/2020', NULL, N'11/28/2020', 318, N'405-8243', N'Ref:Tobago11
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'261495', NULL, NULL, NULL, N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/28/2020', N'8/28/2020', NULL, N'11/28/2020', 319, N'405-8243', N'Ref:Tobago9
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'261495', NULL, NULL, NULL, N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/28/2020', N'8/28/2020', NULL, N'11/28/2020', 320, N'405-8243', N'Ref:Tobago5
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'261495', NULL, NULL, NULL, N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/28/2020', N'8/28/2020', NULL, N'11/28/2020', 321, N'405-8243', N'Ref:Tobago11
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'261495', NULL, NULL, NULL, N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/28/20', N'8/28/20', NULL, N'9/28/20', 322, N'405-8243', N'Ref:Tobago5
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'261495', NULL, NULL, N'PORTAGE IMPORT EXPORT & BROKERAGE', N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/28/20', N'8/28/20', NULL, N'9/28/20', 323, N'405-8243', N'Ref:Tobago9
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'261495', NULL, NULL, N'PORTAGE IMPORT EXPORT & BROKERAGE', N'JOSEPH', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/28/20', N'8/28/20', NULL, N'9/28/20', 324, N'405-8243', N'Ref:Tobago9
City State Management AG, Grammetstiasse i4, 4410 Liestal, Switzerland
,CH
', N'CH', N'261495', NULL, NULL, N'PORTAGE IMPORT EXPORT & BROKERAGE', N'Joseph', N'Jakob Patick', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/2020', N'9/2/2020', NULL, N'12/2/2020', 325, N'405-8243', N'Ref:00197
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/2020', N'9/2/2020', NULL, N'12/2/2020', 326, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/2020', N'9/2/2020', NULL, N'12/2/2020', 327, N'405-8243', N'Ref:1278
4769 NW 72 AVE, Miami, FL 33166
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'PropGlide USA Corp', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/20', N'9/2/20', NULL, N'12/2/20', 328, N'405-8243', N'Ref:00197
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/20', N'9/1/20', NULL, N'12/2/20', 329, N'405-8243', N'REF:1278
4769 NW 72 AVE
MIAMI, FL 33166 US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'JOSEPH BARTHOLOMEW', N'PROPGLIDE USA CORP', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 330, N'405-8243', N'Ref:00197
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 331, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 332, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 333, N'405-8243', N'Ref:1278
4769 NW 72 AVE, Miami, FL 33166
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'PropGlide USA Corp', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 334, N'405-8243', N'Ref:00197
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 335, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 336, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 337, N'405-8243', N'Ref:1278
4769 NW 72 AVE, Miami, FL 33166
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'PropGlide USA Corp', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 338, N'405-8243', N'Ref:00197
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 339, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 340, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 341, N'405-8243', N'Ref:1278
4769 NW 72 AVE, Miami, FL 33166
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'PropGlide USA Corp', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 342, N'405-8243', N'Ref:00197
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 343, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 344, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 345, N'405-8243', N'Ref:1278
4769 NW 72 AVE, Miami, FL 33166
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'PropGlide USA Corp', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 346, N'405-8243', N'Ref:00197
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 347, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 348, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/4/2020', N'9/4/2020', NULL, N'12/4/2020', 349, N'405-8243', N'Ref:1278
4769 NW 72 AVE, Miami, FL 33166
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'PropGlide USA Corp', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/20', N'8/31/20', NULL, N'11/30/20', 350, N'405-8243', N'REF:219593
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/20', N'9/2/20', NULL, N'12/2/20', 351, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/20', N'9/2/20', NULL, N'12/2/20', 352, N'405-8243', N'Ref:5086606
USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'AMAZON. COM', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/2/20', N'9/2/20', NULL, N'12/2/20', 353, N'405-8243', N'Ref:1278
4769 NW 72 AVE, Miami, FL 33166
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'PropGlide USA Corp', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/7/2020', N'9/7/2020', NULL, N'12/7/2020', 354, N'405-8243', N'Ref:00168
Rock Hill DC - East Coast,WEST MARINE #00860, 860 Marine Drive, Rock Hill SC 29730, USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'WEST MARINE Pro', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/7/2020', N'9/7/2020', NULL, N'12/7/2020', 355, N'405-8243', N'Ref:00236
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/7/2020', N'9/7/2020', NULL, N'12/7/2020', 356, N'405-8243', N'Ref:00214
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/7/2020', N'9/7/2020', NULL, N'12/7/2020', 357, N'405-8243', N'Ref:00227
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/7/2020', N'9/7/2020', NULL, N'12/7/2020', 358, N'405-8243', N'Ref:00219
4041 SW 47TH AVENUE, FORT LAUDERDALE, FL 33314
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'STAR BRITE INTERNATIONAL,INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/18/2020', N'9/18/2020', NULL, N'12/18/2020', 1359, N'405-8243', N'Ref:00338

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/18/2020', N'9/18/2020', NULL, N'12/18/2020', 1360, N'405-8243', N'Ref:00338
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/18/2020', N'9/18/2020', NULL, N'12/18/2020', 1361, N'405-8243', N'Ref:00338
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/18/2020', N'9/18/2020', NULL, N'12/18/2020', 1362, N'405-8243', N'Ref:00141
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/14/20', N'9/25/20', NULL, N'12/25/20', 1363, N'405-8243', N'REF:IB512048
P.O. BOX 741503, ATLANTA, GA 30374-1503, U.S.A.', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'JOSEPH BARTHOLOMEW', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/20', N'9/25/20', NULL, N'12/25/20', 1364, N'405-8243', N'REF:220800
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/9/2020', N'10/9/2020', NULL, N'1/9/2021', 1365, N'405-8243', N'Ref:00341
Rock Hill DC - East Coast,WEST MARINE #00860, 860 Marine Drive, Rock Hill SC 29730, USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'WEST MARINE Pro', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/9/2020', N'10/9/2020', NULL, N'1/9/2021', 1366, N'405-8243', N'Ref:00347
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/9/20', N'10/9/20', NULL, N'11/27/20', 1367, N'405-8243', N'Ref:00341
Rock Hill DC - East Coast,WEST MARINE #00860, 860 Marine Drive, Rock Hill SC 29730, USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'WEST MARINE Pro', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/9/20', N'10/9/20', NULL, N'11/27/20', 1368, N'405-8243', N'Ref:00347
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/14/20', N'10/14/20', NULL, N'11/27/20', 1369, N'405-8243', N'REF:IB516916
P.O. BOX 741503, ATLANTA, GA 30374-1503, U.S.A.
,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/20', N'10/14/20', NULL, N'11/27/20', 1370, N'405-8243', N'REF:221542
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/14/20', N'10/14/20', NULL, N'11/27/20', 1371, N'405-8243', N'REF:IB516228
P.O. BOX 741503, ATLANTA, GA 30374-1503, U.S.A.
,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1372, N'405-8243', N'Ref:00504
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1373, N'405-8243', N'Ref:00407
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1374, N'405-8243', N'Ref:00417
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1375, N'405-8243', N'Ref:131984
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1376, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
GO
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1377, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1378, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1379, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1380, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1381, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1382, N'405-8243', N'Ref:00504
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1383, N'405-8243', N'Ref:00407
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1384, N'405-8243', N'Ref:00417
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1385, N'405-8243', N'Ref:131984
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1386, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1387, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1388, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1389, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1390, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/2020', N'11/1/2020', NULL, N'2/1/2021', 1391, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1392, N'405-8243', N'Ref:00388
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1393, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1394, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1395, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1396, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1397, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1398, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1399, N'405-8243', N'Ref:00504
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1400, N'405-8243', N'Ref:00407
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1401, N'405-8243', N'Ref:00417
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1402, N'405-8243', N'Ref:131984
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1403, N'405-8243', N'Ref:00388
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1404, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1405, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1406, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1407, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/2/2020', N'11/2/2020', NULL, N'2/2/2021', 1408, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/1/20', NULL, N'2/2/21', 1409, N'405-8243', N'Ref:00504
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/1/20', NULL, N'2/2/21', 1410, N'405-8243', N'Ref:00407
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/2/20', NULL, N'2/2/21', 1411, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/2/20', NULL, N'2/2/21', 1412, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/2/20', NULL, N'2/2/21', 1413, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/2/20', NULL, N'2/2/21', 1414, N'405-8243', N'Ref:131984
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/2/20', NULL, N'2/2/21', 1415, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/2/20', NULL, N'2/2/21', 1416, N'405-8243', N'Ref:00417
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/2/20', NULL, N'2/2/21', 1417, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2020', N'11/3/2020', NULL, N'2/3/2021', 1418, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2020', N'11/3/2020', NULL, N'2/3/2021', 1419, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2020', N'11/3/2020', NULL, N'2/3/2021', 1420, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2020', N'11/3/2020', NULL, N'2/3/2021', 1421, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/1/20', N'11/2/20', NULL, N'2/2/21', 1422, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1423, N'405-8243', N'Ref:00504
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1424, N'405-8243', N'Ref:00407
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1425, N'405-8243', N'Ref:00417
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1426, N'405-8243', N'Ref:131984
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1427, N'405-8243', N'Ref:00388
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1428, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1429, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1430, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1431, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1432, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1433, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1434, N'405-8243', N'Ref:00504
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1435, N'405-8243', N'Ref:00504
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1436, N'405-8243', N'Ref:00407
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1437, N'405-8243', N'Ref:00407
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1438, N'405-8243', N'Ref:00417
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1439, N'405-8243', N'Ref:00417
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1440, N'405-8243', N'Ref:131984
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1441, N'405-8243', N'Ref:131984
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1442, N'405-8243', N'Ref:00388
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1443, N'405-8243', N'Ref:00388
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1444, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1445, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1446, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1447, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1448, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1449, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1450, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1451, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1452, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1453, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1454, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1455, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1456, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1457, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1458, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/4/2020', N'11/4/2020', NULL, N'2/4/2021', 1459, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1460, N'405-8243', N'Ref:00504
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1461, N'405-8243', N'Ref:00504
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1462, N'405-8243', N'Ref:00407
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1463, N'405-8243', N'Ref:00407
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1464, N'405-8243', N'Ref:00417
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1465, N'405-8243', N'Ref:00417
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1466, N'405-8243', N'Ref:131984
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1467, N'405-8243', N'Ref:131984
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1468, N'405-8243', N'Ref:00388
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1469, N'405-8243', N'Ref:00388
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1470, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1471, N'405-8243', N'Ref:00378
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1472, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1473, N'405-8243', N'Ref:00419
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1474, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1475, N'405-8243', N'Ref:00411
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1476, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
GO
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1477, N'405-8243', N'Ref:00263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1478, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1479, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1480, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1481, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1482, N'405-8243', N'Ref:00312
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1483, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1484, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/5/2020', N'11/5/2020', NULL, N'2/5/2021', 1485, N'405-8243', N'Ref:00264
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/9/2020', N'11/9/2020', NULL, N'2/9/2021', 1486, N'405-8243', N'Ref:INV/SX/2020/000958

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', NULL, NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/2020', N'11/17/2020', NULL, N'2/17/2021', 1487, N'405-8243', N'Ref:00554
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/2020', N'11/17/2020', NULL, N'2/17/2021', 1488, N'405-8243', N'Ref:00555
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/2020', N'11/17/2020', NULL, N'2/17/2021', 1489, N'405-8243', N'Ref:00487
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/2020', N'11/17/2020', NULL, N'2/17/2021', 1490, N'405-8243', N'Ref:00488
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/2020', N'11/17/2020', NULL, N'2/17/2021', 1491, N'405-8243', N'Ref:00554
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/2020', N'11/17/2020', NULL, N'2/17/2021', 1492, N'405-8243', N'Ref:00555
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/2020', N'11/17/2020', NULL, N'2/17/2021', 1493, N'405-8243', N'Ref:00487
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/2020', N'11/17/2020', NULL, N'2/17/2021', 1494, N'405-8243', N'Ref:00488
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/2020', N'11/17/2020', NULL, N'2/17/2021', 1495, N'405-8243', N'Ref:00415
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/20', N'11/17/20', NULL, N'2/17/21', 1496, N'405-8243', N'Ref:00487
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/20', N'11/17/20', NULL, N'2/17/21', 1497, N'405-8243', N'Ref:00555
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/20', N'11/17/20', NULL, N'2/17/21', 1498, N'405-8243', N'Ref:00488
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/17/20', N'11/17/20', NULL, N'2/17/21', 1499, N'405-8243', N'REF:00554
P.O. BOX 741503, ATLANTA, GA 30374-1503, U.S.A.
,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/14/20', N'11/19/20', NULL, N'2/20/21', 1500, N'405-8243', N'REF:IB523227
P.O. BOX 741503, ATLANTA, GA 30374-1503, U.S.A.
,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/19/20', N'11/19/20', NULL, N'2/20/21', 1501, N'405-8243', N'REF:1307
4769 NW 72 AVE
MIAMI, FL 33166 US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'JOSEPH BARTHOLOMEW', N'PROPGLIDE USA CORP', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/9/20', N'11/20/20', NULL, N'2/20/21', 1502, N'405-8243', N'REF:7245935
ROCK HILL DC - EAST COAST,WEST MARINE #00860, 860 MARINE DRIVE, ROCK HILL SC 29730, USA
,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'WEST MARINE Pro', NULL, NULL, NULL, NULL, N'josephbartholomew@outlook.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/3/2020', N'12/3/2020', NULL, N'3/3/2021', 1503, N'405-8243', N'Ref:00613
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/3/2020', N'12/3/2020', NULL, N'3/3/2021', 1504, N'405-8243', N'Ref:00610
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/3/2020', N'12/3/2020', NULL, N'3/3/2021', 1505, N'405-8243', N'Ref:00519
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/3/2020', N'12/3/2020', NULL, N'3/3/2021', 1506, N'405-8243', N'Ref:00520
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/3/2020', N'12/3/2020', NULL, N'3/3/2021', 1507, N'405-8243', N'Ref:00435
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/3/2020', N'12/3/2020', NULL, N'3/3/2021', 1508, N'405-8243', N'Ref:00612
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/3/2020', N'12/3/2020', NULL, N'3/3/2021', 1509, N'405-8243', N'Ref:00609
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/2020', N'12/4/2020', NULL, N'3/4/2021', 1510, N'405-8243', N'Ref:00295
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/2020', N'12/4/2020', NULL, N'3/4/2021', 1511, N'405-8243', N'Ref:00295
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/2020', N'12/4/2020', NULL, N'3/4/2021', 1512, N'405-8243', N'Ref:00613
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/2020', N'12/4/2020', NULL, N'3/4/2021', 1513, N'405-8243', N'Ref:00610
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/2020', N'12/4/2020', NULL, N'3/4/2021', 1514, N'405-8243', N'Ref:00519
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/2020', N'12/4/2020', NULL, N'3/4/2021', 1515, N'405-8243', N'Ref:00520
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/2020', N'12/4/2020', NULL, N'3/4/2021', 1516, N'405-8243', N'Ref:00435
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/2020', N'12/4/2020', NULL, N'3/4/2021', 1517, N'405-8243', N'Ref:00612
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/2020', N'12/4/2020', NULL, N'3/4/2021', 1518, N'405-8243', N'Ref:00609
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/3/20', N'12/3/20', NULL, N'3/4/21', 1519, N'405-8243', N'Ref:00613
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/20', N'12/4/20', NULL, N'3/4/21', 1520, N'405-8243', N'Ref:00295
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/2020', N'12/19/2020', NULL, N'3/19/2021', 1521, N'405-8243', N'Ref:00512
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/2020', N'12/19/2020', NULL, N'3/19/2021', 1522, N'405-8243', N'Ref:00511
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/2020', N'12/19/2020', NULL, N'3/19/2021', 1523, N'405-8243', N'Ref:00671
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/2020', N'12/19/2020', NULL, N'3/19/2021', 1524, N'405-8243', N'Ref:00673
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/2020', N'12/19/2020', NULL, N'3/19/2021', 1525, N'405-8243', N'Ref:8526649
USA
,US
', N'US', N'261495', NULL, NULL, NULL, N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/20', N'12/19/20', NULL, N'3/22/21', 1526, N'405-8243', N'Ref:00511
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/20', N'12/19/20', NULL, N'3/22/21', 1527, N'405-8243', N'Ref:00671
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/20', N'12/19/20', NULL, N'3/22/21', 1528, N'405-8243', N'Ref:00673
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/20', N'12/19/20', NULL, N'3/22/21', 1529, N'405-8243', N'Ref:00512
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/20', N'12/19/20', NULL, N'3/22/21', 1530, N'405-8243', N'Ref:00512
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/20', N'12/19/20', NULL, N'3/22/21', 1531, N'405-8243', N'Ref:00512
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/19/20', N'12/19/20', NULL, N'3/22/21', 1532, N'405-8243', N'Ref:8526649
USA
,US
', N'US', N'261495', NULL, NULL, N'PORTAGE IMPORT EXPORT & BROKERAGE', N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/4/20', N'12/4/20', NULL, N'3/22/21', 1533, N'405-8243', N'Ref:00520
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/16/20', N'12/16/20', NULL, N'3/15/21', 1534, N'405-8243', N'Ref:00612
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/29/2020', N'12/29/2020', NULL, N'3/29/2021', 1535, N'405-8243', N'Ref:00581
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/29/20', N'12/29/20', NULL, N'3/29/21', 1536, N'405-8243', N'Ref:00581
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/8/2021', N'1/8/2021', NULL, N'4/8/2021', 1537, N'405-8243', N'Ref:00623
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/8/2021', N'1/8/2021', NULL, N'4/8/2021', 1538, N'405-8243', N'Ref:00624
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/8/21', N'1/8/21', NULL, N'4/8/21', 1539, N'405-8243', N'Ref:00623
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/8/21', N'1/8/21', NULL, N'4/8/21', 1540, N'405-8243', N'Ref:00624
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/25/2021', N'1/25/2021', NULL, N'4/25/2021', 1541, N'405-8243', N'Ref:00617
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/25/21', N'1/25/21', NULL, N'3/26/21', 1542, N'405-8243', N'Ref:00617
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/2021', N'1/26/2021', NULL, N'4/26/2021', 1543, N'405-8243', N'Ref:00745
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/2021', N'1/26/2021', NULL, N'4/26/2021', 1544, N'405-8243', N'Ref:00746
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/2021', N'1/26/2021', NULL, N'4/26/2021', 1545, N'405-8243', N'Ref:00684
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/2021', N'1/26/2021', NULL, N'4/26/2021', 1546, N'405-8243', N'Ref:00685
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/21', N'1/26/21', NULL, N'3/26/21', 1547, N'405-8243', N'Ref:00685
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/21', N'1/26/21', NULL, N'3/26/21', 1548, N'405-8243', N'Ref:00684
2801 Anvil Street North, Saint Petersburg FL 33710, United States
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'YANMAR MASTRY ENGINE CENTER', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/21', N'1/26/21', NULL, N'3/26/21', 1549, N'405-8243', N'Ref:00746
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/21', N'1/26/21', NULL, N'3/26/21', 1550, N'405-8243', N'Ref:00745
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/2021', N'1/26/2021', NULL, N'4/26/2021', 1551, N'405-8243', N'Ref:00617
3 Rosol Lane Saddle Brook, NJ 07663 -800-338-9143
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'CRESSI USA', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/25/2021', N'2/25/2021', NULL, N'5/25/2021', 1552, N'405-8243', N'Ref:00583
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/25/2021', N'2/25/2021', NULL, N'5/25/2021', 1553, N'405-8243', N'Ref:00820

,
', NULL, N'07290940003049', NULL, NULL, NULL, N'Joseph', N'SEAHAWK SEAHAWK', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/25/21', N'2/25/21', NULL, N'4/30/21', 1554, N'405-8243', N'Ref:00583
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/25/21', N'2/25/21', NULL, N'4/30/21', 1555, N'405-8243', N'REF:00820
NEW NAUTICAL COATINGS, INC
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'SEAHAWK', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/25/2021', N'2/25/2021', NULL, N'5/25/2021', 1556, N'405-8243', N'Ref:00583
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/25/2021', N'2/25/2021', NULL, N'5/25/2021', 1557, N'405-8243', N'Ref:00583
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/25/2021', N'2/25/2021', NULL, N'5/25/2021', 1558, N'405-8243', N'Ref:00583
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/25/2021', N'2/25/2021', NULL, N'5/25/2021', 1559, N'405-8243', N'Ref:00583
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'2/25/2021', N'2/25/2021', NULL, N'5/25/2021', 1560, N'405-8243', N'Ref:00583
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/18/2021', N'3/18/2021', NULL, N'6/18/2021', 1561, N'405-8243', N'Ref:00584
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/28/2021', N'3/28/2021', NULL, N'6/28/2021', 1562, N'405-8243', N'Ref:00876
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/28/2021', N'3/28/2021', NULL, N'6/28/2021', 1563, N'405-8243', N'Ref:00800
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/28/2021', N'3/28/2021', NULL, N'6/28/2021', 1564, N'405-8243', N'Ref:00878
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/13/2021', N'4/13/2021', NULL, N'7/13/2021', 1565, N'405-8243', N'Ref:00876
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/13/2021', N'4/13/2021', NULL, N'7/13/2021', 1566, N'405-8243', N'Ref:00800
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/13/2021', N'4/13/2021', NULL, N'7/13/2021', 1567, N'405-8243', N'Ref:00878
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/28/21', N'3/28/21', NULL, N'6/29/21', 1568, N'405-8243', N'Ref:00800
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/28/21', N'3/28/21', NULL, N'3/29/21', 1569, N'405-8243', N'Ref:00876
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'3/28/21', N'3/28/21', NULL, N'5/28/21', 1570, N'405-8243', N'Ref:00878
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/14/2021', N'4/14/2021', NULL, N'7/14/2021', 1571, N'405-8243', N'Ref:01004
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/14/2021', N'4/14/2021', NULL, N'7/14/2021', 1572, N'405-8243', N'Ref:01006
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'4/14/2021', N'4/14/2021', NULL, N'7/14/2021', 1573, N'405-8243', N'Ref:01005
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/2021', N'5/4/2021', NULL, N'8/4/2021', 1574, N'405-8243', N'Ref:01048
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/2021', N'5/4/2021', NULL, N'8/4/2021', 1575, N'405-8243', N'Ref:01030
Rock Hill DC - East Coast,WEST MARINE #00860, 860 Marine Drive, Rock Hill SC 29730, USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'WEST MARINE Pro', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/2021', N'5/4/2021', NULL, N'8/4/2021', 1576, N'405-8243', N'Ref:00970
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
GO
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/2021', N'5/4/2021', NULL, N'8/4/2021', 1577, N'405-8243', N'Ref:01014
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/2021', N'5/4/2021', NULL, N'8/4/2021', 1578, N'405-8243', N'Ref:01046
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/2021', N'5/4/2021', NULL, N'8/4/2021', 1579, N'405-8243', N'Ref:01047
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/2021', N'5/4/2021', NULL, N'8/4/2021', 1580, N'405-8243', N'Ref:00846
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/2021', N'5/4/2021', NULL, N'8/4/2021', 1581, N'405-8243', N'Ref:01045
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/2021', N'5/4/2021', NULL, N'8/4/2021', 1582, N'405-8243', N'Ref:00991
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/4/21', N'5/4/21', NULL, N'7/30/21', 1583, N'405-8243', N'Ref:00991
1025 PARKWAY INDUSTRIAL PARK DRIVE BUFORD DA 30518 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'GILL NORTH AMERICA INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/10/2021', N'5/10/2021', NULL, N'8/10/2021', 1584, N'405-8243', N'Ref:01118
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/10/2021', N'5/10/2021', NULL, N'8/10/2021', 1585, N'405-8243', N'Ref:00969
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/10/2021', N'5/10/2021', NULL, N'8/10/2021', 1586, N'405-8243', N'Ref:01117
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/10/2021', N'5/10/2021', NULL, N'8/10/2021', 1587, N'405-8243', N'Ref:01119
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/10/2021', N'5/10/2021', NULL, N'8/10/2021', 1588, N'405-8243', N'Ref:01120
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/10/21', N'5/10/21', NULL, N'8/11/21', 1589, N'405-8243', N'
			Ref:01119
			P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
			,US
		', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/10/21', N'5/10/21', NULL, N'8/11/21', 1590, N'405-8243', N'
			Ref:00969
			P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
			,US
		', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/10/21', N'5/10/21', NULL, N'8/11/21', 1591, N'405-8243', N'
			Ref:01118
			P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
			,US
		', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/10/21', N'5/11/21', NULL, N'8/11/21', 1592, N'405-8243', N'REF:01117
			P.O. BOX 741503, ATLANTA, GA 30374-1503, U.S.A.
			,US', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'I REDO LIC. CUZ SYSTEM BLOCK ME', N'International Paint LLC', NULL, N'JOSEPH', NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/12/2021', N'5/12/2021', NULL, N'8/12/2021', 1593, N'405-8243', N'Ref:01136
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/19/2021', N'5/19/2021', NULL, N'8/19/2021', 1594, N'405-8243', N'Ref:01120
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1595, N'405-8243', N'Ref:8251304038
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1596, N'405-8243', N'Ref:8251304309
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1597, N'405-8243', N'Ref:8251302984
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1598, N'405-8243', N'Ref:8251302985
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1599, N'405-8243', N'Ref:8251304038
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1600, N'405-8243', N'Ref:8251304038
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1601, N'405-8243', N'Ref:8251304309
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1602, N'405-8243', N'Ref:8251302984
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1603, N'405-8243', N'Ref:8251302985
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1604, N'405-8243', N'Ref:8251304038
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1605, N'405-8243', N'Ref:8251304309
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1606, N'405-8243', N'Ref:8251302984
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/2021', N'5/24/2021', NULL, N'8/24/2021', 1607, N'405-8243', N'Ref:8251302985
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/1/2021', N'6/1/2021', NULL, N'9/1/2021', 1608, N'405-8243', N'Ref:01013
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/1/2021', N'6/1/2021', NULL, N'9/1/2021', 1609, N'405-8243', N'Ref:01028
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/1/2021', N'6/1/2021', NULL, N'9/1/2021', 1610, N'405-8243', N'Ref:01173
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/1/2021', N'6/1/2021', NULL, N'9/1/2021', 1611, N'405-8243', N'Ref:01174
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/21', N'5/24/21', NULL, N'8/25/21', 1612, N'405-8243', N'Ref:8251304309
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/21', N'5/24/21', NULL, N'8/25/21', 1613, N'405-8243', N'Ref:8251304038
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/21', N'5/24/21', NULL, N'8/25/21', 1614, N'405-8243', N'Ref:8251302984
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'5/24/21', N'5/24/21', NULL, N'8/25/21', 1615, N'405-8243', N'Ref:8251302985
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'BROKER CAN ONLY APPLY IN LITERS', N'International Paint LLC', NULL, N'JOSEPH', NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/1/2021', N'6/1/2021', NULL, N'9/1/2021', 1616, N'405-8243', N'Ref:9417028
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/1/2021', N'6/1/2021', NULL, N'9/1/2021', 1617, N'405-8243', N'Ref:9417028
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/1/2021', N'6/1/2021', NULL, N'9/1/2021', 1618, N'405-8243', N'Ref:9417028
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/1/2021', N'6/1/2021', NULL, N'9/1/2021', 1619, N'405-8243', N'Ref:9417028
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'6/1/2021', N'6/1/2021', NULL, N'9/1/2021', 1620, N'405-8243', N'Ref:00995
P.O. Box 20595 Portland, OR 97294-0595
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Leatherman Tool Group Inc', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/6/2021', N'7/6/2021', NULL, N'10/6/2021', 2540, N'405-8243', N'Ref:01269
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/7/2021', N'7/7/2021', NULL, N'10/7/2021', 2541, N'405-8243', N'Ref:01269
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/7/2021', N'7/7/2021', NULL, N'10/7/2021', 2542, N'405-8243', N'Ref:01264
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/7/2021', N'7/7/2021', NULL, N'10/7/2021', 2543, N'405-8243', N'Ref:01261
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/7/2021', N'7/7/2021', NULL, N'10/7/2021', 2544, N'405-8243', N'Ref:01262
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/7/2021', N'7/7/2021', NULL, N'10/7/2021', 2545, N'405-8243', N'Ref:01263
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/7/2021', N'7/7/2021', NULL, N'10/7/2021', 2546, N'405-8243', N'Ref:01267
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/7/2021', N'7/7/2021', NULL, N'10/7/2021', 2547, N'405-8243', N'Ref:01260
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/7/2021', N'7/7/2021', NULL, N'10/7/2021', 2548, N'405-8243', N'Ref:01265
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2549, N'405-8243', N'Ref:01338
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2550, N'405-8243', N'Ref:01110
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2551, N'405-8243', N'Ref:01336
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2552, N'405-8243', N'Ref:01337
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2553, N'405-8243', N'Ref:01340
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2554, N'405-8243', N'Ref:01335
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2555, N'405-8243', N'Ref:01341
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2556, N'405-8243', N'Ref:01334
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2557, N'405-8243', N'Ref:01338
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2558, N'405-8243', N'Ref:01110
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2559, N'405-8243', N'Ref:01336
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2560, N'405-8243', N'Ref:01337
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2561, N'405-8243', N'Ref:01340
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2562, N'405-8243', N'Ref:01335
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2563, N'405-8243', N'Ref:01341
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/2021', N'7/27/2021', NULL, N'10/27/2021', 2564, N'405-8243', N'Ref:01334
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/21', N'7/27/21', NULL, N'10/31/21', 2565, N'405-8243', N'Ref:01110
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/21', N'7/27/21', NULL, N'10/31/21', 2566, N'405-8243', N'Ref:01336
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/21', N'7/27/21', NULL, N'10/31/21', 2567, N'405-8243', N'Ref:01334
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/21', N'7/27/21', NULL, N'10/31/21', 2568, N'405-8243', N'Ref:01338
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'QTY IS IN LITRES ROND UP TO NXT NBR', N'International Paint LLC', NULL, N'JOSEPH', NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/21', N'7/27/21', NULL, N'10/31/21', 2569, N'405-8243', N'Ref:01340
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'QTY IS IN LITRES ROND UP TO NXT NBR', N'International Paint LLC', NULL, N'JOSEPH', NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/21', N'7/27/21', NULL, N'10/31/21', 2570, N'405-8243', N'Ref:01335
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'QTY IS IN LITRES ROND UP TO NXT NBR', N'International Paint LLC', NULL, N'JOSEPH', NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/21', N'7/27/21', NULL, N'10/31/21', 2571, N'405-8243', N'Ref:01337
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'QTY IS IN LITRES ROND UP TO NXT NBR', N'International Paint LLC', NULL, N'JOSEPH', NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/27/21', N'7/27/21', NULL, N'10/31/21', 2572, N'405-8243', N'Ref:01341
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'QTY IS IN LITRES ROND UP TO NXT NBR', N'International Paint LLC', NULL, N'JOSEPH', NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/2021', N'7/30/2021', NULL, N'10/30/2021', 2573, N'405-8243', N'Ref:01339
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'7/30/2021', N'7/30/2021', NULL, N'10/30/2021', 2574, N'405-8243', N'Ref:01339
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/12/2021', N'8/12/2021', NULL, N'11/12/2021', 2575, N'405-8243', N'Ref:01187
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/12/21', N'8/13/21', NULL, N'10/29/21', 2576, N'405-8243', N'Ref:01187
Sluisweg 12, Fijnaart, Holland
,NL
', N'NL', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'Chugoku Paints B.V.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/16/2021', N'8/16/2021', NULL, N'11/16/2021', 2577, N'405-8243', N'Ref:01400
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/16/2021', N'8/16/2021', NULL, N'11/16/2021', 2578, N'405-8243', N'Ref:01413
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/20/2021', N'8/20/2021', NULL, N'11/20/2021', 2579, N'405-8243', N'Ref:01409
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'8/20/2021', N'8/20/2021', NULL, N'11/20/2021', 2580, N'405-8243', N'Ref:01410
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/8/2021', N'9/8/2021', NULL, N'12/8/2021', 2581, N'405-8243', N'Ref:113-6677331-8022618
USA
,US
', N'US', N'261495', NULL, NULL, NULL, N'Joseph', N'AMAZON.COM', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/8/2021', N'9/8/2021', NULL, N'12/8/2021', 2582, N'405-8243', N'Ref:113-6677331-8022618
USA
,US
', N'US', N'261495', NULL, NULL, NULL, N'Joseph', N'AMAZON.COM', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/8/2021', N'9/8/2021', NULL, N'12/8/2021', 2583, N'405-8243', N'Ref:113-6677331-8022618
USA
,US
', N'US', N'261495', NULL, NULL, NULL, N'Joseph', N'AMAZON.COM', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/2021', N'9/9/2021', NULL, N'12/9/2021', 2584, N'405-8243', N'Ref:113-6677331-8022618
USA
,US
', N'US', N'261495', NULL, NULL, NULL, N'Joseph', N'AMAZON.COM', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/2021', N'9/9/2021', NULL, N'12/9/2021', 2585, N'405-8243', N'Ref:113-6677331-8022618
USA
,US
', N'US', N'261495', NULL, NULL, NULL, N'Joseph', N'AMAZON.COM', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/2021', N'9/9/2021', NULL, N'12/9/2021', 2586, N'405-8243', N'Ref:113-6677331-8022618
USA
,US
', N'US', N'261495', NULL, NULL, NULL, N'Joseph', N'AMAZON.COM', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/2021', N'9/9/2021', NULL, N'12/9/2021', 2587, N'405-8243', N'Ref:01380
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/2021', N'9/9/2021', NULL, N'12/9/2021', 2588, N'405-8243', N'Ref:01402
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/2021', N'9/9/2021', NULL, N'12/9/2021', 2589, N'405-8243', N'Ref:8251320673
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/10/2021', N'9/10/2021', NULL, N'12/10/2021', 2590, N'405-8243', N'Ref:01460
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/21', N'9/9/21', NULL, N'11/26/21', 2591, N'405-8243', N'Ref:01402
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/21', N'9/9/21', NULL, N'11/26/21', 2592, N'405-8243', N'Ref:8251320673
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/9/21', N'9/9/21', NULL, N'11/26/21', 2593, N'405-8243', N'Ref:01380
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2594, N'405-8243', N'Ref:01171
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2595, N'405-8243', N'Ref:01280
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
GO
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2596, N'405-8243', N'Ref:01499
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2597, N'405-8243', N'Ref:01245
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2598, N'405-8243', N'Ref:01354
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2599, N'405-8243', N'Ref:239596
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2600, N'405-8243', N'Ref:01383
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2601, N'405-8243', N'Ref:239594
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2602, N'405-8243', N'Ref:01354
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2603, N'405-8243', N'Ref:239596
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2604, N'405-8243', N'Ref:01383
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2605, N'405-8243', N'Ref:01499
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/13/2021', N'9/13/2021', NULL, N'12/13/2021', 2606, N'405-8243', N'Ref:01245
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2607, N'405-8243', N'Ref:01540
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2608, N'405-8243', N'Ref:01543
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2609, N'405-8243', N'Ref:01541
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2610, N'405-8243', N'Ref:01542
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2611, N'405-8243', N'Ref:01540
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2612, N'405-8243', N'Ref:01543
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2613, N'405-8243', N'Ref:01541
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2614, N'405-8243', N'Ref:01542
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2615, N'405-8243', N'Ref:01540
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2616, N'405-8243', N'Ref:01543
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2617, N'405-8243', N'Ref:01541
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2618, N'405-8243', N'Ref:01542
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2619, N'405-8243', N'Ref:01540
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2620, N'405-8243', N'Ref:01540
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'9/30/2021', N'9/30/2021', NULL, N'12/30/2021', 2621, N'405-8243', N'Ref:01540
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/1/2021', N'10/1/2021', NULL, N'1/1/2022', 2622, N'405-8243', N'Ref:01540
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/4/2021', N'10/4/2021', NULL, N'1/4/2022', 2623, N'405-8243', N'Ref:01559
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/4/2021', N'10/4/2021', NULL, N'1/4/2022', 2624, N'405-8243', N'Ref:01561
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/4/2021', N'10/4/2021', NULL, N'1/4/2022', 2625, N'405-8243', N'Ref:01456
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/4/2021', N'10/4/2021', NULL, N'1/4/2022', 2626, N'405-8243', N'Ref:01503
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/4/2021', N'10/4/2021', NULL, N'1/4/2022', 2627, N'405-8243', N'Ref:01560
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/4/21', N'10/4/21', NULL, N'1/4/22', 2628, N'405-8243', N'Ref:01561
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/4/2021', N'10/4/2021', NULL, N'1/4/2022', 2629, N'405-8243', N'Ref:01559
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/4/2021', N'10/4/2021', NULL, N'1/4/2022', 2630, N'405-8243', N'Ref:01559
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/4/2021', N'10/4/2021', NULL, N'1/4/2022', 2631, N'405-8243', N'Ref:01559
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/21/2021', N'10/21/2021', NULL, N'1/21/2022', 2632, N'405-8243', N'Ref:01417
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/21/2021', N'10/21/2021', NULL, N'1/21/2022', 2633, N'405-8243', N'Ref:01529
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/21/2021', N'10/21/2021', NULL, N'1/21/2022', 2634, N'405-8243', N'Ref:01521
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/21/2021', N'10/21/2021', NULL, N'1/21/2022', 2635, N'405-8243', N'Ref:01538
25B WATERFRONT ROAD COLE BAY ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE ST MAARTEN', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/21/2021', N'10/21/2021', NULL, N'1/21/2022', 2636, N'405-8243', N'Ref:01417
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/21/2021', N'10/21/2021', NULL, N'1/21/2022', 2637, N'405-8243', N'Ref:01529
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/21/2021', N'10/21/2021', NULL, N'1/21/2022', 2638, N'405-8243', N'Ref:01521
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'10/21/2021', N'10/21/2021', NULL, N'1/21/2022', 2639, N'405-8243', N'Ref:01538
25B WATERFRONT ROAD COLE BAY ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE ST MAARTEN', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2021', N'11/3/2021', NULL, N'2/3/2022', 2640, N'405-8243', N'Ref:01394
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2021', N'11/3/2021', NULL, N'2/3/2022', 2641, N'405-8243', N'Ref:01516
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2021', N'11/3/2021', NULL, N'2/3/2022', 2642, N'405-8243', N'Ref:01494
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2021', N'11/3/2021', NULL, N'2/3/2022', 2643, N'405-8243', N'Ref:01682
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2021', N'11/3/2021', NULL, N'2/3/2022', 2644, N'405-8243', N'Ref:01394
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2021', N'11/3/2021', NULL, N'2/3/2022', 2645, N'405-8243', N'Ref:01455
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2021', N'11/3/2021', NULL, N'2/3/2022', 2646, N'405-8243', N'Ref:01516
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2021', N'11/3/2021', NULL, N'2/3/2022', 2647, N'405-8243', N'Ref:01494
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/3/2021', N'11/3/2021', NULL, N'2/3/2022', 2648, N'405-8243', N'Ref:01494
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/15/2021', N'11/15/2021', NULL, N'2/15/2022', 2649, N'405-8243', N'Ref:01641
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/15/2021', N'11/15/2021', NULL, N'2/15/2022', 2650, N'405-8243', N'Ref:01641
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/2021', N'11/23/2021', NULL, N'2/23/2022', 2651, N'405-8243', N'Ref:01782
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/2021', N'11/23/2021', NULL, N'2/23/2022', 2652, N'405-8243', N'Ref:01605
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/2021', N'11/23/2021', NULL, N'2/23/2022', 2653, N'405-8243', N'Ref:01395
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/2021', N'11/23/2021', NULL, N'2/23/2022', 2654, N'405-8243', N'Ref:01323
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/2021', N'11/23/2021', NULL, N'2/23/2022', 2655, N'405-8243', N'Ref:01327
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/2021', N'11/23/2021', NULL, N'2/23/2022', 2656, N'405-8243', N'Ref:01782
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/21', N'11/23/21', NULL, N'1/28/22', 2657, N'405-8243', N'Ref:01327
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/21', N'11/23/21', NULL, N'1/28/22', 2658, N'405-8243', N'Ref:01395
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/21', N'11/23/21', NULL, N'1/28/22', 2659, N'405-8243', N'Ref:01605
3131 N. ANDREWS AVENUE EXT., POMPANO BEACH, FL 33064
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'LAND ''N'' SEA DISTRIBUTING, INC.', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/21', N'11/23/21', NULL, N'1/28/22', 2660, N'405-8243', N'Ref:01323
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'11/23/21', N'11/23/21', NULL, N'1/28/22', 2661, N'405-8243', N'Ref:01782
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2662, N'405-8243', N'Ref:01806
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2663, N'405-8243', N'Ref:01812
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2664, N'405-8243', N'Ref:01814
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2665, N'405-8243', N'Ref:01804
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2666, N'405-8243', N'Ref:01802
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2667, N'405-8243', N'Ref:01801
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2668, N'405-8243', N'Ref:01803
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2669, N'405-8243', N'Ref:01805
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2670, N'405-8243', N'Ref:01807
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2671, N'405-8243', N'Ref:01808
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2672, N'405-8243', N'Ref:01811
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2673, N'405-8243', N'Ref:01809
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2674, N'405-8243', N'Ref:01813
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2675, N'405-8243', N'Ref:01806
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2676, N'405-8243', N'Ref:01806
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/6/2021', N'12/6/2021', NULL, N'3/6/2022', 2677, N'405-8243', N'Ref:01806
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/8/2021', N'12/8/2021', NULL, N'3/8/2022', 2678, N'405-8243', N'Ref:01576
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/8/2021', N'12/8/2021', NULL, N'3/8/2022', 2679, N'405-8243', N'Ref:01828
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/8/2021', N'12/8/2021', NULL, N'3/8/2022', 2680, N'405-8243', N'Ref:01836
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/8/2021', N'12/8/2021', NULL, N'3/8/2022', 2681, N'405-8243', N'Ref:01830
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/8/2021', N'12/8/2021', NULL, N'3/8/2022', 2682, N'405-8243', N'Ref:01576
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/16/2021', N'12/16/2021', NULL, N'3/16/2022', 2683, N'405-8243', N'Ref:112-6522846-0099468
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/16/2021', N'12/16/2021', NULL, N'3/16/2022', 2684, N'405-8243', N'Ref:112-6522846-0099468
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/16/2021', N'12/16/2021', NULL, N'3/16/2022', 2685, N'405-8243', N'Ref:112-6522846-0099468
USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'12/16/21', N'12/16/21', NULL, N'3/17/22', 2686, N'405-8243', N'Ref:112-6522846-0099468
USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'Amazon.com', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2687, N'405-8243', N'Ref:01943
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2688, N'405-8243', N'Ref:01938
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2689, N'405-8243', N'Ref:01945
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2690, N'405-8243', N'Ref:01604
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2691, N'405-8243', N'Ref:01939
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2692, N'405-8243', N'Ref:01944
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2693, N'405-8243', N'Ref:01941
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2694, N'405-8243', N'Ref:01940
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2695, N'405-8243', N'Ref:01936
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
GO
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/2022', N'1/7/2022', NULL, N'4/7/2022', 2696, N'405-8243', N'Ref:01943
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/22', N'1/7/22', NULL, N'3/25/22', 2697, N'405-8243', N'Ref:01940
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/22', N'1/7/22', NULL, N'3/25/22', 2698, N'405-8243', N'Ref:01945
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/22', N'1/7/22', NULL, N'3/25/22', 2699, N'405-8243', N'Ref:01604
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/22', N'1/7/22', NULL, N'3/25/22', 2700, N'405-8243', N'Ref:01938
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/22', N'1/7/22', NULL, N'3/25/22', 2701, N'405-8243', N'Ref:01936
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/7/22', N'1/7/22', NULL, N'3/25/22', 2702, N'405-8243', N'Ref:01939
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/12/2022', N'1/12/2022', NULL, N'4/12/2022', 2703, N'405-8243', N'Ref:01943
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/12/2022', N'1/12/2022', NULL, N'4/12/2022', 2704, N'405-8243', N'Ref:01944
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2705, N'405-8243', N'Ref:158057
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2706, N'405-8243', N'Ref:158090
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2707, N'405-8243', N'Ref:01877
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2708, N'405-8243', N'Ref:01907
25B WATERFRONT ROAD COLE BAY ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'BUDGET MARINE ST MAARTEN', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/22', N'1/17/22', NULL, N'3/25/22', 2709, N'405-8243', N'Ref:158090
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/22', N'1/17/22', NULL, N'3/25/22', 2710, N'405-8243', N'Ref:01877
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/22', N'1/17/22', NULL, N'3/25/22', 2711, N'405-8243', N'Ref:01907
25B WATERFRONT ROAD COLE BAY ST MAARTEN
,QN
', N'QN', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'BUDGET MARINE ST MAARTEN', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/22', N'1/17/22', NULL, N'3/25/22', 2712, N'405-8243', N'Ref:158057
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2713, N'405-8243', N'Ref:01915
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2714, N'405-8243', N'Ref:01915
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2715, N'405-8243', N'Ref:01915
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2716, N'405-8243', N'Ref:01915
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2717, N'405-8243', N'Ref:01915
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/2022', N'1/17/2022', NULL, N'4/17/2022', 2718, N'405-8243', N'Ref:01915
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/17/22', N'1/17/22', NULL, N'3/25/22', 2719, N'405-8243', N'Ref:01915
14806 49TH STREET NORTH CLEARWATER, FLORIDA 33762 USA
,US
', N'US', N'07290940003049', NULL, NULL, N'BUDGET MARINE (GRENADA) LIMITED', N'Joseph', N'NEW NAUTICAL COATINGS, INC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/2022', N'1/26/2022', NULL, N'4/26/2022', 2720, N'405-8243', N'Ref:02019
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
INSERT [dbo].[xLIC_General_segment] ([Arrival_date], [Application_date], [Expiry_date], [Importation_date], [General_segment_Id], [Importer_cellphone], [Exporter_address], [Exporter_country_code], [Importer_code], [Owner_code], [Exporter_email], [Importer_name], [Importer_contact], [Exporter_name], [Exporter_telephone], [Importer_telephone], [Exporter_country_name], [Exporter_cellphone], [Importer_email]) VALUES (N'1/26/2022', N'1/26/2022', NULL, N'4/26/2022', 2721, N'405-8243', N'Ref:02019
P.O. Box 741503, Atlanta, GA 30374-1503, U.S.A.
,US
', N'US', N'07290940003049', NULL, NULL, NULL, N'Joseph', N'International Paint LLC', NULL, NULL, NULL, NULL, N'joseph@auto-brokerage.com')
GO
ALTER TABLE [dbo].[xLIC_General_segment]  WITH CHECK ADD  CONSTRAINT [FK_xLIC_General_segment_xLIC_License] FOREIGN KEY([General_segment_Id])
REFERENCES [dbo].[xLIC_License] ([LicenseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xLIC_General_segment] CHECK CONSTRAINT [FK_xLIC_General_segment_xLIC_License]
GO
