USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InventoryItems]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryItems](
	[ItemNumber] [nvarchar](20) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[Category] [nvarchar](60) NULL,
	[TariffCode] [nvarchar](50) NULL,
	[EntryTimeStamp] [datetime2](7) NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UpgradeKey] [nvarchar](20) NULL,
 CONSTRAINT [PK_InventoryItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[InventoryItems] ON 

INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'MESAILUP 16 INCH LED', 3, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', NULL, N'94054000', NULL, 69107, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'MESAILUP16INCHLEDLIG', 3, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', NULL, N'94054000', NULL, 69108, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'MESAILUP16InchLEDLig', 3, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', NULL, NULL, NULL, 69109, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'MESAILUP 16 INCH LED', 3, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', NULL, N'94054000', NULL, 69110, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'MESAILUP 16 INCH LED', 3, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', NULL, N'94054000', NULL, 69111, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'FRUIT OF THE LOOM WO', 3, N'Fruit of the Loom Women''s 2-Pack T-Shirt Bra, Black/Heather Grey, 42D', NULL, N'62121000', NULL, 69112, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'2024 NEW MAGIC EYEBR', 3, N'2024 New Magic Eyebrow Pencil, Upgraded 3D Waterproof Microblading Eyebrow Pencil Contouring Pen 4 Tipped Precise Brow Pen, Long-Lasting Eyebrow Brush Microblade Eyebrow Pencil (Black)', NULL, N'33079000', NULL, 69113, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'PLAYTEX WOMENS 18 HO', 3, N'PLAYTEX Womens 18 Hour Wireless Seamiess Fuil-coverage T-shirt With Smoothing Support, Us4159 Bras, Real Black, 40DD US', NULL, N'62121000', NULL, 69114, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'POPSICLE HOLDER FOR ', 3, N'Popsicle Holder for Kids, 6 Pieces Ice Pop Neoprene Insulator Sleeves, Freeze Pops Neoprene Sleeves, Cute Ice Sleeves Holder Bag, Reusable Washable Ice Popsicle Sleeves for Boys Girls. (Multicolor-1)', NULL, N'39269000', NULL, 69115, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'BMCITYBM BABY HIGH T', 3, N'BMCITYBM Baby High Top Sneakers Boy Girl Walking Shoes Infant First Walker Shoes for 6 9 12 18 24 Months Black Size 5 Toddler', NULL, N'64039100', NULL, 69116, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'BABY CARRIER NEWBORN', 3, N'Baby Carrier Newborn to Toddler - TSRETE Baby Ergonomic and Cozy Infant Carrier with Lumbar Support for 7-25ibs,Easy Adjustable Baby Chest Carrier, Face-in and Face-Out Positions Baby Sling Carrier', NULL, N'63079000', NULL, 69117, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'4PACK DRIP-FREE SILI', 3, N'4Pack Drip-Free Silicone Popsicle Holders, AODISTUCE Popsicle Holder with Straw | with 2 Slots Fit Standard & Wide sized sticks Drip Free Popsicle Stick Holder Popsicle Drip Catcher', NULL, N'39249000', NULL, 69118, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'ELEMENT MEN''S TOPAZ ', 3, N'Element Men''s Topaz C3 Skate Shoe, Grey, 12', NULL, N'64039100', NULL, 69119, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'AISSWZBER FASHION VI', 3, N'aisswzber Fashion Vintage Classic Semi-Rimless Half Frame Clear Lens Glasses 3016-gold&silver', NULL, N'90041000', NULL, 69120, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'FLIPALM FOR SAMSUNG ', 3, N'FLIPALM for Samsung Galaxy S24 Ultra Wallet Case with RFID Blocking Credit Card Holder, PU Leather Folio Flip Kickstand Protective Shockproof Cover Women Men for Samsung S24UItra Phone case(Black)', NULL, N'42023200', NULL, 69121, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'AMFILM AUTO-ALIGNMEN', 3, N'amFilm Auto-Alignment OneTouch for Samsung Galaxy S24 Ultra 6.8" Screen Protector + Camera Lens Protector, Tempered Glass, 30 seconds Installation, Bubble Free, Case Friendly, Anti-Scratch [2+2 Pack]', NULL, N'70139900', NULL, 69122, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'FLIPALM FOR SAMSUNG ', 3, N'FLIPALM for Samsung Galaxy S24 Ultra Wallet Case with RFID Blocking Credit Card Holder, PU Leather Folio Flip Kickstand Protective Shockproof Cover Women Men for S. 1g S24Ultra Phone case(Blue)', NULL, N'42023200', NULL, 69123, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'ZOYAVA DYNAMIC VITAL', 3, N'Zoyava Dynamic Vitality Bundle, Sea Moss and Shilajit Bundle, Sea Moss 7000mg, Black Seed Oil 4000mg, Ashwagandha 2000mg, Ginger & Shilajit 9000mg, Rhodiola Rosea 1000mg, Panax Ginseng (60 Count)', NULL, N'21069000', NULL, 69124, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'CINVIK MEN''S FUNNY B', 3, N'CINVIK Men''s Funny Boxers 100% Cotton Soft Short Inseam Pickle Print Loose Fit Colorful Boxer Shorts 3 Pack L', NULL, N'61071100', NULL, 69125, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'LASFIT H4 9003 BULBS', 3, N'LASFIT H4 9003 Bulbs, HB2/H19/H1S/P43T High/Low Lights, 400% Super Bright 60W 6O000LM 6000K White, 360 Adjustable Beam, Plug&Play (2 bulbs)', NULL, N'85392100', NULL, 69126, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'O-CEDAR SYSTEM EASY ', 3, N'O-Cedar System Easy Wring Spin Mop & Bucket with 3 Extra Refills with Citrus Pac (Variety Pack)', NULL, N'73239300', NULL, 69127, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'O-CEDAR SYSTEM EASY ', 3, N'O-Cedar System Easy Wring Spin Mop & Bucket with 3 Extra Refills with Citrus Pac (Variety Pack)', NULL, N'73239300', NULL, 69128, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'MOSISO LAPTOP SHOULD', 3, N'MOSISO Laptop Shoulder Bag Compatible with MacBook Air 15 inch M2 A2941 2023/Pro 16 inch 2023-2019 M3 A2991 M2 A2780 M1 A2485 A2141, 15-15.6 inch Notebook, Leopard Grain Briefcase Sleeve with Belt', NULL, N'42021200', NULL, 69129, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'DORMAN 56768 DRIVER ', 3, N'Dorman 56768 Driver Side Door Mirror Glass Compatible with Select Mitsubishi Models', NULL, N'70091000', NULL, 69130, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'ZIAYI 6.5” ORIGINAL ', 3, N'ZiaYi 6.5” Original for Samsung Galaxy A03 Display Touch Screen A03 SM-A035F SM-A035F/DS SM- A035M SM-A035G LCD Screen Replacement', NULL, N'85312000', NULL, 69131, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'SKAR AUDIO SDR-12 D2', 3, N'Skar Audio SDR-12 D2 12" 1200 Watt Max Power Dual 2 Ohm Car Subwoofer', NULL, N'85182900', NULL, 69132, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'FLYFE GRIP STRENGTH ', 3, N'FLYFE Grip Strength Trainer, Plastic, 2 Pack 11-132 Ibs, Forearm Strengthener, Hand Squeezer Adjustable Resistance, Hand Grip Strengthener for Muscle Building and Injury Recovery (Black)', NULL, N'95069100', NULL, 69133, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'RESISTANCE BANDS, HE', 3, N'Resistance Bands, Heavy Exercise Bands with Handles, Fitness Bands for Working Out, Workout Bands for Men, Weight Bands Set for Muscle Training, Strength, Slim, Yoga, Home Gym Equipment', NULL, N'95069100', NULL, 69134, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'SOCIAL STUDIES CURRI', 3, N'Social Studies Curriculum And Methods for the Caribbean, Griffith, Anthony D.', NULL, N'49019900', NULL, 69135, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'SOCIAL STUDIES CURRI', 3, N'Social Studies Curriculum And Methods for the Caribbean, Griffith, Anthony D', NULL, N'49019900', NULL, 69136, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'4 PACK GAS RANGE STO', 3, N'4 Pack Gas Range Stove Ignitor Electrode 8523793 Replacement for Whirlpool Gas Range IgniterAmana oven ignitor and Kenmore gas stove parts WP8523793 AP6012852 PS11746068 74007473 7432M126-60', NULL, N'85169000', NULL, 69137, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'ZIAYI 6.5" ORIGINAL ', 3, N'ZiaYi 6.5" Original for Samsung Galaxy A03 Display Touch Screen A03 SM-A035F SM-A035F/DS SM- A035M SM-A035G LCD Screen Replacement', NULL, N'85177000', NULL, 69138, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'A-PREMIUM IRIDIUM PL', 3, N'A-Premium iridium Platinum Spark Plugs Compatible with Subaru Outback, Forester, impreza, Legacy & Mercedes-Benz C230 & Mitsubishi Outlander, Lancer, Eclipse & Hyundai Genesis Coupe, Pack of 4', NULL, N'85111000', NULL, 69139, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'FOX RUN RECTANGULAR ', 3, N'Fox Run Rectangular Cooling Rack, Iron/Chrome, 10-Inch x 14-Inch', NULL, N'73239300', NULL, 69140, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'WULANKD UNIVERSAL AD', 3, N'Wulankd Universal Adjustable Oven Cooker Shelf Rack, Telescopic Oven Cooker Shelf Rack, Extendable Oven Rack, Adjusting Range About 14 Inches - 26 Inches Wide', NULL, N'73239300', NULL, 69141, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'BERLUNE 5 X 1000 FT ', 3, N'Berlune 5 x 1000 ft Trellis Netting, Soft Mesh Nylon Trellis Netting Bulk Roll, Heavy Duty Garden Trellis Netting for Climbing Plants, Grape Racks, Growing Support Trellising Bean, Pea, Tomatoes', NULL, N'56089000', NULL, 69142, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'KLEIN TOOLS 32950 RA', 3, N'Klein Tools 32950 Ratcheting Impact Rated Hollow Power Nut Driver Set with Handle, Magnetic, Color Coded, 6 SAE Hex Sizes', NULL, N'82042000', NULL, 69143, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'ADIDAS ORIGINALS ORI', 3, N'adidas Originals Originals Trefoil Pocket Backpack, Black, One Size', NULL, N'42021200', NULL, 69144, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'HANES BOYS’ TANK UND', 3, N'Hanes Boys’ Tank Undershirt, EcoSmart Cotton Shirt, Multiple Packs Available, White-10, Medium', NULL, N'61091000', NULL, 69145, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'PUMA MEN''S TAZON 6 F', 3, N'PUMA Men''s TAZON 6 FRACTURE FM Cross Training Sneaker, Purna Black, 8', NULL, N'64039100', NULL, 69146, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'2023 ALL-NEW 10IN TA', 3, N'2023 All-New 10in Tablet Case for Kids(2021/2023 Release 11/13th Generation), DICEKOO ipad Light Weight Anti Slip Shockproof Kids Friendly Case for 10 inch Table - Black', NULL, N'42023200', NULL, 69147, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'AMAZON FIRE HD 10 TA', 3, N'Amazon Fire HD 10 tablet, built for relaxation, 10.1" vibrant Full HD screen, octa-core processor, 3 GB RAM, latest model (2023 release), 32 GB, Ocean', NULL, N'84713000', NULL, 69148, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'AMAZON FIRE 7 KIDS T', 3, N'Amazon Fire 7 Kids tablet, ages 3-7. Top-selling 7° kids tablet on Amazon - 2022 | ad-free content with parental controls included, 10-hr battery, 76 GB, Red', NULL, N'84713000', NULL, 69149, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'AMAZON FIRE HD 8 KID', 3, N'Amazon Fire HD 8 Kids Pro tablet- 2022, ages 6-12 | 8° HD screen, slim case for older kids, ad-free content, parental controls, 13-hr battery, 32 GB, Hello Teal', NULL, N'84713000', NULL, 69150, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'ADIDAS ORIGINALS MEN', 3, N'adidas Originals Men''s Stan Smith (End Plastic Waste) Sneaker, Black/Black/White, 8.5', NULL, N'64039100', NULL, 69151, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'HUGGIES SIZE 1 DIAPE', 3, N'Huggies Size 1 Diapers, Snug & Dry Newborn Diapers, Size 1 (8-14 Ibs), 256 Count (4 Packs of 64), Packaging May Vary', NULL, N'96190012', NULL, 69152, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'ONESIES BRAND UNISEX', 3, N'Onesies Brand Unisex Baby 12-Piece Cap and Mitten Set, stripe, 0-6 Months', NULL, N'61112000', NULL, 69153, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'MUOCOBU ELECTRIC BRE', 3, N'MUOCOBU Electric Breast Pump, Breast Pump Electric Breastfeeding Pump 3 Modes 10 Levels Dual Rechargeable Nursing - Double Breast Milk Pump Massage with Touchscreen LED', NULL, N'90189000', NULL, 69154, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'SIMPLE JOYS BABY BOY', 3, N'Simple Joys Baby Boy''s 6 piece Little Character sets Sleepwear, Green/Grey Rhino, 3-6 Months', NULL, N'61112000', NULL, 69155, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'SIMPLE JOYS BY CARTE', 3, N'Simple Joys by Carter''s Baby Boys'' 6-Pack Short-Sleeve Bodysuit, Multicolor/Dinosaur/Sports/Stripe, 0-3 Months', NULL, N'61112000', NULL, 69156, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'ONESIES BRAND UNISEX', 3, N'Onesies Brand Unisex Baby 12-Piece Cap and Mitten Set, Sayings Board, 0-6 Months', NULL, N'61112000', NULL, 69157, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'SIMPLE JOYS BY CARTE', 3, N'Simple Joys by Carter’s Unisex Babies'' Long-Sleeve Bodysuit, Pack of 5, Grey/White, 6-9 Months', NULL, N'61112000', NULL, 69158, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'ICSAPR [4 PACK] GLAS', 3, N'iCsapr [4 Pack] Glass Screen Protector Compatible for Samsung Galaxy AO3s[9H Hardness]-HD Screen Tempered Glass, Scratch Resistant, Easy install [Case Friendly][Bubble Free] 2.5D Edge', NULL, N'70139100', NULL, 69159, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'FLIPPER ADVENTURER L', 3, N'Flipper Adventurer Logo Washed Cotton Vintage Bullet Holes Distressed Baseball Cap Dad Hat Ballcap (Medium ~ Large, Navy)', NULL, N'65059000', NULL, 69160, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'WELKINLAND 55-POCKET', 3, N'WELKINLAND 55-Pockets Tool backpack, Tool bag backpack, Backpack tool bag, Tool back pack, Electrician backpack, Tool backpack hvac, Too! backpack heavy duty', NULL, N'42021200', NULL, 69161, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'COOFANDY LINEN SHIRT', 3, N'COOFANDY Linen Shirts for Men Surnmer Hawaiian Shirt Short Sleeve Henley Shirt Collarless Linen Beach Shirt Casual Mens T- Shirt Black', NULL, N'62052000', NULL, 69162, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'HEIGBLE 3.3 IB HIGH ', 3, N'Heigble 3.3 Ib High Density Transparent Gel Candle Wax 100 Pcs Candle Wick and 100 Pcs Candle Wick Stickers for Candle Making Supplies DIY Project', NULL, N'34060010', NULL, 69163, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'HOLATO P222 CARBURET', 3, N'HOLATO P222 Carburetor Carb Air filter Throttle Handle Grips Cable Kit for 110cc 125cc XR50 CRFSO CRFSOf CRF110 TaoTao DB14 DB24 SSR110 SSR125 Coolster Apotio Pit Dirt Bike Chinese 4 Wheeler ATV Red', NULL, N'84099100', NULL, 69164, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'AUTO DYNASTY COMPATI', 3, N'Auto Dynasty Compatible with Chevy Tracker/Suzuki Vitara 2pcs Tape-On Window Visor Deflector Rain Guard', NULL, N'87082900', NULL, 69165, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'(PACK OF 2) REPLACEM', 3, N'(Pack of 2) Replacement Universal Remote for All Insignia Fire Smart TVs Toshiba Fire Smart TVs AMZ Omni TV and AMZ 4- Series Smart TVs Remote Controtf', NULL, N'85437000', NULL, 69166, NULL)
INSERT [dbo].[InventoryItems] ([ItemNumber], [ApplicationSettingsId], [Description], [Category], [TariffCode], [EntryTimeStamp], [Id], [UpgradeKey]) VALUES (N'MESAILUP 16 INCH LED', 3, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', NULL, N'94054000', NULL, 69167, NULL)
SET IDENTITY_INSERT [dbo].[InventoryItems] OFF
GO
/****** Object:  Index [SQLOPS_InventoryItems_153_152]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItems_153_152] ON [dbo].[InventoryItems]
(
	[ApplicationSettingsId] ASC
)
INCLUDE([TariffCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_InventoryItems_171_170]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItems_171_170] ON [dbo].[InventoryItems]
(
	[Description] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_InventoryItems_2_1]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItems_2_1] ON [dbo].[InventoryItems]
(
	[ApplicationSettingsId] ASC,
	[EntryTimeStamp] ASC
)
INCLUDE([TariffCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_InventoryItems_29_28]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_InventoryItems_29_28] ON [dbo].[InventoryItems]
(
	[ItemNumber] ASC,
	[ApplicationSettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[InventoryItems] ADD  CONSTRAINT [DF_InventoryItems_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[InventoryItems]  WITH CHECK ADD  CONSTRAINT [FK_InventoryItems_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
GO
ALTER TABLE [dbo].[InventoryItems] CHECK CONSTRAINT [FK_InventoryItems_ApplicationSettings]
GO
ALTER TABLE [dbo].[InventoryItems]  WITH NOCHECK ADD  CONSTRAINT [FK_InventoryItems_TariffCodes] FOREIGN KEY([TariffCode])
REFERENCES [dbo].[TariffCodes] ([TariffCode])
GO
ALTER TABLE [dbo].[InventoryItems] NOCHECK CONSTRAINT [FK_InventoryItems_TariffCodes]
GO
